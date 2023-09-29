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
    public class MiniShip : Enemy
    {
        public static Entity miniShip;

        [DataMemberIgnore]
        public Vector3 direction;
        [DataMemberIgnore]
        public float speedMultiplier;

        public override void Start()
        {
            sizeX = 0.25f;
            sizeY = 0.25f;
            currentHp = 1;
        }

        public override void Update()
        {
            Move();
            if(currentHp <= 0)
            {
                Kill();
            }
        }

        public override void Kill()
        {
            GameLogic.score += 25;

            var particle = AlienShip.alienShipDestroyParticle.Clone();
            particle.Transform.Position = Entity.Transform.Position;
            // Make death particle smaller
            particle.Transform.Scale *= 0.75f;

            MainScript.particlesScene.Entities.Add(particle);

            Utils.PlaySound(AlienShip.AlienDeathSound, 0.25f);

            Entity.Scene.Entities.Remove(Entity);
            Entity.Dispose();
        }

        protected override void Move()
        {
            Entity.Transform.Position.X += speedX * (float) Game.UpdateTime.Elapsed.TotalSeconds * speedMultiplier;
            Entity.Transform.Position.Z += speedZ * (float) Game.UpdateTime.Elapsed.TotalSeconds * speedMultiplier;

            var preferredDirection =  GameLogic.spaceShip.Transform.Position - Entity.Transform.Position;
            preferredDirection.Normalize();

            if (direction.X < preferredDirection.X - 0.01f)
            {
                // Increase slowly
                direction.X += 2.5f * (float)Game.UpdateTime.Elapsed.TotalSeconds * Math.Abs(direction.X - preferredDirection.X) * speedMultiplier;

                // Clamp direction
                if(direction.X > preferredDirection.X)
                    direction.X = preferredDirection.X;
            }
            else if (direction.X > preferredDirection.X + 0.01f)
            {
                direction.X -= 2.5f * (float)Game.UpdateTime.Elapsed.TotalSeconds * Math.Abs(direction.X - preferredDirection.X) * speedMultiplier;

                if (direction.X < preferredDirection.X)
                    direction.X = preferredDirection.X;
            }

            // Same for Z coordinate
            if (direction.Z < preferredDirection.Z - 0.01f)
            {
                // Increase slowly
                direction.Z += 2.5f * (float)Game.UpdateTime.Elapsed.TotalSeconds * Math.Abs(direction.Z - preferredDirection.Z) * speedMultiplier;

                // Clamp direction
                if (direction.Z > preferredDirection.Z)
                    direction.Z = preferredDirection.Z;
            }
            else if (direction.Z > preferredDirection.Z + 0.01f)
            {
                direction.Z -= 2.5f * (float)Game.UpdateTime.Elapsed.TotalSeconds * Math.Abs(direction.Z - preferredDirection.Z) * speedMultiplier;

                if (direction.Z < preferredDirection.Z)
                    direction.Z = preferredDirection.Z;
            }

            direction.Normalize();
            speedX = direction.X;
            speedZ = direction.Z;

            Entity.Transform.Rotation = Quaternion.RotationY(Utils.GetAngleXAxis2D(new Vector2(speedX, speedZ)));
        }
    }
}
