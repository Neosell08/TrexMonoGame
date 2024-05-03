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

namespace TrexRunner
{
    public abstract class Collider
    {
        /// <summary>
        /// Rotation in degrees
        /// </summary>
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
            double dist = MathN.Distance(Position, pos);

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

                //for (int i = 0; i < 4; i++)
                //{
                //    Vector2 p1 = box.Rectangle.Points[i];
                //    Vector2 p2 = box.Rectangle.Points[MathN.CircularClamp(i + 1, 0, 3)];
                //    Vector2 line = MathN.LinearFunctionFromPoints(p1, p2);

                //    if (IsColliding(p1) || IsColliding(p2)) { return true; }

                //    float len = MathN.Distance(p1, p2);
                //    float dot = (((Position.X - p1.X) * (p2.X - p1.X)) + ((Position.Y - p1.Y) * (p2.Y - p1.Y))) / MathF.Pow(len, 2);

                //    Vector2 closest = new Vector2(p1.X + (dot * (p2.X - p1.X)), p1.Y + (dot * (p2.Y - p1.Y)));

                //    float dist = MathN.Distance(closest, Position);
                //    if (dist <= Radius) { return true; }
                //    return false;

                for (int i = 0; i < 4; i++)
                {
                    Vector2 line = MathN.LinearFunctionFromPoints(box.Rectangle.Points[i], box.Rectangle.Points[MathN.CircularClamp(i+1, 0, 3)]);
                    float n = line.Y + Position.Y;

                    if (line.X == float.PositiveInfinity)
                    {
                        float r = MathF.Sqrt(MathF.Pow((float)Radius, 2) - MathF.Pow(box.Rectangle.Points[i].X - Position.X, 2));
                        float y1 = Position.Y + r;
                        float y2 = Position.Y - r;
                        if (MathN.IsInsideRange(y1, y2, box.Rectangle.Points[i].Y, box.Rectangle.Points[MathN.CircularClamp(i + 1, 0, 3)].Y)) { return true; }
                        continue;
                    }

                    float right = MathF.Pow((Position.X + n) / (1f + MathF.Pow(line.X, 2)), 2) - ((MathF.Pow(Position.X, 2) + MathF.Pow(n, 2) - MathF.Pow((float)Radius, 2)) / (1 + MathF.Pow(line.X, 2)));
                    if (right < 0) { continue; }

                    float left = (Position.X + n) / (1 + MathF.Pow(line.X, 2));
                    float x1 = left + MathF.Sqrt(right);
                    float x2 = left - MathF.Sqrt(right);
                    if (MathN.IsInsideRange(x1, x2, box.Rectangle.Points[i].X, box.Rectangle.Points[MathN.CircularClamp(i + 1, 0, 3)].X)) { return true; }
                }

                
            
            
            }
            else if (collider is CircleCollider circle)
            {
                float dist = MathN.Distance(circle.Position, Position);
                if (circle.Radius + Radius >= dist)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

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







    public class BoxCollider : Collider
    {
        public CustomRect Rectangle;
        Vector2 _Position;
        

        public override Vector2 Position { get { return _Position; } set { _Position = value; Rectangle.Position = value; } }
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
            spritebatch.Draw(PixelTexture, new Vector2(Rectangle.Position.X, Rectangle.Position.Y), null, Color.Magenta, Rectangle.Rotation, Vector2.Zero, new Vector2(Rectangle.Width, Rectangle.Height), 0, 0);

        }
        public void Rotate(float rotation)
        {
            Rotation += rotation;
            Rectangle.Rotation += rotation;
        }
         

        public BoxCollider(Vector2 pos, int width, int height, GameObject parent, float rotation)
        {
            Collider.ColliderList.Add(this);
            Rectangle = new CustomRect(new Vector2(Position.X - width / 2, Position.Y - height / 2), Rotation, width, height);
            Position = pos;
            Parent = parent;
            Rotation = rotation;
            Rectangle.Rotation = Rotation;


        }




        public class CustomRect
        {
            public static Texture2D Textr;
            
            public float RotationRad { get; protected set; }
            public float Rotation { get { return (float)(RotationRad * MathN.RadianToDegree); } set { RotationRad = (float)(value * MathN.DegreeToRadian); } }
            Vector2[] _points;
            public Vector2[] Points { get { 
                    for(int i = 0; i < _points.Length; i++)
                    {
                        _points[i] = MathN.RotateVectorDeg(OriginalPoints[i], Rotation)+Position;
                    }
                    return _points;
                        } private set { _points = value; } }
            Vector2[] OriginalPoints;
            Vector2 _Position;

            public float Width;
            public float Height;
            public Vector2 Position { get { return _Position; } set { _Position = value;} }

            public CustomRect(Vector2 pos, ref float rotation, float width, float height)
            {
                Rotation = rotation;

                Height = height;
                Width = width;

                OriginalPoints = new Vector2[4];
                OriginalPoints[0] = new Vector2(pos.X+width/2, pos.Y +height/2);
                OriginalPoints[1] = new Vector2(pos.X+width/2, pos.Y -height/2);
                OriginalPoints[2] = new Vector2(pos.X - width / 2, pos.Y - height / 2);
                OriginalPoints[3] = new Vector2(pos.X-width/2, pos.Y +height/2);
                Points = OriginalPoints; 
            } 

            public CustomRect(Vector2 pos, float rotation, float width, float height)
            {
                Rotation = rotation;

                Height = height;
                Width = width;

                OriginalPoints = new Vector2[4];
                OriginalPoints[0] = new Vector2(-width / 2, height / 2);
                OriginalPoints[1] = new Vector2(-width / 2, -height / 2);
                OriginalPoints[2] = new Vector2(width / 2, -height / 2);
                OriginalPoints[3] = new Vector2(width / 2, height / 2);
                Points = new Vector2[4];
                foreach (Vector2 p in Points)
                {
                    Debug.WriteLine(p);
                }
                Position = pos;
            } 
            public bool Intersects(CustomRect rect) 
            {
                for (int i = 0; i < 4; i++)
                {

                    Vector2 line1 = MathN.LinearFunctionFromPoints(Points[i], Points[MathN.CircularClamp(i+1, 0, 3)]);
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 line2 = MathN.LinearFunctionFromPoints(rect.Points[j], rect.Points[MathN.CircularClamp(j + 1, 0, 3)]);
                        Vector2 collision = MathN.LineCollision(line1, line2);

                        if (MathN.IsInsideRange(collision.X, rect.Points[j].X, rect.Points[MathN.CircularClamp(j + 1, 0, 3)].X) && MathN.IsInsideRange(collision.X, rect.Points[i].X, rect.Points[MathN.CircularClamp(i + 1, 0, 3)].X))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            public bool Intersects(Rectangle rect) 
            {

                Vector2[] points = new Vector2[4];
                points[0] = new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                points[1] = new Vector2(rect.X + rect.Width / 2, rect.Y - rect.Height / 2);
                points[2] = new Vector2(rect.X - rect.Width / 2, rect.Y - rect.Height / 2);
                points[3] = new Vector2(rect.X - rect.Width / 2, rect.Y + rect.Height / 2);
                
                for (int i = 0; i < 4; i++)
                {
                    

                    Vector2 line1 = MathN.LinearFunctionFromPoints(Points[i], Points[MathN.CircularClamp(i+1, 0, 3)]);
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 line2 = MathN.LinearFunctionFromPoints(points[j], points[MathN.CircularClamp(j + 1, 0, 3)]);
                        Vector2 collision = MathN.LineCollision(line1, line2);

                        if (MathN.IsInsideRange(collision.X, points[j].X, points[MathN.CircularClamp(j + 1, 0, 3)].X) && MathN.IsInsideRange(collision.X, Points[i].X, Points[MathN.CircularClamp(i + 1, 0, 3)].X))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            public bool Intersects(Vector2 point)
            {
                float radius = MathN.Distance(Points[0], Points[2]);

                if (!MathN.IsInsideRange(point.X, Position.X-radius, Position.X+radius) || !MathN.IsInsideRange(point.Y, Position.Y - radius, Position.Y + radius))
                {
                    return false;
                }

                Vector2[] lines = new Vector2[2];

                for (int i = 0; i < 4; i++)
                {
                    if (!MathN.IsInsideRange(point.X, Points[i].X, Points[MathN.CircularClamp(i + 1, 0, 3)].X))
                    {
                        continue;
                    }

                    Vector2 line = MathN.LinearFunctionFromPoints(Points[i], Points[MathN.CircularClamp(i + 1, 0, 3)]);

                    for (int j = 0; j < 2; j++)
                    {
                        if (lines[j] == null)
                        {
                            lines[j] = line;
                            break;
                        }
                    }
                }
                float y1 = MathN.LinearFunction(point.X, lines[0]);
                float y2 = MathN.LinearFunction(point.Y, lines[1]);
                return MathN.IsInsideRange(point.Y, y1, y2);




            }

            public void Draw(SpriteBatch spriteBatch, Color? color = null)
            {

                Point txtrScale = new Point(2, 2);

                color = color == null ? Color.White : color;

                for (int i = 0; i < 4; i++)
                {
                    Vector2 topLeftCorner = new Vector2(Points[i].X - (txtrScale.X*Textr.Width)/2, Points[i].Y - (txtrScale.Y * Textr.Height) / 2);
                    //Debug.WriteLine(i.ToString() + ": " + topLeftCorner.ToString() + " " + Points[i].ToString());

                    spriteBatch.Draw(Textr, new Rectangle((int)topLeftCorner.X, (int)topLeftCorner.Y, Convert.ToInt32(Textr.Width * txtrScale.X), Convert.ToInt32(Textr.Height * txtrScale.Y)), null, color.Value, Rotation, Vector2.Zero, SpriteEffects.None, 0);
                }
                
            }
        }
    }
    

    

    
}
