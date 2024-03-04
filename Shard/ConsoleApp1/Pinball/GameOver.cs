using Shard.Pinball;
using Shard.Shard;
using System;
using System.Collections.Generic;

namespace Shard
{
    class GameOver : Game, InputListener
    {
        List<GameObject> gameObjsToDraw = new();
        Dictionary<GameObject, ButtonState> buttonStates = new();
        public GameOver() : base() { }
        public void handleInput(InputEvent inp, string eventType)
        {
            foreach (var button in buttonStates.Keys)
            {
                Transform t = button.Transform;
                bool isMouseInsideButton = GeometryUtils.Contains(inp.X, inp.Y, (int)t.X, (int)t.Y, t.Wid, t.Ht);
                if (eventType.Equals("MouseDown"))
                {
                    if (isMouseInsideButton)
                    {
                        if (buttonStates[button].Tag == "Exit")
                        {
                            Environment.Exit(0);
                        } else if (buttonStates[button].Tag == "Back")
                        {
                            Game game = new MainMenu();
                            GameStateManager.getInstance().SetGame(game);
                            game.initialize();
                            Bootstrap.getInput().removeListener(this);
                        }
                   }
                } else if (eventType.Equals("MouseMotion"))
                {
                    if (isMouseInsideButton)
                    {
                        buttonStates[button].IsHovered = true;
                    } 
                    else
                    {
                        buttonStates[button].IsHovered = false;
                    }
                    t.SpritePath = buttonStates[button].getButtonAsset();
                }
            }
        }


        public override void initialize()
        {
            Display disp = Bootstrap.getDisplay();

            GameObject background = new();
            background.Transform.SpritePath = getAssetManager().getAssetPath("pinball_blurred.png");
            background.Transform.X = 0f;
            background.Transform.Y = 0f;
            gameObjsToDraw.Add(background);

            var backButtonState = new ButtonState("Back", "back.png", "back_hovered.png", null);
            backButtonState.CreateButton(disp.getWidth() / 2 - 100, disp.getHeight() - 300, ref gameObjsToDraw, ref buttonStates);

            Bootstrap.getInput().addListener(this);
        }

        public override void update()
        {
            foreach(var gameObj in gameObjsToDraw)
            {
                Bootstrap.getDisplay().addToDraw(gameObj);
            }
        }
        public override int getTargetFrameRate()
        {
            return 30;
        }
    }
}
