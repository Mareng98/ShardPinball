using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace Shard
{
    class ColliderPolygon : Collider
    {
        private Transform trans;
        private float x;
        private float y;
        private float width;
        private float height;
        private float rotation;
        private Vector2 rotationPivot;
        private Vector2 collisionNormal;
        private Vector2[] vertices; // Starting from top-left, going clockwise

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public Vector2 CollisionNormal
        {
            get
            {
                Vector2 tmp = collisionNormal;
                // Reset normal vector (It can be used only once)
                collisionNormal = new Vector2(0, 0);
                return tmp;
            }
        }

        public Vector2 RotationPivot
        {
            get { return rotationPivot; }
            set { rotationPivot = value;  }
        }

        public float Rotation
        {
            get { return rotation; }
            set {
                RotateVertices(value,  rotationPivot);
            }
        }

        // A simple rectangle with rotation-pivot in the center
        public ColliderPolygon(CollisionHandler gob, Transform trans, float x, float y, float w, float h, float r) : base(gob)
        {
            X = x;
            Y = y;
            width = w;
            height = h;
            vertices = CalculateVertices();
            collisionNormal = new Vector2(0, 0);
            RotationPivot = new Vector2(w / 2, h / 2);
            Rotation = r;
            this.trans = trans;
        }

        // Any polyogon with a rotation-pivot at the first vertex
        public ColliderPolygon(CollisionHandler gob, Transform trans, float x, float y, Vector2[] inputVertices, float r) : base(gob)
        {
            X = x;
            Y = y;
            rotation = r;
            vertices = inputVertices;
            width = CalculateWidth();
            height = CalculateHeight();
            RotationPivot = inputVertices[0];
            Rotation = 0;
            this.trans = trans;
        }

        // Any polygon with a user-defined rotation-pivot
        public ColliderPolygon(CollisionHandler gob, Transform trans, float x, float y, Vector2[] inputVertices, float r, Vector2 rotationPivot) : base(gob)
        {
            X = x;
            Y = y;
            rotation = r;
            vertices = inputVertices;
            width = CalculateWidth();
            height = CalculateHeight();
            RotationPivot = rotationPivot;
            Rotation = 0;
            this.trans = trans;
        }

        private Vector2[] CalculateVertices()
        {

            Vector2[] calculatedVertices = new Vector2[]
            {
            new Vector2(0, 0),  // Top-left
            new Vector2(width, 0),   // Top-right
            new Vector2(width, height),  // Bottom-right
            new Vector2(0, height)  // Bottom-left
            };

            return calculatedVertices;
        }

        // 
        public void RotateVertices(float targetRotation, Vector2 rotationPivot)
        {
            float newRotation = (targetRotation * MathF.PI / 180.0f) - rotation;
            float cosAngle = (float)Math.Cos(newRotation);
            float sinAngle = (float)Math.Sin(newRotation);
            for (int i = 0; i < vertices.Length; i++)
            {
                // Translate the vertex to the origin
                float x = vertices[i].X - rotationPivot.X;
                float y = vertices[i].Y - rotationPivot.Y;

                // Rotate the translated vertex
                float rotatedX = x * cosAngle - y * sinAngle;
                float rotatedY = x * sinAngle + y * cosAngle;

                // Translate the rotated vertex back to its original position
                vertices[i] = new Vector2(rotatedX + rotationPivot.X, rotatedY + rotationPivot.Y);
            }
            rotation = (rotation + newRotation) % (2 * MathF.PI);
        }

        // Helper function to rotate a triangle
        private void RotateTriangle(float angle, Vector2[] vertices, Vector2 rotationPivot)
        {
            float cosAngle = (float)Math.Cos(angle);
            float sinAngle = (float)Math.Sin(angle);

            for (int i = 0; i < vertices.Length; i++)
            {
                // Translate the vertex to the origin
                float x = vertices[i].X - rotationPivot.X;
                float y = vertices[i].Y - rotationPivot.Y;

                // Rotate the translated vertex
                float rotatedX = x * cosAngle - y * sinAngle;
                float rotatedY = x * sinAngle + y * cosAngle;

                // Translate the rotated vertex back to its original position
                vertices[i] = new Vector2(rotatedX , rotatedY + rotationPivot.Y);
            }
        }

        // Aligns the first two vertices with the x-axis
        private void StraigthenTriangle(Vector2[] vertices)
        {
            float angle = (float)Math.Atan2(vertices[1].Y - vertices[0].Y, vertices[1].X - vertices[0].X);
            RotateTriangle(-angle, vertices, vertices[0]);
        }

        private float CalculateWidth()
        {
            float[] xValues = MinAndMaxX;
            return xValues[1] - xValues[0];
        }

        private float CalculateHeight()
        {
            float[] yValues = MinAndMaxY;
            return yValues[1] - yValues[0];
        }

        public override void recalculate()
        {
            MinAndMaxX = getMinAndMaxX();
            MinAndMaxY = getMinAndMaxY();
            RotateVertices(trans.Rotz, rotationPivot);
        }

        public override Vector2? checkCollision(ColliderPolygon c)
        {
            return null;
        }

        public override Vector2? checkCollision(ColliderRect c)
        {
            return null;
        }

        public override Vector2? checkCollision(Vector2 c)
        {
            return null;
        }

        public override Vector2? CheckRaycastCollision(ColliderCircle c)
        {
            return null;
        }

        // For debugging purposes
        private void DrawTriangle(Vector2[] triangle, Color col )
        {
            Display d = Bootstrap.getDisplay();
            for (int i = 0; i < 3; i++)
            {
                d.drawLine((int)(triangle[i].X), (int)(triangle[i].Y), (int)(triangle[(i + 1) % 3].X), (int)(triangle[(i + 1) % 3].Y), col);
            }
        }

        // aka crossing number algorithm/ray casting algorithm
        private bool pointInPolygon(Vector2 point)
        {
            var intersections = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                // first we unpack the vertices points to create a line segment
                var x1 = vertices[i].X + x;
                var y1 = vertices[i].Y + y;

                var x2 = vertices[(i + 1) % vertices.Length].X + x;
                var y2 = vertices[(i + 1) % vertices.Length].Y + y;

                if (((y1 > point.Y && y2 < point.Y) || (y1 < point.Y && y2 > point.Y)) &&
                    (point.X < (x2-x1) * (point.Y-y1) / (y2-y1)+x1))
                {
                    intersections += 1;
                }
            }
            return intersections % 2 == 1;
        }

        // Calculate the normal of a line segment
        private Vector2 CalculateNormalVector(Vector2 a, Vector2 b)
        {
            float deltaX = b.X - a.X;
            float deltaY = b.Y - a.Y;

            // Calculate the normal vector by swapping and negating components
            Vector2 normalVector = new Vector2(deltaY, -deltaX);

            return normalVector;
        }

        // Check if a circle is in a bounding box
        private bool CircleInBoundingBox(Vector2 origin, float radius)
        {
            float[] minMaxX = getMinAndMaxX();
            float maxX = minMaxX[1];
            float minX = minMaxX[0];

            float[] minMaxY = getMinAndMaxY();
            float maxY = minMaxY[1];
            float minY = minMaxY[0];

            // Check if the circle is within the bounding box
            bool isInsideX = (origin.X + radius) > minX && (origin.X - radius) < maxX;
            bool isInsideY = (origin.Y + radius) > minY && (origin.Y - radius) < maxY;

            return isInsideX && isInsideY;
        }

        // Just remembered that this function has to be in colliderCircle to check against this, but we'll fix it later
        public override Vector2? checkCollision(ColliderCircle c)
        {
            Vector2 ballOrigin = new Vector2(c.X, c.Y);
            Vector2 lastBallOrigin = new Vector2(c.Lx, c.Ly);
            bool isPointInPolygon = pointInPolygon(ballOrigin);
            bool isPreviousPointInPolygon = pointInPolygon(lastBallOrigin);

            if (!CircleInBoundingBox(ballOrigin, c.Rad))
            {
                return null;
            }

            int shortestPathIndex = 0;
            float shortestPath = float.MaxValue;
            bool borderCrossed = false;
            // Check if any border has been passed
            if (!isPreviousPointInPolygon) { 
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector2 result;
                    if (FindIntersection(lastBallOrigin, ballOrigin,
                        new Vector2(vertices[i].X + x, vertices[i].Y + y),
                        new Vector2(vertices[(i + 1) % vertices.Length].X + x, vertices[(i + 1) % vertices.Length].Y + y), out result))
                    {
                        // Intersection found
                        if ((lastBallOrigin - result).Length() < shortestPath)
                        {
                            borderCrossed = true;
                            shortestPath = (lastBallOrigin - result).Length();
                            shortestPathIndex = i;
                        }
                    }
                }
                if (borderCrossed)
                {
                    return CalculateNormalVector(vertices[shortestPathIndex], vertices[(shortestPathIndex + 1) % vertices.Length]);
                }
            }

            float[] sideLengths = new float[vertices.Length];
            // If the ballOrigin is completely within the rectangle this wont always work
            // If the ball is at least partly outside, this will work
            // Add the minimum distance from ball to each side
            for (int i = 0; i < vertices.Length; i++)
            {
                // Fix this so that we dont have to create new vectors geeze
                Vector2[] triangle = { new Vector2(vertices[i].X + x, vertices[i].Y+y), 
                    new Vector2(vertices[(i + 1) % vertices.Length].X + x, vertices[(i + 1) % vertices.Length].Y + y), 
                    new Vector2(c.X, c.Y) };
                // Make polygon line-segment parallel with x-axis, and the ball origin above or below it.
                StraigthenTriangle(triangle); 
                
                Vector2 p1 = triangle[0];
                Vector2 p2 = triangle[1];
                ballOrigin = triangle[2];

                if (p1.X > p2.X)
                {
                    // Swap so that p1 is to the left of p2, ugly solution that could be fixed
                    Vector2 tmp = p1;
                    p1 = p2;
                    p2 = tmp;
                }
                // Check if the ball is inside a column extending parallel to the normal of that side
                if(ballOrigin.X < p1.X ||ballOrigin.X > p2.X)
                {
                    // The ball is not within the column, so it can not be colliding with this side
                    sideLengths[i] = float.MaxValue;
                }
                else
                {
                    // The ball is within the column, simply add the height of the triangle
                    sideLengths[i] = Math.Abs((ballOrigin.Y - p1.Y)); 
                }
            }
            // Find the smallest distance
            int smallestDistanceIndex = 0;
            float smallestDistance = sideLengths[0];
            for (int i = 1; i < vertices.Length; i++)
            {
                if ( sideLengths[i] < smallestDistance)
                {
                    smallestDistance = sideLengths[i];
                    smallestDistanceIndex = i;
                }
            }

            // Check if the smallest distance is smaller than the radius of the ball
            if (smallestDistance <= c.Rad || isPointInPolygon)
            {
                Vector2 normal = CalculateNormalVector(vertices[smallestDistanceIndex], vertices[(smallestDistanceIndex + 1) % vertices.Length]);
                return normal;
            }
            return null;
        }

        public float distanceToNearestLine(Vector2 pointOnLine, Vector2 line, Vector2 point)
        {
            // Vector from pointOnLine to point
            Vector2 pointOnLineToPoint = point - pointOnLine;
            // Vector from pointOnLine to Line, let's call this AB
            Vector2 pointOnLineToLine = line - pointOnLine;
            float magnitudeAB = pointOnLineToLine.LengthSquared();
            float prod = Vector2.Dot(pointOnLineToPoint, pointOnLineToLine);
            float dist = prod / magnitudeAB;
            return dist;
        }

        // Draw an arbitrary shape consisting of vertices
        private void DrawShape(Vector2[] a)
        {
            Display d = Bootstrap.getDisplay();
            
            for (int i = 0; i < a.Length; i++)
            {
                d.drawLine((int) a[i].X, (int)a[i].Y, (int)a[(i + 1) % a.Length].X, (int)a[(i + 1) % a.Length].Y,Color.Purple);
            }
            d.display();
        }

        // Check if two finite lines intersect
        private bool FindIntersection(Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2, out Vector2 result)
        {
            // Let Ball's last movement make up the bounding box
            float left = Math.Min(s1.X,e1.X);
            float right = Math.Max(s1.X, e1.X);
            float top = Math.Min(s1.Y, e1.Y);
            float bottom = Math.Max(s1.Y, e1.Y);



            // Ensure that the intersection is between the vertices
            float lineLeft = Math.Min(s2.X, e2.X);
            float lineRight = Math.Max(s2.X, e2.X);
            float lineTop = Math.Min(s2.Y, e2.Y);
            float lineBottom = Math.Max(s2.Y, e2.Y);

            double dy1 = e1.Y - s1.Y;
            double dx1 = s1.X - e1.X;
            double c1 = dy1 * s1.X + dx1 * s1.Y;

            double dy2 = e2.Y - s2.Y;
            double dx2 = s2.X - e2.X;
            double c2 = dy2 * s2.X + dx2 * s2.Y;

            double delta = dy1 * dx2 - dy2 * dx1;
            //If lines are parallel, there can't be an intersection.
            if(delta == 0)
            {
                result = new Vector2();
                return false;
            }
            else
            {
                // The point of intersection
                result = new Vector2((float)((dx2 * c1 - dx1 * c2) / delta), (float)((dy1 * c2 - dy2 * c1) / delta));
                // Check if the intersection is happening between vertices
                if(result.X < lineLeft || result.X > lineRight || result.Y > lineBottom || result.Y < lineTop)
                {
                    return false;
                }
                // Check if the intersection is happening within the bounding box
                if(result.X < left || result.X > right || result.Y > bottom ||result.Y < top)
                {
                    return false;
                }
                // For debugging purposes ( Shows the bounding box where an intersection can be found)
                /*Vector2 topLeft = new Vector2(Math.Min(s1.X, e1.X), Math.Min(s1.Y, e1.Y));
                Vector2 topRight = new Vector2(Math.Max(s1.X, e1.X), Math.Min(s1.Y, e1.Y));
                Vector2 bottomLeft = new Vector2(Math.Min(s1.X, e1.X), Math.Max(s1.Y, e1.Y));
                Vector2 bottomRight = new Vector2(Math.Max(s1.X, e1.X), Math.Max(s1.Y, e1.Y));
                DrawShape([topLeft, topRight, bottomRight, bottomLeft]);*/
                return true;
            }
        }


        public override void drawMe(Color col)
        {
            Display d = Bootstrap.getDisplay();
            
            // Fill polygon with a color
            Vector2[] renderedVertices = new Vector2[vertices.Length];
            for(int i = 0; i < vertices.Length; i++)
            {
                Vector2 v = vertices[i];
                renderedVertices[i] = v + new Vector2(x, y);
            }
            d.renderGeometry(renderedVertices, Color.Coral,255);
            // Debug
            //DrawDebug(d, col);
        }

        // Debug function to see how normals are calculated and bounding-box
        private void DrawDebug(Display d, Color col)
        {
            Vector2 start;
            for (int i = 0; i < vertices.Length; i++)
            {
                d.drawLine((int)(vertices[i].X + x), (int)(vertices[i].Y + y), (int)(vertices[(i + 1) % vertices.Length].X + x), (int)(vertices[(i + 1) % vertices.Length].Y + y), col);
                Vector2 normal = Vector2.Normalize(CalculateNormalVector(vertices[i], vertices[(i + 1) % vertices.Length]));
                Vector2 translation = new Vector2(x, y);
                Vector2 midPoint = (vertices[i] + vertices[(i + 1) % vertices.Length] + translation * 2) / 2;
                start = midPoint + normal * 50;
                Vector2 end = midPoint;
                d.drawLine((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, Color.GreenYellow);
            }
        }

        public override float[] getMinAndMaxY()
        {
            float miny = vertices[0].Y;
            float maxy = miny;
            foreach(Vector2 v in vertices)
            {
                if(v.Y > maxy)
                {
                    maxy = v.Y;
                }
                if(v.Y < miny)
                {
                    miny = v.Y;
                }
            }
            return [ miny + y - 10, maxy + y + 10 ];
        }

        public override float[] getMinAndMaxX()
        {
            float minx = vertices[0].X;
            float maxx = minx;
            foreach (Vector2 v in vertices)
            {
                if (v.X > maxx)
                {
                    maxx = v.X;
                }
                if (v.X < minx)
                {
                    minx = v.X;
                }
            }
            return [minx + x - 10 , maxx + x + 10];
        }
    }
}
