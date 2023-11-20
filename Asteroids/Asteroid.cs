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
    public class Asteroid : Enemy
    {
        public static List<Entity> BigAsteroids;
        public static List<Entity> MediumAsteroids;
        public static List<Entity> SmallAsteroids;

        public static Entity asteroidDestroyParticle;
        public static Sound AsteroidDeath;

        public int tier;

        [DataMemberIgnore]
        private Vector3 rotation;

        public override void Start()
        {
            sizeX = 0.5f / (4.0f - tier);
            sizeY = sizeX;

            var rand = new Random();

            // tier * [7-10]
            currentHp = tier * (int) ((rand.NextDouble() * 3) + 7);
            
            rotation = new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
        }

        public override void Update()
        {
            Move();

            if(currentHp <= 0)
            {
                Kill();
            }
        }

        protected override void Move()
        {
            Entity.Transform.Position.X += speedX * (float)Game.UpdateTime.Elapsed.TotalSeconds;
            Entity.Transform.Position.Z += speedZ * (float)Game.UpdateTime.Elapsed.TotalSeconds;

            Utils.CheckIfEntityOutsideMapAndFix(Entity);

            Entity.Transform.Rotation *= Quaternion.RotationX(rotation.X * (float) Game.UpdateTime.Elapsed.TotalSeconds);
            Entity.Transform.Rotation *= Quaternion.RotationY(rotation.Y * (float) Game.UpdateTime.Elapsed.TotalSeconds);
            Entity.Transform.Rotation *= Quaternion.RotationZ(rotation.Z * (float) Game.UpdateTime.Elapsed.TotalSeconds);
        }

        public override void Kill()
        {
            if (tier != 1)
                SplitAsteroid();

            Utils.PlaySound(AsteroidDeath, 0.25f);

            var particle = asteroidDestroyParticle.Clone();
            particle.Transform.Position = Entity.Transform.Position;
            particle.Transform.Scale *= 1.0f / (4 - tier);
            MainScript.particlesScene.Entities.Add(particle);

            SinglePlayerLogic.score += 10 * (4 - tier); // smaller asteroids give more points
            Entity.Scene.Entities.Remove(Entity);
            Entity.Dispose();
        }

        private void SplitAsteroid()
        {
            List<Entity> asteroids = null;
            if (tier == 3)
                asteroids = MediumAsteroids;
            else if (tier == 2)
                asteroids = SmallAsteroids;

            if(asteroids != null)
            {
                var rand = new Random();

                var asteroid1 = asteroids[rand.Next(0, asteroids.Count)].Clone();
                var asteroid2 = asteroids[rand.Next(0, asteroids.Count)].Clone();

                asteroid1.Transform.Position = Entity.Transform.Position;
                asteroid2.Transform.Position = Entity.Transform.Position;

                var asteroid1Class = asteroid1.Get<Asteroid>();
                var asteroid2Class = asteroid2.Get<Asteroid>();

                asteroid1Class.tier = tier - 1;
                asteroid2Class.tier = tier - 1;

                asteroid1Class.speedX = speedX + speedX * ((float) rand.NextDouble() + 0.5f) * 0.5f; // speedX + speedX * [0.25,0.75]
                asteroid1Class.speedZ = speedZ + speedX * ((float) rand.NextDouble() + 0.5f) * 0.5f;

                asteroid2Class.speedX = speedX + speedX * ((float) rand.NextDouble() + 0.5f) * 0.5f;
                asteroid2Class.speedZ = speedZ + speedX * ((float) rand.NextDouble() + 0.5f) * 0.5f;

                MainScript.enemiesScene.Entities.Add(asteroid1);
                MainScript.enemiesScene.Entities.Add(asteroid2);
            }
        }
    }
}
