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
using System.Windows.Forms.Design;

namespace TrexRunner
{
    public class Boss : GameObject
    {
        public bool IsWhite = true;
        float WhiteTimer;
        float WhiteDuration;

        bool IsDead;
        int MaxHP;
        int HP;
        LoopingAnimation anim;
        float StartTime;

        Vector2[] MovePoints;
        int CurPointIndex;
        Vector2 MoveDir;
        float Speed;
        float NewPointDistanceThreshold = 5f;
        float LerpSpeed;

        public Boss(Vector2 pos, List<Texture2D> frames, Point texturescale, int maxHP, float whiteDuration, Vector2[] movePoints, float speed, float turnSpeed, float newPointDistanceLimit)
        {
            Textr = frames[0];
            Position = pos;

            Tags.Add("boss");

            StartTime = (float)Game1.Time.TotalGameTime.TotalSeconds;

            
            TextureScale = texturescale;
            MaxHP = maxHP;
            HP = maxHP;

            this.Collider = new BoxCollider(TopLeftCorner, Textr.Width * texturescale.X, Textr.Height * texturescale.Y, this);
            anim = new LoopingAnimation(frames, 1f);
            WhiteDuration = whiteDuration;
            MovePoints = movePoints;
            Speed = speed;
            LerpSpeed = turnSpeed;
            NewPointDistanceThreshold = newPointDistanceLimit;
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
            CheckColliders(CheckedColliders);

            WhiteTimer += (float)Game1.Time.ElapsedGameTime.TotalSeconds;
            IsWhite = WhiteTimer < WhiteDuration;

            Vector2 targetDir = MovePoints[CurPointIndex] - Position;

            MoveDir = Vector2.Lerp(MoveDir, targetDir, LerpSpeed);
            MoveDir.Normalize();
            Move(MoveDir, Speed*(float)Game1.Time.ElapsedGameTime.TotalSeconds);
            
            if (Game1.Distance(Position, MovePoints[CurPointIndex]) < NewPointDistanceThreshold)
            {
                CurPointIndex = Game1.CircularClamp(CurPointIndex+1, 0, MovePoints.Length-1);
            }
        }


        
    }




    public abstract class BossAttackPattern
    {
        public float ShootDelay;
        public Boss Parent;
        protected double Timer;
        public Projectile ProjectilePrefab;

        public abstract void Update();
    }

    public class SphereAttack : BossAttackPattern
    {
        float BulletCount;

        public override void Update()
        {
            Timer += Game1.Time.ElapsedGameTime.TotalSeconds;

            if (Timer >= ShootDelay)
            {
                for (int i = 0; i < BulletCount; i++)
                {
                    
                    //PlayerBullet playerBullet = new PlayerBullet(Position, StartRotation, Textr, TextureScale, Game1.RotationToVector(i * (360 / DeathInfo.DebrisAmount)) * DebrisSpeed, deathInfo);
                    
                    //Game1.Projectiles.Add(playerBullet);
                }
            }
        }
    }
}
