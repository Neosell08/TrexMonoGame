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
            Random rng = new Random();
            public Player()
            {
                Color = Color.White;
            }
            public void Update()
            {
                Color = new Color(rng.Next(0, 256), rng.Next(0, 256), rng.Next(0, 256));
            }
            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(Texture, new Vector2(0, 0), Color);
            }
           
           
        }
    }
    interface IGameObject
    {
        void Update();

        void Draw(SpriteBatch spriteBatch);
    }
}
