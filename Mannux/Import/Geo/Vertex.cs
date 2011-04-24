using System;

namespace Import.Geo {
    [Serializable]
    public struct Vertex {
        int x, y;

        public Vertex(int a, int b) {
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
