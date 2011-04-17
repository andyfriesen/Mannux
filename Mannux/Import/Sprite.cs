using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace Import {
    public class SpriteEnumerator : IEnumerator {
        Sprite sprite;
        int idx;

        internal SpriteEnumerator(Sprite s) {
            sprite = s;
            idx = -1;
        }

        public object Current {
            get { return sprite[idx]; }
            set { sprite[idx] = (Bitmap)value; }
        }

        public bool MoveNext() {
            idx++;
            return idx < sprite.NumFrames;
        }

        public void Reset() {
            idx = 0;
        }
    }

    public class Sprite : IEnumerable {
        ArrayList frames;
        internal int width;
        internal int height;
        internal Rectangle hotspot;
        internal string filename;

        internal Sprite() {
            frames = new ArrayList();
        }

        public IEnumerator GetEnumerator() {
            return new SpriteEnumerator(this);
        }

        public Bitmap this[int idx] {
            get { return (Bitmap)frames[idx]; }
            set { frames[idx] = value; }
        }

        public Bitmap InsertFrame(int pos, Bitmap bmp) {
            frames.Insert(pos, bmp);
            return bmp;
        }

        public Bitmap AppendFrame(Bitmap bmp) {
            return InsertFrame(NumFrames, bmp);
        }

        public int Width {
            get { return width; }
        }

        public int Height {
            get { return height; }
        }

        public int NumFrames {
            get { return frames.Count; }
        }

        public string FileName {
            get { return filename; }
            set { filename = value; }
        }

        public Rectangle HotSpot {
            get { return hotspot; }
        }
    }
}
