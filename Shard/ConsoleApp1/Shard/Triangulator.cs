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

                // Is potential ear convex? ( Measured clockwise around polygon )
                if (CrossProduct2D(prev - current, next - current) < 0f)
                {
                    continue;
                }


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

        private static float CrossProduct2D(Vector2 v1, Vector2 v2)
        {
            return (v1.X * -v2.Y) - (-v1.Y * v2.X);
        }
        public static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            Vector2 ab = b - a;
            Vector2 bc = c - b;
            Vector2 ca = a - c;

            Vector2 ap = p - a;
            Vector2 bp = p - b;
            Vector2 cp = p - c;

            float cross1 = CrossProduct2D(ab, ap);
            float cross2 = CrossProduct2D(bc, bp);
            float cross3 = CrossProduct2D(ca, cp);

            if (cross1 >= 0f || cross2 >= 0f || cross3 >= 0f)
            {
                return false;
            }

            return true;
        }

        private static bool IsPointOnLine(Vector2 p, Vector2 start, Vector2 end)
        {
            if (start.X == end.X && p.X == start.X && p.Y >= Math.Min(start.Y, end.Y) && p.Y <= Math.Max(start.Y, end.Y)) return true;
            if (start.Y == end.Y && p.Y == start.Y && p.Y >= Math.Min(start.X, end.X) && p.X <= Math.Max(start.X, end.X)) return true;
            return false;
            float k = (start.Y - end.Y) / (start.X - end.X);
            // Check if the point is collinear with the line segment
            float area = 0.5f * ((end.Y - start.Y) * (p.X - start.X) - (end.X - start.X) * (p.Y - start.Y));
            return IsApproximately(area, 0f) &&
                   (p.X >= Math.Min(start.X, end.X) && p.X <= Math.Max(start.X, end.X)) &&
                   (p.Y >= Math.Min(start.Y, end.Y) && p.Y <= Math.Max(start.Y, end.Y));
        }

        private static bool IsApproximately(float a, float b, float epsilon = 0.0001f)
        {
            return Math.Abs(a - b) < epsilon;
        }

        private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }
    }
}
