using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Shard;

namespace Pinball
{
    
using Shard;

namespace GameBreakout
{
    class Obstacle : GameObject, InputListener, CollisionHandler
    {

        public override void initialize()
        {


            this.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("blueObstacle.png");
            setPhysicsEnabled();
            Transform.Scalex = 2;
            Transform.Scaley = 2;
            Transform.Wid = 6;
            Transform.Ht = 6;
            MyBody.Mass = 15000;
            MyBody.MaxForce = 15000;
            MyBody.Drag = 0f;
            MyBody.UsesGravity = false;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = true;
            MyBody.addCircleCollider();

            addTag("Obstacle");

        }

        public void handleInput(InputEvent inp, string eventType)
        {




        }


        public override void update()
        {


            Bootstrap.getDisplay().addToDraw(this);
        }

        public void onCollisionEnter(PhysicsBody x)
        {

        }

        public void onCollisionExit(PhysicsBody x)
        {

        }

        public void onCollisionStay(PhysicsBody x)
        {
        }

        public override string ToString()
        {
            return "Obstacle: [" + Transform.X + ", " + Transform.Y + ", " + Transform.Wid + ", " + Transform.Ht + "]";
        }

    }
}

}
