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

		GameState game;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = false;
			//graphics.PreferredBackBufferHeight = 720;
			//graphics.PreferredBackBufferWidth = 1280;
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

			//We initialize all content that will ever be used for convenience.
			//Perhaps in the future we can load/unload as needed.

			game.Content = Content;
			AudioHandler.content = Content;
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

			// Spell graphics
			PlayerEntity.texture = Content.Load<Texture2D>("charsheet");
			FireballEntity.texture = Content.Load<Texture2D> ("fire");
			FrostEntity.texture = Content.Load<Texture2D> ("frost");
			PlayerRocksEntity.spritesheet = Content.Load<Texture2D> ("FallingRocks");
			PlayerWindEntity.spritesheet = Content.Load<Texture2D> ("windwalk");

			// HUD elements
			Gui.hpBarBackground = Content.Load<Texture2D> ("barBackground");
			Gui.hpBar = Content.Load<Texture2D> ("hpBar");
			Gui.manaBarBackground = Content.Load<Texture2D> ("barBackground");
			Gui.manaBar = Content.Load<Texture2D> ("manaBar");
			Gui.expBarBackground = Content.Load<Texture2D> ("barBackground");
			Gui.expBar = Content.Load<Texture2D> ("expBar");

			// Dialog elements
			Gui.textBox = Content.Load<Texture2D> ("textboxbg");
			GuiText.dialogFont = Content.Load<SpriteFont> ("SpriteFont1");

			//TODO: Read in animation frames from a file.

			PlayerEntity.InitAnimation ();
			FireballEntity.InitAnimation ();
			FrostEntity.InitAnimation ();
			PlayerRocksEntity.InitAnimation ();
			PlayerWindEntity.InitAnimation ();
			FlytrapEntity.InitAnimation ();
			WolfEntity.InitAnimation ();
			FireElementalEntity.InitAnimation ();
			GateEntity.InitAnimation ();
			PressurePlateEntity.InitAnimation ();

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
			GateEntity.gateTex = Content.Load<Texture2D>("gate");
			PressurePlateEntity.plateTex = Content.Load<Texture2D>("pressurePlate");

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

