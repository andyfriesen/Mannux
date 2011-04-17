using Cataract;

namespace Sprites {

    class TabbySprite : BitmapSprite {
        public int upperframe = 0;
        public int lowerframe = 0;

        public TabbySprite(IGraph g)
            : base(g, Import.ImageSprite.Load("tabby.txt")) {

        }

        public override void Draw(int x, int y, int frame) {
            base.Draw(x, y, upperframe);
            base.Draw(x, y, lowerframe);
        }
    }
}
