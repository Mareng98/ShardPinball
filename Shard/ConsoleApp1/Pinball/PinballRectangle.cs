using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Shard;
namespace Pinball
{
    class PinballRectangle: GameObject, CollisionHandler
    {

        public PinballRectangle(string tag, int x, int y, int width, int height)
        {
            addTag(tag);
            // For some reason NewRectCollider doesnt trigger collisions, so we use addRectCollider in initialize for debugging

            MyBody.addNewRectCollider(x, y, width, height, 0);
        }

        public PinballRectangle(string tag, int x, int y, Vector2[] vertices)
        {
            addTag(tag);

            MyBody.addNewRectCollider(x, y, vertices, 0);
        }

        public override void initialize()
        {
            setPhysicsEnabled();

            MyBody.Mass = 1;
            MyBody.MaxForce = 15000;
            MyBody.Drag = 0f;
            MyBody.UsesGravity = false;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = true;
        }


        public override void update()
        {
            
            Bootstrap.getDisplay().addToDraw(this);
        }

        public void onCollisionEnter(PhysicsBody x)
        {
            int test = 0;
        }

        public void onCollisionExit(PhysicsBody x)
        {
        }

        public void onCollisionStay(PhysicsBody x)
        {
        }

        public override string ToString()
        {
            return "NewRectangle: [" + Transform.X + ", " + Transform.Y + "]";
        }
    }
}
