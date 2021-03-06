using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
	public class TorchEntity : TerrainEntity
	{
		public bool lit = false;
		public static Texture2D torchOff;
		public static Texture2D torchOn;

		public TorchEntity () {
			Visible = true;
		}

		public TorchEntity (Rectangle rect) {
			position.X = rect.X;
			position.Y = rect.Y;
			spriteChoice.texture = torchOff;
			spriteChoice.rect = torchOff.Bounds;
			hitbox = torchOff.Bounds;
			Visible = true;

			baseline = hitbox.Bottom;
		}

		public override void Update (KeyboardState state, GameTime time) {
			
		}

		public override bool Activated() {
			return lit;
		}

		override public void CollidedWith (Entity e) {
			if (!lit && e is FireballEntity) {
				lit = true;
				spriteChoice.texture = torchOn;
			}
			if (lit && e is FrostEntity) {
				lit = false;
				spriteChoice.texture = torchOff;
			}
		}
	}
}

