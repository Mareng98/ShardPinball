using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shard;
namespace Pinball
{
    class Wall : GameObject, CollisionHandler
    {

        public Wall(string tag, int x, int y, int width, int height)
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
            MyBody.SetStatic();
            
            MyBody.addRectCollider();
        }

        public override void update()
        {
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
            return "Wall: [" + Transform.X + ", " + Transform.Y + "]";
        }
    }
}
