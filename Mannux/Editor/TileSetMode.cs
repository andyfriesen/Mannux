using System;
using System.Windows.Forms;

namespace Editor {
    class TileSetMode : IEditorState {
        Editor editor;
        //Engine engine;
        object engine;

        // ooh evil.
        public int curtile = 0;
        public int curlayer = 0;
        int oldx, oldy;

        public TileSetMode(Editor e) {
            editor = e;
            engine = e.engine;
        }

        public void MouseDown(MouseEventArgs e) {
            MouseClick(e);
        }

        public void MouseUp(MouseEventArgs e) {
        }

        public void MouseClick(MouseEventArgs e) {
            int tilex = (e.X + engine.XWin) / engine.tileset.Width;
            int tiley = (e.Y + engine.YWin) / engine.tileset.Height;

            //if (tilex==oldx && tiley==oldy)
            //	return;

            editor.statbar.Panels[0].Text = String.Format("Layer {0}", curlayer);
            editor.statbar.Panels[1].Text = String.Format("({0},{1})", tilex, tiley);

            if (e.Delta != 0) {
                curtile += e.Delta / 120;	// don't ask why, e.delta is in 120s for some reason.

                if (curtile < 0)
                    curtile += engine.tileset.NumTiles;
                if (curtile >= engine.tileset.NumTiles)
                    curtile -= engine.tileset.NumTiles;
                return;
            }


            if (e.Button != MouseButtons.Left)
                return;

            oldx = tilex;
            oldy = tiley;

            if ((Control.ModifierKeys & Keys.Shift) != 0)
                curtile = engine.map[curlayer][tilex, tiley];
            else
                engine.map[curlayer][tilex, tiley] = curtile;
        }

        public void KeyPress(KeyEventArgs e) {
            if (e.KeyCode == Keys.A) {
                if (curtile < engine.tileset.NumTiles) curtile++;
            }
            if (e.KeyCode == Keys.Z) {
                if (curtile > 0) curtile--;
            }
        }

        public void RenderHUD() {
            int x = 2;
            int y = engine.graph.YRes - 18;

            engine.graph.Blit(engine.tileset[curtile], x, y, false);
        }
    }
}
