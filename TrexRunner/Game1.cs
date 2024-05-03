
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using static TrexRunner.BoxCollider;


namespace TrexRunner;

public partial class Game1 : Game
{
    

    private GraphicsDeviceManager _graphics;
    /// <summary>
    /// Main spritebatch
    /// </summary>
    private SpriteBatch _spriteBatch;
    

    Player player;

    /// <summary>
    /// The game's random object
    /// </summary>
    public static Random rng = new Random();

    public static ContentManager _content;
    /// <summary>
    /// Contains information about the game's time
    /// </summary>
    public static GameTime Time = new GameTime();

    /// <summary>
    /// Background Texture
    /// </summary>
    public Texture2D Background;
    Texture2D BulletTexture;

    

    /// <summary>
    /// Buffer texture that collects other textures and then draws them collectively
    /// </summary>
    private RenderTarget2D RenderTarget;

    /// <summary>
    /// The boss GameObject 
    /// </summary>
    public Boss CurBoss;



    /// <summary>
    /// If the mouse is pressed down
    /// </summary>
    private bool MousePressed;
    bool MouseRecentlyPressed;

    /// <summary>
    /// The position of the mouse last frame
    /// </summary>
    private Vector2 LastMousePos;

    /// <summary>
    /// The Velocity of the mouse required to shoot
    /// </summary>
    private float ShootVelocityTreshold = 35;

    float BulletSpeed = 0.4f;

    bool HasShot;

   
    /// <summary>
    /// Resolution of the computer screen
    /// </summary>
    public static Vector2 ScreenResolution;
    /// <summary>
    /// Resolution of the game window
    /// </summary>
    public static Vector2 WindowResolution;

    /// <summary>
    /// List of all projectiles 
    /// </summary>

    public static List<Projectile> Projectiles = new List<Projectile>();

    public enum GameState
    {
        Playing,
        Dead,
        MainMenu
    }
    
    Effect WhiteEffect;

    public GameState CurState;

    

   
    
    
    public Game1()
    {
        
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _content = Content;
        _graphics.PreferredBackBufferHeight = 800;
        _graphics.PreferredBackBufferWidth = 480;
        
        
    }

    protected override void Initialize()
    {

        ScreenResolution = new Vector2(GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);
        WindowResolution = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        CustomRect.Textr = Content.Load<Texture2D>("Resources/Circle");


        List<Texture2D> frames = new List<Texture2D>();

        frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile000"));
        frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile001"));
        frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile002"));
        frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile003"));
        frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile004"));
        frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile005"));
        frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile006"));
        frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile007"));
        //BossAttackPattern attackPattern = new SphereAttack();

        CurBoss = new Boss(new Vector2(250, 100), frames, new Point(2, 2), 20, 1, new Vector2[4] { new Vector2(100, 100), new Vector2(100, 200), new Vector2(350, 100), new Vector2(350, 200)}, 200, 0.004f, 5, 55);

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        player = new Player(new Vector2(0, 0), Content.Load<Texture2D>("Resources/Player"));
        player.TextureScale = new Point(5, 5);

        Background = Content.Load<Texture2D>("Resources/Background");

        BulletTexture = Content.Load<Texture2D>("Resources/fireball");
        player.Collider = new CircleCollider(player.Position, player.TextureScale.X * player.Textr.Width/2, player);
        
        WhiteEffect = Content.Load<Effect>("Resources/test");

        PlayerBullet.DebrisSpeed = 0.1f;
        RenderTarget = new RenderTarget2D(GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            

    }

    protected override void Update(GameTime gameTime)
    {
        
        Time = gameTime;
        InputCheck();

        // TODO: Add your update logic here
        player.Update();
        Projectile[] projectilesToBeUpdated = Projectiles.ToArray();

        foreach (Projectile projectile in projectilesToBeUpdated)
        {
            projectile.Update();
        }
        
        CurBoss.Update();
        

        LastMousePos = Mouse.GetState().Position.ToVector2();

        base.Update(gameTime);
    }


    void InputCheck()
    {
        MouseState mouse = Mouse.GetState();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (mouse.LeftButton == ButtonState.Released)
        {
            player.IsAttachedToMouse = false;
        }
        if (MousePressed && MathN.Distance(LastMousePos, mouse.Position.ToVector2()) >= ShootVelocityTreshold && !HasShot && !player.IsAttachedToMouse)
        {
            //Shooting


            Projectiles.Add(new PlayerBullet(
            mouse.Position.ToVector2(),
            0,
            BulletTexture,
            new Point(1, 1),
            Vector2.Normalize(mouse.Position.ToVector2() - LastMousePos) * BulletSpeed,
            new Projectile.ProjectileDeathInfo(true, false, true, 0, 11, 0).SetDebrisFreeSides(false, false, true, false)));
            HasShot = true;
        }


        //if ((mouse.LeftButton == ButtonState.Pressed && player.Collider.IsColliding(mouse.Position.ToVector2())) || player.IsAttachedToMouse)
        //{
        //    Vector2 dir = mouse.Position.ToVector2() - player.Position;
        //    player.Move(new Vector2(player.Speed * (float)Time.ElapsedGameTime.TotalSeconds, player.Speed * (float)Time.ElapsedGameTime.TotalSeconds) * dir);
        //    player.IsAttachedToMouse = true;

        //    if (!MousePressed)
        //    {
        //        MouseRecentlyPressed = true;
        //    }
        //    else
        //    {
        //        MouseRecentlyPressed = false;
        //    }

        //    MousePressed = true;
        //}
        if (mouse.LeftButton == ButtonState.Pressed)
        {
            if (!MousePressed)
            {
                MouseRecentlyPressed = true;
            }
            else
            {
                MouseRecentlyPressed = false;
            }
            MousePressed = true;
        }
        else
        {
            MousePressed = false;
            MouseRecentlyPressed = false;
            HasShot = false;
        }


        if ((MouseRecentlyPressed && player.Collider.IsColliding(mouse.Position.ToVector2())) || player.IsAttachedToMouse)
        {
            Vector2 dir = mouse.Position.ToVector2() - player.Position;
            player.Move(new Vector2(player.Speed * (float)Time.ElapsedGameTime.TotalSeconds, player.Speed * (float)Time.ElapsedGameTime.TotalSeconds) * dir);
            player.IsAttachedToMouse = true;
        }

    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Transparent);
        RenderTarget2D renderTarget = new RenderTarget2D(_graphics.GraphicsDevice, (int)WindowResolution.X, (int)WindowResolution.Y);
        //GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.Transparent);


        GraphicsDevice.SetRenderTarget(null);

        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

        _spriteBatch.Draw(Background, new Vector2(0, 0), new Color(100, 150, 100));
        player.Draw(_spriteBatch);
        foreach (Projectile bullet in Projectiles)
        {
            bullet.Draw(_spriteBatch);
        }

        //_spriteBatch.Draw(renderTarget, new Vector2(0, 0), Color.White);
       
        
        _spriteBatch.End();
        //GraphicsDevice.SetRenderTarget(null);
        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, CurBoss.IsWhite ? WhiteEffect : null);
        
        CurBoss.Draw(_spriteBatch);

        _spriteBatch.End();

        //_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, testShader);

        //_spriteBatch.Draw(RenderTarget, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

        //_spriteBatch.End();

        // TODO: Add your drawing code here

        base.Draw(gameTime);


        
    }

}

public static class MathN
{
    public const double DegreeToRadian = Math.PI / 180;
    public const double RadianToDegree = 1 / (Math.PI / 180);

    public static float Distance(Vector2 p1, Vector2 p2)
    {
        return MathF.Sqrt(MathF.Pow(p2.X - p1.X, 2) + MathF.Pow(p2.Y - p1.Y, 2));
    }
    public static float Distance(Point p1, Point p2)
    {
        return MathF.Sqrt(MathF.Pow(p2.X - p1.X, 2) + MathF.Pow(p2.Y - p1.Y, 2));
    }
    public static float Distance(Vector2 p1)
    {
        return MathF.Sqrt(MathF.Pow(p1.X, 2) + MathF.Pow(p1.Y, 2));
    }
    public static float Distance(Point p1)
    {
        return MathF.Sqrt(MathF.Pow(p1.X, 2) + MathF.Pow(p1.Y, 2));
    }

    public static Vector2 GetTextureCenter(Texture2D texture, Vector2 offset, Point Scale)
    {
        return new Vector2(offset.X + (texture.Width / 2) * Scale.X, offset.Y + (texture.Height * Scale.Y / 2));
    }
    public static Vector2 RotationToVector(float rotation)
    {
        return new Vector2(MathF.Cos(rotation), MathF.Sin(rotation));
    }
    public static float UnsignedAngle(Vector2 from, Vector2 to)
    {
        float dot = Vector2.Dot(from, to);
        float cross = from.X * to.Y - from.Y * to.X;
        return (float)Math.Atan2(cross, dot);
    }
    public static float CircularClamp(float value, float min, float max)
    {
        if (value < min)
        {
            return max;
        }
        else if (value > max)
        {
            return min;
        }
        return value;
    }
    public static int CircularClamp(int value, int min, int max)
    {
        if (value < min)
        {
            return max;
        }
        else if (value > max)
        {
            return min;
        }
        return value;
    }
    public static Vector2 LinearFunctionFromPoints(Vector2 p1, Vector2 p2)
    {
        float k;

        if (p1.Y - p2.Y == 0) { k = 0; } // straight line along the x axis
        else if (p1.X - p2.X == 0) { return new Vector2(float.PositiveInfinity, 0f); } // straight line along the y axis
        else { k = (p1.Y - p2.Y) / (p1.X - p2.X); }
        
        float m = p1.Y - k * p1.X;
        return new Vector2(k, m);
    }
    public static Vector2 LineCollision(Vector2 l1, Vector2 l2)
    {
        float x = (l2.Y - l1.Y) / (l1.X - l2.X);
        float y = l1.X * x + l1.Y;

        return new Vector2(x, y);
    }
    public static bool IsOnLine(Vector2 l, Vector2 p)
    {
        return IsInsideRange(l.X * p.X + l.Y, p.Y+0.001f, p.Y - 0.001f);
    }
    public static bool IsInsideRange(float value, float n1, float n2)
    {
        return value >= (n1 > n2 ? n2 : n1) && value < (n1 > n2 ? n1 : n2);
    }
    public static bool IsInsideRange(float v1, float v2, float n1, float n2)
    {
        return MathF.Min(v1, v2) <= MathF.Max(n1, n2) && MathF.Max(v1, v2) >= MathF.Min(n1, n2);
    }
    public static float LinearFunction(float x, Vector2 Line)
    {
        return Line.X * x + Line.Y;
    }
    public static float ReverseLinearFunction(float y, Vector2 line)
    {
        return (line.Y - y) / -line.X;
    }

    /// <summary>
    /// Rotates a vector around the origin (0, 0)
    /// </summary>
    /// <param name="vector">Vector originating at (0, 0)</param>
    /// <param name="angle">Angle in radians</param>
    /// <returns>Rotated vector</returns>
    public static Vector2 RotateVectorRad(Vector2 vector, float angle)
    {
        return new Vector2(vector.X * MathF.Cos(angle) - vector.Y * MathF.Sin(angle), vector.X * MathF.Sin(angle) + vector.Y * MathF.Cos(angle));
    }
    /// <summary>
    /// Rotates a vector around the origin (0, 0)
    /// </summary>
    /// <param name="vector">Vector originating at (0, 0)</param>
    /// <param name="angle">Angle in degrees</param>
    /// <returns>Rotated vector</returns>
    public static Vector2 RotateVectorDeg(Vector2 vector, float angle)
    {
        return RotateVectorRad(vector, (float)(angle * DegreeToRadian));
    }

    
}





