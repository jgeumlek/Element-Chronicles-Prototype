using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class PlayerRocksEntity : Entity
	{
		static AnimationManager anim = new AnimationManager();
		public static Texture2D spritesheet;

		public PlayerRocksEntity () {
		}

		public PlayerRocksEntity (Vector2 position) {
			this.position = position - new Vector2(10,0);
			alive = true;
			Visible = true;
			Active = true;
			Collidable = false;
			animState.AnimationName = "alive";
			spriteChoice.texture = spritesheet;
			spriteChoice.rect = anim.GetRectangle (animState);
			// No hitbox
			// No hurtbox
		}

		static public void InitAnimation() {
			anim.AddAnimation ("alive", 0, 0, 100, 150, 1);
			anim.AddAnimation ("alive2", 0, 0, 100, 150, 1);
			anim.AddAnimation ("alive3", 100, 0, 100, 150, 1);
			anim.AddAnimation ("alive4", 100, 0, 100, 150, 1);
			anim.AddAnimation ("alive5", 150, 0, 100, 150, 1);
			anim.AddAnimation ("alive6", 150, 0, 100, 150, 1);

			anim.AddStateChange ("alive", "anim_end", "alive2", true);
			anim.AddStateChange ("alive2", "anim_end", "alive3", true);
			anim.AddStateChange ("alive3", "anim_end", "alive4", true);
			anim.AddStateChange ("alive4", "anim_end", "alive5", true);
			anim.AddStateChange ("alive5", "anim_end", "alive6", true);
			anim.AddStateChange ("alive6", "anim_end", "alive", true);
		}

		public override void Update (KeyboardState keyboard, GameTime gameTime) {
			position = GameScene.player.position - new Vector2(10,0);
		}

		public override void AnimationTick ()
		{
			animState = anim.Tick (animState);
			spriteChoice.rect = anim.GetRectangle (animState);
		}
	}
}

