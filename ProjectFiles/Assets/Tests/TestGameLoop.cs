using Game.Core.Parking;
using Game.Core.Rounds;
using Game.Entity;
using Game.Main;
using Network;
using NUnit.Framework;
using UI;
using UnityEngine;
using Utils;
using Moq;

namespace Tests {
    public class TestGameLoop : IGameLoop {
        // Fields
        private ServerParkingSpaceManager parkingSpaceManager;
        private RoundManager              roundManager;
        private ServerWorld               world;

        private Timer timer;
        public bool TestFinished;

        public bool Init(string[] args) {
            timer = new Timer(0);
            
            TestFinished = false;

            // Initialise Gameplay components
            parkingSpaceManager = new ServerParkingSpaceManager();
            
//            parkingSpaceManager.TEST_ONLY_AddParkingSpace(new Mock);
            
            world               = new ServerWorld(parkingSpaceManager);
            roundManager        = new RoundManager(world, parkingSpaceManager);

            world.CreatePlayer(0, new PlayerOptions());
            world.CreatePlayer(1, new PlayerOptions());
            world.CreatePlayer(2, new PlayerOptions());

            roundManager.GameStartEvent += (length, players) => world.SpawnPlayers();
            roundManager.GameEndEvent += winners => {
                
                Assert.True(winners.Count == 2);
                Assert.True(winners.Contains(1));
                Assert.True(winners.Contains(2));
                
                winners.ForEach(world.DestroyPlayer);
                TestFinished = true;
            };


            roundManager.RoundStartEvent    += parkingSpaceManager.OnRoundStart;
            roundManager.PreRoundStartEvent += parkingSpaceManager.OnPreRoundStart;

            roundManager.RoundStartEvent += (number, active) => {
                var t = new Timer(1);
                t.Elapsed += () => parkingSpaceManager.OnSpaceEnter(0, active[0]);
                t.Start();
            };

            roundManager.StartGame();
            return true;
        }

        public void Shutdown() { }

        public void Update() {
            roundManager.Update();
            timer.Update();
        }

        public void FixedUpdate() { }

        public void LateUpdate() { }
    }
}