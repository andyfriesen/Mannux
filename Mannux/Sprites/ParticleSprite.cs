/*
 * Consider a better name for this class.
 * 
 * It's essentially a comet-like thing.  Given a direction, and some other things, the sprite
 * will draw a bunch of neat particle thingies.  Not unlike a comet, ideally.
 */

using Cataract;

using System.Drawing;

namespace Sprites {

    class ParticleSprite : ISprite {
        struct Particle {
            public float x, y;		// position relative to the position at which the particle will be rendered.
            public float size;		// also serves as a lifespan counter
            //public byte r,g,b,a;	// colour (NYI)
        }

        const int maxparticles = 15;

        Particle[] particles = new Particle[maxparticles];
        XNAGraph graph;
        float direction;		// in degrees
        byte r, g, b, a;			// colour
        Rectangle hotspot;

        public ParticleSprite(XNAGraph g, float d) {
            graph = g;
            direction = d;
            hotspot = new Rectangle(0, 0, 16, 16);
            r = this.g = 255;
            a = 128;
            b = 0;
        }

        void UpdateParticles() {
            for (int i = 0; i < maxparticles; i++) {

                particles[i].x--;
                particles[i].y += Engine.rand.Next(0, 3) - 1;
                particles[i].size -= 0.1f;
                if (particles[i].size <= 0) {
                    particles[i].x = 0;
                    particles[i].y = 0;
                    particles[i].size = 10 + Engine.rand.Next(5);
                }
            }
        }

        public void Draw(int x, int y, int frame) {
            UpdateParticles();

            foreach (Particle p in particles) {
                graph.DrawParticle((int)(x + p.x), (int)(y + p.y), p.size, r, g, b, a);
            }
        }

        public int Width { get { return 16; } }	// ???

        public int Height { get { return 16; } }

        public int NumFrames { get { return 1; } }

        public Rectangle HotSpot { get { return hotspot; } }

        // IDisposable
        public void Dispose() {
            return;
        }
    }

}
