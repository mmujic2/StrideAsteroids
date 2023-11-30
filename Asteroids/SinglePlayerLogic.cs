using Stride.Audio;
using Stride.Core.Mathematics;
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
    public class SinglePlayerLogic : CampaignModeLogic
    {
        public static int numberOfLives;
        public static int numberOfBombs;

        public static Entity spaceShip;

        // The more aliens get spawned, the stronger they become
        public static int nextLiveScore;

        public override void Start()
        {
            base.Start();
            spaceShip = spaceShip.Clone();

            numberOfLives = 3;
            numberOfBombs = 3;

            Entity.Scene.Entities.Add(spaceShip);

            nextLiveScore = 2500;

            UIScript.LivesCountText.Text = "x" + numberOfLives.ToString();
            UIScript.BombsCountText.Text = "x" + numberOfBombs.ToString();
        }

        public override void Update()
        {
            base.Update();

            // Doesn't need to be called every frame, but it's a small game so performance isn't that important
            UIScript.LivesCountText.Text = "x" + numberOfLives.ToString();
            UIScript.BombsCountText.Text = "x" + numberOfBombs.ToString();

            alienShipSpawnTimer += Game.UpdateTime.Elapsed.TotalSeconds;
            if (!isGameOver && alienShipSpawnTimer > alienShipSpawnMaxTimer)
            {
                alienShipSpawnTimer = 0;
                SpawnAlien();
            }
               
            if(score - nextLiveScore > 0)
            {
                numberOfLives++;
                nextLiveScore += 2500;
                Utils.PlaySound(SoundScript.ExtraLife);
            }
        }

        public override bool CheckIfShouldSpawnAsteroid(float posX, float posZ)
        {
            return Utils.CheckIfInRange2D(spaceShip.Transform.Position, new Vector3(posX, 0, posZ), 1.0f, 1.0f);
        }

    }
}
