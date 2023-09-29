using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class ParticleTimer : SyncScript
    {
        public double aliveTimeMax;
        private double aliveTimeCurrent;

        public override void Update()
        {
            aliveTimeCurrent += Game.UpdateTime.Elapsed.TotalSeconds;
            if(aliveTimeCurrent > aliveTimeMax)
            {
                Entity.Scene.Entities.Remove(Entity);
                Entity.Dispose();
                return;
            }
        }
    }
}
