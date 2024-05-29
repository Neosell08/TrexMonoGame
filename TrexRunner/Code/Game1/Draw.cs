using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace SpaceShooter
{
    public partial class Game1
    {
        protected override void Draw(GameTime gameTime)
        {


            if (CurState == GameState.MainMenu)
            {
                

                GraphicsDevice.SetRenderTarget(RenderTarget);
                GraphicsDevice.Clear(new Color(216, 185, 112));

                

                _spriteBatch.Begin();
                Level1Icon.Draw(_spriteBatch);
                SmallRecordText.Text = "Record:\n" + TimeToString(PersonalRecord);
                SmallRecordText.Draw(_spriteBatch);
                MoreSoonText.Draw(_spriteBatch, Color.Black);
                _spriteBatch.End();



                RenderTarget = ApplyEffect(CRTShader, RenderTarget, new SetShaderParamDelegate((shader) => { shader.Parameters["Time"].SetValue((float)Time.TotalGameTime.TotalSeconds); shader.Parameters["ChromaticStrength"].SetValue(ChromaticStrength); }));

                GraphicsDevice.SetRenderTarget(null);

                _spriteBatch.Begin();
                _spriteBatch.Draw(RenderTarget, new Rectangle(0, 0, (int)WindowResolution.X, (int)WindowResolution.Y), Color.White);
                _spriteBatch.End();


            }



            else if (CurState == GameState.Countdown)
            {
                GraphicsDevice.SetRenderTarget(RenderTarget);
                GraphicsDevice.Clear(Color.Transparent);

                if (CurState == GameState.Dead || CurState == GameState.Won)
                {
                    ChromaticStrength = (float)Math.Clamp(Convert.ToDouble(ChromaticStrength + 0.005), 0.0, 0.1);
                }


                _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

                _spriteBatch.Draw(Background, new Vector2(0, 0), Color.White);
                _spriteBatch.End();

                RenderTarget = ApplyEffect(CRTShader, RenderTarget, new SetShaderParamDelegate((shader) => { shader.Parameters["Time"].SetValue((float)Time.TotalGameTime.TotalSeconds); shader.Parameters["ChromaticStrength"].SetValue(ChromaticStrength); }));

                GraphicsDevice.SetRenderTarget(null);

                _spriteBatch.Begin();
                _spriteBatch.Draw(Background, new Vector2(0, 0), new Color(0.85f, 0.85f, 0.85f));
                _spriteBatch.Draw(RenderTarget, new Rectangle(0, 0, (int)WindowResolution.X, (int)WindowResolution.Y), Color.White);
                CountdownText.Draw(_spriteBatch);
                _spriteBatch.End();

            }


            else
            {
                GraphicsDevice.SetRenderTarget(RenderTarget2);
                GraphicsDevice.Clear(Color.Transparent);
                if (CurState == GameState.Dead || CurState == GameState.Won)
                {
                    ChromaticStrength = (float)Math.Clamp(Convert.ToDouble(ChromaticStrength + 0.005), 0.0, 0.1);
                }



                _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null);
                foreach (LightningBossAttackObject lightning in LightningObjects)
                {
                    if (!lightning.IsWarning)
                    {
                        lightning.Draw(_spriteBatch);
                    }
                }
                _spriteBatch.End();

                GraphicsDevice.SetRenderTarget(RenderTarget);
                GraphicsDevice.Clear(Color.Transparent);

                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);
                DrawRenderTarget(RenderTarget2);
                _spriteBatch.End();



                GraphicsDevice.SetRenderTarget(RenderTarget2);
                GraphicsDevice.Clear(Color.Transparent);

                _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
                _spriteBatch.Draw(Background, new Vector2(0, 0), new Color(0.85f, 0.85f, 0.85f));
                DrawRenderTarget(RenderTarget);
                ShootSeperator.Draw(_spriteBatch, new Color(0.25f, 0.25f, 0.25f, 0.25f));

                if (CurState == GameState.Playing)
                {
                    player.Draw(_spriteBatch);
                }

                foreach (Projectile bullet in Projectiles)
                {
                    if (bullet is PlayerBullet pb && pb.Tags.Contains("debris"))
                    {
                        bullet.Draw(_spriteBatch, new Color(1, 0.2f, 0.2f, 0.3f));
                    }
                    else
                    {
                        bullet.Draw(_spriteBatch);
                    }
                    
                }
                foreach (LightningBossAttackObject lightning in LightningObjects)
                {
                    if (lightning.IsWarning)
                    {
                        lightning.Draw(_spriteBatch);
                    }
                }

                _spriteBatch.End();




                _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, CurBoss.IsWhite ? WhiteEffect : null);

                CurBoss.Draw(_spriteBatch);

                _spriteBatch.End();




                _spriteBatch.Begin();

                if (CurState == GameState.Dead || CurState == GameState.Won)
                {
                    _spriteBatch.Draw(Collider.PixelTexture, new Rectangle(0, 0, (int)WindowResolution.X, (int)WindowResolution.Y), new Color(2, 2, 2, 150));
                }

                _spriteBatch.End();



                RenderTarget2 = ApplyEffect(CRTShader, RenderTarget2, new SetShaderParamDelegate((shader) => { shader.Parameters["Time"].SetValue((float)Time.TotalGameTime.TotalSeconds); shader.Parameters["ChromaticStrength"].SetValue(ChromaticStrength); }));
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Transparent);



                _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
                
                
                DrawRenderTarget(RenderTarget2);
                if (CurState == GameState.Dead || CurState == GameState.Won)
                {

                    TimeText.Text = "Time: " + TimeToString(PlayingTime);
                     RecordText.Text = "Personal Record: " + TimeToString(PlayingTime);


                    PlayButton.Draw(_spriteBatch);
                    ExitButton.Draw(_spriteBatch);
                    MainText.Text = CurState == GameState.Won ? "Completed" : "Dead";
                    MainText.Draw(_spriteBatch, CurState == GameState.Dead ? Color.Red : Color.Green);
                    TimeText.Draw(_spriteBatch);
                    if (CurState == GameState.Won)
                    {
                        RecordText.Draw(_spriteBatch);
                    }
                    if (HasNewRecord)
                    {
                        NewRecordText.Draw(_spriteBatch, Color.Yellow);
                    }
                }
                _spriteBatch.End();
                // TODO: Add your drawing code here
            }


            base.Draw(gameTime);



        }
        public delegate void SetShaderParamDelegate(Effect shader);
        /// <summary>
        /// Applies a shader to the rendertarget
        /// </summary>
        /// <param name="shader">Shader to be applied</param>
        /// <param name="renderTarget">RenderTarget to apply the shader to</param>
        /// <param name="shaderParamSetter">Delegate to set the parameters of the shader</param>
        /// <returns>RenderTarget with the effect applied</returns>
        public RenderTarget2D ApplyEffect(Effect shader, RenderTarget2D renderTarget, SetShaderParamDelegate shaderParamSetter = null)
        {
            shaderParamSetter = shaderParamSetter ?? new SetShaderParamDelegate((s) => { });

            RenderTarget2D output = renderTarget;
            RenderTarget2D renderTarget2 = new RenderTarget2D(renderTarget.GraphicsDevice, renderTarget.Width, renderTarget.Height);

            for (int i = 0; i < shader.CurrentTechnique.Passes.Count; i++)
            {
                if (i + 1 == shader.CurrentTechnique.Passes.Count)
                {
                    GraphicsDevice.SetRenderTarget(i % 2 == 0 ? renderTarget2 : renderTarget);
                    output = i % 2 == 0 ? renderTarget2 : renderTarget;
                }
                else
                {
                    GraphicsDevice.SetRenderTarget(i % 2 == 0 ? renderTarget2 : renderTarget);
                }
                GraphicsDevice.Clear(Color.Transparent);

                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null);


                shaderParamSetter.Invoke(shader);


                shader.CurrentTechnique.Passes[i].Apply();

                _spriteBatch.Draw(i % 2 == 0 ? renderTarget : renderTarget2, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);
                _spriteBatch.End();
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Transparent);

            if (renderTarget == output)
            {
                renderTarget2.Dispose();
            }
            else
            {
                renderTarget.Dispose();
            }
            
            return output;
        }
        /// <summary>
        /// Draws a RenderTarget to a the spritebatch
        /// </summary>
        /// <param name="rt">RenderTarget to draw</param>
        public void DrawRenderTarget(RenderTarget2D rt)
        {
            if (rt == null) { return; }
            _spriteBatch.Draw(rt, new Rectangle(0, 0, (int)WindowResolution.X, (int)WindowResolution.Y), Color.White);
        } 
    }
}
