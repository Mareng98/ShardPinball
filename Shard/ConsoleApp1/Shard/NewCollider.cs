using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shard.Shard
{
    abstract class NewCollider
    {
        private CollisionHandler collisionHandler;

        public abstract void recalculate();
        public NewCollider(CollisionHandler ch)
        {
            collisionHandler = ch;
        }

        internal CollisionHandler CollisionHandler { get => collisionHandler; set => collisionHandler = value; }

        public abstract Vector2? checkCollision(ColliderRect c);

        public abstract Vector2? checkCollision(Vector2 c);

        public abstract Vector2? checkCollision(ColliderCircle c);

        public virtual Vector2? checkCollision(Collider c)
        {

            if (c is ColliderRect)
            {
                return checkCollision((ColliderRect)c);
            }else if (c is ColliderCircle)
            {
                return checkCollision((ColliderCircle)c);
            }
            Debug.getInstance().log("Bug");
            // Not sure how we got here but c'est la vie
            return null;
        }

        public abstract void drawMe(Color col);
    }
}
