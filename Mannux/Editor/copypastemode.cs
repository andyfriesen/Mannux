// Currently for copying/pasting to/from one layer only.
// No entities or obstructions are copied as of yet.
// The selection isn't all that accurate. -_-;
// fix it later.

using System;
using System.Windows.Forms;

using Import;
using Import.Geo;
using Microsoft.Xna.Framework;

namespace Editor {
    class CopyPasteMode : IEditorState {
        enum EditState {
            DoingNothing, Copying, Pasting
        }

        Editor editor;
        Engine engine;
        Vertex p1;
        Vertex p2;
        int[,] curselection;
        EditState state = EditState.DoingNothing;

        public CopyPasteMode(Editor e) {
            editor = e;
            engine = e.engine;
        }

        public void MouseDown(Point e, Input.MouseButton b) {
            int x = e.X;
            int y = e.Y;

            ScreenToTile(ref x, ref y);

            switch (state) {
                case EditState.Copying:
                if (x >= p1.X) x++;
                if (y >= p1.Y) y++;

                p2.X = x;
                p2.Y = y;
                break;

                case EditState.Pasting:
                p1.X = x;
                p1.Y = y;
                p2.X = x + curselection.GetLength(0);
                p2.Y = y + curselection.GetLength(1);
                break;
            }
        }

        public void MouseUp(Point e, Input.MouseButton b) {
            switch (state) {
                case EditState.Copying: {
                    var x1 = (int)p1.X;
                    var y1 = (int)p1.Y;
                    var x2 = (int)p2.X;
                    var y2 = (int)p2.Y;

                    var i = x1;
                    if (x1 > x2) { i = x1; x1 = x2; x2 = i; }
                    if (y1 > y2) { i = y1; y1 = y2; y2 = i; }

                    curselection = Copy(x1, y1, x2, y2);

                    state = EditState.DoingNothing;
                    break;
                }
                case EditState.Pasting: {
                    int x = e.X;
                    int y = e.Y;
                    ScreenToTile(ref x, ref y);

                    Paste(x, y, curselection);

                    state = EditState.DoingNothing;
                    break;
                }
            }
        }

        void ScreenToTile(ref int x, ref int y) {
            x = (x + engine.XWin) / engine.tileset.Width;
            y = (y + engine.YWin) / engine.tileset.Height;
        }

        public void MouseClick(Point e, Input.MouseButton b) {
            if ((Control.ModifierKeys & Keys.Shift) != 0) {
                int x = e.X;
                int y = e.Y;
                ScreenToTile(ref x, ref y);

                p1.X = x;
                p1.Y = y;

#if false
                if ((e.Button & MouseButtons.Left) != 0) {
#endif
                    if (state == EditState.Pasting) {
                        // left click while pasting to cancel
                        state = EditState.DoingNothing;
                        return;
                    }

                    state = EditState.Copying;
                    p2.X = x; p2.Y = y;
#if false
                } else if ((e.Button & MouseButtons.Right) != 0 && curselection != null) {
                    state = EditState.Pasting;
                    p2.X = x + curselection.GetLength(0);
                    p2.Y = y + curselection.GetLength(1);
                }
#endif
            }
        }

        public void KeyPress(KeyEventArgs e) {

        }

        public void MouseWheel(Point p, int delta) {
        }

        public void RenderHUD() {
#if false
            if (p1 == null)
                return;

            Gl.glTranslatef(-engine.XWin, -engine.YWin, 0);

            int tx = engine.tileset.Width;
            int ty = engine.tileset.Height;

            int x1 = p1.X * tx;
            int x2 = p2.X * tx;

            int y1 = p1.Y * ty;
            int y2 = p2.Y * ty;

            if (state == EditState.Copying) {
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
                Gl.glColor4f(1, 1, 1, 1);
                Gl.glBegin(Gl.GL_LINE_LOOP);
                Gl.glVertex2i(x1, y1);
                Gl.glVertex2i(x2, y1);
                Gl.glVertex2i(x2, y2);
                Gl.glVertex2i(x1, y2);
                Gl.glEnd();
            } else if (state == EditState.Pasting) {
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
                Gl.glColor4f(0, 0, 1, 1);
                Gl.glBegin(Gl.GL_LINE_LOOP);
                Gl.glVertex2i(x1, y1);
                Gl.glVertex2i(x2, y1);
                Gl.glVertex2i(x2, y2);
                Gl.glVertex2i(x1, y2);
                Gl.glEnd();
            }

            Gl.glLoadIdentity();
#endif
        }

        int[,] Copy(int x1, int y1, int x2, int y2) {
            int[,] a = new int[x2 - x1, y2 - y1];
            for (int y = 0; y < y2 - y1; y++)
                for (int x = 0; x < x2 - x1; x++)
                    a[x, y] = engine.map[editor.tilesetmode.curlayer][x + x1, y + y1];

            return a;
        }

        void Paste(int sx, int sy, int[,] src) {
            // TODO: better clipping?  this is kinda gay

            for (int y = 0; y < src.GetLength(1); y++)
                for (int x = 0; x < src.GetLength(0); x++) {
                    if (sx + x >= engine.map.Width)
                        break;
                    if (sy + y >= engine.map.Height)
                        return;

                    engine.map[editor.tilesetmode.curlayer][sx + x, sy + y] = src[x, y];
                }
        }

        public int[,] Selection {
            get { return curselection; }
            set { curselection = value; }
        }
    }
}
