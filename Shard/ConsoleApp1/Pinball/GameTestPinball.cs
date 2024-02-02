using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Pinball;
using Shard.Shard;

namespace Shard
{
    internal class GameTestPinball : Game, InputListener
    {
        PinballRectangle r;

        public void handleInput(InputEvent inp, string eventType)
        {
        }
        public override void update()
        {
        }
        public override void initialize()
        {
            Bootstrap.getInput().addListener(this);
            PinballBall b = new PinballBall();
            b.Transform.X = Bootstrap.getDisplay().getWidth() / 2;
            b.Transform.Y = Bootstrap.getDisplay().getHeight() / 2;
            //Vector2[] vertices = { new Vector2(0, 0), new Vector2(50, 20), new Vector2(60, 30), new Vector2(10, 15) };
            //PinballRectangle r = new PinballRectangle("Rectangle", 100, 100, vertices);
            PinballRectangle r2 = new PinballRectangle("Rectangle",150,150,600,600);

            Wall leftWall = new Wall("LeftWall", 0, 0, 50,Bootstrap.getDisplay().getHeight());
            Wall rightWall = new Wall("RightWall", Bootstrap.getDisplay().getWidth() - 50, 0, 50, Bootstrap.getDisplay().getHeight());
            Wall topWall = new Wall("TopWall", 0, 0, Bootstrap.getDisplay().getWidth(), 50);
            Wall bottomWall = new Wall("BottomWall", 0, Bootstrap.getDisplay().getHeight() - 50, Bootstrap.getDisplay().getWidth(), 50);
        }

        public override int getTargetFrameRate()
        {

            return 60;


        }
    }
}
