using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;

namespace WarBender {
    [TypeDescriptionProvider(typeof(MutableTypeDescriptionProvider))]
    [TypeConverter(typeof(MutableTypeConverter))]
    class Mutable {
        public struct Child {
            public readonly string Name;
            public readonly object Value;
            public readonly TypeConverter Converter;

            public Child(string name, object value, TypeConverter converter) {
                Name = name;
                Value = value;
                Converter = converter;
            }
        }

        static readonly Dictionary<string, WeakReference> mutables = new Dictionary<string, WeakReference>();

        readonly WarbendService warbend;

        public Mutable Parent { get; }

        public string Path { get; }

        Lazy<Child[]> childrenLazy;

        // Values are indices into the above array.
        readonly Dictionary<string, int> namedChildren = new Dictionary<string, int>();

        public string TypeName { get; }

        public int MutableCount { get; }

        public bool IsArray { get; }

        public bool IsRecord => !IsArray;

        private Mutable(WarbendService warbend, Mutable parent, string path, string typeName, int mutableCount) {
            this.warbend = warbend;
            Parent = parent;
            Path = path;
            TypeName = typeName;
            IsArray = typeName?.Contains("array(") == true;
            MutableCount = mutableCount;

            mutables[path] = new WeakReference(this);
            Invalidate();
        }

        public Mutable(WarbendService warbend, string typeName, int mutableCount = 1)
            : this(warbend, null, "game", typeName, mutableCount) {
        }

        public event EventHandler Invalid;

        public static event EventHandler<ISet<Mutable>> Invalidated;

        public static Mutable At(string path) {
            mutables.TryGetValue(path, out var weakRef);
            return weakRef?.Target as Mutable;
        }

        readonly Dictionary<string, Type> types = new Dictionary<string, Type>() {
            ["int8"] = typeof(sbyte),
            ["int16"] = typeof(short),
            ["int32"] = typeof(int),
            ["int64"] = typeof(long),
            ["uint8"] = typeof(byte),
            ["uint16"] = typeof(ushort),
            ["uint32"] = typeof(uint),
            ["uint64"] = typeof(ulong),
            ["float32"] = typeof(float),
            ["bool8"] = typeof(bool),
            ["bool32"] = typeof(bool),
            ["pstr"] = typeof(string),
        };

        IEnumerable<Child> FetchChildren() {
            var isRecord = IsRecord;
            var items = warbend.FetchAsync(Path).GetAwaiter().GetResult();
            int index = 0;
            foreach (JObject item in items) {
                var prop = item.Properties().First();
                var name = prop.Name;

                var info = (JObject)prop.Value;
                var typeName = info.Value<string>("type");
                object value;
                TypeConverter converter = null;

                if (info.Value<string>("path") is string path) {
                    var mutableCount = info.Value<int>("mutableCount");
                    value = new Mutable(warbend, this, path, typeName, mutableCount);
                } else {
                    Type type = null;

                    if ((info.Value<JObject>("enum") ?? info.Value<JObject>("flags")) is JObject nameObj) {
                        var baseTypeName = info.Value<string>("baseType");
                        type = types[baseTypeName];
                        var names = nameObj.Properties().ToDictionary(
                            p => p.Name,
                            p => Convert.ChangeType(p.Value.Value<string>(), type, CultureInfo.InvariantCulture));
                        if (info.ContainsKey("flags")) {
                            converter = new FlagsTypeConverter(type, names);
                        } else {
                            converter = new EnumTypeConverter(type, names);
                        }
                    } else if (info.Value<string>("refPath") is string refPath) {
                        var baseTypeName = info.Value<string>("baseType");
                        type = types[baseTypeName];
                        converter = new IdRefTypeConverter(warbend, type, refPath);
                    }

                    type = type ?? types[typeName];
                    value = info.Value<string>("value");
                    if (type == typeof(bool)) {
                        value = !Equals(value, "0");
                    } else {
                        value = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
                    }
                }

                yield return new Child(name, value, converter);
                namedChildren.Add(name, index);
                ++index;
            }
        }

        public void Invalidate() {
            childrenLazy = new Lazy<Child[]>(() => FetchChildren().ToArray());
            namedChildren.Clear();
            Invalid?.Invoke(this, EventArgs.Empty);
        }

        public override int GetHashCode() => TypeName.GetHashCode();

        public override bool Equals(object obj) {
            if (!(obj is Mutable other)) {
                return false;
            }
            return TypeName == other.TypeName;
        }

        public Child[] Children => childrenLazy.Value;

        public object this[object selector] {
            get {
                switch (selector) {
                    case int index:
                        return Children[index].Value;
                    case string name:
                        return Children[namedChildren[name]].Value;
                    default:
                        throw new ArgumentException();
                }
            }
            set {
                if (childrenLazy.IsValueCreated) {
                    var oldValue = Children.FirstOrDefault(child => Equals(child.Name, selector)).Value;
                    if (Equals(value, oldValue)) {
                        return;
                    }
                }

                var response = warbend.UpdateAsync(Path, selector, value).GetAwaiter().GetResult();
                if (response is JObject error) {
                    var message = error.Value<string>("error");
                    throw new InvalidOperationException(message);
                }

                var affectedPaths = response.Select(token => token.Value<string>()).ToArray();
                var affectedSet = new HashSet<Mutable>();
                foreach (var path in affectedPaths) {
                    var affected = At(path);
                    if (affected != null) { 
                        affected.Invalidate();
                        affectedSet.Add(affected);
                    }
                }
                Invalidated?.Invoke(this, affectedSet);
            }
        }
    }

    class MutableTypeConverter : ExpandableObjectConverter {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            destinationType == typeof(string);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType != typeof(string)) {
                throw new InvalidOperationException();
            }
            return ((Mutable)value).TypeName;
        }
    }

    class MutablePropertyDescriptor : PropertyDescriptor {
        readonly Mutable owner;
        readonly object selector;

        public MutablePropertyDescriptor(Mutable owner, object selector, Mutable.Child child)
            : base(child.Name, ComputeAttributes(owner, child).ToArray()) {
            this.owner = owner;
            this.selector = selector;
            Child = child;
            IsReadOnly = child.Value is Mutable || child.Name.StartsWith("(");
        }

        public Mutable.Child Child { get; }

        public object Value => owner[selector];

        static IEnumerable<Attribute> ComputeAttributes(Mutable owner, Mutable.Child child) {
            if (child.Value is string) {
                yield return new EditorAttribute(typeof(MultilineStringEditor), typeof(UITypeEditor));
            } else if (child.Converter is FlagsTypeConverter) {
                yield return new EditorAttribute(typeof(FlagsEditor), typeof(UITypeEditor));
            }

            if (owner.IsArray) {
                yield return new CategoryAttribute("Items");
            } else { 
                if (child.Value is Mutable mutable && mutable.IsArray) {
                    yield return new CategoryAttribute("Collections");
                } else {
                    yield return new CategoryAttribute("Attributes");
                }
            }
        }

        public override TypeConverter Converter => Child.Converter;

        public override Type ComponentType => typeof(Mutable);

        public override bool IsReadOnly { get; }

        public override bool SupportsChangeEvents => false;

        public override Type PropertyType => Value.GetType();

        public override bool ShouldSerializeValue(object component) => false;

        public override bool CanResetValue(object component) => false;

        public override void ResetValue(object component) =>
            throw new NotImplementedException();

        private void EnsureOwner(object component) {
            if (!(component is Mutable mut) || mut.Path != owner.Path) {
                throw new ArgumentException("component is not owner", "component");
            }
        }

        public override object GetValue(object component) {
            EnsureOwner(component);
            return Value;
        }

        public override void SetValue(object component, object value) {
            EnsureOwner(component);
            owner[selector] = value;
        }

        public override void AddValueChanged(object component, EventHandler handler) {
            EnsureOwner(component);
            owner.Invalid += handler;
        }

        public override void RemoveValueChanged(object component, EventHandler handler) {
            EnsureOwner(component);
            owner.Invalid -= handler;
        }
    }

    class MutableTypeDescriptor : ICustomTypeDescriptor {
        class PropertyChangedEventDescriptor : EventDescriptor {
            public PropertyChangedEventHandler Handlers;

            public PropertyChangedEventDescriptor()
                : base("PropertyChanged", new Attribute[0]) {
            }

            public override Type ComponentType => typeof(MutableTypeDescriptor);

            public override Type EventType => typeof(PropertyChangedEventHandler);

            public override bool IsMulticast => true;

            public override void AddEventHandler(object component, Delegate value) {
                Handlers += (PropertyChangedEventHandler)value;
            }

            public override void RemoveEventHandler(object component, Delegate value) {
                Handlers -= (PropertyChangedEventHandler)value;
            }
        }

        readonly Mutable mutable;
        Lazy<PropertyDescriptorCollection> properties;

        public MutableTypeDescriptor(Mutable mutable) {
            this.mutable = mutable;
            properties = new Lazy<PropertyDescriptorCollection>(() =>
                new PropertyDescriptorCollection(CreateProperties().ToArray()));
        }

        public PropertyDescriptorCollection GetProperties() => properties.Value;

        IEnumerable<PropertyDescriptor> CreateProperties() {
            object Selector(int index, string name) =>
                mutable.IsArray ? (object)index : name;
            return mutable.Children.Select((child, i) =>
                new MutablePropertyDescriptor(mutable, Selector(i, child.Name), child));
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => GetProperties();

        public string GetClassName() => null;

        public string GetComponentName() => null;

        public AttributeCollection GetAttributes() => AttributeCollection.Empty;

        public TypeConverter GetConverter() => new MutableTypeConverter();

        public EventDescriptor GetDefaultEvent() => null;

        public PropertyDescriptor GetDefaultProperty() => null;

        public object GetEditor(Type editorBaseType) => null;

        public EventDescriptorCollection GetEvents() => EventDescriptorCollection.Empty;

        public EventDescriptorCollection GetEvents(Attribute[] attributes) => EventDescriptorCollection.Empty;

        public object GetPropertyOwner(PropertyDescriptor pd) => this;
    }

    class MutableTypeDescriptionProvider : TypeDescriptionProvider {
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
            return new MutableTypeDescriptor((Mutable)instance);
        }
    }
}
