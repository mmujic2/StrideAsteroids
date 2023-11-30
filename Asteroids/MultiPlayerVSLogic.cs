using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Rendering;
using Stride.Rendering.Materials;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class MultiplayerVSLogic : GameLogic
    {
        public static int currentNumberOfAsteroids;
        public static bool spawnAsteroids; // Controlled by UI script
        // NumberOfLives/NumberOfProjedtiles on screen
        public static List<Entity> SpaceShipSelection;
        public static Dictionary<Entity, Tuple<int, int>> playerInfo = new(); // Controlled by UI script

        public override void Start()
        {
            base.Start();
            SpawnShips(SpaceShipSelection, Entity.Scene);

            foreach (var spaceShip in SpaceShipSelection)
                if(spaceShip != null)
                    playerInfo.Add(spaceShip, new(3, 0));

            currentNumberOfAsteroids = 3;
            if (spawnAsteroids)
                SpawnAsteroids(currentNumberOfAsteroids);

            spawnAliens = false;
        }

        public static void SpawnShips(List<Entity> spaceShipSelection, Scene scene)
        {
            for (int i = 0; i < spaceShipSelection.Count; i++)
            {
                var spaceShip = spaceShipSelection[i];

                if (spaceShip != null)
                {
                    var spaceShipClone = spaceShip.Clone();
                    spaceShipClone.Transform.Position = GetShipPosition(i);
                    spaceShipClone.Transform.Rotation = GetShipRotation(i);

                    var spaceShipClass = spaceShipClone.Get<Spaceship>();
                    GetControls(spaceShipClass, i);
                    GetShipColor(i, spaceShipClone.GetChildren().First().Get<ModelComponent>());

                    spaceShipSelection[i] = spaceShipClone;
                }
            }

            foreach (var spaceShip in spaceShipSelection)
                if (spaceShip != null)
                    scene.Entities.Add(spaceShip);
        }

        public override void Update()
        {
            base.Update();

            if(spawnAsteroids && MainScript.enemiesScene.Entities.Count == 0)
            {
                currentNumberOfAsteroids++;
                SpawnAsteroids(currentNumberOfAsteroids);
            }

            for(int i = 0; i < SpaceShipSelection.Count; i++)
                if(SpaceShipSelection[i] != null)
                    UIScript.PlayerInfos[i].FindVisualChildOfType<TextBlock>().Text = "x" + playerInfo[SpaceShipSelection[i]].Item1.ToString();
        }

        public override void Cancel()
        {
            foreach (var spaceShip in SpaceShipSelection)
            {
                if(spaceShip != null)
                {
                    spaceShip.Scene = null;
                    spaceShip.Dispose();
                }
            }

            playerInfo.Clear();

            for (int i = 0; i < SpaceShipSelection.Count; i++)
                SpaceShipSelection[i] = null;
        }

        public static void GetControls(Spaceship spaceShip, int index)
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
            else if(index == 2)
            {
                spaceShip.moveForward = Stride.Input.Keys.U;
                spaceShip.shootProjectile = Stride.Input.Keys.V;
                spaceShip.rotateLeft = Stride.Input.Keys.H;
                spaceShip.rotateRight = Stride.Input.Keys.K;
            }
            else if(index == 3)
            {
                spaceShip.moveForward = Stride.Input.Keys.NumPad8;
                spaceShip.shootProjectile = Stride.Input.Keys.NumPad0;
                spaceShip.rotateLeft = Stride.Input.Keys.NumPad4;
                spaceShip.rotateRight = Stride.Input.Keys.NumPad6;
            }
        }

        public static Vector3 GetShipPosition(int index)
        {
            if (index == 0) return new Vector3(2.0f, 0.0f, 0.0f);
            else if (index == 1) return new Vector3(-2.0f, 0.0f, 0.0f);
            else if (index == 2) return new Vector3(0.0f, 0.0f, 2.0f);
            else if (index == 3) return new Vector3(0.0f, 0.0f, -2.0f);
            else  return new Vector3(0.0f, 0.0f, 0.0f);
        }

        public static Quaternion GetShipRotation(int index)
        {
            if (index == 0) return Quaternion.RotationY((float) Math.PI / 2.0f);
            else if (index == 1) return Quaternion.RotationY((float)Math.PI / -2.0f);
            else if (index == 2) return Quaternion.RotationY((float)Math.PI);
            else if (index == 3) return Quaternion.RotationY(0.0f);
            else return Quaternion.RotationY(0.0f);
        }

        private static void GetShipColor(int index, ModelComponent model)
        {
            model.Model.Materials[0].Material = Utils.CopyMaterial(model.Model.Materials[0].Material, new Material());
            model.Materials[0] = Utils.CopyMaterial(model.Model.Materials[0].Material, new Material());

            model.GetMaterial(0).Passes[0].Parameters.Set(MaterialKeys.EmissiveIntensity, 0.1f);
            model.Materials[0].Passes[0].Parameters.Set(MaterialKeys.EmissiveIntensity, 0.1f);
            // model.GetMaterial(0).Passes[0].Parameters.Set(MaterialKeys.EmissiveValue, new Color4(0, 1, 0, 1));
            if (index == 0)
            {
                model.GetMaterial(0).Passes[0].Parameters.Set(MaterialKeys.EmissiveValue, new Color4(1, 0, 0, 1));
                model.Materials[0].Passes[0].Parameters.Set(MaterialKeys.EmissiveValue, new Color4(1, 0, 0, 1));
            } 
            else if(index == 1)
            {
                model.GetMaterial(0).Passes[0].Parameters.Set(MaterialKeys.EmissiveValue, new Color4(0, 1, 0, 1));
                model.Materials[0].Passes[0].Parameters.Set(MaterialKeys.EmissiveValue, new Color4(0, 1, 0, 1));
            }
            else if(index == 2)
            {
                model.GetMaterial(0).Passes[0].Parameters.Set(MaterialKeys.EmissiveValue, new Color4(0, 0, 1, 1));
                model.Materials[0].Passes[0].Parameters.Set(MaterialKeys.EmissiveValue, new Color4(0, 0, 1, 1));
            }
                
            else if(index == 3)
            {
                model.GetMaterial(0).Passes[0].Parameters.Set(MaterialKeys.EmissiveValue, new Color4(1, 1, 0, 1));
                model.Materials[0].Passes[0].Parameters.Set(MaterialKeys.EmissiveValue, new Color4(1, 1, 0, 1));
            } 
        }

        private void SpawnAsteroids(int numberToSpawn)
        {
            var rand = new Random();

            for (int i = 0; i < numberToSpawn; i++)
            {
                var x = ((float)rand.NextDouble() - 0.5f) * mapSizeX * 2.0f; // generates number in range [-mapSizex, mapSizex]
                var z = ((float)rand.NextDouble() - 0.5f) * mapSizeZ * 2.0f;


                /*if (Utils.CheckIfInRange2D(spaceShip.Transform.Position, new Vector3(x, 0, z), 1.0f, 1.0f))
                {
                    i--;
                    continue;
                }*/

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
