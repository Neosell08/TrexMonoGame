using SpaceShooter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpaceShooter.GameInstance;
using static TrexRunner.Code.Game1.Projectile;
using TrexRunner.Code.Game1;
using Microsoft.Xna.Framework;

namespace TrexRunner.Code.BossCode
{

    /// <summary>
    /// Projectile that is launched by the boss
    /// </summary>
    public class BossProjectile : Projectile
    {
        float Inertia;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deathInfo">Info about when the projectile is supposed to destroy itself</param>
        /// <param name="pos">Position of the centre of the projectile</param>
        /// <param name="velocity">Unnormalised velocity of the projectile. Rotation is changed according to velocity</param>
        /// <param name="rotation">Rotation offset of the projectile</param>
        /// <param name="textr">Texture info</param>
        /// <param name="inertia">Roation change</param>
        /// <param name="radius">Radius of the CircleCollider</param>
        public BossProjectile(ProjectileDeathInfo deathInfo, Vector2 pos, Vector2 velocity, float rotation, Texture textr, float inertia = 0, float radius = 0) : base()
        {
            DeathInfo = deathInfo;
            Velocity = velocity;
            Textr = textr;
            Rotation = rotation;
            Position = pos;
            Collider = new CircleCollider(pos, radius == 0 ? (Textr.ScaledTextureScale.X + Textr.ScaledTextureScale.Y) / 2 : radius, this);
            Inertia = inertia;

            Rotation += MathF.Atan2(velocity.Y, velocity.X);
        }
        protected override void OnEnterCollider(Collider collider)
        {
            if (collider.Parent is Player plr)
            {
                plr.Kill();
            }
        }
        public override void Update()
        {
            Rotation += Inertia * (float)Globals.Time.ElapsedGameTime.TotalSeconds;
            if (IsOutsideOfWindow() && DeathInfo.DestroyAtBorder)
            {
                Remove();
            }


            base.Update();
        }
    }
}
