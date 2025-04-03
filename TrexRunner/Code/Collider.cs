using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SharpDX.MediaFoundation;
using System.CodeDom;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Xml.Linq;
using SharpDX.Direct3D9;
using SharpDX.DirectWrite;
using System.Security.Cryptography.Xml;
using System.Net.WebSockets;
using System.Text.Json.Serialization;

namespace SpaceShooter
{

    /// <summary>
    /// Abstract class to be used by all collider variants
    /// </summary>
    public abstract class Collider
    {
        /// <summary>
        /// Rotation in degrees
        /// </summary>
        public float Rotation;

        /// <summary>
        /// List of all colliders in the game
        /// </summary>
        public static List<Collider> ColliderList = new List<Collider>();

        /// <summary>
        /// 1x1 White texture for mainly used for debugging
        /// </summary>
        public static Texture2D PixelTexture = GameInstance._content.Load<Texture2D>("Resources/pixel");

        /// <summary>
        /// Position of the colliders center 
        /// </summary>
        public abstract Vector2 Position {  get; set; }

        /// <summary>
        /// Game object using this collider
        /// </summary>
        public GameObject? Parent;

        /// <summary>
        /// List of strings used for differentiating between objects of the same class
        /// </summary>
        public List<string> Tags = new List<string>();
        




        /// <summary>
        /// Checks if the point is inside of the collider
        /// </summary>
        /// <param name="pos">Position in world space</param>
        /// <returns>Whether or not the point is inside of the collider</returns>
        public abstract bool IsColliding(Vector2 pos);

        /// <summary>
        /// Checks whether or not a collider is either partially or completely inside of the collider
        /// </summary>
        /// <param name="collider">Collider to be checked</param>
        /// <returns>Whether or not the two colliders are colliding</returns>
        public abstract bool IsColliding(Collider collider);

        /// <summary>
        /// Draws the collider as a square to the spritebatch
        /// </summary>
        /// <param name="spritebatch">Spritebatch to draw to</param>
        public abstract void Draw(SpriteBatch spritebatch);

        /// <summary>
        /// Converts the colliders info to a string
        /// </summary>
        /// <returns>Collider as a string</returns>
        public override string ToString()
        {

            return $"GameObject: {this.GetType().Name}, Position: ({Position.X}, {Position.Y})";
        }

    }


    


    /// <summary>
    /// Collider of a circle with a radius and position
    /// </summary>
    public class CircleCollider : Collider
    {
        Vector2 _Position;

        /// <summary>
        /// Radius of the circle
        /// </summary>
        double Radius;

        /// <summary>
        /// Top left corner of the circle texture
        /// </summary>
        Vector2 TopLeftCorner;

        /// <summary>
        /// Position of the center of the circle
        /// </summary>
        public override Vector2 Position { get { return _Position; } set 
            {
                _Position = value;
                TopLeftCorner = new Vector2(Convert.ToInt32(Position.X - Radius), Convert.ToInt32(Position.Y - Radius));
            } }


        /// <summary>
        /// Checks if the point is inside the circle
        /// </summary>
        /// <param name="pos">Point to be checked</param>
        /// <returns>Whether or not the the point is inside the circle</returns>
        public override bool IsColliding(Vector2 pos)
        {
            double dist = Globals.Distance(Position, pos);

            if (dist <= Radius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        
        public override bool IsColliding(Collider collider)
        {
            if (collider == null) { return false; }

            if (collider is BoxCollider box)
            {
                //Not my code. This is from Stack Overflow https://stackoverflow.com/questions/401847/circle-rectangle-collision-detection-intersection

                float distX = MathF.Abs(Position.X - box.Position.X);
                float distY = MathF.Abs(Position.Y - box.Position.Y);

                if (distX > (box.Rectangle.Width / 2 + Radius)) { return false; }
                if (distX > (box.Rectangle.Height / 2 + Radius)) { return false; }
                if (distX <= (box.Rectangle.Width / 2)) { return true; }
                if (distY <= (box.Rectangle.Height / 2)) { return true; }

                float cornerDist = MathF.Pow(distX - box.Rectangle.Width / 2, 2) + MathF.Pow((distY - box.Rectangle.Height / 2), 2);

                return (cornerDist <= MathF.Pow((float)Radius, 2));
            }
            else if (collider is CircleCollider circle)
            {
                float dist = Globals.Distance(circle.Position, Position);
                if (circle.Radius + Radius >= dist)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return collider.IsColliding(this);
            }
        }
            
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos">Position of the centre of the circle</param>
        /// <param name="radius">Radius of the circle</param>
        /// <param name="parent">Parent of the collider</param>
        public CircleCollider(Vector2 pos, double radius, GameObject parent)
        {
            Collider.ColliderList.Add(this);
            Position = pos;
            Radius = radius;
            Parent = parent;
            TopLeftCorner = new Vector2(Convert.ToInt32(pos.X-radius), Convert.ToInt32(pos.Y - radius));
            
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            
            spritebatch.Draw(PixelTexture, TopLeftCorner, null, Color.Magenta, 0, Vector2.Zero, new Vector2((int)(Radius*2), (int)(Radius*2)), 0, 0);
        }
        
    }
    /// <summary>
    /// Rectangular collider 
    /// </summary>
    public class BoxCollider : Collider
    {
        /// <summary>
        /// Represents the dimensions of the collider
        /// </summary>
        public Rectangle Rectangle;
        Vector2 _Position;
        public Point TopLeftCorner;

        public override Vector2 Position { get { return _Position; } set { _Position = value; Rectangle.Location = value.ToPoint(); TopLeftCorner = value.ToPoint() - new Point(Rectangle.Width / 2, Rectangle.Height / 2); } }
        public override bool IsColliding(Vector2 pos)
        {

            if (Rectangle.Intersects(new Rectangle(Convert.ToInt32(Math.Floor(pos.X)), Convert.ToInt32(Math.Floor(pos.Y)), 0, 0)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override bool IsColliding(Collider collider)
        {
            if (collider == null) { return false; }

            if (collider is BoxCollider boxCollider)
            {
                if (Rectangle.Intersects(boxCollider.Rectangle))
                {
                    return true;
                }
                return false;
            }
            else if (collider is CircleCollider)
            {
                return collider.IsColliding(this);
            }
            else
            {
                return collider.IsColliding(this);
            }

        }
        public override void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(PixelTexture, new Vector2(Rectangle.Location.X, Rectangle.Location.Y), null, Color.Magenta, 0, Vector2.Zero, new Vector2(Rectangle.Width, Rectangle.Height), 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos">Centre of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="parent">Owner of the collider</param>
        public BoxCollider(Vector2 pos, int width, int height, GameObject parent)
        {
            Collider.ColliderList.Add(this);
            Rectangle = new Rectangle(pos.ToPoint(), new Point(height, width));
            Position = pos;
            Parent = parent;
            TopLeftCorner = pos.ToPoint() - new Point(width / 2, height / 2);
        }

    }
}
