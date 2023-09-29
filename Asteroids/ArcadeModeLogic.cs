using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    // Global class that keeps track of misc stuff (points, bombs etc.)
    public class ArcadeModeLogic : GameLogic
    {
        double countdownBeforeNextSpawn;
        double timer;
        int currentNumberOfAsteroids;

        public override void Start()
        {
            base.Start();
            currentNumberOfAsteroids = 3;
            countdownBeforeNextSpawn = 2.0;
            SpawnAsteroids();
        }

        public override void Update()
        {
            base.Update();

            if (MainScript.enemiesScene.Entities.Count == 0)
            {
                timer += Game.UpdateTime.Elapsed.TotalSeconds;
                if(timer >= countdownBeforeNextSpawn)
                {
                    timer = 0.0;
                    currentNumberOfAsteroids++;

                    // Strength of aliens is reset after every new asteroid cycle
                    numberOfAliensSpawned = 0;
                    SpawnAsteroids();
                }
            }

        }

        private void SpawnAsteroids()
        {
            var rand = new Random();

            for (int i = 0; i < currentNumberOfAsteroids; i++)
            {
                var x = ((float)rand.NextDouble() - 0.5f) * mapSizeX * 2.0f; // generates number in range [-mapSizex, mapSizex]
                var z = ((float)rand.NextDouble() - 0.5f) * mapSizeZ * 2.0f;

                // avoid spawning asteroid close to player
                if (Utils.CheckIfInRange2D(spaceShip.Transform.Position, new Vector3(x, 0, z), 1.0f, 1.0f))
                {
                    i--;
                    continue;
                }

                var newAsteroid = Asteroid.BigAsteroids[rand.Next(0, Asteroid.BigAsteroids.Count)].Clone();
                newAsteroid.Transform.Position.X = x;
                newAsteroid.Transform.Position.Z = z;

                var newAsteroidClass = newAsteroid.Get<Asteroid>();
                newAsteroidClass.speedX = (float)rand.NextDouble();
                newAsteroidClass.speedZ = (float)rand.NextDouble();

                MainScript.enemiesScene.Entities.Add(newAsteroid);
            }
        }
    }
}
