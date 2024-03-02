using GameBreakout;
using Pinball;
using Shard.Shard;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Shard
{
    class GameBreakout : Game
    {
        GameObject top, left, right, bottom;
        Random rand;
        List<Brick> myBricks;
        bool callOnce = false;

        public GameBreakout() : base()
        {
            GameStateManager.getInstance().SetGame(this);
        }
        public override void update()
        {


            Bootstrap.getDisplay().showText("FPS: " + Bootstrap.getFPS(), 10, 10, 12, 255, 255, 255);
            Bootstrap.getDisplay().showText("Delta: " + Bootstrap.getDeltaTime(), 10, 20, 12, 255, 255, 255);

            foreach (Brick b in myBricks)
            {
                if (b != null && b.ToBeDestroyed == false)
                {
                    return;
                }

            }

            createBricks();
        }


        public void createBricks()
        {
            myBricks.Clear();

            for (int i = 0; i < 17; i++)
            {
                for (int j = 0; j < 10; j++)
                {

                    if (i % 2 == 0 || j % 2 == 0)
                    {
                        continue;
                    }

                    Brick br = new Brick(100 + (i * 65), 100 + (j * 33));
                    br.Health = 1 + rand.Next(3);
                    myBricks.Add(br);
                }
            }
            PinballPolygon leftWall = new PinballPolygon("LeftWall", 0, 0, 50, Bootstrap.getDisplay().getHeight());
            PinballPolygon rightWall = new PinballPolygon("RightWall", Bootstrap.getDisplay().getWidth(), 0, 50, Bootstrap.getDisplay().getHeight());
            PinballPolygon topWall = new PinballPolygon("TopWall", 0, -50, Bootstrap.getDisplay().getWidth(), 50);
            PinballPolygon well = new PinballPolygon("Well", 0, Bootstrap.getDisplay().getHeight() , Bootstrap.getDisplay().getWidth(), 50);
        }

        public override void initialize()
        {


            rand = new Random();

            myBricks = new List<Brick>();

            Paddle p = new Paddle();

            Ball b = new Ball();
            b.Transform.X = 50;
            b.Transform.Y = 50;


        }

        public override int getTargetFrameRate()
        {

            return 60;

            
        }
    }



}
