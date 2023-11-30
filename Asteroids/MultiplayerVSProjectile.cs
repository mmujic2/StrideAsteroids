using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class MultiplayerVSProjectile : Projectile
    {
        public override void Start()
        {
            base.Start();
            var info = MultiplayerVSLogic.playerInfo[spaceShip];
            MultiplayerVSLogic.playerInfo[spaceShip] = new(info.Item1, info.Item2 + 1);
        }

        protected override void CheckIfHit()
        {
            base.CheckIfHit();

            // If it didn't hit an asteroid earlier
            if (Entity != null)
            {
                foreach (var player in MultiplayerVSLogic.playerInfo.Keys)
                {
                    if (player != spaceShip)
                    {
                        var spaceShipClass = player.Get<MultiplayerVSSpaceShip>();
                        if (!spaceShipClass.isInvincible && Utils.CheckIfInRange2D(Entity.Transform.Position, player.Transform.Position, 0.03f + 0.2f, 0.03f + 0.2f))
                        {
                            player.Get<MultiplayerVSSpaceShip>().Kill();
                            Kill();
                            break;
                        }
                    }
                }
            }
        }

        protected override void SwitchToParticle()
        {
            base.SwitchToParticle();

            var info = MultiplayerVSLogic.playerInfo[spaceShip];
            MultiplayerVSLogic.playerInfo[spaceShip] = new(info.Item1, info.Item2 - 1);
        }
    }
}
