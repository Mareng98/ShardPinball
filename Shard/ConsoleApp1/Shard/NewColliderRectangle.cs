using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;

namespace Shard
{
    class NewColliderRectangle : Collider
    {
        private float x;
        private float y;
        private float width;
        private float height;
        private float rotation;
        private Vector2[] vertices; // Starting from top-left, going clockwise
        private Vector2[] triangle;
        private Vector2[] currentTriangle;

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

        public NewColliderRectangle(CollisionHandler gob, float x, float y) :base(gob)
        {
            X = x;
            Y = y;
            width = 2;
            height = 2;
            rotation = 0;
            vertices = CalculateVertices();
        }
        public NewColliderRectangle(CollisionHandler gob, float x, float y, float w, float h) : base(gob)
        {
            X = x;
            Y = y;
            width = w;
            height = h;
            rotation = 0;
            vertices = CalculateVertices();
        }
        public static float CalculateLineLength(Vector2 startPoint, Vector2 endPoint)
        {
            float deltaX = endPoint.X - startPoint.X;
            float deltaY = endPoint.Y - startPoint.Y;

            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
        public NewColliderRectangle(CollisionHandler gob, float x, float y, float w, float h, float r) : base(gob)
        {
            X = x;
            Y = y;
            width = w;
            height = h;
            rotation = r;
            vertices = CalculateVertices();

            triangle = new Vector2[] { new Vector2(200, 200), new Vector2(300, 230), new Vector2(250, 150) };
            currentTriangle = new Vector2[] { new Vector2(200, 200), new Vector2(300, 230), new Vector2(250, 150) };
        }
        // Use this if you want an uneven rectangle
        public NewColliderRectangle(CollisionHandler gob, float x, float y, Vector2[] inputVertices, float r) : base(gob)
        {
            if (inputVertices.Length != 4)
                throw new ArgumentException("Invalid number of vertices. Must be 4 for a rectangle.");
            X = x;
            Y = y;
            rotation = r;
            vertices = inputVertices;
            // Calculate width, height, and rotation based on the given vertices
            // You might want to implement the logic for reverse engineering these properties.
            // This is a placeholder and might not work in every case.
            width = CalculateWidth();
            height = CalculateHeight();
            rotation = CalculateRotation();

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

            // Rotate the vertices based on the given rotation angle
            RotateVertices(calculatedVertices, rotation);

            return calculatedVertices;
        }

        public static float CalculateRotationAngle(Vector2[] vertices)
        {
            // Calculate the angle of the first vector with respect to the x-axis
            float angle = (float)Math.Atan2(vertices[1].Y - vertices[0].Y, vertices[1].X - vertices[0].X);
            return -angle; // Negative because the rotation is performed in the opposite direction
        }

        public void RotateShape(float angle, Vector2[] vertices, Vector2 pivot)
        {
            float cosAngle = (float)Math.Cos(angle);
            float sinAngle = (float)Math.Sin(angle);

            for (int i = 0; i < vertices.Length; i++)
            {
                // Translate the vertex to the origin
                float x = vertices[i].X - pivot.X;
                float y = vertices[i].Y - pivot.Y;

                // Rotate the translated vertex
                float rotatedX = x * cosAngle - y * sinAngle;
                float rotatedY = x * sinAngle + y * cosAngle;

                // Translate the rotated vertex back to its original position
                vertices[i] = new Vector2(rotatedX + pivot.X, rotatedY + pivot.Y);
            }
        }

        // Aligns the first two vertices with the x-axis
        private void StraigthenTriangle(Vector2[] vertices)
        {
            float angle = (float)Math.Atan2(Math.Abs(vertices[1].Y - vertices[0].Y), Math.Abs(vertices[1].X - vertices[0].X));
            RotateShape(-angle, vertices, vertices[0]);
        }

        private void RotateVertices(Vector2[] verticesToRotate, float angle)
        {
            float cosAngle = (float)Math.Cos(angle);
            float sinAngle = (float)Math.Sin(angle);

            for (int i = 0; i < verticesToRotate.Length; i++)
            {
                float x = verticesToRotate[i].X;
                float y = verticesToRotate[i].Y;

                verticesToRotate[i] = new Vector2(x * cosAngle - y * sinAngle, x * sinAngle + y * cosAngle);
            }
        }

        private float CalculateWidth()
        {
            // Implement the logic to calculate width based on vertices.
            // This is a placeholder and might not work in every case.
            return 0; // Placeholder value, replace with actual calculation.
        }

        private float CalculateHeight()
        {
            // Implement the logic to calculate height based on vertices.
            // This is a placeholder and might not work in every case.
            return 0; // Placeholder value, replace with actual calculation.
        }

        private float CalculateRotation()
        {
            // Implement the logic to calculate rotation based on vertices.
            // This is a placeholder and might not work in every case.
            return 0; // Placeholder value, replace with actual calculation.
        }

        public override void recalculate()
        {
            MinAndMaxX = getMinAndMaxX();
            MinAndMaxY = getMinAndMaxY();
        }

        public override Vector2? checkCollision(NewColliderRectangle c)
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

        // Used to compare lengths without taking an expensive square root every time
        private float GetSquaredSideLength(Vector2 p1, Vector2 p2)
        {
            float deltaX = p2.X - p1.X;
            float deltaY = p2.Y - p1.Y;
            return deltaX * deltaX + deltaY * deltaY;
        }

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
            bool collision = false;

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
                    collision = !collision;
                }
            }
            return collision;
        }

        // Just remembered that this function has to be in colliderCircle to check against this, but we'll fix it later
        public override Vector2? checkCollision(ColliderCircle c)
        {

            bool isPointInPolygon = pointInPolygon(new Vector2(c.X, c.Y));
            if (isPointInPolygon)
            {
                DrawTriangle(new Vector2[] { new Vector2(200, 200), new Vector2(100, 100), new Vector2(50, 50) }, Color.Green);
                //Debug.Log("Point is in Polygon");
            }

            Vector2 ballOrigin = new Vector2(c.X, c.Y);
            float[] sideLengths = new float[4];
            // If the ballOrigin is completely within the rectangle this wont always work
            // If the ball is at least partly outside, this will work
            // Add the minimum distance from ball to each side
            for (int i = 0; i < 4; i++)
            {
                // Fix this so that we dont have to create new vectors geeze
                Vector2[] triangle = new Vector2[] { new Vector2(vertices[i].X + x, vertices[i].Y+y), new Vector2(vertices[(i + 1) % 4].X + x, vertices[(i + 1) % 4].Y + y), new Vector2(c.X, c.Y) };
                StraigthenTriangle(triangle); // Make p1 and p2 parallel with x-axis
                // Debugging purpose, color one triangle from side to ball
                if(i == 2)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        currentTriangle[j] = new Vector2(triangle[j].X, triangle[j].Y);
                    }
                }
                DrawTriangle(triangle, Color.AliceBlue);
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
                // Check if the ball is inside a column extending from the normal of that side
                if(ballOrigin.X < p1.X ||ballOrigin.X > p2.X)
                {
                    // The ball is not within the column; make the distance the minimum distance of sides going out from ball
                    sideLengths[i] = Math.Min(GetSquaredSideLength(ballOrigin, p1), GetSquaredSideLength(ballOrigin, p2));
                }
                else
                {
                    // The ball is within the column, simply add the height of the triangle
                    sideLengths[i] = (float)Math.Pow(Math.Abs(ballOrigin.Y - p1.Y),2); 
                }
            }
            // Find the smallest distance
            int smallestDistanceIndex = 0;
            float smallestDistance = sideLengths[0];
            for (int i = 1; i < 4; i++)
            {
                if (sideLengths[i] < smallestDistance)
                {
                    smallestDistance = sideLengths[i];
                    smallestDistanceIndex = i;
                }
            }
            // Check if the smallest distance is smaller than the radius of the ball
            float cubedRad = (float)Math.Pow(c.Rad, 2);
            if (smallestDistance <= cubedRad)
            {
                Debug.Log(smallestDistance.ToString());
                switch (smallestDistanceIndex)
                {
                    case 0:
                        Debug.Log("Top");
                        break;
                    case 1:
                        Debug.Log("Right");
                        break;
                    case 2:
                        Debug.Log("Bottom");
                        break;
                    case 3:
                        Debug.Log("Left");
                        break;
                }
            }
            if (triangle != null)
            {

                triangle = new Vector2[] { new Vector2(vertices[1].X + x, vertices[1].Y + y), new Vector2(vertices[2].X + x, vertices[2].Y + y), new Vector2(c.X,c.Y) };
            }
            return null;
        }

        public override void drawMe(Color col)
        {
            Display d = Bootstrap.getDisplay();

            float centerX = (vertices[0].X + vertices[2].X) / 2;
            float centerY = (vertices[0].Y + vertices[2].Y) / 2;

            for (int i = 0; i < 4; i++)
            {
                d.drawLine((int)(vertices[i].X + x), (int)(vertices[i].Y + y), (int)(vertices[(i+1)%4].X + x), (int)(vertices[(i+1)%4].Y + y), col);
            }
            if(triangle != null && currentTriangle != null)
            {
                //StraigthenTriangle(triangle);
                //DrawTriangle(triangle,col);
                //DrawTriangle(currentTriangle, col);
            }
            

            d.drawCircle((int)centerX, (int)centerY, 2, col);
        }

        public override float[] getMinAndMaxX()
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
            return new float[]{ miny + y,maxy + y};
        }

        public override float[] getMinAndMaxY()
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
            return new float[] { minx + x , maxx + x};
        }
    }
}
