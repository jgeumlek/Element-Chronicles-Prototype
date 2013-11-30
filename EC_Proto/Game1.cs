#region Using Statements
using System.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace EC_Proto
{
	/// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
		SoundPlayer bgm;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;		
		public static Texture2D blankTex; //For drawing rectangles!
		int tickcount = 0; //For dividing the framerate. This implementation will change.
		private KeyboardState prevState;


		GameState game = new GameState();


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = false;
			//TargetElapsedTime = TimeSpan.FromMilliseconds(500);
			graphics.PreferredBackBufferHeight = 480;
			graphics.PreferredBackBufferWidth = 640;
            Content.RootDirectory = "Content";
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
			PlayerEntity.texture = Content.Load<Texture2D>("blob");
			FireballEntity.texture = Content.Load<Texture2D> ("fire");
			FrostEntity.texture = Content.Load<Texture2D> ("frost");

			bgm = new SoundPlayer ("Content\\bgm.wav");
			bgm.PlayLooping ();

			//TODO: This probably isn't the cleanest spot for initializing the player

			PlayerEntity.InitAnimation ();
			FlytrapEntity.InitAnimation ();

			FlytrapEntity.spritesheet = Content.Load<Texture2D> ("flytrap");
			TorchEntity.torchOff = Content.Load<Texture2D>("torchOff");
			TorchEntity.torchOn = Content.Load<Texture2D>("torchOn");
			WaterEntity.waterTex = Content.Load<Texture2D> ("water");
			WaterEntity.iceTex = Content.Load<Texture2D> ("ice");
			BoulderEntity.texture = Content.Load<Texture2D> ("boulder");

			blankTex = new Texture2D(GraphicsDevice, 1, 1);
			blankTex.SetData(new Color[] { Color.White });
			WarpTrigger.texture = blankTex;

			//game.LoadMap ("First_level_torch_maze.tmx");
			game.LoadMap ("level2");


            //TODO: use this.Content to load your game content here 
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
			KeyboardState state = Keyboard.GetState ();

            // Back or Escape to quit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}
            // TODO: Add your update logic here		
			//Toggle Fullscreen with a common shortcut. 
			if (state.IsKeyDown(Keys.RightAlt) && state.IsKeyDown(Keys.Enter)  && (prevState.IsKeyUp(Keys.Enter) || prevState.IsKeyUp(Keys.RightAlt)))
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
			game.Draw (spriteBatch);
            
            base.Draw(gameTime);
        }
    }
}

