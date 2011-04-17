using System;
using System.Collections;
using System.Windows.Forms;

using Import;

namespace Editor {

    class MapEntPropertiesView : Form {
        Editor editor;
        Engine engine;

        ListBox enttypes;
        TextBox[] entprops;

        const int numprops = 5;	// the number of property slots to display.  Change if 5 is insufficient.

        public MapEntPropertiesView(Editor e)
            : base() {
            editor = e;
            engine = e.engine;

            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Text = "MapEnt properties";
            ShowInTaskbar = false;

            enttypes = new ListBox();
            enttypes.Location = new System.Drawing.Point(10, 10);
            enttypes.Items.Clear();
            foreach (string s in MapEnt.enttypes)
                enttypes.Items.Add(s);
            enttypes.Show();

            Controls.Add(enttypes);

            entprops = new TextBox[5];
            int x = 10;
            int y = enttypes.Bottom + 5;

            for (int i = 0; i < numprops; i++) {
                entprops[i] = new TextBox();
                entprops[i].Location = new System.Drawing.Point(x, y);
                entprops[i].Show();
                Controls.Add(entprops[i]);
                y = entprops[i].Bottom + 5;
            }
        }

        public void UpdateEnt(MapEnt e) {
            if (e == null)
                return;

            e.type = (string)enttypes.SelectedItem;
            e.data = new string[numprops];

            for (int i = 0; i < numprops; i++)
                e.data[i] = entprops[i].Text;
        }

        public void UpdateDlg(MapEnt e) {
            if (e == null)
                return;

            int idx = Array.IndexOf(MapEnt.enttypes, e.type);
            if (idx == -1) {
                e.type = (string)enttypes.Items[0];
                enttypes.SelectedIndex = 0;
            } else
                enttypes.SelectedIndex = idx;

            int i = 0;
            do {
                if (e.data != null && i < e.data.Length)
                    entprops[i].Text = e.data[i];
                else
                    entprops[i].Text = "";
            } while (++i < numprops);
        }
    }

}
