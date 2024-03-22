using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrexRunner
{
    
    class Background : IGameObject
    {
        Texture2D Texture;
        List<Vector2> VisibleBackgrounds;
        Vector2 _Position;
        float backgroundPosX;
        float backgroundPosY;
        public Vector2 Position { get { return _Position; } set { _Position = value; } }
        GameWindow Window;
        public void Update()
        {
            backgroundPosX = -((Position.X - Mod(Position.X, 1920)) / 1920);
            backgroundPosY = -((Position.Y - Mod(Position.Y, 1080)) / 1080); //I don't know what this is. Beppe's code

            VisibleBackgrounds[0] = new Vector2(backgroundPosX, backgroundPosY);
            VisibleBackgrounds[1] = new Vector2(backgroundPosX - 1, backgroundPosY);
            VisibleBackgrounds[2] = new Vector2(backgroundPosX, backgroundPosY - 1);
            VisibleBackgrounds[3] = new Vector2(backgroundPosX - 1, backgroundPosY - 1);
        }
        

        
        public void Draw(SpriteBatch spritebatch)
        {
            for (int i = 0; i < 4; i++)
            {
                spritebatch.Draw(Texture, new Vector2(Position.X + VisibleBackgrounds[i].X * Window.ClientBounds.Width, Position.Y + VisibleBackgrounds[i].Y * Window.ClientBounds.Height), Color.White);
            }
        }
        private static float Mod(float x, float m)
        {
            return ((x % m) + m) % m;
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
        public Background(Texture2D texture, GameWindow window)
        {
            Texture = texture;
            Window = window;
            
        }
    }
}
