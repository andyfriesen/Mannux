using System;

namespace Mannux {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args) {
            using (MannuxGame game = new MannuxGame()) {
                game.Run();
            }
        }
    }
}

