
using Pinball;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;

namespace Shard
{
    internal class PinballMVP : Game, InputListener
    {
        Display display = Bootstrap.getDisplay();
        List<Obstacle> obstacles;
        Flipper leftFlipper;
        Flipper rightFlipper;
        LifeBar lifeBar;
        PinballBall ball;
        Spring flipperSpring;
        Vector2 initialBallPosition = new Vector2(1008, 964);
        public void handleInput(InputEvent inp, string eventType)
        {
            switch (inp.Key)
            {
                case 80: // 80 left arrow
                    if (eventType.Equals("KeyDown"))
                    {
                        leftFlipper.RotatationDirection = FlipperDirection.Up;
                    }
                    else
                    {
                        leftFlipper.RotatationDirection = FlipperDirection.Stop;
                    }
                    break;
                case 79: // 79 right arrow
                    if (eventType.Equals("KeyDown"))
                    {
                        rightFlipper.RotatationDirection = FlipperDirection.Up;
                    }
                    else
                    {
                        rightFlipper.RotatationDirection = FlipperDirection.Stop;
                    }
                    break;
                case 44: // Space bar
                    if (eventType.Equals("KeyDown"))
                    {
                        flipperSpring.Charge = FlipperDirection.Up;
                    }
                    else
                    {
                        flipperSpring.Charge = FlipperDirection.Stop;
                    }
                    break;
            }
        }
        public override void update()
        {
            display.renderGeometry([new Vector2(0,0),
                new Vector2(display.getWidth(),0),
                new Vector2(display.getWidth(),display.getHeight()),
                new Vector2(0, display.getHeight())
            ], Color.DarkSlateGray);
        }
        public override void initialize()
        {
            Bootstrap.getInput().addListener(this);

            int arenaWidth = 825;
            int arenaHeight = 1080;
            leftFlipper = new Flipper("Flipper", 505, arenaHeight-65, 100, 30, 20, FlipperSide.Left);
            rightFlipper = new Flipper("Flipper", 655, arenaHeight-65, 100, 20, 30, FlipperSide.Right);
            obstacles = new List<Obstacle>();
            for (int i = 0; i < 4; i++)
            {
                int offset = 220;
                for (int j = 0; j < 5; j++)
                {
                    ObstacleTypes ot;
                    if((j+i)%2 == 0)
                    {
                        ot = ObstacleTypes.SimpleBlue;
                    }
                    else
                    {
                        ot = ObstacleTypes.SimpleRed;
                    }
                    Obstacle o = new Obstacle(ot);
                    o.Transform.X = offset + 200 + j * 100;
                    o.Transform.Y = offset + i * 100;
                    obstacles.Add(o);
                }
            }
            lifeBar = new LifeBar(3,1200, 20);
            ball = new PinballBall("Ball",(int)initialBallPosition.X,(int)initialBallPosition.Y, Vector2.Zero);
            /*PinballRectangle ramp = new PinballRectangle("LeftRamp", 210, Bootstrap.getDisplay().getHeight() - 480,
                [new Vector2(0, 0),
                    new Vector2(300, 400),
                    new Vector2(0, 400)]);
            PinballRectangle ramp2 = new PinballRectangle("RightRamp", Bootstrap.getDisplay().getWidth() - 510, Bootstrap.getDisplay().getHeight() - 480,
                [new Vector2(300, 0),
                    new Vector2(300, 400),
                    new Vector2(0, 400)
                ]);

            PinballRectangle wellish = new PinballRectangle("Test", 500, Bootstrap.getDisplay().getHeight() - 280,
                [new Vector2(0,0),
                    new Vector2(150,100),
                    new Vector2(300,0),
                    new Vector2(300,200),
                    new Vector2(0,200),
                ]);*/

            PinballPolygon arena = new PinballPolygon("Arena", 250, 0,
                [new Vector2(0, 0),
                    new Vector2(arenaWidth, 0),
                    new Vector2(arenaWidth, arenaHeight),
                    new Vector2(arenaWidth - 100, arenaHeight),
                    new Vector2(arenaWidth - 100, arenaHeight-100),
                    new Vector2(arenaWidth - 50, arenaHeight - 100),
                    // Rounded right corner
                    new Vector2(arenaWidth - 50, 94),
                    new Vector2(arenaWidth - 51, 89),
                    new Vector2(arenaWidth - 53, 83),
                    new Vector2(arenaWidth - 55, 78),
                    new Vector2(arenaWidth - 58, 73),
                    new Vector2(arenaWidth - 61, 69),
                    new Vector2(arenaWidth - 65, 65),
                    new Vector2(arenaWidth - 69, 61),
                    new Vector2(arenaWidth - 73, 58),
                    new Vector2(arenaWidth - 78, 55),
                    new Vector2(arenaWidth - 83, 53),
                    new Vector2(arenaWidth - 89, 51),
                    new Vector2(arenaWidth - 94, 50),
                    // Rounded left corner
                    new Vector2(100, 50),
                    new Vector2(94, 50),
                    new Vector2(89, 51),
                    new Vector2(83, 53),
                    new Vector2(78, 55),
                    new Vector2(73, 58),
                    new Vector2(69, 61),
                    new Vector2(65, 65),
                    new Vector2(61, 69),
                    new Vector2(58, 73),
                    new Vector2(55, 78),
                    new Vector2(53, 83),
                    new Vector2(51, 89),
                    new Vector2(50, 94),
                    new Vector2(50, 100),



                    // Bottomleft ramp
                    new Vector2(50, arenaHeight-300),
                    new Vector2(257, arenaHeight - 70),
                    new Vector2(257, arenaHeight-50),
                    new Vector2(0, arenaHeight-50),
                ]);
            PinballPolygon gutterDivider = new PinballPolygon("gutterDivider", 253, -3,
                    [
                        // Rounded corner
                        new Vector2(arenaWidth - 95, 70),
                        new Vector2(arenaWidth - 90, 72),
                        new Vector2(arenaWidth - 85, 74),
                        new Vector2(arenaWidth - 81, 77),
                        new Vector2(arenaWidth - 77, 81),
                        new Vector2(arenaWidth - 74, 85),
                        new Vector2(arenaWidth - 72, 90),
                        new Vector2(arenaWidth - 70, 95),
                        // Bottomright ramp
                        new Vector2(arenaWidth - 70, arenaHeight-50),
                        new Vector2(arenaWidth - 310, arenaHeight-50),
                        new Vector2(arenaWidth - 310, arenaHeight - 70),
                        new Vector2(arenaWidth - 95, arenaHeight-300),
                    ]
                );
            /*PinballRectangle leftWall = new PinballRectangle("LeftWall", 210, 0, 50, Bootstrap.getDisplay().getHeight());
            PinballRectangle rightWall = new PinballRectangle("RightWall", Bootstrap.getDisplay().getWidth() - 260, 0, 50, Bootstrap.getDisplay().getHeight());
            PinballRectangle topWall = new PinballRectangle("TopWall", 0, 0, Bootstrap.getDisplay().getWidth(), 50);*/
            Well well = new Well("Well", 0, Bootstrap.getDisplay().getHeight() - 50, Bootstrap.getDisplay().getWidth(), 50, this);

            flipperSpring = new Spring("Spring", (int)initialBallPosition.X - 10, (int)initialBallPosition.Y - 20, 40, 40, 30);
        }

        public void ResetBall()
        {
            if (lifeBar.ReduceLives())
            {
                // Lose a life
                ball = new PinballBall("Ball",(int)initialBallPosition.X, (int)initialBallPosition.Y, Vector2.Zero);
            }
            else
            {
                // Game over
            }
        }

        public override int getTargetFrameRate()
        {

            return 200;


        }
    }
    }
