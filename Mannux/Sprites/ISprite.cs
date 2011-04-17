using System;
using System.Drawing;

namespace Sprites {

    interface ISprite : IDisposable {
        int Width { get; }
        int Height { get; }
        int NumFrames { get; }

        Rectangle HotSpot { get; }

        void Draw(int x, int y, int frame);
    }

}
