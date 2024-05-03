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
    public abstract class GameObject
    {
        Vector2 _Position;
        public Texture2D Textr { get { return _Texture; } set { _Texture = value; Position = Position; } }
        Texture2D _Texture;

        Point _TextureScale = new Point(1, 1);
        public float Rotation { get; set; }
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
            spriteBatch.Draw(Textr, new Rectangle((int)Position.X, (int)Position.Y, Convert.ToInt32(Textr.Width * TextureScale.X), Convert.ToInt32(Textr.Height * TextureScale.Y)), null, color.Value, Rotation, new Vector2(Textr.Width / 2f, Textr.Height / 2f), SpriteEffects.None, 0);




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
            Collider[] temp = checkedColliders.ToArray();
            foreach (Collider collider in temp)
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
