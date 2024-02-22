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
                RotateVertices(value, vertices, rotationPivot);
            }
        }

        public NewColliderRectangle(CollisionHandler gob, Transform trans, float x, float y) : base(gob)
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

        public NewColliderRectangle(CollisionHandler gob, Transform trans, float x, float y, float w, float h, float r) : base(gob)
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

        public NewColliderRectangle(CollisionHandler gob, Transform trans, float x, float y, float w, float h, float r, Vector2 rotationPivot) : base(gob)
        {
            X = x;
            Y = y;
            width = w;
            height = h;
            vertices = CalculateVertices();
            collisionNormal = new Vector2(0, 0);
            RotationPivot = rotationPivot;
            Rotation = 0;
            this.trans = trans;
        }
        // Use this if you want an uneven rectangle
        public NewColliderRectangle(CollisionHandler gob, Transform trans, float x, float y, Vector2[] inputVertices, float r) : base(gob)
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
            this.trans = trans;
        }

        public NewColliderRectangle(CollisionHandler gob, Transform trans, float x, float y, Vector2[] inputVertices, float r, Vector2 rotationPivot) : base(gob)
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
            this.trans = trans;
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
            RotateVertices(trans.Rotz, vertices, rotationPivot);
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
            Vector2 normalVector = new Vector2(deltaY, -deltaX);

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



        public Vector2? checkCollisionv2(ColliderCircle c)
        {
            Vector2 ballOrigin = new Vector2(c.X, c.Y);
            Vector2 lastBallOrigin = new Vector2(c.Lx, c.Ly);
            bool isPointInPolygon = pointInPolygon(ballOrigin);
            bool isPreviousPointInPolygon = pointInPolygon(lastBallOrigin);
            
            if (!CircleInBoundingBox(ballOrigin, c.Rad) || isPreviousPointInPolygon)
            {
                // If ballorigin is not in bounding box, or it was previously inside of polygon, return null
                return null;
            }
            int smallestDistanceIndex = 0;
            float smallestDistance = float.MaxValue;
            bool borderCrossed = false;
            // Check if any border has been passed
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector2 result;
                if(FindIntersection(lastBallOrigin, ballOrigin, new Vector2(vertices[i].X + x, vertices[i].Y + y),
                    new Vector2(vertices[(i + 1) % vertices.Length].X + x, vertices[(i + 1) % vertices.Length].Y), out result))
                {
                    // Intersection found
                    if((lastBallOrigin - result).Length() < smallestDistance)
                    {
                        borderCrossed = true;
                        smallestDistance = (lastBallOrigin - result).Length();
                        smallestDistanceIndex = i;
                    }
                }
            }
            if (borderCrossed)
            {
                return CalculateNormalVector(vertices[smallestDistanceIndex], vertices[(smallestDistanceIndex + 1) % vertices.Length]);
            }
            // (Distance , vertices start index)
            List<(float,int)> collisionSegmentCanidates = new List<(float,int)>();
            for(int i = 0; i < vertices.Length; i++)
            {
                Vector2[] currentDistanceTriangle = { new Vector2(vertices[i].X + x, vertices[i].Y+y),
                    new Vector2(vertices[(i + 1) % vertices.Length].X + x, vertices[(i + 1) % vertices.Length].Y + y),
                    new Vector2(c.X, c.Y) };
                StraigthenTriangle(currentDistanceTriangle);
                Vector2 p0 = currentDistanceTriangle[0];
                Vector2 p2 = currentDistanceTriangle[2];

                Vector2[] lastDistanceTriangle = { new Vector2(vertices[i].X + x, vertices[i].Y+y),
                    new Vector2(vertices[(i + 1) % vertices.Length].X + x, vertices[(i + 1) % vertices.Length].Y + y),
                    new Vector2(c.Lx, c.Ly) };
                StraigthenTriangle(lastDistanceTriangle);
                Vector2 lp0 = lastDistanceTriangle[0];
                Vector2 lp2 = lastDistanceTriangle[2];
                // Check if the ball has passed through this line segment
                if (p2.Y > p0.Y != lp2.Y > lp0.Y)
                {
                    // Add current distance of ball to segment, along with the interesting index
                    collisionSegmentCanidates.Add((Math.Abs(p2.Y - p0.Y),i));
                }
            }
            if(collisionSegmentCanidates.Count > 0)
            {
                var tmp = collisionSegmentCanidates.Min();
                float distance = tmp.Item1;
                int vertexIndex = tmp.Item2;
                if (distance < c.Rad || isPointInPolygon)
                {
                    return CalculateNormalVector(vertices[vertexIndex], vertices[(vertexIndex + 1) % vertices.Length]);
                }
            }
            return null;
            
        }

        // Just remembered that this function has to be in colliderCircle to check against this, but we'll fix it later
        public override Vector2? checkCollision(ColliderCircle c)
        {
            //return checkCollisionv2(c);
            /*Vector2 ballOrigin = new Vector2(c.X, c.Y);
            if (!CircleInBoundingBox(ballOrigin, c.Rad))
            {
                return null;
            }
            bool isPointInPolygon = pointInPolygon(ballOrigin);
            if (isPointInPolygon)
            {
                DrawTriangle(new Vector2[] { new Vector2(Bootstrap.getDisplay().getWidth() / 2 + 200, Bootstrap.getDisplay().getHeight() / 2), new Vector2(Bootstrap.getDisplay().getWidth() / 2 + 300, Bootstrap.getDisplay().getHeight() / 2), new Vector2(Bootstrap.getDisplay().getWidth() / 2 + 250, Bootstrap.getDisplay().getHeight() - 200)}, Color.Green);
                //Debug.Log("Point is in Polygon");
            }*/
            Vector2 ballOrigin = new Vector2(c.X, c.Y);
            Vector2 lastBallOrigin = new Vector2(c.Lx, c.Ly);
            bool isPointInPolygon = pointInPolygon(ballOrigin);
            bool isPreviousPointInPolygon = pointInPolygon(lastBallOrigin);

            if (!CircleInBoundingBox(ballOrigin, c.Rad) || isPreviousPointInPolygon)
            {
                // If ballorigin is not in bounding box, or it was previously inside of polygon, return null
                return null;
            }
            //Display d = Bootstrap.getDisplay();
            //d.drawCircle((int)c.X, (int)c.Y, (int)c.Rad, Color.AliceBlue);
            //drawMe(Color.Green);
            //d.display();
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
                            FindIntersection(lastBallOrigin, ballOrigin,
                        new Vector2(vertices[i].X + x, vertices[i].Y + y),
                        new Vector2(vertices[(i + 1) % vertices.Length].X + x, vertices[(i + 1) % vertices.Length].Y + y), out result);
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
                Vector2[] trianglePrevious = { new Vector2(vertices[i].X + x, vertices[i].Y+y),
                    new Vector2(vertices[(i + 1) % vertices.Length].X + x, vertices[(i + 1) % vertices.Length].Y + y),
                    new Vector2(c.MyRect.Lx + c.Rad, c.MyRect.Ly + c.Rad) };
                StraigthenTriangle(triangle); // Make p1 and p2 parallel with x-axis
                StraigthenTriangle(trianglePrevious);
                
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
                    //sideLengths[i] = Math.Min(GetSquaredSideLength(ballOrigin, p1), GetSquaredSideLength(ballOrigin, p2));
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
                /*float lx = c.MyRect.Lx;
                float ly = c.MyRect.Ly;
                Vector2[] triangle = new Vector2[] { new Vector2(vertices[smallestDistanceIndex].X + x, vertices[smallestDistanceIndex].Y+y),
                    new Vector2(vertices[(smallestDistanceIndex + 1) % vertices.Length].X + x, vertices[(smallestDistanceIndex + 1) % vertices.Length].Y + y),
                    new Vector2(lx, ly) };

                Vector2 p1 = triangle[0];
                Vector2 p2 = triangle[1];
                ballOrigin = triangle[2];
                var d = distanceToNearestLine(p1, p2, ballOrigin);
                var line = p1 + (p2 - p1) * d;
                c.MyRect.Centre.X = line.X;
                c.MyRect.Centre.Y = line.Y;
                */
                /*if(isPointInPolygon){
                    Vector2 intersection;
                    Vector2 ballReverseDirection = Vector2.Normalize((new Vector2(c.Lx, c.Ly) - new Vector2(c.X, c.Y)));
                    bool isThereAnIntersection = FindIntersection(vertices[smallestDistanceIndex] + new Vector2(x,y), vertices[(smallestDistanceIndex + 1) % vertices.Length ] + new Vector2(x, y), 
                        new Vector2(c.X,c.Y), new Vector2(c.Lx,c.Ly), out intersection);
                    
                    if (isThereAnIntersection)
                    {
                        Vector2 newPosition = intersection + ballReverseDirection * c.Rad;
                        c.X = newPosition.X;
                        c.Y = newPosition.Y;
                    }
                    // Fix this later to verify -> if(isThereAnIntersection && isPointInPolygon && )
                }*/
                // Check if the ball is already heading away from the shape
                //Vector2 speedVector = new Vector2(c.Lx, c.Ly) - new Vector2(c.X, c.Y);
                Vector2 normal = CalculateNormalVector(vertices[smallestDistanceIndex], vertices[(smallestDistanceIndex + 1) % vertices.Length]);
                /*float dotProduct = Vector2.Dot(normal, speedVector);
                if(dotProduct >= 0)
                {
                    return normal;
                }
                else
                {
                    return null;
                }*/
                return normal;
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


        private void DrawShape(Vector2[] a)
        {
            Display d = Bootstrap.getDisplay();
            
            for (int i = 0; i < a.Length; i++)
            {
                d.drawLine((int) a[i].X, (int)a[i].Y, (int)a[(i + 1) % a.Length].X, (int)a[(i + 1) % a.Length].Y,Color.Purple);
            }
            d.display();
        }

        private bool FindIntersection(Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2, out Vector2 result)
        {
            // Let Ball's last movement make up the bounding box
            float left = Math.Min(s1.X,e1.X);
            float right = Math.Max(s1.X, e1.X);
            float top = Math.Min(s1.Y, e1.Y);
            float bottom = Math.Max(s1.Y, e1.Y);
            float lineLeft = Math.Min(s2.X, e2.X);
            float lineRight = Math.Max(s2.X, e2.X);
            float lineTop = Math.Min(s2.Y, e2.Y);
            float lineBottom = Math.Max(s2.Y, e2.Y);
            Vector2 topLeft = new Vector2(Math.Min(s1.X, e1.X), Math.Min(s1.Y, e1.Y));
            Vector2 topRight = new Vector2(Math.Max(s1.X, e1.X), Math.Min(s1.Y, e1.Y));
            Vector2 bottomLeft = new Vector2(Math.Min(s1.X, e1.X), Math.Max(s1.Y, e1.Y));
            Vector2 bottomRight = new Vector2(Math.Max(s1.X, e1.X), Math.Max(s1.Y, e1.Y));
            

            /*Vector2[] vectors = { s1, e1, s2, e2 };
            for(int i = 0; i < 4; i++)
            {
                float vx = vectors[i].X;
                float vy = vectors[i].Y;
                if (vx < left)
                {
                    left = vx;
                }
                if(vx > right)
                {
                    right = vx;
                }
                if(vy < top)
                {
                    top = vy;
                }
                if(vy > bottom)
                {
                    bottom = vy;
                }
            }*/
            double dy1 = e1.Y - s1.Y;
            double dx1 = s1.X - e1.X;
            double c1 = dy1 * s1.X + dx1 * s1.Y;

            double dy2 = e2.Y - s2.Y;
            double dx2 = s2.X - e2.X;
            double c2 = dy2 * s2.X + dx2 * s2.Y;

            double delta = dy1 * dx2 - dy2 * dx1;
            //If lines are parallel, the result will be null.
            if(delta == 0)
            {
                result = new Vector2();
                return false;
            }
            else
            {
                result = new Vector2((float)((dx2 * c1 - dx1 * c2) / delta), (float)((dy1 * c2 - dy2 * c1) / delta));
                // Check if the intersection is happening outside of line
                if(result.X < lineLeft || result.X > lineRight || result.Y > lineBottom || result.Y < lineTop)
                {
                    return false;
                }
                // Check if the intersection is happening within the bounding box
                if(result.X < left || result.X > right || result.Y > bottom ||result.Y < top)
                {
                    return false;
                }
                DrawShape([topLeft, topRight, bottomRight, bottomLeft]);
                return true;
            }
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
            Vector2[] triangle = { new Vector2(100, 182), new Vector2(150, 300), new Vector2(300, 50) };
            Vector2[] triangleTmp = { new Vector2(100, 182), new Vector2(150, 300), new Vector2(300, 50) };
            Vector2[] newTriangle = { triangle[0], triangle[1], triangle[2]};
            float dv = distanceToNearestLine(triangle[0], triangle[1], triangle[2]);
            //Vector2 line = triangle[0] + (triangle[2] - triangle[1]) * dv;
            StraigthenTriangle(triangleTmp);
            Vector2 start; 
            //bool isOk = FindIntersection(triangle[0], triangle[1], triangle[2], new Vector2(50,220), out start);
            //d.drawLine((int)triangle[2].X, (int)triangle[2].Y, (int)line.X, (int)line.Y, Color.GreenYellow);
            //DrawTriangle(triangle, Color.AliceBlue);
            for(int i = 0; i < vertices.Length; i++)
            {
                //line = Vector2.Normalize(CalculateNormalVector(vertices[i], vertices[(i + 1) % vertices.Length]))  + vertices[i] + new Vector2(x,y) + (vertices[(i + 1) % vertices.Length] - vertices[i]) / 2;
                //d.drawLine((int)line.X, (int)line.Y, (int)(vertices[i].X + x), (int)(vertices[(i + 1) % vertices.Length].Y + y + (vertices[(i + 1) % vertices.Length] - vertices[i]).Y / 2), Color.GreenYellow);
                Vector2 normal = Vector2.Normalize(CalculateNormalVector(vertices[i], vertices[(i + 1) % vertices.Length]));
                Vector2 translation = new Vector2(x, y);
                Vector2 midPoint = (vertices[i] + vertices[(i + 1) % vertices.Length] + translation*2) / 2;
                start = midPoint + normal*50;
                Vector2 end = midPoint;
                d.drawLine((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, Color.GreenYellow);
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
