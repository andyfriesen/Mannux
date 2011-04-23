using System;
using System.Collections.Generic;
using System.Text;

namespace Input {
    class InputHandler {
        private readonly KeyboardDevice kd = new KeyboardDevice();
        private readonly MouseDevice md = new MouseDevice();

        public IInputDevice Keyboard {
            get {
                return kd;
            }
        }

        public void Poll() {
            kd.Poll();
        }
    }
}
