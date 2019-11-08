using System.Collections.Generic;
using Boo.Lang.Environments;
using Unity.Collections;
using UnityEngine;

namespace Game
{
    public class World
    {
        public List<DriveController> Cars { get; private set; }
        private Spawner spawner;
        
        public World(Spawner spawner)
        {
            this.spawner = spawner;
            Cars = new List<DriveController>();
        }

        public void SpawnCar(DriveController prefab, Vector3 position)
        {
            var newCar = spawner.spawn(prefab, position, Quaternion.identity);
            Cars.Add(newCar);
        }
    }
}