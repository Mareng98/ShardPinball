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
    class Obstacle : GameObject, CollisionHandler
    {
        ColliderCircle circleCollider;
        private string obstacleLightOnPath;
        private string obstacleLightOffPath;
        private int obstacleLightOnDuration = 15;
        private int lightDuration = 0;
        Display d = Bootstrap.getDisplay();
        public Obstacle()
        {

            setPhysicsEnabled();
            Transform.Scalex = 3;
            Transform.Scaley = 3;
            Transform.Wid = 6;
            Transform.Ht = 6;
            MyBody.Mass = 1;
            MyBody.Drag = 0f;
            MyBody.UsesGravity = false;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = false;
            circleCollider = MyBody.addCircleCollider();
            circleCollider.DrawingColor = Color.White;
            addTag("Obstacle");
        }



        public override void initialize()
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

