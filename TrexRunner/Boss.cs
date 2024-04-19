using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace TrexRunner
{
    internal class Boss : GameObject
    {
        public bool IsWhite = true;
        float WhiteTimer;
        float WhiteDuration;

        bool IsDead;
        int MaxHP;
        int HP;
        Animation anim;
        float StartTime;

        public Boss(Vector2 pos, List<Texture2D> frames, Point texturescale, int maxHP, float whiteDuration)
        {
            Textr = frames[0];
            Position = pos;
            

            StartTime = (float)Game1.Time.TotalGameTime.TotalSeconds;

            
            TextureScale = texturescale;
            MaxHP = maxHP;
            HP = maxHP;

            this.Collider = new BoxCollider(TopLeftCorner, Textr.Width * texturescale.X, Textr.Height * texturescale.Y, this);
            anim = new Animation(frames, 1f);
            WhiteDuration = whiteDuration;
        }

        protected override void OnEnterCollider(Collider collider)
        {
            if (collider.Parent is PlayerBullet playerBullet && !playerBullet.Tags.Contains("debris"))
            {
                Game1.Projectiles.Remove(playerBullet);
                HP--;
                WhiteTimer = 0;

                if (HP <= 0) {
                    IsDead = true;
                    Textr = null;
                    this.Collider = null;
                }

            }


        }
        protected override void OnExitCollider(Collider collider)
        {

        }
        protected override void OnUpdateCollider(Collider collider)
        {

        }
        public override void Update()
        {
            Textr = anim.GetCurrentFrame((float)Game1.Time.TotalGameTime.TotalSeconds - StartTime);
            Collider[] colliders = new Collider[Game1.Projectiles.Count];

            for (int i = 0; i < Game1.Projectiles.Count; i++)
            {
                colliders[i] = Game1.Projectiles[i].Collider;
            }
            CheckColliders(colliders);

            WhiteTimer += (float)Game1.Time.ElapsedGameTime.TotalSeconds;

            IsWhite = WhiteTimer < WhiteDuration;
        }
        public override void Move(Vector2 dir)
        {
            Position += dir;
        }
        public override void Move(Vector2 dir, float speed)
        {
            dir *= speed;
            Move(dir);
        }

        public void Move(Vector2 dir, double speed)
        {
            dir = new Vector2((float)(dir.X * speed), (float)(dir.Y * speed));
            Move(dir);
        }
        
    }




    public class BossAttackPattern
    {
        
    }
}
