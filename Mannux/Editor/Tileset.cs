using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Editor {
    class Tileset {
        public readonly Bitmap Bitmap;
        public readonly int TileWidth;
        public readonly int TileHeight;
        public readonly int NumTiles;
        public readonly bool Padded;
        private readonly int TilesPerRow;

        public Tileset(Bitmap bitmap, int tileWidth, int tileHeight, int tilesPerRow, int numTiles) {
            Bitmap = bitmap;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            TilesPerRow = tilesPerRow;
            NumTiles = numTiles;
            Padded = true;
        }

        public int TileFromPoint(int x, int y) {
            var xtile = TileWidth + (Padded ? 1 : 0);
            x /= xtile;
            y /= TileHeight + (Padded ? 1 : 0);

            return y * TilesPerRow + x;
        }

        public Point PointFromTile(int i) {
            int tx = TileWidth + (Padded ? 1 : 0);
            int ty = TileHeight + (Padded ? 1 : 0);

            int tilex = TilesPerRow;

            return new Point(
                (i / tilex) * ty,
                (i % tilex) * tx
            );
        }
    }
}
