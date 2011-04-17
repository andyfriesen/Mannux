using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Import {
    //! Represents a map
    [Serializable]
    public class Map {
        //! Represents one map layer.
        [Serializable]
        public class Layer {
            int[,] tiles;
            string name;

            internal float parx, pary;

            public bool visible;	// if true, the layer is rendered

            internal Layer(int width, int height, string n) {
                name = n;
                tiles = new int[width, height];
                parx = pary = 1;
                visible = true;
            }

            public int Width {
                get { return tiles.GetUpperBound(0); }
            }

            public int Height {
                get { return tiles.GetUpperBound(1); }
            }

            public float ParallaxX { get { return parx; } }

            public float ParallaxY { get { return pary; } }

            public string Name {
                get { return name; }
                set { name = value; }
            }

            // TODO: range checking
            public int this[int x, int y] {
                get {
                    if (x >= tiles.GetLength(0) || x < 0) return 0;
                    if (y >= tiles.GetLength(1) || y < 0) return 0;

                    return tiles[x, y];
                }
                set {
                    if (x >= tiles.GetLength(0) || x < 0) return;
                    if (y >= tiles.GetLength(1) || y < 0) return;
                    tiles[x, y] = value;
                }
            }

            public void Resize(int newx, int newy) {
                int[,] newdata = new int[newx, newy];
                int sx = newx;
                int sy = newy;
                if (sx > Width) sx = Width;
                if (sy > Height) sy = Height;

                for (int y = 0; y < sy; y++)
                    for (int x = 0; x < sx; x++)
                        newdata[x, y] = tiles[x, y];

                tiles = newdata;
            }
        }

        ArrayList layers;
        ArrayList entities;
        VectorIndexBuffer obs;
        int width, height;

        public string vspname, musicname, renderstring;

        public Map(int x, int y) {
            width = x;
            height = y;
            layers = new ArrayList();
            entities = new ArrayList();
            obs = new VectorIndexBuffer();
        }

        public Map()
            : this(100, 100) { }

        //! Creates a new map layer, and returns it
        public Layer AddLayer() {
            return AddLayer(NumLayers);
        }

        //! Adds a layer before the specified layer index and returns it.
        public Layer AddLayer(int idx) {
            Layer l = new Layer(width, height, "New Layer " + NumLayers);
            layers.Insert(idx, l);

            return l;
        }

        //! Creates a new map layer, with the given tile data.
        internal Layer AddLayer(int[] data) {
            Layer l = new Layer(width, height, "New Layer " + NumLayers);

            int i = 0;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++) {
                    l[x, y] = data[i++];
                }

            layers.Add(l);
            return l;
        }

        //! Removes a map layer, and returns it.
        public Layer RemoveLayer(int idx) {
            if (idx < 0 || idx >= layers.Count)
                return null;

            Layer l = (Layer)layers[idx];
            layers.RemoveAt(idx);

            return l;
        }

        public void SwapLayers(int a, int b) {
            if (a < 0 || a >= NumLayers) return;
            if (b < 0 || b >= NumLayers) return;

            object o = layers[a];
            layers[a] = layers[b];
            layers[b] = o;
        }

        public void Resize(int x, int y) {
            foreach (Layer l in layers)
                l.Resize(x, y);

            width = x;
            height = y;
        }

        //! Returns the specified layer.
        // TODO phase this out.
        public Layer this[int idx] {
            get {
                if (idx < 0 || idx >= layers.Count)
                    return null;

                return (Layer)layers[idx];
            }
        }

        public ArrayList Layers { get { return layers; } }

        public int NumLayers { get { return layers.Count; } }

        public int Width { get { return width; } }

        public int Height { get { return height; } }

        public VectorIndexBuffer Obs { get { return obs; } }

        public ArrayList Entities { get { return entities; } }
    }
}
