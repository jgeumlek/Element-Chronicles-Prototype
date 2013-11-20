using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class FireballEntity : ProjectileEntity 
	{
		private const float FIREBALL_SPEED = 6;
		private const int FIREBALL_LIFESPAN = 40;
		private const float FIREBALL_DAMAGE = 5;
		public static Texture2D texture;

		public FireballEntity () {
			speed = FIREBALL_SPEED;
			lifespan = FIREBALL_LIFESPAN;
			fireDamage = FIREBALL_DAMAGE;
			hitbox = new Rectangle (13, 24, 6, 6);
			spriteChoice.texture = texture;
		}

		public FireballEntity(Vector2 position, Direction direction, Vector2 momentum) : this()
		{
			this.position = position;
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
			if (e is TerrainEntity && e.Collidable) {
				alive = false;
			}
		}
	}
}