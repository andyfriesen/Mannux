
using System;
using System.Windows.Forms;
using System.Drawing;

namespace Editor {

    class AutoSelectionThing : Form {
        Editor editor;

        TextBox firsttile;
        TextBox width;
        TextBox height;
        TextBox span;	// defaults to 18 because that's how corey does his tiles

        Point pos = new Point(0, 0);

        TextBox AddBox(string name) {
            Label l = new Label();
            l.Text = name;
            l.Location = pos;
            Controls.Add(l);

            TextBox t = new NumberEditBox();	// NumberEditBox is defined in util.cs
            t.Location = new Point(l.Right, l.Top);
            Controls.Add(t);

            pos.Y = t.Bottom + 5;

            return t;
        }

        public AutoSelectionThing(Editor e)
            : base() {
            editor = e;

            firsttile = AddBox("First Tile");
            width = AddBox("Width");
            height = AddBox("Height");
            span = AddBox("Span");
            span.Text = "19";

            Button b = new Button();
            b.Text = "Use Current";
            b.Location = new Point(firsttile.Right + 5, firsttile.Top);
            b.Click += new EventHandler(UseCurrentTile);
            Controls.Add(b);

            b = new Button();
            b.Text = "Do it";
            b.Location = new Point(0, span.Bottom + 5);
            b.Click += new EventHandler(DoIt);
            Controls.Add(b);

            Button c = new Button();
            c.Text = "Close";
            c.Location = new Point(b.Right + 5, b.Top);
            CancelButton = c;
            Controls.Add(c);
        }

        void UseCurrentTile(object o, EventArgs e) {
            firsttile.Text = editor.tilesetmode.curtile.ToString();
        }

        void DoIt(object o, EventArgs e) {
            int t = Convert.ToInt32(firsttile.Text);
            int w = Convert.ToInt32(width.Text);
            int h = Convert.ToInt32(height.Text);
            int s = Convert.ToInt32(span.Text);

            if (t + w * h >= editor.engine.tileset.NumTiles)
                return;

            int[,] b = new int[w, h];

            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    b[x, y] = t;
                    t++;
                }
                t += s - w;
            }

            editor.copypastemode.Selection = b;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            e.Cancel = true;
            Hide();
        }
    }

}
