using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace WarBender {
    class EnumTypeConverter : TypeConverter {
        public readonly Type BaseType;
        public readonly Dictionary<string, object> Names;

        public EnumTypeConverter(Type baseType, IDictionary<string, object> names) {
            BaseType = baseType;
            Names = new Dictionary<string, object>(names);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(string);

        protected virtual object Parse(string s) => s;

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (!(value is string s)) {
                throw new ArgumentException("", "value");
            }

            int i = s.IndexOf('(');
            if (i >= 0) {
                s = s.Substring(0, i);
            }

            return Convert.ChangeType(Parse(s), BaseType, CultureInfo.InvariantCulture);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            destinationType == typeof(string);

        protected virtual IEnumerable<string> GetNames(object value) =>
            Names.Where(kv => Equals(kv.Value, value)).Select(kv => kv.Key);

        protected virtual string Stringify(object value) => value.ToString();

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            var s = Stringify(value);
            var names = GetNames(value).ToArray();
            if (names.Length != 0) {
                s += $" ({string.Join(", ", names)})";
            }
            return s;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) =>
            new StandardValuesCollection(Names.Values);
    }

    class FlagsTypeConverter : EnumTypeConverter {
        public FlagsTypeConverter(Type baseType, IDictionary<string, object> names)
            : base(baseType, names) {
        }

        protected override string Stringify(object value) {
            int maxLen = 0;
            foreach (var kv in Names) {
                var s = string.Format("{0:X}", kv.Value);
                if (s.Length > maxLen) {
                    maxLen = s.Length;
                }
            }

            var result = string.Format("{0:X}", value);
            result = "0x" + result.PadLeft(maxLen, '0');
            return result;
        }

        protected override object Parse(string s) =>
            s.StartsWith("0x") ? ulong.Parse(s.Substring(2), NumberStyles.HexNumber) : base.Parse(s);

        protected override IEnumerable<string> GetNames(dynamic value) {
            var flags = new List<string>();
            dynamic x = value & 0;
            foreach (var kv in Names) {
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

    class FlagsEditor : UITypeEditor {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) =>
            UITypeEditorEditStyle.DropDown;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, dynamic value) {
            var converter = (EnumTypeConverter)context.PropertyDescriptor.Converter;

            var listBox = new CheckedListBox {
                CheckOnClick = true,
            };
            var flags = new List<dynamic>();
            foreach (var kv in converter.Names) {
                dynamic flag = kv.Value;
                if (flag == 0) {
                    continue;
                }

                var text = converter.ConvertToString(context, CultureInfo.InvariantCulture, flag);
                var isChecked = (value & flag) == flag;
                listBox.Items.Add(text, isChecked);
                flags.Add(flag);
            }

            listBox.ItemCheck += (sender, e) => {
                if (e.NewValue == CheckState.Checked) {
                    value |= flags[e.Index];
                } else {
                    value &= ~flags[e.Index];
                }
            };

            var service = ((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService)));
            service.DropDownControl(listBox);
            return value;
        }
    }
}
