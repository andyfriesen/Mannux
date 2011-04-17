
using System;
using System.Windows.Forms;
using System.Drawing;

namespace Editor {
    delegate void ChangeTileHandler(int t);

    class TileSetPreview : Form {
        Import.TileSet tileset;
        Panel panel = new Panel();
        bool pad = false;	// if true, we draw one pixel of padding between each tile

        public TileSetPreview(Import.TileSet t) {
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Text = "Tile set";
            ShowInTaskbar = false;

            BackColor = Color.Black;

            tileset = t;

            AutoScroll = true;

            panel.Size = ClientSize;
            Controls.Add(panel);
            panel.Paint += new PaintEventHandler(Draw);
            panel.Show();

            ClientSize = new Size(t.Width * 19, ClientSize.Height);
            UpdatePanel();

            Resize += new EventHandler(OnResize);
            panel.MouseDown += new MouseEventHandler(OnClick);
        }

        void UpdatePanel() {
            // figure out how many tiles to draw on each axis
            int xt = tileset.Width;
            int yt = tileset.Height;

            if (pad) {
                xt++;
                yt++;
            }

            // don't care how much it takes on the y axis.  Howevermany it takes.
            int xtile = panel.Width / xt;
            int ytile = tileset.NumTiles / xtile;

            panel.Width = ClientSize.Width;
            panel.Height = yt * ytile;

            using (Graphics gfx = panel.CreateGraphics()) {
                gfx.CompositingMode = CompositingMode.SourceCopy;
                gfx.CompositingQuality = CompositingQuality.HighSpeed;

                int drawx = pad ? 1 : 0;
                int drawy = drawx;
                for (int y = 0; y < ytile; y++) {
                    for (int x = 0; x < xtile; x++) {
                        var index = y * xtile + x;
                        if (index > tileset.NumTiles) {
                            return;
                        }
                        gfx.DrawImageUnscaled(tileset[y * xtile + x], drawx, drawy);
                        drawx += xt;
                    }

                    drawx = pad ? 1 : 0;
                    drawy += yt;
                }
            }
        }

        void Draw(object o, PaintEventArgs e) {
            UpdatePanel();
        }

        void OnResize(object o, EventArgs e) {
            Refresh();
        }

        int TileFromPoint(int x, int y) {
            int xtile = tileset.Width + (pad ? 1 : 0);
            x /= xtile;
            y /= tileset.Height + (pad ? 1 : 0);

            return y * (panel.Width / xtile) + x;
        }

        Point PointFromTile(int i) {
            int tx = tileset.Width + (pad ? 1 : 0);
            int ty = tileset.Height + (pad ? 1 : 0);

            int tilex = panel.Width / tx;

            Point p = new Point();

            p.Y = (i / tilex) * ty;
            p.X = (i % tilex) * tx;

            return p;
        }

        void OnClick(object o, MouseEventArgs e) {
            int i = TileFromPoint(e.X, e.Y);

            Console.WriteLine("Click on tile: {0}", i);
            Point p = PointFromTile(i);
            Console.WriteLine("{0},{1}", p.X, p.Y);

            if (ChangeTile != null)
                ChangeTile(i);
        }

        public event ChangeTileHandler ChangeTile;

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }
    }
}
