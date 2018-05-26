using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace WarBender {
    class IdRefTypeConverter : TypeConverter {
        readonly WarbendService warbend;
        public readonly Type BaseType;
        public readonly string Path;
        readonly Lazy<StandardValuesCollection> standardValues;

        public IdRefTypeConverter(WarbendService warbend, Type baseType, string path) {
            this.warbend = warbend;
            BaseType = baseType;
            Path = path;

            standardValues = new Lazy<StandardValuesCollection>(() => {
                var resp = warbend.GetArrayInfoAsync(Path).GetAwaiter().GetResult();
                var size = resp.Value<int>("size");
                return new StandardValuesCollection(Enumerable.Range(0, size).ToArray());
            });
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

            return Convert.ChangeType(s, BaseType, CultureInfo.InvariantCulture);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            destinationType == typeof(string);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (value == null) {
                return "";
            }

            var index = ((IConvertible)value).ToInt32(culture);
            var resp = warbend.GetArrayInfoAsync(Path).GetAwaiter().GetResult();
            var keys = resp.Value<JObject>("keys");
            var fmt = "{0}";
            string key = null;
            foreach (var kv in keys) {
                if (kv.Value.Value<int>() == index) {
                    fmt += " ({1})";
                    key = kv.Key;
                    break;
                }
            }
            return string.Format(CultureInfo.InvariantCulture, fmt, value, key);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) =>
            standardValues.Value;
    }
}
