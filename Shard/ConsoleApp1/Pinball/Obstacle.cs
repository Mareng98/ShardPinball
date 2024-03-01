using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Shard;

namespace Pinball
{
    class Obstacle : GameObject, InputListener, CollisionHandler
    {
        private ObstacleTypes obstacleType;
        private string obstacleLightOnPath;
        private string obstacleLightOffPath;
        private int obstacleLightOnDuration = 15;
        private int lightDuration = 0;
        public Obstacle(ObstacleTypes type)
        {
            if(type == ObstacleTypes.SimpleBlue)
            {
                obstacleLightOnPath = "blueObstacleOn.png";
                obstacleLightOffPath = "blueObstacleOff.png";
            }
            else if(type == ObstacleTypes.SimpleRed)
            {
                obstacleLightOnPath = "redObstacleOn.png";
                obstacleLightOffPath = "redObstacleOff.png";
            }
            ObstacleLightOff();
            setPhysicsEnabled();
            Transform.Scalex = 2;
            Transform.Scaley = 2;
            Transform.Wid = 6;
            Transform.Ht = 6;
            MyBody.Mass = 1;
            MyBody.MaxForce = 15000;
            MyBody.Drag = 0f;
            MyBody.UsesGravity = false;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = false;
            MyBody.addCircleCollider();
            MyBody.FrictionCoefficient = 0.04f;
            MyBody.Force = new Vector2(0, 0);
            addTag("Obstacle");
        }

        private void ObstacleLightOff()
        {
            if(lightDuration > 0)
            {
                lightDuration -= 1;
            }
            else
            {
                Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath(obstacleLightOffPath);
            }
           
        }

        private void ObstacleLightOn()
        {
            Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath(obstacleLightOnPath);
            lightDuration = obstacleLightOnDuration;
        }

        public override void initialize()
        {




        }

        public void handleInput(InputEvent inp, string eventType)
        {




        }


        public override void update()
        {

            ObstacleLightOff();
            Bootstrap.getDisplay().addToDraw(this);
        }

        public void onCollisionEnter(PhysicsBody x)
        {
            if (x.Parent.checkTag("Ball"))
            {
                ObstacleLightOn();
            }
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

