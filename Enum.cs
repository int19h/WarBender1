using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarBender {
    class EnumTypeConverter : TypeConverter {
        protected readonly Type baseType;
        protected readonly Dictionary<string, object> names;

        public EnumTypeConverter(Type baseType, IDictionary<string, object> names) {
            this.baseType = baseType;
            this.names = new Dictionary<string, object>(names);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (!(value is string s)) {
                throw new ArgumentException("", "value");
            }

            int i = s.IndexOf('(');
            if (i >= 0) {
                s = s.Substring(0, i);
            }

            return Convert.ChangeType(s, baseType, CultureInfo.InvariantCulture);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            destinationType == typeof(string);

        protected virtual IEnumerable<string> GetNames(object value) =>
            names.Where(kv => Equals(kv.Value, value)).Select(kv => kv.Key);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            var fmt = "{0}";
            var names = GetNames(value);
            if (names != null) {
                fmt += " ({1})";
            }
            return string.Format(CultureInfo.InvariantCulture, fmt, value, string.Join(", ", names));
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) =>
            new StandardValuesCollection(names.Values);
    }

    class FlagsTypeConverter : EnumTypeConverter {
        public FlagsTypeConverter(Type baseType, IDictionary<string, object> names)
            : base(baseType, names) {
        }

        protected override IEnumerable<string> GetNames(dynamic value) {
            var flags = new List<string>();
            dynamic x = value & 0;
            foreach (var kv in names) {
                dynamic flag = kv.Value;
                if (flag == 0 && value != 0) {
                    continue;
                }
                if (Equals(value & flag, flag)) {
                    flags.Add(kv.Key);
                    x |= flag;
                }
            }

            if (!Equals(value, x)) {
                flags.Clear();
            }

            return flags;
        }
    }
}
