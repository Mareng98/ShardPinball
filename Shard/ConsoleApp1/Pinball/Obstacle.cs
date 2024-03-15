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
    class Obstacle : GameObject, CollisionHandler
    {
        ColliderCircle circleCollider;
        private ObstacleTypes obstacleType;
        private string obstacleLightOnPath;
        private string obstacleLightOffPath;
        private int obstacleLightOnDuration = 15;
        private int lightDuration = 0;
        private Random rnd;
        private ScoreKeeper scoreKeeper;
        Display d = Bootstrap.getDisplay();
        public Obstacle(ObstacleTypes type, ScoreKeeper scoreKeeper)
        {
            this.scoreKeeper = scoreKeeper;
            obstacleType = type;
            if(obstacleType == ObstacleTypes.SimpleBlue)
            {
                obstacleLightOnPath = "blueObstacleOn.png";
                obstacleLightOffPath = "blueObstacleOff.png";
            }
            else if(obstacleType == ObstacleTypes.SimpleRed)
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
            MyBody.Drag = 0f;
            MyBody.UsesGravity = false;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = false;
            circleCollider = MyBody.addCircleCollider();
            addTag("Obstacle");
            rnd = new Random();
            MyBody.SetStatic();
        }

        private void ObstacleLightOff()
        {
            if(lightDuration > 0)
            {
                lightDuration -= 1;
                int opacity = 1;
                if(obstacleType == ObstacleTypes.SimpleRed)
                {
                    for (int rad = (int)circleCollider.Rad + 5; rad > circleCollider.Rad; rad--)
                    {
                        opacity++;
                        d.drawCircle((int)circleCollider.X, (int)circleCollider.Y, rad, 255, 80, 80, 20 * opacity);
                    }
                }
                else
                {
                    for (int rad = (int)circleCollider.Rad + 5; rad > circleCollider.Rad; rad--)
                    {
                        opacity++;
                        d.drawCircle((int)circleCollider.X, (int)circleCollider.Y, rad, 80, 80, 255, 20 * opacity);
                    }
                }

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


        public override void update()
        {

            ObstacleLightOff();
            Bootstrap.getDisplay().addToDraw(this);
        }

        public void onCollisionEnter(PhysicsBody x)
        {
            if (x.Parent.checkTag("Ball"))
            {
                if(rnd.Next(3) == 1)
                {
                    Bootstrap.getSound().playSound("obstacle1.wav");
                }
                else
                {

                    Bootstrap.getSound().playSound("obstacle2.wav");
                }
                scoreKeeper.AddScore(x.Parent.Transform.Centre + new Vector2(0,-5), 10);
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

