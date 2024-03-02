using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Shard;
namespace Pinball
{
    internal class PinballBallIcon: GameObject
    {

            public PinballBallIcon(int x, int y)
            {
                Transform.X = x;
                Transform.Y = y;
            }

            public override void initialize()
            {
                this.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("pinball.png");
                Transform.Scalex = 1f;
                Transform.Scaley = 1f;


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
