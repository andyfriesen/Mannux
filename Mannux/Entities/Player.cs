using System;

using Entities.Enemies;
using Input;

/*
 * Physics notes
 * 
 * There should be a max air velocity for jumping straight, then moving left/right,
 * but it should be disregarged when you already have velocity from running then
 * jumping. If you change direction, then it should be taken into account.
 * 
 * Everything still seems very "floaty", a side effect of using floats I believe.
 * 
 * Need two different jumps: straight up jumps which have no spin (and also have the 
 * regular falling frames after the peak of the jump), and running jumps which do
 * have spin.
 * 
 * Right now, when you change direction of your jump, her facing doesn't change.
 * This needs to be added.
 * 
 * Maximum jump height should be increased in a running jump.
 * 
*/

namespace Entities {

    sealed class Player : Entity {
        // --------------- constants ------------
        const int key_C = 0;
        const int key_SPACE = 1;

        const float groundfriction = 0.16f;
        const float airfriction = 0.01f;

        const float groundaccel = 0.32f;
        const float airaccel = 0.12f;

        const float maxxvelocity = 1.5f;
        const float maxyvelocity = 50;

        const int jumpheight = 50;

        /*	const float groundfriction=0.08f;
                const float airfriction=0.08f;
		
                const float groundaccel=0.16f;
                const float airaccel=0.16f;
		
                const float maxxvelocity=1.5f;
                const float maxyvelocity=50;*/

        // ------------- data -------------

        IInputDevice input;

        int jumpcount;	// when this hits 0, the player is once again affected by gravity.

        private int hp = 100;

        // --------------- code --------------

        public Player(Engine e)
            : base(e, e.TabbySprite) {
            input = engine.input.Keyboard;

            width = 12;

            System.Console.WriteLine("Player {0}x{1}", width, height);

            UpdateState = Walk;
        }

        protected override void Update() {
            base.Update();
            CollisionThings();
            anim.Update();

            if (firedelay > 0) firedelay--;

            vx = Vector.Clamp(vx, maxxvelocity);
            vy = Vector.Clamp(vy, maxyvelocity);
        }

        // player states
        public void CollisionThings() {
            if (touchingleftwall && vx < 0)
                vx = 0;
            if (touchingrightwall && vx > 0)
                vx = 0;

            Entity temp = engine.DetectCollision(this);
            if (temp is Enemy) {
                if (hurtcount == 0) {
                    //hurtcount=20;
                    //HP-=temp.Damage;
                    //HP-=8;
                    //vx=0;
                    //SetHurtState();
                }
            }

        }

        // ------------------ State changers ---------------

        public void SetStandState() {
            UpdateState = Stand;
            anim.Set(AnimState.playerstand[(int)direction]);
        }

        public void SetShootUpState() {
            UpdateState = Stand;
            anim.Set(AnimState.playershootup[(int)direction]);
        }

        public void SetWalkState() {
            UpdateState = Walk;
            anim.Set(AnimState.playerwalk[(int)direction]);
        }

        public void SetWalkFireState() {
            UpdateState = Walk;
            anim.Set(AnimState.playerwalkshooting[(int)direction]);
        }

        public void SetFireState() {
            UpdateState = Stand;
            anim.Set(AnimState.playershooting[(int)direction]);
        }

        public void SetWalkFireAngleUpState() {
            UpdateState = Walk;
            anim.Set(AnimState.playerwalkshootingangleup[(int)direction]);
        }

        public void SetWalkFireAngleDownState() {
            UpdateState = Walk;
            anim.Set(AnimState.playerwalkshootingangledown[(int)direction]);
        }

        public void SetJumpState() {
            jumpcount = jumpheight;
            UpdateState = Jump;
            anim.Set(AnimState.playerjump[(int)direction]);
        }

        public void SetFallState() {
            UpdateState = Fall;
            anim.Set(AnimState.playerfall[(int)direction]);
        }

        public void SetCrouchState() {
            UpdateState = Crouch;
            anim.Set(AnimState.playercrouch[(int)direction]);
        }

        public void SetCrouchFireState() {
            UpdateState = Crouch;
            anim.Set(AnimState.playercrouchshooting[(int)direction]);
        }

        public void SetFallFireState(Dir d) {
            UpdateState = Fall;
            Fire(d);
            switch (d) {
                case Dir.up:
                    anim.Set(AnimState.playerfallshootingup[(int)direction]);
                    break;
                case Dir.up_right:
                    anim.Set(AnimState.playerfallshootingangleup[(int)direction]);
                    break;
                case Dir.right:
                    anim.Set(AnimState.playerfallshooting[(int)direction]);
                    break;
                case Dir.down_right:
                    anim.Set(AnimState.playerfallshootingangledown[(int)direction]);
                    break;
                case Dir.down:
                    anim.Set(AnimState.playerfallshootingdown[(int)direction]);
                    break;
                case Dir.down_left:
                    anim.Set(AnimState.playerfallshootingangledown[(int)direction]);
                    break;
                case Dir.left:
                    anim.Set(AnimState.playerfallshooting[(int)direction]);
                    break;
                case Dir.up_left:
                    anim.Set(AnimState.playerfallshootingangleup[(int)direction]);
                    break;
            }
        }

        public void SetHurtState() {
            UpdateState = Hurt;
            anim.Set(AnimState.hurt[(int)direction]);
        }

        // ----------------------------------------------------

        public void Crouch() {
            if (input.Axis(0) == 0) {
                SetStandState();
            }

            if (input.Button(key_SPACE)) {
                Fire(direction);
                SetCrouchFireState();
            }
        }

        public void Stand() {

            if (input.Button(key_SPACE)) {
                if (input.Axis(0) == 0) //shoot up
				{
                    Fire(Dir.up);
                    SetShootUpState();
                } else //not pressing up/down
				{
                    Fire(direction);
                    SetFireState();
                }
            }

            if (firedelay == 0) { SetStandState(); }


            if (input.Axis(0) == 255) {
                SetCrouchState();
            }

            if (input.Axis(1) == 0) {
                SetWalkState();
                return;
            }

            if (input.Axis(1) == 255) {
                SetWalkState();
                return;
            }

            if (input.Button(key_C))	// jump
			{
                SetJumpState();
                return;
            }

            if (!touchingground) {
                SetFallState();
                return;
            } else
                y = floor.atX(x + width / 2) - height;
        }

        public void Walk() {
            vx = Vector.Decrease(vx, groundfriction);

            vy = 0;

            if (input.Axis(0) == 0) {
                SetWalkFireAngleUpState();
                if (input.Button(key_SPACE)) {
                    if (direction == Dir.left) {
                        Fire(Dir.up_left);
                    } else {
                        Fire(Dir.up_right);
                    }
                }
            }
            if (input.Axis(0) == 255) {
                SetWalkFireAngleDownState();
                if (input.Button(key_SPACE)) {
                    if (direction == Dir.left) {
                        Fire(Dir.down_left);

                    } else {
                        Fire(Dir.down_right);
                    }
                }
            }
            if (input.Button(key_SPACE) && input.Axis(0) != 255 && input.Axis(0) != 0) {
                Fire(direction);
                SetWalkFireState();
            }

            if (input.Axis(1) == 0)	// left
			{
                vx -= groundaccel;
                if (direction != Dir.left) {
                    direction = Dir.left;
                    anim.Set(AnimState.playerwalk[(int)Dir.left]);
                }
            }
            if (input.Axis(1) == 255) // right
			{
                vx += groundaccel;
                if (direction != Dir.right) {
                    direction = Dir.right;
                    anim.Set(AnimState.playerwalk[(int)Dir.right]);
                }
            }

            if (vx == 0) {
                SetStandState();
                return;
            }

            if (!touchingceiling && input.Button(key_C)) {
                SetJumpState();
            }

            if (!touchingground)
                SetFallState();
        }

        public void Jump() {
            if (!input.Button(key_C) || jumpcount == 0) {
                jumpcount = 0;
                vy = -2;
                SetFallState();
                return;
            }

            if (input.Button(key_SPACE)) {

                if (input.Axis(0) == 0) {
                    //pointing up
                    if (input.Axis(1) == 0) {
                        //upleft
                        SetFallFireState(Dir.up_left);
                    }
                    if (input.Axis(1) == 255) {
                        //upright
                        SetFallFireState(Dir.up_right);
                    } else {
                        //straight up
                        SetFallFireState(Dir.up);
                    }
                }
                if (input.Axis(0) == 255) //pointing down
				{
                    if (input.Axis(1) == 0) //downleft
					{
                        SetFallFireState(Dir.down_left);
                    }
                    if (input.Axis(1) == 255) //downright
					{
                        SetFallFireState(Dir.down_right);
                    } else //straight down
					{
                        SetFallFireState(Dir.down);
                    }
                }
                if (input.Axis(0) != 255 && input.Axis(0) != 0) //not pressing up/down
				{
                    SetFallFireState(direction);
                }
            }

            if (touchingceiling) {
                vy = 0;
                SetFallState();
            }

            //y-=2;
            vy = -2;
            jumpcount--;

            vx = Vector.Decrease(vx, airfriction);

            if (input.Axis(1) == 0) {
                // left
                vx -= airaccel;
            }
            if (input.Axis(1) == 255) {
                // right
                vx += airaccel;
            }
        }

        public void Fall() {
            if (input.Button(key_SPACE)) {
                if (input.Axis(0) == 0) {
                    //pointing up
                    if (input.Axis(1) == 0) {
                        //upleft
                        SetFallFireState(Dir.up_left);
                    }
                    if (input.Axis(1) == 255) {
                        //upright
                        SetFallFireState(Dir.up_right);
                    }
                    if (input.Axis(1) != 255 && input.Axis(1) != 0) {
                        //straight up
                        SetFallFireState(Dir.up);
                    }
                }
                if (input.Axis(0) == 255) {
                    //pointing down
                    if (input.Axis(1) == 0) {
                        //downleft
                        SetFallFireState(Dir.down_left);
                    }
                    if (input.Axis(1) == 255) {
                        //downright
                        SetFallFireState(Dir.down_right);
                    }
                    if (input.Axis(1) != 255 && input.Axis(1) != 0) {
                        //straight down
                        SetFallFireState(Dir.down);
                    }
                }
                if (input.Axis(0) != 255 && input.Axis(0) != 0) {
                    //not pressing up/down
                    SetFallFireState(direction);
                }
            }

            if (touchingceiling && vy < 0)
                vy = 0;

            if (touchingground) {
                if (vx != 0)
                    SetWalkState();
                else
                    SetStandState();


                vy = 0;

                // to avoid being embedded in the floor
                y = floor.atX(x + width / 2) - height;
            }

            vx = Vector.Decrease(vx, airfriction);

            if (input.Axis(1) == 0) {
                // left
                vx -= airaccel;

                if (direction != Dir.left) {
                    direction = Dir.left;
                    anim.Set(AnimState.playerfall[(int)Dir.left]);
                }
            }
            if (input.Axis(1) == 255) {
                // right
                vx += airaccel;

                if (direction != Dir.right) {
                    direction = Dir.right;
                    anim.Set(AnimState.playerfall[(int)Dir.right]);
                }
            }

            HandleGravity();
        }

        int hurtcount = 0;
        public void Hurt() {
            if (hurtcount > 0) {
                hurtcount--;
                visible = hurtcount % 8 < 4;
            } else {
                visible = true;
                SetStandState();
            }

            //if(!touchingground) 
            HandleGravity();
        }

        //int firecount;  //how many shots are onscreen?
        int firedelay;  //how much time left until you can fire again?

        public void Fire(Dir d) {
            float bx = x, by = y + 14;

            if (direction == Dir.right) bx += width;
            if (direction == Dir.left) bx -= 8;

            if (firedelay == 0) {
                Bullet B = new Bullet(engine, bx, by, d);
                engine.SpawnEntity(B);
                firedelay = 20;

                //engine.s_effect.Play();

            }
        }

        void Die() {
            hp = 0;
            //engine.DestroyEntity(this);
        }

        public int HP {
            get { return hp; }
            set {
                if (value <= 0)
                    Die();
                else
                    hp = value;
            }
        }
    }
}
