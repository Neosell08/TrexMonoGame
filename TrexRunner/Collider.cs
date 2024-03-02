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

namespace TrexRunner
{
    public abstract class Collider
    {
        
        
        

        public abstract void Draw(SpriteBatch spritebatch);

        
    }


    



    public class CircleCollider : Collider
    {
        public override void Draw(SpriteBatch spritebatch)
        {
            
        }
    }

    

    
}
