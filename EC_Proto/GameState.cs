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
		private PlayerEntity player;

		private Matrix screenMatrix = Matrix.Identity;
		public int ViewWidth { get; set; }
		public int ViewHeight { get; set; }
		private float zoomLevel = 1.5f;

		private GameMap map; //Game state, scene handling, level changes still need work.

		public Dictionary<String, Spawn> EntitySpawners = new Dictionary<String, Spawn>();
		Dictionary<String, String> LevelNames = new Dictionary<String, String> ();

		public List<FrostEntity> frostEntities = new List<FrostEntity> ();
		public List<FireballEntity> projectileEntities = new List<FireballEntity> ();
		public List<TerrainEntity> terrainEntities = new List<TerrainEntity> ();
		public List<BoulderEntity> movableEntities = new List<BoulderEntity> ();

		public ContentManager Content;

		public GameState() {
			//Add in spawning code. Not yet used.
			EntitySpawners.Add("flytrap",delegate(Rectangle position) {return new FlytrapEntity(position);});
			EntitySpawners.Add("torch",delegate(Rectangle position) {return new TorchEntity(position);});
			EntitySpawners.Add("Terrain",delegate(Rectangle position) {return new TerrainEntity(position);});
			EntitySpawners.Add("boulder",delegate(Rectangle position) {return new BoulderEntity(position);});

			//Add in level names. Should be read from a file in the future.
			LevelNames.Add ("level1", "Level1/level1.tmx");
			LevelNames.Add ("level2", "Level2/level2.tmx");
			LevelNames.Add ("Third_level", "Level3/Third_level.tmx");
		}
		public void Update (GameTime gameTime,KeyboardState state, KeyboardState prevState) {


			//TODO: Better event based system? Let entities register as Keyboard listeners, etc.

			player.Update (state, gameTime);


			foreach (FireballEntity e in projectileEntities) {
				if (e.Active) e.Update (state, gameTime); 
			}

			foreach (FrostEntity e in frostEntities) {
				if (e.Active) e.Update (state, gameTime);
			}

			foreach (TerrainEntity e in terrainEntities) {
				e.Update (state, gameTime);
			}

			foreach (BoulderEntity e in movableEntities) {
				e.Update (state, gameTime);
			}

			terrainEntities = terrainEntities.Where( x => x.Alive()).ToList();
			projectileEntities = projectileEntities.Where( x => x.Alive()).ToList();
			frostEntities = frostEntities.Where (x => x.Alive ()).ToList ();
			movableEntities = movableEntities.Where( x => x.Alive()).ToList();
			DetectCollisions ();


			//Fire spawning. Should later be handled my some spell managing class.
			if (state.IsKeyDown (Keys.Up) && prevState.IsKeyUp(Keys.Up)) { //Use prev state to simulate onKeyDown
				FireballEntity fireball = new FireballEntity (player.position, player.direction, player.getCurrentSpeed());
				projectileEntities.Add (fireball);
			}

			//Frost spawning.
			if (state.IsKeyDown (Keys.Down) && prevState.IsKeyUp (Keys.Down)) {
				FrostEntity frost = new FrostEntity (player.position + new Vector2(7,10), player.direction, player.getCurrentSpeed());
				frostEntities.Add (frost);
			}

			if (state.IsKeyDown (Keys.Left) && prevState.IsKeyUp (Keys.Left)) {
				player.EarthenShield ();
			}


			//Debug code!
			if (state.IsKeyDown (Keys.F3) && prevState.IsKeyUp(Keys.F3))
				drawHitBoxes = !drawHitBoxes;

		
			ScreenCalculate ();



		}

		private void ScreenCalculate() {
			//These calculations requirement significant refinement! They are pretty naive in several ways.
			screenMatrix = Matrix.CreateTranslation (-player.position.X, -player.position.Y, 1);

			screenMatrix *= Matrix.CreateScale (zoomLevel, zoomLevel, 1);
			screenMatrix *= Matrix.CreateTranslation (300,200,0);

			Matrix invScreen = Matrix.Invert (screenMatrix);
			Vector2 topLeft = Vector2.Transform(new Vector2(0,0), invScreen);
			Vector2 bottomRight = Vector2.Transform (new Vector2 (ViewWidth, ViewHeight), invScreen);
		
			//Clip with map boundaries! If a corner coordinate is valid, no translation needed.
			topLeft.X = topLeft.X < 0 ? topLeft.X : 0;
			topLeft.Y = topLeft.Y < 0 ? topLeft.Y : 0;

			bottomRight.X = bottomRight.X > map.MapWidth  ? bottomRight.X - map.MapWidth  : 0;
			bottomRight.Y = bottomRight.Y > map.MapHeight ? bottomRight.Y - map.MapHeight : 0;

			screenMatrix *= Matrix.CreateTranslation (zoomLevel*topLeft.X,zoomLevel*topLeft.Y,0); //Fix scrolling too far left/up
			screenMatrix *= Matrix.CreateTranslation (zoomLevel*bottomRight.X,zoomLevel*bottomRight.Y,0); //Fix scrolling too far right/down


		}

		public void AnimationTick() {
			player.AnimationTick ();
			foreach (Entity e in projectileEntities) {
				e.AnimationTick ();
			}
			foreach (Entity e in frostEntities) {
				e.AnimationTick ();
			}
			foreach (Entity e in terrainEntities) {
				e.AnimationTick ();
			}
			foreach (Entity e in movableEntities) {
				e.AnimationTick ();
			}
		}

		public void DetectCollisions() {
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

				foreach (BoulderEntity boulder in movableEntities) {
					if (proj_rect.Intersects (boulder.getHitBox())) {
						e.CollidedWith (boulder);
						if (e is FireballEntity) {
							boulder.CollidedWith (e);
						}
					}
				}
			}

			foreach (Entity e in frostEntities) {
				Rectangle frost_rect = e.getHitBox ();

				foreach (TerrainEntity terrain in terrainEntities) {
					if (frost_rect.Intersects (terrain.getHitBox ())) {
						e.CollidedWith (terrain);
						if (e is FrostEntity) {
							terrain.CollidedWith (e);
						}
					}
				}

				foreach (BoulderEntity boulder in movableEntities) {
					if (frost_rect.Intersects (boulder.getHitBox ())) {
						e.CollidedWith (boulder);
						if (e is FrostEntity) {
							boulder.CollidedWith (e);
						}
					}
				}
			}

			//Now let's check player collisions.

			Rectangle playerbox = player.getHitBox ();
			foreach (TerrainEntity terrain in terrainEntities) {
				if (playerbox.Intersects(terrain.getHitBox())) {
					player.CollidedWith(terrain);
					terrain.CollidedWith (player);
				}
				foreach (BoulderEntity boulder in movableEntities) {
					if (terrain.getHitBox ().Intersects (boulder.getHitBox ())) {
						boulder.CollidedWith (terrain);
						terrain.CollidedWith (boulder);
					}
				}
			}

			foreach (BoulderEntity boulder in movableEntities) {
				if (playerbox.Intersects(boulder.getHitBox())) {
					player.CollidedWith(boulder);
					if (player.strength)
						boulder.CollidedWith (player);
				}
				foreach (TerrainEntity terrain in terrainEntities) {
					if (boulder.getHitBox ().Intersects (terrain.getHitBox ())) {
						terrain.CollidedWith (boulder);
						boulder.CollidedWith (terrain);
					}
				}
				foreach (BoulderEntity b in movableEntities) {
					if (boulder != b && boulder.getHitBox ().Intersects (b.getHitBox ())) {
						boulder.CollidedWith (b);
						b.CollidedWith (boulder);
					}
				}
			}
		}

		//TODO: Still needs implementation!
		public void ResolveCollisions() {
		}


		public void Draw(SpriteBatch spriteBatch) {
		//TODO: Screen transformations, like camera translation.
			spriteBatch.Begin(SpriteSortMode.Deferred,null, null, null, null, null,screenMatrix);
			//graphics.GraphicsDevice.SamplerStates[0].Filter = TextureFilter.Point; Fixes blurred sprites, but requires Sort mode immediate?
			foreach (TiledMax.Layer layer in map.Layers) {

				int width = layer.Width;
				int height = layer.Height;
				for (int y = 0; y < layer.Height; y++) {
					for (int x = 0; x < layer.Width; x++) {
						Rectangle destination = new Rectangle (x * map.TileWidth, y * map.TileHeight, map.TileWidth, map.TileHeight);
						int tileID = layer.Data [y, x] - 1;
						//tileID = 3;
						if (tileID > 0) {
							GameTile tile = map.Tiles [tileID];
							if (!EntitySpawners.ContainsKey(tile.SpawnType)) spriteBatch.Draw (tile.display.texture, destination, tile.display.rect, Color.White);
						}
					}
				}
			}



			foreach (FireballEntity e in projectileEntities) { //Currently just the fireballs.
				if (e.Visible) spriteBatch.Draw (e.getTexture (), e.position, Color.White);
				if (drawHitBoxes) //Debugging
					spriteBatch.Draw (Game1.blankTex, e.getHitBox (), Color.White);
			}
			foreach (FrostEntity e in frostEntities) {
				if (e.Visible)
					spriteBatch.Draw (e.getTexture (), e.position, Color.White);
				if (drawHitBoxes) //Debugging
					spriteBatch.Draw (Game1.blankTex, e.getHitBox (), Color.White);
			}
			foreach (TerrainEntity e in terrainEntities) {
				if (e.Visible)
					spriteBatch.Draw (e.getTexture (), e.position, e.spriteChoice.rect, Color.White);
				if (drawHitBoxes) //Debugging
					spriteBatch.Draw (Game1.blankTex, e.getHitBox (), Color.White);
			}
			foreach (BoulderEntity e in movableEntities) {
				if (e.Visible)
					spriteBatch.Draw (e.getTexture (), e.position, e.spriteChoice.rect, Color.White);
				if (drawHitBoxes) //Debugging
					spriteBatch.Draw (Game1.blankTex, e.getHitBox (), Color.White);
			}
			spriteBatch.Draw (player.getTexture (),player.position, player.spriteChoice.rect, Color.White);

			if (drawHitBoxes)
				spriteBatch.Draw (Game1.blankTex, player.getHitBox (), Color.Aquamarine); //Debugging!

			spriteBatch.End();

		}

		public void SpawnEntity(String entityType, Rectangle position) {
			//Ideally use EntitySpawners dictionary, and add entity to appropriate list based on type!
			switch (entityType) {
			case "torch":
				terrainEntities.Add (new TorchEntity (position));
				break;
			case "flytrap":
				terrainEntities.Add (new FlytrapEntity (position));
				break;
			case "Terrain":
				terrainEntities.Add (new TerrainEntity (position));
				break;
			case "water":
				terrainEntities.Add (new WaterEntity (position));
				break;
			case "boulder":
				movableEntities.Add (new BoulderEntity (position));
				break;
			case "player":
				Point center = position.Center;
				player.SetResetPosition (new Vector2(center.X - 16, center.Y -16)); //HACK: -16 to reach top left corner! Should probably clean up player entity interface.
				player.ResetWarp ();
				break;
			}

		}

		public void AddLoadTrigger (String mapName,Rectangle position) {
			terrainEntities.Add (new WarpTrigger (mapName, position, this));

		}

		public void LoadMap(String mapName) {
			if (!LevelNames.ContainsKey (mapName))
				return; //No such map! Do nothing.

			String mapfile = LevelNames [mapName];

			frostEntities = new List<FrostEntity> ();
			projectileEntities = new List<FireballEntity> ();
			terrainEntities = new List<TerrainEntity> ();
			movableEntities = new List<BoulderEntity> ();


			player = new PlayerEntity (new Vector2 (400, 400));
			map = new GameMap ("Content/" + mapfile);
			map.Load (Content,this);
			screenMatrix = Matrix.Identity;
		}
	}
}

