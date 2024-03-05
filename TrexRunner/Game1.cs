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



    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _content = Content;


    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        player = new Player(new Vector2(0, 0));
        player.Collider = new CircleCollider(GetTextureCenter(player.Texture, player.Position), player.Texture.Height/2);
        player.DrawCollider = true;
        

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        player.Texture = Content.Load<Texture2D>("Resources/Player");

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        player.Update();

        MouseState mouse = Mouse.GetState();
       
        
        

        if (mouse.LeftButton == ButtonState.Pressed) 
        {
            Vector2 dir = mouse.Position.ToVector2() - player.Position;
            player.Move(new Vector2(player.Speed* (float)gameTime.ElapsedGameTime.TotalSeconds, player.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds) * dir);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        player.Draw(_spriteBatch);
            
        _spriteBatch.End();

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }

    public static double Distance(Vector2 p1, Vector2 p2)
    {
        return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p1.Y, 2));
    }
    public static double Distance(Point p1, Point p2)
    {
        return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p1.Y, 2));
    }

    public static Vector2 GetTextureCenter(Texture2D texture, Vector2 offset)
    {
        return new Vector2(offset.X + (texture.Width / 2), offset.Y + (texture.Height / 2));
    }
}
