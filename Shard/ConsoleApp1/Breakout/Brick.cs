
using Shard;

namespace GameBreakout
{
    class Brick : GameObject, InputListener, CollisionHandler
    {
        private int health;

        public int Health { get => health; set => health = value; }

        public Brick(int x, int y)
        {
            health = 3;
            this.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("brick" + Health + ".png");
            Transform.X = x;
            Transform.Y = y;
            Transform.Wid = 100;
            Transform.Ht = 20;
            MyBody.addPolygonCollider(x, y, 60, 30, 0);
        }

        public override void initialize()
        {


            setPhysicsEnabled();

            MyBody.Mass = 1;
            MyBody.Kinematic = true;

            

            addTag("Brick");

        }

        public void handleInput(InputEvent inp, string eventType)
        {




        }


        public override void update()
        {

            this.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("brick" + Health + ".png");

            Bootstrap.getDisplay().addToDraw(this);
        }

        public void onCollisionEnter(PhysicsBody x)
        {
            Health -= 1;

            if (Health <= 0)
            {
                this.ToBeDestroyed = true;
            }
        }

        public void onCollisionExit(PhysicsBody x)
        {

        }

        public void onCollisionStay(PhysicsBody x)
        {
        }

        public override string ToString()
        {
            return "Brick: [" + Transform.X + ", " + Transform.Y + ", " + Transform.Wid + ", " + Transform.Ht + "]";
        }

    }
}
