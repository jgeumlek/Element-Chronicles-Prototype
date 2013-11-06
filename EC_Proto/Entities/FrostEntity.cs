using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class FrostEntity : Entity
	{
		int lifespan = 40;
		public static Texture2D texture;

		public FrostEntity () {
			spriteChoice.texture = texture;
			hitbox = new Rectangle (0, 0, 20, 20);
		}

		public FrostEntity(Vector2 position, Direction direction) : this()
		{
			this.position = position + 20*Entity.dirVector(direction);
			spriteChoice.texture = texture;
			this.direction = direction;
		}

		public override void Update (KeyboardState keyboard, GameTime gameTime) {
			lifespan--;

			if (lifespan < 0) {
				alive = false;
			}
		}

		override public void CollidedWith(Entity e) {

		}
	}
}

