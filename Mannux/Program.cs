using System;

namespace Mannux {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            using (Engine game = new Engine()) {
                game.Run();
            }
        }
    }
}
