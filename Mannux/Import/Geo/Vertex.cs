using System;

namespace Import.Geo {
    public struct Vertex {
        float x, y;

        public Vertex(int a, int b) {
            x = a;
            y = b;
        }

        public Vertex(float a, float b) {
            x = a;
            y = b;
        }

        public float X {
            get { return x; }
            set { x = value; }
        }

        public float Y {
            get { return y; }
            set { y = value; }
        }
    }
}
