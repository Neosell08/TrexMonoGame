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
        class Player : GameObject
        {
            
            
            public Color Color;


            public bool IsDead;
            
            public float Speed = 20;

            public Collider BossCollider;

            public bool IsAttachedToMouse;
           


            public Player(Vector2 pos, Texture2D texture, Collider bossCollider)
            {
                Color = Color.White;
                Textr = texture;
                Position = pos;
               
                BossCollider = bossCollider;
            }
            public override void Update()
            {
                SetColliderPos();
                Color = new Color(rng.Next(0, 256), rng.Next(0, 256), rng.Next(0, 256));
                

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
                if (collider.Parent.Tags.Contains("debris") || collider.Parent.Tags.Contains("boss"))
                {
                    IsDead = true;

                    if (collider.Parent is Projectile projectile)
                    {
                        Game1.Projectiles.Remove(projectile);
                    }
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
    public abstract class GameObject
    {
        Vector2 _Position;
        public Texture2D Textr { get { return _Texture; } set { _Texture = value; Position = Position; } } 
        Texture2D _Texture;
        
        Point _TextureScale = new Point(1, 1);
        public float Rotation;
        public Point TextureScale { get { return _TextureScale; } set { _TextureScale = value; Position = Position; } }
        public Vector2 Position { get { return _Position; } set { _Position = value; if (Textr != null) { TopLeftCorner = value - new Vector2(Textr.Width * TextureScale.X / 2, Textr.Height * TextureScale.Y / 2); } } }
        public Collider Collider;
        public List<Collider> CheckedColliders = Collider.ColliderList;
        List<Collider> CollidingColliders = new List<Collider>();
        public Vector2 TopLeftCorner { get; protected set; }
        public List<string> Tags = new List<string>();
        public virtual void Update()
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch, Color? color = null)
        {
            if (Textr == null || TextureScale == Point.Zero) { return; }


            color = color == null ? Color.White : color;
            spriteBatch.Draw(Textr, new Rectangle((int)TopLeftCorner.X, (int)TopLeftCorner.Y, Convert.ToInt32(Textr.Width * TextureScale.X), Convert.ToInt32(Textr.Height * TextureScale.Y)), null, color.Value, Rotation, Vector2.Zero, SpriteEffects.None, 0);
            
            


        }



        public virtual void Move(Vector2 dir)
        {
            Position += dir;

            if (Collider != null)
            {
                Collider.Position += dir;
            }
        }


        public virtual void Move(Vector2 dir, float speed)
        {
            Move(dir * speed);
        }

        protected virtual void OnEnterCollider(Collider collider) { }
        protected virtual void OnUpdateCollider(Collider collider) { }
        protected virtual void OnExitCollider(Collider collider) { }
        public void CheckColliders(List<Collider> checkedColliders)
        {
            
            foreach (Collider collider in checkedColliders)
            {

                if (collider.IsColliding(Collider))
                {
                    OnUpdateCollider(collider);
                    if (!CollidingColliders.Contains(collider))
                    {
                        CollidingColliders.Add(collider);
                        OnEnterCollider(collider);

                    }
                }
                else if (CollidingColliders.Contains(collider))
                {
                    CollidingColliders.Remove(collider);
                    OnUpdateCollider(collider);
                    OnExitCollider(collider);

                }
            }
        }
        public void CheckColliders(Collider[] checkedColliders)
        {
            CheckColliders(checkedColliders.ToList());
        }
    }


    public class BasicGameObject : GameObject
    {
        public BasicGameObject(Vector2 pos, float rotation, Texture2D texture, Point textureScale)
        {
            Position = pos;
            Rotation = rotation;
            Textr = texture;
            TextureScale = textureScale;
        }
    }
    
}
