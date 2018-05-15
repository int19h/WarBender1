from __future__ import absolute_import, division, print_function

import json
from sys import exit, stderr, stdin
import traceback

from warbend import mode
mode.is_quiet = True

from warbend.data import path, selector, transaction
from warbend.data.mutable import is_mutable
from warbend.data.array import is_array
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

    def fetch(self, selectors):
        obj = self._game
        for sel in selectors:
            try:
                obj = obj[sel]
            except Exception:
                traceback.print_exc(file=stderr)
                return None

        def propvalue(value):
            t = type(value)
            if not is_mutable(value):
                return value
            return {
                'selector': selector(value),
                'path': path(value),
                'type': t.__name__,
                'totalCount': len(value),
                'mutableCount': sum(is_mutable(child) for child in value)
            }

        t = type(obj)
        if is_record(obj):
            return [{fname: propvalue(fvalue)}
                    for fname, fvalue in obj._fields.iteritems()
                    if not fname.startswith('_')]
        elif is_array(obj):
            keys = t.keys
            fmt = u'[%%0%dd]\u2002' % len(str(len(obj) - 1))
            def propname(i):
                return (fmt % i) + keys.get(i, '')
            return [{propname(i): propvalue(item)}
                    for i, item in enumerate(obj)]
        else:
            return obj

    def update(self, selectors, value):
        if not selectors:
            raise ValueError()
        obj = self._game
        for sel in selectors[:-1]:
            try:
                obj = obj[sel]
            except Exception:
                traceback.print_exc(file=stderr)
                return {}
        with transaction(obj) as tr:
            try:
                obj[selectors[-1]] = value
                affected = tr.commit()
            except Exception as ex:
                traceback.print_exc(file=stderr)
                tr.rollback()
                return {'error': str(ex)}
            else:
                return [path(obj) for obj in affected]
  

RequestHandler()




