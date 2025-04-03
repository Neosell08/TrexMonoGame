using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrexRunner.Code.Game1
{
    public struct Texture
    {
        Texture2D _Texture = null;


        Vector2 _TextureScale = Vector2.One;
        /// <summary>
        /// Scale of the texture when drawing to the spritebatch
        /// </summary>
        public Vector2 TextureScale { get { return _TextureScale; } set { _TextureScale = value; if (Textr != null) { ScaledTextureScale = TextureScale * new Vector2(Textr.Width, Textr.Height); } else { ScaledTextureScale = Vector2.Zero; } } }
        /// <summary>
        /// Texture scale multiplied by the texture size
        /// </summary>
        public Vector2 ScaledTextureScale { get; private set; }
        public Texture2D Textr { get { return _Texture; } set { _Texture = value; if (Textr != null) { ScaledTextureScale = TextureScale * new Vector2(Textr.Width, Textr.Height); } else { ScaledTextureScale = Vector2.Zero; } } }

        public Texture(Texture2D textr, Vector2? textrScale = null)
        {
            textrScale = textrScale ?? Vector2.One;
            _Texture = textr;
            _TextureScale = textrScale.Value;
            if (textr != null && textrScale != null)
            {
                ScaledTextureScale = _TextureScale * new Vector2(_Texture.Width, _Texture.Height);
            }
            else
            {
                ScaledTextureScale = Vector2.Zero;
            }

        }
    }
}
