from __future__ import absolute_import, division, print_function

import json
from sys import exit, stderr, stdin
import traceback

from warbend import mode
mode.is_quiet = True

from warbend.data import path as path_of, selector, transaction
from warbend.data.array import is_array
from warbend.data.enum import Enum, Flags
from warbend.data.id_ref import IdRef
from warbend.data.mutable import is_mutable
from warbend.data.record import is_record
from warbend.game.mount_and_blade.native import *
from warbend.serialization import binary, xml
from warbend.util.progress import ProgressBar


class RequestHandler(object):
    def __init__(self):
        ProgressBar.callback = self._progress
        self._game = None
        self._loop()

    def _loop(self):
        while True:
            if stdin.isatty():
                print('? ', end='', file=stderr)
            try:
                req = raw_input()
            except EOFError:
                break
            print('->', req, '\n', file=stderr)
            req = json.loads(req)
            (name, args), = req.iteritems()
            resp = getattr(self, name)(**args)
            self._send(resp)

    def _send(self, message):
        s = json.dumps(message)
        print('<-', s, '\n', file=stderr)
        print(s)

    def _progress(self, name, percent):
        if percent is True:
            self._send(name)
        elif percent is False:
            self._send('')
        else:
            self._send(percent)

    def exit(self):
        exit(0)

    @staticmethod
    def _strtofmt(s):
        if s == 'binary':
            return binary
        elif s == 'xml':
            return xml
        else:
            raise ValueError('Invalid format %r; must be %r or %r' %
                             (s, 'binary', 'xml'))

    def _walk(self, path):
        env = {'__builtins__': __builtins__, 'game': self._game}
        return eval(path, env)

    def load(self, fileName, format):
        try:
            self._game = load(fileName, self._strtofmt(format))
            return {}
        except Exception as ex:
            traceback.print_exc(file=stderr)
            return {'error': str(ex)}

    def save(self, fileName, format):
        try:
            save(self._game, fileName, self._strtofmt(format))
            return {}
        except Exception as ex:
            traceback.print_exc(file=stderr)
            return {'error': str(ex)}

    @staticmethod
    def _propvalue(value):
        t = type(value)
        if is_mutable(value):
            r = {
                'path': path_of(value),
                'name': getattr(value, '_name', None),
                'totalCount': len(value),
                'mutableCount': sum(is_mutable(child) for child in value),
            }
        else:
            r = {'value': str(value)}
            if isinstance(value, Enum):
                r['baseType'] = t.base_type.__name__
                is_flags = isinstance(value, Flags)
                names = {v: str(k) for k, v in t.names.iteritems()}
                r['flags' if is_flags else 'enum'] = names
            elif isinstance(value, IdRef):
                r['baseType'] = t.base_type.__name__
                r['refPath'] = path_of(value.target)
        r['type'] = t.__name__
        return r

    def fetch(self, path):
        obj = self._walk(path)
        t = type(obj)
        propvalue = self._propvalue
        if is_record(obj):
            return [{fname: propvalue(fvalue)}
                    for fname, fvalue in obj._fields.iteritems()
                    if not fname.startswith('_')]
        elif is_array(obj):
            keys = t.keys
            return [{keys.get(i, ''): propvalue(item)}
                    for i, item in enumerate(obj)]
        else:
            return obj

    def update(self, path, selector, value):
        obj = self._walk(path)
        with transaction(obj) as tr:
            try:
                obj[selector] = value
                affected = tr.commit()
            except Exception as ex:
                traceback.print_exc(file=stderr)
                tr.rollback()
                return {'error': str(ex)}
            else:
                return [path_of(obj) for obj in affected]

    def getArrayInfo(self, path):
        array = self._walk(path)
        keys = type(array).keys
        return {
            'size': len(array),
            'keys': {key: i for i, key in keys.iteritems()},
            'names': [item._name for item in array],
        }
  

RequestHandler()




