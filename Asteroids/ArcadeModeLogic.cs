using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class ArcadeModeLogic : GameLogic
    {
        public static Entity spaceShip;

        public static int nextLiveScore;

        double countdownBeforeNextSpawn;
        double timer;
        int currentNumberOfAsteroids;

        public override void Start()
        {
            base.Start();
            currentNumberOfAsteroids = 3;
            countdownBeforeNextSpawn = 2.0;
            SpawnAsteroids(currentNumberOfAsteroids);

            spaceShip = spaceShip.Clone();

            // Scrape these variables from other logics
            CampaignModeLogic.score = 0;
            SinglePlayerLogic.numberOfLives = 3;
            SinglePlayerLogic.numberOfBombs = 3;

            Entity.Scene.Entities.Add(spaceShip);

            nextLiveScore = 2500;

            UIScript.LivesCountText.Text = "x" + SinglePlayerLogic.numberOfLives.ToString();
            UIScript.BombsCountText.Text = "x" + SinglePlayerLogic.numberOfBombs.ToString();
            UIScript.ScoreText.Text = "Score: " + CampaignModeLogic.score.ToString();
        }

        public override void Update()
        {
            base.Update();

            UIScript.ScoreText.Text = "Score: " + CampaignModeLogic.score.ToString();

            if (MainScript.enemiesScene.Entities.Count == 0)
            {
                timer += Game.UpdateTime.Elapsed.TotalSeconds;
                if(timer >= countdownBeforeNextSpawn)
                {
                    timer = 0.0;
                    currentNumberOfAsteroids++;

                    // Strength of aliens is reset after every new asteroid cycle
                    numberOfAliensSpawned = 0;
                    SpawnAsteroids(currentNumberOfAsteroids);
                }
            }

            // Doesn't need to be called every frame, but it's a small game so performance isn't that important
            UIScript.LivesCountText.Text = "x" + SinglePlayerLogic.numberOfLives.ToString();
            UIScript.BombsCountText.Text = "x" + SinglePlayerLogic.numberOfBombs.ToString();

            alienShipSpawnTimer += Game.UpdateTime.Elapsed.TotalSeconds;
            if (!isGameOver && alienShipSpawnTimer > alienShipSpawnMaxTimer)
            {
                alienShipSpawnTimer = 0;
                SpawnAlien();
            }

            if (CampaignModeLogic.score - nextLiveScore > 0)
            {
                SinglePlayerLogic.numberOfLives++;
                nextLiveScore += 2500;
                Utils.PlaySound(SoundScript.ExtraLife);
            }
        }

        private void SpawnAsteroids(int numberToSpawn)
        {
            var rand = new Random();

            for (int i = 0; i < numberToSpawn; i++)
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
