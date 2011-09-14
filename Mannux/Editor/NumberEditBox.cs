
/*
 * Handly little util things for the editor.
 */

using System;
using System.Windows.Forms;

namespace Editor {
    class NumberEditBox : TextBox {
        protected override CreateParams CreateParams {
            get {
                CreateParams c = base.CreateParams;
                c.ClassStyle |= 0x2000; // ES_NUMBER

                return c;
            }
        }
    }
}
