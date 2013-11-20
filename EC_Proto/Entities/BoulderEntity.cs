using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class BoulderEntity : TerrainEntity
	{
		static public Texture2D texture;
		public BoulderEntity ()
		{
			Visible = true;
		}

		public BoulderEntity (Rectangle rect) {
			position.X = rect.X;
			position.Y = rect.Y;
			spriteChoice.texture = texture;
			spriteChoice.rect = new Rectangle (0, 0, 20, 20);
			Visible = true;
		}

		public override void Update (KeyboardState state, GameTime time) {

		}

		override public void CollidedWith (Entity e) {
			if (e is PlayerEntity) {
				position += 20 * Entity.dirVector (((PlayerEntity)e).direction); // ((PlayerEntity)e).getCurrentSpeed ()
			}
		}
	}
}