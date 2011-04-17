using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Import {
    using Geo;

    public sealed class MannuxMap {
        const string mapsig = "Mannux Map wee!";

        static string ReadString(BinaryReader file) {
            string s = "";

            do {
                char c = (char)file.ReadByte();

                if (c == 0) break;

                s += c;
            } while (true);

            return s;
        }

        public static Map Load(Stream s) {
            BinaryReader file = new BinaryReader(s);

            string sig = ReadString(file);
            if (sig != mapsig) {
                throw new Exception("Not a Mannux map!");
            }

            Map map = new Map();

            int width = file.ReadInt32();
            int height = file.ReadInt32();

            map.Resize(width, height);

            int numlayers = file.ReadInt32();

            int[] layerdata = new int[width * height];

            for (int i = 0; i < numlayers; i++) {
                // I wish there was a more efficient way to do this. -_-;
                for (int j = 0; j < width * height; j++) {
                    layerdata[j] = file.ReadInt32();
                }
                // ---

                map.AddLayer(layerdata);

                map[i].parx = (float)file.ReadDouble();
                map[i].pary = (float)file.ReadDouble();
                map[i].Name = ReadString(file);
            }

            int numents = file.ReadInt32();

            for (int i = 0; i < numents; i++) {
                MapEnt e = new MapEnt();

                e.x = file.ReadInt32();
                e.y = file.ReadInt32();
                e.type = ReadString(file);

                int numprops = file.ReadInt32();

                e.data = new string[numprops];
                for (int j = 0; j < numprops; j++)
                    e.data[j] = ReadString(file);

                map.Entities.Add(e);
            }

            int numpoints = file.ReadInt32();

            for (int i = 0; i < numpoints; i++) {
                int x = file.ReadInt32();
                int y = file.ReadInt32();

                map.Obs.Points.Add(new Point(x, y));
            }

            int numsegments = file.ReadInt32();

            for (int i = 0; i < numsegments; i++) {
                int p1 = file.ReadInt32();
                int p2 = file.ReadInt32();

                map.Obs.Lines.Add(new int[] { p1, p2 });
            }

            return map;

        }

        //-----

        static void WriteString(BinaryWriter file, string s) {
            foreach (char c in s) {
                byte b = (byte)c;
                file.Write(b);
            }

            file.Write((byte)0);
        }

        public static void Save(Map map, Stream s) {
            BinaryWriter file = new BinaryWriter(s);

            WriteString(file, mapsig);

            file.Write(map.Width);
            file.Write(map.Height);

            file.Write(map.Layers.Count);
            foreach (Map.Layer lay in map.Layers) {
                for (int y = 0; y < map.Height; y++) {
                    for (int x = 0; x < map.Width; x++)
                        file.Write(lay[x, y]);
                }

                file.Write((double)lay.parx);
                file.Write((double)lay.pary);
                WriteString(file, lay.Name);
            }

            file.Write(map.Entities.Count);

            foreach (MapEnt ent in map.Entities) {
                file.Write(ent.x);
                file.Write(ent.y);
                WriteString(file, ent.type);
                if (ent.data != null) {
                    file.Write(ent.data.Length);
                    foreach (string str in ent.data)
                        WriteString(file, str);
                } else
                    file.Write((int)0);
            }

            file.Write(map.Obs.Points.Count);
            foreach (Point p in map.Obs.Points) {
                file.Write(p.X);
                file.Write(p.Y);
            }

            file.Write(map.Obs.Lines.Count);
            foreach (int[] i in map.Obs.Lines) {
                file.Write(i[0]);
                file.Write(i[1]);
            }
        }
    }
}
