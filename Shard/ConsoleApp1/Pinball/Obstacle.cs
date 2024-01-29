using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Shard;

namespace Pinball
{
    internal class Obstacle: GameObject, CollisionHandler
    {
        float cx, cy;
        Vector2 dir, lastDir;
        internal Vector2 LastDir { get => lastDir; set => lastDir = value; }
        internal Vector2 Dir { get => dir; set => dir = value; }

        public override void initialize()
        {


            this.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("ball.png");
            setPhysicsEnabled();


            MyBody.addCircleCollider();

            MyBody.Mass = 1;
            MyBody.MaxForce = 15;
            MyBody.Drag = 0f;
            MyBody.UsesGravity = false;
            MyBody.StopOnCollision = true;
            MyBody.ReflectOnCollision = false;
            Transform.Scalex = 3;
            Transform.Scaley = 3;
            Transform.rotate(90);


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

            if (other.Parent.checkTag("Pinball"))
            {
               
            }

            if (other.Parent.checkTag("Brick"))
            {
                //                            Debug.Log("Hit the Brick");

                //                            Dir = new Shard.Vector();
                //                            Dir.X = (float)(Transform.Centre.X - other.Trans.Centre.X);
                //                            Dir.Y = (float)(Transform.Centre.Y - other.Trans.Centre.Y);

            }



        }





        public void changeDir(int x, int y)
        {
            if (Dir == Vector2.Zero)
            {
                dir = lastDir;
            }

            if (x != 0)
            {
                dir = new Vector2(x, dir.Y);
            }

            if (y != 0)
            {
                dir = new Vector2(dir.X, y);
            }

        }


        public override void physicsUpdate()
        {


        }
        public void onCollisionExit(PhysicsBody x)
        {

        }


        public override string ToString()
        {
            return "Obstacle: [" + Transform.X + ", " + Transform.Y + ", Dir: " + Dir + ", LastDir: " + LastDir + ", " + Transform.Lx + ", " + Transform.Ly + "]";
        }


    }
}
