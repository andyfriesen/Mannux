using System.Collections;

using Import.Geo;

namespace Import {
    // TODO: BSP tree, depending on how intensive this gets.
    // Something else to consider is making this a bunch of points
    // then a list of ints indexing them.  Then interconnected things would be a bit easier to edit.
    // When shifting this to a BSP, I'll have to make a BSP tree, then compile it from this class.
    // With any luck, we'll be able to do so without having to put the compiled BSP in the map file,
    // which would nuke the map file format again.  Unless, of course, we moved to file loaders/savers
    // that don't rely on .NET serializers.
    public class VectorObstructionMap {
        public Line[] lines;

        public VectorObstructionMap(VectorIndexBuffer b) {
            Generate(b);
        }

        public void Generate(VectorIndexBuffer b) {
            lines = new Line[b.Lines.Count];

            int pos = 0;
            foreach (int[] i in b.Lines) {
                lines[pos++] = new Line((Vertex)b.Points[i[0]], (Vertex)b.Points[i[1]]);
            }
        }

        public Line Test(int x, int y, int x2, int y2) {
            System.Drawing.Rectangle r = new System.Drawing.Rectangle(x, y, x2 - x, y2 - y);
            Vertex p = new Vertex(0, 0);

            foreach (Line l in lines) {
                if (l.Touches(r, ref p))
                    return l;
            }

            return null;
        }
    }

}
