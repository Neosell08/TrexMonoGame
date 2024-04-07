using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using SharpDX.MediaFoundation;

namespace TrexRunner
{
    internal class PlayerBullet : Projectile
    {
        bool ExplodesAtBorder;
        int DebrisAmount;
        
        public override void Update()
        {
            Move(Velocity, Game1.Time.ElapsedGameTime.Milliseconds);
            if (Position.X < 0 || Position.Y < 0 || Position.X > Game1.ScreenResolution.X || Position.Y > Game1.ScreenResolution.Y)
            {
                if (ExplodesAtBorder)
                {
                    for (int i = 0; i < DebrisAmount; i++)
                    {
                        
                    }
                }
                //remove from projectilelist
            }
        }

        public override void Move(Vector2 dir)
        {
            Position += dir;
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
        public PlayerBullet(Vector2 pos, float rotation, Texture2D texture, Point textureScale, Vector2 velocity, int debrisAmount, bool explodesAtBorder = true)
        {
            Textr = texture;
            Position = pos;

            TextureScale = textureScale;   
            Velocity = velocity;
            Rotation = rotation;

            Rotation += MathF.Atan2(Velocity.Y, velocity.X); // gör så att den pekar framåt baserat på velocity
            
            DebrisAmount = debrisAmount;
        }

        
    }



    public abstract class Projectile: GameObject
    {
        public Vector2 Velocity;
        Vector2 _pos;
        
        
        
        
        

        public override  void Update()
        {
            Move(Velocity * Game1.Time.ElapsedGameTime.Milliseconds);
        }

        

    }
    class PlayerDebris: Projectile
    {
        public override void Move(Vector2 dir)
        {

        }
        public override void Move(Vector2 dir, float speed)
        {
            
        }
        public override void Update()
        {
            
        }
        
        protected override void OnUpdateCollider(Collider collider)
        {
            
        }
        protected override void OnEnterCollider(Collider collider)
        {
            
        }
        protected override void OnExitCollider(Collider collider)
        {
            
        }

    }
}
