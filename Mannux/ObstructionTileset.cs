using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Squared.Tiled;
using Mannux;

class ObstructionTileset {
    public int FirstTileID; // comes from TileEd

    private const int F = 16;
    private readonly Point[][] vectors = {
        new Point[]{},
        new Point[]{new Point(0, 0), new Point(F, 0), new Point(F, F), new Point(0, F) },
        new Point[]{new Point(0, 0), new Point(F, F), new Point(0, F) },
        new Point[]{new Point(F, 0), new Point(0, F), new Point(F, F) },
        new Point[]{new Point(0, 0), new Point(F, 8), new Point(F, F), new Point(0, F)},
        new Point[]{new Point(0, 8), new Point(F, F), new Point(0, F)},
        new Point[]{new Point(0, F), new Point(F, 8), new Point(F, F)},
        new Point[]{new Point(0, 8), new Point(F, 0), new Point(F, F), new Point(F, 0)}
    };

    private readonly Line[][] lines;
    public int tileWidth;
    public int tileHeight;
    public Layer Layer;

    public ObstructionTileset() {
        tileWidth = 1;
        tileHeight = 1;
        Layer = null;

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

    public Point[] PointsForTile(int tileIndex) {
        tileIndex -= FirstTileID;
        if (tileIndex < 0 || tileIndex >= vectors.Length) {
            return null;
        }
        return vectors[tileIndex];
    }

    public Line[] LinesForTile(int tileIndex) {
        tileIndex -= FirstTileID;
        if (tileIndex < 0 || tileIndex >= vectors.Length) {
            return null;
        }
        return lines[tileIndex];
    }

    public Line? Test(int tileIndex, Rectangle r) {
        var v = LinesForTile(tileIndex);
        if (v != null) {
            Point intercept = Point.Zero;
            foreach (var l in v) {
                if (l.Touches(r, out intercept)) {
                    return l;
                }
            }
        }

        return null;
    }

    public void Test(Rect r, Action<Line> cb) {
        var ty = (int)(r.Top / tileHeight);
        var tey = (int)(r.Bottom / tileHeight);
        var tx = (int)(r.Left / tileWidth);
        var tex = (int)(r.Right / tileWidth);

        if (ty < 0) {
            cb(new Line(0, 0, Layer.Width, 0));
            return;
        }
        if (ty >= Layer.Height) {
            cb(new Line(0, Layer.Height, Layer.Width, Layer.Height));
            return;
        }
        if (tx < 0) {
            cb(new Line(0, 0, 0, Layer.Height));
            return;
        }
        if (tx >= Layer.Width) {
            cb(new Line(Layer.Width, 0, Layer.Width, Layer.Height));
            return;
        }

        for (var y = ty; y <= tey; ++y) {
            for (var x = tx; x <= tex; ++x) {
                var tileSpaceRect = r;
                tileSpaceRect.Offset(-x * tileWidth, -y * tileHeight);

                var lines = LinesForTile(Layer.GetTile(x, y));
                if (lines == null) {
                    continue;
                }

                foreach (var l in lines) {
                    var q = l;
                    q.Offset(x * tileWidth, y * tileHeight);
                    cb(q);
                }
            }
        }
    }

    public IEnumerable<Point[]> GetObsTiles(Rect r) {
        var ty = (int)(r.Top / tileHeight);
        var tey = (int)(r.Bottom / tileHeight);
        var tx = (int)(r.Left / tileWidth);
        var tex = (int)(r.Right / tileWidth);

        if (ty < 0) {
            yield break;
        }
        if (ty >= Layer.Height) {
            yield break;
        }
        if (tx < 0) {
            yield break;
        }
        if (tx >= Layer.Width) {
            yield break;
        }

        for (var y = ty; y <= tey; ++y) {
            for (var x = tx; x <= tex; ++x) {
                var p = PointsForTile(Layer.GetTile(x, y));

                if (p != null) {
                    var pc = new Point[p.Length];
                    for (var i = 0; i < p.Length; ++i) {
                        pc[i].X = p[i].X + x * tileWidth;
                        pc[i].Y = p[i].Y + y * tileHeight;
                    }
                    yield return pc;
                }
            }
        }
    }

    // FIXME: We only ever need to have like 20 instances of this class.
    // TODO: Construct them all at startup instead of every frame.
    public class Shape {
        public Shape(Vector2[] vertices) {
            this.vertices = vertices;
            center = computeCenter();
            axes = computeAxes();
        }

        public Shape(Point[] points) {
            vertices = new Vector2[points.Length];
            for (var i = 0; i < vertices.Length; ++i) {
                vertices[i] = new Vector2(points[i].X, points[i].Y);
            }
            center = computeCenter();
            axes = computeAxes();
        }

        public Shape(Rect r)
            : this(new Vector2[]{
                    new Vector2(r.Left, r.Top),
                    new Vector2(r.Right, r.Top),
                    new Vector2(r.Right, r.Bottom),
                    new Vector2(r.Left, r.Bottom)
                }
        ) { }

        private Vector2[] computeAxes() {
            var result = new Vector2[vertices.Length];
            for (var i = 0; i < result.Length; ++i) {
                var a = vertices[i];
                var b = vertices[(i + 1) % result.Length];

                var c = a - b;
                c.Normalize();

                var normal = new Vector2(-c.Y, c.X);
                result[i] = normal;
            }
            return result;
        }

        private Vector2 computeCenter() {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);
            for (var i = 0; i < vertices.Length; ++i) {
                var v = vertices[i];
                min.X = Math.Min(v.X, min.X);
                min.Y = Math.Min(v.Y, min.Y);
                max.X = Math.Max(v.X, max.X);
                max.Y = Math.Max(v.X, max.Y);
            }
            return new Vector2((min.X + max.X) / 2, (min.Y + max.Y) / 2);
        }

        public Projection Project(Vector2 a) {
            float min = Vector2.Dot(a, vertices[0]);
            float max = min;
            for (var i = 1; i < vertices.Length; ++i) {
                var p = Vector2.Dot(a, vertices[i]);
                if (p < min) {
                    min = p;
                }
                if (p > max) {
                    max = p;
                }
            }

            return new Projection(min, max);
        }

        public readonly Vector2[] vertices;
        public readonly Vector2[] axes;
        public readonly Vector2 center;
    }

    public struct Projection {
        public Projection(float min, float max) {
            this.min = min;
            this.max = max;
        }

        private readonly float min, max;

        public float Overlap(Projection other) {
            if (!(max > other.min && min < other.max)) {
                return 0;
            }

            return Math.Min(max, other.max) - Math.Max(min, other.min);
        }
    }

    public struct CollisionResult {
        public readonly Shape collidingShape;
        public readonly Vector2 mtv;

        public CollisionResult(Shape s, Vector2 mtv) {
            this.collidingShape = s;
            this.mtv = mtv;
        }
    }

    public CollisionResult? TestSAT(Rect r) {
        var entityShape = new Shape(r);
        var obstructions = new List<Shape>();

        foreach (var l in GetObsTiles(r)) {
            if (l != null) {
                var s = new Shape(l);
                obstructions.Add(s);
            }
        }

        foreach (var s in obstructions) {
            var mtv = TestShapes(entityShape, s);
            if (mtv.HasValue) {
                return new CollisionResult(s, mtv.Value);
            }
        }
        return null;
    }

    private Vector2? TestShapes(Shape s1, Shape s2) {
        var smallestOverlap = float.MaxValue;
        Vector2 bestAxis = Vector2.Zero;

        foreach (var a in s1.axes) {
            var overlap = TestAxis(a, s1, s2);

            if (overlap == 0) {
                return null;
            }

            if (overlap < smallestOverlap) {
                smallestOverlap = overlap;
                bestAxis = a;
            }
        }

        foreach (var a in s2.axes) {
            var overlap = TestAxis(a, s1, s2);

            if (overlap == 0) {
                return null;
            }

            if (overlap < smallestOverlap) {
                smallestOverlap = overlap;
                bestAxis = a;
            }
        }

        if (s1.center.X < s2.center.X) {
            bestAxis.X = -Math.Abs(bestAxis.X);
        } else {
            bestAxis.X = Math.Abs(bestAxis.X);
        }

        if (s1.center.Y < s2.center.Y) {
            bestAxis.Y = -Math.Abs(bestAxis.Y);
        } else {
            bestAxis.Y = Math.Abs(bestAxis.Y);
        }

        return bestAxis * smallestOverlap;
    }

    private float TestAxis(Vector2 axis, Shape s1, Shape s2) {
        var p1 = s1.Project(axis);
        var p2 = s2.Project(axis);
        return p1.Overlap(p2);
    }
}
