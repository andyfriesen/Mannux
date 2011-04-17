using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Input {
    class X360Gamepad : IInputDevice {
        GamePadState ps;

        public void Poll() {
            ps = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
        }

        public float Axis(int N) {
            switch (N) {
                case 0: return ps.ThumbSticks.Left.X;
                case 1: return ps.ThumbSticks.Left.Y;
                default: throw new InvalidOperationException();
            }
        }

        public bool Button(int b) {
            switch (b) {
                case 0: return ps.Buttons.A == ButtonState.Pressed;
                case 1: return ps.Buttons.X == ButtonState.Pressed;
                default: throw new InvalidOperationException();
            }
        }
    }
}
