using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrexRunner.Code.Game1;

namespace TrexRunner.Code.BossCode
{
    /// <summary>
    /// Handles attacking logic for the boss
    /// </summary>
    public abstract class BossAttackPattern
    {
        public Vector2 ShootDelays;
        public float CurDelay;
        public Boss Parent;
        public float ProjectileSpeed;
        protected double Timer;
        public Projectile ProjectilePrefab;
        public Texture Textr;
        public Vector2 TextureScale;
        public SoundEffect DeathSoundEffect;

        public abstract void Update();
    }
}
