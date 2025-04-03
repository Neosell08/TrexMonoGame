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
using TrexRunner.Code.Game1;
using Texture = TrexRunner.Code.Game1.Texture;

namespace SpaceShooter
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
            
            

            StartInvincibilityTimer += Globals.Time.ElapsedGameTime.TotalSeconds;
            
            Move(Velocity, Globals.Time.ElapsedGameTime.Milliseconds);
            

            if (IsOutsideOfWindow() && DeathInfo.DestroyAtBorder && StartInvincibilityTimer >= DeathInfo.StartInvincibilityTime) 
            {
                if (ShouldMakeDebris(DeathInfo.DebrisAtBorder, Position))
                {
                    for (int i = 0; i < DeathInfo.DebrisAmount; i++)
                    {
                        
                        ProjectileDeathInfo deathInfo = new ProjectileDeathInfo(true, false, false, 0, 11, 1);

                        PlayerBullet playerBullet = new PlayerBullet(Position, StartRotation, new Texture(Textr.Textr, Textr.TextureScale), Globals.RotationToVector(i * (360 / DeathInfo.DebrisAmount)) * DebrisSpeed, deathInfo);
                        playerBullet.Tags.Add("debris");
                        //IsDestroyed = true;

                        Projectile.Projectiles.Add(playerBullet);
                    }
                }
                Projectile.Projectiles.Remove(this);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos">Position of the centre</param>
        /// <param name="rotation">Rotation of the texture</param>
        /// <param name="texture">Texture info</param>
        /// <param name="velocity">Velocity of the projectile</param>
        /// <param name="deathInfo">info about when the bullet should be destroyed</param>
        public PlayerBullet(Vector2 pos, float rotation, Texture texture, Vector2 velocity, ProjectileDeathInfo deathInfo) : base()
        {
            
            Textr = texture;
            Position = pos;
            Collider = new CircleCollider(Position, (Textr.Textr.Height+Textr.Textr.Width)/4, this);
             
            Velocity = velocity;
            Rotation = rotation;
            StartRotation = rotation;
            Rotation += MathF.Atan2(velocity.Y, velocity.X); // Rotates based on velocity

            DeathInfo = deathInfo;

        }
        
        
    }



    

    
    
}
