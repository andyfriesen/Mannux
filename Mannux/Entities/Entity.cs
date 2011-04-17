using System;

using Import.Geo;
using Sprites;

namespace Entities {

    delegate void StateHandler();

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
        protected const float gravity = 0.13f;

        // lots of fun state flags for fun funness.
        protected bool touchingground;
        protected bool touchingceiling;
        protected bool touchingleftwall;
        protected bool touchingrightwall;
        protected Line leftwall;
        protected Line rightwall;
        protected Line floor;
        protected Line ceiling;

        // More general purpose state things
        protected float x = 0, y = 0;				//!< Position
        protected int width, height;				//!< for convenience.  Size of the entity. (same as the sprite's hotspot)
        protected float vx = 0, vy = 0;				//!< Velocity
        public ISprite sprite;					//!< Sprite
        public AnimState anim = new AnimState();	//!< Sprite animation state
        protected Dir direction = Dir.left;		//!< Which way is the entity facing?

        protected bool visible = true;

        protected StateHandler UpdateState;		//!< Updates the entity's state.  This delegate gets changed around a lot. ;)

        protected Engine engine;

        public Entity(Engine e, ISprite s) {
            engine = e;
            sprite = s;

            width = sprite.HotSpot.Width;
            height = sprite.HotSpot.Height;

            UpdateState = new StateHandler(DoNothing);	// avoid null references		
        }

        void DoNothing() { }

        protected virtual void Update() {
            UpdateState();
            anim.Update();
        }

        public void Dispose() {
            engine.sprites.Free(sprite);
            sprite = null;
        }

        public bool Touches(Entity e) {
            if (x > e.x + e.width)
                return false;
            if (y > e.y + e.height)
                return false;
            if (e.x > x + width)
                return false;
            if (e.y > y + height)
                return false;

            return true;
        }

        public void Tick() {
            /*	vx=Vector.Clamp(vx,maxvelocity);*/
            vy = Vector.Clamp(vy, maxvelocity);

            float x2 = x + width;
            float y2 = y + height;

            // up
            ceiling = engine.IsObs((int)(x + 2), (int)(y + vy), (int)(x2 - 4), (int)(y - vy));
            touchingceiling = ceiling != null;

            // down
            floor = engine.IsObs((int)(x + 2), (int)(y2 - vy - 2), (int)(x2 - 4), (int)(y2));
            touchingground = floor != null;

            // left
            leftwall = engine.IsObs((int)(x + vx), (int)(y + 4), (int)(x - vx), (int)(y2 - 2));
            touchingleftwall = leftwall != null;

            // right
            rightwall = engine.IsObs((int)(x2 + vx), (int)(y + 4), (int)(x2 - vx + 4), (int)(y2 - 2));
            touchingrightwall = rightwall != null;

            Update();

            x += vx; y += vy;
        }

        public void HandleGravity() {
            if (!touchingground)
                vy += gravity;
            else vy = 0;
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
