﻿/*
*
*   The collider for circles.   Handles circle/circle, circle/rect, and circle/point collisions.
*   @author Michael Heron
*   @version 1.0
*   
*/

using Shard.Shard;
using System;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;

namespace Shard
{
    class ColliderCircle : Collider
    {
        private Transform myRect;
        private float x, y, rad, lx, ly;
        private float xoff, yoff;
        private bool fromTrans;
        public ColliderCircle(CollisionHandler gob, Transform t) : base(gob)
        {

            this.MyRect = t;
            fromTrans = true;
            RotateAtOffset = false;
            calculateBoundingBox();
        }

        public ColliderCircle(CollisionHandler gob, Transform t, float x, float y, float rad) : base(gob)
        {

            Xoff = x;
            Yoff = y;
            X = Xoff;
            Y = Yoff;
            Rad = rad;
            RotateAtOffset = true;

            this.MyRect = t;

            fromTrans = false;

            calculateBoundingBox();

        }



        public void calculateBoundingBox()
        {
            float x1, x2, y1, y2;
            float intWid;
            float angle = (float)(Math.PI * MyRect.Rotz / 180.0f);

            if (fromTrans)
            {
                intWid = MyRect.Wid * (float)MyRect.Scalex;
                Rad = (float)(intWid / 2);
                X = (float)MyRect.X + Xoff + Rad;
                Y = (float)MyRect.Y + Yoff + Rad;
            }
            else
            {
                X = (float)MyRect.X + Xoff;
                Y = (float)MyRect.Y + Yoff;
            }

            if (RotateAtOffset == true) {
                // Now we work out the X and Y based on the rotation of the body to 
                // which this belongs,.
                x1 = X - MyRect.Centre.X;
                y1 = Y - MyRect.Centre.Y;

                x2 = (float)(x1 * Math.Cos(angle) - y1 * Math.Sin(angle));
                y2 = (float)(x1 * Math.Sin(angle) + y1 * Math.Cos(angle));

                X = x2 + (float)MyRect.Centre.X;
                Y = y2 + (float)MyRect.Centre.Y;
            }

            MinAndMaxX[0] = X - Rad;
            MinAndMaxX[1] = X + Rad;
            MinAndMaxY[0] = Y - Rad;
            MinAndMaxY[1] = Y + Rad;
        }
        internal Transform MyRect { get => myRect; set => myRect = value; }

        public float Lx { get => lx; set => lx = value; }
        public float Ly { get => ly; set => ly = value; }
        public float X { get => x; set
            {
                lx = x;
                x = value;
            }
        }
        public float Y { get => y; set
            {
                ly = y;
                y = value;
            }
        }
        public float Rad { get => rad; set => rad = value; }

        public float Left { get => MinAndMaxX[0]; set => MinAndMaxX[0] = value; }
        public float Right { get => MinAndMaxX[1]; set => MinAndMaxX[1] = value; }
        public float Top { get => MinAndMaxY[0]; set => MinAndMaxY[0] = value; }
        public float Bottom { get => MinAndMaxY[1]; set => MinAndMaxY[1] = value; }
        public float Xoff { get => xoff; set => xoff = value; }
        public float Yoff { get => yoff; set => yoff = value; }

        public override void recalculate()
        {
            calculateBoundingBox();
        }

        public override Vector2? checkCollision(ColliderPolygon c)
        {
            // The collisionCheck for this combination is already defined in ColliderPolygon
            return -c.checkCollision(this);
        }
        public override Vector2? checkCollision(ColliderRect other)
        {

            double tx = X;
            double ty = Y;
            double dx, dy, dist;
            Vector2 dir;
            double depth;

            if (X < other.Left)
            {
                tx = other.Left;
            }
            else if (X > other.Right)
            {
                tx = other.Right;
            }


            if (Y < other.Top)
            {
                ty = other.Top;
            }
            else if (Y > other.Bottom)
            {
                ty = other.Bottom;
            }


            // PYTHAGORAS YO
            dx = X - tx;
            dy = Y - ty;

            dist = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

            // if the distance is less than the radius, collision!
            if (dist < Rad)
            {
                depth = Rad - dist;

                if (dist == 0)
                {
                    // Here we hit the exact edge, oh no.  This will cause the vector calculations to break.
                    // You can't normalize a 0,0 vector - it's mathematically incoherent.   
                    //
                    // So what we need to do is get the direction the circle was moving, reverse it, and then push it 
                    // out that way.  We have to do it that way otherwise we *might* push it through a collider.
                    // We have to assume if the last position it was in was fine after the physics took effect, then 
                    // it is hopefully fine for us to push it there.
                    

                    dir = MyRect.getLastDirection();
                    Vector2 newDir = new Vector2(dir.X, dir.Y);

                    dir = Vector2.Normalize(newDir);

                }
                else
                {

                    dir = new Vector2((float)dx, (float)dy);

                    dir = Vector2.Normalize(dir);
                    dir *= (float)depth;
                }



                return dir;
            }

            return null;
        }

        public override void drawMe(Color col)
        {
            Display d = Bootstrap.getDisplay();

            d.drawCircle((int)X, (int)Y, (int)Rad, col);

        }



        public override Vector2? checkCollision(ColliderCircle c)
        {

            Vector2 normal = new Vector2(c.x, c.y) - new Vector2(this.x, this.y);
            if (normal.Length() <= c.Rad + this.Rad) {
                float test = normal.Length();
                return Vector2.Normalize(normal);
            } 

            return null;
        }

        public override float[] getMinAndMaxX()
        {
            return MinAndMaxX;
        }

        public override float[] getMinAndMaxY()
        {
            return MinAndMaxY;
        }

        public override Vector2? checkCollision(Vector2 c)
        {

            if (c.X >= Left &&
                c.X <= Right &&
                c.Y >= Top &&
                c.Y <= Bottom)
            {
                return new Vector2(0, 0);
            }
            return null;
        }
    }
}
