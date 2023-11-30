using Stride.Core.Serialization.Contents;
using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class EntityPooling
    {
        // Stride's instancing can be very slow, so it's best to instantiate some used entities at load time to avoid freezes while playing
        public static Dictionary<string, Entity> spaceShips;
        public static Dictionary<string, Entity> multiplayerVsSpaceShips;
        public static Dictionary<string, Entity> multiplayerCoopSpaceeShips;
        public static Dictionary<string, Entity> bosses;
        public static Entity bossTeleport;

        public static void InstantiateEntites(ContentManager content)
        {
            spaceShips = new()
            {
                { "projectileShip", content.Load<Prefab>("My Prefabs/Spaceships/projectileShip").Instantiate().First() },
                { "agilityShip", content.Load<Prefab>("My Prefabs/Spaceships/agilityShip").Instantiate().First() },
                { "damageShip", content.Load<Prefab>("My Prefabs/Spaceships/damageShip").Instantiate().First() }
            };

            bosses = new()
            {
                { "stage1", content.Load<Prefab>("My Prefabs/Bosses/Boss1").Instantiate().First() },
                { "stage2", content.Load<Prefab>("My Prefabs/Bosses/Boss2").Instantiate().First() },
                { "stage3", content.Load<Prefab>("My Prefabs/Bosses/Boss3").Instantiate().First() },
                { "stage4", content.Load<Prefab>("My Prefabs/Bosses/Boss4").Instantiate().First() }
            };

            multiplayerVsSpaceShips = new()
            {
                { "projectileShip", content.Load<Prefab>("My Prefabs/MultiplayerVsSpaceShips/projectileShip").Instantiate().First() },
                { "agilityShip", content.Load<Prefab>("My Prefabs/MultiplayerVsSpaceShips/agilityShip").Instantiate().First() },
                { "damageShip", content.Load<Prefab>("My Prefabs/MultiplayerVsSpaceShips/damageShip").Instantiate().First() }
            };

            multiplayerCoopSpaceeShips = new()
            {
                { "projectileShip", content.Load<Prefab>("My Prefabs/MultiplayerCoopSpaceShips/projectileShip").Instantiate().First() },
                { "agilityShip", content.Load<Prefab>("My Prefabs/MultiplayerCoopSpaceShips/agilityShip").Instantiate().First() },
                { "damageShip", content.Load<Prefab>("My Prefabs/MultiplayerCoopSpaceShips/damageShip").Instantiate().First() }
            };

            Spaceship.spaceShipDestroyParticle = content.Load<Prefab>("My VFX/spaceShipDestroy").Instantiate().First();
            Spaceship.bomb = content.Load<Prefab>("My Prefabs/bomb").Instantiate().First(); 

            AlienShip.alienShipDestroyParticle = content.Load<Prefab>("My VFX/alienShipDestroy").Instantiate().First();
            AlienShip.bombIncreaseParticle = content.Load<Prefab>("My Prefabs/AlienShip/bombBonusParticle").Instantiate().First();
            bossTeleport = content.Load<Prefab>("My VFX/bossTeleport").Instantiate().First();

            Asteroid.asteroidDestroyParticle = content.Load<Prefab>("My VFX/asteroidDestroy").Instantiate().First();

            Projectile.projectileHitParticle = content.Load<Prefab>("My VFX/rocketHit").Instantiate().First();
            Bomb.BombHitParticle = content.Load<Prefab>("My VFX/bombHit").Instantiate().First();

            Asteroid.SmallAsteroids = new();
            Asteroid.MediumAsteroids = new();
            Asteroid.BigAsteroids = new();
            for (int i = 1; i <= 3; i++)
            {
                Asteroid.SmallAsteroids.Add(content.Load<Prefab>("My Prefabs/Asteroids/SmallAsteroid" + i).Instantiate().First());
                Asteroid.MediumAsteroids.Add(content.Load<Prefab>("My Prefabs/Asteroids/MediumAsteroid" + i).Instantiate().First());
                Asteroid.BigAsteroids.Add(content.Load<Prefab>("My Prefabs/Asteroids/BigAsteroid" + i).Instantiate().First());
            }

            MiniShip.miniShip = content.Load<Prefab>("My Prefabs/Projectiles/Boss3Projectile").Instantiate().First();
        }
    }
}
