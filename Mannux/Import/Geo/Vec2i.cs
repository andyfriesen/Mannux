using System;

namespace Import.Geo {
    public struct Vec2i {
        int x, y;

        public Vec2i(int a, int b) {
            x = a;
            y = b;
        }

        public int X {
            get { return x; }
            set { x = value; }
        }

        public int Y {
            get { return y; }
            set { y = value; }
        }
    }
}
