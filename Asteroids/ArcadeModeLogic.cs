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
    // Global class that keeps track of misc stuff (points, bombs etc.)
    public class ArcadeModeLogic : GameLogic
    {
        public static int score;
        public static int numberOfLives;
        public static int numberOfBombs;

        public static Entity spaceShip;

        // The more aliens get spawned, the stronger they become
        public static int nextLiveScore;

        double countdownBeforeNextSpawn;
        double timer;
        int currentNumberOfAsteroids;

        public override void Start()
        {
            base.Start();
            currentNumberOfAsteroids = 3;
            countdownBeforeNextSpawn = 2.0;
            SpawnAsteroids();

            spaceShip = spaceShip.Clone();

            numberOfLives = 3;
            numberOfBombs = 3;

            Entity.Scene.Entities.Add(spaceShip);

            nextLiveScore = 2500;

            UIScript.LivesCountText.Text = "x" + numberOfLives.ToString();
            UIScript.BombsCountText.Text = "x" + numberOfBombs.ToString();

            score = 0;
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

            // Doesn't need to be called every frame, but it's a small game so performance isn't that important
            UIScript.LivesCountText.Text = "x" + numberOfLives.ToString();
            UIScript.BombsCountText.Text = "x" + numberOfBombs.ToString();

            // !isGameOver to avoid text change when game over
            if (!isGameOver && Input.IsKeyReleased(Stride.Input.Keys.Escape))
            {
                if (UIScript.GameOverPanel.Visibility != Visibility.Visible)
                    UIScript.GameOverPanel.Visibility = Visibility.Visible;
                else
                    UIScript.GameOverPanel.Visibility = Visibility.Collapsed;

                UIScript.GameOverTitle.Text = "";
                UIScript.GameOverInfo.Text = "";
                UIScript.HideGameOverButton.Visibility = Visibility.Visible;
            }

            alienShipSpawnTimer += Game.UpdateTime.Elapsed.TotalSeconds;
            if (!isGameOver && alienShipSpawnTimer > alienShipSpawnMaxTimer)
            {
                alienShipSpawnTimer = 0;
                SpawnAlien();
            }

            if (score - nextLiveScore > 0)
            {
                numberOfLives++;
                nextLiveScore += 2500;
                Utils.PlaySound(SoundScript.ExtraLife);
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
