using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class EnemyProjectile : Projectile
    {
        public float enemySpeed;

        public override void Start()
        {
            base.Start();

            // alien ship moves faster as time passes
            aliveTimeMax /= enemySpeed;
            Utils.PlaySound(fireSound, 0.5f);

        }

        public override void Update()
        {

            aliveTimeCurrent += Game.UpdateTime.Elapsed.TotalSeconds;
            if (aliveTimeCurrent > aliveTimeMax)
            {
                Entity.Scene.Entities.Remove(Entity);
            }

            Move();

            // Search for spaceship instead
            if(!SinglePlayerLogic.spaceShip.Get<Spaceship>().isInvincible && !SinglePlayerLogic.isGameOver &&
                Utils.CheckIfInRange2D(Entity.Transform.Position, SinglePlayerLogic.spaceShip.Transform.Position, 0.02f + SinglePlayerLogic.spaceShip.Get<Spaceship>().sizeX, 0.02f + SinglePlayerLogic.spaceShip.Get<Spaceship>().sizeY))
            {
                SinglePlayerLogic.spaceShip.Get<Spaceship>().Kill();
            }
        }
    }
}
