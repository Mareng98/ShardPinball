

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using Plinko;
using Shard.Pinball;
using Shard.Shard;

namespace Shard
{
    internal class Plinko : Game, InputListener
    {
        Display display = Bootstrap.getDisplay();
        List<Obstacle> obstacles;
        PlinkoBall ball;
        List<PlinkoBall> balls;
        Vector2 initialBallPosition = new Vector2(1008, 964);
        PlinkoPolygon rotator;

        public Plinko() : base() { }
        public void handleInput(InputEvent inp, string eventType)
        {

            if (eventType.Equals("MouseDown"))
            {
                balls.Add(new PlinkoBall("test", inp.X, inp.Y, Vector2.Zero));
            }
        }

        // Plinko Simulation

        public override void update()
        {
        }
        public override void initialize()
        {
            balls = new List<PlinkoBall>();

            int arenaWidth = 825;
            int arenaHeight = 1080;
            int arenaMinX = 250;
            int arenaMaxX = arenaMinX + arenaWidth;
            Bootstrap.getInput().addListener(this);
            obstacles = new List<Obstacle>();


            for (int i = 0; i < 20; i++)
            {
                int offset = 30;
                for (int j = 0; j < i; j++)
                {
                    Obstacle o = new Obstacle();
                    o.Transform.X = -offset*i + offset*j*2 + 650 - 5;
                    o.Transform.Y = offset + i * offset+180;
                    obstacles.Add(o);
                }
            }


            List<Vector2> someList = new List<Vector2>();
            int slotWidth = 30;
            int slotHeight = 200;
            int slotLength = 20;
            bool up = true;
            bool justSwitched;
            int indexToUse = 0;
            for (int i = 1; i < slotLength*4;i++)
            {
                indexToUse++;
                justSwitched = false;
                if (i % 2 == 0)
                {
                    indexToUse--;
                    up = !up;
                    justSwitched = true;
                }
                if (up)
                {
                    if (justSwitched)
                    {
                        someList.Add(new Vector2(slotWidth * indexToUse, 0));
                    }
                    else
                    {
                        someList.Add(new Vector2(slotWidth * indexToUse - 20, 0));
                    }
                }
                else
                {
                    if (justSwitched)
                    {
                        someList.Add(new Vector2(slotWidth * indexToUse - 20, 200));
                    }
                    else
                    {
                        someList.Add(new Vector2(slotWidth * indexToUse, 200));
                    }
                }
            }
            someList.Add(new Vector2(1200, 0));
            someList.Add(new Vector2(1210, 0));
            someList.Add(new Vector2(1210, 250));
            someList.Add(new Vector2(0, 250));
            someList.Add(new Vector2(0, 0));
            PlinkoPolygon well = new PlinkoPolygon("test", 20, 800,
                someList.ToArray()
                );
        }
        // Rotation simulation
        /*
        public override void update()
        {
            rotator.MyBody.AngularVelocity = 0.5f;
            display.renderGeometry([new Vector2(0, 0),
                new Vector2(display.getWidth(), 0),
                new Vector2(display.getWidth(), display.getHeight()),
                new Vector2(0, display.getHeight())
            ], Color.DarkSlateGray);
        }
        
        public override void initialize()
        {
            balls = new List<PlinkoBall>();

            int arenaWidth = 825;
            int arenaHeight = 1080;
            int arenaMinX = 250;
            int arenaMaxX = arenaMinX + arenaWidth;
            Bootstrap.getInput().addListener(this);


            Vector2[] topRight = [
                new Vector2(600, 237),
                new Vector2(800, 237),
                new Vector2(800, 600),
                new Vector2(0, 600),
                new Vector2(0, -100),
                new Vector2(800, -100),
                new Vector2(800, 237),
                new Vector2(600, 237),
                new Vector2(598, 225),
                new Vector2(596, 213),
                new Vector2(594, 200),
                new Vector2(590, 188),
                new Vector2(586, 176),
                new Vector2(581, 165),
                new Vector2(575, 154),
                new Vector2(569, 143),
                new Vector2(562, 132),
                new Vector2(554, 123),
                new Vector2(546, 113),
                new Vector2(537, 104),
                new Vector2(527, 96),
                new Vector2(518, 88),
                new Vector2(507, 81),
                new Vector2(496, 75),
                new Vector2(485, 69),
                new Vector2(474, 64),
                new Vector2(462, 60),
                new Vector2(450, 56),
                new Vector2(437, 54),
                new Vector2(425, 52),
                new Vector2(413, 50),
                new Vector2(400, 50),
                new Vector2(387, 50),
                new Vector2(375, 52),
                new Vector2(363, 54),
                new Vector2(350, 56),
                new Vector2(338, 60),
                new Vector2(326, 64),
                new Vector2(315, 69),
                new Vector2(304, 75),
                new Vector2(293, 81),
                new Vector2(282, 88),
                new Vector2(273, 96),
                new Vector2(263, 104),
                new Vector2(254, 113),
                new Vector2(246, 123),
                new Vector2(238, 132),
                new Vector2(231, 143),
                new Vector2(225, 154),
                new Vector2(219, 165),
                new Vector2(214, 176),
                new Vector2(210, 188),
                new Vector2(206, 200),
                new Vector2(204, 213),
                new Vector2(202, 225),
                new Vector2(200, 237),
                new Vector2(200, 250),
                new Vector2(200, 263),
                new Vector2(202, 275),
                new Vector2(204, 287),
                new Vector2(206, 300),
                new Vector2(210, 312),
                new Vector2(214, 324),
                new Vector2(219, 335),
                new Vector2(225, 346),
                new Vector2(231, 357),
                new Vector2(238, 368),
                new Vector2(246, 377),
                new Vector2(254, 387),
                new Vector2(263, 396),
                new Vector2(273, 404),
                new Vector2(282, 412),
                new Vector2(293, 419),
                new Vector2(304, 425),
                new Vector2(315, 431),
                new Vector2(326, 436),
                new Vector2(338, 440),
                new Vector2(350, 444),
                new Vector2(363, 446),
                new Vector2(375, 448),
                new Vector2(387, 450),
                new Vector2(400, 450),
                new Vector2(413, 450),
                new Vector2(425, 448),
                new Vector2(437, 446),
                new Vector2(450, 444),
                new Vector2(462, 440),
                new Vector2(474, 436),
                new Vector2(485, 431),
                new Vector2(496, 425),
                new Vector2(507, 419),
                new Vector2(518, 412),
                new Vector2(527, 404),
                new Vector2(537, 396),
                new Vector2(546, 387),
                new Vector2(554, 377),
                new Vector2(562, 368),
                new Vector2(569, 357),
                new Vector2(575, 346),
                new Vector2(581, 335),
                new Vector2(586, 324),
                new Vector2(590, 312),
                new Vector2(594, 300),
                new Vector2(596, 287),
                new Vector2(598, 275),
                new Vector2(600, 263),
                new Vector2(600, 250),


            ];
            rotator = new PlinkoPolygon("rotator", 590,430, 24, 440);
            rotator.Transform.Pivot = new Vector2(590 + 12, 455 + 195);
            PlinkoPolygon test2 = new PlinkoPolygon("Test", 200, 400, topRight);
        }*/

        public override int getTargetFrameRate()
        {
            return 200;
        }
    }
}
