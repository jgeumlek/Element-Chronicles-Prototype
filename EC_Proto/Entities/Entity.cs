using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EC_Proto;

//Base class for all entities, such as players, enemies, projectiles, etc.
namespace EC_Proto
{
	abstract public class Entity
	{
		public Vector2 position;
		public Direction direction { get; set; }
		protected bool alive = true;
		public bool Visible { get; set; }
		public bool Active { get; set; } //Should we bother updating?
		public bool Collidable { get; set; } //Should we bother checking collisions?
		public Rectangle hitbox { get; set;}//Collision detection.
		public Rectangle hurtbox { get; set; }
		public SpriteChoice spriteChoice { get; set; }
		protected AnimationState animState = new AnimationState();

		public Entity ()
		{
			Visible = true;
			Active = true;
			Collidable = true;
			spriteChoice = new SpriteChoice();
		}

		public Entity(Vector2 position, Texture2D texture) : this()
		{
			this.position = position;

		}

		public Texture2D getTexture () {
			return spriteChoice.texture;
		}

		public void setTexture (Texture2D texture) {
			spriteChoice.texture = texture;
		}

		public void moveOffset(Vector2 offset) {
			position.X += offset.X;
			position.Y += offset.Y;
		}

		//Convert enum values to actual 2D directions.
		public static Vector2 dirVector(Direction direction) {

			if (direction == Direction.North) {
				return new Vector2 (0, -1);
			} else if (direction == Direction.East) {
				return new Vector2 (1, 0);
			} else if (direction == Direction.South) {
				return new Vector2 (0, 1);
			} else { //West
				return new Vector2 (-1, 0);
			}
		}

		//Convert enum values to names
		public static String dirName(Direction direction) {
			if (direction == Direction.North) {
				return "north";
			} else if (direction == Direction.East) {
				return "east";
			} else if (direction == Direction.South) {
				return "south";
			} else { //West
				return "west";
			}
		}

		//Takes a vector, and returns the closest axis-aligned vector.
		public static Vector2 align(Vector2 vector) {
			float x = Math.Abs (vector.X);
			float y = Math.Abs (vector.Y);

			if (x > y) {
				if (vector.X < 0)
					return new Vector2 (-1, 0);
				return new Vector2 (1, 0);
			} else {
				if (vector.Y < 0)
					return new Vector2 (0, -1);
				return new Vector2 (0, 1);
			}
			//Note the arbitrary tie breakers.
		}


		public bool Alive() {
			return alive;
		}
		
		public Rectangle getHitBox() {
			//This could probably be optimized.
			return new Rectangle(hitbox.X + (int)position.X, hitbox.Y + (int)position.Y, hitbox.Width, hitbox.Height);
		}

		//Handle collision how you want.
		virtual public void CollidedWith(Entity e) {
		}
		//A frame's worth of time has passed. Do nothing by defualt, override if you have an animation.
		virtual public void AnimationTick() {
		}
		abstract public void Update (KeyboardState state, GameTime gametime);



	}
}

