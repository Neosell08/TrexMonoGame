using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;

namespace TrexRunner
{
    partial class Game1
    {
        class Player : IGameObject
        {
            public Texture2D Texture;
            public Vector2 TextureScale = new Vector2(1, 1);
            public Color Color;
            public Vector2 Position;
            public float Speed = 20;
            public Collider Collider;
            public bool DrawCollider;

            
            public Player(Vector2 pos)
            {
                Color = Color.White;
                Position = pos;
            }
            public void Update()
            {
                Color = new Color(rng.Next(0, 256), rng.Next(0, 256), rng.Next(0, 256));
            }
            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(Texture, Position, null, Color, 0, Game1.GetTextureCenter(Texture, Vector2.Zero, null), TextureScale, SpriteEffects.None, 0);
                if (DrawCollider)
                {
                    Collider.Draw(spriteBatch);
                }
            }

            public void Move(Vector2 dir)
            {
                Position += dir;
                Collider.Position += dir;
            }
            public void Move(Vector2 dir, float speed)
            {
                dir *= speed;
                Move(dir);
            }

            public void Move(Vector2 dir, double speed)
            {
                dir = new Vector2((float)(dir.X * speed), (float)(dir.Y * speed));
                Move(dir);
            }


            public void SetPos(Vector2 target)
            {
                Move(target - Position);
            }

            
        }
    }
    interface IGameObject
    {
        public void Update();

        public void Draw(SpriteBatch spriteBatch);

        public void Move(Vector2 dir);

        public void Move(Vector2 dir, float speed);

        public void Move(Vector2 dir, double speed);

        public void SetPos(Vector2 target);
    }
    
}
