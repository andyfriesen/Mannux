using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mannux;

class ObstructionTileset {
    public int FirstTileID; // comes from TileEd

    private const int F = 16;
    private readonly Point[][] vectors = {
        new Point[]{},
        new Point[]{new Point(0, 0), new Point(F, 0), new Point(F, F), new Point(0, F) },
        new Point[]{new Point(0, 0), new Point(F, F) },
        new Point[]{new Point(F, 0), new Point(0, F) },
        new Point[]{new Point(0, 0), new Point(F, 8), new Point(F, F), new Point(0, F)},
        new Point[]{new Point(0, 8), new Point(F, F), new Point(0, F)},
        new Point[]{new Point(0, F), new Point(F, 8), new Point(F, F)},
        new Point[]{new Point(0, 8), new Point(F, 0), new Point(F, F), new Point(F, 0)}
    };

    private readonly Line[][] lines;

    public ObstructionTileset() {
        lines = new Line[vectors.Length][];
        for (var i = 0; i < vectors.Length; ++i) {
            var pa = vectors[i];
            var l = pa.Length > 0
                ? new Line[pa.Length - 1]
                : new Line[0]
            ;
            lines[i] = l;
            for (var j = 0; j < pa.Length - 1; ++j) {
                l[j] = new Line(pa[j], pa[j + 1]);
            }
        }
    }

    public Line[] LinesForTile(int tileIndex) {
        tileIndex -= FirstTileID;
        if (tileIndex < 0 || tileIndex >= vectors.Length) {
            return null;
        }
        Point intercept = Point.Zero;
        return lines[tileIndex];
    }

    public Line? Test(int tileIndex, Rectangle r) {
        var v = LinesForTile(tileIndex);
        if (v != null) {
            Point intercept = Point.Zero;
            foreach (var l in v) {
                if (l.Touches(r, ref intercept)) {
                    return l;
                }
            }
        }

        return null;
    }
}
