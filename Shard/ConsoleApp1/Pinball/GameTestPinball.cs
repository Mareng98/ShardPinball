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
            b.Transform.X = 50;
            b.Transform.Y = 50;
            // Bootstrap.getDisplay().getHeight()
            // Bootstrap.getDisplay().getWidth()
            Wall leftWall = new Wall("LeftWall", 0, 0, 20,Bootstrap.getDisplay().getHeight());
            Wall topWall = new Wall("TopWall", 0, 0, Bootstrap.getDisplay().getWidth(), 20);
            Wall rightWall = new Wall("RightWall", Bootstrap.getDisplay().getWidth() - 20, 0, 20, Bootstrap.getDisplay().getHeight());
            Wall bottomWall = new Wall("BottomWall", 0, Bootstrap.getDisplay().getHeight() - 20, Bootstrap.getDisplay().getWidth(), 20);
        }

        public override int getTargetFrameRate()
        {

            return 60;


        }
    }
}
