using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using static SpaceShooter.BoxCollider;

namespace SpaceShooter
{
    /// <summary>
    /// Static Math class containing several methods of calculation and logic
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// Resolution of the computer screen
        /// </summary>
        public static Vector2 ScreenResolution;
        /// <summary>
        /// Resolution of the game window
        /// </summary>
        public static Vector2 WindowResolution;


        public static GameTime Time = new GameTime();
        /// <summary>
        /// Factor to be multiplied when turning a degree into a radian value
        /// </summary>
        public const double DegreeToRadian = Math.PI / 180;
        /// <summary>
        /// Factor to be multiplied when turning a radian into a degree value
        /// </summary>
        public const double RadianToDegree = 1 / (Math.PI / 180);
        /// <summary>
        /// Calculates the distance between two points
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>Distance between the points</returns>
        public static float Distance(Vector2 p1, Vector2 p2)
        {
            return MathF.Sqrt(MathF.Pow(p2.X - p1.X, 2) + MathF.Pow(p2.Y - p1.Y, 2));
        }
        /// <summary>
        /// Calculates the distance between two points
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>Distance between the points</returns>
        public static float Distance(Point p1, Point p2)
        {
            return MathF.Sqrt(MathF.Pow(p2.X - p1.X, 2) + MathF.Pow(p2.Y - p1.Y, 2));
        }
        /// <summary>
        /// Calculates the distance between the point and (0, 0)
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <returns>Distance between the points</returns>
        public static float Distance(Vector2 p1)
        {
            return MathF.Sqrt(MathF.Pow(p1.X, 2) + MathF.Pow(p1.Y, 2));
        }
        /// <summary>
        /// Calculates the distance between the point and (0, 0)
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <returns>Distance between the points</returns>
        public static float Distance(Point p1)
        {
            return MathF.Sqrt(MathF.Pow(p1.X, 2) + MathF.Pow(p1.Y, 2));
        }
        /// <summary>
        /// Splits a string after a specified index
        /// </summary>
        /// <param name="txt">Text to be split</param>
        /// <param name="index">Index after which the text will be split</param>
        /// <returns>Split string</returns>
        /// <exception cref="IndexOutOfRangeException">index is not valid</exception>
        public static string[] SplitText(string txt, int index)
        {
            if (index >= txt.Length || index < 0) { throw new IndexOutOfRangeException("Text Index Out of Range"); }

            string s1 = txt.Substring(0, index);
            string s2 = txt.Substring(index, txt.Length - index);
            return new string[2] { s1, s2 };
        }

        /// <summary>
        /// Gets the centre of a texture
        /// </summary>
        /// <param name="texture">Texture to calculete based on</param>
        /// <param name="offset">Offset to add to the centre position</param>
        /// <param name="Scale">Scale of the texture</param>
        /// <returns></returns>
        public static Vector2 GetTextureCenter(Texture2D texture, Vector2 offset, Point Scale)
        {
            return new Vector2(offset.X + (texture.Width / 2) * Scale.X, offset.Y + (texture.Height * Scale.Y / 2));
        }
        /// <summary>
        /// Converts the a rotation into a normalized vector
        /// </summary>
        /// <param name="rotation">Rotation to convert in degrees</param>
        /// <returns>Normalised vector</returns>
        public static Vector2 RotationToVector(float rotation)
        {
            rotation = rotation * (MathF.PI / 180);
            return new Vector2(MathF.Round(MathF.Cos(rotation), 6), MathF.Round(MathF.Sin(rotation), 6));
        }
        /// <summary>
        /// Gets the angle of the line between two points
        /// </summary>
        /// <param name="from">Point 1 at the origin of the line</param>
        /// <param name="to">Point 2 at the target of the line</param>
        /// <returns>angle between the points in degrees</returns>
        public static float UnsignedAngle(Vector2 from, Vector2 to)
        {
            float dot = Vector2.Dot(from, to);
            float cross = from.X * to.Y - from.Y * to.X;
            return (float)Math.Atan2(cross, dot);
        }
        /// <summary>
        /// Clamps the value so that the value stays within the min and max range and returns the opposite of the min/max value that it exceeds
        /// </summary>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <returns>Circular clamped value</returns>
        public static float CircularClamp(float value, float min, float max)
        {
            if (value < min)
            {
                return max;
            }
            else if (value > max)
            {
                return min;
            }
            return value;
        }
        /// <summary>
        /// Clamps the value so that the value stays within the min and max range and returns the opposite of the min/max value that it exceeds
        /// </summary>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Minimum</param>
        /// <param name="max">Maximum</param>
        /// <returns>Circular clamped value</returns>
        public static int CircularClamp(int value, int min, int max)
        {
            if (value < min)
            {
                return max;
            }
            else if (value > max)
            {
                return min;
            }
            return value;
        }
        /// <summary>
        /// Calculates a linear fomula function from the points specified in the format f(x) = kx + m
        /// </summary>
        /// <param name="p1">point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>Vector where x is the k value and y is the m value</returns>
        public static Vector2 LinearFunctionFromPoints(Vector2 p1, Vector2 p2)
        {
            float k;

            if (p1.Y - p2.Y == 0) { k = 0; } // straight line along the x axis
            else if (p1.X - p2.X == 0) { return new Vector2(float.PositiveInfinity, 0f); } // straight line along the y axis
            else { k = (p1.Y - p2.Y) / (p1.X - p2.X); }

            float m = p1.Y - k * p1.X;
            return new Vector2(k, m);
        }

        /// <summary>
        /// Calculates the point where the two lines collide 
        /// </summary>
        /// <param name="l1">Line 1 where x is the slope and y is the offset</param>
        /// <param name="l2">Line 2 where x is the slope and y is the offset</param>
        /// <returns>Point at which the </returns>
        public static Vector2 LineCollision(Vector2 l1, Vector2 l2)
        {
            float x = (l2.Y - l1.Y) / (l1.X - l2.X);
            float y = l1.X * x + l1.Y;

            return new Vector2(x, y);
        }
        /// <summary>
        /// Checks whether a position is on the line
        /// </summary>
        /// <param name="l">Line, where x is the slope and y is the offset</param>
        /// <param name="p">Point in the world</param>
        /// <returns>whether or not the point is on the line</returns>
        public static bool IsOnLine(Vector2 l, Vector2 p)
        {
            return IsInsideRange(l.X * p.X + l.Y, p.Y + 0.001f, p.Y - 0.001f);
        }
        /// <summary>
        /// Checks if the value is inside the range between n1 and n2
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="n1">1D point 1 of the range</param>
        /// <param name="n2">1D point 2 of the range</param>
        /// <returns>If the value is somewhere between the two numbers</returns>
        public static bool IsInsideRange(float value, float n1, float n2)
        {
            return value >= (n1 > n2 ? n2 : n1) && value < (n1 > n2 ? n1 : n2);
        }
        /// <summary>
        /// Checks if the value is inside the range between n1 and n2
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="n1">1D point 1 of the range</param>
        /// <param name="n2">1D point 2 of the range</param>
        /// <returns>If the value is somewhere between the two numbers</returns>
        public static bool IsInsideRange(float v1, float v2, float n1, float n2)
        {
            return MathF.Min(v1, v2) <= MathF.Max(n1, n2) && MathF.Max(v1, v2) >= MathF.Min(n1, n2);
        }
        /// <summary>
        /// Invokes the linear function to get the y value at x
        /// </summary>
        /// <param name="x">x position to calculate</param>
        /// <param name="Line">line to calculate</param>
        /// <returns>Y position where x is the specified value</returns>
        public static float LinearFunction(float x, Vector2 Line)
        {
            return Line.X * x + Line.Y;
        }

        /// <summary>
        /// Gets the x value from a y value on a line
        /// </summary>
        /// <param name="y">Y position to calculate</param>
        /// <param name="line">Line to calculate</param>
        /// <returns></returns>
        public static float ReverseLinearFunction(float y, Vector2 line)
        {
            return (line.Y - y) / -line.X;
        }

        /// <summary>
        /// Rotates a vector around the origin (0, 0)
        /// </summary>
        /// <param name="vector">Vector originating at (0, 0)</param>
        /// <param name="angle">Angle in radians</param>
        /// <returns>Rotated vector</returns>
        public static Vector2 RotateVectorRad(Vector2 vector, float angle)
        {
            return new Vector2(vector.X * MathF.Cos(angle) - vector.Y * MathF.Sin(angle), vector.X * MathF.Sin(angle) + vector.Y * MathF.Cos(angle));
        }
        /// <summary>
        /// Rotates a vector around the origin (0, 0)
        /// </summary>
        /// <param name="vector">Vector originating at (0, 0)</param>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>Rotated vector</returns>
        public static Vector2 RotateVectorDeg(Vector2 vector, float angle)
        {
            return RotateVectorRad(vector, (float)(angle * DegreeToRadian));
        }
        /// <summary>
        /// Generates a random float between min and max
        /// </summary>
        /// <param name="min">Inclusive minimal return value </param>
        /// <param name="max">Exclusive maximum return value</param>
        /// <returns>A randomly generated float</returns>
        public static float RandomFloat(float min, float max)
        {
            return (float)(GameInstance.rng.NextDouble() * (max - min) + min);
        }
        /// <summary>
        /// Turns an array into a string showing all of the values inside the array
        /// </summary>
        /// <param name="a">Array to convert</param>
        /// <returns>String representation of the array</returns>
        public static string ArrayToString(Array a)
        {
            string output = "[ ";
            for (int i = 0; i < a.Length; i++)
            {

                output += a.GetValue(i).ToString() + (i == a.Length - 1 ? "" : ", ");
            }
            return output + " ]";
        }
    }
}
