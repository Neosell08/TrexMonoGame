
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
using static SpaceShooter.Game1;
using SpaceShooter;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework.Audio;
using System.Runtime.Versioning;
using MonoGameTexture = Microsoft.Xna.Framework.Graphics.Texture;
using Texture = SpaceShooter.Texture;





namespace SpaceShooter
{

    /// <summary>
    /// The boss of a level
    /// </summary>
    public class Boss : GameObject
    {
        /// <summary>
        /// If the Boss should be rendered as all white
        /// </summary>
        public bool IsWhite;
        float WhiteTimer;
        float WhiteDuration;

        bool IsDead;
        /// <summary>
        /// Maximum amount of Health for the boss
        /// </summary>
        public int MaxHP { get; private set; }
        /// <summary>
        /// Current health of the boss
        /// </summary>
        public int HP;
        /// <summary>
        /// Animation of the boss
        /// </summary>
        public LoopingAnimation Anim;
        /// <summary>
        /// Points where the boss should move
        /// </summary>
        Vector2[] MovePoints;
        /// <summary>
        /// At what index the current target point is 
        /// </summary>
        int CurPointIndex;
        Vector2 MoveDir;
        float Speed;
        /// <summary>
        /// At what distance the boss should move on to the next point
        /// </summary>
        float NewPointDistanceThreshold = 5f;
        float LerpSpeed;

        public BossAttackPattern AttackPattern;
        /// <summary>
        /// Sound played at death
        /// </summary>
        public SoundEffect DeathSound;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos">Position of the centre of the boss</param>
        /// <param name="frames">Frames which the boss animates</param>
        /// <param name="textr">Texture info of the boss. The textr.Textr will be replaced</param>
        /// <param name="maxHP">HP at start</param>
        /// <param name="whiteDuration">How long the boss will have the white effect after being hit</param>
        /// <param name="movePoints">Points which the boss will move between</param>
        /// <param name="speed">Speed of the boss movement</param>
        /// <param name="turnSpeed">Speed at which the boss turns towards it's point</param>
        /// <param name="newPointDistanceLimit">The distance limit at which the boss will change to a new point</param>
        /// <param name="ColliderRadius">Radius of the CircleCollider</param>
        /// <param name="deathSound">Sound that plays at death</param>
        public Boss(Vector2 pos, List<Texture2D> frames, Texture textr, int maxHP, float whiteDuration, Vector2[] movePoints, float speed, float turnSpeed, float newPointDistanceLimit, float ColliderRadius, SoundEffect deathSound)
        {
            Textr = textr;
            textr.Textr = frames[0];
            Position = pos;

            Tags.Add("boss");

            MaxHP = maxHP;
            HP = maxHP;

            Collider = new CircleCollider(pos, ColliderRadius, this);
            Anim = new LoopingAnimation(frames, 1f);
            WhiteDuration = whiteDuration;
            WhiteTimer = whiteDuration;
            MovePoints = movePoints;
            Speed = speed;
            LerpSpeed = turnSpeed;
            NewPointDistanceThreshold = newPointDistanceLimit;
            DeathSound = deathSound;
        }

        protected override void OnEnterCollider(Collider collider)
        {
            if (collider.Parent is PlayerBullet playerBullet && !playerBullet.Tags.Contains("debris"))
            {
                playerBullet.Remove();
                HP--;
                WhiteTimer = 0;
                

                if (HP <= 0 && Game.CurState == GameState.Playing) {
                    IsDead = true;
                    Textr.Textr = null;
                    this.Collider = null;
                    Game.SetState(GameState.Won);
                    DeathSound.Play();
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
            if (IsDead) { return; }
            Textr.Textr = Anim.GetCurrentFrame((float)Game1.Time.TotalGameTime.TotalSeconds); 
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



    /// <summary>
    /// Handles attacking logic for the boss
    /// </summary>
    public abstract class BossAttackPattern
    {
        public Vector2 ShootDelays;
        public float CurDelay;
        public Boss Parent;
        public float ProjectileSpeed;
        protected double Timer;
        public Projectile ProjectilePrefab;
        public Texture Textr;
        public Vector2 TextureScale;
        public SoundEffect DeathSoundEffect;

        public abstract void Update();
    }
    /// <summary>
    /// Attack that launches projectiles in a circle around the boss
    /// </summary>
    public class SphereAttack : BossAttackPattern
    {
        int BulletCount;
        float Inertia;
        float BulletRadius;

        public override void Update()
        {
            Timer += Game1.Time.ElapsedGameTime.TotalSeconds;

            if (Timer >= CurDelay)
            {
                for (int i = 0; i < BulletCount; i++)
                {
                    Projectile.ProjectileDeathInfo death = new Projectile.ProjectileDeathInfo(true, false, false, 0, 0, 1);

                    BossProjectile prj = new BossProjectile(death, Parent.Position, MathN.RotationToVector(i * (360 / BulletCount)) * ProjectileSpeed, 0, Textr, Inertia, BulletRadius);

                    Game1.Projectiles.Add(prj);
                }
                CurDelay = MathN.RandomFloat(ShootDelays.X, ShootDelays.Y);
                Timer = 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shootDelays">Min and max of the delay at which the boss will shoot</param>
        /// <param name="bulletCount">Amount of bullets at each shooting action</param>
        /// <param name="bulletSpeed">Speed of the bullets</param>
        /// <param name="inertia">Inertia of the bullets</param>
        /// <param name="parent">Boss that contains this AttackPattern</param>
        /// <param name="textr">Texture info of the bullets</param>
        /// <param name="radius">Radius of the bullet CircleCollider</param>
        public SphereAttack(Vector2 shootDelays, int bulletCount, float bulletSpeed, float inertia, Boss parent, Texture textr, float radius = 0)
        {
            ShootDelays = shootDelays;
            CurDelay = shootDelays.X;
            BulletCount = bulletCount;
            Parent = parent;
            Textr = textr;
            ProjectileSpeed = bulletSpeed;
            Inertia = inertia;
            BulletRadius = radius;
        }
    }
    /// <summary>
    /// Attack that launches projectiles in a circle around the boss and strikes lightning rays above the player
    /// </summary>
    public class SphereAndLightningAttack : BossAttackPattern
    {
        int BulletCount;
        float Inertia;
        float BulletRadius;
        float LightningTimer;
        float LightningDelay;
        Texture2D[] LightningFrames;
        Vector2 LightningTextureScale;
        float LightningAnimationSpeed;
        float LightningDeathTime;
        Player _Player;
        Texture WarnTexture;
        public SoundEffect LightningSound;
        public SoundEffect WarningSound;

        public override void Update()
        {
            Timer += Game1.Time.ElapsedGameTime.TotalSeconds;
            LightningTimer += (float)Game1.Time.ElapsedGameTime.TotalSeconds;

            if (Timer >= CurDelay)
            {
                for (int i = 0; i < BulletCount; i++)
                {
                    Projectile.ProjectileDeathInfo death = new Projectile.ProjectileDeathInfo(true, false, false, 0, 0, 1);

                    BossProjectile prj = new BossProjectile(death, Parent.Position, MathN.RotationToVector(i * (360 / BulletCount)) * ProjectileSpeed, 0, Textr, Inertia, BulletRadius);

                    Game1.Projectiles.Add(prj);
                }
                CurDelay = MathN.RandomFloat(ShootDelays.X, ShootDelays.Y);
                Timer = 0;
            }

            if (LightningTimer >= LightningDelay)
            {
                new LightningBossAttackObject(LightningFrames, LightningAnimationSpeed, new Texture(null, new Vector2(2, WindowResolution.Y / Textr.Textr.Height)), new Vector2(_Player.Position.X, 0), LightningDeathTime, 2, WarnTexture, LightningSound, WarningSound);
                LightningTimer = 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shootDelays">Min and max of the delay at which the boss will shoot</param>
        /// <param name="bulletCount">Amount of bullets at each shooting action</param>
        /// <param name="bulletSpeed">Speed of the bullets</param>
        /// <param name="inertia">Inertia of the bullets</param>
        /// <param name="parent">Boss that contains this AttackPattern</param>
        /// <param name="textr">Texture info of the bullets</param>
        /// <param name="radius">Radius of the bullet CircleCollider</param>
        /// <param name="lightningdelay">Delay of when the boss strikes lightning</param>
        /// <param name="lightningAnimationSpeed">Speed of the Lightnings animation</param>
        /// <param name="lightningDeathTime">Time at which the lightning disappears</param>
        /// <param name="lightningFrames">Frames of the lightning animation</param>
        /// <param name="player">Player in the game</param>
        /// <param name="warnTexture">Texture info of the warning sign before the lightning</param>
        public SphereAndLightningAttack(Vector2 shootDelays, float lightningdelay, int bulletCount, float bulletSpeed, float inertia, Boss parent, Texture textr, Texture2D[] lightningFrames, float lightningAnimationSpeed, float lightningDeathTime, Player player, Texture warnTexture, SoundEffect lightningSound, SoundEffect warningSound, float radius = 0 )
        {
            
            ShootDelays = shootDelays;
            CurDelay = MathN.RandomFloat(shootDelays.X, shootDelays.Y);
            BulletCount = bulletCount;
            Parent = parent;
            Textr = textr;
            ProjectileSpeed = bulletSpeed;
            Inertia = inertia;
            BulletRadius = radius;
            LightningFrames = lightningFrames;
            LightningDelay = lightningdelay;
            LightningAnimationSpeed = lightningAnimationSpeed;
            LightningDeathTime = lightningDeathTime;
            LightningSound = lightningSound;
            WarningSound = warningSound;
            _Player = player;
            WarnTexture = warnTexture;
        }
    }
}
/// <summary>
/// Projectile that is launched by the boss
/// </summary>
public class BossProjectile : Projectile
{
    float Inertia;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="deathInfo">Info about when the projectile is supposed to destroy itself</param>
    /// <param name="pos">Position of the centre of the projectile</param>
    /// <param name="velocity">Unnormalised velocity of the projectile. Rotation is changed according to velocity</param>
    /// <param name="rotation">Rotation offset of the projectile</param>
    /// <param name="textr">Texture info</param>
    /// <param name="inertia">Roation change</param>
    /// <param name="radius">Radius of the CircleCollider</param>
    public BossProjectile(ProjectileDeathInfo deathInfo, Vector2 pos, Vector2 velocity, float rotation, Texture textr, float inertia = 0, float radius = 0)
    {
        DeathInfo = deathInfo;
        Velocity = velocity;
        Textr = textr;
        Rotation = rotation;
        Position = pos;
        Collider = new CircleCollider(pos, radius == 0 ? (Textr.ScaledTextureScale.X + Textr.ScaledTextureScale.Y) / 2 : radius, this);
        Inertia = inertia;

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
        Rotation += Inertia * (float)Game1.Time.ElapsedGameTime.TotalSeconds;
        if (IsOutsideOfWindow() && DeathInfo.DestroyAtBorder)
        {
            Remove();
        }


        base.Update();
    }
}

/// <summary>
/// Lightning that strikes above the player
/// </summary>
public class LightningBossAttackObject : GameObject
{
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
            DeathTimer += (float)Time.ElapsedGameTime.TotalSeconds;
            Textr.Textr = Anim.GetCurrentFrame((float)Time.TotalGameTime.TotalSeconds);
        }
        
        
        WarnTimer += (float)Time.ElapsedGameTime.TotalSeconds;

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
                
                spriteBatch.Draw(WarnTexture.Textr, new Rectangle((int)Position.X,(int)WarnTexture.ScaledTextureScale.Y, (int)WarnTexture.ScaledTextureScale.X, (int)WarnTexture.ScaledTextureScale.Y), null, color.Value, Rotation, new Vector2(WarnTexture.Textr.Width / 2f, WarnTexture.Textr.Height / 2f), SpriteEffects.None, 0);
            }
        }
        
    }
    /// <summary>
    /// Sets up the object to be collected by the garbage collector
    /// </summary>
    public void Dispose()
    {
        Game1.LightningObjects.Remove(this);
        Collider.ColliderList.Remove(Collider);
    }   
}

