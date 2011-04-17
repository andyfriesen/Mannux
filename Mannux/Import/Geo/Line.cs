using System.Drawing;

namespace Import.Geo {

    public class Line {
        Point a;
        Point b;
        float slope;
        float yint;

        public Line(Point A, Point B) {
            a = A;
            b = B;

            if (a.X == b.X)
                slope = 9999999999999;
            else
                slope = (1.0f * a.Y - b.Y) / (a.X - b.X);
            yint = a.Y - slope * a.X;
        }

        public Line(int x1, int y1, int x2, int y2)
            : this(new Point(x1, y1), new Point(x2, y2)) { }

        public float Slope { get { return slope; } }
        public float YIntercept { get { return yint; } }

        public float atX(float x) {
            // y=mx+b
            return x * Slope + YIntercept;
        }

        public float atY(float y) {
            // y=mx+b
            // x=(y-b)/m
            return (y - YIntercept) / Slope;
        }

        public bool Intersects(Line l, ref Point intercept) {
            float this_m = Slope;
            float l_m = l.Slope;

            float this_b = YIntercept;
            float l_b = l.YIntercept;

            // case 1: two vertical lines
            if (a.X == b.X && l.a.X == l.b.X)
                return a.X == l.a.X;

            // case 2: 'l' is vertical.  'this' is not.
            if (l.a.X == l.b.X) {
                intercept.X = l.a.X;
                intercept.Y = (int)atX(l.a.X);
                return true;
            }

            // case 3: 'this' is vertical.  'l' is not.
            if (a.X == b.X)
                return l.Intersects(this, ref intercept);

            // case 4: the way always works unless you try to divide by zero

            // Given:
            // y           == m1*x+b1
            // y           == m2*x+b2
            //
            // m1*x+b1     == m2*x+b2
            // m1*x - m2*x == b2-b1
            // x*(m1-m2)   == b2-b1

            // Solution:
            // x           == (b2-b1) / (m1-m2)
            // y           == m1*x+b1

            if (l_m == this_m && a.X != l.a.X)	// if they're parallel, and not the same line
                return false;				// then they don't intersect anywhere at all.

            intercept.X = (int)((l_b - this_b) / (this_m - l_m));
            intercept.Y = (int)(this_m * intercept.X + this_b);

            return true;
        }

        // UGLY
        bool Touches(Line l, ref Point intercept) {
            // If the bounding boxes that the lines live in don't touch, then we don't need to do any more work.
            if (a.X < l.a.X && a.X < l.b.X && b.X < l.a.X && b.X < l.b.X)
                return false;
            if (a.X > l.a.X && a.X > l.b.X && b.X > l.a.X && b.X > l.b.X)
                return false;
            if (a.Y < l.a.Y && a.Y < l.b.Y && b.Y < l.a.Y && b.Y < l.b.Y)
                return false;
            if (a.Y > l.a.Y && a.Y > l.b.Y && b.Y > l.a.Y && b.Y > l.b.Y)
                return false;

            if (!Intersects(l, ref intercept))
                return false;

            // now we know that the lines, if extended infinitely, touch.  And we know where they touch.
            // All that remains is to verify that this point is on both line segments.

            if ((intercept.X < a.X && intercept.X < b.X) || (intercept.X > a.X && intercept.X > b.X))
                return false;

            if ((intercept.Y < a.Y && intercept.Y < b.Y) || (intercept.Y > a.Y && intercept.Y > b.Y))
                return false;

            if ((intercept.X < l.a.X && intercept.X < l.b.X) || (intercept.X > l.a.X && intercept.X > l.b.X))
                return false;

            if ((intercept.Y < l.a.Y && intercept.Y < l.b.Y) || (intercept.Y > l.a.Y && intercept.Y > l.b.Y))
                return false;

            return true;
        }

        public bool Touches(Rectangle r, ref Point intercept) {
            // bounding box type things
            if (a.X < r.Left && b.X < r.Left) return false;
            if (a.X > r.Right && b.X > r.Right) return false;
            if (a.Y < r.Top && b.Y < r.Top) return false;
            if (a.Y > r.Bottom && b.Y > r.Bottom) return false;

            int x, y;

            x = (int)atY(r.Top);
            if (x >= r.Left && x <= r.Right) {
                intercept = new Point(x, r.Top);
                return true;
            }

            x = (int)atY(r.Bottom);
            if (x >= r.Left && x <= r.Right) {
                intercept = new Point(x, r.Bottom);
                return true;
            }

            y = (int)atX(r.Left);
            if (y >= r.Top && y <= r.Bottom) {
                intercept = new Point(r.Left, y);
                return true;
            }

            y = (int)atX(r.Right);
            if (y >= r.Top && y <= r.Bottom) {
                intercept = new Point(r.Right, y);
                return true;
            }

            return false;
        }

        public Point A { get { return a; } }
        public Point B { get { return b; } }

        public override string ToString() {
            return System.String.Format("({0},{1})-({2},{3})", a.X, a.Y, b.X, b.Y);
        }
    }
}
