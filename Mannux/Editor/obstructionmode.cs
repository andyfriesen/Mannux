using System;
using System.Windows.Forms;
using Import.Geo;
using Microsoft.Xna.Framework;

namespace Editor {
    class ObstructionMode : IEditorState {
        Editor editor;
        Engine engine;
        public int curpoint;
        bool leftdown = false;
        bool rightdown = false;
        bool snap = true;


        public ObstructionMode(Editor e) {
            editor = e;
            engine = e.engine;
        }

        int dist(ref Import.Geo.Point p, int x, int y) {
            int dx = p.X - x;
            int dy = p.Y - y;

            return (int)Math.Sqrt(dx * dx + dy * dy);
        }

        void LeftClick(int x, int y) {
            if (leftdown)
                return;

            leftdown = true;

            // find the nearest point
            int idx = 0;
            int best = 99999;
            for (int i = 0; i < engine.map.Obs.Points.Count; i++) {
                var p = (Import.Geo.Point)engine.map.Obs.Points[i];
                var d = dist(ref p, x, y);

                // we skip the current one, so that you can get the next closest one.
                // Makes handling dots that wind up close to each other easier.
                if (d < best && i != curpoint) {
                    idx = i;
                    best = d;
                }
            }

            if (idx == curpoint)
                return;

            if ((Control.ModifierKeys & Keys.Shift) != 0) {
                // first, make sure that these two points aren't already connected.
                foreach (int[] i in engine.map.Obs.Lines) {
                    if (i[0] == idx && i[1] == curpoint ||
                        i[1] == idx && i[0] == curpoint)
                        return;				// yup, already connected
                }

                // obviously not.  Connect them, then.
                engine.map.Obs.Lines.Add(new int[] { idx, curpoint });
            }

            curpoint = idx;
        }

        void RightClick(int x, int y, MouseEventArgs e) {
            if (rightdown) {
                return;
            }
            rightdown = true;

            if (!snap) {
                engine.map.Obs.Points.Add(new Import.Geo.Point(x, y));
            } else {
                //snap to corners of tiles
                if (x % 16 <= 8) while (x % 16 != 0) x--;
                else while (x % 16 != 0) x++;
                if (y % 16 <= 8) while (y % 16 != 0) y--;
                else while (y % 16 != 0) y++;


                engine.map.Obs.Points.Add(new Import.Geo.Point(x, y));
            }
        }

        public void MouseDown(Microsoft.Xna.Framework.Point e) {
        }

        public void MouseUp(Microsoft.Xna.Framework.Point e) {
#if true
            leftdown = false;
#else
            switch (e.Button) {
                case MouseButtons.Left: leftdown = false; break;
                case MouseButtons.Right: rightdown = false; break;
            }
#endif
        }

        public void MouseClick(Microsoft.Xna.Framework.Point e) {
            int x = e.X + engine.XWin;
            int y = e.Y + engine.YWin;

#if true
            LeftClick(x, y);
#else
            if (e.Button == MouseButtons.Left)
                LeftClick(x, y, e);
            else if (e.Button == MouseButtons.Right)
                RightClick(x, y, e);
#endif
        }

        public unsafe void KeyPress(KeyEventArgs e) {
            int size = 1;
            if ((Control.ModifierKeys & Keys.Shift) != 0)
                size = 10;

            switch (e.KeyCode) {
                case Keys.NumPad8: engine.map.Obs.Points[curpoint].Y -= size; break;
                case Keys.NumPad2: engine.map.Obs.Points[curpoint].Y += size; break;
                case Keys.NumPad4: engine.map.Obs.Points[curpoint].X -= size; break;
                case Keys.NumPad6: engine.map.Obs.Points[curpoint].X += size; break;
                case Keys.Delete: engine.map.Obs.RemovePoint(curpoint); curpoint = 0; break;
            }
        }

        public void MouseWheel(Microsoft.Xna.Framework.Point p, int delta) {
        }

        public void RenderHUD() {
#if false
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
            Gl.glColor4f(1, 1, 1, 1);
            Gl.glTranslatef(-engine.XWin, -engine.YWin, 0);

            Gl.glBegin(Gl.GL_LINES);

            Import.PointCollection points = engine.map.Obs.Points;
            foreach (int[] p in engine.map.Obs.lines) {
                Gl.glVertex2i(points[p[0]].X, points[p[0]].Y);
                Gl.glVertex2i(points[p[1]].X, points[p[1]].Y);
            }
            Gl.glEnd();

            Gl.glPointSize(5);
            Gl.glBegin(Gl.GL_POINTS);
            foreach (Point p in points) {
                if (p == (Point)points[curpoint]) {
                    Gl.glColor3f(1, 1, 1);
                } else {
                    Gl.glColor3f(0, 1, 0);
                }
                Gl.glVertex2i(p.X, p.Y);
            }
            Gl.glEnd();

            Gl.glTranslatef(engine.XWin, engine.YWin, 0);
#endif
        }
    }
}
