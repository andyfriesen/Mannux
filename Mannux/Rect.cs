using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mannux {
    struct Rect {
        public float x, y, width, height;

        public Rect(float x, float y, float width, float height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public float Left { get { return x; } }
        public float Right { get { return x + width; } }
        public float Top { get { return y; } }
        public float Bottom { get { return y + height; } }

        public void Offset(float x, float y) {
            this.x += x;
            this.y += y;
        }
    }
}
