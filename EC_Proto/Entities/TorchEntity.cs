using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
	public class TorchEntity : TerrainEntity
	{
		public bool lit = false;
		public static Texture2D torchUnlit;
		public static Texture2D torchLit;

		public TorchEntity () {
			Visible = true;
		}

		public TorchEntity (Rectangle rect) {
			position.X = rect.X;
			position.Y = rect.Y;
			spriteChoice.texture = torchUnlit;
			Visible = true;
		}

		public override void Update (KeyboardState state, GameTime time) {
			
		}

		override public void CollidedWith (Entity e) {
			if (!lit && e is FireballEntity) {
				lit = true;
				spriteChoice.texture = torchLit;
			}
		}
	}
}

