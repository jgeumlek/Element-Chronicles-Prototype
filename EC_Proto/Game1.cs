#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;



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
		Texture2D playertex;
		Texture2D firetex;
		private PlayerEntity player;
		private KeyboardState prevState;

		System.Collections.ArrayList entities = new System.Collections.ArrayList();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = true;	

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
				
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
			playertex = Content.Load<Texture2D>("blob");
			firetex = Content.Load<Texture2D> ("fire");
			//TODO: This probably isn't the cleanest spot for initializing the player
			player = new PlayerEntity (new Vector2 (300, 300), playertex);
			entities.Add (player);


            //TODO: use this.Content to load your game content here 
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // Back or Escape to quit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}
            // TODO: Add your update logic here		

			KeyboardState state = Keyboard.GetState ();

			//TODO: Better event based system? Let entities register as Keyboard listeners, etc.

			for (int i = 0; i < entities.Count; i++) {
				Entity e = (Entity)entities [i];
				if (e.Active) e.update (state, gameTime); 
				if (!e.Alive ()) {
					entities.RemoveAt (i);
					i--; //One less element in the list
				}

			}



			//Fire spawning. Should later be handled my some spell managing class.
			if (state.IsKeyDown (Keys.A) && prevState.IsKeyUp(Keys.A)) { //Use prev state to simulate onKeyDown
				FireballEntity fireball = new FireballEntity (player.getPosition(), firetex, player.getDirection(), player.getCurrentSpeed());
				entities.Add (fireball);
			}

			//Toggle Fullscreen with a common shortcut. Still needs work so it doesn't toggle constantly if buttons are held.
			if (state.IsKeyDown(Keys.RightAlt) && state.IsKeyDown(Keys.Enter) )
			{
				graphics.ToggleFullScreen ();
			}

            base.Update(gameTime);
			prevState = state;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
           	graphics.GraphicsDevice.Clear(Color.Tan);
		
            //TODO: Screen transformations, like camera translation.
			spriteBatch.Begin();
			foreach (Entity e in entities) {
				if (e.Visible) spriteBatch.Draw (e.getTexture (), e.getPosition (), Color.White);
			}

			spriteBatch.End();

            base.Draw(gameTime);
        }

		void updatePlayer(GameTime gameTime) {

		}
    }
}
