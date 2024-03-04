using Shard.Pinball;
using Shard.Shard;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace Shard
{
    class MainMenu : Game, InputListener
    {
        List<GameObject> gameObjsToDraw = new();
        Dictionary<GameObject, ButtonState> buttonStates = new();
        Game pinball = new PinballMVP();

        public MainMenu() : base() { }

        public void handleInput(InputEvent inp, string eventType)
        {
            // TODO: Every button state should also hold something similar to an "Action" (func ptr) that is called
            foreach (var button in buttonStates.Keys)
            {
                Transform t = button.Transform;
                bool isMouseInsideButton = GeometryUtils.Contains(inp.X, inp.Y, (int)t.X, (int)t.Y, t.Wid, t.Ht);
                if (eventType.Equals("MouseDown"))
                {
                    if (isMouseInsideButton)
                    {
                        if (buttonStates[button].Tag == "Play")
                        {
                            pinball.physicsManager.GravityModifier = 0.15f;
                            GameStateManager.getInstance().SetGame(pinball);
                            pinball.initialize();
                        }
                        else if (buttonStates[button].Tag == "Exit")
                        {
                            Environment.Exit(0);
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
            // Important that background is added first, otherwise it will be potentially render
            // on top of other gameobjects.
            // probably should implement a fix such that we can sort our gameobjects depending on render "priority" or visibility.
            Display disp = Bootstrap.getDisplay();

            GameObject background = new();
            background.Transform.SpritePath = getAssetManager().getAssetPath("pinball_blurred.png");
            background.Transform.X = 0f;
            background.Transform.Y = 0f;
            gameObjsToDraw.Add(background);

            var playButtonState = new ButtonState("Play", "play.png", "play_hovered.png", null);
            CreateButton(disp.getWidth() / 2 - 100, disp.getHeight() / 2 - 100, playButtonState); 

            var exitButtonState = new ButtonState("Exit", "exit.png", "exit_hovered.png", null);
            CreateButton(disp.getWidth() - 100, 10, exitButtonState);

            var highScoreButtonState = new ButtonState("Highscore", "highscore.png", "highscore_hovered.png", null);
            CreateButton(disp.getWidth() / 2 + 100, disp.getHeight() / 2 - 140, highScoreButtonState);

            Bootstrap.getInput().addListener(this);
        }

        public void CreateButton(float x, float y, ButtonState buttonState)
        {
            GameObject button = new();
            button.Transform.X = x;
            button.Transform.Y = y;
            gameObjsToDraw.Add(button);

            buttonStates.Add(button, buttonState);
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
