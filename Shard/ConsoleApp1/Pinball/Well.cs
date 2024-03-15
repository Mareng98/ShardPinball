using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard
{
    internal class Well : GameObject, CollisionHandler
    {
        private PinballMVP parent;

        public Well(string tag, int x, int y, int width, int height, PinballMVP parent)
        {
            addTag(tag);
            Transform.X = x;
            Transform.Y = y;
            Transform.Wid = width;
            Transform.Ht = height;
            this.parent = parent;
            //initialize();
        }

        public override void initialize()
        {

            setPhysicsEnabled();
            MyBody.Mass = 10000;
            MyBody.Kinematic = false;
            MyBody.PassThrough = true;
            MyBody.SetStatic();
            MyBody.addRectCollider();
        }

        public override void update()
        {
        }

        public void onCollisionEnter(PhysicsBody x)
        {
            if (x.Parent.checkTag("Ball"))
            {
                x.Parent.ToBeDestroyed = true;
                parent.ResetBall();
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
            return "Wall: [" + Transform.X + ", " + Transform.Y + "]";
        }
    }
}
