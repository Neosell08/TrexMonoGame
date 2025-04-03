using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.Code
{
    /// <summary>
    /// Random Sound generator
    /// </summary>
    /// <typeparam name="T">Type of sound</typeparam>
    public class RandomSound<T> 
    {
        /// <summary>
        /// Possible sounds that the object could generate
        /// </summary>
        public List<T> PotentialSounds;

        public RandomSound(List<T> potentials) 
        {
            if (!(typeof(T)==typeof(Song)) && !(typeof(T) == typeof(SoundEffect))) { throw new ArgumentException("Type has to be a playable sound"); }
            PotentialSounds = potentials;
        }
        /// <summary>
        /// Gets a random sound from the potential sounds
        /// </summary>
        /// <returns>Sound</returns>
        public T GetSound()
        {
            return PotentialSounds[GameInstance.rng.Next(0, PotentialSounds.Count)];
        }
    }
}
