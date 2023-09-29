using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    // Same as ParticleTimer component, but moves the particle upwards
    public class BombIncreaseParticleTimer : ParticleTimer
    {
        public override void Update()
        {
            base.Update();
            Entity.Transform.Position.Z -= (float) Game.UpdateTime.Elapsed.TotalSeconds / 2.0f;
        }
    }
}
