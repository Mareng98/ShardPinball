using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard.Shard
{
    class GameStateManager
    {
        private static GameStateManager self;
        //public PhysicsManager physicsManager;
        //public GameObjectManager gameObjectManager;
        public Game runningGame;
        Game RunningGame { get => runningGame; set => runningGame = value; }
        //PhysicsManager PhysicsManager { get => physicsManager; set => physicsManager = value; }
        //GameObjectManager GameObjectManager { get => gameObjectManager; set => gameObjectManager = value; }

        public PhysicsManager GetPhysicsManager()
        {
            return runningGame.physicsManager;
        }

        public GameObjectManager GetGameObjectManager()
        {
            return runningGame.gameObjectManager;
        }

        private GameStateManager() { }

        public static GameStateManager getInstance()
        {
            if (self == null)
            {
                self = new GameStateManager();
            }

            return self;
        }

        public void SetGame(Game game)
        {
            runningGame = game;
            //gameObjectManager = game.gameObjectManager;
            //physicsManager = game.physicsManager;
        }
    }
}
