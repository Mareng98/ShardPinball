using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Shard;
namespace Plinko
{
    class PlinkoPolygon : GameObject, CollisionHandler
    {
        public ColliderPolygon Collider { get; set; }

        public PlinkoPolygon(string tag, int x, int y, int width, int height)
        {
            addTag(tag);

            Collider = MyBody.addPolygonCollider(x, y, width, height, 0);
            Collider.DrawingColor = Color.Coral;
        }

        public PlinkoPolygon(string tag, int x, int y, Vector2[] vertices)
        {
            addTag(tag);
            Collider = MyBody.addPolygonCollider(x, y, vertices, 0);
            Collider.DrawingColor = Color.Coral;
        }

        public override void initialize()
        {
            setPhysicsEnabled();

            MyBody.Mass = 15000000;
            MyBody.MaxForce = 15000;
            MyBody.AngularDrag = 0;
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
