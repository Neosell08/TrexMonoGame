using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SpaceShooter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Texture = TrexRunner.Code.Game1.Texture;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TrexRunner.Code.BossCode;

namespace TrexRunner.Code.BossCode
{
    /// <summary>
    /// Lightning that strikes above the player
    /// </summary>
    public class LightningBossAttackObject : GameObject
    {
        public static List<LightningBossAttackObject> LightningObjects = new List<LightningBossAttackObject>();
        public LoopingAnimation Anim;
        public float DeathTime;
        float DeathTimer;
        float WarnTimer;
        float WarnTime;
        public bool IsWarning = true;
        SoundEffect LightningSound;
        SoundEffect WarnSound;
        Texture WarnTexture;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="frames">Frames of the lightning animation</param>
        /// <param name="animationSpeed">Speed of the animation</param>
        /// <param name="textr">Texture info</param>
        /// <param name="top">Top of the lightning Texture</param>
        /// <param name="deathTime">Time at which the object will destroy itself</param>
        /// <param name="warnTime">amount of time the warning sign is on screen</param>
        /// <param name="warnTexture">Warning sign texture</param>
        public LightningBossAttackObject(Texture2D[] frames, float animationSpeed, Texture textr, Vector2 top, float deathTime, float warnTime, Texture warnTexture, SoundEffect ligthningSound, SoundEffect warningSound)
        {
            Textr = textr;
            Textr.Textr = frames[0];
            Position = top + new Vector2(0, Textr.ScaledTextureScale.Y / 2);
            DeathTime = deathTime;
            WarnTime = warnTime;
            WarnTexture = warnTexture;
            LightningSound = ligthningSound;
            WarnSound = warningSound;
            WarnSound.Play(0.5f, 1, 1);

            Anim = new LoopingAnimation(frames.ToList(), animationSpeed);
            Collider = null;

            LightningObjects.Add(this);
        }
        public override void Update()
        {
            if (!IsWarning)
            {
                DeathTimer += (float)Globals.Time.ElapsedGameTime.TotalSeconds;
                Textr.Textr = Anim.GetCurrentFrame((float)Globals.Time.TotalGameTime.TotalSeconds);
            }


            WarnTimer += (float)Globals.Time.ElapsedGameTime.TotalSeconds;

            if (WarnTimer >= WarnTime && IsWarning)
            {
                LightningSound.Play();
                IsWarning = false;
                Collider = new BoxCollider(Position, (int)Textr.ScaledTextureScale.X, (int)Textr.ScaledTextureScale.Y, this);
            }

            if (DeathTimer >= DeathTime)
            {
                Dispose();
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Color? color = null)
        {
            if (!IsWarning)
            {
                base.Draw(spriteBatch, color);
            }
            else
            {
                color = color ?? Color.White;
                if (Textr.Textr != null)
                {

                    spriteBatch.Draw(WarnTexture.Textr, new Rectangle((int)Position.X, (int)WarnTexture.ScaledTextureScale.Y, (int)WarnTexture.ScaledTextureScale.X, (int)WarnTexture.ScaledTextureScale.Y), null, color.Value, Rotation, new Vector2(WarnTexture.Textr.Width / 2f, WarnTexture.Textr.Height / 2f), SpriteEffects.None, 0);
                }
            }

        }
        /// <summary>
        /// Sets up the object to be collected by the garbage collector
        /// </summary>
        public void Dispose()
        {
            LightningObjects.Remove(this);
            Collider.ColliderList.Remove(Collider);
        }
    }
}
