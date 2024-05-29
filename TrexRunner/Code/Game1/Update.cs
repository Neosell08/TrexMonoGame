using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace SpaceShooter
{
    public partial class Game1
    {
        protected override void Update(GameTime gameTime)
        {
            Time = gameTime;
            if (CurState == GameState.MainMenu)
            {
                Level1Icon.Update();
            }
            else if (CurState == GameState.Countdown)
            {
                CountdownTimer -= (float)Time.ElapsedGameTime.TotalSeconds;
                CountdownText.Text = MathF.Floor(CountdownTimer+1).ToString();

                if (CountdownTimer <=0)
                {
                    SetState(GameState.Playing);
                }
            }
            else
            {
                
                
                if (CurState == GameState.Playing)
                {
                    InputCheck();
                    PlayingTime += (float)Time.ElapsedGameTime.TotalSeconds;
                }


                // TODO: Add your update logic here
                if (CurState == GameState.Playing)
                {
                    player.Update();
                }

                Projectile[] projectilesToBeUpdated = Projectiles.ToArray();

                foreach (Projectile projectile in projectilesToBeUpdated)
                {
                    projectile.Update();
                }
                LightningBossAttackObject[] lightningsToBeUpdated = LightningObjects.ToArray();
                foreach (LightningBossAttackObject lightning in lightningsToBeUpdated)
                {
                    lightning.Update();
                }

                CurBoss.Update();


                LastMousePos = Mouse.GetState().Position.ToVector2();

                if (CurState == GameState.Dead || CurState == GameState.Won)
                {
                    ExitButton.Update();
                    PlayButton.Update();
                }
                
            }




            base.Update(gameTime);
        }

        /// <summary>
        /// Runs input logic
        /// </summary>
        void InputCheck()
        {
            MouseState mouse = Mouse.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (mouse.LeftButton == ButtonState.Released)
            {
                player.IsAttachedToMouse = false;
            }
            if (RightMousePressed && MathN.Distance(LastMousePos, mouse.Position.ToVector2()) >= ShootVelocityTreshold && !HasShot && !player.IsAttachedToMouse && mouse.Position.ToVector2().Y >= ShootSeperator.Position.Y)
            {
                //Shooting


                Projectiles.Add(new PlayerBullet(
                mouse.Position.ToVector2(),
                0,
                new Texture(BulletTexture, Vector2.One),
                Vector2.Normalize(mouse.Position.ToVector2() - LastMousePos) * BulletSpeed,
                new Projectile.ProjectileDeathInfo(true, false, true, 0, 11, 0).SetDebrisFreeSides(false, false, true, false)));
                HasShot = true;
                
                SoundEffect sound = ShootSound.GetSound();
                SoundEffectInstance inst = sound.CreateInstance();
                inst.Volume = 0.2f;
                sound.Play(0.5f, 1, 1);
            }



            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (!RightMousePressed)
                {
                    MouseRecentlyPressed = true;
                }
                else
                {
                    MouseRecentlyPressed = false;
                }
            }
            else
            {
                MouseRecentlyPressed = false;
                
            }
            
            if (mouse.RightButton == ButtonState.Pressed)
            {
                RightMousePressed = true;
            }
            else
            {
                HasShot = false;
                RightMousePressed = false;
            }

            if ((MouseRecentlyPressed && player.Collider.IsColliding(mouse.Position.ToVector2())) || player.IsAttachedToMouse)
            {
                Vector2 dir = mouse.Position.ToVector2() - player.Position;
                player.Move(new Vector2(player.Speed * (float)Time.ElapsedGameTime.TotalSeconds, player.Speed * (float)Time.ElapsedGameTime.TotalSeconds) * dir);
                player.IsAttachedToMouse = true;
            }

        }
    }
}
