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
using static TrexRunner.Game1;
using TrexRunner;
using System.Security.Cryptography.X509Certificates;

namespace TrexRunner
{
    public class Boss : GameObject
    {
        public bool IsWhite;
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

        public BossAttackPattern AttackPattern;

        public Boss(Vector2 pos, List<Texture2D> frames, Point texturescale, int maxHP, float whiteDuration, Vector2[] movePoints, float speed, float turnSpeed, float newPointDistanceLimit, float ColliderRadius)
        {
            Textr = frames[0];
            Position = pos;

            Tags.Add("boss");

            StartTime = (float)Game1.Time.TotalGameTime.TotalSeconds;


            TextureScale = texturescale;
            MaxHP = maxHP;
            HP = maxHP;

            Collider = new CircleCollider(pos, ColliderRadius, this);
            anim = new LoopingAnimation(frames, 1f);
            WhiteDuration = whiteDuration;
            WhiteTimer = whiteDuration;
            MovePoints = movePoints;
            Speed = speed;
            LerpSpeed = turnSpeed;
            NewPointDistanceThreshold = newPointDistanceLimit;
            
        }

        protected override void OnEnterCollider(Collider collider)
        {
            if (collider.Parent is PlayerBullet playerBullet && !playerBullet.Tags.Contains("debris"))
            {
                playerBullet.Remove();
                HP--;
                WhiteTimer = 0;

                if (HP <= 0) {
                    IsDead = true;
                    Textr = null;
                    this.Collider = null;
                }
            }
            else if (collider.Parent is Player plr)
            {
                plr.Kill();
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
            //Debug.WriteLine("Hello");
            Textr = anim.GetCurrentFrame((float)Game1.Time.TotalGameTime.TotalSeconds - StartTime); 
            CheckColliders(CheckedColliders);

            WhiteTimer += (float)Game1.Time.ElapsedGameTime.TotalSeconds;
            IsWhite = WhiteTimer < WhiteDuration;

            Vector2 targetDir = MovePoints[CurPointIndex] - Position;

            MoveDir = Vector2.Lerp(MoveDir, targetDir, LerpSpeed);
            MoveDir.Normalize();
            Move(MoveDir, Speed*(float)Game1.Time.ElapsedGameTime.TotalSeconds);
            
            if (MathN.Distance(Position, MovePoints[CurPointIndex]) < NewPointDistanceThreshold)
            {
                CurPointIndex = MathN.CircularClamp(CurPointIndex+1, 0, MovePoints.Length-1);
            }
            if (AttackPattern != null)
            {
                AttackPattern.Update();
            }
            
        }


        
    }




    public abstract class BossAttackPattern
    {
        public float ShootDelay;
        public Boss Parent;
        public float ProjectileSpeed;
        protected double Timer;
        public Projectile ProjectilePrefab;
        public Texture2D Textr;
        public Point TextureScale;

        public abstract void Update();
    }

    public class SphereAttack : BossAttackPattern
    {
        int BulletCount;
        

        public override void Update()
        {
            Timer += Game1.Time.ElapsedGameTime.TotalSeconds;

            if (Timer >= ShootDelay)
            {
                for (int i = 0; i < BulletCount; i++)
                {
                    Projectile.ProjectileDeathInfo death = new Projectile.ProjectileDeathInfo(true, false, false, 0, 0, 1);

                    BossProjectile prj = new BossProjectile(death, Parent.Position, MathN.RotationToVector(i * (360 / BulletCount)) * ProjectileSpeed, 0, Textr, TextureScale);

                    Game1.Projectiles.Add(prj);
                }
                Timer = 0;
            }

            
        }
        public SphereAttack(float shootDelay, int bulletCount, float bulletSpeed, Boss parent, Texture2D textr, Point textureScale)
        {
            ShootDelay = shootDelay;
            BulletCount = bulletCount;
            Parent = parent;
            Textr = textr;
            TextureScale = textureScale;
            ProjectileSpeed = bulletSpeed;
        }
    }
}

public class BossProjectile: Projectile
{
    
    public BossProjectile(ProjectileDeathInfo deathInfo, Vector2 pos, Vector2 velocity, float rotation, Texture2D textr, Point textureScale)
    {
        DeathInfo = deathInfo;
        Velocity = velocity;
        TextureScale = textureScale;
        Rotation = rotation;
        Textr = textr;
        Position = pos;
        Collider = new CircleCollider(pos, (textr.Width * textureScale.X + textr.Height * textureScale.Y)/2, this);

        Rotation += MathF.Atan2(velocity.Y, velocity.X);
    }
    protected override void OnEnterCollider(Collider collider)
    {
        if (collider.Parent is Player plr)
        {
            plr.Kill();
        }
    }
    public override void Update()
    {
        base.Update();
    }



}
