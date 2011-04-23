using System.Windows.Forms;
using Import;
using Microsoft.Xna.Framework;

namespace Editor {
    class EntityEditMode : IEditorState {
        Editor editor;
        Engine engine;

        MapEnt curent;

        public EntityEditMode(Editor e) {
            editor = e;
            engine = e.engine;
        }

        // returns the ent under the cursor, or null if there isn't one
        MapEnt FindEnt(int x, int y) {
            const int width = 16;
            const int height = 16;

            foreach (MapEnt e in engine.map.Entities) {
                // for now, assume all entities are 16x16 since we don't know how big their hotspots are
                // get smart and change this later

                if (e.x > x || e.y > y) continue;
                if (x > e.x + width || y > e.y + height) continue;
                return e;
            }

            return null;
        }

        public void MouseDown(Point e) {
        }

        public void MouseUp(Point e) {

        }

        public void MouseClick(Point e) {
            int x = e.X + engine.XWin;
            int y = e.Y + engine.YWin;

            if ((Control.ModifierKeys & Keys.Shift) != 0) {
                // create a new entity
                MapEnt ent = new MapEnt();
                ent.type = "Ripper";
                ent.x = x;
                ent.y = y;

                engine.map.Entities.Add(ent);

                CurEnt = ent;
            } else {
                MapEnt ent = FindEnt(x, y);

                if (ent == null)
                    return;

                CurEnt = ent;
            }
        }

        public void KeyPress(KeyEventArgs e) {
            if (CurEnt == null)
                return;

            int size = 1;	// number of pixels to move
            if ((Control.ModifierKeys & Keys.Shift) != 0)
                size = 10;

            switch (e.KeyCode) {
                case Keys.NumPad8: CurEnt.y -= size; break;
                case Keys.NumPad2: CurEnt.y += size; break;
                case Keys.NumPad4: CurEnt.x -= size; break;
                case Keys.NumPad6: CurEnt.x += size; break;

                case Keys.Delete: engine.map.Entities.Remove(CurEnt); CurEnt = null; break;
            }
        }

        public MapEnt CurEnt {
            get { return curent; }
            set {
                editor.mapentpropertiesview.UpdateEnt(curent);
                editor.mapentpropertiesview.UpdateDlg(value);

                curent = value;
            }
        }

        public void RenderHUD() {
#if false
            Gl.glTranslatef(-engine.XWin, -engine.YWin, 0);

            foreach (Import.MapEnt e in engine.map.Entities) {
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);

                if (e == CurEnt)
                    Gl.glColor4f(1, 1, 1, 1);
                else
                    Gl.glColor4f(0, 0, 1, 1);

                Gl.glBegin(Gl.GL_QUADS);
                Gl.glVertex2i(e.x, e.y);
                Gl.glVertex2i(e.x + 16, e.y);
                Gl.glVertex2i(e.x + 16, e.y + 16);
                Gl.glVertex2i(e.x, e.y + 16);
                Gl.glEnd();
            }

            Gl.glTranslatef(engine.XWin, engine.YWin, 0);
#endif
        }

        public void MouseWheel(Point p, int delta) {
        }
    }
}
