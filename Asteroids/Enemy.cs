using Stride.Core;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public abstract class Enemy : SyncScript
    {
        [DataMemberIgnore]
        public float speedX;
        [DataMemberIgnore]
        public float speedZ;
        [DataMemberIgnore]
        public float sizeX;
        [DataMemberIgnore]
        public float sizeY;

        [DataMemberIgnore]
        public int currentHp;

        protected abstract void Move();
        public abstract void Kill();

    }
}
