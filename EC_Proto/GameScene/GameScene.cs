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
						if (e1 is PhysicsEntity && e2 is PhysicsEntity) {
							ResolveCollisions ((PhysicsEntity)e1, (PhysicsEntity)e2);
						}
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
					if (entity is PhysicsEntity && e2 is PhysicsEntity) {
						ResolveCollisions ((PhysicsEntity)entity, (PhysicsEntity)e2);
					}
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
						if (e1 is PhysicsEntity && e2 is PhysicsEntity) {
							ResolveCollisions ((PhysicsEntity)e1, (PhysicsEntity)e2);
						}
			            }
			 	  }
              }


		}


		//TODO: Implementation relies on AABB
		public void ResolveCollisions(PhysicsEntity e1, PhysicsEntity e2) {
			if (e1.inverseMass == 0 && e2.inverseMass == 0)
				return; // Two immovable objects. Nothing to be done.
			if (!e1.Collidable || !e2.Collidable) {
				return; //One of the objects doesn't wish for collsion resolution.
			}
			Vector2 diff = new Vector2 (e1.getHitBox ().Center.X - e2.getHitBox ().Center.X, e1.getHitBox ().Center.Y - e2.getHitBox ().Center.Y);
			Vector2 rel_motion = e1.Momentum - e2.Momentum;
			if (diff.X * rel_motion.X + diff.Y * rel_motion.Y > 0)
				return; //Don't mess with objects moving out of each other!

			float x_depth = (e1.getHitBox().Width + e2.getHitBox().Width) / 2.0f - Math.Abs (e1.getHitBox().Center.X - e2.getHitBox().Center.X);
			float y_depth = (e1.getHitBox().Height + e2.getHitBox().Height) / 2.0f - Math.Abs (e1.getHitBox().Center.Y - e2.getHitBox().Center.Y);

			if (e1.inverseMass == 0) {
				MomentumSink (e2, e1.getHitBox (), x_depth, y_depth);
				return;
			} else if (e2.inverseMass == 0) {
				MomentumSink (e1, e2.getHitBox (), x_depth, y_depth);
				return;
			}

			if ( x_depth < y_depth && x_depth >= Math.Min (e1.getHitBox ().Width, e2.getHitBox ().Width)) {
				diff.X = 0;
			} else if (y_depth >= Math.Min (e1.getHitBox ().Height, e2.getHitBox ().Height)) {
				diff.Y = 0;
			}

			x_depth = Math.Max (0, x_depth);



			y_depth = Math.Max (0, y_depth);
			if (diff.LengthSquared() == 0)
				return;


			x_depth *= Math.Sign (e1.getHitBox().Center.X - e2.getHitBox().Center.X);
			y_depth *= Math.Sign (e1.getHitBox ().Center.Y - e2.getHitBox ().Center.Y);
			diff.Normalize();
			float displac_dot = diff.X * x_depth + diff.Y * y_depth;

			Vector2 displacement = diff * displac_dot;
			diff.Normalize ();
			double totalMass = e1.inverseMass + e2.inverseMass;
			Console.Out.WriteLine (x_depth.ToString () + "," + y_depth.ToString ());
			Console.Out.WriteLine (diff);

			Console.Out.WriteLine (displacement);
			//displacement.Normalize ();
			e1.Impulse (displacement * (float)(e1.inverseMass/totalMass));
			e2.Impulse (-displacement * (float)(e2.inverseMass/totalMass));
			Console.Out.WriteLine (displacement * (float)(e1.inverseMass/totalMass));
			Console.Out.WriteLine (-displacement * (float)(e2.inverseMass/totalMass));



		}

		private void MomentumSink(PhysicsEntity entity, Rectangle immovable, float x_depth, float y_depth) {
			Point entityCenter = entity.getHitBox ().Center;
			Vector2 momentum = entity.Momentum;
			if (x_depth < y_depth && entityCenter.X < immovable.X ) {
				momentum.X = 0;
				entity.moveOffset (new Vector2 (-x_depth, 0));
			} else if (x_depth < y_depth && entityCenter.X > immovable.Right) {
				momentum.X = 0;
				entity.moveOffset (new Vector2 (x_depth, 0));

			}
			if (x_depth > y_depth && entityCenter.Y < immovable.Y ) {
				momentum.Y = 0;
				entity.moveOffset (new Vector2 (0,-y_depth));

			} else if (x_depth > y_depth && entityCenter.Y > immovable.Y + immovable.Height) {
				momentum.Y = 0;
				entity.moveOffset (new Vector2 (0,y_depth));

			}
		}

		virtual public void Draw(Matrix screenMatrix, SpriteBatch spriteBatch, GraphicsDeviceManager graphics, bool drawHitBoxes) {
			spriteBatch.Begin(SpriteSortMode.Deferred,null, null, null, null, null,screenMatrix);

			DrawList (spriteBatch, terrainEntities, drawHitBoxes);
            DrawList (spriteBatch, movableEntities, drawHitBoxes);
			DrawList (spriteBatch, projectileEntities, drawHitBoxes);
			DrawList (spriteBatch, frostEntities, drawHitBoxes);

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

