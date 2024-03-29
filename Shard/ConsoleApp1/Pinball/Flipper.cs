﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Shard;
namespace Pinball
{
    class Flipper : GameObject, CollisionHandler
    {
        private FlipperSide side;
        private FlipperDirection rotationDirection;

        public FlipperSide Side
        {
            get { return side; }
        }

        public FlipperDirection RotatationDirection
        {
            get { return rotationDirection; }
            set { rotationDirection = value; }
        }
        public ColliderPolygon Collider { get; set; }

        public Flipper(string tag, int x, int y, int width, int leftHeight, int rightHeight, FlipperSide side)
        {
            addTag(tag);
            Vector2[] topLeft = new Vector2[4];
            Vector2[] topRight = new Vector2[4];
            Vector2[] bottomRight = new Vector2[4];
            Vector2[] bottomLeft = new Vector2[4];
            List<Vector2> vertices = new List<Vector2>();
            float leftRadius = leftHeight / 2;
            float leftSectionSpacing = leftRadius / 5;
            float rightRadius = rightHeight / 2;
            float rightSectionSpacing = rightRadius / 5;
            // Topleft
            for(float xCoord = leftSectionSpacing; xCoord <= leftRadius; xCoord += leftSectionSpacing)
            {
                vertices.Add(new Vector2(xCoord, -YfromX(leftRadius,xCoord - leftRadius)));
            }
            // Topright
            for (float xCoord = rightSectionSpacing; xCoord <= rightRadius; xCoord += rightSectionSpacing)
            {
                vertices.Add(new Vector2(xCoord + width, -YfromX(rightRadius, xCoord)));
            }
            // Bottomright
            for (float xCoord = rightRadius - rightSectionSpacing; xCoord >= 0; xCoord -= rightSectionSpacing)
            {
                vertices.Add(new Vector2(xCoord + width, YfromX(rightRadius, xCoord)));
            }
            // Bottomleft
            for (float xCoord = leftRadius - leftSectionSpacing; xCoord >= 0; xCoord -= leftSectionSpacing)
            {
                vertices.Add(new Vector2(xCoord, YfromX(leftRadius, xCoord - leftRadius)));
            }
            Vector2 rotationPivot;
            if(side == FlipperSide.Left)
            {
                rotationPivot = new Vector2(leftRadius, 0);
            }
            else
            {
                rotationPivot = new Vector2(width,0);
            }

            Collider = MyBody.addPolygonCollider(x, y, vertices.ToArray(), 0);
            this.side = side;
            rotationDirection = FlipperDirection.Stop;
            MyBody.Trans.Pivot = rotationPivot + new Vector2(x,y);
        }

        private float YfromX(float radius, float x)
        {

            float y = (float)Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(x, 2));
            return y;
        }

        public override void initialize()
        {
            setPhysicsEnabled();

            MyBody.Mass = 1;
            MyBody.MaxForce = 1337;
            MyBody.MaxTorque = 15000;
            MyBody.AngularDrag = 0f;
            MyBody.Drag = 0f;
            MyBody.UsesGravity = false;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = false;
            MyBody.Trans.UsesMaxAngle = true;
            MyBody.Trans.MaxAngle = 90f; // degrees
            MyBody.MomentOfInertia = 0.2f;
        }

        public override void update()
        {
            if(side == FlipperSide.Left)
            {
                switch (rotationDirection)
                {
                    case FlipperDirection.Up:
                        if (MyBody.Trans.Rotz > -1)
                        {
                            if (MyBody.AngularVelocity > 0) MyBody.AngularVelocity = 0;
                            MyBody.NetTorque = -0.2f;
                        }
                        break;
                    case FlipperDirection.Stop:
                        if (MyBody.Trans.Rotz < 0)
                        {
                            MyBody.NetTorque = 0.2f;
                        }
                        else
                        {
                            MyBody.AngularVelocity = 0;
                            MyBody.NetTorque = 0f;
                        }
                        break;
                }
            }
            else
            {
                switch (rotationDirection)
                {
                    case FlipperDirection.Up:
                        if (MyBody.Trans.Rotz < 1)
                        {
                            if (MyBody.AngularVelocity < 0) MyBody.AngularVelocity = 0;
                            MyBody.NetTorque = 0.2f;
                        }
                        break;
                    case FlipperDirection.Stop:
                        if (MyBody.Trans.Rotz > 0)
                        {
                            MyBody.NetTorque = -0.2f;
                        }
                        else
                        {
                            MyBody.AngularVelocity = 0;
                            MyBody.NetTorque = 0;
                        }
                        break;
                }
            }
            Collider.DrawingColor = Color.LightCoral;
            Bootstrap.getDisplay().addToDraw(this);

        }



        public void onCollisionEnter(PhysicsBody x)
        {
        }

        public void onCollisionExit(PhysicsBody x)
        {
        }

        public void onCollisionStay(PhysicsBody x)
        {
        }

        public override string ToString()
        {
            return "NewRectangle: [" + Transform.X + ", " + Transform.Y + "]";
        }
    }
}
