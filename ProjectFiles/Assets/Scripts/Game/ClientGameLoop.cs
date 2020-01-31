﻿using Network;
using UnityEngine;

namespace Game
{
    public class ClientGameLoop : IGameLoop
    {
        private IClient client;
        private World world;
        private bool isStandalone;
        
        public bool Init(string[] args)
        {
            if (args.Length > 0)
            {
                isStandalone = args[0].Equals("standalone");
            }
            else
            {
                isStandalone = false;
            }

            // Create world
            world = new World();

            // Create HUD
            Object.Instantiate(Resources.Load<GameObject>("Canvas"), Vector3.zero, Quaternion.identity);

            
            // Start client connection
            if (isStandalone)
            {
                client = Client.getDummyClient(world);
            }
            else
            {
                client = new Client(world);
            }
#if UNITY_EDITOR
                var success = client.Start();
            #else
                var success = client.Start("18.191.231.10");
            #endif

            // Create HUD class
            
            // Subscribe HUD to client events.
            
            
            return success;
        }

        public void Shutdown()
        {
            client.Shutdown();
//          Destroy the world here.
        }

        public void Update()
        {
            client.HandleNetworkEvents();
            world.Update();
        }

        public void FixedUpdate()
        {
            if (world.ClientID >= 0)
            {
                client.SendLocationUpdate();
            }
        }

        public void LateUpdate()
        {
            // Nothing required here yet.
        }
    }
}
