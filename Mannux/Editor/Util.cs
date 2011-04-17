
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

    sealed class Util {
        private Util() { }

        /* // no need for the following monkey feces:
        public static MenuItem CreateMenu(string name,object[][] items)
        {
            MenuItem m=new MenuItem(name);
            foreach (object[] o in items)
            {
                m.MenuItems.Add(new MenuItem((string)(o[0]),(EventHandler)(o[1])));
            }

            return m;
        }
        */
    }
}
