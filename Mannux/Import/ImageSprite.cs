using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Import {

    public sealed class ImageSprite {
        public static Sprite Load(string fname) {

            FileStream fs = new FileStream(fname, FileMode.Open);
            StreamReader txt = new StreamReader(fs);

            string bmpname = txt.ReadLine();
            int width = System.Convert.ToInt32(txt.ReadLine());
            int height = System.Convert.ToInt32(txt.ReadLine());
            int hotx = System.Convert.ToInt32(txt.ReadLine());
            int hoty = System.Convert.ToInt32(txt.ReadLine());
            int hotw = System.Convert.ToInt32(txt.ReadLine());
            int hoth = System.Convert.ToInt32(txt.ReadLine());
            int framesperrow = System.Convert.ToInt32(txt.ReadLine());
            int numframes = System.Convert.ToInt32(txt.ReadLine());

            fs.Close();

            Bitmap b = new Bitmap(bmpname);
            Sprite s = new Sprite();
            s.width = width;
            s.height = height;
            s.hotspot = new Rectangle(hotx, hoty, hotw, hoth);
            const int pad = 1;

            int curx = pad;
            int cury = pad;

            int xinc = width + pad;
            int yinc = height + pad;
            int curcol = 0;

            for (int i = 0; i < numframes; i++) {
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                Graphics gfx = Graphics.FromImage(bmp);
                gfx.DrawImage(b, -curx, -cury, b.Width, b.Height);
                gfx.Dispose();

                curx += xinc;

                curcol++;
                if (curcol == framesperrow) {
                    curcol = 0;
                    curx = pad;
                    cury += yinc;
                }

                s.AppendFrame(bmp);
            }

            return s;
        }
    }
}
