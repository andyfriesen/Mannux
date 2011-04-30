using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Cataract {
    //! An OpenGL based graphics renderer.
    class XNAGraph {
        private readonly GraphicsDevice device;
        private readonly ContentManager contentManager;
        private readonly SpriteBatch spriteBatch;

        public XNAGraph(GraphicsDevice device, ContentManager contentManager) {
            this.device = device;
            this.contentManager = contentManager;
            spriteBatch = new SpriteBatch(device);
        }

        public void Begin() {
            spriteBatch.Begin();
        }

        public void End() {
            spriteBatch.End();
        }

        public void Blit(Texture2D img, Vector2 pos, Rectangle slice) {
            var destRect = new Rectangle((int)pos.X, (int)pos.Y, slice.Width, slice.Height);
            spriteBatch.Draw(img, destRect, slice, Color.White);
        }

        public void Blit(Texture2D src, int x, int y, bool trans) {
            spriteBatch.Draw(src, new Vector2(x, y), Color.White);
        }

        public void ScaleBlit(Texture2D src, int x, int y, int w, int h, bool trans) {
            spriteBatch.Draw(src, new Rectangle(x, y, w, h), Color.White);
        }

        public void DrawParticle(int x, int y, float size, byte r, byte g, byte b, byte a) {
            // wtf.  This API is dumb.
        }

        public void DrawLine(int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a) {
            // blah.
        }

        public void DrawPoints(Import.Geo.Vertex[] points, Color color) {
        }

        public void DrawPoints(VertexPositionColor[] points, Color color) {
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.PointList, points, 0, points.Length);
        }

        public void Clear() {
            device.Clear(Color.Black);
        }

        public Texture2D LoadImage(string fname) {
            return contentManager.Load<Texture2D>(fname);
        }

        public int XRes { get { return device.Viewport.Width; } }
        public int YRes { get { return device.Viewport.Height; } }
    }

}
