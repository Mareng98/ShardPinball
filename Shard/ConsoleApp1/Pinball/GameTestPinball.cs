using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Pinball;

namespace Shard
{
    internal class GameTestPinball : Game, InputListener
    {

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
            Wall leftWall = new Wall("LeftWall", 0, 0, 20,Bootstrap.getDisplay().getHeight());
            Wall rightWall = new Wall("RightWall", Bootstrap.getDisplay().getWidth() - 20, 0, 20, Bootstrap.getDisplay().getHeight());
            Wall topWall = new Wall("TopWall", 0, 0, Bootstrap.getDisplay().getWidth(), 20);
            Wall bottomWall = new Wall("BottomWall", 0, Bootstrap.getDisplay().getHeight() - 20, Bootstrap.getDisplay().getWidth(), 20);
        }

        public override int getTargetFrameRate()
        {

            return 60;


        }
    }
}
