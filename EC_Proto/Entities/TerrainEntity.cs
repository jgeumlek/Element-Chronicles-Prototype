using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class TerrainEntity : Entity
	{
		public TerrainEntity ()
		{
			hitbox = new Rectangle (0, 0, 32, 32);
		}

		override public void update(KeyboardState state, GameTime time) {
		}
	}
}

