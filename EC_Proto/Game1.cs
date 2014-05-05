#region Using Statements
using System.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Microsoft.Xna.Framework.Media;

#endregion

namespace EC_Proto
{
	/// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
		public static Texture2D blankTex; //For drawing rectangles!
		int tickcount = 0; //For dividing the framerate. This implementation will change.
		private KeyboardState prevState;

		//Move all of these to a Gui class
		public Texture2D hpBarBackground; // Black bar behind the health bar, which does not change in length
		public Texture2D hpBar;
		public Texture2D manaBarBackground;
		public Texture2D manaBar;
		public Texture2D expBarBackground;
		public Texture2D expBar;

		GameState game;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = false;
			//graphics.PreferredBackBufferHeight = 720;
			//graphics.PreferredBackBufferWidth = 640;
            Content.RootDirectory = "Content";
			game = new GameState (this);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
			prevState = Keyboard.GetState ();
			game.ViewWidth = GraphicsDevice.Viewport.Width;
			game.ViewHeight = GraphicsDevice.Viewport.Height;

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
			game.Content = Content;
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

			PlayerEntity.texture = Content.Load<Texture2D>("spritesheetcharacter1 copy");
			FireballEntity.texture = Content.Load<Texture2D> ("fire");
			FrostEntity.texture = Content.Load<Texture2D> ("frost");
			PlayerRocksEntity.spritesheet = Content.Load<Texture2D> ("FallingRocks");
			PlayerWindEntity.spritesheet = Content.Load<Texture2D> ("windwalk");

			Gui.hpBarBackground = Content.Load<Texture2D> ("barBackground");
			Gui.hpBar = Content.Load<Texture2D> ("hpBar");
			Gui.manaBarBackground = Content.Load<Texture2D> ("barBackground");
			Gui.manaBar = Content.Load<Texture2D> ("manaBar");
			Gui.expBarBackground = Content.Load<Texture2D> ("barBackground");
			Gui.expBar = Content.Load<Texture2D> ("expBar");


			//TODO: This probably isn't the cleanest spot for initializing the player

			PlayerEntity.InitAnimation ();
			FireballEntity.InitAnimation ();
			FrostEntity.InitAnimation ();
			PlayerRocksEntity.InitAnimation ();
			PlayerWindEntity.InitAnimation ();
			FlytrapEntity.InitAnimation ();
			WolfEntity.InitAnimation ();
			FireElementalEntity.InitAnimation ();

			FlytrapEntity.spritesheet = Content.Load<Texture2D> ("flytrap");
			WolfEntity.spritesheet = Content.Load<Texture2D> ("wolf");
			FireElementalEntity.spritesheet = Content.Load<Texture2D> ("fireElemental");
			TorchEntity.torchOff = Content.Load<Texture2D>("torchOff");
			TorchEntity.torchOn = Content.Load<Texture2D>("torchOn");
			WaterEntity.waterTex = Content.Load<Texture2D> ("water");
			WaterEntity.iceTex = Content.Load<Texture2D> ("ice");
			BoulderEntity.texture = Content.Load<Texture2D> ("boulder");
			PitEntity.texture = Content.Load<Texture2D> ("pit");
			ScrollEntity.texture = Content.Load<Texture2D> ("magicscroll");

			blankTex = new Texture2D(GraphicsDevice, 1, 1);
			blankTex.SetData(new Color[] { Color.White });
			WarpTrigger.texture = blankTex;


			MediaPlayer.Volume = 0.5f;

			game.LoadScene ("logo");

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
			KeyboardState state = Keyboard.GetState ();

           
            // TODO: Add your update logic here		
			//Toggle Fullscreen with a common shortcut. 
			if ((state.IsKeyDown(Keys.F11) && prevState.IsKeyUp(Keys.F11)) || state.IsKeyDown(Keys.RightAlt) && state.IsKeyDown(Keys.Enter)  && (prevState.IsKeyUp(Keys.Enter) || prevState.IsKeyUp(Keys.RightAlt)))
			{
				graphics.ToggleFullScreen ();
				graphics.ApplyChanges ();
			}

			game.Update (gameTime, state, prevState);
			tickcount++;
			if (tickcount == 3) { //Divide framerate by three, should get 20fps animations.
				tickcount = 0;
				game.AnimationTick ();
			}

			prevState = state;
			base.Update (gameTime);

        }




        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
           	graphics.GraphicsDevice.Clear(Color.Tan);
			//Perhaps: Hud.draw
			game.Draw (spriteBatch,graphics);
            
            base.Draw(gameTime);
        }
    }
}

