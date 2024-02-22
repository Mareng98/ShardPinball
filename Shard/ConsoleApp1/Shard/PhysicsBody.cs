/*
*
*   The physics body class does... a lot.  It handles the computation of internal values such as 
*       the min and max values for X and Y (used by the Sweep and Prune algorithm, as well as 
*       collision detection in general).  It registers and processes the colliders that belong to 
*       an object.  It handles the application of forces and torque as well as drag and angular drag.
*       It lets an object add colliders, and then exposes those colliders for narrow phase collision 
*       detection.  It handles some naive default collision responses such as a simple reflection
*       or 'stop on collision'.
*       
*   Important to note though that while this is called a PhysicsBody, no claims are made for the 
*       *accuracy* of the physics.  If you are planning to do anything that requires the physics
*       calculations to be remotely correct, you're going to have to extend the engine so it does 
*       that.  All I'm interested in here is showing you how it's *architected*. 
*       
*   This is also the subsystem which I am least confident about people relying on, because it is 
*       virtually untestable in any meaningful sense.  I spent three days trying to track down a 
*       bug that mean that an object would pass through another one at a rate of approximately
*       once every half hour...
*       
*   @author Michael Heron
*   @version 1.0
*   
*   Several substantial contributions to the code made by others:
*   @author Mårten Åsberg (see Changelog for 1.0.1)
*   
*/

using Shard.Shard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Shard
{
    class PhysicsBody
    {
        List<Collider> myColliders;
        List<Collider> collisionCandidates;
        GameObject parent;
        CollisionHandler colh;
        Transform trans;
        private float angularDrag;
        private float drag;
        private float torque;
        private Vector2 force;
        private float mass;
        private float frictionCoefficient;
        private double timeInterval;
        private float maxForce, maxTorque;
        private bool kinematic;
        private bool stopOnCollision;
        private bool reflectOnCollision;
        private bool impartForce;
        private bool passThrough;
        private bool usesGravity;
        private Color debugColor;
        private List<Vector2> collisionNormals;
        private List<PhysicsBody> collisionObjects;
        private Vector2 gravityDir;
        public Color DebugColor { get => debugColor; set => debugColor = value; }
        public Vector2 Force { get => force; set => force = value; }

        public float FrictionCoefficient {
            get { return frictionCoefficient; }
            set { frictionCoefficient = value; }
        }

        private float[] minAndMaxX;
        private float[] minAndMaxY;

        public void AddFriction()
        {

            // Calculate the magnitude of the velocity
            float speed = force.Length();

            // Only apply friction when there's a sliding collision, the gravityDir.Y is never 1 when this happens.
            if(gravityDir.Y != 1)
            {
                Vector2 frictionForce = -frictionCoefficient * force;
                // Only reduce speed if it's above a threshold, so it never truly stops
                if (speed - frictionForce.Length() > 1f)
                {
                    // Apply the friction force to the ball
                    addForce(frictionForce);
                }
                else
                {
                    force = Vector2.Normalize(force) * 1f;
                }
            }
        }

        public void applyGravity(float modifier, Vector2 dir)
        {
            if (modifier == 0 || dir == Vector2.Zero) return;
            Vector2 projectedGravity;
            Vector2 newDirection;
            Vector2 collisionNormal = new Vector2(0,0);
            // Calculate the direction and magnitude of gravity (Depends on the surface which the object might be in contact with.
            for (int i = 0; i < collisionNormals.Count; i++)
            {
                Vector2 newNormal = collisionNormals[i];
                if (collisionNormal.X == 0 && collisionNormal.Y != 0)
                {
                    // CollisionNormal's plane is already horizontal

                    break;
                }
                else if (collisionNormal.X == 0 && collisionNormal.Y == 0 || newNormal.X == 0 && newNormal.Y < 0)
                {
                    // CollisionNormal is either 0, or newNormal's plane is horizontal
                    collisionNormal = newNormal;
                }
                else if (newNormal.X == 0 && newNormal.Y == 0)
                {
                    // newNormal is 0 (should never happen)
                    continue;
                }
                else if (newNormal.Y < 0)
                {
                    // collisionNormal X != 0 and Y > 0
                    // newNormal X != 0 and Y > 0
                    // compare the slopes
                    float newNormalSlope = Math.Abs(newNormal.Y / newNormal.X);
                    float collisionNormalSlope = Math.Abs(collisionNormal.Y / collisionNormal.X);
                    if (newNormalSlope < collisionNormalSlope)
                    {
                        // collisionNormal has steeper slope than newNormal, set it to newNormal.
                        collisionNormal = newNormal;
                    }
                }
            }
            if(collisionNormal.Y < 0)
            {
                if(collisionNormal.X > 0)
                {
                    // First quadrant - Rotate normal 90 degrees clockwise
                    newDirection = new Vector2(-collisionNormal.Y,collisionNormal.X);
                    projectedGravity = (Vector2.Dot(dir, newDirection) / Vector2.Dot(newDirection, newDirection)) * newDirection;
                }
                else if(collisionNormal.X < 0)
                {
                    // Second quadrant - Rotate normal 90 degrees anti-clockwise
                    newDirection = new Vector2(collisionNormal.Y, -collisionNormal.X);
                    projectedGravity = (Vector2.Dot(dir, newDirection) / Vector2.Dot(newDirection, newDirection)) * newDirection;
                }
                else
                {
                    projectedGravity = new Vector2(0, 0);
                }
            }
            else
            {
                projectedGravity = dir;
            }

            gravityDir = projectedGravity;
            force += projectedGravity* modifier;
        }

        public float AngularDrag { get => angularDrag; set => angularDrag = value; }
        public float Drag { get => drag; set => drag = value; }
        internal GameObject Parent { get => parent; set => parent = value; }
        internal Transform Trans { get => trans; set => trans = value; }
        public float Mass { get => mass; set => mass = value; }
        public float[] MinAndMaxX { get => minAndMaxX; set => minAndMaxX = value; }
        public float[] MinAndMaxY { get => minAndMaxY; set => minAndMaxY = value; }
        public float MaxForce { get => maxForce; set => maxForce = value; }
        public float MaxTorque { get => maxTorque; set => maxTorque = value; }

        public float Torque { get => torque; set => torque = value; }
        public bool Kinematic { get => kinematic; set => kinematic = value; }
        public bool PassThrough { get => passThrough; set => passThrough = value; }
        public bool UsesGravity { get => usesGravity; set => usesGravity = value; }
        public bool StopOnCollision { get => stopOnCollision; set => stopOnCollision = value; }
        public bool ReflectOnCollision { get => reflectOnCollision; set => reflectOnCollision = value; }
        public bool ImpartForce { get => this.impartForce; set => this.impartForce = value; }
        internal CollisionHandler Colh { get => colh; set => colh = value; }

        public void drawMe()
        {
            foreach (Collider col in myColliders)
            {
                col.drawMe(DebugColor);
            }
        }

        public float[] getMinAndMax(bool x)
        {
            float min = Int32.MaxValue;
            float max = -1 * min;
            float[] tmp;

            // distance holds the x or y component of the vector from old point to new point
            // depending on if minXAndMaxX or minYAndMaxY
            float distance;
            foreach (Collider col in myColliders)
            {

                if (x)
                {
                    tmp = col.MinAndMaxX;
                    distance = Math.Abs(trans.X - trans.Lx);
                }
                else
                {
                    tmp = col.MinAndMaxY;
                    distance = Math.Abs(trans.Y - trans.Ly);
                }

                // we then use distance to increase the perimeter of the min and max points
                // that are later used in sweep and prune alg.
                min = Math.Min(tmp[0], min) - distance;
                max = Math.Max(max, tmp[1]) + distance;
            }

            return new float[2] { min, max };
        }

        public PhysicsBody(GameObject p)
        {
            DebugColor = Color.Green;
            collisionNormals = new List<Vector2>();
            collisionObjects = new List<PhysicsBody>();
            myColliders = new List<Collider>();
            collisionCandidates = new List<Collider>();

            Parent = p;
            Trans = p.Transform;
            Colh = (CollisionHandler)p;

            AngularDrag = 0.01f;
            Drag = 0.01f;
            Drag = 0.01f;
            Mass = 1;
            MaxForce = 10;
            MaxTorque = 2;
            usesGravity = false;
            stopOnCollision = true;
            reflectOnCollision = false;
            frictionCoefficient = 0;
            MinAndMaxX = new float[2];
            MinAndMaxY = new float[2];
            gravityDir = new Vector2(0, 1);
            timeInterval = PhysicsManager.getInstance().TimeInterval;
            //            Debug.getInstance().log ("Setting physics enabled");

            PhysicsManager.getInstance().addPhysicsObject(this);
        }

        public void addTorque(float dir)
        {
            if (Kinematic)
            {
                return;
            }

            torque += dir / Mass;

            if (torque > MaxTorque)
            {
                torque = MaxTorque;
            }

            if (torque < -1 * MaxTorque)
            {
                torque = -1 * MaxTorque;
            }
        }

        public void reverseForces(float prop)
        {
            if (Kinematic)
            {
                return;
            }

            force *= -prop;
        }

        public void impartForces(PhysicsBody other, float massProp)
        {
            other.addForce(force * massProp);

            recalculateColliders();

        }

        public void stopForces()
        {
            force = Vector2.Zero;
        }

        // Normal of the collision surface, and the physics body of the collider this collided with.
        public void AddCollisionInfo(Vector2 normal, PhysicsBody other)
        {
            normal = Vector2.Normalize(normal);
            if(!(normal.X == 0 && normal.Y == 0))
            {
                collisionNormals.Add(normal);
                collisionObjects.Add(other);
            }
        }

        public void Reflect()
        {
            for(int i = 0; i < collisionNormals.Count; i++)
            {

                Vector2 normal = collisionNormals[i];
                PhysicsBody other = collisionObjects[i];
                // The speed at which this object is moving away from the other object
                Vector2 dspeed = other.Force - this.Force;
                // The proportation of which dspeed is along the normal pointing out from the collision surface of other object
                float dotProduct = Vector2.Dot(normal, dspeed);
                // If dotProduct is positive, then the objects are colliding (without accounting for the possiblity of rotation)
                if(dotProduct > 0)
                {
                    if(other.Torque != 0)
                    {
                        // Add angular forces before reflecting
                        Vector2 pivot = other.Trans.Pivot;
                        Vector2 distanceVector = new Vector2(this.Trans.X, this.Trans.Y) - pivot;

                        // Determine the direction of the torque (clockwise or anticlockwise)
                        int torqueDirection = Math.Sign(other.Torque);

                        // Calculate the cross product to get a vector perpendicular to distanceVector and torque direction
                        Vector3 crossProduct = Vector3.Cross(new Vector3(distanceVector, 0), new Vector3(0, 0, torqueDirection));

                        // Extract the 2D result from the cross product
                        Vector2 addedVelocityDirection = new Vector2(crossProduct.X, crossProduct.Y);

                        // Calculate the magnitude of the added velocity
                        float addedVelocityMagnitude = Math.Abs(distanceVector.Length() * other.Torque * 0.02f);

                        if(addedVelocityDirection.Y > 0)
                        {
                            addedVelocityDirection.Y *= -1;
                        }

                        // Add the velocity in the correct direction
                        this.Force += addedVelocityMagnitude * Vector2.Normalize(addedVelocityDirection);
                    }
                    
                    
                    if (other.reflectOnCollision && this.reflectOnCollision)
                    {
                        // The force which to add along the collision normal to both objects
                        Vector2 impulse = 2 * dotProduct * normal;
                        // Split the impulse in proportion to their masses
                        this.force += (other.mass / (this.mass + other.mass)) * impulse;
                        other.force -= (this.mass / (this.mass + other.mass)) * impulse;
                    }
                    else
                    {
                        // The mass of other is infinite, simply reflect along the normal
                        force = Vector2.Reflect(this.Force, normal);
                    }
                    
                    
                }
            }
            collisionNormals.Clear();
            collisionObjects.Clear();
        }

        public void reduceForces(float prop) {
            force *= prop;
        }

        public void addForce(Vector2 dir, float force) {
            addForce(dir * force);
        }

        public void addForce(Vector2 dir)
        {
            if (Kinematic)
            {
                return;
            }

            dir /= Mass;

            // Set a lower bound.
            if (dir.LengthSquared() < 0.0001)
            {
                return;
            }

            force += dir;

            // Set a higher bound.
            if (force.Length() > MaxForce)
            {
                force = Vector2.Normalize(force) * MaxForce;
            }
        }

        public void recalculateColliders()
        {
            foreach (Collider col in getColliders())
            {
                col.recalculate();
            }

            MinAndMaxX = getMinAndMax(true);
            MinAndMaxY = getMinAndMax(false);
        }

        public void physicsTick()
        {
            List<Vector2> toRemove;
            float force;
            float rot = 0;


            toRemove = new List<Vector2>();

            rot = torque;

            if (Math.Abs(torque) < AngularDrag)
            {
                torque = 0;
            }
            else
            {
                torque -= Math.Sign(torque) * AngularDrag;
            }

            trans.rotate(rot);
            force = this.force.Length();
            trans.translate(this.force);

            if (force < Drag)
            {
                stopForces();
            }
            else if (force > 0)
            {
                this.force = (this.force / force) * (force - Drag);
            }


        }


        public ColliderRect addRectCollider()
        {
            ColliderRect cr = new ColliderRect((CollisionHandler)parent, parent.Transform);

            addCollider(cr);

            return cr;
        }

        public ColliderCircle addCircleCollider()
        {
            ColliderCircle cr = new ColliderCircle((CollisionHandler)parent, parent.Transform);

            addCollider(cr);

            return cr;
        }

        public ColliderCircle addCircleCollider(int x, int y, int rad)
        {
            ColliderCircle cr = new ColliderCircle((CollisionHandler)parent, parent.Transform, x, y, rad);

            addCollider(cr);

            return cr;
        }


        public ColliderRect addRectCollider(int x, int y, int wid, int ht)
        {
            ColliderRect cr = new ColliderRect((CollisionHandler)parent, parent.Transform, x, y, wid, ht);

            addCollider(cr);

            return cr;
        }

        public NewColliderRectangle addNewRectCollider(float x, float y, float w, float h, float r)
        {
            NewColliderRectangle cnr = new NewColliderRectangle((CollisionHandler)parent, parent.Transform, x, y, w, h, r);
            addCollider(cnr);
            return cnr;
        }

        public NewColliderRectangle addNewRectCollider(float x, float y, Vector2[] vertices, float r)
        {
            NewColliderRectangle cnr = new NewColliderRectangle((CollisionHandler)parent, parent.Transform, x, y, vertices, r);
            addCollider(cnr);
            return cnr;
        }

        public NewColliderRectangle addNewRectCollider(float x, float y, Vector2[] vertices, float r, Vector2 rotationPivot)
        {
            NewColliderRectangle cnr = new NewColliderRectangle((CollisionHandler)parent, parent.Transform, x, y, vertices, r, rotationPivot);
            addCollider(cnr);
            return cnr;
        }


        public void addCollider(Collider col)
        {
            myColliders.Add(col);
        }

        public List<Collider> getColliders()
        {
            return myColliders;
        }

        public Vector2? checkCollisions(Vector2 other)
        {
            Vector2? d;


            foreach (Collider c in myColliders)
            {

                d = c.checkCollision(other);

                if (d != null)
                {
                    return d;
                }
            }

            return null;
        }


        public Vector2? checkCollisions(Collider other)
        {
            Vector2? d;

//            Debug.Log("Checking collision with " + other);
            foreach (Collider c in myColliders)
            {
                d = c.checkCollision(other);

                if (d != null)
                {
                    return d;
                }
            }

            return null;
        }
    }
}