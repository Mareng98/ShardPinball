/*
*
*   The Bootstrap - this loads the config file, processes it and then starts the game loop
*   @author Michael Heron
*   @version 1.0
*   
*/

using Shard.Shard;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Shard
{
    class Bootstrap
    {
        public static string DEFAULT_CONFIG = "config.cfg";

        private static Display displayEngine;
        private static Sound soundEngine;
        private static InputSystem input;
        private static AssetManagerBase asset;

        private static int targetFrameRate;
        private static int millisPerFrame;
        private static double deltaTime;
        private static double timeElapsed;
        private static int frames;
        private static List<long> frameTimes;
        private static long startTime;
        private static string baseDir;
        private static Dictionary<string,string> enVars;
        private static Lighting lighting;
        private static string COLLISION_SYSTEM = "";
        public static bool checkEnvironmentalVariable (string id) {
            return enVars.ContainsKey (id);
        }

        
        public static string getEnvironmentalVariable (string id) {
            if (checkEnvironmentalVariable (id)) {
                return enVars[id];
            }

            return null;
        }

        public static string getCollisionSystem()
        {
            return COLLISION_SYSTEM;
        }


        public static double TimeElapsed { get => timeElapsed; set => timeElapsed = value; }

        public static string getBaseDir() {
            return baseDir;
        }

        public static void setup()
        {
            baseDir = Environment.CurrentDirectory;
            setupEnvironmentalVariables(baseDir + "\\" + "envar.cfg");
            setup(baseDir + "\\" + DEFAULT_CONFIG);


        }

        public static void setupEnvironmentalVariables (String path) {
                Console.WriteLine("Path is " + path);

                Dictionary<string, string> config = BaseFunctionality.getInstance().readConfigFile(path);

                enVars = new Dictionary<string,string>();

                foreach (KeyValuePair<string, string> kvp in config)
                {
                    enVars[kvp.Key] = kvp.Value;
                }
        }
        public static double getDeltaTime()
        {

            return deltaTime;
        }

        public static Display getDisplay()
        {
            return displayEngine;
        }

        public static Sound getSound()
        {
            return soundEngine;
        }

        public static InputSystem getInput()
        {
            return input;
        }

        public static AssetManagerBase getAssetManager() {
            return asset;
        }

        public static PhysicsManager GetPhysicsManager()
        {
            return GameStateManager.getInstance().GetPhysicsManager();
        }

        public static GameObjectManager GetGameObjectManager()
        {
            return GameStateManager.getInstance().GetGameObjectManager();
        }


        public static Game getRunningGame()
        {
            return GameStateManager.getInstance().runningGame;
        }
        public static void EnableLight()
        {
            lighting.EnableLight();
        }

        public static void DisableLight()
        {
            lighting.DisableLight();
        }

        public static bool IsLightingOn()
        {
            return lighting.IsLightingOn();
        }

        public static void DrawLightMap()
        {
            lighting.DrawLightMap();
        }

        public static List<LightInfo> GetLightObjects()
        {
            return lighting.GetLightObjects(); 
        }
        public static void ClearLightObjects()
        {
            lighting.ClearLightObjects();
        }

        public static void AddLightObject(int x, int y, int radius, Color lightColor)
        {
            lighting.AddLightObject(x, y, radius, lightColor);
        }

        public static void SetShadowColor(Color shadowColor)
        {
            lighting.ShadowColor = shadowColor;
        }

        public static Color GetShadowColor()
        {
            return lighting.ShadowColor;    
        }


        public static void setup(string path)
        {
            // Makes sure that "0.1" is parsable as a float, instead of "0,1"
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            Console.WriteLine ("Path is " + path);

            Dictionary<string, string> config = BaseFunctionality.getInstance().readConfigFile(path);
            Type t;
            object ob;
            bool bailOut = false;

            lighting = new();

            //phys = PhysicsManager.getInstance();

            foreach (KeyValuePair<string, string> kvp in config)
            {
                t = Type.GetType("Shard." + kvp.Value);

                if (t == null && kvp.Key != "collision")
                {
                    Debug.getInstance().log("Missing Class Definition: " + kvp.Value + " in " + kvp.Key, Debug.DEBUG_LEVEL_ERROR);
                    Environment.Exit(0);
                }

                if (kvp.Key == "collision")
                {
                    ob = null;

                }
                else
                {
                    ob = Activator.CreateInstance(t);
                }






                switch (kvp.Key)
                {
                    case "display":
                        displayEngine = (Display)ob;
                        displayEngine.initialize();
                        break;
                    case "sound":
                        soundEngine = (Sound)ob;
                        break;
                    case "asset":
                        asset = (AssetManagerBase)ob;
                        asset.registerAssets();
                        break;
                    case "game":
                        GameStateManager.getInstance().SetGame((Game)ob);
                        //GameStateManager.getInstance().runningGame = (Game)ob;
                        targetFrameRate = GameStateManager.getInstance().runningGame.getTargetFrameRate();
                        millisPerFrame = 1000 / targetFrameRate;
                        break;
                    case "input":
                        input = (InputSystem)ob;
                        input.initialize();
                        break;
                    case "collision":
                        if (kvp.Value != "quadtree" && kvp.Value != "sap")
                        {
                            bailOut = true;
                        }
                        COLLISION_SYSTEM = kvp.Value;
                        break;
                }

                Debug.getInstance().log("Config file... setting " + kvp.Key + " to " + kvp.Value);
            }

            if (GameStateManager.getInstance().runningGame == null)
            {
                Debug.getInstance().log("No game set", Debug.DEBUG_LEVEL_ERROR);
                bailOut = true;
            }

            if (displayEngine == null)
            {
                Debug.getInstance().log("No display engine set", Debug.DEBUG_LEVEL_ERROR);
                bailOut = true;
            }

            if (soundEngine == null)
            {
                Debug.getInstance().log("No sound engine set", Debug.DEBUG_LEVEL_ERROR);
                bailOut = true;
            }

            if (bailOut)
            {
                Environment.Exit(0);
            }
        }

        public static long getCurrentMillis()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static int getFPS()
        {
            int fps;
            double seconds;

            seconds = (getCurrentMillis() - startTime) / 1000.0;

            fps = (int)(frames / seconds);

            return fps;
        }

        public static int getSecondFPS()
        {
            int count = 0;
            long now = getCurrentMillis();
            int lastEntry;




            if (frameTimes.Count == 0) {
                return -1;
            }

            lastEntry = frameTimes.Count - 1;

            while (frameTimes[lastEntry] > (now - 1000) && lastEntry > 0) {
                lastEntry -= 1;
                count += 1;
            }

            if (lastEntry > 0) {
                frameTimes.RemoveRange (0, lastEntry);
            }

            return count;
        }

        public static int getCurrentFrame()
        {
            return frames;
        }

        static void Main(string[] args)
        {
            long timeInMillisecondsStart, lastTick, timeInMillisecondsEnd;
            long interval;
            int sleep;
            int tfro = 1;
            bool physUpdate = false;
            bool physDebug = false;
            bool callOnce = false;



            // Setup the engine.
            setup();

            // When we start the program running.
            startTime = getCurrentMillis();
            frames = 0;
            frameTimes = new List<long>();
            // Start the game running.
            /*            GameStateManager gameState = GameStateManager.getInstance();
                        Game runningGame = gameState.runningGame;
                        runningGame.initialize();
                        PhysicsManager phys = gameState.physicsManager;
                        GameObjectManager gameObjectManager = gameState.gameObjectManager;

                        */
            getRunningGame().initialize();

            timeInMillisecondsStart = startTime;
            lastTick = startTime;

            // Sätt denna när vi initialiserar running game
            GetPhysicsManager().GravityModifier = 0.3f;
            // This is our game loop.

            if (getEnvironmentalVariable("physics_debug") == "1")
            {
                physDebug = true;
            }
            int tickNumber = 0;


            //paneManager.SetPane(new MainMenuPane());

            while (true)
            {
                // we might've changed "game" (scene) so let's update our targetfps
                targetFrameRate = getRunningGame().getTargetFrameRate();
                frames += 1;

                timeInMillisecondsStart = getCurrentMillis();
                
                // Clear the screen.
                Bootstrap.getDisplay().clearDisplay();

                // Update 
                GameStateManager.getInstance().runningGame.update();
                // Input

                if (getRunningGame().isRunning() == true && 
                    GetGameObjectManager() != null && GetPhysicsManager() != null)
                {

                    // Get input, which works at 50 FPS to make sure it doesn't interfere with the 
                    // variable frame rates.
                    input.getInput();

                    // Update runs as fast as the system lets it.  Any kind of movement or counter 
                    // increment should be based then on the deltaTime variable.
                    GetGameObjectManager().update();

                    // This will update every 20 milliseconds or thereabouts.  Our physics system aims 
                    // at a 50 FPS cycle.
                    if (GetPhysicsManager().willTick())
                    {
                        GetGameObjectManager().prePhysicsUpdate();
                        tickNumber += 1;
                    }
                    // Update the physics.  If it's too soon, it'll return false.   Otherwise 
                    // it'll return true.
                    physUpdate = GetPhysicsManager().update();

                    if (physUpdate)
                    {
                        // If it did tick, give every object an update
                        // that is pinned to the timing of the physics system.
                        GetGameObjectManager().physicsUpdate();
                    }

                    if (physDebug) {
                        GetPhysicsManager().drawDebugColliders();
                    }
                }

                // Render the screen.
                Bootstrap.getDisplay().display();

                timeInMillisecondsEnd = getCurrentMillis();

                frameTimes.Add (timeInMillisecondsEnd);

                interval = timeInMillisecondsEnd - timeInMillisecondsStart;

                sleep = (int)(millisPerFrame - interval);

                TimeElapsed += deltaTime;

                if (sleep >= 0)
                {
                    // Frame rate regulator.  Bear in mind since this is millisecond precision, and we 
                    // only get whole numbers from our interval, it will only rarely match a target 
                    // FPS.  Milliseconds just aren't precise enough.
                    //
                    //  (I'm hinting if this bothers you, you might have found an engine modification to make...)
                    Thread.Sleep(sleep);
                }

                timeInMillisecondsEnd = getCurrentMillis();
                deltaTime = (timeInMillisecondsEnd - timeInMillisecondsStart) / 1000.0f;

                millisPerFrame = 1000 / targetFrameRate;

                lastTick = timeInMillisecondsStart;


            } 


        }
    }
}
