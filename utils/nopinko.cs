// Mask color removing utility.
// A bit hackish, but it's a crappy utility anyway.
// - Ear

using System;
using System.Drawing;

class App
{
    static void Main(string[] args)
    {
        Color maskcolor;

        if (args.Length == 2)
            maskcolor = Color.FromArgb(255, 0, 255);
        else if (args.Length == 3) {
            string[] channels = args[0].Split(',');

            maskcolor = Color.FromArgb(
                Convert.ToByte(channels[0]),
                Convert.ToByte(channels[1]),
                Convert.ToByte(channels[2])
            );
        }
        else {
            Console.WriteLine("syntax: infile.img outfile.img [maskcolor=255,0,255]");
            return;
        }

        Bitmap infile = new Bitmap(args[0]);
        infile.MakeTransparent(maskcolor);
        infile.Save(args[1]);
    }
}
