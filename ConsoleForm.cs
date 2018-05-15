using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WarBender {
    public partial class ConsoleForm : Form {
        public ConsoleForm() {
            InitializeComponent();
            CreateHandle();

            const string familyName = "Consolas";
            var font = new Font(familyName, richTextBox.Font.Size);
            if (font.FontFamily.Name == familyName) {
                richTextBox.Font = font;
            }
        }

        void ScrollToCaret() {
            // RichTextBox.ScrollToCaret is broken for fast updates.
            NativeMethods.SendMessage(richTextBox.Handle, NativeMethods.WM_VSCROLL, (IntPtr)NativeMethods.SB_PAGEBOTTOM, IntPtr.Zero);
        }

        public void WriteLine(string s) {
            BeginInvoke((Action)delegate {
                richTextBox.AppendText(s + "\r\n");
                ScrollToCaret();
            });
        }

        private void ConsoleForm_FormClosing(object sender, FormClosingEventArgs e) {
            Hide();
            e.Cancel = true;
        }

        public void MakeVisible(bool visible, bool blocking = true) {
            Action doIt = delegate {
                Visible = visible;
                if (visible) {
                    Activate();
                }
                Refresh();
            };
            if (blocking) {
                Invoke(doIt);
            } else {
                BeginInvoke(doIt);
            }
        }
    }
}
