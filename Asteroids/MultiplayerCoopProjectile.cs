using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class MultiplayerCoopProjectile : Projectile
    {
        public override void Start()
        {
            base.Start();
            MultiplayerCoopLogic.playerInfo[spaceShip]++;
        }

        protected override void CheckIfHit()
        {
            base.CheckIfHit();

            // If it didn't hit an asteroid earlier
            if(MultiplayerCoopLogic.friendlyFire)
            {
                if (Entity != null)
                {
                    foreach (var player in MultiplayerCoopLogic.playerInfo.Keys)
                    {
                        if (player != spaceShip)
                        {
                            var spaceShipClass = player.Get<MultiplayerCoopSpaceShip>();
                            if (!spaceShipClass.isInvincible && Utils.CheckIfInRange2D(Entity.Transform.Position, player.Transform.Position, 0.03f + 0.2f, 0.03f + 0.2f))
                            {
                                player.Get<MultiplayerCoopSpaceShip>().Kill();
                                Kill();
                                break;
                            }
                        }
                    }
                }
            }
        }

        protected override void SwitchToParticle()
        {
            base.SwitchToParticle();
            MultiplayerCoopLogic.playerInfo[spaceShip]--;
        }
    }
}
