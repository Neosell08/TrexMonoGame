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
        Projectile.ProjectileDeathInfo DeathInfo;
        bool IsDestroyed;
        double StartInvincibilityTimer;
        public static float DebrisSpeed;
        float StartRotation;


        
        public override void Update()
        {
            if (IsDestroyed) { return; }
            
            

            StartInvincibilityTimer += Game1.Time.ElapsedGameTime.TotalSeconds;
            
            Move(Velocity, Game1.Time.ElapsedGameTime.Milliseconds);
            bool isOutsideOfWindow = (Position.X < 0 || Position.Y < 0 || Position.X > Game1.WindowResolution.X || Position.Y > Game1.WindowResolution.Y);

            if (isOutsideOfWindow && DeathInfo.DestroyAtBorder && StartInvincibilityTimer >= DeathInfo.StartInvincibilityTime) 
            {
                if (ShouldMakeDebris(DeathInfo.DebrisAtBorder, Position))
                {
                    for (int i = 0; i < DeathInfo.DebrisAmount; i++)
                    {
                        Debug.WriteLine(i * (360 / DeathInfo.DebrisAmount));
                        ProjectileDeathInfo deathInfo = new Projectile.ProjectileDeathInfo(true, false, false, 0, 11, 1);

                        PlayerBullet playerBullet = new PlayerBullet(Position, StartRotation, Textr, TextureScale, Game1.RotationToVector(i * (360 / DeathInfo.DebrisAmount)) * DebrisSpeed, deathInfo);
                        playerBullet.Tags.Add("debris");
                        //IsDestroyed = true;

                        Game1.Projectiles.Add(playerBullet);
                    }
                }
                Game1.Projectiles.Remove(this);
            }
        }

        bool ShouldMakeDebris(bool debrisAtBorder, Vector2 pos) 
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
            throw new NotImplementedException();
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
            Rotation += MathF.Atan2(Velocity.Y, velocity.X); // gör så att den pekar framåt baserat på velocity

            DeathInfo = deathInfo;

        }
        
        
    }



    public abstract class Projectile: GameObject
    {
        public Vector2 Velocity;
        Vector2 _pos;
        

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




        public override  void Update()
        {
            Move(Velocity * Game1.Time.ElapsedGameTime.Seconds);
            
        }

        public override void Move(Vector2 dir)
        {
            base.Move(dir);
        }



    }

    public class BossBullet : Projectile
    {
        public override void Update()
        {
            Move(Velocity);

            
        }
    }
    
}
