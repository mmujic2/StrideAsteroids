using Stride.Audio;
using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class Bomb : Projectile
    {
        public static Entity BombHitParticle;
        public Sound bombMove;

        protected override void CheckIfHit()
        {
            if (Utils.CheckIfEnemyIsInRange(Entity, new Vector2(0.02f, 0.02f)) != null)
            {
                SwitchToParticle();
            }
        }

        protected override void SwitchToParticle()
        {
            // Bomb does damage even if it reaches timer max, it's pretty inefficient since it loops through all enemies twice when hit occurs
            ApplyDamage();

            var particle = BombHitParticle.Clone();
            particle.Transform.Position = Entity.Transform.Position;
            MainScript.particlesScene.Entities.Add(particle);

            Utils.PlaySound(hitSound);
            base.SwitchToParticle();
        }

        private void ApplyDamage()
        {
            foreach(var enemyEntity in MainScript.enemiesScene.Entities)
            {
                var enemyClass = enemyEntity.Get<Enemy>();
                if(enemyClass != null)
                {
                    if(Utils.CheckIfInRange2D(enemyEntity.Transform.Position, Entity.Transform.Position, 2.5f, 2.5f))
                    {
                        enemyClass.currentHp -= damage;
                    }
                }
            }
        }
    }
}
