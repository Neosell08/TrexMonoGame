using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System.Threading.Tasks;

namespace TrexRunner
{
    internal class Boss : IGameObject
    {
        Vector2 _Position;
        Vector2 TopLeftCorner;
        Texture2D _Texture;
        public Texture2D Texture { get { return _Texture; } set { _Texture = value; Position = Position; } } //Updating Position
        Vector2 _TextureScale = new Vector2(1, 1);
        public Vector2 TextureScale { get { return _TextureScale; } set { _TextureScale = value; Position = Position; } }

        public Vector2 Position { get { return _Position; } set { _Position = value; TopLeftCorner = value - new Vector2(Texture.Width * TextureScale.X, Texture.Height * TextureScale.Y); } }
        public Boss(Vector2 pos, Texture2D texture)
        {
            Texture = texture;
            Position = pos;
        }

        public void Update()
        {

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

        public void Move(Vector2 dir, double speed)
        {
            dir = new Vector2((float)(dir.X * speed), (float)(dir.Y * speed));
            Move(dir);
        }
        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(Texture, new Rectangle(TopLeftCorner.ToPoint(), TextureScale.ToPoint()), Color.White);
        }
    }
}
