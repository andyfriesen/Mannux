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
            : base(e, (Sprites.ISprite)e.sprites.Load("tabby.txt"))//e.LoadSpriteImage("tabby.txt"))
        {
            input = engine.input.Keyboard;

            width = 12;

            System.Console.WriteLine("Player {0}x{1}", width, height);

            UpdateState = new StateHandler(Walk);
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
            UpdateState = new StateHandler(Stand);
            anim.Set(AnimState.playerstand[(int)direction]);
        }

        public void SetShootUpState() {
            UpdateState = new StateHandler(Stand);
            anim.Set(AnimState.playershootup[(int)direction]);
        }

        public void SetWalkState() {
            UpdateState = new StateHandler(Walk);
            anim.Set(AnimState.playerwalk[(int)direction]);
        }

        public void SetWalkFireState() {
            UpdateState = new StateHandler(Walk);
            anim.Set(AnimState.playerwalkshooting[(int)direction]);
        }

        public void SetFireState() {
            UpdateState = new StateHandler(Stand);
            anim.Set(AnimState.playershooting[(int)direction]);
        }

        public void SetWalkFireAngleUpState() {
            UpdateState = new StateHandler(Walk);
            anim.Set(AnimState.playerwalkshootingangleup[(int)direction]);
        }

        public void SetWalkFireAngleDownState() {
            UpdateState = new StateHandler(Walk);
            anim.Set(AnimState.playerwalkshootingangledown[(int)direction]);
        }

        public void SetJumpState() {
            jumpcount = jumpheight;
            UpdateState = new StateHandler(Jump);
            anim.Set(AnimState.playerjump[(int)direction]);
        }

        public void SetFallState() {
            UpdateState = new StateHandler(Fall);
            anim.Set(AnimState.playerfall[(int)direction]);
        }

        public void SetCrouchState() {
            UpdateState = new StateHandler(Crouch);
            anim.Set(AnimState.playercrouch[(int)direction]);
        }

        public void SetCrouchFireState() {
            UpdateState = new StateHandler(Crouch);
            anim.Set(AnimState.playercrouchshooting[(int)direction]);
        }

        public void SetFallFireState(Dir d) {
            UpdateState = new StateHandler(Fall);
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
            UpdateState = new StateHandler(Hurt);
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

            if (input.Axis(0) == 0) //aim up
			{
                SetWalkFireAngleUpState();
                if (input.Button(key_SPACE)) {
                    if (direction == Dir.left) {
                        Fire(Dir.up_left);
                    } else {
                        Fire(Dir.up_right);
                    }
                }
            }
            if (input.Axis(0) == 255) //shoot down
			{
                SetWalkFireAngleDownState();
                if (input.Button(key_SPACE)) {
                    if (direction == Dir.left) {
                        Fire(Dir.down_left);

                    } else {
                        Fire(Dir.down_right);
                    }
                }
            }
            if (input.Button(key_SPACE) && input.Axis(0) != 255 && input.Axis(0) != 0) //not pressing up/down
			{
                Fire(direction);
                SetWalkFireState();
            }

            if (input.Axis(1) == 0)	// left
			{
                vx -= groundaccel;
                if (direction != Dir.left) {
                    direction = Dir.left;
                    anim.Set(AnimState.playerwalk[(int)Dir.left]);//8,13,10,true);
                }
            }
            if (input.Axis(1) == 255) // right
			{
                vx += groundaccel;
                if (direction != Dir.right) {
                    direction = Dir.right;
                    anim.Set(AnimState.playerwalk[(int)Dir.right]);//16,21,10,true);
                }
            }

            if (vx == 0) {
                SetStandState();
                return;
            }

            if (!touchingceiling && input.Button(key_C))	// jump
			{
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

                if (input.Axis(0) == 0) //pointing up
				{
                    if (input.Axis(1) == 0) //upleft
					{
                        SetFallFireState(Dir.up_left);
                    }
                    if (input.Axis(1) == 255) //upright
					{
                        SetFallFireState(Dir.up_right);
                    } else //straight up
					{
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

            if (input.Axis(1) == 0)	// left
                vx -= airaccel;
            if (input.Axis(1) == 255)	// right
                vx += airaccel;
        }

        public void Fall() {
            if (input.Button(key_SPACE)) {
                if (input.Axis(0) == 0) //pointing up
				{
                    if (input.Axis(1) == 0) //upleft
					{
                        SetFallFireState(Dir.up_left);
                    }
                    if (input.Axis(1) == 255) //upright
					{
                        SetFallFireState(Dir.up_right);
                    }
                    if (input.Axis(1) != 255 && input.Axis(1) != 0) //straight up
					{
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
                    }
                    if (input.Axis(1) != 255 && input.Axis(1) != 0) //straight down
					{
                        SetFallFireState(Dir.down);
                    }
                }
                if (input.Axis(0) != 255 && input.Axis(0) != 0) //not pressing up/down
				{
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

            if (input.Axis(1) == 0)	// left
			{
                vx -= airaccel;

                if (direction != Dir.left) {
                    direction = Dir.left;
                    anim.Set(AnimState.playerfall[(int)Dir.left]);
                }
            }
            if (input.Axis(1) == 255)	// right
			{
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

        const int key_ESC = 1;
        const int key_1 = 2;
        const int key_2 = 3;
        const int key_3 = 4;
        const int key_4 = 5;
        const int key_5 = 6;
        const int key_6 = 7;
        const int key_7 = 8;
        const int key_8 = 9;
        const int key_9 = 10;
        const int key_0 = 11;
        const int key_PLUS = 12;
        const int key_MINUS = 13;
        const int key_BACKSPACE = 14;
        const int key_TAB = 15;
        const int key_ENTER = 28;
        const int key_CTRL = 29;
        const int key_LSHIFT = 42;
        const int key_RSHIFT = 54;
        const int key_STAR = 55;
        const int key_ALT = 56;
        const int key_SPACE = 57;
        const int key_CAPSLOCK = 58;
        const int key_NUMLOCK = 69;
        const int key_SCROLL_LOCK = 70;
        const int key_HOME = 71;
        const int key_UP = 72;
        const int key_PGUP = 73;
        const int key_PADMINUS = 74;
        const int key_LEFT = 75;
        const int key_PAD5 = 76;
        const int key_RIGHT = 77;
        const int key_PADPLUS = 78;
        const int key_END = 79;
        const int key_DOWN = 80;
        const int key_PGDN = 81;
        const int key_INSERT = 82;
        const int key_DELETE = 83;
        const int key_MACRO = 111;
        const int key_F1 = 59;
        const int key_F2 = 60;
        const int key_F3 = 61;
        const int key_F4 = 62;
        const int key_F5 = 63;
        const int key_F6 = 64;
        const int key_F7 = 65;
        const int key_F8 = 66;
        const int key_F9 = 67;
        const int key_F10 = 68;
        const int key_F11 = 87;
        const int key_F12 = 88;
        const int key_COMMA = 51;
        const int key_PERIOD = 52;
        const int key_BACKSLASH = 53;
        const int key_SEMICOLON = 39;
        const int key_QUOTE = 40;
        const int key_LEFTBRACKET = 26;
        const int key_RIGHTBRACKET = 27;
        const int key_A = 30;
        const int key_B = 48;
        const int key_C = 46;
        const int key_D = 32;
        const int key_E = 18;
        const int key_F = 33;
        const int key_G = 34;
        const int key_H = 35;
        const int key_I = 23;
        const int key_J = 36;
        const int key_K = 37;
        const int key_L = 38;
        const int key_M = 50;
        const int key_N = 49;
        const int key_O = 24;
        const int key_P = 25;
        const int key_Q = 16;
        const int key_R = 19;
        const int key_S = 31;
        const int key_T = 20;
        const int key_U = 22;
        const int key_V = 47;
        const int key_W = 17;
        const int key_X = 45;
        const int key_Y = 21;
        const int key_Z = 44;
    }

}
