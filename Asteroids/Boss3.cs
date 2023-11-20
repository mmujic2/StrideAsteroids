using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;

namespace Asteroids
{
    public class Boss3 : Enemy
    {

        private double shootCooldown;
        private double shootTimer;
        bool isDead;

        public Entity TopPart;
        public Entity BottomPart;

        public Prefab enemyMiniShipPrefab;
        private Entity enemyMiniShip;
        private int numberOfProjectilesFired;

        private int maxHp;

        public override void Start()
        {

            sizeX = 0.65f;
            sizeY = 0.65f;

            maxHp = 1000;
            currentHp = maxHp;

            enemyMiniShip = enemyMiniShipPrefab.Instantiate().First();

            // Move down since it's in the centre of the map, and when player dies view would be blocked
            Entity.Transform.Position.Y -= 0.5f;

            enemyMiniShip = enemyMiniShipPrefab.Instantiate().First();
            numberOfProjectilesFired = 0;
        }

        public override void Update()
        {
            if (!isDead)
            {
                UIScript.BossHpDecorator.Width = UIScript.BossHpDecorator.Parent.ActualWidth * ((float)currentHp / maxHp);
                // Less hp means boss will shoot faster
                shootCooldown = Math.Max(0.75f, (currentHp + 250.0f) / 250.0f);

                shootTimer += Game.UpdateTime.Elapsed.TotalSeconds;
                if (shootTimer >= shootCooldown && !SinglePlayerLogic.spaceShip.Get<Spaceship>().isDead)
                {
                    shootTimer = 0;
                    Shoot();
                }

                Move();

                if (currentHp <= 0)
                    Kill();
            }
        }

        protected override void Move()
        {
            var sizeIncrease = (float)Game.UpdateTime.Elapsed.TotalSeconds / 10.0f;

            Entity.Transform.Scale += sizeIncrease;

            // don't scale on Y axis to not block view
            Entity.Transform.Scale.Y = 1.0f;
            sizeX += sizeIncrease / 2;
            sizeY += sizeIncrease / 2;

            TopPart.Transform.Rotation *= Quaternion.RotationY((float)Game.UpdateTime.Elapsed.TotalSeconds);
            BottomPart.Transform.Rotation *= Quaternion.RotationY(-(float)Game.UpdateTime.Elapsed.TotalSeconds);
        }

        private void Shoot()
        {
            var projectile = enemyMiniShip.Clone();
            projectile.Transform.Position = GetProjectilePosition();

            var direction = GetProjectileDirection();
            var enemyClass = projectile.Get<MiniShip>();
            enemyClass.speedX = direction.X;
            enemyClass.sizeY = direction.Y;
            enemyClass.speedMultiplier = Math.Max(1.0f, (maxHp - currentHp) / 200.0f);

            MainScript.enemiesScene.Entities.Add(projectile);

            numberOfProjectilesFired++;
        }

        public override void Kill()
        {
            isDead = true;
            SinglePlayerLogic.isGameOver = true;

            if (CampaignModeLogic.bossMusic.PlayState == Stride.Media.PlayState.Playing)
                CampaignModeLogic.bossMusic.Stop();

            var countdown = new CountdownScript();
            countdown.countdownType = CountdownScript.CountdownType.BossDead;
            countdown.maxTimer = 5.0;
            MainScript.Camera.Add(countdown);
        }

        private Vector3 GetProjectilePosition()
        {
            switch(numberOfProjectilesFired % 8)
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
            switch (numberOfProjectilesFired % 8)
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
    }
}
