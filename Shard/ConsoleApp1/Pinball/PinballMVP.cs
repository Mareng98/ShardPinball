using Pinball.GameBreakout;
using Pinball;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shard
{
    internal class PinballMVP : Game, InputListener
    {
        List<Obstacle> obstacles;
        Flipper leftFlipper;
        Flipper rightFlipper;
        public void handleInput(InputEvent inp, string eventType)
        {
            switch (inp.Key)
            {
                case 80: // 80 left arrow
                    if (eventType.Equals("KeyDown"))
                    {
                        leftFlipper.RotatationDirection = FlipperRotationDirection.Up;
                    }
                    else
                    {
                        leftFlipper.RotatationDirection = FlipperRotationDirection.Stop;
                    }
                    break;
                case 79: // 79 right arrow
                    if (eventType.Equals("KeyDown"))
                    {
                        rightFlipper.RotatationDirection = FlipperRotationDirection.Up;
                    }
                    else
                    {
                        rightFlipper.RotatationDirection = FlipperRotationDirection.Stop;
                    }
                    break;
            }
        }
        public override void update()
        {

        }
        public override void initialize()
        {
            Bootstrap.getInput().addListener(this);


            leftFlipper = new Flipper("Flipper", 500, 800, 100, 30, 20, FlipperSide.Left);
            rightFlipper = new Flipper("Flipper", 660, 800, 100, 20, 30, FlipperSide.Right);

            obstacles = new List<Obstacle>();
            for (int i = 0; i < 4; i++)
            {
                int offset = 220;
                for (int j = 0; j < 5; j++)
                {
                    Obstacle o = new Obstacle();
                    o.Transform.X = offset + 200 + j * 100;
                    o.Transform.Y = offset + i * 100;
                    obstacles.Add(o);
                }
            }
            PinballBall b = new PinballBall();
            b.Transform.X = 620;
            b.Transform.Y = 100;
            //Flipper f4 = new Flipper("Flipper", 850, 400, 80, 40, 20);
            PinballRectangle ramp = new PinballRectangle("LeftRamp", 260, Bootstrap.getDisplay().getHeight() - 480,
                [new Vector2(0, 0),
                    new Vector2(250, 400),
                    new Vector2(0, 400)]);
            PinballRectangle ramp2 = new PinballRectangle("RightRamp", Bootstrap.getDisplay().getWidth() - 510, Bootstrap.getDisplay().getHeight() - 480,
                [new Vector2(250, 0),
                    new Vector2(250, 400),
                    new Vector2(0, 400)
                ]);
            PinballRectangle leftWall = new PinballRectangle("LeftWall", 210, 0, 50, Bootstrap.getDisplay().getHeight());
            PinballRectangle rightWall = new PinballRectangle("RightWall", Bootstrap.getDisplay().getWidth() - 260, 0, 50, Bootstrap.getDisplay().getHeight());
            PinballRectangle topWall = new PinballRectangle("TopWall", 0, 0, Bootstrap.getDisplay().getWidth(), 50);
            PinballRectangle well = new PinballRectangle("Well", 0, Bootstrap.getDisplay().getHeight() - 50, Bootstrap.getDisplay().getWidth(), 50);
        }

        public override int getTargetFrameRate()
        {

            return 200;


        }
    }
    }
