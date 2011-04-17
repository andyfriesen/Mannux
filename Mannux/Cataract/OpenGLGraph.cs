
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Cataract {
    //! An OpenGL based graphics renderer.
    public class OpenGLForm : Form, IGraph {
        int xres;							//!< Width of the viewport
        int yres;							//!< Height of the viewport
        int bpp;							//!< Pixel depth of the viewport.
        bool windowed;						//!< if true, the viewport is contained in a window, and does not occupy the whole screen.

        /*!
         * Default constructor.
         * \param x Width of the desired viewport, in pixels.
         * \param y Height of the desired viewport, in pixels.
         * \param b Desired pixel depth, in bits.
         * \param w if true, the viewport will be contained in a window.  If false, the viewport will occupy the whole screen.
         */
        public OpenGLForm(int x, int y, int b, bool w) {
            xres = x; yres = y;
            bpp = b; windowed = w;

            ClientSize = new System.Drawing.Size(x, y);

            MaximizeBox = false;
            KeyPreview = true;

            stuff();
            InitGL();

            Show();
        }

        private IntPtr hDC;
        private IntPtr hRC;

        private void stuff() {
            var form = this;
            var fullscreen = false;
            var bits = 32;

            if (fullscreen) {                                                    // Are We Still In Fullscreen Mode?
                form.FormBorderStyle = FormBorderStyle.None;                    // No Border
                Cursor.Hide();                                                  // Hide Mouse Pointer
            } else {                                                              // If Windowed
                form.FormBorderStyle = FormBorderStyle.Sizable;                 // Sizable
                Cursor.Show();                                                  // Show Mouse Pointer
            }

            Gdi.PIXELFORMATDESCRIPTOR pfd = new Gdi.PIXELFORMATDESCRIPTOR();    // pfd Tells Windows How We Want Things To Be
            pfd.nSize = (short)Marshal.SizeOf(pfd);                            // Size Of This Pixel Format Descriptor
            pfd.nVersion = 1;                                                   // Version Number
            pfd.dwFlags = Gdi.PFD_DRAW_TO_WINDOW |                              // Format Must Support Window
                Gdi.PFD_SUPPORT_OPENGL |                                        // Format Must Support OpenGL
                Gdi.PFD_DOUBLEBUFFER;                                           // Format Must Support Double Buffering
            pfd.iPixelType = (byte)Gdi.PFD_TYPE_RGBA;                          // Request An RGBA Format
            pfd.cColorBits = (byte)bits;                                       // Select Our Color Depth
            pfd.cRedBits = 0;                                                   // Color Bits Ignored
            pfd.cRedShift = 0;
            pfd.cGreenBits = 0;
            pfd.cGreenShift = 0;
            pfd.cBlueBits = 0;
            pfd.cBlueShift = 0;
            pfd.cAlphaBits = 0;                                                 // No Alpha Buffer
            pfd.cAlphaShift = 0;                                                // Shift Bit Ignored
            pfd.cAccumBits = 0;                                                 // No Accumulation Buffer
            pfd.cAccumRedBits = 0;                                              // Accumulation Bits Ignored
            pfd.cAccumGreenBits = 0;
            pfd.cAccumBlueBits = 0;
            pfd.cAccumAlphaBits = 0;
            pfd.cDepthBits = 16;                                                // 16Bit Z-Buffer (Depth Buffer)
            pfd.cStencilBits = 0;                                               // No Stencil Buffer
            pfd.cAuxBuffers = 0;                                                // No Auxiliary Buffer
            pfd.iLayerType = (byte)Gdi.PFD_MAIN_PLANE;                         // Main Drawing Layer
            pfd.bReserved = 0;                                                  // Reserved
            pfd.dwLayerMask = 0;                                                // Layer Masks Ignored
            pfd.dwVisibleMask = 0;
            pfd.dwDamageMask = 0;

            hDC = User.GetDC(form.Handle);                                      // Attempt To Get A Device Context
            if (hDC == IntPtr.Zero) {                                            // Did We Get A Device Context?
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Create A GL Device Context.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int pixelFormat = Gdi.ChoosePixelFormat(hDC, ref pfd);                  // Attempt To Find An Appropriate Pixel Format
            if (pixelFormat == 0) {                                              // Did Windows Find A Matching Pixel Format?
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Find A Suitable PixelFormat.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Gdi.SetPixelFormat(hDC, pixelFormat, ref pfd)) {                // Are We Able To Set The Pixel Format?
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Set The PixelFormat.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            hRC = Wgl.wglCreateContext(hDC);                                    // Attempt To Get The Rendering Context
            if (hRC == IntPtr.Zero) {                                            // Are We Able To Get A Rendering Context?
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Create A GL Rendering Context.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Wgl.wglMakeCurrent(hDC, hRC)) {                                 // Try To Activate The Rendering Context
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Activate The GL Rendering Context.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            form.Show();                                                        // Show The Window
            form.TopMost = true;                                                // Topmost Window
            form.Focus();                                                       // Focus The Window

            if (fullscreen) {                                                    // This Shouldn't Be Necessary, But Is
                Cursor.Hide();
            }
            ReSizeGLScene(form.Width, form.Height);                                       // Set Up Our Perspective GL Screen

            InitGL();
        }

        private void ReSizeGLScene(int width, int height) {
        }

        private void KillGLWindow() {
        }

        //! Initializes default OpenGL states.
        private void InitGL() {
            Gl.glClearColor(0, 0, 0, 255);
            Gl.glClearDepth(1);

            Gl.glEnable(Gl.GL_SCISSOR_TEST);
            Gl.glScissor(0, 0, xres, yres);
            Gl.glDisable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glShadeModel(Gl.GL_SMOOTH);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Gl.glOrtho(0.0f, xres, yres, 0.0f, -1.0f, 1.0f);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            Gl.glEnable(Gl.GL_POINT_SMOOTH);
        }

        MouseEventArgs AdjustForZoom(MouseEventArgs e) {
            return new MouseEventArgs(e.Button, e.Clicks,
                                      e.X * xres / ClientSize.Width,
                                      e.Y * yres / ClientSize.Height,
                                      e.Delta);
        }

        void OnMouseEnter(object o, EventArgs e) { base.OnMouseEnter(e); }
        void OnMouseMove(object o, MouseEventArgs e) { base.OnMouseMove(AdjustForZoom(e)); }
        void OnMouseDown(object o, MouseEventArgs e) { base.OnMouseDown(AdjustForZoom(e)); }
        void OnMouseHover(object o, EventArgs e) { base.OnMouseHover(e); }
        void OnMouseWheel(object o, MouseEventArgs e) { base.OnMouseWheel(AdjustForZoom(e)); }
        void OnMouseUp(object o, MouseEventArgs e) { base.OnMouseUp(AdjustForZoom(e)); }
        void OnMouseLeave(object o, EventArgs e) { base.OnMouseLeave(e); }

        uint lasttex = 0;
        void SetTex(uint tex) {
            if (tex == lasttex)
                return;

            lasttex = tex;
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, tex);
        }

        public void Blit(IImage src, int x, int y, bool trans) {
            ScaleBlit(src, x, y, src.Width, src.Height, trans);
        }

        public void ScaleBlit(IImage src, int x, int y, int w, int h, bool trans) {
            OpenGLImage Src = (OpenGLImage)src;

            float nTexendx = 1.0f;//*src.Width/Src.TexWidth;
            float nTexendy = 1.0f;//*src.Height/Src.TexHeight;

            if (trans)
                Gl.glEnable(Gl.GL_BLEND);
            else
                Gl.glDisable(Gl.GL_BLEND);

            //Gl.glBindTexture(Gl.GL_TEXTURE_2D,Src.Tex);
            SetTex(Src.Tex);
            Gl.glColor4f(1, 1, 1, 1);

            Gl.glBegin(Gl.GL_QUADS);

            Gl.glTexCoord2f(0, 0); Gl.glVertex2i(x, y);
            Gl.glTexCoord2f(nTexendx, 0); Gl.glVertex2i(x + w, y);
            Gl.glTexCoord2f(nTexendx, nTexendy); Gl.glVertex2i(x + w, y + h);
            Gl.glTexCoord2f(0, nTexendy); Gl.glVertex2i(x, y + h);

            Gl.glEnd();
        }


        /*public void TintBlit(IImage src,RGBA[] colours,int x,int y,bool trans)
        {
                TintDistortBlit(
                        src,
                        colours,
                        new int[] { 0,src.Width,src.Width,0 },
                        new int[] { 0,0,src.Height,src.Height },
                        trans
                        );
        }
		
        public void TintDistortBlit(IImage src,RGBA[] colours,int[] x,int[] y,bool trans)
        {
                Image img=(Image)src;
			
                float nTexendx=1.0f;//*src.Width/Src.TexWidth;
                float nTexendy=1.0f;//*src.Height/Src.TexHeight;

                if (trans)
                        Gl.glEnable(Gl.GL_BLEND);
                else
                        Gl.glDisable(Gl.GL_BLEND);
			
                Gl.glBindTexture(Gl.GL_TEXTURE_2D,img.Tex);
			
                float[] texx=new float[] { 0,nTexendx,nTexendx,0 };
                float[] texy=new float[] { 0,0,nTexendy,nTexendy };
			
                Gl.glBegin(Gl.GL_QUADS);
			
                for (int i=0; i<4; i++)
                {
                        Gl.glColor4f(colours[i].r,colours[i].g,colours[i].b,colours[i].a);
                        Gl.glTexCoord2f(texx[i],texy[i]);
                        Gl.glVertex2f(x[i],y[i]);
                }
			
                Gl.glEnd();
        }*/

        public void DrawParticle(int x, int y, float size, byte r, byte g, byte b, byte a) {
            // this could use optimizing.  A batch point renderer would be ideal.
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
            Gl.glColor4ub(r, g, b, a);
            Gl.glPointSize(size);
            Gl.glBegin(Gl.GL_POINTS);
            Gl.glVertex2i(x, y);
            Gl.glEnd();
        }

        public void DrawLine(int x1, int y1, int x2, int y2, byte r, byte g, byte b, byte a) {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0);
            Gl.glColor4ub(r, g, b, a);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex2i(x1, y1);
            Gl.glVertex2i(x2, y2);
            Gl.glEnd();
        }

        public void ShowPage() {
            Wgl.wglSwapBuffers(hDC);
            //glcontrol.Context.SwapBuffer();
        }

        public void Clear() {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
        }

        public IImage CreateImage(int x, int y, System.IntPtr ptr) {
            return new OpenGLImage(x, y, ptr);
        }

        public IImage LoadImage(string fname) {
            return new OpenGLImage(fname);
        }

        public int XRes { get { return xres; } }
        public int YRes { get { return yres; } }

        protected override void OnResize(EventArgs e) {
            // tweak the GL stuff
            Gl.glScissor(0, 0, Width, Height);

            base.OnResize(e);
        }
    }

}
