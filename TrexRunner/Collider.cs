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

namespace TrexRunner
{
    public abstract class Collider
    {

        public float Rotation;

        public static List<Collider> ColliderList = new List<Collider>();

        public static Texture2D PixelTexture = Game1._content.Load<Texture2D>("Resources/pixel");
        public abstract Vector2 Position {  get; set; }


        public GameObject? Parent;

        public List<string> tags = new List<string>();
        


        public abstract bool IsColliding(Vector2 pos);

        public abstract bool IsColliding(Collider collider);

        public abstract void Draw(SpriteBatch spritebatch);

        public override string ToString()
        {

            return $"GameObject: {this.GetType().Name}, Position: ({Position.X}, {Position.Y})";
        }

    }


    



    public class CircleCollider : Collider
    {
        Vector2 _Position;
        double Radius;
        Vector2 TopLeftCorner;
        public override Vector2 Position { get { return _Position; } set 
            {
                _Position = value;
                TopLeftCorner = new Vector2(Convert.ToInt32(Position.X - Radius), Convert.ToInt32(Position.Y - Radius));
            } }

        public override bool IsColliding(Vector2 pos)
        {
            double dist = Game1.Distance(Position, pos);

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
            if (collider is BoxCollider box)
            {
                float x = Math.Max(box.Rectangle.Left, Math.Min(Position.X, box.Rectangle.Right));
                float y = Math.Max(box.Rectangle.Top, Math.Min(Position.Y, box.Rectangle.Bottom));
                
                
                return IsColliding(new Vector2(x, y));
            }
            else if (collider is CircleCollider circle)
            {

                double dist = Game1.Distance(circle.Position, Position);
                
                if (circle.Radius + Radius > dist)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public CircleCollider(Vector2 pos, double radius, GameObject parent)
        {
            Position = pos;
            Radius = radius;
            Parent = parent;
            TopLeftCorner = new Vector2(Convert.ToInt32(pos.X-radius), Convert.ToInt32(pos.Y - radius));
            Collider.ColliderList.Add(this);
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            
            spritebatch.Draw(PixelTexture, TopLeftCorner, null, Color.Magenta, 0, Vector2.Zero, new Vector2((int)(Radius*2), (int)(Radius*2)), 0, 0);
        }
        
    }







    public class BoxCollider : Collider
    {
        public Rectangle Rectangle = new Rectangle();
        Vector2 _Position;
        

        public override Vector2 Position { get { return _Position; } set { _Position = value; Rectangle.X = (int)value.X; Rectangle.Y = (int)value.Y; } }
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
            return false;
        }
        public override void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(PixelTexture, new Vector2(Rectangle.X, Rectangle.Y), null, Color.Magenta, 0, Vector2.Zero, new Vector2(Rectangle.Width, Rectangle.Height), 0, 0);

        }
         

        public BoxCollider(Vector2 pos, int width, int height, GameObject parent)
        {
            Rectangle = new Rectangle((int)Position.X - width / 2, (int)Position.Y - height / 2, width, height);
            Position = pos;
            Parent = parent;
            Collider.ColliderList.Add(this);
        }




        public class CustomRect
        {
            float Rotation;
            Vector2[] Points;
            Vector2 _Position;
            public Vector2 Position { get { return _Position; } set { _Position = value;} }

            public CustomRect(Vector2 pos, float rotation, float width, float height)
            {
                Rotation = rotation;

                Points = new Vector2[4];
                Points[0] = new Vector2(pos.X+width/2, pos.Y +height/2);
                Points[1] = new Vector2(pos.X+width/2, pos.Y -height/2);
                Points[2] = new Vector2(pos.X - width / 2, pos.Y - height / 2);
                Points[3] = new Vector2(pos.X-width/2, pos.Y +height/2);
            } 
            public bool Intersects()
        }
    }
    

    

    
}
