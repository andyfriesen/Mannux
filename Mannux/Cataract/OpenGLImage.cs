// TODO: images that don't fit into happy texture sizes

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Cataract {
    internal class OpenGLImage : IImage {
        private int width, height;
        private int texwidth, texheight;
        private uint[] hTex;

        private string fname;	// if the image is to be loaded from a file
        private byte[] data;		// if the image is to be loaded from some memory

        public OpenGLImage(string filename) {
            width = height = texwidth = texheight = 0;

            fname = filename;
        }

        public unsafe OpenGLImage(int w, int h, IntPtr src) {
            fname = "";
            width = texwidth = w; height = texheight = h;

            data = new byte[w * h * 4];
            Marshal.Copy(src, data, 0, w * h * 4);
        }

        ~OpenGLImage() {
            Dispose();
        }

        internal bool Load() {
            // bleh, this is pretty gay
            hTex = new uint[1];
            //IntPtr ptr;

            if (fname != "") {
                Bitmap bmp = new Bitmap(fname);
                width = bmp.Width;
                height = bmp.Height;

                texwidth = 1; texheight = 1;
                while (texwidth < bmp.Width) texwidth <<= 1;
                while (texheight < bmp.Height) texheight <<= 1;

                // ARGH need a way to enlarge the bitmap without scaling its contents. ;P
                bmp = new Bitmap(bmp, texwidth, texheight);

                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                BitmapData pixels = bmp.LockBits(rect, ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                CopyData(bmp.Width, bmp.Height, pixels.Scan0);

                bmp.UnlockBits(pixels);
            } else {
                CopyData(width, height, data);
                data = null;	// so the GCer can clean it up at some point
            }

            return true;	// no error checking yet.
        }

        private void CopyData(int x, int y, System.IntPtr data) {
            Gl.glGenTextures(1, hTex);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, hTex[0]);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, 4, x, y, 0, Gl.GL_BGRA_EXT, Gl.GL_UNSIGNED_BYTE, data);

            width = x;
            height = y;

            texwidth = x;
            texheight = y;
        }

        private unsafe void CopyData(int x, int y, byte[] data) {
            fixed (void* p = &data[0]) {
                IntPtr ptr = new IntPtr(p);
                CopyData(x, y, ptr);
            }
        }

        // Accessors
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public uint Tex {
            get {
                if (hTex == null)
                    Load();
                return hTex[0];
            }
        }

        public int TexWidth { get { return texwidth; } }
        public int TexHeight { get { return texheight; } }

        // IDisposable

        // 0 - undisposed
        // 1 - disposing (thread safety is neat)
        // 2 - disposed
        int disposestate = 0;
        public void Dispose() {
            if (disposestate != 0)
                return;

            disposestate = 1;
            if (hTex != null)
                Gl.glDeleteTextures(1, hTex);
            hTex = null;

            disposestate = 2;
        }
    }

}
