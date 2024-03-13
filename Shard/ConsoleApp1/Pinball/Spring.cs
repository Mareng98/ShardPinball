using Shard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Pinball
{
    internal class Spring:GameObject, CollisionHandler
    {
        private float power;
        private float maxPower;
        FlipperDirection charge;
        public FlipperDirection Charge { get => charge; set => charge = value; }

        public Spring(string tag, int x, int y, int width, int height, float maxPower)
        {
            addTag(tag);
            Transform.X = x;
            Transform.Y = y;
            Transform.Wid = width;
            Transform.Ht = height;
            
            this.maxPower = maxPower;
            charge = FlipperDirection.Stop;
        }

        public override void initialize()
        {

            setPhysicsEnabled();
            MyBody.Kinematic = false;
            MyBody.PassThrough = true;
            Collider c = MyBody.addRectCollider();
        }

        public override void update()
        {
            if(charge == FlipperDirection.Up)
            {
                if(power < maxPower)
                {
                    power += 50*(float)Bootstrap.getDeltaTime();
                }
                else
                {
                    power = maxPower;
                }
            }
            else
            {
                if(power > 0.1f)
                {
                    power -= 100*(float)Bootstrap.getDeltaTime();
                }
                else
                {
                    power = 0;
                }
            }
            if (power != 0)
            {

                Display d = Bootstrap.getDisplay();

                d.renderGeometry([

                    new Vector2(Transform.X, Transform.Y - 40*power/maxPower),
                    new Vector2(Transform.X + Transform.Wid, Transform.Y  - 40 * power / maxPower),
                    new Vector2(Transform.X + Transform.Wid, Transform.Y + Transform.Ht),
                    new Vector2(Transform.X + Transform.Wid, Transform.Y + Transform.Ht),
                ], Color.GreenYellow);
            }
        }

        public void onCollisionEnter(PhysicsBody x)
        {

        }

        public void onCollisionExit(PhysicsBody x)
        {
        }

        public void onCollisionStay(PhysicsBody x)
        {
            if (x.Parent.checkTag("Ball") && charge == FlipperDirection.Stop && power != 0)
            {

                 Bootstrap.getSound().playSound("spring.wav");
                x.addForce(new Vector2(0, -power));
                power = 0;
            }
        }

        public override string ToString()
        {
            return "Wall: [" + Transform.X + ", " + Transform.Y + "]";
        }
    }
}
