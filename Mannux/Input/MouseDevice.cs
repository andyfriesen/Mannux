using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Input {
    enum MouseButton {
        None = 0,
        Left  = 1 << 0,
        Right = 1 << 1,
        Middle = 1 << 2
    }

    class MouseDevice {
        private MouseState oldms;
        private MouseState ms;

        public event Action<Point, MouseButton> MouseDown;
        public event Action<Point, MouseButton> MouseUp;
        public event Action<Point, int> MouseWheel;
        public event Action<Point, MouseButton> Moved;

        public void Poll() {
            oldms = ms;
            ms = Mouse.GetState();
        }

        private bool LeftClicked() {
            return ms.LeftButton == ButtonState.Pressed && ms.LeftButton != oldms.LeftButton;
        }

        private bool RightClicked() {
            return ms.RightButton == ButtonState.Pressed && ms.RightButton != oldms.RightButton;
        }

        public void SendEvents() {
            if (ms.LeftButton != oldms.LeftButton) {
                if (ms.LeftButton == ButtonState.Pressed) {
                    if (MouseDown != null) {
                        MouseDown(new Point(ms.X, ms.Y), MouseButton.Left);
                    }
                } else {
                    if (MouseUp != null) {
                        MouseUp(new Point(ms.X, ms.Y), MouseButton.Left);
                    }
                }
            }

            if (ms.RightButton != oldms.RightButton) {
                if (ms.RightButton == ButtonState.Pressed) {
                    if (MouseDown != null) {
                        MouseDown(new Point(ms.X, ms.Y), MouseButton.Right);
                    }
                } else {
                    if (MouseUp != null) {
                        MouseUp(new Point(ms.X, ms.Y), MouseButton.Right);
                    }
                }
            }

            var wheelDelta = oldms.ScrollWheelValue - ms.ScrollWheelValue;
            if (wheelDelta != 0) {
                if (MouseWheel != null) {
                    MouseWheel(new Point(ms.X, ms.Y), wheelDelta);
                }
                return;
            }

            if (oldms.X != ms.X || oldms.Y != ms.Y) {
                if (Moved != null) {
                    var mouseMask = MouseButton.None;
                    if (ms.LeftButton == ButtonState.Pressed) {
                        mouseMask |= MouseButton.Left;
                    }
                    if (ms.RightButton == ButtonState.Pressed) {
                        mouseMask |= MouseButton.Right;
                    }
                    Moved(new Point(ms.X, ms.Y), mouseMask);
                }
                return;
            }
        }
    }
}
