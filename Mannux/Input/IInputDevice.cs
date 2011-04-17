using System;
using System.Collections.Generic;
using System.Text;

namespace Input {
    interface IInputDevice {
        void Poll();
        float Axis(int N);
        bool Button(int b);
    }
}
