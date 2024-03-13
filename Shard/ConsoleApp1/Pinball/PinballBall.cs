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

        public PinballBall(string tag, int x, int y, Vector2 force)
        {
            addTag(tag);
            MyBody.Force = force;
            Transform.X = x;
            Transform.Y = y;
        }

        public override void initialize()
        {
            this.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("pinball.png");
            setPhysicsEnabled();
            Transform.Scalex = 1f;
            Transform.Scaley = 1f;
            Transform.Wid = 6;
            Transform.Ht = 6;
            MyBody.addCircleCollider();

            MyBody.Mass = 0.2f;
            MyBody.MaxForce = 30;
            MyBody.Drag = 0f;
            MyBody.UsesGravity = true;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = true;
            MyBody.FrictionCoefficient = 0.014f;


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


        }


        public override void physicsUpdate()
        {
        }
        public void onCollisionExit(PhysicsBody x)
        {

        }


        public override string ToString()
        {
            return "PinBall: [" + Transform.X + ", " + Transform.Y + ", " + Transform.Lx + ", " + Transform.Ly + "]";
        }


    }
}
