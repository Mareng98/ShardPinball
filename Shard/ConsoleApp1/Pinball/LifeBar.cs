using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shard;
using System.Numerics;
namespace Pinball
{
    internal class LifeBar
    {
        List<PinballBallIcon> lives;
        public LifeBar(int noOfLives, int x, int y)
        {
            lives = new List<PinballBallIcon>();
            for(int i = 0; i < noOfLives; i++)
            {
                PinballBallIcon b = new PinballBallIcon(x + i*20, y);
                lives.Add(b);
            }
        }

        public bool ReduceLives()
        {
            if(lives.Count >= 1)
            {
                lives[lives.Count - 1].ToBeDestroyed = true;
                lives.RemoveAt(lives.Count - 1);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
