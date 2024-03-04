using Shard.Pinball;
using System;
using System.Collections.Generic;
namespace Shard
{
    class Highscore : Game, InputListener
    {
        List<GameObject> gameObjsToDraw = new();
        Dictionary<GameObject, ButtonState> buttonStates = new();

        public Highscore() : base() {}

        public void handleInput(InputEvent inp, string eventType)
        {
        }

        public override void initialize()
        {
            Display disp = Bootstrap.getDisplay();

            GameObject background = new();
            background.Transform.SpritePath = getAssetManager().getAssetPath("pinball_blurred.png");
            background.Transform.X = 0f;
            background.Transform.Y = 0f;
            gameObjsToDraw.Add(background);
        }

        public override void update()
        {
            foreach(var gameObj in gameObjsToDraw)
            {
                Bootstrap.getDisplay().addToDraw(gameObj);
            }
        }
    }
}
