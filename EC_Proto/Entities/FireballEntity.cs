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

		public FireballEntity ()
		{
			hitbox = new Rectangle (5, 5, 22, 22);
		}

		public FireballEntity(Vector2 position, Texture2D texture, Direction direction) 
		{
			this.position = position;
			spriteChoice.texture = texture;
			this.direction = direction;
			movement = speed * Entity.dirVector (direction);
		}

		public FireballEntity(Vector2 position, Texture2D texture, Direction direction, Vector2 momentum) 
		{
			this.position = position;
			spriteChoice.texture = texture;
			this.direction = direction;
			movement = momentum + speed * Entity.dirVector (direction);
		}

		public override void Update (KeyboardState keyboard, GameTime gameTime) {
			if (lifespan > 10) moveOffset( movement); //Stop moving at the end so it doesn't seem like the fireballs are just vanishing.
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

