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
    public class MultiplayerCoopLogic : CampaignModeLogic
    {
        public static bool friendlyFire; // Controlled by UIScript
        public static int numberOfLives;
        // spaceship, numberOfProjectilesOnScreen
        public static List<Entity> SpaceShipSelection;
        public static Dictionary<Entity, int> playerInfo = new(); // Controlled by UIScript

        public override void Start()
        {
            base.Start();

            MultiplayerVSLogic.SpawnShips(SpaceShipSelection, Entity.Scene);

            foreach (var spaceShip in SpaceShipSelection)
                playerInfo.Add(spaceShip, 0);

            numberOfLives = 3;
            spawnAliens = true;
        }

        public override void Update()
        {
            base.Update();

            UIScript.LivesCountText.Text = "x" + numberOfLives.ToString();
        }

        public override void Cancel()
        {
            base.Cancel();

            foreach (var spaceShip in SpaceShipSelection)
            {
                if (spaceShip != null)
                {
                    spaceShip.Scene = null;
                    spaceShip.Dispose();
                }
            }

            playerInfo.Clear();

            for (int i = 0; i < SpaceShipSelection.Count; i++)
                SpaceShipSelection[i] = null;
        }

        public override bool CheckIfShouldSpawnAsteroid(float posX, float posZ)
        {
            // SpaceShip position hasn't changed yet, so get position directly from provider
            int i = 0;
            foreach(var spaceShip in SpaceShipSelection)
            {
                if (Utils.CheckIfInRange2D(MultiplayerVSLogic.GetShipPosition(i), new Vector3(posX, 0, posZ), 1.25f, 1.25f))
                    return true;

                i++;
            }
            return false;
        }
    }
}
