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
    [Obsolete]
    class MovingBackground : GameObject
    {
        
        List<Vector2> VisibleBackgrounds;
        
        float backgroundPosX;
        float backgroundPosY;
        
        
        public override void Update()
        {
            backgroundPosX = -((Position.X - Mod(Position.X, 1920)) / 1920);
            backgroundPosY = -((Position.Y - Mod(Position.Y, 1080)) / 1080); //I don't know what this is. Beppe's code

            VisibleBackgrounds[0] = new Vector2(backgroundPosX, backgroundPosY);
            VisibleBackgrounds[1] = new Vector2(backgroundPosX - 1, backgroundPosY);
            VisibleBackgrounds[2] = new Vector2(backgroundPosX, backgroundPosY - 1);
            VisibleBackgrounds[3] = new Vector2(backgroundPosX - 1, backgroundPosY - 1);
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

        public override void Draw(SpriteBatch spritebatch, Color? color)
        {
            for (int i = 0; i < 4; i++)
            {
                spritebatch.Draw(Textr, new Vector2(Position.X + VisibleBackgrounds[i].X * Game1.WindowResolution.X, Position.Y + VisibleBackgrounds[i].Y * Game1.WindowResolution.Y), Color.White);
            }
        }
        private static float Mod(float x, float m)
        {
            return ((x % m) + m) % m;
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
        public MovingBackground(Texture2D texture)
        {
            Textr = texture;
            
            
        }
    }
}
