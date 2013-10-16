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
		public Texture2D texture;
		public Direction direction;
		protected bool alive = true;
		public bool Visible { get; set; }
		public bool Active { get; set; } //Should we bother updating?
		public bool Collidable { get; set; } //Should we bother checking collisions?
		public Rectangle hitbox { get; set;}//Collision detection.
		public Rectangle hurtbox { get; set; }

		public Entity ()
		{
			Visible = true;
			Active = true;
			Collidable = true;

		}

		public Entity(Vector2 position, Texture2D texture) : this()
		{
			this.position = position;
			this.texture = texture;

		}

		//Old habits of daa encapsulation. Should probably make better use of C# get/set.
		public Vector2 getPosition() {
			return position;
		}

		public Texture2D getTexture() {
			return texture;
		}

		public void setPostion(Vector2 position) {
			this.position = position;
		}

		public void setDirection(Direction direction) {
			this.direction = direction;
		}

		public Direction getDirection() {
			return direction;
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

		public bool Alive() {
			return alive;
		}



		public Rectangle getHitBox() {
			//This could probably be optimized.
			return new Rectangle(hitbox.X + (int)position.X, hitbox.Y + (int)position.Y, hitbox.Width, hitbox.Height);
		}


		virtual public void CollidedWith(Entity e) {
		}

		abstract public void update (KeyboardState state, GameTime gametime);
	}
}

