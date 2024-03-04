using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.Shard
{
    class GeometryUtils
    {
        public static bool Contains(int x, int y, int squareX, int squareY, int squareWidth, int squareHeight)
        {
            return x >= squareX && x < squareX + squareWidth && y >= squareY && y < squareY + squareHeight;
        }
    }
}
