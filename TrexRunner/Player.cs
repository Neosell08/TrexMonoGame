using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using SharpDX.MediaFoundation;

namespace TrexRunner
{
    partial class Game1
    {
        public class Player : GameObject
        {
            
            
            public Color Color;


            public bool IsDead;
            
            public float Speed = 20;
            public bool IsAttachedToMouse;
           


            public Player(Vector2 pos, Texture2D texture)
            {
                Color = Color.White;
                Textr = texture;
                Position = pos;
               
            }

            public void Kill()
            {
                Game1.SetState(GameState.Dead);
            }

            public override void Update()
            {
                SetColliderPos();
                Color = new Color(rng.Next(0, 256), rng.Next(0, 256), rng.Next(0, 256));

                CheckedColliders = new List<Collider>();

                List<Projectile> collidingProjectiles = Game1.Projectiles.Where<Projectile>((n) => { return n is BossProjectile || n.Tags.Contains("debris"); }).ToList();
                foreach (Projectile proj in collidingProjectiles)
                {
                    if (proj.Collider != null)
                    {
                        CheckedColliders.Add(proj.Collider);
                    }
                    
                }
                CheckedColliders.Add(Game1.CurBoss.Collider);

                CheckColliders(CheckedColliders);
            }



            void SetColliderPos()
            {
                if (Collider is CircleCollider circle)
                {
                    circle.Position = Position;
                }
            }
            

            

            public override void Move(Vector2 dir)
            {
                Position += dir;
                Position = new Vector2(Math.Clamp(Position.X, 0, Game1.WindowResolution.X - (Textr.Width*TextureScale.X/2)), Math.Clamp(Position.Y, 0, Game1.WindowResolution.X - (Textr.Height * TextureScale.Y/2)));
                Collider.Position = Position;
            }
            public override void Move(Vector2 dir, float speed)
            {
                dir *= speed;
                Move(dir);
            }
            
            
           

            protected override void OnEnterCollider(Collider collider)
            {
                Kill();
                if (collider.Parent is Projectile projectile)
                {
                    Projectiles.Remove(projectile);
                }
            }
            protected override void OnUpdateCollider(Collider collider)
            {
               
            }
            protected override void OnExitCollider(Collider collider)
            {

            }

            
        }
    }
    
}
