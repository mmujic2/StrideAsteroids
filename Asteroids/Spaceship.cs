using Stride.Audio;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using Stride.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class Spaceship : SyncScript
    {
        public static Entity spaceShipDestroyParticle;
        public static Sound moveSound;
        public SoundInstance moveSoundInstance;
        public static Sound ShipDeathSound;

        public float acceleration;
        public float maxSpeed;

        [DataMemberIgnore]
        public float currentXSpeed;
        [DataMemberIgnore]
        public float currentZSpeed;
        public int maxProjectilesOnScreen;

        private Vector3 direction;

        public Entity spaceShipModel;
        public Entity imagniaryPoint; // used for easier direction calculation

        public Prefab projectileRocketPrefab;
        private Entity projectileRocket;
        public static Entity bomb;

        private Entity trailParticle;

        [DataMemberIgnore]
        public float sizeX;
        [DataMemberIgnore]
        public float sizeY;

        // Manually keep track of spaceship status because removing it from scene removes all particles it emitted
        public bool isDead;
        // When ship "respawns", it's invincible
        public bool isInvincible;

        public override void Cancel()
        {
            if(moveSoundInstance.PlayState == Stride.Media.PlayState.Playing)
                moveSoundInstance.Stop();
        }

        public override void Start()
        {
            if (Entity.GetChildren().Count() > 2)
                trailParticle = Entity.GetChild(2);

            sizeX = 0.2f;
            sizeY = 0.2f;

            currentXSpeed = 0.0f;
            currentZSpeed = 0.0f;

            projectileRocket = projectileRocketPrefab.Instantiate().First();

            moveSoundInstance = moveSound.CreateInstance();
            moveSoundInstance.Volume = 0.25f;
            moveSoundInstance.IsLooping = true;
        }

        public override void Update()
        {
            if(!isDead)
            {
                Move();

                if (Input.IsKeyReleased(Keys.Space))
                {
                    int projectileCount = 0;
                    foreach (var entity in MainScript.projectilesScene.Entities)
                    {
                        // Scene can also contain particles
                        if (entity.Get<Projectile>() != null) projectileCount++;
                    }

                    if (projectileCount < maxProjectilesOnScreen)
                        Shoot();
                }

                if(GameLogic.numberOfBombs > 0 && (Input.IsKeyReleased(Keys.LeftCtrl) || Input.IsKeyReleased(Keys.RightCtrl)))
                {
                    GameLogic.numberOfBombs--;
                    ShootBomb();
                }

                // If game is over, don't process collisions (player can't die)
                if (!isInvincible && !GameLogic.isGameOver)
                {
                    var enemyEntity = Utils.CheckIfEnemyIsInRange(Entity, new Vector2(sizeX, sizeY));
                    if (enemyEntity != null)
                    {
                        enemyEntity.currentHp -= 10;
                        Kill();
                    }
                }
            }
        }

        private void Move()
        {
            Vector3 imaginaryPointWorldPosition;
            Vector3 spaceShipWorldPosition;

            imagniaryPoint.Transform.GetWorldTransformation(out imaginaryPointWorldPosition, out _, out _);
            spaceShipModel.Transform.GetWorldTransformation(out spaceShipWorldPosition, out _, out _);

            direction = imaginaryPointWorldPosition - spaceShipWorldPosition;
            direction.Normalize();

            if (Input.IsKeyDown(Keys.Up))
            {
                currentXSpeed += direction.X * acceleration * (float)Game.UpdateTime.Elapsed.TotalSeconds * 2.5f;
                currentZSpeed += direction.Z * acceleration * (float)Game.UpdateTime.Elapsed.TotalSeconds * 2.5f;
                trailParticle.Transform.Scale = new Vector3(1.0f, 1.0f, 1.0f);
                if(moveSoundInstance.PlayState != Stride.Media.PlayState.Playing)
                    moveSoundInstance.Play();
            }
            else if (Input.IsKeyDown(Keys.Down))
            {
                
            }
            else
            {
                trailParticle.Transform.Scale *= 0.0f;
                if (moveSoundInstance.PlayState == Stride.Media.PlayState.Playing)
                    moveSoundInstance.Pause();
            }
                

            // Clamp speed
            if (Math.Abs(currentXSpeed) > maxSpeed)
                currentXSpeed = Math.Sign(currentXSpeed) * maxSpeed;

            if (Math.Abs(currentZSpeed) > maxSpeed)
                currentZSpeed = Math.Sign(currentZSpeed) * maxSpeed;

            Entity.Transform.Position.X += currentXSpeed * (float)Game.UpdateTime.Elapsed.TotalSeconds;
            Entity.Transform.Position.Z += currentZSpeed * (float)Game.UpdateTime.Elapsed.TotalSeconds;

            Utils.CheckIfEntityOutsideMapAndFix(Entity);

            // Rotation
            if (Input.IsKeyDown(Keys.Right))
                Entity.Transform.Rotation *= Quaternion.RotationY(-5.0f * acceleration * (float) Game.UpdateTime.Elapsed.TotalSeconds);
            else if (Input.IsKeyDown(Keys.Left))
                Entity.Transform.Rotation *= Quaternion.RotationY(5.0f * acceleration * (float)Game.UpdateTime.Elapsed.TotalSeconds);
        }

        private void Shoot()
        {
            // Imaginary point is the point from which the ship shoots
            var projectile = projectileRocket.Clone();
            imagniaryPoint.Transform.GetWorldTransformation(out projectile.Transform.Position, out _, out _);

            projectile.Transform.Rotation = Quaternion.RotationY(Utils.GetAngleXAxis2D(new Vector2(direction.X, direction.Z)));

            var projectileClass = projectile.Get<Projectile>();
            projectileClass.speedX = direction.X * 4 * acceleration + currentXSpeed;
            projectileClass.speedZ = direction.Z * 4 * acceleration + currentZSpeed;

            MainScript.projectilesScene.Entities.Add(projectile);
        }

        private void ShootBomb()
        {
            // Same as shoot, just different speed
            var projectile = bomb.Clone();
            imagniaryPoint.Transform.GetWorldTransformation(out projectile.Transform.Position, out _, out _);

            projectile.Transform.Rotation = Quaternion.RotationY(Utils.GetAngleXAxis2D(new Vector2(direction.X, direction.Z)));

            var projectileClass = projectile.Get<Projectile>();
            projectileClass.speedX = direction.X + currentXSpeed;
            projectileClass.speedZ = direction.Z + currentZSpeed;

            MainScript.projectilesScene.Entities.Add(projectile);
        }

        public void Kill()
        {
            Utils.PlaySound(ShipDeathSound, 0.2f);

            var particle = spaceShipDestroyParticle.Clone();
            particle.Transform.Position = Entity.Transform.Position;
            MainScript.particlesScene.Entities.Add(particle);

            currentXSpeed = 0;
            currentZSpeed = 0;
            Entity.Transform.Position = new Vector3(0.0f, 0.0f, 0.0f);

            // Disable the model instead of removing from scene to keep particles
            spaceShipModel.Get<ModelComponent>().Enabled = false;

            isInvincible = true;
            isDead = true;

            // If died while moving, prevent particle from emitting until ship isn't dead anymore
            trailParticle.Transform.Scale *= 0.0f;

            // Also pause move sound
            if (moveSoundInstance.PlayState == Stride.Media.PlayState.Playing)
                moveSoundInstance.Pause();

            if (GameLogic.numberOfLives > 0)
            {
                var countdown = new CountdownScript();
                countdown.countdownType = CountdownScript.CountdownType.spaceShipDead;
                countdown.maxTimer = 2.0;

                MainScript.Camera.Add(countdown);

                GameLogic.numberOfLives--;
            }
            else
            {
                Utils.PlaySound(SoundScript.loseSound);

                UIScript.GameOverPanel.Visibility = Visibility.Visible;
                UIScript.GameOverTitle.Text = "Game over";
                UIScript.GameOverInfo.Text = "All lives lost";
                UIScript.HideGameOverButton.Visibility = Visibility.Hidden;
                GameLogic.isGameOver = true;
            }
        }
    }
}
