using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TiledMax;

namespace EC_Proto
{
	public class PressurePlateEntity : Entity
	{
		public static Texture2D plateTex;
		static AnimationManager anim = new AnimationManager();
		List<String> requireOn = new List<String>();
		List<String> requireOff = new List<String>();

		bool hitBoulder = false;
		bool pressed = false;

		public PressurePlateEntity (Rectangle rect)
		{
			position.X = rect.X;
			position.Y = rect.Y;
			spriteChoice.texture = plateTex;
			animState.AnimationName = "unpressed";
			Visible = true;
			Collidable = false;

		


			hitbox = new Rectangle(0,0,100,100);

		}

		public override bool Activated() {
			return pressed;
		}

		public override void Update (KeyboardState state, GameTime time) {
			if (hitBoulder) {
				pressed = true;
			} else {
				pressed = false;
			}

			hitBoulder = false;
		}

		public override void AnimationTick ()
		{
			if (animState.AnimationName.Equals("unpressed") && pressed) {
				animState = anim.Update (animState, "press");
			} else if (animState.AnimationName == "pressed" && !pressed) {
				animState = anim.Update (animState, "unpress");
			}
			animState = anim.Tick (animState);
			spriteChoice.rect = anim.GetRectangle (animState);





		}

		static public void InitAnimation() {
			anim.AddAnimation ("unpressed", 0, 0, 100, 100,1);
			anim.AddAnimation ("pressing", 0, 0, 100, 100,1);
			anim.AddAnimation ("pressed", 100, 0, 100, 100,1);
			anim.AddAnimation ("unpressing", 100, 0, 100, 100,1);

			anim.AddStateChange ("unpressed", "press", "pressing", true);
			anim.AddStateChange ("pressed", "unpress", "unpressing", true);
			anim.AddStateChange ("pressing", "anim_end", "pressed", true);
			anim.AddStateChange ("unpressing", "anim_end", "unpressed", true);
		}

		override public void CollidedWith (Entity e) {
			if (e is BoulderEntity) {
				hitBoulder = true;
			}
		}
	}
}

