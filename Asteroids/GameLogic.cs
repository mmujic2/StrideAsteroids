using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class GameLogic : SyncScript
    {
        public static bool isGameOver;

        public static float mapSizeX;
        public static float mapSizeZ;

        // The more aliens get spawned, the stronger they become
        public static int numberOfAliensSpawned;
        public static double alienShipSpawnMaxTimer;
        public static double alienShipSpawnTimer;

        public override void Update()
        {
            
        }

        public override void Start()
        {
            isGameOver = false;

            mapSizeX = 5.0f;
            mapSizeZ = 5.0f;

            alienShipSpawnMaxTimer = 15.0;
            alienShipSpawnTimer = 0.0;
            numberOfAliensSpawned = 0;
        }

        public void SpawnAlien()
        {
            var rand = new Random();

            float x;
            float z;
            if (rand.NextDouble() < 0.5f)
            {
                // Spawn alien ship from -x/+x side
                if (rand.NextDouble() < 0.5f)
                    x = -mapSizeX;
                else
                    x = mapSizeX;

                z = ((float)rand.NextDouble() - 0.5f) * mapSizeZ * 2.0f; // [-mapSizeZ, mapSizeZ]
            }
            else
            {
                // Spawn alien ship from -z/+z side
                if (rand.NextDouble() < 0.5f)
                    z = -mapSizeZ;
                else
                    z = mapSizeZ;

                x = ((float)rand.NextDouble() - 0.5f) * mapSizeX * 2.0f;
            }

            var alienShip = Content.Load<Prefab>("My Prefabs/AlienShip/alienShip").Instantiate().First().Clone();
            alienShip.Transform.Position.X = x;
            alienShip.Transform.Position.Z = z;
            alienShip.Get<AlienShip>().speed = 1.0f * Math.Max(numberOfAliensSpawned / 2.5f, 1.0f);
            MainScript.enemiesScene.Entities.Add(alienShip);

            numberOfAliensSpawned++;
        }
    }
}
