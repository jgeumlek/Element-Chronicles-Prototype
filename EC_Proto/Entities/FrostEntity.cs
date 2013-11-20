using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EC_Proto;

namespace EC_Proto
{
	public class FrostEntity : Entity
	{
		private float speed = 1;
		private Vector2 movement = new Vector2(0,0);
		int lifespan = 40;
		public static Texture2D texture;

		public FrostEntity () {
			hitbox = new Rectangle (0, 0, 20, 20);
			spriteChoice.texture = texture;
		}

		public FrostEntity (Vector2 position, Direction direction, Vector2 momentum) : this()
		{
			this.position = position + 20*Entity.dirVector(direction);
			spriteChoice.texture = texture;
			this.direction = direction;
			movement = momentum + speed * Entity.dirVector (direction);
		}

		public override void Update (KeyboardState keyboard, GameTime gameTime) {
			moveOffset(movement);
			lifespan--;
			//position += 10* new Vector2(1,0);
			if (lifespan < 0) {
				alive = false;
			}
		}

		override public void CollidedWith(Entity e) {
			if (e is TerrainEntity && e.Collidable) {
				alive = false;
			}
		}
	}
}