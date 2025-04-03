using Microsoft.Xna.Framework;
using SpaceShooter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrexRunner.Code.Game1
{
    public abstract class Projectile : GameObject
    {
        Vector2 _velocity;
        /// <summary>
        /// Velocity of the projectile
        /// </summary>
        public Vector2 Velocity;
        /// <summary>
        /// Info of when the projectile should be destroyed
        /// </summary>
        protected ProjectileDeathInfo DeathInfo;
        public static HashSet<Projectile> Projectiles = new HashSet<Projectile>();

        public Projectile()
        {
            Projectiles.Add(this);
        }

        /// <summary>
        /// Contains info about when a projectile should be destroyed
        /// </summary>
        public struct ProjectileDeathInfo
        {
            public bool DestroyAtBorder;
            public bool DestroyAfterTime;
            public bool DebrisAtBorder;
            public float DeathTime;
            public int DebrisAmount;
            public float StartInvincibilityTime;
            public bool DebrisFreeRight = false;
            public bool DebrisFreeLeft = false;
            public bool DebrisFreeTop = false;
            public bool DebrisFreeBottom = false;
            /// <summary>
            /// Sets the sides at which the projectile will not create debris
            /// </summary>
            /// <param name="right"></param>
            /// <param name="left"></param>
            /// <param name="top"></param>
            /// <param name="bottom"></param>
            /// <returns>new info object with the specified changes applied</returns>
            public ProjectileDeathInfo SetDebrisFreeSides(bool right = false, bool left = false, bool top = false, bool bottom = false)
            {
                DebrisFreeRight = right;
                DebrisFreeLeft = left;
                DebrisFreeTop = top;
                DebrisFreeBottom = bottom;
                return this;
            }


            public ProjectileDeathInfo(bool destroyAtBorder, bool destroyAfterTime, bool debrisAtBorder, float deathTime, int debrisAmount, float startInvincibiltyTime)
            {
                DestroyAfterTime = destroyAfterTime;
                DestroyAtBorder = destroyAtBorder;
                DeathTime = deathTime;
                DebrisAmount = debrisAmount;
                DebrisAtBorder = debrisAtBorder;
                StartInvincibilityTime = startInvincibiltyTime;

            }


        }
        /// <summary>
        /// Set up the projectile for garbage collection
        /// </summary>
        public void Remove()
        {
            Collider.ColliderList.Remove(Collider);
            Projectile.Projectiles.Remove(this);
        }

        /// <summary>
        /// Checks whether or not the projectile is outside of the application window
        /// </summary>
        /// <returns></returns>
        public bool IsOutsideOfWindow()
        {
            return (Position.X < 0 || Position.Y < 0 || Position.X > Globals.WindowResolution.X || Position.Y > Globals.WindowResolution.Y);
        }

        public override void Update()
        {
            Move(Velocity * (float)Globals.Time.ElapsedGameTime.TotalSeconds);

        }

        public override void Move(Vector2 dir)
        {
            base.Move(dir);
        }
        /// <summary>
        /// Checks if the side closest to the player is debris free
        /// </summary>
        /// <param name="debrisAtBorder">if it should make debris at the border</param>
        /// <param name="pos">Projectiles position</param>
        /// <returns>If the closest side is not debris free</returns>
        protected bool ShouldMakeDebris(bool debrisAtBorder, Vector2 pos)
        {
            if (!debrisAtBorder) { return false; }

            if (pos.X < 0)
            {
                return !DeathInfo.DebrisFreeLeft;
            }
            else if (pos.Y < 0)
            {
                return !DeathInfo.DebrisFreeTop;
            }
            else if (pos.X > Globals.WindowResolution.X)
            {
                return !DeathInfo.DebrisFreeRight;
            }
            else if (pos.Y > Globals.WindowResolution.Y)
            {
                return !DeathInfo.DebrisFreeBottom;
            }

            return false;
        }

    }
}
