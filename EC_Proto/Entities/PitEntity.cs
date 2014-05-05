using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class PitEntity : TerrainEntity
	{
		public static Texture2D texture;

		public PitEntity (Rectangle rect) {
			this.position = new Vector2 (rect.X, rect.Y);
			alive = true;
			Visible = false;
			Active = true;
			Collidable = true;
			animState.AnimationName = "alive";
			spriteChoice.texture = texture;
			spriteChoice.rect = texture.Bounds;
			hitbox = texture.Bounds;
			// No hurtbox
		}

		public override void Update (KeyboardState state, GameTime time) {
			if (SpellManager.activeSpell == "windwalk") {
				Collidable = false;
			}
			if (!Collidable && SpellManager.activeSpell != "windwalk") {
				Collidable = true;
			}
		}
	}
}

