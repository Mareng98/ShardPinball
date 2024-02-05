using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using Shard;

namespace Pinball
{
    class PinballBall : GameObject, CollisionHandler
    {
        private List<double> maxHeight = new List<double>();
        float cx, cy;
        public override void initialize()
        {
            this.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("ball.png");
            setPhysicsEnabled();

            MyBody.addCircleCollider();

            MyBody.Mass = 1;
            MyBody.MaxForce = 15000;
            MyBody.Drag = 0f;
            MyBody.Force = new Vector2(10, 0);
            MyBody.UsesGravity = true;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = true;

            Transform.Scalex = 2;
            Transform.Scaley = 2;

            Debug.Log(this.Transform.ToString());
        }

        public override void update()
        {
            //            Debug.Log ("" + this);

            Bootstrap.getDisplay().addToDraw(this);

        }

        public void onCollisionStay(PhysicsBody other)
        {
        }

        public void onCollisionEnter(PhysicsBody other)
        {
            // hehe magic
            // First find a vector that is parallel with the width of the bounding box
            Vector2 copyOther = new Vector2(other.Trans.X, other.Trans.Y);
            Vector2 diagonalOther = new Vector2(other.Trans.Wid, other.Trans.Y);
            Vector2 parallelOther = diagonalOther - copyOther;
            Vector2 n = new Vector2(-parallelOther.X, parallelOther.Y);
            n = Vector2.Normalize(n);
            Console.WriteLine("Collision");
            // testing reflection vector:

            // Commented this code out since we should try to fix ReflectOnCollision
            // Uncomment this and set reflectionOnCollision = false to try this out
            /* Vector2 newForce = new Vector2(MyBody.Force.X, MyBody.Force.Y);
             if (other.Parent.checkTag("BottomWall") && MyBody.Force.Y > 0)
             {
                 newForce = new Vector2(MyBody.Force.X, -MyBody.Force.Y);
             }else if (other.Parent.checkTag("TopWall") && MyBody.Force.Y < 0)
             {
                 newForce = new Vector2(MyBody.Force.X, -MyBody.Force.Y);
             }else if(other.Parent.checkTag("LeftWall") && MyBody.Force.X < 0)
             {
                 newForce = new Vector2(-MyBody.Force.X, MyBody.Force.Y);
             }else if (other.Parent.checkTag("RightWall") && MyBody.Force.X > 0)
             {
                 newForce = new Vector2(-MyBody.Force.X, MyBody.Force.Y);
             }
             MyBody.Force = newForce;*/

        }


        public override void physicsUpdate()
        {
            if(maxHeight.Count >= 200)
            {
                Debug.Log(maxHeight.Max().ToString());
                maxHeight.Clear();
            }
            else
            {
                maxHeight.Add(MyBody.Force.Y);
            }
            /*// Top wall
            if (Transform.Centre.Y - Transform.Ht <= 0 && MyBody.Force.Y >= 0)
            {

                Debug.Log("Top wall");

            }

            // Bottom wall (same as top wall currently but prints differently)
            if (Transform.Centre.Y + Transform.Ht >= Bootstrap.getDisplay().getHeight() && MyBody.Force.Y >= 0)
            {
                Debug.Log("Bottom wall");

            }

            // Left wall
            if (Transform.Centre.X - Transform.Wid <= 0 && MyBody.Force.X < 0 )
            {
                Debug.Log("Left wall");

            }

            // Right wall
            if (Transform.Centre.X + Transform.Wid >= Bootstrap.getDisplay().getWidth() && MyBody.Force.X > 0)
            {
                Debug.Log("Right wall");

            }*/
        }
        public void onCollisionExit(PhysicsBody x)
        {

        }


        public override string ToString()
        {
            return "Ball: [" + Transform.X + ", " + Transform.Y + ", " + Transform.Lx + ", " + Transform.Ly + "]";
        }


    }
}
