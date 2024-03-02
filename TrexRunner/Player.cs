using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TrexRunner
{
    partial class Game1
    {
        class Player : IGameObject
        {
            public Texture2D Texture;
            public Color Color;
            public Vector2 Position;
            public float Speed = 20; 


            Random rng = new Random();
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
                spriteBatch.Draw(Texture, Position, Color);
            }
           
           
        }
    }
    interface IGameObject
    {
        void Update();

        void Draw(SpriteBatch spriteBatch);
    }
}
