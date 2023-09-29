using Stride.Audio;
using Stride.Core.Serialization.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    public class SoundScript
    {
        public List<SoundInstance> loopingSounds;
        public static Sound UIClickSound;
        public static Sound BossDeath;
        public static Sound winSound;
        public static Sound loseSound;
        public static Sound BossTeleport;
        public static Sound Timer;
        public static Sound BossStart;
        // Music file is small so no need for streaming from disk
        public static Sound BossMusic;

        public static Sound ExtraLife;


        public static void LoadSounds(ContentManager content)
        {
            Spaceship.moveSound = content.Load<Sound>("Audio/FX/shipMove");
            UIClickSound = content.Load<Sound>("Audio/FX/UIClick");

            Spaceship.ShipDeathSound = content.Load<Sound>("Audio/FX/shipDeath");

            AlienShip.AlienDeathSound = content.Load<Sound>("Audio/FX/alienExplosion");

            Asteroid.AsteroidDeath = content.Load<Sound>("Audio/FX/asteroidDeath");

            BossDeath = content.Load<Sound>("Audio/FX/bossDeath");
            winSound = content.Load<Sound>("Audio/FX/win");
            loseSound = content.Load<Sound>("Audio/FX/lose");
            BossTeleport = content.Load<Sound>("Audio/FX/bossTeleport");
            Timer = content.Load<Sound>("Audio/FX/timer");
            BossStart = content.Load<Sound>("Audio/FX/bossIncoming");
            BossMusic = content.Load<Sound>("Audio/Music/bossMusic");

            ExtraLife = content.Load<Sound>("Audio/FX/extraLife");
        }

        // Separate method because UI click is used in a lot of places
        public static void PlayUISound()
        {
            var sound = UIClickSound.CreateInstance();
            sound.Volume = 0.1f;
            sound.Play();
        }
    }
}
