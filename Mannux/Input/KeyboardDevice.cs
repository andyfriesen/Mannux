using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Input {
    class KeyboardDevice : IInputDevice {
        private KeyboardState ks;

        private float xAxis;
        private float yAxis;

        public void Poll() {
            ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Left)) xAxis = 0.0f;
            else if (ks.IsKeyDown(Keys.Right)) xAxis = 255.0f;
            else xAxis = 127f;

            if (ks.IsKeyDown(Keys.Up)) yAxis = 0.0f;
            else if (ks.IsKeyDown(Keys.Down)) yAxis = 255.0f;
            else yAxis = 127f;
        }

        public float Axis(int N) {
            switch (N) {
                case 0: return yAxis;
                case 1: return xAxis;
                default:
                    throw new NotImplementedException();
            }
        }

        public bool Button(int b) {
            switch (b) {
                case 0:
                    return ks.IsKeyDown(Keys.Space);
                case 1:
                    return ks.IsKeyDown(Keys.C);
                default:
                    return false;
            }
        }
    }
}
