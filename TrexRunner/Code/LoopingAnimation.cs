using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation.DirectX;

namespace SpaceShooter
{
    /// <summary>
    /// Animation which loops around frames
    /// </summary>
    public class LoopingAnimation
    {
        
        bool HasConstantSpeed;
        /// <summary>
        /// Speed of the animation on all frames
        /// </summary>
        public float ConstantSpeed;
        /// <summary>
        /// Speed of the animation on the specified frame at index
        /// </summary>
        public List<float> DynamicSpeeds;
        /// <summary>
        /// Frames of the animation
        /// </summary>
        public List<Texture2D> Frames;

        public LoopingAnimation(List<Texture2D> frames, float speed) 
        {
            Frames = frames;
            ConstantSpeed = speed;
            HasConstantSpeed = true;
        }

        public LoopingAnimation(List<Texture2D> frames, List<float> speeds)
        {
            Frames = frames;

            if (speeds.Count != frames.Count)
            {
                throw new ArgumentException("Too few speeds. Only: " + speeds.Count + ". Expected: " + frames.Count);
            }

            DynamicSpeeds = speeds;
            bool HasConstantSpeed = false;
        }

        /// <summary>
        /// Gets the frame at the specified time
        /// </summary>
        /// <param name="time">Time in seconds</param>
        /// <returns>Frame at the specified time</returns>
        /// <exception cref="InvalidOperationException">Unkown error caused: Couldn't get dynamic frame</exception>
        public Texture2D GetCurrentFrame(float time)
        {
            if (HasConstantSpeed)
            {
                return Frames[(int)Math.Floor((time*ConstantSpeed%1)*Frames.Count)];
            }
            else
            {
                time %= DynamicSpeeds.Sum();
                for (int i = 0; i < DynamicSpeeds.Count; i++)
                {
                    if (time - DynamicSpeeds[i] < 0)
                    {
                        return Frames[i];
                    }
                    else
                    {
                        time -= DynamicSpeeds[i];
                    }
                }
                throw new InvalidOperationException("Couldn't get dynamic frame");
            }

        }
    }
}
