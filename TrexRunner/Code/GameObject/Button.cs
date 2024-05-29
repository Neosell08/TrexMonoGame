
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter
{
    /// <summary>
    /// Button with a texture that can be pressed
    /// </summary>
    public class Button : GameObject
    {
        Action OnButtonPressed;
        Rectangle Bounds;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos">Position of the centre</param>
        /// <param name="rotation">rotation of the texture</param>
        /// <param name="textr">Texture info</param>
        /// <param name="onButtonPressed">Action to invoke on the button being pressed</param>
        public Button(Vector2 pos, float rotation, Texture textr, Action onButtonPressed = null)
        { 
            OnButtonPressed = onButtonPressed;

            Position = pos;
            Rotation = rotation;
            Textr = textr;
            Bounds = new Rectangle(pos.ToPoint(), Textr.ScaledTextureScale.ToPoint());
        }
        public override void Update()
        {
            Point mousePos = Mouse.GetState().Position;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && MathN.IsInsideRange(mousePos.X, Position.X + Bounds.Width, Position.X - Bounds.Width) && MathN.IsInsideRange(mousePos.Y, Position.Y + Bounds.Height, Position.Y - Bounds.Height))
            {
                if (OnButtonPressed != null)
                {
                    OnButtonPressed.Invoke();
                }
            }

        }
    }
}
