using Microsoft.Xna.Framework.Audio;
using SpaceShooter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpaceShooter.GameInstance;
using TrexRunner.Code.Game1;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Texture = TrexRunner.Code.Game1.Texture;

namespace TrexRunner.Code.BossCode
{

    /// <summary>
    /// Attack that launches projectiles in a circle around the boss and strikes lightning rays above the player
    /// </summary>
    public class SphereAndLightningAttack : BossAttackPattern
    {
        int BulletCount;
        float Inertia;
        float BulletRadius;
        float LightningTimer;
        float LightningDelay;
        Texture2D[] LightningFrames;
        Vector2 LightningTextureScale;
        float LightningAnimationSpeed;
        float LightningDeathTime;
        Player _Player;
        Texture WarnTexture;
        public SoundEffect LightningSound;
        public SoundEffect WarningSound;

        public override void Update()
        {
            Timer += Globals.Time.ElapsedGameTime.TotalSeconds;
            LightningTimer += (float)Globals.Time.ElapsedGameTime.TotalSeconds;

            if (Timer >= CurDelay)
            {
                for (int i = 0; i < BulletCount; i++)
                {
                    Projectile.ProjectileDeathInfo death = new Projectile.ProjectileDeathInfo(true, false, false, 0, 0, 1);

                    BossProjectile prj = new BossProjectile(death, Parent.Position, Globals.RotationToVector(i * (360 / BulletCount)) * ProjectileSpeed, 0, Textr, Inertia, BulletRadius);


                }
                CurDelay = Globals.RandomFloat(ShootDelays.X, ShootDelays.Y);
                Timer = 0;
            }

            if (LightningTimer >= LightningDelay)
            {
                new LightningBossAttackObject(LightningFrames, LightningAnimationSpeed, new Texture(null, new Vector2(2, Globals.WindowResolution.Y / Textr.Textr.Height)), new Vector2(_Player.Position.X, 0), LightningDeathTime, 2, WarnTexture, LightningSound, WarningSound);
                LightningTimer = 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shootDelays">Min and max of the delay at which the boss will shoot</param>
        /// <param name="bulletCount">Amount of bullets at each shooting action</param>
        /// <param name="bulletSpeed">Speed of the bullets</param>
        /// <param name="inertia">Inertia of the bullets</param>
        /// <param name="parent">Boss that contains this AttackPattern</param>
        /// <param name="textr">Texture info of the bullets</param>
        /// <param name="radius">Radius of the bullet CircleCollider</param>
        /// <param name="lightningdelay">Delay of when the boss strikes lightning</param>
        /// <param name="lightningAnimationSpeed">Speed of the Lightnings animation</param>
        /// <param name="lightningDeathTime">Time at which the lightning disappears</param>
        /// <param name="lightningFrames">Frames of the lightning animation</param>
        /// <param name="player">Player in the game</param>
        /// <param name="warnTexture">Texture info of the warning sign before the lightning</param>
        public SphereAndLightningAttack(Vector2 shootDelays, float lightningdelay, int bulletCount, float bulletSpeed, float inertia, Boss parent, Texture textr, Texture2D[] lightningFrames, float lightningAnimationSpeed, float lightningDeathTime, Player player, Texture warnTexture, SoundEffect lightningSound, SoundEffect warningSound, float radius = 0)
        {

            ShootDelays = shootDelays;
            CurDelay = Globals.RandomFloat(shootDelays.X, shootDelays.Y);
            BulletCount = bulletCount;
            Parent = parent;
            Textr = textr;
            ProjectileSpeed = bulletSpeed;
            Inertia = inertia;
            BulletRadius = radius;
            LightningFrames = lightningFrames;
            LightningDelay = lightningdelay;
            LightningAnimationSpeed = lightningAnimationSpeed;
            LightningDeathTime = lightningDeathTime;
            LightningSound = lightningSound;
            WarningSound = warningSound;
            _Player = player;
            WarnTexture = warnTexture;
        }
    }
}

