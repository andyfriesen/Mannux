
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Editor {
    delegate void ChangeTileHandler(int t);

    class TileSetPreview : Form {
        class DoubleBufferedPanel : Panel {
            public DoubleBufferedPanel() {
                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.AllPaintingInWmPaint,
                    true
                );
            }
        }

        Tileset tileset;
        Panel panel = new DoubleBufferedPanel();

        public TileSetPreview(Tileset t) {
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Text = "Tile set";
            ShowInTaskbar = false;

            BackColor = Color.Black;

            tileset = t;

            AutoScroll = true;

            DoubleBuffered = true;
            panel.Size = new Size(t.Bitmap.Width, t.Bitmap.Height);
            Controls.Add(panel);
            panel.Paint += Draw;
            panel.Show();

            ClientSize = new Size(t.Bitmap.Width, ClientSize.Height);
            Refresh();

            Resize += OnResize;
            panel.MouseDown += OnClick;
        }

        void Draw(object o, PaintEventArgs e) {
            e.Graphics.DrawImageUnscaled(tileset.Bitmap, new Point(0, 0));
        }

        void OnResize(object o, EventArgs e) {
            Refresh();
        }

        void OnClick(object o, MouseEventArgs e) {
            int i = tileset.TileFromPoint(e.X, e.Y);

            Console.WriteLine("Click on tile: {0}", i);
            Point p = tileset.PointFromTile(i);
            Console.WriteLine("{0},{1}", p.X, p.Y);

            if (ChangeTile != null) {
                ChangeTile(i);
            }
        }

        public event ChangeTileHandler ChangeTile;

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }
    }
}
