using Stride.Audio;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class AlienShip : Enemy
    {
        public static Entity alienShipDestroyParticle;
        public static Entity bombIncreaseParticle;
        public static Sound AlienDeathSound;
        public static Sound AlienWarningSound;

        [DataMemberIgnore]
        public float speed;
        private float lerpValue;
        private Vector3 nextPosition;
        private Vector3 prevPosition;

        private double shootCooldown;
        private double shootTimer;

        public Prefab projectilePelletPrefab;
        private Entity projectilePellet;

        public Entity rotatePart;

        public override void Start()
        {
            lerpValue = 0;
            currentHp = 15;
            sizeX = 0.2f;
            sizeY = 0.2f;
            prevPosition = Entity.Transform.Position;
            Entity.Dispose();

            nextPosition = GameLogic.spaceShip.Transform.Position;

            shootCooldown = Math.Max(0.75, 5.0 / speed);
            shootTimer = 0;

            projectilePellet = projectilePelletPrefab.Instantiate().First();
        }

        public override void Update()
        {
            shootTimer += Game.UpdateTime.Elapsed.TotalSeconds;
            if(shootTimer >= shootCooldown && !GameLogic.spaceShip.Get<Spaceship>().isDead)
            {
                shootTimer = 0;
                Shoot();
            }

            Move();

            if (currentHp <= 0)
                Kill();
        }

        protected override void Move()
        {
            lerpValue += (float) Game.UpdateTime.Elapsed.TotalSeconds / Vector3.Distance(prevPosition, nextPosition) * speed;

            if (lerpValue > 1.0f)
            {
                Vector3.Lerp(prevPosition, nextPosition, 1.0f);
                prevPosition = Entity.Transform.Position;

                // prevent alien from camping spawn position
                if(!GameLogic.spaceShip.Get<Spaceship>().isDead)
                    nextPosition = GameLogic.spaceShip.Transform.Position;
                else
                {
                    var rand = new Random();
                    nextPosition = new Vector3(GameLogic.mapSizeX * ((float) rand.NextDouble() - 0.5f), 0, GameLogic.mapSizeZ * ((float)rand.NextDouble() - 0.5f));
                }
                    
                
                lerpValue = 0;
            }
            else
            {
                Entity.Transform.Position = Vector3.Lerp(prevPosition, nextPosition, lerpValue);
            }

            rotatePart.Transform.Rotation *= Quaternion.RotationY(speed * (float)Game.UpdateTime.Elapsed.TotalSeconds);
        }

        private void Shoot()
        {
            var direction = GameLogic.spaceShip.Transform.Position.XZ() - Entity.Transform.Position.XZ();
            direction.Normalize();

            var newProjectile = projectilePellet.Clone();
            newProjectile.Transform.Position = Entity.Transform.Position;

            var newProjectileClass = newProjectile.Get<EnemyProjectile>();
            newProjectileClass.speedX = direction.X * 2 * speed;
            newProjectileClass.speedZ = direction.Y * 2 * speed;
            newProjectileClass.enemySpeed = speed;


            MainScript.enemiesScene.Entities.Add(newProjectile);
        }

        public override void Kill()
        {
            Utils.PlaySound(AlienDeathSound, 0.2f);
            GameLogic.score += (int)(50 * speed);
            GameLogic.numberOfBombs++;

            // Explosion
            var particle = alienShipDestroyParticle.Clone();
            particle.Transform.Position = Entity.Transform.Position;
            MainScript.particlesScene.Entities.Add(particle);

            // Bomb bonus
            particle = bombIncreaseParticle.Clone();
            particle.Transform.Position = Entity.Transform.Position;
            MainScript.particlesScene.Entities.Add(particle);

            Entity.Scene.Entities.Remove(Entity);
            Entity.Dispose();
        }
    }
}
