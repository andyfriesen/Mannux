// created on 30/06/2002 at 11:15 PM

namespace Entities.Enemies {

    class Hopper : Enemy {
        int delay = 0;


        public Hopper(Engine e, int startx, int starty)
            : base(e, (Sprites.ISprite)e.sprites.Load("ripper.txt"))//e.LoadSpriteImage("ripper.txt"))
        {
            x = startx;
            y = starty;
            delay = 0;
            vx = 1;
            vy = 0;

            hp = 10;

            direction = Dir.right;
            anim.Set(2, 3, 10, true);

            UpdateState = new StateHandler(DoTick);
        }


        // states
        public void DoTick() {
            HandleGravity();

            if (touchingground) {
                y = floor.atX(x + width / 2) - height;	// so that it doesn't go through the floor
                if (delay == 0) {
                    delay = 5;
                    vx = 0;
                    vy = 0;
                } else {
                    if (delay > 0) {
                        delay--;
                    }
                    if (delay == 0) {
                        vy = -3;
                        if (direction == Dir.right)
                            vx = 1;
                        else
                            vx = -1;
                    }
                }
                return;
            }

            if (touchingleftwall) {
                vx = 1;
                direction = Dir.right;
                anim.Set(2, 3, 10, true);
            }

            if (touchingrightwall) {
                vx = -1;
                direction = Dir.left;
                anim.Set(0, 1, 10, true);
            }
        }

    }

}
