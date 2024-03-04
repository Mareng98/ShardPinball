using Shard.Pinball;
using System.Collections.Generic;

namespace Shard
{
    class GameOver : Game, InputListener
    {
        List<GameObject> gameObjsToDraw = new();
        Dictionary<GameObject, ButtonState> buttonStates = new();
        public void handleInput(InputEvent inp, string eventType)
        {
        }

        public GameOver() : base() { }

        public override void initialize()
        {
            Display disp = Bootstrap.getDisplay();

            GameObject background = new();
            background.Transform.SpritePath = getAssetManager().getAssetPath("pinball_blurred.png");
            background.Transform.X = 0f;
            background.Transform.Y = 0f;
            gameObjsToDraw.Add(background);
            Bootstrap.getInput().addListener(this);
        }

        public override void update()
        {
        }
        public override int getTargetFrameRate()
        {
            return 200;
        }
    }
}
