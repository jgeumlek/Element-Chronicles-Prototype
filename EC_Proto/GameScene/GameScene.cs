using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace EC_Proto
{
	public class GameScene
	{
		public PlayerEntity player = new PlayerEntity( new Vector2(0,0));
		protected Dictionary<String, Spawn> EntitySpawners;
		protected GameState game;
		public List<FrostEntity> frostEntities = new List<FrostEntity> ();
		public List<Entity> projectileEntities = new List<Entity> ();
		public List<TerrainEntity> terrainEntities = new List<TerrainEntity> ();
                public List<Entity> movableEntities = new List<Entity>();

		public int SceneWidth;
		public int SceneHeight;

		public GameScene (GameState state)
		{
			this.game = state;
			this.EntitySpawners = state.EntitySpawners;
		}

		virtual public void SpawnEntity(String entityType, Rectangle position) {
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

			}

		}

		public void AddLoadTrigger (String mapName,String locationTarget, Rectangle position) {
			terrainEntities.Add (new WarpTrigger (mapName, locationTarget, position, game));

		}

		public void AddSpellEntity(Entity spell) {
			projectileEntities.Add (spell);
		}


		virtual public void Update (GameTime gameTime,KeyboardState state, KeyboardState prevState) {

			// Back or Escape to quit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				game.gamesystem.Exit();
			}


			//TODO: Better event based system? Let entities register as Keyboard listeners, etc.

			player.Update (state, gameTime);


			foreach (Entity e in projectileEntities) {
				if (e.Active) e.Update (state, gameTime); 
			}

			foreach (FrostEntity e in frostEntities) {
				if (e.Active) e.Update (state, gameTime);
			}

			foreach (TerrainEntity e in terrainEntities) {
				e.Update (state, gameTime);

			}

                  	foreach (Entity e in movableEntities) {
				e.Update (state, gameTime);

			}

			terrainEntities = terrainEntities.Where( x => x.Alive()).ToList();
			projectileEntities = projectileEntities.Where( x => x.Alive()).ToList();
			frostEntities = frostEntities.Where (x => x.Alive ()).ToList();
                        movableEntities = movableEntities.Where (x => x.Alive ()).ToList();
			DetectCollisions ();
		}

		virtual public void AnimationTick() {
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
			DetectCollisionsBetween (projectileEntities, terrainEntities);
			DetectCollisionsBetween (frostEntities, terrainEntities);
			DetectCollisionsBetween (player, terrainEntities);
                        DetectCollisionsBetween (player, movableEntities);
                        DetectCollisionsBetween (movableEntities, terrainEntities);
                        DetectCollisionsAmong (movableEntities);

		}

		private void DetectCollisionsBetween<A,B>(List<A> list1, List<B> list2) where A:Entity where B:Entity {
			foreach (Entity e1 in list1) {

				Rectangle rect1 = e1.getHitBox ();

				foreach (Entity e2 in list2) {

					if (rect1.Intersects (e2.getHitBox())) {
						e1.CollidedWith (e2);
						e2.CollidedWith (e1);
					}
				}

			}
		}
		private void DetectCollisionsBetween<E>(Entity entity, List<E> list) where E:Entity {


			Rectangle rect1 = entity.getHitBox ();

			foreach (Entity e2 in list) {

				if (rect1.Intersects (e2.getHitBox())) {
					entity.CollidedWith (e2);
					e2.CollidedWith (entity);
				}
			}


		}
                
        private void DetectCollisionsAmong<E>(List<E> list) where E:Entity {
            
             for (int i = 0; i < list.Count; i++) {
				Entity e1 = list [i];
			    Rectangle rect1 = e1.getHitBox ();

				for (int j = i+1; j < list.Count; j++) {
      					Entity e2 = list[j];

				    	if (rect1.Intersects (e2.getHitBox())) {
				       		e1.CollidedWith (e2);
							e2.CollidedWith (e1);
			            }
			 	  }
              }


		}


		//TODO: Still needs implementation!
		public void ResolveCollisions() {
		}

		virtual public void Draw(Matrix screenMatrix, SpriteBatch spriteBatch, GraphicsDeviceManager graphics, bool drawHitBoxes) {



			spriteBatch.Begin(SpriteSortMode.Deferred,null, null, null, null, null,screenMatrix);


			DrawList (spriteBatch, terrainEntities, drawHitBoxes); 
			DrawList (spriteBatch, projectileEntities, drawHitBoxes);
			DrawList (spriteBatch, frostEntities, drawHitBoxes);
                        DrawList (spriteBatch, movableEntities, drawHitBoxes);

			spriteBatch.Draw (player.getTexture (),player.position, player.spriteChoice.rect, Color.White);
			if (drawHitBoxes)
				spriteBatch.Draw (Game1.blankTex, player.getHitBox (), Color.Aquamarine); //Debugging!

			spriteBatch.End();

		}

		protected void DrawList<E>(SpriteBatch spriteBatch, List<E> entities, bool drawHitBoxes) where E:Entity {
			foreach (Entity e in entities) {
				if (e.Visible)
					spriteBatch.Draw (e.getTexture (), e.position, e.spriteChoice.rect, Color.White);
				if (drawHitBoxes) //Debugging
					spriteBatch.Draw (Game1.blankTex, e.getHitBox (), Color.White);
			}
		}

	}
}

