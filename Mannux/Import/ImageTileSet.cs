using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Import {
    public class ImageTileSet {
        // grah.  Need to figure out some way to make this faster. -_-;
        public unsafe static TileSet Load(string fname, int tilex, int tiley, int numtiles) {
            const int pad = 1;

            Bitmap bmp = new Bitmap(fname);

            BitmapData data = bmp.LockBits(
                            new Rectangle(0, 0, bmp.Width, bmp.Height),
                            ImageLockMode.ReadOnly,
                            PixelFormat.Format32bppArgb
                            );

            TileSet t = new TileSet();

            int curx = pad;
            int cury = pad;

            int xinc = tilex + pad;
            int yinc = tiley + pad;

            for (int curtile = 0; curtile < numtiles; curtile++) {
                Bitmap b = new Bitmap(tilex, tiley, PixelFormat.Format32bppArgb);
                BitmapData data2 = b.LockBits(
                                            new Rectangle(0, 0, b.Width, b.Height),
                                            ImageLockMode.WriteOnly,
                                            PixelFormat.Format32bppArgb
                                            );

                UInt32* src = ((UInt32*)data.Scan0.ToPointer()) + (cury * data.Stride / 4) + curx;

                UInt32* dest = (UInt32*)data2.Scan0.ToPointer();

                for (int y = 0; y < tiley; y++) {
                    for (int x = 0; x < tilex; x++) {
                        *dest++ = *src++;	// HURRAY FOR MEMCPY
                    }

                    src += data.Stride / 4 - tilex;
                }

                b.UnlockBits(data2);

                t.AppendTile(b);

                curx += xinc;
                if (curx >= bmp.Width) {
                    curx = pad;
                    cury += yinc;
                }
            }

            bmp.UnlockBits(data);
            bmp.Dispose();

            return t;
        }
    }
}
