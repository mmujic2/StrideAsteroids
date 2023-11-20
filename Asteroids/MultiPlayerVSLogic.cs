using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class MultiplayerVSLogic : GameLogic
    {
        public static bool SpawnAsteroids; // Controlled by UI script
        // NumberOfLives/NumberOfProjedtiles on screen
        public static List<Entity> SpaceShipSelection = new List<Entity>(4);
        public static Dictionary<Entity, Tuple<int, int>> playerInfo = new(); // Controlled by UI script


        public override void Start()
        {
            base.Start();

            int i = 0;
            foreach(var spaceShip in SpaceShipSelection) {
                playerInfo.Add(spaceShip.Clone(), new(3, 0));
                var spaceShipClass = spaceShip.Get<MultiplayerVSSpaceShip>();
                Entity.Scene.Entities.Add(spaceShip);

                i++;
            }
        }

        private void GetControls(MultiplayerVSSpaceShip spaceShip, int index)
        {
            if(index == 0)
            {
                spaceShip.moveForward = Stride.Input.Keys.W;
                spaceShip.shootProjectile = Stride.Input.Keys.LeftCtrl;
                spaceShip.rotateLeft = Stride.Input.Keys.A;
                spaceShip.rotateRight = Stride.Input.Keys.D;
            }
            else if(index == 1)
            {
                spaceShip.moveForward = Stride.Input.Keys.Up;
                spaceShip.shootProjectile = Stride.Input.Keys.RightCtrl;
                spaceShip.rotateLeft = Stride.Input.Keys.Left;
                spaceShip.rotateRight = Stride.Input.Keys.Right;
            }
        }
    }
}
