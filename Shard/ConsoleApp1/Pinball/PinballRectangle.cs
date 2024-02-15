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
        public NewColliderRectangle Collider { get; set; }

        public PinballRectangle(string tag, int x, int y, int width, int height)
        {
            addTag(tag);

            Collider = MyBody.addNewRectCollider(x, y, width, height,0);
        }

        public PinballRectangle(string tag, int x, int y, Vector2[] vertices)
        {
            addTag(tag);

            Collider = MyBody.addNewRectCollider(x, y, vertices, 0);
        }

        public override void initialize()
        {
            setPhysicsEnabled();

            MyBody.Mass = 15000;
            MyBody.MaxForce = 15000;
            MyBody.Drag = 0f;
            MyBody.UsesGravity = false;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = false;
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
            return "NewRectangle: [" + Transform.X + ", " + Transform.Y + "]";
        }
    }
}
