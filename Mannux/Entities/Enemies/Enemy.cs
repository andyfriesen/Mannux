using Sprites;
using Entities;

namespace Entities.Enemies {
    abstract class Enemy : Entity {
        protected int hp = 0;
        protected int damage = 0;

        public Enemy(Engine e, BitmapSprite s)
            : base(e, s) { }

        void Die() {
            hp = 0;
            engine.DestroyEntity(this);
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

        public int Damage {
            get { return damage; }
            set {
                damage = value;
            }
        }

    }
}
