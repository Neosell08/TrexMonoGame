using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using SharpDX.MediaFoundation;

namespace SpaceShooter
{
    partial class Game1
    {
        public class Player : GameObject
        {
            
            
            public Color Color;
            

            public bool IsDead;
            
            public float Speed = 20;
            public bool IsAttachedToMouse;
            public SoundEffect DeathSound;
           

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos">Position of the centre of the player</param>
            /// <param name="texture">Texture info</param>
            /// <param name="deathSound">Sound played at death</param>
            public Player(Vector2 pos, Texture texture, SoundEffect deathSound)
            {
                Color = Color.White;
                Textr = texture;
                Position = pos;
                DeathSound = deathSound;
               
            }
            /// <summary>
            /// Kills the player
            /// </summary>
            public void Kill()
            {
                Game.SetState(GameState.Dead);
                DeathSound.Play();
            }

            public override void Update()
            {
                if (Collider is CircleCollider circle)
                {
                    circle.Position = Position;
                }
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

                foreach (LightningBossAttackObject obj in Game1.LightningObjects)
                {
                    CheckedColliders.Add(obj.Collider);
                }
  
                CheckColliders(CheckedColliders);
            }
            public override void Move(Vector2 dir)
            {
                Position += dir;
                Position = new Vector2(Math.Clamp(Position.X, 0, Game1.WindowResolution.X - (Textr.ScaledTextureScale.X/2)), Math.Clamp(Position.Y, 0, Game1.WindowResolution.X - (Textr.ScaledTextureScale.Y/2)));
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

            
        }
    }
    
}
