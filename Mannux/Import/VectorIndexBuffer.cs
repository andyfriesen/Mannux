using System;
using System.Collections;
using System.Collections.Generic;
using Import.Geo;

namespace Import {
    [Serializable]
    public class VectorIndexBuffer {
        private readonly List<Vertex> points = new List<Vertex>();
        public ArrayList lines;		// arraylist of int[2]'s

        public VectorIndexBuffer() {
            points = new List<Vertex>();
            lines = new System.Collections.ArrayList();
        }

        public List<Vertex> Points { get { return points; } }

        public System.Collections.ArrayList Lines { get { return lines; } }

        public void RemovePoint(int idx) {
            // if this point is connected to anything, remove the line as well
            int i = 0;
            while (i < lines.Count) {
                int[] j = (int[])lines[i];

                if (j[0] == idx || j[1] == idx) {
                    lines.RemoveAt(i);
                    continue;
                }
                i++;
            }

            // move any references down one so they reference the proper points one more
            foreach (int[] p in lines) {
                if (p[0] > idx) p[0]--;
                if (p[1] > idx) p[1]--;
            }

            points.RemoveAt(idx);
        }
    }
}
