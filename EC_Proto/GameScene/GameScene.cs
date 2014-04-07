using TiledMax;

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
		static public PlayerEntity player = new PlayerEntity( new Vector2(0,0));
		protected Dictionary<String, Spawn> EntitySpawners;
		protected GameState game;
		public List<FrostEntity> frostEntities = new List<FrostEntity> ();
		public List<Entity> projectileEntities = new List<Entity> ();
		public List<TerrainEntity> terrainEntities = new List<TerrainEntity> ();
		//public List<Entity> enemyEntities = new List<Entity>();
		public List<Entity> movableEntities = new List<Entity>();
		public List<NodeEntity> nodeEntities = new List<NodeEntity> ();
		public List<MonsterEntity> monsterEntities = new List<MonsterEntity> ();

		public int SceneWidth;
		public int SceneHeight;

		public GameScene (GameState state)
		{
			this.game = state;
			this.EntitySpawners = state.EntitySpawners;
			player = new PlayerEntity(new Vector2(0,0));
		}

		virtual public void SpawnEntity(String entityType, Rectangle position, Properties properties) {
			//Ideally use EntitySpawners dictionary, and add entity to appropriate list based on type!
			switch (entityType) {
			case "torch":
				terrainEntities.Add (new TorchEntity (position));
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
			case "node":
				nodeEntities.Add (new NodeEntity (position, properties));
				break;
			case "flytrap":
				monsterEntities.Add (new FlytrapEntity (position));
				break;
			case "wolf":
				monsterEntities.Add (new WolfEntity (position, properties, this));
				break;
			case "fireelemental":
				monsterEntities.Add (new FireElementalEntity (position, properties, this));
				break;
			}

		}

		public void AddNode (String nodeName, String nextNode, Rectangle position) {
			nodeEntities.Add (new NodeEntity (nodeName,nextNode,position));
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
			player.moveOffset(player.Momentum * (float)player.inverseMass);


			foreach (Entity e in projectileEntities) {
				if (e.Active) e.Update (state, gameTime); 
			}

			foreach (FrostEntity e in frostEntities) {
				if (e.Active) e.Update (state, gameTime);
			}

			foreach (PhysicsEntity e in monsterEntities) {
				e.Update (state, gameTime);
				e.position += e.Momentum * (float)e.inverseMass;
			}

            foreach (Entity e in movableEntities) {
				e.Update (state, gameTime);
			}

			terrainEntities = terrainEntities.Where( x => x.Alive()).ToList();
			projectileEntities = projectileEntities.Where( x => x.Alive()).ToList();
			frostEntities = frostEntities.Where (x => x.Alive ()).ToList();
            movableEntities = movableEntities.Where (x => x.Alive ()).ToList();
			monsterEntities = monsterEntities.Where (x => x.Alive ()).ToList();

			DetectCollisions ();
		}

		virtual public void AnimationTick() {
			player.AnimationTick ();
			foreach (Entity e in projectileEntities) {
				e.AnimationTick ();
			}
			foreach (Entity e in monsterEntities) {
				e.AnimationTick ();
			}
		}

		public void DetectCollisions() {
			//Care should be taken to not check the same pair of objects twice.
			//Let's check each projectile with the terrain and the enemies.
			DetectCollisionsBetween (projectileEntities, terrainEntities);
			DetectCollisionsBetween (projectileEntities, movableEntities);
			DetectCollisionsBetween (projectileEntities, monsterEntities);
			DetectCollisionsBetween (frostEntities, terrainEntities);
			DetectCollisionsBetween (frostEntities, movableEntities);
			DetectCollisionsBetween (frostEntities, monsterEntities);
            DetectCollisionsBetween (player, movableEntities);
			DetectCollisionsBetween (player, monsterEntities);
			DetectCollisionsBetween (player, terrainEntities);
            DetectCollisionsBetween (movableEntities, terrainEntities);
			DetectCollisionsBetween (monsterEntities, terrainEntities);
			DetectCollisionsBetween (monsterEntities, movableEntities);

            DetectCollisionsAmong (movableEntities);
			DetectCollisionsAmong (monsterEntities);


		}

		private void DetectCollisionsBetween<A,B>(List<A> list1, List<B> list2) where A:Entity where B:Entity {
			foreach (Entity e1 in list1) {

				Rectangle rect1 = e1.getHitBox ();

				foreach (Entity e2 in list2) {

					if (rect1.Intersects (e2.getHitBox())) {
						if (e1 is PhysicsEntity && e2 is PhysicsEntity) {
							ResolveCollisions ((PhysicsEntity)e1, (PhysicsEntity)e2);
						}
						e1.CollidedWith (e2);
						e2.CollidedWith (e1);

					}
				}

			}
		}
		private void DetectCollisionsBetween<E>(Entity entity, List<E> list) where E:Entity {


			Rectangle rect1 = entity.getHitBox ();
			List<Tuple<Entity,Entity>> collisions = new List<Tuple<Entity, Entity>>();

			foreach (Entity e2 in list) {

				if (rect1.Intersects (e2.getHitBox())) {
					collisions.Add (new Tuple<Entity, Entity> (entity, e2));


				}
			}
			foreach (Tuple<Entity,Entity> coll in collisions) {
				if (coll.Item1 is PhysicsEntity && coll.Item2 is PhysicsEntity)
					ResolveCollisions ((PhysicsEntity)coll.Item1, (PhysicsEntity)coll.Item2);
			}
			foreach (Tuple<Entity,Entity> coll in collisions) {
				coll.Item1.CollidedWith (coll.Item2);
				coll.Item2.CollidedWith (coll.Item1);
			}

		}
                
        private void DetectCollisionsAmong<E>(List<E> list) where E:Entity {
            
             for (int i = 0; i < list.Count; i++) {
				Entity e1 = list [i];
			    Rectangle rect1 = e1.getHitBox ();

				for (int j = i+1; j < list.Count; j++) {
      					Entity e2 = list[j];

				    	if ( rect1.Intersects (e2.getHitBox())) {
							if (e1 is PhysicsEntity && e2 is PhysicsEntity) {
								ResolveCollisions ((PhysicsEntity)e1, (PhysicsEntity)e2);
							}
				       		e1.CollidedWith (e2);
							e2.CollidedWith (e1);

			            }
			 	  }
              }


		}


		//TODO: Implementation relies on AABB
		//HACK: This code is ugly! It has a lot that isn't being used, and it isn't authentic physics.
		public void ResolveCollisions(PhysicsEntity e1, PhysicsEntity e2) {

			if (e1.inverseMass == 0 && e2.inverseMass == 0)
				return; // Two immovable objects. Nothing to be done.
			if (!e1.Collidable || !e2.Collidable) {
				return; //One of the objects doesn't wish for collsion resolution.
			}

			Vector2 diff = new Vector2 (e1.getHitBox ().Center.X - e2.getHitBox ().Center.X, e1.getHitBox ().Center.Y - e2.getHitBox ().Center.Y);
//			Vector2 rel_motion = e1.Momentum - e2.Momentum;
//			if (diff.X * rel_motion.X + diff.Y * rel_motion.Y > 0)
//				return; //Don't mess with objects moving out of each other!

			float x_depth = (e1.getHitBox().Width + e2.getHitBox().Width) / 2.0f - Math.Abs (e1.getHitBox().Center.X - e2.getHitBox().Center.X);
			float y_depth = (e1.getHitBox().Height + e2.getHitBox().Height) / 2.0f - Math.Abs (e1.getHitBox().Center.Y - e2.getHitBox().Center.Y);

			if (e1.inverseMass == 0 || (e1 is PlayerEntity && e2.inverseMass != 0)) {
				MomentumSink (e2, e1.getHitBox (), x_depth, y_depth);
				return;
			} else if (e2.inverseMass == 0) {
				MomentumSink (e1, e2.getHitBox (), x_depth, y_depth);
				return;
			}






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
			DrawList (spriteBatch, monsterEntities, drawHitBoxes);


			if (player.getTexture () != null) {
				spriteBatch.Draw (player.getTexture (), player.position, player.spriteChoice.rect, Color.White);
			}
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

		public bool LineOfSight(Point point1, Point point2) {
			Vector2 p1 = new Vector2 (point1.X, point1.Y);
			Vector2 p2 = new Vector2 (point2.X, point2.Y);

			foreach ( TerrainEntity block in terrainEntities) {
				if (AABBSegmentIntersect (block.getHitBox (), p1, p2)) {
					return false;
				}
			}
			return true;
		}

		bool AABBSegmentIntersect(Rectangle aabb, Vector2 point1, Vector2 point2) {
			Vector2 max = new Vector2 (aabb.Right, aabb.Bottom);
			Vector2 min = new Vector2 (aabb.Left, aabb.Top);
			//Let's check for a plane of sepration.
			Vector2 segmid = (point2 - point1) * 0.5f;

			Vector2 boxmid = (max - min) * 0.5f;

			Vector2 axis = point1 + segmid - (min + max) * 0.5f;

			Vector2 absolutesegmid = segmid;
			absolutesegmid.X = (absolutesegmid.X < 0) ? -absolutesegmid.X : absolutesegmid.X;
			absolutesegmid.Y = (absolutesegmid.Y < 0) ? -absolutesegmid.Y : absolutesegmid.Y;

			if (Math.Abs(axis.X) > boxmid.X + absolutesegmid.X)
				return false;

			if (Math.Abs(axis.Y) > boxmid.Y + absolutesegmid.Y)
				return false;



			return true;
		}


	}
}

