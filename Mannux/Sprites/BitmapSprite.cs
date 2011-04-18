/*
 * The most common type of sprite.  One that's a bunch of images.  Each "frame" is its own image.
 */

using Cataract;

using Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sprites {

    class BitmapSprite {
        int width;
        int height;
        int rowLength;
        Rectangle hotspot;
        Texture2D tex;
        XNAGraph graph;

        public BitmapSprite(XNAGraph g, string assetName, int cellWidth, int cellHeight, int rowLength, Rectangle hotspot) {
            graph = g;

            tex = g.LoadImage(assetName);
            this.width = cellWidth;
            this.height = cellHeight;
            this.rowLength = rowLength;
            this.hotspot = hotspot;
        }

        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public Rectangle HotSpot { get { return hotspot; } }

        public virtual void Draw(int x, int y, int frame) {
            var row = frame / rowLength;
            var col = frame % rowLength;
            var left = col * (width + 1);
            var top = row * (height + 1);
            var slice = new Rectangle(1 + left, 1 + top, width, height);
            graph.Blit(tex, new Vector2(x, y), slice);
        }
    }

}
