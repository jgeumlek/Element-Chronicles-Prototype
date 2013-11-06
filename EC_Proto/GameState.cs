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
		private GameMap map; //Game state, scene handling, level changes still need work.

		public Dictionary<String, Spawn> EntitySpawners = new Dictionary<String, Spawn>();

		public List<FrostEntity> frostEntities = new List<FrostEntity> ();
		public List<FireballEntity> projectileEntities = new List<FireballEntity> ();
		public List<TerrainEntity> terrainEntities = new List<TerrainEntity> ();
		public List<TorchEntity> torchEntities = new List<TorchEntity> ();

		public ContentManager Content;

		public GameState() {
			EntitySpawners.Add("flytrap",delegate(Rectangle position) {return new FlytrapEntity(position);});
			EntitySpawners.Add("torch",delegate(Rectangle position) {return new TorchEntity(position);});
			EntitySpawners.Add("Terrain",delegate(Rectangle position) {return new TerrainEntity(position);});


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

			terrainEntities = terrainEntities.Where( x => x.Alive()).ToList();
			projectileEntities = projectileEntities.Where( x => x.Alive()).ToList();
			frostEntities = frostEntities.Where (x => x.Alive ()).ToList ();
			DetectCollisions ();


			//Fire spawning. Should later be handled my some spell managing class.
			if (state.IsKeyDown (Keys.A) && prevState.IsKeyUp(Keys.A)) { //Use prev state to simulate onKeyDown
				FireballEntity fireball = new FireballEntity (player.position, player.direction, player.getCurrentSpeed());
				projectileEntities.Add (fireball);
			}

			//Frost spawning.
			if (state.IsKeyDown (Keys.S) && prevState.IsKeyUp (Keys.S)) {
				FrostEntity frost = new FrostEntity (player.position + new Vector2(7,10), player.direction);
				frostEntities.Add (frost);
			}



			//Debug code!
			if (state.IsKeyDown (Keys.F3) && prevState.IsKeyUp(Keys.F3))
				drawHitBoxes = !drawHitBoxes;

		



			//These calculations requirement significant refinement! They are pretty naive in several ways.
			screenMatrix = Matrix.CreateTranslation (-player.position.X, -player.position.Y, 1);

			screenMatrix *= Matrix.CreateScale (1.5f, 1.5f, 1);
			screenMatrix *= Matrix.CreateTranslation (300,200,0);
		}

		public void AnimationTick() {
			player.AnimationTick ();
			foreach (Entity e in projectileEntities) {
				e.AnimationTick ();
			}
			foreach (Entity e in terrainEntities) {
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
						if (e is FrostEntity) {
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
			spriteBatch.Draw (player.getTexture (),player.position, player.spriteChoice.rect, Color.White);

			if (drawHitBoxes)
				spriteBatch.Draw (Game1.blankTex, player.getHitBox (), Color.Aquamarine); //Debugging!

			spriteBatch.End();

		}

		public void SpawnEntity(String entityType, Rectangle position) {
			//Ideally use EntitySpawners dictionary, and add entity to appropriate list based on type!
			switch (entityType) {
			case "torch":
				torchEntities.Add (new TorchEntity (position));
				break;
			case "flytrap":
				terrainEntities.Add (new FlytrapEntity (position));
				break;
			case "Terrain":
				terrainEntities.Add (new TerrainEntity (position));
				break;
			}

		}

		public void LoadMap(String mapName) {
			player = new PlayerEntity (new Vector2 (400, 400));
			map = new GameMap (mapName);
			map.Load (Content,this);
			screenMatrix = Matrix.Identity;
		}
	}
}

