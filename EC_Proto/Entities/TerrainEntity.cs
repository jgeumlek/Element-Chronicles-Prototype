using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class TerrainEntity : Entity
	{
		public TerrainEntity ()
		{
			hitbox = new Rectangle (0, 0, 20, 20);
		}
		public TerrainEntity(Rectangle rect) {
			position.X = rect.X;
			position.Y = rect.Y;
			hitbox = new Rectangle (0, 0, rect.Width, rect.Height);
		}

		override public void Update(KeyboardState state, GameTime time) {
		}
	}
}

