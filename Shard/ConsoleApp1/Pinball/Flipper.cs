using System;
using System.Collections.Generic;
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
        private FlipperRotationDirection rotationDirection;

        public FlipperSide Side
        {
            get { return side; }
        }

        public FlipperRotationDirection RotatationDirection
        {
            get { return rotationDirection; }
            set { rotationDirection = value; }
        }
        public NewColliderRectangle Collider { get; set; }

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
                rotationPivot = new Vector2(leftRadius, leftRadius);
            }
            else
            {
                rotationPivot = new Vector2(leftRadius + width, rightRadius);
            }

            Collider = MyBody.addNewRectCollider(x, y, vertices.ToArray(), 0, rotationPivot);
            this.side = side;
            rotationDirection = FlipperRotationDirection.Stop;
        }

        private float YfromX(float radius, float x)
        {

            float y = (float)Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(x, 2));
            return y;
        }

        public override void initialize()
        {
            setPhysicsEnabled();

            MyBody.Mass = 15000;
            MyBody.MaxForce = 15000;
            MyBody.Drag = 0f;
            MyBody.UsesGravity = false;
            MyBody.StopOnCollision = false;
            MyBody.ReflectOnCollision = true;
        }

        public override void update()
        {
            if(side == FlipperSide.Left)
            {
                switch (rotationDirection)
                {
                    case FlipperRotationDirection.Up:
                        if (this.Collider.Rotation > -1)
                        {
                            this.Collider.Rotation -= 0.01f;
                        }
                        break;
                    case FlipperRotationDirection.Stop:
                        if (this.Collider.Rotation < 0)
                        {
                            this.Collider.Rotation += 0.01f;
                        }
                        break;
                }
            }
            else
            {
                switch (rotationDirection)
                {
                    case FlipperRotationDirection.Up:
                        if (this.Collider.Rotation < 1)
                        {
                            this.Collider.Rotation += 0.01f;
                        }
                        break;
                    case FlipperRotationDirection.Stop:
                        if (this.Collider.Rotation > 0)
                        {
                            this.Collider.Rotation -= 0.01f;
                        }
                        break;
                }
            }
            
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
