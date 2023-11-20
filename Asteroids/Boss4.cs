using Stride.Engine;
using Stride.Core.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class Boss4 : Enemy
    {
        // A bit more complicated so will be implemented using state machines
        enum State
        {
            wait,
            startSpin,
            spinAndShoot,
            stopSpin,
            moveOffScreen,
            shootOffScreen,
            moveBack
        }

        private double shootTimer;
        private double shootCooldown;
        private int numberOfProjectilesFired;
        private int numberOfProjectilesPerCircle;
        private int numberOfProjectilesPerLine;

        bool isDead;
        int maxHp;

        public Entity rightParticle;
        public Entity leftParticle;
        public Entity topParticle;
        public Entity bottomParticle;
        public Entity spinParticle;
        private List<Entity> moveParticles;

        private State currentState;
        private int signal;

        private double timer;

        private float moveSpeed;
        private float spinSpeed;
        
        public Prefab projectilePelletPrefab;
        private Entity projectilePellet;

        private float lerpValue;
        private Vector3 prevPosition;
        private Vector3 nextPosition;

        public override void Start()
        {
            isDead = false;

            moveParticles = new List<Entity>() { rightParticle, leftParticle, topParticle, bottomParticle, spinParticle };

            sizeX = 0.75f;
            sizeY = 0.75f;

            maxHp = 1000;
            currentHp = maxHp;

            projectilePellet = projectilePelletPrefab.Instantiate().First();

            spinSpeed = 0.0f;
            numberOfProjectilesFired = 0;
            numberOfProjectilesPerCircle = 32;
            numberOfProjectilesPerLine = 20;

            foreach (var particle in moveParticles)
                particle.Transform.Scale *= 0.0f;

            signal = 0;
            currentState = State.wait;
        }

        public override void Update()
        {
            if (!isDead)
            {
                moveSpeed = Math.Max(5.0f, (maxHp - currentHp + 250.0f) / 250.0f);
                UIScript.BossHpDecorator.Width = UIScript.BossHpDecorator.Parent.ActualWidth * ((float)currentHp / maxHp);
                ExecuteState();

                if(currentHp <= 0)
                    Kill();
            }
        }

        private void Transition()
        {
            switch(currentState)
            {
                case State.wait:
                    {
                        if (signal == 0) {
                            foreach (var particle in moveParticles)
                                particle.Transform.Scale *= 0.0f;

                            spinParticle.Transform.Scale += 1.0f;
                            currentState = State.startSpin;
                        } 
                        else if (signal == 1)
                        {
                            lerpValue = 0f;
                            GetOffScreenPosition();
                            currentState = State.moveOffScreen;
                        }
                        
                        break;
                    }
                case State.startSpin:
                    {
                        numberOfProjectilesFired = 0;
                        currentState = State.spinAndShoot;
                        break;
                    }
                case State.spinAndShoot:
                    {
                        currentState = State.stopSpin;
                        break;
                    }
                case State.stopSpin:
                    {
                        signal = 1;
                        currentState = State.wait;
                        break;
                    }
                case State.moveOffScreen:
                    {
                        numberOfProjectilesFired = 0;
                        currentState = State.shootOffScreen;
                        break;
                    }
                case State.shootOffScreen:
                    {
                        GetBackScreenPosition();
                        currentState = State.moveBack;
                        break;
                    }
                case State.moveBack:
                    {
                        signal = 0;
                        currentState = State.wait;
                        break;
                    }
                default: currentState = State.wait; break;
            }
        }

        private void ExecuteState()
        {
            switch (currentState)
            {
                case State.wait: Wait(); break;
                case State.startSpin: StartSpin(); break;
                case State.spinAndShoot: SpinAndShoot(); break;
                case State.stopSpin: StopSpin(); break;
                case State.moveOffScreen: MoveOffScreen(); break;
                case State.shootOffScreen: ShootOffScreen(); break;
                case State.moveBack: MoveBack(); break;
                default: Wait(); break;
            }
        }

        private void Wait()
        {
            timer += Game.UpdateTime.Elapsed.TotalSeconds;
            if (timer > 2.0)
            {
                timer = 0.0;
                Transition();
            }
        }

        private void StartSpin()
        {
            spinSpeed += 2.5f * (float) Game.UpdateTime.Elapsed.TotalSeconds;
            if(spinSpeed > 5.0f)
            {
                spinSpeed = 5.0f;
                Transition();
            }
            Entity.Transform.Rotation *= Quaternion.RotationY(spinSpeed * (float)Game.UpdateTime.Elapsed.TotalSeconds);
        }

        private void SpinAndShoot()
        {
            Entity.Transform.Rotation *= Quaternion.RotationY(spinSpeed * (float) Game.UpdateTime.Elapsed.TotalSeconds);

            Shoot();
        }
       
        private void StopSpin()
        {
            spinSpeed -= 2.5f * (float)Game.UpdateTime.Elapsed.TotalSeconds;
            if (spinSpeed < 0.0f)
            {
                foreach (var particle in moveParticles)
                    particle.Transform.Scale *= 0.0f;

                spinSpeed = 0.0f;
                Transition();
            }
            Entity.Transform.Rotation *= Quaternion.RotationY(spinSpeed * (float)Game.UpdateTime.Elapsed.TotalSeconds);
        }

        private void MoveOffScreen()
        {
            Move();
        }

        private void ShootOffScreen()
        {
            Shoot();
        }

        private void MoveBack()
        {
            Move();
        }

        private void Shoot()
        {
            shootTimer += Game.UpdateTime.Elapsed.TotalSeconds;

            if (currentState == State.spinAndShoot)
            {
                shootCooldown = Math.Max(0.025f, (currentHp + 200.0f) / 8000.0f);

                if (shootTimer > shootCooldown)
                {
                    // PI = 180 degrees, contains half of the projectiles, so split circle into equal angles
                    // Also add offset so that standing in a single spot won't dodge all projectiles

                    var angle = Math.PI / (numberOfProjectilesPerCircle / 2.0f) * (numberOfProjectilesFired % numberOfProjectilesPerCircle) + (numberOfProjectilesFired % 16) * Math.PI / numberOfProjectilesPerCircle / 3.5f;
                    var direction = new Vector3((float) Math.Cos(angle), 0.0f, (float) Math.Sin(angle));

                    CreateProjectile(direction);

                    shootTimer = 0.0;
                    numberOfProjectilesFired++;
                    // 2 circles of projectiles
                    if (numberOfProjectilesFired == 2 * numberOfProjectilesPerCircle)
                    {
                        Transition();
                    }
                }
            }
            else if(currentState == State.shootOffScreen) 
            {
                shootCooldown = Math.Max(0.025f, (currentHp + 50.0f) / 2000.0f);

                if (shootTimer > shootCooldown)
                {
                    var direction = -Entity.Transform.Position;
                    direction.Normalize();

                    float offsetX = 0f;
                    float offsetZ = 0f;
                    if(Math.Abs(Entity.Transform.Position.X) > 0)
                    {
                        offsetZ = ((numberOfProjectilesFired % numberOfProjectilesPerLine) - (numberOfProjectilesPerLine / 2.0f)) / (numberOfProjectilesPerLine / 2.0f) * SinglePlayerLogic.mapSizeZ;
                    }
                    else
                    {
                        offsetX = ((numberOfProjectilesFired % numberOfProjectilesPerLine) - (numberOfProjectilesPerLine / 2.0f)) / (numberOfProjectilesPerLine / 2.0f) * SinglePlayerLogic.mapSizeX;
                    }

                    CreateProjectile(direction, 10.0f, moveSpeed * 0.25f, offsetX, offsetZ);


                    shootTimer = 0.0;

                    numberOfProjectilesFired++;
                    // 5 Lines of projectiles
                    if (numberOfProjectilesFired > 3 * numberOfProjectilesPerLine)
                    {
                        Transition();
                    }
                }
            }
        }

        private void CreateProjectile(Vector3 direction, float aliveTime = 2.5f, float speedBonus = 1.0f, float offsetX = 0.0f, float offsetZ = 0.0f)
        {
            var newProjectile = projectilePellet.Clone();
            newProjectile.Transform.Position = Entity.Transform.Position;

            newProjectile.Transform.Position.X += offsetX;
            newProjectile.Transform.Position.Z += offsetZ;

            var newProjectileClass = newProjectile.Get<EnemyProjectile>();
            newProjectileClass.speedX = direction.X * 2 * speedBonus;
            newProjectileClass.speedZ = direction.Z * 2 * speedBonus;
            newProjectileClass.enemySpeed = 1.0f;

            // Increase projectile time
            newProjectileClass.aliveTimeMax = aliveTime;

            MainScript.enemiesScene.Entities.Add(newProjectile);
        }

        private void GetOffScreenPosition()
        {
            foreach(var particle in moveParticles)
                particle.Transform.Scale *= 0.0f;

            var rand = new Random();
            var number = rand.NextDouble();

            prevPosition = Entity.Transform.Position;
            // 25% chance to move any of 4 directions
            if (number < 0.25)
            {
                leftParticle.Transform.Scale += 0.5f;
                nextPosition = new Vector3(7.0f, -2.0f, 0f);
            }
            else if (number < 0.5)
            {
                rightParticle.Transform.Scale += 0.5f;
                nextPosition = new Vector3(-7.0f, -2.0f, 0f);
            }
            else if (number < 0.75)
            {
                topParticle.Transform.Scale += 0.5f;
                nextPosition = new Vector3(0f, -2.0f, 7.0f);
            }
            else
            {
                bottomParticle.Transform.Scale += 0.5f;
                nextPosition = new Vector3(0f, -2.0f, -7.0f);
            }
        }

        private void GetBackScreenPosition()
        {
            foreach (var particle in moveParticles)
                particle.Transform.Scale *= 0.0f;

            prevPosition = Entity.Transform.Position;
            nextPosition = new Vector3(0f, -2.0f, 0f);

            if (prevPosition.X > 0.001)
                rightParticle.Transform.Scale += 0.5f;
            else if (prevPosition.X < 0.001)
                leftParticle.Transform.Scale += 0.5f;
            else if (prevPosition.Z > 0.001)
                bottomParticle.Transform.Scale += 0.5f;
            else if (prevPosition.Z < 0.001) 
                topParticle.Transform.Scale += 0.5f;
        }

        protected override void Move()
        {
            lerpValue += (float)Game.UpdateTime.Elapsed.TotalSeconds * moveSpeed / 5.0f;
            if (lerpValue > 1.0f)
            {
                Transition();
                Entity.Transform.Position = Vector3.Lerp(prevPosition, nextPosition, 1.0f);
                lerpValue = 0.0f;
            }
            else
            {
                Entity.Transform.Position = Vector3.Lerp(prevPosition, nextPosition, lerpValue);
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
