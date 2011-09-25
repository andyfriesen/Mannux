using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Mannux;

namespace Mannux {
    //! An OpenGL based graphics renderer.
    class XNAGraph {
        private readonly GraphicsDevice device;
        private readonly BasicEffect effect;
        private readonly ContentManager contentManager;
        private readonly SpriteBatch spriteBatch;

        public XNAGraph(GraphicsDevice device, ContentManager contentManager) {
            this.device = device;
            this.contentManager = contentManager;
            spriteBatch = new SpriteBatch(device);

            effect = new BasicEffect(device);
            effect.Projection = Matrix.CreateOrthographicOffCenter(
                0,
                (float)device.Viewport.Width,
                (float)device.Viewport.Height,
                0,
                1.0f, 1000.0f
            );
            effect.View = Matrix.CreateLookAt(
                new Vector3(0.0f, 0.0f, 1.0f),
                Vector3.Zero,
                Vector3.Up
            );
        }

        public SpriteBatch SpriteBatch {
            get {
                return spriteBatch;
            }
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

        public void Clear() {
            device.Clear(Color.Black);
        }

        public Texture2D LoadImage(string fname) {
            return contentManager.Load<Texture2D>(fname);
        }

        private readonly VertexPositionColor[] vertices = new VertexPositionColor[5];

        public void DrawRect(Rectangle r) {
            vertices[0] = new VertexPositionColor(new Vector3(r.X, r.Y, 0), Color.White);
            vertices[1] = new VertexPositionColor(new Vector3(r.Right, r.Y, 0), Color.White);
            vertices[2] = new VertexPositionColor(new Vector3(r.Right, r.Bottom, 0), Color.White);
            vertices[3] = new VertexPositionColor(new Vector3(r.X, r.Bottom, 0), Color.White);
            vertices[4] = new VertexPositionColor(new Vector3(r.X, r.Y, 0), Color.White);

            device.DepthStencilState = DepthStencilState.None;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, 4);
            }
        }

        public void DrawLine(Line l) {
            DrawLine(l, Color.White);
        }

        public void DrawLine(Line l, Color c) {
            vertices[0] = new VertexPositionColor(new Vector3(l.A.X, l.A.Y, 0), c);
            vertices[1] = new VertexPositionColor(new Vector3(l.B.X, l.B.Y, 0), c);

            effect.VertexColorEnabled = true;
            device.DepthStencilState = DepthStencilState.None;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, 1);
            }
        }

        public int XRes { get { return device.Viewport.Width; } }
        public int YRes { get { return device.Viewport.Height; } }
    }

}
