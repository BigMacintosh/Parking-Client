using System;
using System.Collections.Generic;
using Gameplay;
using Network;
using UnityEngine;
using Utils;


namespace Game
{
    static class DefaultRoundProperties
    {
        // All times in seconds
        public const ushort FreeroamLength = 10;
        public const ushort PreRoundLength = 5;
        public const ushort RoundLength = 120;
        public const ushort MaxRounds = 5;
    }

    
    
    public class RoundManager
    {
        public bool GameInProgress { get; private set; }
        private World world;
        private ushort roundNumber = 0;

        // Timer to countdown to the start of the round.
        private Timer roundTimer;
        private ushort freeroamLength;
        private ushort preRoundLength;
        private ushort roundLength;
        private ushort maxRounds;
        private ServerParkingSpaceManager spaceManager;
        private System.Random random;

        // Spawn all the players that have connected. Allow free-roam for the players. Disallow new connections.
        public event GameStartDelegate GameStartEvent;

        // Start a countdown until the beginning of a new round. Provides all the initial info for a round.
        public event PreRoundStartDelegate PreRoundStartEvent;

        // Immediately start a round.
        public event RoundStartDelegate RoundStartEvent;

        // Immediately end a around.
        public event RoundEndDelegate RoundEndEvent;
        
        public event EliminatePlayersDelegate EliminatePlayersEvent;
        
        public event GameEndDelegate GameEndEvent;

        public RoundManager(World world, ServerParkingSpaceManager spaceManager)
        {
            this.world = world;
            this.spaceManager = spaceManager;
            this.random = new System.Random();
        }

        public void Update()
        {
            if (GameInProgress)
            {
                roundTimer.Update();
            }
        }

        public void StartGame()
        {
            preRoundLength = DefaultRoundProperties.PreRoundLength;
            roundLength = DefaultRoundProperties.RoundLength;
            freeroamLength = DefaultRoundProperties.FreeroamLength;
            maxRounds = DefaultRoundProperties.MaxRounds;
            NotifyGameStart(freeroamLength);
            StartFreeroam();
            GameInProgress = true;
        }

        public void StartFreeroam()
        {
            // Start timer to for PreRoundCountdown 
            roundTimer = new Timer(freeroamLength);

            // Add StartRoundEvent to timer observers.
            roundTimer.Elapsed += StartPreRound;
            roundTimer.Start();
        }

        public void StartPreRound()
        {
            // TODO: give an actual dynamic value to num space
            Vector2 spacesAround = new Vector2(random.Next(-200, 201), random.Next(-200, 201));
            List<ushort> activeSpaces = spaceManager.GetNearestSpaces(spacesAround, 3);
            Debug.Log($"Round { roundNumber } spaces from point ({ spacesAround.x }, { spacesAround.y }): { String.Join(", ", activeSpaces) }.");

            // Send 5 seconds round warning.
            NotifyPreRoundStart(activeSpaces);

            // Start timer to for PreRoundCountdown 
            roundTimer = new Timer(preRoundLength);

            // Add StartRoundEvent to timer observers.
            roundTimer.Elapsed += StartRoundEvent;
            roundTimer.Start();
        }

        private void StartRoundEvent()
        {
            NotifyRoundStart();

            roundTimer = new Timer(roundLength);
            roundTimer.Elapsed += EndRoundEvent;
            roundTimer.Start();
        }

        private void EndRoundEvent()
        {
            NotifyRoundEnd();
//            List<ushort> eliminatedPlayers = world.GetPlayersNotInSpace();
//            NotifyEliminatePlayers(eliminatedPlayers);
            roundNumber++;
            if (roundNumber < maxRounds)
            {
                StartPreRound();
            }
            else
            {
                GameEndEvent?.Invoke();
            }
        }

        private void NotifyGameStart(ushort freeRoamLength)
        {
            GameStartEvent?.Invoke(freeRoamLength, world.GetNumPlayers());
        }

        private void NotifyPreRoundStart(List<ushort> spacesActive)
        {
            PreRoundStartEvent?.Invoke(roundNumber, preRoundLength, roundLength, world.GetNumPlayers(), spacesActive);
        }

        private void NotifyRoundStart()
        {
            RoundStartEvent?.Invoke(roundNumber);
        }

        private void NotifyRoundEnd()
        {
            RoundEndEvent?.Invoke(roundNumber);
        }

        private void NotifyEliminatePlayers(List<int> eliminatedPlayers)
        {
            EliminatePlayersEvent?.Invoke(roundNumber, eliminatedPlayers);
        }
    }
}