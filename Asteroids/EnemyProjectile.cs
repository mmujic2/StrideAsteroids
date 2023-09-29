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
            if(!GameLogic.spaceShip.Get<Spaceship>().isInvincible && !GameLogic.isGameOver &&
                Utils.CheckIfInRange2D(Entity.Transform.Position, GameLogic.spaceShip.Transform.Position, 0.02f + GameLogic.spaceShip.Get<Spaceship>().sizeX, 0.02f + GameLogic.spaceShip.Get<Spaceship>().sizeY))
            {
                GameLogic.spaceShip.Get<Spaceship>().Kill();
            }
        }
    }
}
