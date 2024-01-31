using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shard.Shard
{
    internal class NewCircleCollider: NewCollider
    {
        CollisionHandler collisionHandler;
        private float x, y, radius;
        public NewCircleCollider(CollisionHandler collisionHandler, float x, float y, float radius) : base(collisionHandler)
        {
            X = x;
            Y = x;
            Radius = radius;
        }

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

        public float Radius
        {
            get { return radius; }
            set { 
                if(value >= 0)
                {
                    radius = value;
                }
                else
                {
                    radius = 0;
                }
            }
        }

        // These functions return the normal of the surface that the circle collided with
        public override Vector2? checkCollision(ColliderRect other)
        {
            // Check if the circle is colliding with the rectangle
            float otherX = X;
            float otherY = Y;

            // Check if this circle is to left or right of rectangle
            if (X < other.Left)
            {
                otherX = other.Left;
            }
            else if (X > other.Right)
            {
                otherX = other.Right;
            }

            // Check if this circle is to top or bottom of rectangle
            if (Y < other.Top) otherY = other.Top;
            else if (Y > other.Bottom) otherY = other.Bottom;


            // Difference between one of rectangles sides and circle's middle
            float dx = X - otherX;
            float dy = Y - otherY;
            // Get the length of the hypotenuse from circle to rectangle
            float dist = (float)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
            if(dist < radius)
            {
                // Collision is occuring

            }
            return null;
        }

        public override Vector2? checkCollision(Vector2 c)
        {
            throw new NotImplementedException();
        }

        public override Vector2? checkCollision(ColliderCircle c)
        {
            throw new NotImplementedException();
        }

        public override void drawMe(Color col)
        {
            throw new NotImplementedException();
        }

        public override void recalculate()
        {
            throw new NotImplementedException();
        }
    }
}
