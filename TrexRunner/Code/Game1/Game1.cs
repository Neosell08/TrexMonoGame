
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using System.Linq;
using System.Net;
using SpaceShooter.Code;
using static SpaceShooter.BoxCollider;
using System.Text.Json;
using System.IO;


namespace SpaceShooter;

public partial class Game1 : Game
{
    

    private GraphicsDeviceManager _graphics;
    /// <summary>
    /// Main spritebatch
    /// </summary>
    private SpriteBatch _spriteBatch;

    /// <summary>
    /// Amount of time the level has been played
    /// </summary>
    public float PlayingTime;
    /// <summary>
    /// The lowest amount of time the level has been completed in
    /// </summary>
    public float PersonalRecord = float.PositiveInfinity;
    /// <summary>
    /// Whether or not the record has been updated after winning
    /// </summary>
    public bool HasNewRecord;
    

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


    public RenderTarget2D RenderTarget;
    public RenderTarget2D RenderTarget2;

    /// <summary>
    /// The boss GameObject 
    /// </summary>
    public static Boss CurBoss;

    public static List<LightningBossAttackObject> LightningObjects = new List<LightningBossAttackObject>();



    /// <summary>
    /// If the mouse is pressed down
    /// </summary>
    private bool RightMousePressed;
    bool MouseRecentlyPressed;

    /// <summary>
    /// The position of the mouse last frame
    /// </summary>
    private Vector2 LastMousePos;
    private Effect CRTShader;

    /// <summary>
    /// The Velocity of the mouse required to shoot
    /// </summary>
    private float ShootVelocityTreshold = 22;

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


    float ChromaticStrength = 0.01f;

    Button Level1Icon;

    public enum GameState
    {
        Playing,
        Dead,
        MainMenu,
        Won,
        Countdown
    }
    
    Effect WhiteEffect;
    Effect BloomEffect;

    public GameState CurState = GameState.MainMenu;
    public Button PlayButton;
    public Button ExitButton;
    public TextGameObject MainText;
    public TextGameObject TimeText;
    public TextGameObject RecordText;
    public TextGameObject SmallRecordText;
    public TextGameObject NewRecordText;
    public TextGameObject MoreSoonText;

    public TextGameObject CountdownText;
    public float CountdownTimer;
    public int CountdownSeconds = 3;

    RandomSound<SoundEffect> ShootSound;
    Song MainMenuSong;
    Song Level1Song;

    BasicGameObject ShootSeperator;
   
    

    

    

   
    
    
    public Game1()
    {
        LoadExternalData();
        _graphics = new GraphicsDeviceManager(this);
        _graphics.GraphicsProfile = GraphicsProfile.HiDef;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _content = Content;
        _graphics.PreferredBackBufferHeight = 600;
        _graphics.PreferredBackBufferWidth = 600;
        GameObject.Game = this;
        MediaPlayer.IsRepeating = true;
    }
    
    

    protected override void Initialize()
    {
        

        ScreenResolution = new Vector2(GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);
        WindowResolution = new Vector2(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        Collider.PixelTexture = Content.Load<Texture2D>("Resources/pixel");
        MainMenuSong = Content.Load<Song>("Resources/MainMenuSong");
        Level1Song = Content.Load<Song>("Resources/Level1Song");
        ShootSound = new RandomSound<SoundEffect>(new List<SoundEffect>() { Content.Load<SoundEffect>("Resources/laserShoot"), Content.Load<SoundEffect>("Resources/laserShoot (1)"), Content.Load<SoundEffect>("Resources/laserShoot (2)") }) ;

        MediaPlayer.Play(MainMenuSong);


        ShootSeperator = new BasicGameObject(WindowResolution/2, 0, new Texture(Collider.PixelTexture, new Vector2(WindowResolution.X, 10)));
        
        CRTShader = Content.Load<Effect>("Resources/CRTShader");
        CRTShader.Parameters["PixelSize"].SetValue(8);
        CRTShader.Parameters["ScreenSize"].SetValue(ScreenResolution);
        Background = Content.Load<Texture2D>("Resources/Background");

        InitBoss(true);

        RenderTarget = new RenderTarget2D(GraphicsDevice, (int)WindowResolution.X, (int)WindowResolution.Y);
        RenderTarget2 = new RenderTarget2D(GraphicsDevice, (int)WindowResolution.X, (int)WindowResolution.Y);
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        player = new Player(new Vector2(300, 300), new Texture(Content.Load<Texture2D>("Resources/Player"), new Vector2(2, 2)), Content.Load<SoundEffect>("Resources/PlayerDead"));
        player.Textr.TextureScale = new Vector2(5, 5);
        
        

        BulletTexture = Content.Load<Texture2D>("Resources/fireball");
        player.Collider = new CircleCollider(player.Position, player.Textr.TextureScale.X * player.Textr.Textr.Width/2, player);
        
        WhiteEffect = Content.Load<Effect>("Resources/test");
        BloomEffect = Content.Load<Effect>("Resources/Bloom");

        PlayerBullet.DebrisSpeed = 0.1f;
        

        PlayButton = new Button(new Vector2(50, 550), 0, new Texture(Content.Load<Texture2D>("Resources/Buttons/tile000"), new Vector2(2,2)), () => { SetState(GameState.Countdown); });
        ExitButton = new Button(new Vector2(550, 550), 0, new Texture(Content.Load<Texture2D>("Resources/Buttons/tile001"), new Vector2(2,2)), () => { SetState(GameState.MainMenu); });
        Texture2D textr = Content.Load<Texture2D>("Resources/Level1Icon");
        Vector2 textureScale = new Vector2(100f/textr.Width, 100f/textr.Height);
        Level1Icon = new Button(new Vector2(75 + ((textr.Width*textureScale.X)/2), 120 + (textr.Height*textureScale.Y)/2), 0, new Texture(textr, textureScale), () => { SetState(GameState.Countdown); });

        SpriteFont titleFont = Content.Load<SpriteFont>("TitleFont");
        SpriteFont smallFont = Content.Load<SpriteFont>("SmallFont");
        SpriteFont smallerFont = Content.Load<SpriteFont>("SmallerFont");


        textureScale = Vector2.One;
        TimeText = new TextGameObject(smallerFont, "Time: 00 : 00", new Vector2(WindowResolution.X / 2, WindowResolution.Y / 2 ), textureScale);
        MainText = new TextGameObject(titleFont, "Dead", new Vector2(WindowResolution.X/2, WindowResolution.Y / 2 - (smallFont.MeasureString("Time: 00 : 00").Y/2 + titleFont.MeasureString("DEAD").Y/2)), textureScale);
        RecordText = new TextGameObject(smallerFont, "Personal Record: 00 : 00", new Vector2(WindowResolution.X / 2, WindowResolution.Y / 2 + smallFont.MeasureString("Time: 00 : 00").Y), textureScale);
        NewRecordText = new TextGameObject(smallerFont, "New Personal Record!", RecordText.Position + new Vector2(0, smallFont.MeasureString("New RECORD:LDAWJDIOALKSDAWDLMDFA").Y), textureScale);

        SmallRecordText = new TextGameObject(smallFont, "Record:\n00 : 00", new Vector2(Level1Icon.Position.X, Level1Icon.Position.Y + 80), textureScale);
        
        MoreSoonText = new TextGameObject(smallFont, "More Coming Soon!", new Vector2((WindowResolution.X - (Level1Icon.TopLeftCorner.X + Level1Icon.Textr.ScaledTextureScale.X) + 350) / 2, WindowResolution.Y / 2), textureScale);
        CountdownText = new TextGameObject(titleFont, "3", WindowResolution/2, textureScale);

        
    }
    /// <summary>
    /// Sets the GameState and performs logic for transition
    /// </summary>
    /// <param name="state">State to change to</param>
    public void SetState(GameState state)
    {
        CurState = state;

        HasNewRecord = false;

        if (state == GameState.Playing)
        {
            List<Projectile> temp = Projectiles.ToList();

            InitBoss();
            LightningBossAttackObject[] lightnings = LightningObjects.ToArray();

            foreach (LightningBossAttackObject lightning in lightnings)
            {
                lightning.Dispose();
            }


            foreach (Projectile p in temp)
            {
                p.Remove();
            }

            player.Position = new Vector2(300, 300);
            ChromaticStrength = 0.01f;
            
            PlayingTime = 0f;
            
        }


        else if (state == GameState.MainMenu)
        {
            MediaPlayer.Play(MainMenuSong);
            ChromaticStrength = 0.01f;
        }


        else if (state == GameState.Won)
        {
            MediaPlayer.Stop();
            ChromaticStrength = 0.01f;
            if (PlayingTime < PersonalRecord)
            {
                PersonalRecord = PlayingTime;
                HasNewRecord = true;
            }
        }


        else if (state == GameState.Dead)
        {
            MediaPlayer.Stop();
        }


        else if (state == GameState.Countdown)
        {
            MediaPlayer.Play(Level1Song);
            ChromaticStrength = 0.01f;
            CountdownTimer = CountdownSeconds;
        }


    }
    /// <summary>
    /// Genereates a string representation of time consisting of seconds and minutes
    /// </summary>
    /// <param name="seconds">Amount of seconds in the timespan</param>
    /// <returns>String representation of the timespan</returns>
    public static string TimeToString(float seconds)
    {
        if (seconds > 3599) { return "A lot"; }


        float mins = MathF.Floor(seconds / 60);
        seconds = MathF.Floor(seconds % 60);
        return (mins < 10 ? "0" : "") + mins + " : " + (seconds < 10 ? "0" : "") + seconds;
    }

    /// <summary>
    /// Initializes a boss data
    /// </summary>
    /// <param name="loadContent">Whether or not to load textures</param>
    public void InitBoss(bool loadContent = false)
    {
        List<Texture2D> frames = new List<Texture2D>();

        if (loadContent)
        {
            frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile000"));
            frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile001"));
            frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile002"));
            frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile003"));
            frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile004"));
            frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile005"));
            frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile006"));
            frames.Add(Content.Load<Texture2D>("Resources/ComputerFrames/tile007"));
        }
        CurBoss = new Boss(new Vector2(250, 100), loadContent ? frames : CurBoss.Anim.Frames, new Texture(null, new Vector2(2, 2)), 23, 1, new Vector2[4] { new Vector2(100, 100), new Vector2(100, 200), new Vector2(WindowResolution.X - 100, 100), new Vector2(WindowResolution.X-100, 200) }, 250, 0.0075f, 5, 55, loadContent ? Content.Load<SoundEffect>("Resources/explosion") : CurBoss.DeathSound);
        
        frames = new List<Texture2D>();
        frames.Add(Content.Load<Texture2D>("Resources/LightningFrames/tile000"));

        CurBoss.AttackPattern = new SphereAndLightningAttack(new Vector2(0.6f, 1.1f), 10, 8, 100, 5, CurBoss, new Texture(Content.Load<Texture2D>("Resources/ChipBullet"), new Vector2(2, 2)), frames.ToArray(), 2, 2, GameObject.Game.player, new Texture(Content.Load<Texture2D>("Resources/Warning")), Content.Load<SoundEffect>("Resources/RaySound"), Content.Load<SoundEffect>("Resources/WarningSound"), 12 );
    }
    public void LoadExternalData()
    {
        if (File.Exists("ExternalData.json"))
        {
            try
            {
                string jsonString = File.ReadAllText("ExternalData.json");
                PersonalRecord = JsonSerializer.Deserialize<float>(jsonString);
            }
            catch
            {

            }

        }
        else
        {
            File.Create("ExternalData.json");
        }

    }
    public void SaveExternalData()
    {
        if (File.Exists("ExternalData.json"))
        {
            string jsonString = JsonSerializer.Serialize(PersonalRecord);
            File.WriteAllText("ExternalData.json", jsonString);
        }
    }
    protected override void OnExiting(object sender, EventArgs args)
    {
        SaveExternalData();
        base.OnExiting(sender, args);
    }


}







