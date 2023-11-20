using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class Boss2 : Enemy
    {
        // A lot of stuff is very similar to Boss1, most notable changes are in Shoot() and Move() methods
        private Vector3 nextPosition;
        private Vector3 prevPosition;
        private float lerpValue;
        private float speed;

        public Entity rightParticle;
        public Entity leftParticle;
        public Entity top1Particle;
        public Entity top2Particle;
        public Entity top3Particle;
        public Entity bottom1Particle;
        public Entity bottom2Particle;
        public Entity bottom3Particle;
        private List<Entity> particles;

        bool isDead;
        int maxHp;

        private double shootCooldown;
        private double shootTimer;

        public Prefab projectilePelletPrefab;
        private Entity projectilePellet;

        private bool moveHorizontal;
        private int numberOfProjectiles;

        public override void Start()
        {
            particles = new List<Entity>() { rightParticle, leftParticle, top1Particle, top2Particle, top3Particle, bottom1Particle, bottom2Particle, bottom3Particle};
            prevPosition = new Vector3();
            nextPosition = new Vector3(SinglePlayerLogic.mapSizeX - 0.75f, 0.0f, SinglePlayerLogic.mapSizeZ - 0.75f);

            sizeX = 0.75f;
            sizeY = 0.75f;

            maxHp = 500;
            currentHp = maxHp;

            projectilePellet = projectilePelletPrefab.Instantiate().First();

            moveHorizontal = true;
        }


        public override void Update()
        {
            if (!isDead)
            {
                UIScript.BossHpDecorator.Width = UIScript.BossHpDecorator.Parent.ActualWidth * ((float)currentHp / maxHp);
                // Less hp means boss will shoot faster
                shootCooldown = Math.Max(0.25f, currentHp / 150.0f);

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
            if (currentHp != 0) // avoid division by 0
                speed = Math.Min(5.0f, Math.Abs((maxHp - currentHp + 100) / 100));
            else
                speed = 10.0f;

            lerpValue += (float)Game.UpdateTime.Elapsed.TotalSeconds / Vector3.Distance(prevPosition, nextPosition) * speed;
            if (lerpValue > 1.0f)
            {
                lerpValue = 0;

                foreach (var particle in particles)
                    particle.Transform.Scale *= 0.0f;

                var rand = new Random();
                Entity.Transform.Position = Vector3.Lerp(prevPosition, nextPosition, 1.0f);
                prevPosition = nextPosition;

                if (moveHorizontal)
                {
                    moveHorizontal = false;
                    if (Entity.Transform.Position.Z < 0)
                    {
                        // Top and bottom particles are smaller
                        top1Particle.Transform.Scale = new Vector3(0.2f, 0.2f, 0.2f);
                        top2Particle.Transform.Scale = new Vector3(0.2f, 0.2f, 0.2f);
                        top3Particle.Transform.Scale = new Vector3(0.2f, 0.2f, 0.2f);

                        nextPosition.Z = SinglePlayerLogic.mapSizeZ - 0.75f;
                    }
                    else
                    {
                        bottom1Particle.Transform.Scale = new Vector3(0.2f, 0.2f, 0.2f);
                        bottom2Particle.Transform.Scale = new Vector3(0.2f, 0.2f, 0.2f);
                        bottom3Particle.Transform.Scale = new Vector3(0.2f, 0.2f, 0.2f);

                        nextPosition.Z = -SinglePlayerLogic.mapSizeZ + 1.0f;
                    }
                }
                else
                {
                    // 20% chance to move horizontal
                    if (rand.NextDouble() < 0.20)
                    {
                        // Next waypoint will be horizontal, right now find a random position at bottom/top
                        moveHorizontal = true;
                        var randX = (float)rand.NextDouble() * SinglePlayerLogic.mapSizeX;
                        if(nextPosition.X < randX)
                            leftParticle.Transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);
                        else
                            rightParticle.Transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);

                        nextPosition.X = randX;
                    }
                    else
                    {
                        if (Entity.Transform.Position.X < 0)
                        {
                            leftParticle.Transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);
                            nextPosition.X = SinglePlayerLogic.mapSizeX - 0.75f;
                        } 
                        else
                        {
                            rightParticle.Transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);
                            nextPosition.X = -SinglePlayerLogic.mapSizeX + 0.75f;
                        }
                            
                    }
                }
            }
            else
            {
                Entity.Transform.Position = Vector3.Lerp(prevPosition, nextPosition, lerpValue);
            }
        }

        protected void Shoot()
        {
            numberOfProjectiles = Math.Min(5, (maxHp - currentHp + 100) / 100);

            var direction = SinglePlayerLogic.spaceShip.Transform.Position.XZ() - Entity.Transform.Position.XZ();
            direction.Normalize();

            for(int i = -numberOfProjectiles / 2; i <= numberOfProjectiles / 2; i++)
            {
                // when 2 or 4 projectiles, skip center one
                if (i == 0 && numberOfProjectiles % 2 == 0)
                    continue;

                var newProjectile = projectilePellet.Clone();
                newProjectile.Transform.Position = Entity.Transform.Position;

                var newProjectileClass = newProjectile.Get<EnemyProjectile>();

                // Offset the projectile
                newProjectileClass.speedX = direction.X * 2 * Math.Min(speed, 2.5f) + i / 3.0f;
                newProjectileClass.speedZ = direction.Y * 2 * Math.Min(speed, 2.5f) - i / 3.0f;

                newProjectileClass.enemySpeed = speed;

                // Increase projectile time
                newProjectileClass.aliveTimeMax = 7.5;

                MainScript.enemiesScene.Entities.Add(newProjectile);
            }
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
    }
}
