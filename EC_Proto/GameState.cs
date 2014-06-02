using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;


/*
 * Handles the game state.
 */
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
		public Dictionary<String, Spawn> EntitySpawners = new Dictionary<String, Spawn>();
		Dictionary<String, String> LevelNames = new Dictionary<String, String> ();

		public bool tutorialComplete = false;

		String respawnLevel = "START";
		String respawnLocation = "default";

		public ContentManager Content;

		public GameState(Game1 gameSystem) {
			//Add in spawning code. Not yet used.
			Content = gameSystem.Content;
			EntitySpawners.Add("flytrap",delegate(Rectangle position) {return new FlytrapEntity(position);});
			EntitySpawners.Add ("wolf", delegate(Rectangle position) {return null;});
			EntitySpawners.Add ("fireElemental", delegate(Rectangle position) {return null;});
			EntitySpawners.Add("torch",delegate(Rectangle position) {return new TorchEntity(position);});
			EntitySpawners.Add("Terrain",delegate(Rectangle position) {return new TerrainEntity(position);});
			EntitySpawners.Add("boulder",delegate(Rectangle position) {return new BoulderEntity(position);});
			EntitySpawners.Add("pit",delegate(Rectangle position) {return new PitEntity(position);});
			EntitySpawners.Add("water",delegate(Rectangle position) {return new WaterEntity(position);});
			EntitySpawners.Add ("scroll", delegate(Rectangle position) {return null;});
			EntitySpawners.Add ("nothing", delegate(Rectangle position) {return null;});

			scene = new GameScene (this);
			this.gamesystem = gameSystem;

			//Add in level names from a file
			String line;

			//Note that this file name is hard coded!
			System.IO.StreamReader file = 
				new System.IO.StreamReader(Path.Combine(Content.RootDirectory,"level_list.txt"));
			while((line = file.ReadLine()) != null)
			{
				line = line.Trim ();
				if (line.StartsWith ("#") || line.Equals("")) {
					//Skip comments and blank lines.
					continue;
				}

				//Cheesy arrow separator probably won't be in names.
				//There is no current way to escape it; so avoid putting "==>" in names!
				String[] separator = { "==>" };
				String[] fields = line.Split(separator,StringSplitOptions.RemoveEmptyEntries);

				if (fields.Length != 2) {
					Console.Out.WriteLine ("Couldn't parse level list: \"" + line + "\"");
				} else {
					//HACK: These two names are special, and must point to a previously set name.
					if (fields [0].Equals ("TUTORIAL") || fields [0].Equals ("START")) {
						if (LevelNames.ContainsKey (fields [1])) {
							LevelNames.Add (fields [0], LevelNames [fields [1]]);
						} else {
							Console.Error.WriteLine ("Could not set " + fields [0] + ", " + fields [1] + " was not set.");
						}
					} else {
						LevelNames.Add (fields [0], fields [1]);
					}
				}
			}

			file.Close();

			//TODO: Verify "TUTORIAL" and "START" are set, fail appropriately if they are not.





		}
		public void Update (GameTime gameTime,KeyboardState state, KeyboardState prevState) {
			//TODO: Better event based system? Let entities register as Keyboard listeners, etc.

			scene.Update (gameTime, state, prevState);

			PlayerStats.Update (gameTime);
			if (PlayerStats.curHp <= 0) {

				PlayerStats.Respawn ();

				LoadMap (respawnLevel, respawnLocation);
			}

			Gui.Update (gameTime, state, prevState);

			SpellManager.Update (gameTime, state, prevState);

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

			AudioHandler.SetMap (mapName);

			String mapfile = LevelNames [mapName];

			scene = new TmxScene (this);
			((TmxScene)scene).LoadMap (mapfile,locationTarget,Content);

			if (!tutorialComplete && mapfile != LevelNames["TUTORIAL"]) {
				tutorialComplete = true;
				//Set the respawn to whatever level follows the tutorial.
				//This should happen only once.
				respawnLevel = mapName;
				respawnLocation = locationTarget;
			}

			screenMatrix = Matrix.Identity;
		}

		public void LoadScene(String sceneName) {
			Gui.sceneName = sceneName;

			//TODO: specify these!
			if (sceneName == "logo") {
				scene = new ImageScene (this, Content.Load<Texture2D> ("TitlePage"), "instruction");
			}
			if (sceneName == "instruction") {
				scene = new ImageScene (this, Content.Load<Texture2D> ("tempintro"), "game");
			}
			if (sceneName == "game") {
				scene = new TmxScene(this);
				((TmxScene)scene).LoadMap (LevelNames["START"],"default",Content);
			}
			AudioHandler.SetScene (sceneName);
		}

		public void Draw( SpriteBatch spriteBatch, GraphicsDeviceManager graphics) {
			scene.Draw (screenMatrix, spriteBatch, graphics, drawHitBoxes);
			if (Gui.sceneName == "game")
				Gui.Draw (screenMatrix, spriteBatch, graphics);
		}

		public static GameScene Scene() {
			return scene;
		}
	}
}

