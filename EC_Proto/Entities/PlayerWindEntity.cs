using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class PlayerWindEntity : Entity { // The tornado that surrounds the player during the wind spell 

		static AnimationManager anim = new AnimationManager();
		public static Texture2D spritesheet;

		public PlayerWindEntity () {
		}

		public PlayerWindEntity (Vector2 position) {
			this.position = position - new Vector2(15,-15);
			alive = true;
			Visible = true;
			Active = true;
			Collidable = false;
			// No hitbox
			// No hurtbox
			animState.AnimationName = "alive";
			spriteChoice.texture = spritesheet;
			spriteChoice.rect = anim.GetRectangle (animState);
			baseline = 150;
		}

		static public void InitAnimation() {
			anim.AddAnimation ("alive", 0, 0, 100, 150, 4);

			anim.AddStateChange ("alive", "anim_end", "alive", true);
		}

		public override void Update (KeyboardState keyboard, GameTime gameTime) {
			position = GameScene.player.position - new Vector2(15,-15);
		}

		public override void AnimationTick ()
		{
			animState = anim.Tick (animState);
			spriteChoice.rect = anim.GetRectangle (animState);
		}
	}
}

