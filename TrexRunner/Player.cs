using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Texture2D _Texture;
            public Texture2D Texture { get { return _Texture; } set { _Texture = value; Position = Position; } } //Updating Position
            Vector2 _TextureScale = new Vector2(1, 1);
            public Vector2 TextureScale { get { return _TextureScale; } set { _TextureScale = value; Position = Position; } } 
            public Color Color;
            public Vector2 TopLeftCorner;
            Vector2 _Position;
            
            public Vector2 Position { get { return _Position; } set { _Position = value; TopLeftCorner = value - new Vector2(Texture.Width * TextureScale.X/2, Texture.Height * TextureScale.Y/2); } }


            public float Speed = 20;
            public Collider Collider;
            public bool DrawCollider;
            public GameWindow Window;

            public bool IsAttachedToMouse;
            public List<Collider> CheckedColliders = new List<Collider>();
            List<Collider> CollidingColliders = new List<Collider>(); // colliders that are currently colliding with players collider


            public Player(Vector2 pos, GameWindow window, Texture2D texture)
            {
                Color = Color.White;
                Texture = texture;
                Position = pos;
                Window = window;
            }
            public void Update()
            {
                SetColliderPos();
                Color = new Color(rng.Next(0, 256), rng.Next(0, 256), rng.Next(0, 256));
                CheckColliders();
            }



            void SetColliderPos()
            {
                if (Collider is CircleCollider circle)
                {
                    circle.Position = Position;
                }
            }
            void CheckColliders()
            {
                foreach (Collider collider in CheckedColliders)
                {

                        if (collider.IsColliding(Collider))
                    {
                        OnUpdateCollider(collider);
                        if(!CollidingColliders.Contains(collider))
                        {
                            CollidingColliders.Add(collider);
                            OnEnterCollider(collider);
                            
                        }
                    }
                    else if(CollidingColliders.Contains(collider))
                    {
                        CollidingColliders.Remove(collider);
                        OnUpdateCollider(collider);
                        OnExitCollider(collider);

                    }
                }
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                
                spriteBatch.Draw(Texture, new Rectangle((int)TopLeftCorner.X, (int)TopLeftCorner.Y, Convert.ToInt32(Texture.Width * TextureScale.X), Convert.ToInt32(Texture.Height * TextureScale.Y)), Color.White);
                if (DrawCollider)
                {
                    Collider.Draw(spriteBatch);
                }
            }

            public void Move(Vector2 dir)
            {
                Position += dir;
                Position = new Vector2(Math.Clamp(Position.X, 0, Window.ClientBounds.Width - Texture.Width*TextureScale.X), Math.Clamp(Position.Y, 0, Window.ClientBounds.Height - Texture.Height * TextureScale.Y));
                Collider.Position = Position;
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

            void OnEnterCollider(Collider collider)
            {
                
                
            }
            void OnUpdateCollider(Collider collider)
            {
                Debug.WriteLine(collider);
                Debug.WriteLine(Game1.Distance(Collider.Position, collider.Position));
            }
            void OnExitCollider(Collider collider)
            {

            }
        }
    }
    interface IGameObject
    {
        
        public void Update();

        public void Draw(SpriteBatch spriteBatch);

        public void Move(Vector2 dir);

        public void Move(Vector2 dir, float speed);

        void OnEnterCollider(Collider collider)
        {

        }
        void OnUpdateCollider(Collider collider)
        {

        }
        void OnExitCollider(Collider collider)
        {

        }
        public void CheckColliders()
        {

        }
    }
    
}
