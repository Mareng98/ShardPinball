using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Pinball;
using Pinball.GameBreakout;
using Shard.Shard;

namespace Shard
{
    internal class GameTestPinball : Game, InputListener
    {
        PinballRectangle r;
        bool test = true;
        List<Obstacle> obstacles;
        public void handleInput(InputEvent inp, string eventType)
        {
        }
        public override void update()
        {
            if(r.Collider != null && test)
            {
                r.Collider.Rotation = 1;
                test = false;
            }
            r.Collider.Rotation = -0.001f;
        }
        public override void initialize()
        {
            Bootstrap.getInput().addListener(this);
            PinballBall b = new PinballBall();
            b.Transform.X = 650;
            b.Transform.Y = 200;

            for(int i = 0; i < 4; i++)
            {
                int offset = 150;
                for(int j = 0; j < 5; j++)
                {
                    Obstacle o = new Obstacle();
                    o.Transform.X = offset + 200 + j * 100;
                    o.Transform.Y = offset + i * 100;
                }
            }
            //Vector2[] vertices = { new Vector2(0, 0), new Vector2(50, 20), new Vector2(60, 30), new Vector2(10, 15) };
            //PinballRectangle r = new PinballRectangle("Rectangle", 100, 100, vertices);
            r = new PinballRectangle("Rectangle",150,150,50,50);
            PinballRectangle ramp = new PinballRectangle("LeftRamp", 50, Bootstrap.getDisplay().getHeight() - 450, [new Vector2(0, 0),
                new Vector2(400, 400),
                new Vector2(0, 400)]);
            PinballRectangle ramp2 = new PinballRectangle("LeftRamp", Bootstrap.getDisplay().getWidth() - 450, Bootstrap.getDisplay().getHeight() - 450, [new Vector2(400, 0),
                new Vector2(0, 400),
                new Vector2(400, 400)]);
            PinballRectangle leftWall = new PinballRectangle("LeftWall", 0,0, 50, Bootstrap.getDisplay().getHeight());
            PinballRectangle rightWall = new PinballRectangle("RightWall", Bootstrap.getDisplay().getWidth() - 50, 0, 50, Bootstrap.getDisplay().getHeight());
            PinballRectangle topWall = new PinballRectangle("TopWall", 0, 0, Bootstrap.getDisplay().getWidth(), 50);
            PinballRectangle bottomWall = new PinballRectangle("BottomWall", 0, Bootstrap.getDisplay().getHeight() - 50, Bootstrap.getDisplay().getWidth(), 50);
            //r2.initialize();
            /*
            Wall leftWall = new Wall("LeftWall", 0, 0, 50,Bootstrap.getDisplay().getHeight());
            Wall rightWall = new Wall("RightWall", Bootstrap.getDisplay().getWidth() - 50, 0, 50, Bootstrap.getDisplay().getHeight());
            Wall topWall = new Wall("TopWall", 0, 0, Bootstrap.getDisplay().getWidth(), 50);
            Wall bottomWall = new Wall("BottomWall", 0, Bootstrap.getDisplay().getHeight() - 50, Bootstrap.getDisplay().getWidth(), 50);*/
        }

        public override int getTargetFrameRate()
        {

            return 60;


        }
    }
}
