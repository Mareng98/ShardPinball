using Pinball;
using Shard.Pinball;
using Shard.Shard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Shard
{
    class GameOver : Game, InputListener
    {
        List<GameObject> gameObjsToDraw = new();
        Dictionary<GameObject, ButtonState> buttonStates = new();
        int score;
        StringBuilder name = new();

        public GameOver(int score) : base() 
        {
            this.score = score;
        }

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
                            SaveScore("Anonymous", score);
                            SetMainMenu();
                            // cleanup after ourselves
                            // probably should implement IDisposable for this.
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

            if (eventType.Equals("KeyDown"))
            {
                var key = inp.Key;

                // Backspace
                if (key == 42)
                {
                    if (name.Length > 0)
                    {
                        name.Remove(name.Length - 1, 1);
                    }
                // Enter
                } else if (key == 40)
                {
                    SaveScore(name.ToString(), score);
                    SetMainMenu();
                }
                // Any key A-Z
                else if (key >= 4 && (int) key <= 29 && name.Length <= 10)
                {
                    // idk why ConsoleKey stuff is so buggy, but ugly fix:
                    var character = (char)('a' + key - 4);
                    name.Append(character);
                }
            }
        }

        private void SaveScore(string name, int score)
        {
            var highscores = PinballUtils.LoadHighscores();
            Tuple<string, int> newEntry = new Tuple<string, int>(name, score);
            var updatedHighscores = PinballUtils.UpdateHighscores(highscores, newEntry);
            PinballUtils.saveHighscores(updatedHighscores);
        }

        private void SetMainMenu()
        {
            Game game = new MainMenu();
            GameStateManager.getInstance().SetGame(game);
            game.initialize();
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
            backButtonState.CreateButton(100, 10, ref gameObjsToDraw, ref buttonStates);

            Bootstrap.getInput().addListener(this);
        }

        public override void update()
        {
            foreach(var gameObj in gameObjsToDraw)
            {
                Bootstrap.getDisplay().addToDraw(gameObj);
            }

            var disp = Bootstrap.getDisplay();
            disp.showText("Game", disp.getWidth() / 2 - 100, 100, 70, Color.White);
            disp.showText("Over!", disp.getWidth() / 2 - 90, 200, 70, Color.White);

            disp.drawLine(disp.getWidth() / 2 - 100, 500, disp.getWidth() / 2 + 100, 500, Color.White);
            // Draw "indicator"
            disp.showText(name.ToString(), disp.getWidth() / 2 - 100 - 10, 450, 50, Color.White);
            disp.showText("I", disp.getWidth() / 2 - 100 + 21 * name.Length, 450, 50, Color.White);

        }
        public override int getTargetFrameRate()
        {
            return 30;
        }

        enum Keys
        {
            a = 4, 
            b = 5, 
            c = 6, 
            d = 7, e = 8, f = 9,   g = 10, h = 11, i = 12, j = 13, k = 14, l = 15, m = 16, n = 17,
        }
    }
}
