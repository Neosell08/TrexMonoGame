using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using SharpDX.MediaFoundation;
using Microsoft.Xna.Framework.Content;

namespace TrexRunner
{
    internal class PlayerBullet : Projectile
    {
        
        bool IsDestroyed;
        double StartInvincibilityTimer;
        public static float DebrisSpeed;
        float StartRotation;


        
        public override void Update()
        {
            if (IsDestroyed) { return; }
            
            

            StartInvincibilityTimer += Game1.Time.ElapsedGameTime.TotalSeconds;
            
            Move(Velocity, Game1.Time.ElapsedGameTime.Milliseconds);
            

            if (IsOutsideOfWindow() && DeathInfo.DestroyAtBorder && StartInvincibilityTimer >= DeathInfo.StartInvincibilityTime) 
            {
                if (ShouldMakeDebris(DeathInfo.DebrisAtBorder, Position))
                {
                    for (int i = 0; i < DeathInfo.DebrisAmount; i++)
                    {
                        
                        ProjectileDeathInfo deathInfo = new ProjectileDeathInfo(true, false, false, 0, 11, 1);

                        PlayerBullet playerBullet = new PlayerBullet(Position, StartRotation, Textr, TextureScale, MathN.RotationToVector(i * (360 / DeathInfo.DebrisAmount)) * DebrisSpeed, deathInfo);
                        playerBullet.Tags.Add("debris");
                        //IsDestroyed = true;

                        Game1.Projectiles.Add(playerBullet);
                    }
                }
                Game1.Projectiles.Remove(this);
            }
        }

        
        public override void Move(Vector2 dir)
        {
            Position += dir;
            Collider.Position += dir;
        }

        public override void Move(Vector2 dir, float speed)
        {
            dir *= speed;
            Move(dir);
        }
        protected override void OnEnterCollider(Collider collider)
        {

        }
        protected override void OnExitCollider(Collider collider)
        {
           
        }
        protected override void OnUpdateCollider(Collider collider)
        {
           
        }
        
        public PlayerBullet(Vector2 pos, float rotation, Texture2D texture, Point textureScale, Vector2 velocity, ProjectileDeathInfo deathInfo)
        {
            Textr = texture;
            Position = pos;
            Collider = new CircleCollider(Position, (Textr.Height+Textr.Width)/4, this);
            

            TextureScale = textureScale;   
            Velocity = velocity;
            Rotation = rotation;
            StartRotation = rotation;
            Rotation += MathF.Atan2(velocity.Y, velocity.X); // gör så att den pekar framåt baserat på velocity

            DeathInfo = deathInfo;

        }
        
        
    }



    public abstract class Projectile: GameObject
    {
        Vector2 _velocity;
        public Vector2 Velocity;
        protected ProjectileDeathInfo DeathInfo;
        

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
        public void Remove()
        {
            Collider.ColliderList.Remove(Collider);
            Game1.Projectiles.Remove(this);
        }


        public bool IsOutsideOfWindow()
        {
           return (Position.X < 0 || Position.Y < 0 || Position.X > Game1.WindowResolution.X || Position.Y > Game1.WindowResolution.Y);
        }

        public override void Update()
        {
            Move(Velocity * Game1.Time.ElapsedGameTime.Seconds);
            
        }

        public override void Move(Vector2 dir)
        {
            base.Move(dir);
        }

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
            else if (pos.X > Game1.WindowResolution.X)
            {
                return !DeathInfo.DebrisFreeRight;
            }
            else if (pos.Y > Game1.WindowResolution.Y)
            {
                return !DeathInfo.DebrisFreeBottom;
            }

            return false;
        }

    }

    
    
}
