using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarBender {
    enum ObjectKind {
        None,
        Record,
        Array,
    }

    class ObjectPath {
        public Mutable Parent { get; }

        public object Selector { get; }

        public ObjectPath()
            : this(null, null) {
        }

        public ObjectPath(Mutable parent, object selector) {
            Parent = parent;
            Selector = selector;
        }

        public ObjectPath this[Mutable parent, object selector] =>
            new ObjectPath(parent, selector);

        public IEnumerable<object> Selectors {
            get {
                for (var path = this; path.Parent != null; path = path.Parent) {
                    yield return path.Selector;
                }
            }
        }

        public bool Matches(ObjectPath other) {
            if (other == null) {
                return false;
            }
            if (ParentKind != other.ParentKind) {
                return false;
            }
            if (Selector != null && other.Selector != null && !Selector.Equals(other.Selector)) {
                return false;
            }
            return Parent == other.Parent || Parent?.Matches(other.Parent) == true;
        }

        public override string ToString() {
            string s = Parent?.ToString() ?? "";
            object selectorOrStar = Selector ?? "*";
            switch (ParentKind) {
                case ObjectKind.None:
                    break;
                case ObjectKind.Record:
                    s += "." + selectorOrStar;
                    break;
                case ObjectKind.Array:
                    if (Selector is string) {
                        s += "['" + selectorOrStar + "']";
                    } else {
                        s += "[" + selectorOrStar + "]";
                    }
                    break;
            }
            return s;
        }
    }
}
