﻿using Shard.Pinball;
using Shard.Shard;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
namespace Shard
{
    class Highscore : Game, InputListener
    {
        List<GameObject> gameObjsToDraw = new();
        Dictionary<GameObject, ButtonState> buttonStates = new();
        List<Tuple<string, int>> highscores; 

        public Highscore() : base() {}

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

            var exitButtonState = new ButtonState("Exit", "exit.png", "exit_hovered.png", null);
            exitButtonState.CreateButton(disp.getWidth() - 100, 10, ref gameObjsToDraw, ref buttonStates);

            var backButtonState = new ButtonState("Back", "back.png", "back_hovered.png", null);
            backButtonState.CreateButton(100, 10, ref gameObjsToDraw, ref buttonStates);

            highscores = PinballUtils.HighScores;

            Bootstrap.getInput().addListener(this);
        }

        public override void update()
        {
            var disp = Bootstrap.getDisplay();
            foreach(var gameObj in gameObjsToDraw)
            {
                disp.addToDraw(gameObj);
            }

            disp.showText("Highscores:", disp.getWidth() / 2 - 200, disp.getHeight() / 2 - 400, 90, Color.White);

            for (int i = 0; i <  highscores.Count; i++)
            {
                var highScore = highscores[i];
                var name = highScore.Item1;
                var score = highScore.Item2;
                // lerp :) 
                var height = -200 + (500 * i / highscores.Count);

                disp.showText(i + 1 + ". " +  name + " | " + score + "", disp.getWidth() / 2 - 300, 
                    disp.getHeight() / 2 + height, 70, Color.White); 
            }
        }

        public override int getTargetFrameRate()
        {
            return 30;
        }

    }
}
