using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EC_Proto;

namespace EC_Proto
{
	public class FireballEntity : Entity 
	{
		private float speed = 6;
		private Vector2 movement = new Vector2(0,0);
		int lifespan = 40;
		public static Texture2D texture;

		public FireballEntity ()
		{
			hitbox = new Rectangle (13, 24, 6, 6);
			spriteChoice.texture = texture;
		}

		public FireballEntity(Vector2 position, Direction direction, Vector2 momentum) : this()
		{
			this.position = position;
			spriteChoice.texture = texture;
			this.direction = direction;
			movement = momentum + speed * Entity.dirVector (direction);
		}

		public override void Update (KeyboardState keyboard, GameTime gameTime) {
			moveOffset(movement);
			lifespan--;

			if (lifespan < 0) {
				alive = false;
			}
		}

		override public void CollidedWith(Entity e) {
			if (e is TerrainEntity) {
				alive = false;
			}
		}
	}
}

