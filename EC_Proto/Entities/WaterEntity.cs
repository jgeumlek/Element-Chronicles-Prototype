using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
	public class WaterEntity : TerrainEntity
	{
		public bool frozen = false;
		public Texture2D waterTex;
		public Texture2D iceTex;

		public WaterEntity () {
			Visible = true;
		}

		public WaterEntity (Rectangle rect) {
			position.X = rect.X;
			position.Y = rect.Y;
			spriteChoice.texture = waterTex;
			spriteChoice.rect = new Rectangle (0, 0, 20, 20);
			Visible = true;
		}

		public override void Update (KeyboardState state, GameTime time) {

		}

		override public void CollidedWith (Entity e) {
			if (!frozen && e is FrostEntity) {
				frozen = true;
				spriteChoice.texture = iceTex;
			}
		}
	}
}

