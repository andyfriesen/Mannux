using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Import {
    public sealed class v2Sprite {
        private unsafe static void CreateFramesFromBuffer(Sprite sprite, byte[] pixeldata, byte[] pal, int width, int height, int numframes) {
            int idx = 0;

            for (int curframe = 0; curframe < numframes; curframe++) {
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                BitmapData data = bitmap.LockBits(
                        new Rectangle(0, 0, width, height),
                        ImageLockMode.WriteOnly,
                        PixelFormat.Format32bppArgb
                        );

                Int32* temp = (Int32*)data.Scan0.ToPointer();

                // First, convert data to 32bpp ARGB
                for (int i = 0; i < width * height; i++) {
                    byte c = pixeldata[idx++];

                    if (c != 0)
                        temp[i] =
                                (255 << 24) |
                                pal[c * 3] << 18 |
                                pal[c * 3 + 1] << 10 |
                                pal[c * 3 + 2] << 2;
                    else
                        temp[i] = 0;
                }

                bitmap.UnlockBits(data);

                // Second, add it to the list.
                sprite.AppendFrame(bitmap);
            }
        }

        public static Sprite Load(string filename) {
            return Load(filename, Palette.vergepal);
        }

        public static Sprite Load(string filename, byte[] palette) {
            Sprite sprite = new Sprite();

            FileStream f = new FileStream(filename, FileMode.Open);
            BinaryReader stream = new BinaryReader(f, System.Text.ASCIIEncoding.ASCII);

            byte version = stream.ReadByte();
            if (version != 2)
                return null;	// TODO: implement other versions?

            sprite.width = stream.ReadUInt16();
            sprite.height = stream.ReadUInt16();
            UInt16 hotx = stream.ReadUInt16();
            UInt16 hoty = stream.ReadUInt16();
            UInt16 hotw = stream.ReadUInt16();
            UInt16 hoth = stream.ReadUInt16();

            sprite.hotspot = new Rectangle(hotx, hoty, hotw, hoth);

            UInt16 numframes = stream.ReadUInt16();

            int bufsize = stream.ReadInt32();

            byte[] cbuffer = new byte[bufsize];
            byte[] data = new byte[sprite.width * sprite.height * numframes];

            stream.Read(cbuffer, 0, bufsize);

            RLE.Read(data, sprite.width * sprite.height * numframes, cbuffer);

            CreateFramesFromBuffer(sprite, data, palette, sprite.width, sprite.height, numframes);

            stream.Close();
            f.Close();

            return sprite;
        }
    }
}
