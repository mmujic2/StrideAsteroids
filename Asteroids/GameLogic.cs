using Stride.Audio;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class GameLogic : SyncScript
    {
        public static int numberOfLives;
        public static int score;
        public static int numberOfBombs;
        public static bool isGameOver;
       
        public static float mapSizeX;
        public static float mapSizeZ;

        public static Entity spaceShip;

        // The more aliens get spawned, the stronger they become
        public static int numberOfAliensSpawned;
        public static double alienShipSpawnMaxTimer;
        public static double alienShipSpawnTimer;
        public static int nextLiveScore;

        public override void Start()
        {
            spaceShip = spaceShip.Clone();

            isGameOver = false;
            score = 0;
            numberOfLives = 3;
            numberOfBombs = 3;

            mapSizeX = 5.0f;
            mapSizeZ = 5.0f;

            Entity.Scene.Entities.Add(spaceShip);

            alienShipSpawnMaxTimer = 15.0;
            alienShipSpawnTimer = 0.0;
            numberOfAliensSpawned = 0;

            nextLiveScore = 2500;

            UIScript.ScoreText.Text = "Score: " + score.ToString();
            UIScript.LivesCountText.Text = "x" + numberOfLives.ToString();
            UIScript.BombsCountText.Text = "x" + numberOfBombs.ToString();
        }

        public override void Update()
        {
            // Doesn't need to be called every frame, but it's a small game so performance isn't that important
            UIScript.ScoreText.Text = "Score: " + score.ToString();
            UIScript.LivesCountText.Text = "x" + numberOfLives.ToString();
            UIScript.BombsCountText.Text = "x" + numberOfBombs.ToString();

            // !isGameOver to avoid text change when game over
            if(!isGameOver && Input.IsKeyReleased(Stride.Input.Keys.Escape))
            {
                if(UIScript.GameOverPanel.Visibility != Visibility.Visible)
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
               
            if(score - nextLiveScore > 0)
            {
                numberOfLives++;
                nextLiveScore += 10000;
                Utils.PlaySound(SoundScript.ExtraLife);
            }
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
