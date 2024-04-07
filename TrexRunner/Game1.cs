using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;


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

    

    /// <summary>
    /// Buffer texture that collects other textures and then draws them collectively
    /// </summary>
    private RenderTarget2D RenderTarget;

    /// <summary>
    /// The boss GameObject 
    /// </summary>
    private Boss CurBoss;

    /// <summary>
    /// If the mouse is pressed down
    /// </summary>
    private bool MousePressed;

    /// <summary>
    /// The position of the mouse last frame
    /// </summary>
    private Vector2 LastMousePos;

    /// <summary>
    /// The Velocity of the mouse required to shoot
    /// </summary>
    private float ShootVelocityTreshold = 1;

    private PlayerBullet TestBullet;
    /// <summary>
    /// Resolution of the computer screen
    /// </summary>
    public static Vector2 ScreenResolution;
    /// <summary>
    /// Resolution of the game window
    /// </summary>
    public static Vector2 WindowResolution;

    public List<Projectile> Projectiles;
    
    public Game1()
    {
        
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _content = Content;
        
        
    }

    protected override void Initialize()
    {

        ScreenResolution = new Vector2(GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);
        WindowResolution = new Vector2(GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        player = new Player(new Vector2(0, 0), Content.Load<Texture2D>("Resources/Player"));
        player.TextureScale = new Point(5, 5);

        Background = Content.Load<Texture2D>("Resources/Background");
        

        player.Collider = new CircleCollider(player.Position, 17);
        TestBullet = new PlayerBullet(
            new Vector2(_graphics.PreferredBackBufferWidth / 2,
            _graphics.PreferredBackBufferHeight / 2),
            0,
            Content.Load<Texture2D>("Resources/fireball"),
            new Point(1, 1), 
            new Vector2(0, -0.1f), 
            5);
        
        RenderTarget = new RenderTarget2D(GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

        CurBoss = new Boss(new Vector2(0, 0), Content.Load<Texture2D>("Resources/Computer"), new Point(2, 2));
        


        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        
        Time = gameTime;

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        player.Update();
        TestBullet.Update();
        MouseState mouse = Mouse.GetState();
       
        if (mouse.LeftButton == ButtonState.Released)
        {
            player.IsAttachedToMouse = false;
        }
        if(MousePressed && Distance(LastMousePos, mouse.Position.ToVector2()) >= ShootVelocityTreshold)
        {
            //Shoot
        }
        
        
        

        if ((mouse.LeftButton == ButtonState.Pressed && player.Collider.IsColliding(mouse.Position.ToVector2())) || player.IsAttachedToMouse) 
        {
            Vector2 dir = mouse.Position.ToVector2() - player.Position;
            player.Move(new Vector2(player.Speed* (float)gameTime.ElapsedGameTime.TotalSeconds, player.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds) * dir);
            player.IsAttachedToMouse = true;
            MousePressed = true;
        }
        else if (mouse.LeftButton == ButtonState.Pressed)
        {
            MousePressed = true;
        }
        else
        {
            MousePressed = false;
        }

        LastMousePos = mouse.Position.ToVector2();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        GraphicsDevice.SetRenderTarget(null);

        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

        _spriteBatch.Draw(Background, new Vector2(0, 0), new Color(200, 200, 200));
        TestBullet.Draw(_spriteBatch);
        player.Draw(_spriteBatch);
        CurBoss.Draw(_spriteBatch); 

        _spriteBatch.End();
        //GraphicsDevice.SetRenderTarget(null);
        //_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, testShader);

        //_spriteBatch.Draw(RenderTarget, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

        //_spriteBatch.End();

        // TODO: Add your drawing code here
        
        base.Draw(gameTime);



    }

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

}
