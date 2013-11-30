using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace EC_Proto
{
	public delegate Entity Spawn(Rectangle position);
	public class GameState {
		private bool drawHitBoxes = false; //For debugging.

		public Game1 gamesystem;

		private GameScene scene;
		private Matrix screenMatrix = Matrix.Identity;
		public int ViewWidth { get; set; }
		public int ViewHeight { get; set; }
		private float zoomLevel = 0.5f;




		public Dictionary<String, Spawn> EntitySpawners = new Dictionary<String, Spawn>();
		Dictionary<String, String> LevelNames = new Dictionary<String, String> ();


		public ContentManager Content;

		public GameState(Game1 gameSystem) {
			//Add in spawning code. Not yet used.
			EntitySpawners.Add("flytrap",delegate(Rectangle position) {return new FlytrapEntity(position);});
			EntitySpawners.Add("torch",delegate(Rectangle position) {return new TorchEntity(position);});
			EntitySpawners.Add("Terrain",delegate(Rectangle position) {return new TerrainEntity(position);});
			EntitySpawners.Add("boulder",delegate(Rectangle position) {return new BoulderEntity(position);});
			EntitySpawners.Add("water",delegate(Rectangle position) {return new WaterEntity(position);});

			scene = new GameScene (this);
			this.gamesystem = gameSystem;
			//Add in level names. Should be read from a file in the future.
			LevelNames.Add ("level1", "Level1/level1.tmx");
			LevelNames.Add ("level2", "Level2/level2.tmx");
			LevelNames.Add ("Third_level", "Level3/Third_level.tmx");
			LevelNames.Add ("central_hub", "central_hub.tmx");
			LevelNames.Add ("lazylogo", "lazylogo.tmx");
			LevelNames.Add ("central_earth", "central_earth.tmx");
			LevelNames.Add ("central_tutorial", "central_tutorial.tmx");
			LevelNames.Add ("central_water", "central_water.tmx");

		}
		public void Update (GameTime gameTime,KeyboardState state, KeyboardState prevState) {


			//TODO: Better event based system? Let entities register as Keyboard listeners, etc.

			scene.Update (gameTime, state, prevState);
			//Fire spawning. Should later be handled my some spell managing class.
			if (state.IsKeyDown (Keys.H) && prevState.IsKeyUp(Keys.H)) { //Use prev state to simulate onKeyDown
				FireballEntity fireball = new FireballEntity (scene.player.Center(), scene.player.direction, scene.player.getCurrentSpeed());
				scene.AddSpellEntity (fireball);
			}

			//Frost spawning.
			if (state.IsKeyDown (Keys.J) && prevState.IsKeyUp (Keys.J)) {
				FrostEntity frost = new FrostEntity (scene.player.Center() + new Vector2(7,10), scene.player.direction, scene.player.getCurrentSpeed());
				scene.AddSpellEntity (frost);
			}

			if (state.IsKeyDown (Keys.K) && prevState.IsKeyUp (Keys.K)) {
				scene.player.EarthenShield ();
			}


			//Debug code!
			if (state.IsKeyDown (Keys.F3) && prevState.IsKeyUp(Keys.F3))
				drawHitBoxes = !drawHitBoxes;

			if (state.IsKeyDown (Keys.F4) && prevState.IsKeyUp (Keys.F4))
				LoadMap ("lazylogo", "default");

		
			screenMatrix = CameraManager.LookAtPoint (new Point ((int)scene.player.Center().X, (int)scene.player.Center().Y), ViewWidth, ViewHeight, zoomLevel, scene.SceneWidth, scene.SceneHeight);



		}



		public void AnimationTick() {
			scene.AnimationTick ();
		}



		public void LoadMap(String mapName, String locationTarget) {
			if (!LevelNames.ContainsKey (mapName))
				return; //No such map! Do nothing.

			String mapfile = LevelNames [mapName];

			scene = new TmxScene (this);
			((TmxScene)scene).LoadMap (mapfile,locationTarget,Content);



			screenMatrix = Matrix.Identity;
		}

		public void LoadScene(String sceneName) {
			//TODO: specify these!
			if (sceneName == "logo") {
				scene = new ImageScene (this, Content.Load<Texture2D> ("tempintro"), "game");
			}
			if (sceneName == "game") {
				scene = new TmxScene(this);
				((TmxScene)scene).LoadMap (LevelNames["central_tutorial"],"default",Content);
			}
		}

		public void Draw( SpriteBatch spriteBatch, GraphicsDeviceManager graphics) {
			scene.Draw (screenMatrix, spriteBatch, graphics, drawHitBoxes);
		}
	}
}

