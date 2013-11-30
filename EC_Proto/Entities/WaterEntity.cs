using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
	public class WaterEntity : TerrainEntity {
		public bool frozen = false;
		static public Texture2D waterTex;
		static public Texture2D iceTex;

		public WaterEntity () {
			Visible = true;
		}

		public WaterEntity (Rectangle rect) {
			position.X = rect.X;
			position.Y = rect.Y;
			spriteChoice.texture = waterTex;
			spriteChoice.rect = waterTex.Bounds;
			hitbox = waterTex.Bounds;
			Visible = true;
		}

		public override void Update (KeyboardState state, GameTime time) {

		}

		override public void CollidedWith (Entity e) {
			if (!frozen && e is FrostEntity) {
				frozen = true;
				spriteChoice.texture = iceTex;
				Collidable = false;
			}
		}
	}
}