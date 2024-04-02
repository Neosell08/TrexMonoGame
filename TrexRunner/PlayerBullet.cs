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
    internal class PlayerBullet : Projectile, IGameObject
    {
        bool ExplodesAtBorder;
        int DebrisAmount;
        Game CurGame;
        public override void Update()
        {
            Move(Velocity, Game1.Time.ElapsedGameTime.Milliseconds);
            if (Position.X < 0 || Position.Y < 0 || Position.X > Game1.ScreenResolution.X || Position.Y > Game1.ScreenResolution.Y)
            {
                if (ExplodesAtBorder)
                {
                    for (int i = 0; i < DebrisAmount; i++)
                    {
                        //add to list
                    }
                }
                //remove from projectilelist
            }
        }

        public void Move(Vector2 dir)
        {
            Position += dir;
        }

        public void Move(Vector2 dir, float speed)
        {
            dir *= speed;
            Move(dir);
        }

        public PlayerBullet(Vector2 pos, float rotation, Texture2D texture, Point textureScale, Vector2 velocity, Game curGame bool explodesAtBorder = true) 
        {
            CurTexture = texture;
            Position = pos;

            TextureScale = textureScale;   
            Velocity = velocity;
            Rotation = rotation;

            Rotation += Game1.UnsignedAngle(Vector2.Normalize(Game1.RotationToVector(Rotation)), Vector2.Normalize(velocity)); // gör så att den pekar framåt baserat på velocity
            CurGame = curGame;
        }

        
    }



    public abstract class Projectile
    {
        public Vector2 Velocity;
        Vector2 _pos;
        public float Rotation;
        public Texture2D CurTexture;
        public Point TextureScale;
        Vector2 TopLeftCorner;
        public Vector2 Position { get { return _pos; } set { _pos = value; TopLeftCorner = value - new Vector2(CurTexture.Width/2, CurTexture.Height/2); } }

        public abstract void Update();

        public void Draw(SpriteBatch spritebatch)
        {
            
            spritebatch.Draw(CurTexture, new Rectangle((int)TopLeftCorner.X, (int)TopLeftCorner.Y, TextureScale.X * CurTexture.Width, TextureScale.Y * CurTexture.Height), null, Color.White, Rotation, new Vector2(0.5f, 0.5f), SpriteEffects.None, 0);
        }

    }
}
