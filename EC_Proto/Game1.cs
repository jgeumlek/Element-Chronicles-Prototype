#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
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
		Texture2D blankTex; //For drawing rectangles!
		int tickcount = 0; //For dividing the framerate. This implementation will change.

		//Should probably make a class to hold the game state.
		private bool drawHitBoxes = false; //For debugging.
		private PlayerEntity player;
		private KeyboardState prevState;
		private Matrix screenMatrix;
		private GameMap map; //Game state, scene handling, level changes still need work.


		public List<FireballEntity> projectileEntities = new List<FireballEntity> ();
		public List<TerrainEntity> terrainEntities = new List<TerrainEntity> ();
		public List<TorchEntity> torchEntities = new List<TorchEntity> ();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
			graphics.IsFullScreen = false;
			//TargetElapsedTime = TimeSpan.FromMilliseconds(500);
			//graphics.PreferredBackBufferHeight = 720;
			//graphics.PreferredBackBufferWidth = 1280;
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
			player = new PlayerEntity (new Vector2 (500, 400), playertex);
			PlayerEntity.InitAnimation ();
			FlytrapEntity.InitAnimation ();

			FlytrapEntity.spritesheet = Content.Load<Texture2D> ("flytrap");
			TorchEntity.torchUnlit = Content.Load<Texture2D>("torchunlit");
			TorchEntity.torchLit = Content.Load<Texture2D>("torchlit");

			blankTex = new Texture2D(GraphicsDevice, 1, 1);
			blankTex.SetData(new Color[] { Color.White });

			screenMatrix = Matrix.Identity;

			map = new GameMap ("Content/First_level_torch_maze.tmx");
			map.Load (Content,terrainEntities);

            //TODO: use this.Content to load your game content here 
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

			tickcount++;
			if (tickcount == 3) //Divide framerate by three, should get 20fps animations.
				tickcount = 0;
            // Back or Escape to quit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}
            // TODO: Add your update logic here		

			KeyboardState state = Keyboard.GetState ();

			//TODO: Better event based system? Let entities register as Keyboard listeners, etc.

			player.Update (state, gameTime);
			if (tickcount == 0)
				player.AnimationTick ();

			foreach (FireballEntity e in projectileEntities) {
				if (e.Active) e.Update (state, gameTime); 
			}

			foreach (TerrainEntity e in terrainEntities) {
				e.Update (state, gameTime);
				if (tickcount == 0)
					e.AnimationTick ();
			}
			terrainEntities = terrainEntities.Where( x => x.Alive()).ToList();
			projectileEntities = projectileEntities.Where( x => x.Alive()).ToList();
			DetectCollisions ();


			//Fire spawning. Should later be handled my some spell managing class.
			if (state.IsKeyDown (Keys.A) && prevState.IsKeyUp(Keys.A)) { //Use prev state to simulate onKeyDown
				FireballEntity fireball = new FireballEntity (player.position, firetex, player.direction, player.getCurrentSpeed());
				projectileEntities.Add (fireball);
			}

			//Toggle Fullscreen with a common shortcut. 
			if (state.IsKeyDown(Keys.RightAlt) && state.IsKeyDown(Keys.Enter)  && (prevState.IsKeyUp(Keys.Enter) || prevState.IsKeyUp(Keys.RightAlt)))
			{
				graphics.ToggleFullScreen ();
				graphics.ApplyChanges ();
			}

			//Debug code!
			if (state.IsKeyDown (Keys.F3) && prevState.IsKeyUp(Keys.F3))
				drawHitBoxes = !drawHitBoxes;

            base.Update(gameTime);
			prevState = state;


			//These calculations requirement significant refinement! They are pretty naive in several ways.
			screenMatrix = Matrix.CreateTranslation (-player.position.X, -player.position.Y, 1);

			screenMatrix *= Matrix.CreateScale (1.5f, 1.5f, 1);
			screenMatrix *= Matrix.CreateTranslation (300,200,0);

        }

		private void DetectCollisions() {

			//Care should be taken to not check the same pair of objects twice.
			//Let's check each projectile with the terrain and the enemies.
			foreach (Entity e in projectileEntities) {

				Rectangle proj_rect = e.getHitBox ();

				foreach (TerrainEntity terrain in terrainEntities) {

					if (proj_rect.Intersects (terrain.getHitBox())) {
						e.CollidedWith (terrain);
						if (e is FireballEntity) {
							terrain.CollidedWith (e);
						}
					}
				}

				

			}

			//Now let's check player collisions.

			Rectangle playerbox = player.getHitBox ();
			foreach (TerrainEntity terrain in terrainEntities) {
				if (playerbox.Intersects(terrain.getHitBox())) {
					player.CollidedWith(terrain);
				}
			}

		}


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
           	graphics.GraphicsDevice.Clear(Color.Tan);
		
            //TODO: Screen transformations, like camera translation.
			spriteBatch.Begin(SpriteSortMode.Deferred,null, null, null, null, null,screenMatrix);
			//graphics.GraphicsDevice.SamplerStates[0].Filter = TextureFilter.Point; Fixes blurred sprites, but requires Sort mode immediate?
			foreach (TiledMax.Layer layer in map.Layers) {
				int width = layer.Width;
				int height = layer.Height;
				for (int y = 0; y < layer.Height; y++) {
					for (int x = 0; x < layer.Width; x++) {
						Rectangle destination = new Rectangle (x * map.TileWidth, y * map.TileHeight, map.TileWidth, map.TileHeight);
						int tileID = layer.Data [x, y] - 1;
						//tileID = 3;
						if (tileID > 0) {
							GameTile tile = map.Tiles [tileID];
							 if (tile.SpawnType.Equals("")) spriteBatch.Draw (tile.display.texture, destination, tile.display.rect, Color.White);
						}
					}
				}
			}
		


			foreach (FireballEntity e in projectileEntities) { //Currently just the fireballs.
				if (e.Visible) spriteBatch.Draw (e.getTexture (), e.position, Color.White);
				if (drawHitBoxes) //Debugging
					spriteBatch.Draw (blankTex, e.getHitBox (), Color.White);
			}
			foreach (TerrainEntity e in terrainEntities) {
				if (e.Visible)
					spriteBatch.Draw (e.getTexture (), e.position, e.spriteChoice.rect, Color.White);
				if (drawHitBoxes) //Debugging
					spriteBatch.Draw (blankTex, e.getHitBox (), Color.White);
			}
			spriteBatch.Draw (player.getTexture (),player.position, player.spriteChoice.rect, Color.White);

			if (drawHitBoxes)
				spriteBatch.Draw (blankTex, player.getHitBox (), Color.Aquamarine); //Debugging!

			spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

