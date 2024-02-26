using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard
{
    internal class Well : GameObject, CollisionHandler
    {

        public Well(string tag, int x, int y, int width, int height)
        {
            addTag(tag);
            Transform.X = x;
            Transform.Y = y;
            Transform.Wid = width;
            Transform.Ht = height;
            //initialize();
        }

        public override void initialize()
        {

            setPhysicsEnabled();
            MyBody.Mass = 10000;
            MyBody.Kinematic = false;
            MyBody.PassThrough = true;
            MyBody.addRectCollider();
        }

        public override void update()
        {
        }

        public void onCollisionEnter(PhysicsBody x)
        {
            x.Parent.ToBeDestroyed = true;
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
