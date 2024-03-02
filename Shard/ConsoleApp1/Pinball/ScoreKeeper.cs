using Shard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Pinball
{
    internal class ScoreKeeper: GameObject
    {
        private int noOfFramesToDisplayAddedScore;
        private int score;
        private Vector2 scorePos;
        private List<Tuple<Vector2, int,int>> addedScores; // Position, score, number of frames displayed so far

        public ScoreKeeper(Vector2 scorePos, int noOfFramesToDisplayAddedScore, int initialScore)
        {
            this.scorePos = scorePos;
            this.noOfFramesToDisplayAddedScore = noOfFramesToDisplayAddedScore;
            score = initialScore;
            addedScores = new List<Tuple<Vector2, int, int>>();
        }
        
        public int Score { get => score;}

        public override void update()
        {
            Bootstrap.getDisplay().showText("Score: " + score.ToString(), scorePos.X, scorePos.Y, 30, Color.GreenYellow);
            // Display added scores as floating text above the ball
            for(int i = 0; i < addedScores.Count; i++)
            {
                Tuple<Vector2, int, int> displayedScore = addedScores[i];
                if(displayedScore.Item3 < noOfFramesToDisplayAddedScore)
                {
                    Bootstrap.getDisplay().showText(displayedScore.Item2.ToString() + '+', displayedScore.Item1.X, displayedScore.Item1.Y, 13, Color.LightBlue);
                    addedScores[i] = new Tuple<Vector2, int, int>(displayedScore.Item1 + new Vector2(0, -1), displayedScore.Item2, displayedScore.Item3 + 1);
                }
                else
                {
                    i--;
                    addedScores.Remove(displayedScore);
                }
            }
        }
        public void AddScore(Vector2 position, int addedScore)
        {
            addedScores.Add(new Tuple<Vector2,int, int>(position,addedScore,0));
            score += addedScore;
        }
    }
}
