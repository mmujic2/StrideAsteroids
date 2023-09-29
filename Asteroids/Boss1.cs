using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class Boss1 : Enemy
    {
        private Vector3 nextPosition;
        private Vector3 prevPosition;
        private float lerpValue;
        private float speed;

        public Entity rightParticle;
        public Entity leftParticle;
        public Entity topParticle;
        public Entity bottomParticle;
        private List<Entity> particles;

        public Prefab projectilePelletPrefab;
        private Entity projectilePellet;

        private double shootCooldown;
        private double shootTimer;

        private bool isDead;

        // Required so that the hp bar can be shown proprely
        private int maxHp;
        

        public override void Start()
        {
            particles = new List<Entity>() { rightParticle, leftParticle, topParticle, bottomParticle };
            prevPosition = new Vector3();
            nextPosition = new Vector3(GameLogic.mapSizeX - 0.75f, 0.0f, GameLogic.mapSizeZ - 0.75f);

            sizeX = 0.75f;
            sizeY = 0.75f;

            maxHp = 300;
            currentHp = maxHp;

            projectilePellet = projectilePelletPrefab.Instantiate().First();
        }

        public override void Update()
        {
            if(!isDead)
            {
                UIScript.BossHpDecorator.Width = UIScript.BossHpDecorator.Parent.ActualWidth * ((float)currentHp / maxHp);
                // Less hp means boss will shoot faster
                shootCooldown = Math.Max(0.25f, currentHp / 150.0f);

                shootTimer += Game.UpdateTime.Elapsed.TotalSeconds;
                if (shootTimer >= shootCooldown && !GameLogic.spaceShip.Get<Spaceship>().isDead)
                {
                    shootTimer = 0;
                    Shoot();
                }

                Move();

                if (currentHp <= 0)
                    Kill();
            }
        }

        public override void Kill()
        {
            isDead = true;
            GameLogic.isGameOver = true;

            if (CampaignModeLogic.bossMusic.PlayState == Stride.Media.PlayState.Playing)
                CampaignModeLogic.bossMusic.Stop();

            var countdown = new CountdownScript();
            countdown.countdownType = CountdownScript.CountdownType.BossDead;
            countdown.maxTimer = 5.0;
            MainScript.Camera.Add(countdown);
        }

        protected override void Move()
        {
            // Less hp means boss will move faster

            if (currentHp != 0) // avoid division by 0
                speed = Math.Min(7.5f, Math.Abs((maxHp - currentHp + 50) / 50));
            else
                speed = 10.0f;

            lerpValue += (float)Game.UpdateTime.Elapsed.TotalSeconds / Vector3.Distance(prevPosition, nextPosition) * speed;
            if (lerpValue > 1.0f)
            {
                Entity.Transform.Position = Vector3.Lerp(prevPosition, nextPosition, 1.0f);
                prevPosition = Entity.Transform.Position;

                foreach (var particle in particles)
                    particle.Transform.Scale *= 0.0f;

                // Move in a square around map
                if (nextPosition.X > 0 && nextPosition.Z > 0)
                {
                    nextPosition = new Vector3(GameLogic.mapSizeX - 0.5f, 0.0f, -GameLogic.mapSizeZ + 0.5f);
                    bottomParticle.Transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                else if (nextPosition.X > 0 && nextPosition.Z < 0)
                {
                    nextPosition = new Vector3(-GameLogic.mapSizeX + 0.5f, 0.0f, -GameLogic.mapSizeZ + 0.5f);
                    rightParticle.Transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                else if (nextPosition.X < 0 && nextPosition.Z < 0)
                {
                    nextPosition = new Vector3(-GameLogic.mapSizeX + 0.5f, 0.0f, GameLogic.mapSizeZ - 0.5f);
                    topParticle.Transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                else if (nextPosition.X < 0 && nextPosition.Z > 0)
                {
                    nextPosition = new Vector3(GameLogic.mapSizeX - 0.5f, 0.0f, GameLogic.mapSizeZ - 0.5f);
                    leftParticle.Transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);
                }

                lerpValue = 0;
            }
            else
            {
                Entity.Transform.Position = Vector3.Lerp(prevPosition, nextPosition, lerpValue);
            }
        }

        protected void Shoot()
        {
            var direction = GameLogic.spaceShip.Transform.Position.XZ() - Entity.Transform.Position.XZ();
            direction.Normalize();

            var newProjectile = projectilePellet.Clone();
            newProjectile.Transform.Position = Entity.Transform.Position;

            var newProjectileClass = newProjectile.Get<EnemyProjectile>();
            newProjectileClass.speedX = direction.X * 2 * Math.Min(speed, 2.5f);
            newProjectileClass.speedZ = direction.Y * 2 * Math.Min(speed, 2.5f);
            newProjectileClass.enemySpeed = speed;

            // Increase projectile time
            newProjectileClass.aliveTimeMax = 7.5;

            MainScript.enemiesScene.Entities.Add(newProjectile);
        }
    }
}
