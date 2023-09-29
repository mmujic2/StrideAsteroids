using Stride.Core;
using Stride.Engine;
using Stride.Core.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Audio;

namespace Asteroids
{
    public class Projectile : SyncScript
    {
        public static Entity projectileHitParticle;
        public Sound fireSound;
        public Sound hitSound;

        protected Entity trailParticle;

        public int damage;

        public double aliveTimeMax;
        protected double aliveTimeCurrent;

        [DataMemberIgnore]
        public float speedX;
        [DataMemberIgnore]
        public float speedZ;

        public override void Start()
        {
            if(Entity.GetChildren().Count() > 1)
                trailParticle = Entity.GetChild(1);

            aliveTimeCurrent = 0.0;
            Utils.PlaySound(fireSound, 0.25f);
        }

        public override void Update()
        {
            aliveTimeCurrent += Game.UpdateTime.Elapsed.TotalSeconds;
            if(aliveTimeCurrent > aliveTimeMax)
            {
                SwitchToParticle();
                return;
            }

            Move();
            CheckIfHit();
        }

        protected void Move()
        {
            Entity.Transform.Position.X += speedX * (float)Game.UpdateTime.Elapsed.TotalSeconds;
            Entity.Transform.Position.Z += speedZ * (float)Game.UpdateTime.Elapsed.TotalSeconds;

            Utils.CheckIfEntityOutsideMapAndFix(Entity);
        }

        protected virtual void SwitchToParticle()
        {
            // Remove projectile model from entity and script
            Entity.RemoveChild(Entity.GetChild(0));

            // Now it's practically a particle entity
            // Add particle script
            var timer = new ParticleTimer();
            timer.aliveTimeMax = 2.0;
            Entity.Add(timer);

            // Change particle scale to stop spawning new trail particles, since projectile hit it's target
            if(trailParticle != null)
                trailParticle.Transform.Scale *= 0.0f;

            Entity.Remove<Projectile>();
        }

        protected virtual void CheckIfHit()
        {
            var enemyEntity = Utils.CheckIfEnemyIsInRange(Entity, new Vector2(0.02f, 0.02f));
            if (enemyEntity != null)
            {
                enemyEntity.currentHp -= damage;

                var particle = projectileHitParticle.Clone();
                particle.Transform.Position = Entity.Transform.Position;
                MainScript.particlesScene.Entities.Add(particle);

                Utils.PlaySound(hitSound);
                SwitchToParticle();
            }
        }
    }
}
