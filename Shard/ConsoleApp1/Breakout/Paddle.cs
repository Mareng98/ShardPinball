using SDL2;

using Shard;

namespace GameBreakout
{
    class Paddle : GameObject, InputListener, CollisionHandler
    {
        bool left, right;
        int wid;


        public Paddle()
        {
            MyBody.addPolygonCollider(500, 800, 80, 20, 0);
        }

        public override void initialize()
        {

            this.Transform.X = 500.0f;
            this.Transform.Y = 800.0f;
            this.Transform.SpritePath = Bootstrap.getAssetManager().getAssetPath("test.png");
            this.Transform.Scaley = 0.5f;
            this.Transform.Scalex = 1.5f;


            Bootstrap.getInput().addListener(this);

            left = false;
            right = false;

            setPhysicsEnabled();

            MyBody.Mass = 1000;
            MyBody.MaxForce = 20;
            MyBody.Drag = 0.1f;


            addTag("Paddle");

            wid = Bootstrap.getDisplay().getWidth();
        }

        public void handleInput(InputEvent inp, string eventType)
        {



            if (eventType == "KeyDown")
            {
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_D)
                {
                    right = true;
                }

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_A)
                {
                    left = true;
                }

            }
            else if (eventType == "KeyUp")
            {
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_D)
                {
                    right = false;
                }

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_A)
                {
                    left = false;
                }


            }



        }

        public override void update()
        {
            Bootstrap.getDisplay().addToDraw(this);
        }

        public override void physicsUpdate()
        {

            double boundsx;

            if (left)
            {
                MyBody.addForce(new System.Numerics.Vector2(-1,0), 2000f);
            }


            if (right)
            {
                MyBody.addForce(new System.Numerics.Vector2(1, 0), 2000f);
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
            return "Paddle: [" + Transform.X + ", " + Transform.Y + ", " + Transform.Wid + ", " + Transform.Ht + "]";
        }

    }
}
