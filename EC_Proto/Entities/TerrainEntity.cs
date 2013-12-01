using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class TerrainEntity : Entity
	{
		public TerrainEntity ()
		{
			hitbox = new Rectangle (0, 0, 100, 100);
			Visible = false;
			Collidable = true;
		}
		public TerrainEntity(Rectangle rect) {
			position.X = rect.X;
			position.Y = rect.Y;
			hitbox = new Rectangle (0, 0, rect.Width, rect.Height);
			Visible = false;
			Collidable = true;
		}

		override public void Update(KeyboardState state, GameTime time) {
		}
	}
}