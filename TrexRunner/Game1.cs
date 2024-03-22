using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Net;


namespace TrexRunner;

public partial class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    Player player;
    public static Random rng = new Random();
    public static ContentManager _content;
    public static GameTime Time = new GameTime();
    public Texture2D Background;
    public Effect testShader;
    private RenderTarget2D RenderTarget;
    private Boss CurBoss;

    Vector2 Resolution;
    
    public Game1()
    {
        
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _content = Content;
        Resolution = new Vector2(2222222,2,,2e.,async-.,we);
        
    }

    protected override void Initialize()
    {

        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        player = new Player(new Vector2(0, 0), Window, Content.Load<Texture2D>("Resources/Player"));
        player.TextureScale = new Vector2(5, 5);

        Background = Content.Load<Texture2D>("Resources/Background");


        player.Collider = new CircleCollider(player.Position, 17);
        
        
        RenderTarget = new RenderTarget2D(GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        //RenderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);

        testShader = Content.Load<Effect>("Resources/test");


        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        player.Update();

        MouseState mouse = Mouse.GetState();
       
        if (mouse.LeftButton == ButtonState.Released)
        {
            player.IsAttachedToMouse = false;
        }
        
        
        

        if ((mouse.LeftButton == ButtonState.Pressed && player.Collider.IsColliding(mouse.Position.ToVector2())) || player.IsAttachedToMouse) 
        {
            Vector2 dir = mouse.Position.ToVector2() - player.Position;
            player.Move(new Vector2(player.Speed* (float)gameTime.ElapsedGameTime.TotalSeconds, player.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds) * dir);
            player.IsAttachedToMouse = true;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        GraphicsDevice.SetRenderTarget(null);

        _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

        _spriteBatch.Draw(Background, new Vector2(0, 0), new Color(200, 200, 200));

        player.Draw(_spriteBatch);
        

        _spriteBatch.End();
        //GraphicsDevice.SetRenderTarget(null);
        //_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, testShader);

        //_spriteBatch.Draw(RenderTarget, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

        //_spriteBatch.End();

        // TODO: Add your drawing code here
        
        base.Draw(gameTime);



    }

    public static double Distance(Vector2 p1, Vector2 p2)
    {
        return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
    }
    public static double Distance(Point p1, Point p2)
    {
        return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
    }

    public static Vector2 GetTextureCenter(Texture2D texture, Vector2 offset, Vector2 Scale)
    {
        return new Vector2(offset.X + (texture.Width / 2) * Scale.X, offset.Y + (texture.Height * Scale.Y / 2));
    }
}
