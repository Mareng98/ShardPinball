using Pinball;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shard.Pinball
{
    internal class TouchSensorArray : GameObject
    {
        Display d = Bootstrap.getDisplay();
        private Vector2 position;
        private ScoreKeeper scoreKeeper;
        List<TouchSensor> sensors;
        private class TouchSensor : GameObject, CollisionHandler
        {
            Display d = Bootstrap.getDisplay();
            private Vector2 pos;
            private int rad;
            public bool touched = false;
            private Color onColor;
            private Color offColor;
            private string text;

            internal TouchSensor(Vector2 pos, int rad, Color onColor, Color offColor, string text)
            {
                this.pos = pos;
                this.rad = rad;
                this.touched = false;
                this.onColor= onColor;
                this.offColor= offColor;
                this.text = text;
                setPhysicsEnabled();
                MyBody.ReflectOnCollision = false;
                MyBody.PassThrough = true;
                MyBody.addCircleCollider((int)pos.X,(int)pos.Y,rad);
            }
            public override void update()
            {
                d.drawCircle((int)pos.X, (int)pos.Y, rad + 1, Color.Black);
                if (touched)
                {

                    d.drawFilledCircle((int)pos.X, (int)pos.Y, rad, onColor);

                    d.showText(text, pos.X - rad / 2, pos.Y - rad / 2 - 4, rad * 2 - 1, Color.Orange);
                }
                else
                {
                    d.drawFilledCircle((int)pos.X, (int)pos.Y, rad, offColor);
                    
                    d.showText(text, pos.X - rad/2, pos.Y - rad/2 - 4, rad*2 - 1, Color.LightGray);
                }
            }

            public void onCollisionEnter(PhysicsBody x)
            {
                if (x.Parent.checkTag("Ball") && !touched)
                {
                    Bootstrap.getSound().playSound("pointIncrease.wav");
                    touched = true;
                }
            }

            public void onCollisionExit(PhysicsBody x)
            {
            }

            public void onCollisionStay(PhysicsBody x)
            {
            }
        }
        public TouchSensorArray( ScoreKeeper scoreKeeper, Vector2 pos, Vector2 direction, int sensorRad, Color onColor, Color offColor, string text)
        {
            position = pos;
            sensors = new List<TouchSensor>();
            foreach(char c in text)
            {
                if(!c.Equals(' '))
                {
                    sensors.Add(new TouchSensor(pos,sensorRad,onColor,offColor,c.ToString()));
                }
                pos += Vector2.Normalize(direction)*sensorRad*2*2;
            }
            this.scoreKeeper = scoreKeeper;
        }

        public static int GetPixelLength(string text, int rad)
        {
            return (int) (rad * 2 * 2)*text.Length;
        }

        public override void update()
        {
            foreach(TouchSensor sensor in sensors)
            {
                if (!sensor.touched)
                {
                    return;
                }
            }
            scoreKeeper.AddScore(position, 1000);
            foreach(TouchSensor sensor in sensors)
            {
                sensor.touched = false;
            }
        }
    }
}
