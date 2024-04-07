using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TrexRunner
{
    internal class Boss : GameObject
    {
        public Boss(Vector2 pos, Texture2D texture, Point texturescale)
        {
            Textr = texture;
            Position = pos;
            TextureScale = texturescale;
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
        public override void Update()
        {

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

        public void Move(Vector2 dir, double speed)
        {
            dir = new Vector2((float)(dir.X * speed), (float)(dir.Y * speed));
            Move(dir);
        }
        
    }
}
