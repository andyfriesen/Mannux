/*
 * The most common type of sprite.  One that's a bunch of images.  Each "frame" is its own image.
 */

using Cataract;

using System.Drawing;

using Sprites;

namespace Sprites {

    class BitmapSprite : ISprite {
        IImage[] frames;
        int width, height;
        Rectangle hotspot;
        IGraph graph;

        public BitmapSprite(IGraph g, Import.Sprite s) {
            graph = g;
            frames = new IImage[s.NumFrames];
            width = s.Width;
            height = s.Height;
            hotspot = s.HotSpot;

            int i = 0;
            foreach (Bitmap b in s) {
                BitmapData bd = b.LockBits(
                        new Rectangle(0, 0, b.Width, b.Height),
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppArgb);

                frames[i] = graph.CreateImage(b.Width, b.Height, bd.Scan0);

                b.UnlockBits(bd);
                i++;
            }

        }

        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public int NumFrames { get { return frames.Length; } }
        public Rectangle HotSpot { get { return hotspot; } }

        /*public IImage this[int idx]
        {
                get	{	return	frames[idx];	}
        }*/

        public virtual void Draw(int x, int y, int frame) {
            graph.Blit(frames[frame], x, y, true);
        }


        // IDisposable
        int disposestate = 0;
        public void Dispose() {
            if (disposestate != 0)
                return;

            disposestate = 1;
            foreach (IImage i in frames)
                i.Dispose();

            frames = null;

            disposestate = 2;
        }
    }

}
