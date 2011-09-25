using System;

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
        public Line? groundSurface;
        protected Line? ceilingSurface;
        protected Line? leftSurface;
        protected Line? rightSurface;

        protected bool touchingleftwall {
            get { return leftSurface != null; }
        }

        protected bool touchingrightwall {
            get { return rightSurface != null; }
        }

        protected bool touchingground {
            get { return groundSurface != null; }
        }

        protected bool touchingceiling {
            get { return ceilingSurface != null; }
        }

        // More general purpose state things
        protected float x = 0, y = 0;				//!< Position
        protected int width, height;				//!< for convenience.  Size of the entity. (same as the sprite's hotspot)
        protected float vx = 0, vy = 0;				//!< Velocity
        public BitmapSprite sprite;					//!< Sprite
        public AnimState anim = new AnimState();	//!< Sprite animation state
        protected Dir direction = Dir.left;		//!< Which way is the entity facing?

        protected bool visible = true;

        protected Action UpdateState;		//!< Updates the entity's state.  This delegate gets changed around a lot. ;)

        protected Engine engine;

        public Entity(Engine e, BitmapSprite s) {
            engine = e;
            sprite = s;

            width = sprite.HotSpot.Width;
            height = sprite.HotSpot.Height;

            UpdateState = DoNothing;	// avoid null references
        }

        void DoNothing() { }

        protected virtual void Update() {
            UpdateState();
            anim.Update();
        }

        public bool Touches(Entity e) {
            return
                (x <= e.x + e.width) &&
                (y <= e.y + e.height) &&
                (e.x <= x + width) &&
                (e.y <= y + height)
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

            DoCollision();

            Update();

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

        protected void DoCollision() {
            var w = sprite.HotSpot.Width;
            var h = sprite.HotSpot.Height;

            var x2 = x + w;
            var y2 = y + h;

            const float MAX_GROUND_SLOPE = 1.1f; // anything steeper than this is not a walkable surface
            const int GROUND_THRESHHOLD = 4;

            var oldGround = groundSurface;
            groundSurface = null;
            var groundInterceptY = int.MaxValue;
            var floorRect = new Rectangle((int)(x + vx), (int)(y2 - 8), w, GROUND_THRESHHOLD);

            Action<Line> cb = (line) => {
                if (Math.Abs(line.Slope) > MAX_GROUND_SLOPE) {
                    return;
                }

                /*
                 * I think what we want here is to track, for each direction,
                 * the surface that penetrates the entity's hotspot the most.
                 */

                var ay1 = line.atX(x + vx + 1);
                var ay2 = line.atX(x2 + vx - 1);

                Point intercept;
                if (ay1 < ay2) {
                    intercept = new Point((int)(x + vx + 1), (int)ay1);
                } else {
                    intercept = new Point((int)(x2 + vx - 1), (int)ay2);
                }

                if (y2 - 16 <= intercept.Y && intercept.Y < y2 + GROUND_THRESHHOLD) {
                    if (intercept.Y < groundInterceptY) {
                        groundInterceptY = intercept.Y;
                        groundSurface = line;
                    }
                }
            };
            var projectedY = y + vy;
            if (groundSurface.HasValue && vy == 0.0f) {
                if (vx > 0) {
                    projectedY = groundSurface.Value.atX(x + vx + sprite.HotSpot.Width);
                } else {
                    projectedY = groundSurface.Value.atX(x + vx);
                }
            }

            var r = new Rectangle(
                (int)(x + vx),
                (int)(projectedY),
                sprite.HotSpot.Width,
                sprite.HotSpot.Height + GROUND_THRESHHOLD
            );

            engine.GetObstructions(r, cb);
        }

        private float LineAtX(Line l, float x) {
            if (x < l.A.X && x < l.B.X) {
                x = Math.Min(l.A.X, l.B.X);
            }
            if (x > l.A.X && x > l.B.X) {
                x = Math.Max(l.A.X, l.B.X);
            }
            return l.atX(x);
        }

        protected void PlaceOnGround() {
            if (groundSurface == null) {
                return;
            }

            var y1 = LineAtX(groundSurface.Value, x);
            var y2 = LineAtX(groundSurface.Value, x + sprite.HotSpot.Width);

            var newy = Math.Min(y1, y2) - height;

            var dy = Math.Abs((int)y - (int)newy);
            if (dy > 1 && dy > vx) {
                Console.WriteLine("Moving from {0} to {1} dy={2}", y + height, newy + height, dy);
                y = newy;
            }
        }

        public void HandleGravity() {
            if (!touchingground) {
                vy += gravity;
            } else {
                vy = 0;
            }
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

        public int VX {
            get { return (int)vx; }
            set { vx = value; }
        }

        public int VY {
            get { return (int)vy; }
            set { vy = value; }
        }


        public int Width {
            get { return (int)Height; }
            set { height = value; }
        }

        public int Height {
            get { return (int)height; }
            set { height = value; }
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
