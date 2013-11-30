using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class FrostEntity : ProjectileEntity
	{
		private const float FROST_SPEED = 1;
		private const int FROST_LIFESPAN = 40;
		private const float FROST_DAMAGE = 3;
		public static Texture2D texture;

		public FrostEntity () {
			speed = FROST_SPEED;
			lifespan = FROST_LIFESPAN;
			fireDamage = FROST_DAMAGE;
			hitbox = new Rectangle (20, 20, 60, 60);
			spriteChoice.texture = texture;
			spriteChoice.rect = texture.Bounds;
		}

		public FrostEntity (Vector2 position, Direction direction, Vector2 momentum) : this()
		{
			this.position = position + 20*Entity.dirVector(direction) - new Vector2(50,50); //Subtract to fix top left corner. TODO: Clean up entity interface to handle this.
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
