using System;
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
    class NewColliderRectangle : Collider
    {
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
                RotateVertices(value, vertices, rotationPivot);
            }
        }

        public NewColliderRectangle(CollisionHandler gob, float x, float y) : base(gob)
        {
            X = x;
            Y = y;
            width = 2;
            height = 2;
            vertices = CalculateVertices();
            collisionNormal = new Vector2(0, 0);
            RotationPivot = new Vector2(width/2,height/2);

            Rotation = 0;
        }

        public NewColliderRectangle(CollisionHandler gob, float x, float y, Vector2 rotationPivot) :base(gob)
        {
            X = x;
            Y = y;
            width = 2;
            height = 2;
            vertices = CalculateVertices();
            collisionNormal = new Vector2(0, 0);
            RotationPivot = rotationPivot;

            Rotation = 0;
        }
        public NewColliderRectangle(CollisionHandler gob, float x, float y, float w, float h, Vector2 rotationPivot) : base(gob)
        {
            X = x;
            Y = y;
            width = w;
            height = h;
            vertices = CalculateVertices();
            collisionNormal = new Vector2(0, 0);
            RotationPivot = rotationPivot;
            Rotation = 0;
        }

        public NewColliderRectangle(CollisionHandler gob, float x, float y, float w, float h) : base(gob)
        {
            X = x;
            Y = y;
            width = w;
            height = h;
            vertices = CalculateVertices();
            collisionNormal = new Vector2(0, 0);
            RotationPivot = new Vector2(w/2, h/2);
            Rotation = 0;
        }

        public NewColliderRectangle(CollisionHandler gob, float x, float y, float w, float h, float r) : base(gob)
        {
            X = x;
            Y = y;
            width = w;
            height = h;
            vertices = CalculateVertices();
            collisionNormal = new Vector2(0, 0);
            RotationPivot = new Vector2(w / 2, h / 2);
            Rotation = r;
        }

        public NewColliderRectangle(CollisionHandler gob, float x, float y, float w, float h, float r, Vector2 rotationPivot) : base(gob)
        {
            X = x;
            Y = y;
            width = w;
            height = h;
            vertices = CalculateVertices();
            collisionNormal = new Vector2(0, 0);
            RotationPivot = rotationPivot;
            Rotation = 0;
        }
        // Use this if you want an uneven rectangle
        public NewColliderRectangle(CollisionHandler gob, float x, float y, Vector2[] inputVertices, float r) : base(gob)
        {
            X = x;
            Y = y;
            rotation = r;
            vertices = inputVertices;
            // Fix these later
            width = CalculateWidth();
            height = CalculateHeight();
            RotationPivot = inputVertices[0];
            Rotation = 0;
        }

        public NewColliderRectangle(CollisionHandler gob, float x, float y, Vector2[] inputVertices, float r, Vector2 rotationPivot) : base(gob)
        {
            //if (inputVertices.Length != 4)
            //    throw new ArgumentException("Invalid number of vertices. Must be 4 for a rectangle.");
            X = x;
            Y = y;
            rotation = r;
            vertices = inputVertices;
            // Calculate width, height, and rotation based on the given vertices
            // You might want to implement the logic for reverse engineering these properties.
            // This is a placeholder and might not work in every case.
            width = CalculateWidth();
            height = CalculateHeight();
            RotationPivot = rotationPivot;
            Rotation = 0;
        }
        public static float CalculateLineLength(Vector2 startPoint, Vector2 endPoint)
        {
            float deltaX = endPoint.X - startPoint.X;
            float deltaY = endPoint.Y - startPoint.Y;

            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
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

        public static float CalculateRotationAngle(Vector2[] vertices)
        {
            // Calculate the angle of the first vector with respect to the x-axis
            float angle = (float)Math.Atan2(vertices[1].Y - vertices[0].Y, vertices[1].X - vertices[0].X);
            return -angle; // Negative because the rotation is performed in the opposite direction
        }

        public void RotateVertices(float targetRotation, Vector2[] vertices, Vector2 rotationPivot)
        {
            float newRotation = targetRotation % 6.283f - rotation;
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
                rotation = targetRotation % 6.283f;
            }
        }

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
                vertices[i] = new Vector2(rotatedX + rotationPivot.X, rotatedY + rotationPivot.Y);
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

        public override Vector2? CheckRaycastCollision(ColliderCircle c)
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

        private Vector2 CalculateNormalVector(Vector2 a, Vector2 b)
        {
            float deltaX = b.X - a.X;
            float deltaY = b.Y - a.Y;

            // Calculate the normal vector by swapping and negating components
            Vector2 normalVector = new Vector2(-deltaY, deltaX);

            return normalVector;
        }

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
            if (!CircleInBoundingBox(ballOrigin, c.Rad))
            {
                return null;
            }
            bool isPointInPolygon = pointInPolygon(ballOrigin);
            if (isPointInPolygon)
            {
                DrawTriangle(new Vector2[] { new Vector2(Bootstrap.getDisplay().getWidth() / 2 + 200, Bootstrap.getDisplay().getHeight() / 2), new Vector2(Bootstrap.getDisplay().getWidth() / 2 + 300, Bootstrap.getDisplay().getHeight() / 2), new Vector2(Bootstrap.getDisplay().getWidth() / 2 + 250, Bootstrap.getDisplay().getHeight() - 200)}, Color.Green);
                //Debug.Log("Point is in Polygon");
            }

            
            float[] sideLengths = new float[vertices.Length];
            // If the ballOrigin is completely within the rectangle this wont always work
            // If the ball is at least partly outside, this will work
            // Add the minimum distance from ball to each side
            for (int i = 0; i < vertices.Length; i++)
            {
                
                // Fix this so that we dont have to create new vectors geeze
                Vector2[] triangle = new Vector2[] { new Vector2(vertices[i].X + x, vertices[i].Y+y), 
                    new Vector2(vertices[(i + 1) % vertices.Length].X + x, vertices[(i + 1) % vertices.Length].Y + y), 
                    new Vector2(c.X, c.Y) };
                StraigthenTriangle(triangle); // Make p1 and p2 parallel with x-axis
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
            for (int i = 1; i < vertices.Length; i++)
            {
                if (sideLengths[i] < smallestDistance)
                {
                    smallestDistance = sideLengths[i];
                    smallestDistanceIndex = i;
                }
            }
            // Check if the smallest distance is smaller than the radius of the ball
            float cubedRad = (float)Math.Pow(c.Rad, 2);
            if (smallestDistance <= cubedRad || isPointInPolygon)
            {
                bool hittingCorner = false;
                int secondIndex = 0;
                for (int i = 0; i < sideLengths.Length; i++)
                {
                    if (smallestDistanceIndex != i && smallestDistance == sideLengths[i])
                    {
                        hittingCorner = true;
                        secondIndex = i;
                    }
                }
                Debug.Log(smallestDistance.ToString());
                Vector2 normal;
                return CalculateNormalVector(vertices[smallestDistanceIndex], vertices[(smallestDistanceIndex + 1) % vertices.Length]);
                switch (smallestDistanceIndex)
                {
                    case 0:
                        Debug.Log("Top");
                        Debug.Log(CalculateNormalVector(vertices[0], vertices[1]).ToString());
                        normal = CalculateNormalVector(vertices[0], vertices[1]);
                        return normal;
                        break;
                    case 1:
                        Debug.Log("Right");
                        Debug.Log(CalculateNormalVector(vertices[1], vertices[2]).ToString());
                        normal = CalculateNormalVector(vertices[1], vertices[2]);
                        return normal;
                        break;
                    case 2:
                        Debug.Log("Bottom");
                        Debug.Log(CalculateNormalVector(vertices[2], vertices[3]).ToString());
                        normal = CalculateNormalVector(vertices[2], vertices[3]);
                        return normal;
                        break;
                    case 3:
                        Debug.Log("Left");
                        Debug.Log(CalculateNormalVector(vertices[3], vertices[0]).ToString());
                        normal = CalculateNormalVector(vertices[3], vertices[0]);
                        return normal;
                        break;
                }
            }
            return null;
        }

        public override void drawMe(Color col)
        {
            Display d = Bootstrap.getDisplay();

            float centerX = (vertices[0].X + vertices[2].X) / 2;
            float centerY = (vertices[0].Y + vertices[2].Y) / 2;

            for (int i = 0; i < vertices.Length; i++)
            {
                d.drawLine((int)(vertices[i].X + x), (int)(vertices[i].Y + y), (int)(vertices[(i+1)% vertices.Length].X + x), (int)(vertices[(i+1)% vertices.Length].Y + y), col);
            }


            d.drawCircle((int)centerX, (int)centerY, 2, col);
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
            return new float[]{ miny + y - 10,maxy + y + 10};
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
            return new float[] { minx + x - 10 , maxx + x + 10};
        }
    }
}
