using Stride.Audio;
using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public abstract class CampaignModeLogic : GameLogic
    {
        public static bool isBossStarted;
        public static Entity Boss;
        // Sounds are all over the place because creating centralized sound management is too much effort
        public static SoundInstance bossStartSound;
        public static SoundInstance bossMusic;

        private static int numberOfDronesSpawned;
        private static double droneTimer;
        public static int score;

        public override void Cancel()
        {
            if (bossMusic.PlayState == Stride.Media.PlayState.Playing)
                bossMusic.Stop();
        }

        public override void Start()
        {
            base.Start();
            bossStartSound = SoundScript.BossStart.CreateInstance();
            isBossStarted = false;
            Boss = Boss.Clone();

            SpawnAsteroids();

            bossMusic = SoundScript.BossMusic.CreateInstance();
            bossMusic.IsLooping = true;
            bossMusic.Volume = 0.5f;

            numberOfDronesSpawned = 0;

            score = 0;

            UIScript.ScoreText.Text = "Score: " + score.ToString();
        }

        public override void Update()
        {
            base.Update();

            UIScript.ScoreText.Text = "Score: " + score.ToString();

            // Only one boss per map
            if (!isBossStarted && MainScript.enemiesScene.Entities.Count == 0)
            {
                isBossStarted = true;
                bossStartSound.Play();
                var countdown = new CountdownScript();
                countdown.countdownType = CountdownScript.CountdownType.BossStart;
                countdown.maxTimer = 6.0;
                Entity.Add(countdown);
            }

            // For stage 4 additionally spawn small drone enemies
            if (MainScript.stage == MainScript.Stage.stage4)
            {
                if (!isBossStarted)
                {

                    droneTimer += Game.UpdateTime.Elapsed.TotalSeconds;
                    if (droneTimer > 2.5)
                    {
                        droneTimer = 0;
                        SpawnDrone();
                    }

                    // if all asteroids and alienships are dead, kill drones
                    bool foundNonDrone = false;
                    foreach (var enemy in MainScript.enemiesScene.Entities)
                    {
                        if (enemy.Get<MiniShip>() == null)
                        {
                            foundNonDrone = true;
                            break;
                        }
                    }

                    if (!foundNonDrone)
                    {
                        foreach (var enemy in MainScript.enemiesScene.Entities)
                        {
                            var droneClass = enemy.Get<MiniShip>();
                            if (droneClass != null) droneClass.Kill();
                        }
                    }
                }
            }
        }

        private void SpawnAsteroids()
        {
            // Spawns a bunch of asteroids manually for each stage

            var rand = new Random();
            if (MainScript.stage == MainScript.Stage.stage1)
            {
                // 10 small
                for(int i = 0; i < 10; i++)
                {
                    var x = ((float)rand.NextDouble() - 0.5f) * mapSizeX * 2.0f; // generates number in range [-mapSizex, mapSizex]
                    var z = ((float)rand.NextDouble() - 0.5f) * mapSizeZ * 2.0f;

                    // avoid spawning asteroid close to player
                    if (CheckIfShouldSpawnAsteroid(x, z))
                    {
                        i--;
                        continue;
                    }

                    var newAsteroid = Asteroid.SmallAsteroids[rand.Next(0, Asteroid.BigAsteroids.Count)].Clone();
                    newAsteroid.Transform.Position.X = x;
                    newAsteroid.Transform.Position.Z = z;

                    var newAsteroidClass = newAsteroid.Get<Asteroid>();
                    newAsteroidClass.speedX = (float)rand.NextDouble() * 2.0f;
                    newAsteroidClass.speedZ = (float)rand.NextDouble() * 2.0f;

                    MainScript.enemiesScene.Entities.Add(newAsteroid);
                }
            }
            else if (MainScript.stage == MainScript.Stage.stage2)
            {
                // 5 small and 7 medium
                for (int i = 0; i < 5; i++)
                {
                    var x = ((float)rand.NextDouble() - 0.5f) * mapSizeX * 2.0f; // generates number in range [-mapSizex, mapSizex]
                    var z = ((float)rand.NextDouble() - 0.5f) * mapSizeZ * 2.0f;

                    // avoid spawning asteroid close to player
                    if (CheckIfShouldSpawnAsteroid(x, z))
                    {
                        i--;
                        continue;
                    }

                    var newAsteroid = Asteroid.SmallAsteroids[rand.Next(0, Asteroid.BigAsteroids.Count)].Clone();
                    newAsteroid.Transform.Position.X = x;
                    newAsteroid.Transform.Position.Z = z;

                    var newAsteroidClass = newAsteroid.Get<Asteroid>();
                    newAsteroidClass.speedX = (float)rand.NextDouble() * 2.0f;
                    newAsteroidClass.speedZ = (float)rand.NextDouble() * 2.0f;

                    MainScript.enemiesScene.Entities.Add(newAsteroid);
                }

                for (int i = 0; i < 7; i++)
                {
                    var x = ((float)rand.NextDouble() - 0.5f) * mapSizeX * 2.0f; // generates number in range [-mapSizex, mapSizex]
                    var z = ((float)rand.NextDouble() - 0.5f) * mapSizeZ * 2.0f;

                    // avoid spawning asteroid close to player
                    if (CheckIfShouldSpawnAsteroid(x, z))
                    {
                        i--;
                        continue;
                    }

                    var newAsteroid = Asteroid.MediumAsteroids[rand.Next(0, Asteroid.BigAsteroids.Count)].Clone();
                    newAsteroid.Transform.Position.X = x;
                    newAsteroid.Transform.Position.Z = z;

                    var newAsteroidClass = newAsteroid.Get<Asteroid>();
                    newAsteroidClass.speedX = (float)rand.NextDouble() * 2.0f;
                    newAsteroidClass.speedZ = (float)rand.NextDouble() * 2.0f;

                    MainScript.enemiesScene.Entities.Add(newAsteroid);
                }
            }
            else if (MainScript.stage == MainScript.Stage.stage3)
            {
                // 3 small, 5 medium, 3 large
                for (int i = 0; i < 3; i++)
                {
                    var x = ((float)rand.NextDouble() - 0.5f) * mapSizeX * 2.0f; // generates number in range [-mapSizex, mapSizex]
                    var z = ((float)rand.NextDouble() - 0.5f) * mapSizeZ * 2.0f;

                    // avoid spawning asteroid close to player
                    if (CheckIfShouldSpawnAsteroid(x, z))
                    {
                        i--;
                        continue;
                    }

                    var newAsteroid = Asteroid.SmallAsteroids[rand.Next(0, Asteroid.BigAsteroids.Count)].Clone();
                    newAsteroid.Transform.Position.X = x;
                    newAsteroid.Transform.Position.Z = z;

                    var newAsteroidClass = newAsteroid.Get<Asteroid>();
                    newAsteroidClass.speedX = (float)rand.NextDouble() * 2.0f;
                    newAsteroidClass.speedZ = (float)rand.NextDouble() * 2.0f;

                    MainScript.enemiesScene.Entities.Add(newAsteroid);
                }

                for (int i = 0; i < 5; i++)
                {
                    var x = ((float)rand.NextDouble() - 0.5f) * mapSizeX * 2.0f; // generates number in range [-mapSizex, mapSizex]
                    var z = ((float)rand.NextDouble() - 0.5f) * mapSizeZ * 2.0f;

                    // avoid spawning asteroid close to player
                    if (CheckIfShouldSpawnAsteroid(x, z))
                    {
                        i--;
                        continue;
                    }

                    var newAsteroid = Asteroid.MediumAsteroids[rand.Next(0, Asteroid.BigAsteroids.Count)].Clone();
                    newAsteroid.Transform.Position.X = x;
                    newAsteroid.Transform.Position.Z = z;

                    var newAsteroidClass = newAsteroid.Get<Asteroid>();
                    newAsteroidClass.speedX = (float)rand.NextDouble() * 1.5f;
                    newAsteroidClass.speedZ = (float)rand.NextDouble() * 1.5f;

                    MainScript.enemiesScene.Entities.Add(newAsteroid);
                }

                for (int i = 0; i < 3; i++)
                {
                    var x = ((float)rand.NextDouble() - 0.5f) * mapSizeX * 2.0f; // generates number in range [-mapSizex, mapSizex]
                    var z = ((float)rand.NextDouble() - 0.5f) * mapSizeZ * 2.0f;

                    // avoid spawning asteroid close to player
                    if (CheckIfShouldSpawnAsteroid(x, z))
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
            else if (MainScript.stage == MainScript.Stage.stage4)
            {
                // 5 medium, 9 large
                for (int i = 0; i < 5; i++)
                {
                    var x = ((float)rand.NextDouble() - 0.5f) * mapSizeX * 2.0f; // generates number in range [-mapSizex, mapSizex]
                    var z = ((float)rand.NextDouble() - 0.5f) * mapSizeZ * 2.0f;

                    // avoid spawning asteroid close to player
                    if (CheckIfShouldSpawnAsteroid(x, z))
                    {
                        i--;
                        continue;
                    }

                    var newAsteroid = Asteroid.MediumAsteroids[rand.Next(0, Asteroid.BigAsteroids.Count)].Clone();
                    newAsteroid.Transform.Position.X = x;
                    newAsteroid.Transform.Position.Z = z;

                    var newAsteroidClass = newAsteroid.Get<Asteroid>();
                    newAsteroidClass.speedX = (float)rand.NextDouble() * 2.0f;
                    newAsteroidClass.speedZ = (float)rand.NextDouble() * 2.0f;

                    MainScript.enemiesScene.Entities.Add(newAsteroid);
                }

                for (int i = 0; i < 9; i++)
                {
                    var x = ((float)rand.NextDouble() - 0.5f) * mapSizeX * 2.0f; // generates number in range [-mapSizex, mapSizex]
                    var z = ((float)rand.NextDouble() - 0.5f) * mapSizeZ * 2.0f;

                    // avoid spawning asteroid close to player
                    if (CheckIfShouldSpawnAsteroid(x, z))
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

        // Drone spawsning code below (copied from boss3 class)

        private void SpawnDrone()
        {
            var projectile = MiniShip.miniShip.Clone();
            projectile.Transform.Position = GetProjectilePosition();

            var direction = GetProjectileDirection();
            var enemyClass = projectile.Get<MiniShip>();
            enemyClass.speedX = direction.X;
            enemyClass.sizeY = direction.Y;
            // enemyClass.speedMultiplier = Math.Max(1.0f, (maxHp - currentHp) / 200.0f);
            // less asteroids == drones are faster => 5.0f / [1.0f-5.0f] = [5.0f-1.0f]
            enemyClass.speedMultiplier = 5.0f / (Math.Min(17, MainScript.enemiesScene.Entities.Count) - 1) / 4.0f + 1.0f;

            MainScript.enemiesScene.Entities.Add(projectile);

            numberOfDronesSpawned++;
        }

        private Vector3 GetProjectilePosition()
        {
            switch (numberOfDronesSpawned % 8)
            {
                case 0: return new Vector3(-SinglePlayerLogic.mapSizeX, 0f, 0f);                    // Left
                case 1: return new Vector3(-SinglePlayerLogic.mapSizeX, 0f, -SinglePlayerLogic.mapSizeZ);   // Top left
                case 2: return new Vector3(0f, 0f, -SinglePlayerLogic.mapSizeZ);                    // Top
                case 3: return new Vector3(SinglePlayerLogic.mapSizeX, 0f, -SinglePlayerLogic.mapSizeZ);    // Top right
                case 4: return new Vector3(SinglePlayerLogic.mapSizeX, 0f, 0f);                     // Right
                case 5: return new Vector3(SinglePlayerLogic.mapSizeX, 0f, SinglePlayerLogic.mapSizeZ);     // Bottom right (don't ask why +z is bottom and -z is top)
                case 6: return new Vector3(0f, 0f, SinglePlayerLogic.mapSizeZ);                     // Bottom
                case 7: return new Vector3(-SinglePlayerLogic.mapSizeX, 0f, SinglePlayerLogic.mapSizeZ);    // Bottom left
                default: return new Vector3();
            }
        }

        private Vector3 GetProjectileDirection()
        {
            switch (numberOfDronesSpawned % 8)
            {
                case 0: return new Vector3(1.0f, 0f, 0f); // Spawns left so it moves right logic, same for all others
                case 1: return new Vector3(0.707f, 0, 0.707f); // Normaliized 45 degree vector
                case 2: return new Vector3(0f, 0f, 1.0f);
                case 3: return new Vector3(-0.707f, 0, 0.707f);
                case 4: return new Vector3(-1.0f, 0f, 0f);
                case 5: return new Vector3(-0.707f, 0, -0.707f);
                case 6: return new Vector3(0f, 0f, -1.0f);
                case 7: return new Vector3(0.707f, 0, -0.707f);
                default: return new Vector3();
            }
        }

        public abstract bool CheckIfShouldSpawnAsteroid(float posX, float posZ);
    }
}
