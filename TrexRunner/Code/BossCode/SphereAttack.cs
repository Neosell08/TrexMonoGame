using Microsoft.Xna.Framework;
using SpaceShooter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrexRunner.Code.Game1;

namespace TrexRunner.Code.BossCode
{



    /// <summary>
    /// Attack that launches projectiles in a circle around the boss
    /// </summary>
    public class SphereAttack : BossAttackPattern
    {
        int BulletCount;
        float Inertia;
        float BulletRadius;

        public override void Update()
        {
            Timer += Globals.Time.ElapsedGameTime.TotalSeconds;

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
        public SphereAttack(Vector2 shootDelays, int bulletCount, float bulletSpeed, float inertia, Boss parent, Texture textr, float radius = 0)
        {
            ShootDelays = shootDelays;
            CurDelay = shootDelays.X;
            BulletCount = bulletCount;
            Parent = parent;
            Textr = textr;
            ProjectileSpeed = bulletSpeed;
            Inertia = inertia;
            BulletRadius = radius;
        }
    }
}
