using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shard
{
    /// <summary>
    /// Finds which triangles are to be drawn to fill any polygon, 
    /// given some assumptions about the polygon for it to function correctly.
    /// 1# The polygon can not have a vertex located on a straight line between two other vertices.
    /// 2# No lines can intersect.
    /// </summary>
    internal static class Triangulator
    {
        public static List<Vector2[]> Triangulate(Vector2[] vertices)
        {
            List<Vector2[]> triangles = new List<Vector2[]>();

            List<Vector2> verticesList = new List<Vector2>(vertices);

            while (verticesList.Count >= 3)
            {
                int i = FindEarTip(verticesList);
                int iPrev = (i - 1 + verticesList.Count) % verticesList.Count;
                int iNext = (i + 1) % verticesList.Count;

                triangles.Add([verticesList[iPrev], verticesList[i], verticesList[iNext]]);

                verticesList.RemoveAt(i);
            }

            return triangles;
        }

        private static int FindEarTip(List<Vector2> vertices)
        {
            int count = vertices.Count;

            for (int i = 0; i < count; i++)
            {
                int iPrev = (i - 1 + count) % count;
                int iNext = (i + 1) % count;

                Vector2 prev = vertices[iPrev];
                Vector2 current = vertices[i];
                Vector2 next = vertices[iNext];

                if (IsEar(prev, current, next, vertices))
                    return i;
            }

            throw new InvalidOperationException("No ear found. The shape might be degenerate.");
        }

        private static bool IsEar(Vector2 a, Vector2 b, Vector2 c, List<Vector2> vertices)
        {
            foreach (var vertex in vertices)
            {
                if (vertex != a && vertex != b && vertex != c && IsPointInTriangle(vertex, a, b, c))
                    return false;
            }

            return true;
        }

        private static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float d1 = Sign(p, a, b);
            float d2 = Sign(p, b, c);
            float d3 = Sign(p, c, a);

            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(hasNeg && hasPos);
        }

        private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }
    }
}
