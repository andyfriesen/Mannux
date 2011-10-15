using System;
using System.Collections.Generic;

using Sprites;
using Mannux;
using Microsoft.Xna.Framework;

namespace Entities {
    // direction.
    enum Dir {
        left,
        right,
        up,
        down,
        up_left,
        up_right,
        down_left,
        down_right
        // up down?  diagonals?
    }

    abstract class Entity {
        // constants
        protected const float maxvelocity = 4;
        protected const float gravity = 0.217f;

        // lots of fun state flags for fun funness.
        protected Line? ceilingSurface;
        protected Line? leftSurface;
        protected Line? rightSurface;
        public bool touchingground {
            get {
                return groundHeight.HasValue;
            }
        }
        protected bool touchingleftwall {
            get { return leftSurface != null; }
        }

        protected bool touchingrightwall {
            get { return rightSurface != null; }
        }

        protected bool touchingceiling {
            get { return ceilingSurface != null; }
        }

        // More general purpose state things
        protected float x = 0, y = 0;
        public int Width, Height;
        protected float vx = 0, vy = 0;
        public BitmapSprite sprite;
        public AnimState anim = new AnimState();
        protected Dir direction = Dir.left;

        protected bool visible = true;

        protected Action UpdateState;

        protected Engine engine;

        public Entity(Engine e, BitmapSprite s) {
            engine = e;
            sprite = s;

            Width = sprite.HotSpot.Width;
            Height = sprite.HotSpot.Height;

            UpdateState = DoNothing;	// avoid null references
        }

        void DoNothing() { }

        protected virtual void Update() {
            UpdateState();
            anim.Update();
        }

        public bool Touches(Entity e) {
            return
                (x <= e.x + e.Width) &&
                (y <= e.y + e.Height) &&
                (e.x <= x + Width) &&
                (e.y <= y + Height)
            ;
        }

        const float collisionGranularity = 8.0f;
        public void Tick() {
            /*	vx=Vector.Clamp(vx,maxvelocity);*/
            vy = Vector.Clamp(vy, maxvelocity);

            /* If needed, this can probably be made even faster by simply precomputing the sub-tile (x,y) offset
             * of the points we are testing, then merely iterating over tiles.
             * 
             * As written, we just compute pixel coordinates, and translate each one back into tile coordinates.
             * This is probably inefficient.
             * -- andy
             */

            Update();

            DoCollision();
            x += vx;
            y += vy;
        }

        protected bool TestFloor(float ty) {
            var g = false;
#if false
            for (var tx = x + sprite.HotSpot.Width; tx > x; tx -= collisionGranularity) {
                g |= engine.IsObs(tx, (int)ty);
            }
#endif
            return g;
        }

        private const int GROUND_THRESHHOLD = 10;

        protected void DoCollision() {
            var limit = 4;
            do {
                var r = new Rect(x + vx, y + vy, Width, Height);
                var cr = engine.obstructionTiles.TestSAT(r);
                if (!cr.HasValue) {
                    break;
                }
                var mtv = cr.Value.mtv;

                var velocity = new Vector2(vx, vy);
                velocity += mtv;

                vx = velocity.X;
                vy = velocity.Y;

                --limit;
            } while (limit > 0);
            var count = 4 - limit;
            if (count > 1) {
                Console.WriteLine("Resolved in {0} steps", count);
            }
        }

        const int SLOPE_THRESHHOLD = 8;
        private float? groundHeight;

        protected float? SenseGround() {
            var r = new Rect(x + vx, y + vy + Height, Width, SLOPE_THRESHHOLD);
            var x1 = x + vx;
            var x2 = x1 + Width;

            var bestIntercept = float.MaxValue;
            groundHeight = null;

            Action<Line> cb = (line) => {
                /*
                 * I think what we want here is to track, for each direction,
                 * the surface that penetrates the entity's hotspot the most.
                 */

                var intercept = float.MaxValue;
                if ((x1 > line.A.X || x1 > line.B.X) &&
                    (x1 < line.A.X || x1 < line.B.X)
                ) {
                    var ay1 = line.atX(x1);
                    intercept = ay1;
                }

                var ay2 = line.atX(x2);
                if ((x2 > line.A.X || x2 > line.B.X) &&
                    (x2 < line.A.X || x2 < line.B.X) &&
                    ay2 < intercept
                ) {
                    intercept = ay2;
                }

                if (intercept < bestIntercept) {
                    bestIntercept = intercept;
                }
            };

            engine.obstructionTiles.Test(r, cb);
            if (bestIntercept < float.MaxValue) {
                groundHeight = bestIntercept;
            }

            return groundHeight;
        }

        public void PlaceOnGround() {
            if (groundHeight.HasValue) {
                y = groundHeight.Value - Height;
                vy = 0;
            }
        }

        public void HandleGravity() {
            vy += gravity;
        }

        // -Accessors

        public int X {
            get { return (int)x; }
            set { x = value; }
        }

        public int Y {
            get { return (int)y; }
            set { y = value; }
        }

        public bool Visible {
            get { return visible; }
            set { visible = value; }
        }

        public Dir Facing {
            get { return direction; }
            set { direction = value; }
        }
    }
}
