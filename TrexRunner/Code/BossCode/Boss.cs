
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
using static SpaceShooter.GameInstance;
using SpaceShooter;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework.Audio;
using System.Runtime.Versioning;
using MonoGameTexture = Microsoft.Xna.Framework.Graphics.Texture;
using Texture = TrexRunner.Code.Game1.Texture;
using TrexRunner.Code.Game1;





namespace TrexRunner.Code.BossCode
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


                if (HP <= 0 && Game.CurState == GameState.Playing)
                {
                    IsDead = true;
                    Textr.Textr = null;
                    Collider = null;
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
            Textr.Textr = Anim.GetCurrentFrame((float)Globals.Time.TotalGameTime.TotalSeconds);
            CheckColliders(CheckedColliders);

            WhiteTimer += (float)Globals.Time.ElapsedGameTime.TotalSeconds;
            IsWhite = WhiteTimer < WhiteDuration;

            Vector2 targetDir = MovePoints[CurPointIndex] - Position;

            MoveDir = Vector2.Lerp(MoveDir, targetDir, LerpSpeed);
            MoveDir.Normalize();
            Move(MoveDir, Speed * (float)Globals.Time.ElapsedGameTime.TotalSeconds);

            if (Globals.Distance(Position, MovePoints[CurPointIndex]) < NewPointDistanceThreshold)
            {
                CurPointIndex = Globals.CircularClamp(CurPointIndex + 1, 0, MovePoints.Length - 1);
            }
            if (AttackPattern != null)
            {
                AttackPattern.Update();
                
            }

        }


       
    }



}



