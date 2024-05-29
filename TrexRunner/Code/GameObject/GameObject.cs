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

namespace SpaceShooter
{

    /// <summary>
    /// Abstract class for representation on a graphics device and physics logic
    /// </summary>
    public abstract class GameObject
    {
        /// <summary>
        /// Texture of the game object
        /// </summary>
        Vector2 _Position;

        /// <summary>
        /// Rotation of the texture in degrees
        /// </summary>
        public float Rotation { get; set; }

        public static Game1 Game;

        public Texture Textr;

        /// <summary>
        /// The position of the center of the object
        /// </summary>
        public Vector2 Position { get { Position = _Position; return _Position; } set { _Position = value; if (Textr.Textr != null) { TopLeftCorner = value - new Vector2(Textr.Textr.Width * Textr.TextureScale.X / 2, Textr.Textr.Height * Textr.TextureScale.Y / 2); } } }

        /// <summary>
        /// This objects collider
        /// </summary>
        public Collider Collider;

        /// <summary>
        /// Built in list for checking collisions. By default checks all colliders
        /// </summary>
        public List<Collider> CheckedColliders = Collider.ColliderList;

        /// <summary>
        /// List of colliders that are currently colliding with this objects collider
        /// </summary>
        List<Collider> CollidingColliders = new List<Collider>();

        /// <summary>
        /// Top left corner of the texture in world space
        /// </summary>
        public Vector2 TopLeftCorner { get; protected set; }


        /// <summary>
        /// List of strings used to identify different objects of the same class
        /// </summary>
        public List<string> Tags = new List<string>();



        /// <summary>
        /// Has to be called every frame for the object to run per-frame logic
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Draws the objects texture to the specified spritebatch
        /// </summary>
        /// <param name="spriteBatch">Spritebatch to draw texture on</param>
        /// <param name="color">Color overlayed on the texture</param>
        public virtual void Draw(SpriteBatch spriteBatch, Color? color = null)
        {
            if (Textr.Textr == null || Textr.TextureScale == Vector2.Zero) { return; }

            color = color ?? Color.White;
            spriteBatch.Draw(Textr.Textr, new Rectangle((int)Position.X, (int)Position.Y, Convert.ToInt32(Textr.Textr.Width * Textr.TextureScale.X), Convert.ToInt32(Textr.Textr.Height * Textr.TextureScale.Y)), null, color.Value, Rotation, new Vector2(Textr.Textr.Width / 2f, Textr.Textr.Height / 2f), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Moves the object in the 2D direction of the direction parameter
        /// </summary>
        /// <param name="dir">Direction of the movement represented as a non-normalised vector</param>
        public virtual void Move(Vector2 dir)
        {
            Position += dir;

            if (Collider != null)
            {
                Collider.Position += dir;
            }
        }

        /// <summary>
        /// Moves the object in the 2D direction of the direction parameter
        /// </summary>
        /// <param name="dir">Direction of the movement represented as a non-normalised vector</param>
        /// <param name="speed">Speed to multiply the direction vector with</param>
        public virtual void Move(Vector2 dir, float speed) { Move(dir * speed); }

        /// <summary>
        /// Called when the CheckCollider method identifies a vector that has entered the objects collider
        /// </summary>
        /// <param name="collider">The collider detected in CheckColliders</param>
        /// <seealso cref="CheckColliders(Collider[])"/>
        /// <see cref="CheckColliders(List{Collider})"/>
        protected virtual void OnEnterCollider(Collider collider) { }

        /// <summary>
        /// Called every frame for every collider in the objects collider detected by 
        /// </summary>
        /// <param name="collider">Collider that is colliding with the objects collider</param>
        /// <seealso cref="CheckColliders(Collider[])"/>
        /// <see cref="CheckColliders(List{Collider})"/>
        protected virtual void OnUpdateCollider(Collider collider) { }

        /// <summary>
        /// Called the frame after a collider has left the objects collider
        /// </summary>
        /// <param name="collider">Collider that exited the objects collider</param>
        /// <seealso cref="CheckColliders(Collider[])"/>
        /// <see cref="CheckColliders(List{Collider})"/>
        protected virtual void OnExitCollider(Collider collider) { }

        /// <summary>
        /// Checks for collision with all colliders in the collider list
        /// </summary>
        /// <param name="checkedColliders">Colliders checked by this method</param>
        public void CheckColliders(List<Collider> checkedColliders)
        {
            Collider[] temp = checkedColliders.ToArray();
            foreach (Collider collider in temp)
            {
                if (collider == null) { continue; }


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

        /// <summary>
        /// Checks for collision with all colliders in the collider list
        /// </summary>
        /// <param name="checkedColliders">Colliders checked by this method</param>
        public void CheckColliders(Collider[] checkedColliders)
        {
            CheckColliders(checkedColliders.ToList());
        }

        /// <summary>
        /// Checks for collision with all colliders in the collider list
        /// </summary>
        public void CheckColliders()
        {
            CheckColliders(CheckedColliders);
        }
    }

    /// <summary>
    /// Basic Game Object that does not implenment any special functions
    /// </summary>
    public class BasicGameObject : GameObject
    {

        Action UpdateAction;
        public BasicGameObject(Vector2 pos, float rotation, Texture textr, Action updateAction = null, Collider collider = null)
        {
            Position = pos;
            Rotation = rotation;
            Textr = textr;
            UpdateAction = updateAction;
            Collider = collider;
        }
        public override void Update()
        {
            if (UpdateAction != null) { UpdateAction.Invoke(); }
            base.Update();
        }
    }
    public class TextGameObject : GameObject
    {
        public SpriteFont Font;
        public string Text;
        public Vector2 TextureScale;

        public TextGameObject(SpriteFont font, string text, Vector2 pos, Vector2 textureScale)
        {
            Font = font;
            Text = text;
            Position = pos;
            TextureScale = textureScale;
        }

        public override void Draw(SpriteBatch spriteBatch, Color? color = null)
        {
            if (Font == null || TextureScale == Vector2.Zero) { return; }

            color = color ?? Color.White;
            Vector2 size = Font.MeasureString(Text);

            spriteBatch.DrawString(Font, Text, new Vector2(Position.X - size.X/2, Position.Y - size.Y/2), color.Value, 0, Vector2.Zero, TextureScale, SpriteEffects.None, 0);
        }

    }
    public struct Texture
    {
        Texture2D _Texture = null;


        Vector2 _TextureScale = Vector2.One;
        /// <summary>
        /// Scale of the texture when drawing to the spritebatch
        /// </summary>
        public Vector2 TextureScale { get { return _TextureScale; } set { _TextureScale = value; if (Textr != null) { ScaledTextureScale = TextureScale * new Vector2(Textr.Width, Textr.Height); } else { ScaledTextureScale = Vector2.Zero; } } }
        /// <summary>
        /// Texture scale multiplied by the texture size
        /// </summary>
        public Vector2 ScaledTextureScale { get; private set; }
        public Texture2D Textr { get { return _Texture; } set { _Texture = value; if (Textr != null) { ScaledTextureScale = TextureScale * new Vector2(Textr.Width, Textr.Height); } else { ScaledTextureScale = Vector2.Zero; } } }

        public Texture(Texture2D textr, Vector2? textrScale = null)
        {
            textrScale = textrScale ?? Vector2.One;
            _Texture = textr;
            _TextureScale = textrScale.Value;
            if (textr != null && textrScale != null)
            {
                ScaledTextureScale = _TextureScale * new Vector2(_Texture.Width, _Texture.Height);
            }
            else
            {
                ScaledTextureScale = Vector2.Zero;
            }
            
        }
    }
}
