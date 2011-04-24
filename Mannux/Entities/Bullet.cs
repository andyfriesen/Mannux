// created on 30/06/2002 at 8:15 PM
using System;
using Entities.Enemies;

namespace Entities {
    class Bullet : Entity {
        const float velocity = 6.6f;

        public Bullet(Engine e, float startx, float starty, Dir d) :
            base(e, e.BulletSprite) {
            UpdateState = (CheckCollision);

            x = startx;
            y = starty;

            direction = d;
            switch (d) {
                case Dir.up:
                    vy = -velocity;
                    anim.Set(3, 3, 0, false);
                    break;
                case Dir.up_right:
                    vy = -velocity; vx = velocity;
                    anim.Set(7, 7, 0, false);
                    break;
                case Dir.right:
                    vx = velocity;
                    anim.Set(0, 0, 0, false);
                    break;
                case Dir.down_right:
                    vx = velocity; vy = velocity;
                    anim.Set(4, 4, 0, false);
                    break;
                case Dir.down:
                    vy = velocity;
                    anim.Set(2, 2, 0, false);
                    break;
                case Dir.down_left:
                    vy = velocity; vx = -velocity;
                    anim.Set(5, 5, 0, false);
                    break;
                case Dir.left:
                    vx = -velocity;
                    anim.Set(1, 1, 0, false);
                    break;
                case Dir.up_left:
                    vx = -velocity; vy = -velocity;
                    anim.Set(6, 6, 0, false);
                    break;
            }

            //anim.Set(2,3,10,true);
        }

        public void CheckCollision() {
            float nx;

            if (touchingleftwall || touchingrightwall || touchingground || touchingceiling) {

                if (direction == Dir.left) nx = x - 8;
                else nx = x;

                Boom B = new Boom(engine, nx, y - 4);
                engine.SpawnEntity(B);

                engine.DestroyEntity(this); //boom!
                return;
            }

            Entity temp = engine.DetectCollision(this);
            if (!(temp is Enemy))
                return;

            Enemy ent = (Enemy)temp;
            ent.HP -= 5;
            System.Console.WriteLine("HP: " + ent.HP);

            if (direction == Dir.left) nx = x - 8;
            else nx = x;

            Boom Bo = new Boom(engine, x, y - 4);
            engine.SpawnEntity(Bo);

            engine.DestroyEntity(this);
        }
    }
}
