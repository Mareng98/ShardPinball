using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Shard;
namespace Pinball
{
    class PinballPolygon: GameObject, CollisionHandler
    {
        public ColliderPolygon Collider { get; set; }

        public PinballPolygon(string tag, int x, int y, int width, int height)
        {
            addTag(tag);

            Collider = MyBody.addPolygonCollider(x, y, width, height,0);
        }

        public PinballPolygon(string tag, int x, int y, Vector2[] vertices)
        {
            addTag(tag);
            Collider = MyBody.addPolygonCollider(x, y, vertices, 0);
            if (!tag.Equals("Well"))
            {
                Collider.DrawingColor = Color.Coral;
            }
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
            MyBody.SetStatic();
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
