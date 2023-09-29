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
    public class Utils
    {
        public static void CheckIfEntityOutsideMapAndFix(Entity entity)
        {
            if (Math.Abs(entity.Transform.Position.X) > GameLogic.mapSizeX)
                entity.Transform.Position.X = -Math.Sign(entity.Transform.Position.X) * GameLogic.mapSizeX; 

            // Fix Z separately because of UI being in the way
            if (entity.Transform.Position.Z > GameLogic.mapSizeZ)
            {
                entity.Transform.Position.Z = -GameLogic.mapSizeZ + 0.35f;
            }
            else if(entity.Transform.Position.Z < -GameLogic.mapSizeZ + 0.35f)
            {
                entity.Transform.Position.Z = GameLogic.mapSizeZ;
            }
                
        }

        public static Enemy CheckIfEnemyIsInRange(Entity entity, Vector2 entitySize)
        {
            foreach(var enemy in MainScript.enemiesScene.Entities)
            {
                var enemyClass = enemy.Get<Enemy>();
                if(enemyClass != null)
                {
                    // 2D square collider
                    if(CheckIfInRange2D(entity.Transform.Position, enemy.Transform.Position, entitySize.X + enemyClass.sizeX, entitySize.Y + enemyClass.sizeY))
                        return enemyClass;
                }
            }

            return null;
        }

        public static bool CheckIfInRange2D(Vector3 position1, Vector3 position2, float range1, float range2)
        {
            // 2D rectangle collider
            if (Math.Abs(position1.X - position2.X) < range1 && 
                Math.Abs(position1.Z - position2.Z) < range2)
            {
                return true;
            }

            return false;
        }

        public static float GetAngleXAxis2D(Vector2 vec1)
        {
            var cosAngle = vec1.X / Math.Sqrt(vec1.X * vec1.X + vec1.Y * vec1.Y);
            if (Math.Abs(cosAngle) > 1.0)
                cosAngle = Math.Round(cosAngle);

            float angle = (float)Math.Acos(cosAngle);

            if (vec1.Y > 0)
                angle = 2 * (float) Math.PI - angle;

            return angle;
        }

        public static int GetSpriteFrameFromShipName()
        {
            if(GameLogic.spaceShip != null)
            {
                switch (GameLogic.spaceShip.Name)
                {
                    case "projectileShip": return 0;
                    case "agilityShip": return 3;
                    case "damageShip": return 4;
                    default: return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public static void PlaySound(Sound sound, float volume = 0.5f, float pitch = 1.0f)
        {
            var instance = sound.CreateInstance();
            instance.Volume = volume;
            instance.Pitch = pitch;
            instance.Play();
        }
    }
}
