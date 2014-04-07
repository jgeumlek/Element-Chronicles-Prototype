using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;

namespace EC_Proto
{
	public delegate Entity Spawn(Rectangle position);
	public class GameState {

		private bool drawHitBoxes = false; //For debugging.

		public Game1 gamesystem;

		private static GameScene scene;
		private Matrix screenMatrix = Matrix.Identity;
		public int ViewWidth { get; set; }
		public int ViewHeight { get; set; }
		private float zoomLevel = 0.5f;
		SoundEffect bgm;
		SoundEffect title;
		private bool projectileLaunched = false;
		TimeSpan timer = new TimeSpan (0, 0, 0, 0, 500);
		public Dictionary<String, Spawn> EntitySpawners = new Dictionary<String, Spawn>();
		Dictionary<String, String> LevelNames = new Dictionary<String, String> ();

		public bool tutorialComplete = false;

		SoundEffectInstance bgmInstance;
		SoundEffectInstance titleInstance;
		SoundEffectInstance airZoneInstance;

		bool airPlaying = false;

		public ContentManager Content;

		public GameState(Game1 gameSystem) {
			//Add in spawning code. Not yet used.
			EntitySpawners.Add("flytrap",delegate(Rectangle position) {return new FlytrapEntity(position);});
			//EntitySpawners.Add ("wolf", delegate(Rectangle position) {return new WolfEntity (position);});
			EntitySpawners.Add ("fireElemental", delegate(Rectangle position) {return null;});
			EntitySpawners.Add("torch",delegate(Rectangle position) {return new TorchEntity(position);});
			EntitySpawners.Add("Terrain",delegate(Rectangle position) {return new TerrainEntity(position);});
			EntitySpawners.Add("boulder",delegate(Rectangle position) {return new BoulderEntity(position);});
			EntitySpawners.Add("water",delegate(Rectangle position) {return new WaterEntity(position);});
			EntitySpawners.Add ("nothing", delegate(Rectangle position) {return null;});

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
			LevelNames.Add ("central_fire", "central_fire.tmx");
			LevelNames.Add ("central_air", "central_air.tmx");
		}
		public void Update (GameTime gameTime,KeyboardState state, KeyboardState prevState) {
			//TODO: Better event based system? Let entities register as Keyboard listeners, etc.

			scene.Update (gameTime, state, prevState);

			PlayerStats.Update (gameTime);
			if (PlayerStats.curHp <= 0) {
				if (!tutorialComplete) {
					LoadMap ("central_tutorial", "default");
				} else {
					LoadMap ("central_hub", "respawn");
				}
				PlayerStats.Respawn ();
			}

			Gui.Update (gameTime);

			//Fire spawning. Should later be handled my some spell managing class.
			if (!projectileLaunched && GameScene.player.HasEnoughMana(2) && state.IsKeyDown (Keys.H) && prevState.IsKeyUp(Keys.H)) { //Use prev state to simulate onKeyDown
				FireballEntity fireball = new FireballEntity (GameScene.player.Center(), GameScene.player.direction, GameScene.player.getCurrentSpeed());
				scene.AddSpellEntity (fireball);
				projectileLaunched = true;
				GameScene.player.ConsumeMana (2);
			}

			//Frost spawning.
			if (!projectileLaunched && GameScene.player.HasEnoughMana(1) && state.IsKeyDown (Keys.J) && prevState.IsKeyUp (Keys.J)) {
				FrostEntity frost = new FrostEntity (GameScene.player.Center() + new Vector2(7,10), GameScene.player.direction, GameScene.player.getCurrentSpeed());
				scene.AddSpellEntity (frost);
				projectileLaunched = true;
				GameScene.player.ConsumeMana (1);
			}

			if (projectileLaunched) {
				if (timer > TimeSpan.Zero) {
					timer -= gameTime.ElapsedGameTime;
				} else {
					projectileLaunched = false;
					timer = new TimeSpan (0, 0, 0, 0, 500);
				}
			}

			if (GameScene.player.HasEnoughMana(5) && state.IsKeyDown (Keys.K) && prevState.IsKeyUp (Keys.K)) {
				GameScene.player.EarthenShield ();
			}

			if (GameScene.player.HasEnoughMana(3) && state.IsKeyDown (Keys.L) && prevState.IsKeyUp (Keys.L)) {
				GameScene.player.TornadoJump ();
			}

			//Debug code!
			if (state.IsKeyDown (Keys.F3) && prevState.IsKeyUp(Keys.F3))
				drawHitBoxes = !drawHitBoxes;

			screenMatrix = CameraManager.LookAtPoint (new Point ((int)GameScene.player.Center().X, (int)GameScene.player.Center().Y), ViewWidth, ViewHeight, zoomLevel, scene.SceneWidth, scene.SceneHeight);

		}



		public void AnimationTick() {
			scene.AnimationTick ();
		}



		public void LoadMap(String mapName, String locationTarget) {
			if (!LevelNames.ContainsKey (mapName))
				return; //No such map! Do nothing.

			Console.Out.WriteLine ("Map Name: " + mapName);
			if (mapName == "central_air") {
				airPlaying = true;

				bgmInstance.Stop ();
				bgmInstance.Dispose ();
				bgm = Content.Load<SoundEffect> ("AirZone.wav");
				bgmInstance = bgm.CreateInstance ();
				bgmInstance.IsLooped = true;
				bgmInstance.Play ();
			} else if (airPlaying) {
				airPlaying = false;

				bgmInstance.Stop ();
				bgmInstance.Dispose ();
				bgm = Content.Load<SoundEffect> ("bgm.wav");
				bgmInstance = bgm.CreateInstance ();
				bgmInstance.IsLooped = true;
				bgmInstance.Play ();
			}

			String mapfile = LevelNames [mapName];

			scene = new TmxScene (this);
			((TmxScene)scene).LoadMap (mapfile,locationTarget,Content);

			if (!tutorialComplete && mapfile != "central_tutorial.tmx") {
				tutorialComplete = true;
			}

			screenMatrix = Matrix.Identity;
		}

		public void LoadScene(String sceneName) {
			Gui.sceneName = sceneName;

			//TODO: specify these!
			if (sceneName == "logo") {
				scene = new ImageScene (this, Content.Load<Texture2D> ("TitlePage"), "instruction");
				title = Content.Load<SoundEffect>("TitleSong.wav");
				titleInstance = title.CreateInstance();
				titleInstance.IsLooped = true;
				titleInstance.Play ();
			}
			if (sceneName == "instruction") {
				scene = new ImageScene (this, Content.Load<Texture2D> ("tempintro"), "game");
			}
			if (sceneName == "game") {
				scene = new TmxScene(this);
				((TmxScene)scene).LoadMap (LevelNames["central_tutorial"],"default",Content);

				titleInstance.Stop ();
				bgm = Content.Load<SoundEffect>("bgm.wav");
				bgmInstance = bgm.CreateInstance();
				bgmInstance.IsLooped = true;
				bgmInstance.Play();
			}
		}

		public void Draw( SpriteBatch spriteBatch, GraphicsDeviceManager graphics) {
			scene.Draw (screenMatrix, spriteBatch, graphics, drawHitBoxes);
			if(Gui.sceneName == "game")
				Gui.Draw (screenMatrix, spriteBatch, graphics);
		}

		public static GameScene Scene() {
			return scene;
		}
	}
}

