
using Pinball;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using Shard.Pinball;
using Shard.Shard;

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
        ScoreKeeper scoreKeeper;

        Vector2 initialBallPosition = new Vector2(1008, 964);

        public PinballMVP() : base() {}
        public void handleInput(InputEvent inp, string eventType)
        {
            switch (inp.Key)
            {
                case 80: // 80 left arrow
                    if (eventType.Equals("KeyDown") && leftFlipper.RotatationDirection == FlipperDirection.Stop)
                    {
                        Bootstrap.getSound().playSound("mechanical.wav");
                        leftFlipper.RotatationDirection = FlipperDirection.Up;
                    }
                    else if(eventType.Equals("KeyUp"))
                    {
                        leftFlipper.RotatationDirection = FlipperDirection.Stop;
                    }
                    break;
                case 79: // 79 right arrow
                    if (eventType.Equals("KeyDown") && rightFlipper.RotatationDirection == FlipperDirection.Stop)
                    {
                        Bootstrap.getSound().playSound("mechanical.wav");
                        rightFlipper.RotatationDirection = FlipperDirection.Up;
                    }
                    else if (eventType.Equals("KeyUp"))
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
            Bootstrap.AddLightObject((int)ball.Transform.Centre.X, (int)ball.Transform.Centre.Y, 40, Color.FromArgb(175, 237, 223, 128));
            display.renderGeometry([new Vector2(0,0),
                new Vector2(display.getWidth(),0),
                new Vector2(display.getWidth(),display.getHeight()),
                new Vector2(0, display.getHeight())
            ], Color.DarkSlateGray);
        }
        public override void initialize()
        {
            //Bootstrap.SetShadowColor(Color.FromArgb(200, 0, 0, 0));
            //Bootstrap.EnableLight();
            int arenaWidth = 825;
            int arenaHeight = 1080;
            int arenaMinX = 250;
            int arenaMaxX = arenaMinX + arenaWidth;
            Bootstrap.getInput().addListener(this);

            leftFlipper = new Flipper("Flipper", 505, arenaHeight-65, 107, 32, 22, FlipperSide.Left);
            rightFlipper = new Flipper("Flipper", 654, arenaHeight-65, 107, 22, 32, FlipperSide.Right);
            obstacles = new List<Obstacle>();
            scoreKeeper = new ScoreKeeper(new Vector2(20, 20), 60, 0);
            string sensorArrayText = "FLIPPER";
            int sensorArrayRad = 15;
            int sensorArrayWidth = TouchSensorArray.GetPixelLength(sensorArrayText, sensorArrayRad);
            TouchSensorArray sensorArray = new TouchSensorArray(scoreKeeper, 
                new Vector2((arenaMaxX + arenaMinX) / 2 - sensorArrayWidth/2, 700), 
                new Vector2(1, 0), sensorArrayRad, 
                Color.Wheat, Color.DarkSalmon, 
                sensorArrayText);

            // Add obstacles in bottom corners
            ObstacleTypes blue = ObstacleTypes.SimpleBlue;
            ObstacleTypes red = ObstacleTypes.SimpleRed;
            Obstacle bottomLeftCornerOb = new Obstacle(red, scoreKeeper);
            Obstacle bottomRightCornerOb = new Obstacle(red, scoreKeeper);
            bottomLeftCornerOb.Transform.X = arenaMinX + 90;
            bottomLeftCornerOb.Transform.Y = 720;
            bottomRightCornerOb.Transform.X = arenaMaxX - 165;
            bottomRightCornerOb.Transform.Y = 720;
            // Add obstacles next to left slide
            Obstacle slideOb1 = new Obstacle(red, scoreKeeper);
            Obstacle slideOb2 = new Obstacle(red, scoreKeeper);
            Obstacle slideOb3 = new Obstacle(red, scoreKeeper);
            Obstacle slideOb4 = new Obstacle(blue, scoreKeeper);

            slideOb1.Transform.X = arenaMinX + 260;
            slideOb1.Transform.Y = 100;
            slideOb2.Transform.X = arenaMinX + 223;
            slideOb2.Transform.Y = 170;
            slideOb3.Transform.X = arenaMinX + 187;
            slideOb3.Transform.Y = 240;
            slideOb4.Transform.X = arenaMinX + 150;
            slideOb4.Transform.Y = 310;

            // Add obstacles middle
            Obstacle middleOb1 = new Obstacle(blue, scoreKeeper);
            Obstacle middleOb2 = new Obstacle(blue, scoreKeeper);
            Obstacle middleOb3 = new Obstacle(blue, scoreKeeper);

            middleOb1.Transform.X = arenaMinX + 560 ;
            middleOb1.Transform.Y = 200;
            middleOb2.Transform.X = arenaMinX + 650;
            middleOb2.Transform.Y = 270;
            middleOb3.Transform.X = arenaMinX + 500;
            middleOb3.Transform.Y = 300;

            lifeBar = new LifeBar(0,1200, 20);
            ball = new PinballBall("Ball",(int)initialBallPosition.X,(int)initialBallPosition.Y, Vector2.Zero);

            PinballPolygon arena = new PinballPolygon("Arena", arenaMinX, 0,
                    [new Vector2(0, 0),
                        new Vector2(arenaWidth, 0),
                        new Vector2(arenaWidth, arenaHeight),
                        new Vector2(arenaWidth - 100, arenaHeight),
                        new Vector2(arenaWidth - 100, arenaHeight - 100),
                        new Vector2(arenaWidth - 50, arenaHeight - 100),
                        // Rounded right corner
                        new Vector2(arenaWidth - 50, 250),
                        new Vector2(arenaWidth - 51, 229),
                        new Vector2(arenaWidth - 54, 208),
                        new Vector2(arenaWidth - 60, 188),
                        new Vector2(arenaWidth - 67, 169),
                        new Vector2(arenaWidth - 77, 150),
                        new Vector2(arenaWidth - 88, 132),
                        new Vector2(arenaWidth - 101, 116),
                        new Vector2(arenaWidth - 116, 101),
                        new Vector2(arenaWidth - 132, 88),
                        new Vector2(arenaWidth - 150, 77),
                        new Vector2(arenaWidth - 169, 67),
                        new Vector2(arenaWidth - 188, 60),
                        new Vector2(arenaWidth - 208, 54),
                        new Vector2(arenaWidth - 229, 51),

                        // Middle dip down
                        new Vector2(600, 51),
                        new Vector2(520, 70),
                        new Vector2(330, 70),
                        new Vector2(250, 50),
                        // Rounded left corner
                        new Vector2(237, 50),
                        new Vector2(225, 52),
                        new Vector2(213, 54),
                        new Vector2(200, 56),
                        new Vector2(188, 60),
                        new Vector2(176, 64),
                        new Vector2(165, 69),
                        new Vector2(154, 75),
                        new Vector2(143, 81),
                        new Vector2(132, 88),
                        new Vector2(123, 96),
                        new Vector2(113, 104),
                        new Vector2(104, 113),
                        new Vector2(96, 123),
                        new Vector2(88, 132),
                        new Vector2(81, 143),
                        new Vector2(75, 154),
                        new Vector2(69, 165),
                        new Vector2(64, 176),
                        new Vector2(60, 188),
                        new Vector2(56, 200),
                        new Vector2(54, 213),
                        new Vector2(53, 225),
                        new Vector2(55, 237),

                        // Slide ledge
                        new Vector2(56, 240),
                        new Vector2(57, 242),
                        new Vector2(58, 244),
                        new Vector2(100, 350),

                        // Bottomleft ramp rounded corner
                        new Vector2(50, 768),
                        new Vector2(51, 772),
                        new Vector2(52, 775),
                        new Vector2(54, 777),
                        new Vector2(55, 779),
                        new Vector2(57, 780),

                        // Bottomeleft ramp
                        new Vector2(257, arenaHeight - 70),
                        new Vector2(257, arenaHeight - 50),
                        new Vector2(0, arenaHeight - 50),
                    ]
            );
            PinballPolygon gutterDivider = new PinballPolygon("gutterDivider", arenaMinX + 3, -3,
                    [
                        // Rounded corner
                        new Vector2(arenaWidth + -95, 163),
                        new Vector2(arenaWidth + -89, 174),
                        new Vector2(arenaWidth + -83, 185),
                        new Vector2(arenaWidth + -78, 196),
                        new Vector2(arenaWidth + -74, 208),
                        new Vector2(arenaWidth + -70, 220),
                        // Bottomright ramp
                        new Vector2(arenaWidth - 70, arenaHeight - 50),
                        new Vector2(arenaWidth - 302, arenaHeight - 50),
                        new Vector2(arenaWidth - 302, arenaHeight - 70),
                        new Vector2(arenaWidth - 115, arenaHeight - 280), // 95, 300
                        new Vector2(725, 784),
                        new Vector2(726, 782),
                        new Vector2(728, 780),
                        new Vector2(729, 777),
                        new Vector2(730, 773),
                    ]
            );
            PinballPolygon gutterSlide = new PinballPolygon("gutterSlide", arenaMinX + 3 +95, -3 + 95,
                [
                    new Vector2(36, 240),
                    new Vector2(35, 239),
                    new Vector2(33, 238),
                    new Vector2(0, 150),
                    new Vector2(2, 127),
                    new Vector2(7, 104),
                    new Vector2(16, 82),
                    new Vector2(29, 62),
                    new Vector2(44, 44),
                    new Vector2(62, 29),
                    new Vector2(82, 16),
                    new Vector2(104, 7),
                    new Vector2(127, 2),
                    new Vector2(150, 1),
                    new Vector2(153, 0),
                    new Vector2(155, 1),
                ]
            );

            PinballPolygon middleBarrier = new PinballPolygon("middleBarrier", arenaMinX+ 500, 500,
                [
                    new Vector2(2, 28),
                    new Vector2(0, 24),
                    new Vector2(0, 20),
                    new Vector2(0, 16),
                    new Vector2(2, 12),
                    new Vector2(4, 8),
                    new Vector2(7, 5),
                    new Vector2(10, 3),
                    new Vector2(14, 1),
                    new Vector2(18, 0),
                    new Vector2(22, 0),
                    new Vector2(26, 1),
                    new Vector2(30, 3),
                    new Vector2(33, 5),
                    new Vector2(36, 8),
                    new Vector2(38, 12),
                    new Vector2(100, 120),
                    new Vector2(100, 124),
                    new Vector2(98, 128),
                    new Vector2(96, 132),
                    new Vector2(93, 135),
                    new Vector2(90, 137),
                    new Vector2(86, 139),
                    new Vector2(82, 140),
                    new Vector2(78, 140),
                    new Vector2(74, 139),
                    new Vector2(70, 137),
                    new Vector2(67, 135),
                ]
            );

            List<PinballPolygon> pins = new List<PinballPolygon>();
            for (int i = 0; i < 3; i++)
            {
                PinballPolygon pin = new PinballPolygon("pin" + i, (arenaMaxX + arenaMinX) / 2 - sensorArrayWidth / 2 + 135 + 80*i, 90,
                [
                new Vector2(20, 10),
                new Vector2(17, 17),
                new Vector2(10, 20),
                new Vector2(3, 17),
                new Vector2(0, 10),
                new Vector2(3, 3),
                new Vector2(10, 0),
                new Vector2(17, 3),
                ]
                );
                pins.Add(pin);
            }


            Well well = new Well("Well", 10, Bootstrap.getDisplay().getHeight() - 50, Bootstrap.getDisplay().getWidth(), 200, this);
            flipperSpring = new Spring("Spring", (int)initialBallPosition.X - 10, (int)initialBallPosition.Y - 20, 40, 40, 35);

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
                GameOver gameOver = new GameOver(scoreKeeper.Score);
                GameStateManager.getInstance().SetGame(gameOver);
                gameOver.initialize();
                Bootstrap.getInput().removeListener(this);
                Bootstrap.DisableLight();
                // Game over
            }
        }

        public override int getTargetFrameRate()
        {
            return 200;
        }
    }
}
