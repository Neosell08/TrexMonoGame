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

namespace TrexRunner
{
    public abstract class Collider
    {
        public static Texture2D PixelTexture = Game1._content.Load<Texture2D>("Resources/pixel");
        public Vector2 Position;


        public abstract bool IsColliding(Vector2 pos);

        public abstract bool IsColliding(Collider collider);

        public abstract void Draw(SpriteBatch spritebatch);

        
    }


    



    public class CircleCollider : Collider
    {
        
        public double Radius;
        
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
            if (collider is BoxCollider)
            {
                double m = collider.Position.Y - Position.Y;
                double n = collider.Position.X - Position.X;
       
                double ratio = Math.Sqrt(Math.Pow(n, 2) + Math.Pow(m, 2)) / Radius;



                Vector2 newPos = new Vector2((float)(n * ratio), (float)(m * ratio));
                
                return collider.IsColliding(newPos);
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

        public CircleCollider(Vector2 pos, double radius)
        {
            Position = pos;
            Radius = radius;
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            
            spritebatch.Draw(PixelTexture, Position, null, Color.Magenta, 0, Vector2.Zero, new Vector2((int)(Radius*2), (int)(Radius*2)), 0, 0);
        }

    }
    public class BoxCollider : Collider
    {
        Rectangle Rectangle = new Rectangle();
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
            spritebatch.Draw(PixelTexture, Position, null, Color.Magenta, 0, Vector2.Zero, new Vector2(Rectangle.Width, Rectangle.Height), 0, 0);
        }
    }
    

    

    
}
