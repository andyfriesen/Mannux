using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

//! Represents a tileset.

namespace Import {
    public class TileSetEnumerator : IEnumerator {
        TileSet tiles;
        int i = -1;

        internal TileSetEnumerator(TileSet t) {
            tiles = t;
        }

        public object Current {
            get { return tiles[i]; }
            set { tiles[i] = (Bitmap)value; }
        }

        public bool MoveNext() {
            i++;
            return i < tiles.NumTiles;
        }

        public void Reset() {
            i = 0;
        }
    }

    public class TileSet : IEnumerable {
        ArrayList tiles;		//!< Array of Bitmaps; used for tile images
        internal byte[] palette;			//!< Palette. ;P
        int tilex, tiley;		//!< Tile dimensions

        internal TileSet()
            : this(16, 16) { }

        internal TileSet(int x, int y) {
            tilex = x;
            tiley = y;
            tiles = new ArrayList();
            palette = new byte[768];
        }

        public IEnumerator GetEnumerator() {
            return new TileSetEnumerator(this);
        }

        public Bitmap InsertTile(int idx) {
            return InsertTile(new Bitmap(tilex, tiley, PixelFormat.Format32bppArgb), idx);
        }

        public Bitmap AppendTile() {
            return AppendTile(new Bitmap(tilex, tiley, PixelFormat.Format32bppArgb));
        }

        //! Inserts a tile into the tileset and returns it.
        public Bitmap InsertTile(Bitmap b, int idx) {
            if (idx < 0 || idx > tiles.Count)
                return null;

            tiles.Insert(idx, b);

            return b;
        }

        //! Appends a new tile at the end of the tileset, and returns it.
        public Bitmap AppendTile(Bitmap b) {
            tiles.Add(b);

            return b;
        }

        //! Removes a tile from the tileset, and returns it.
        public Bitmap RemoveTile(int idx) {
            if (idx < 0 || idx >= tiles.Count)
                return null;

            Bitmap b = (Bitmap)tiles[idx];
            tiles.RemoveAt(idx);

            return b;
        }

        //! Get/set a given tile
        public Bitmap this[int idx] {
            get { return (Bitmap)tiles[idx]; }
            set { tiles[idx] = value; }
        }

        public int Width {
            get { return tilex; }
        }

        public int Height {
            get { return tiley; }
        }

        public int NumTiles {
            get { return tiles.Count; }
        }

        void Save(string fname) {
            // TODO: implement this
        }

        public byte[] Palette {
            get { return palette; }
        }
    }
}
