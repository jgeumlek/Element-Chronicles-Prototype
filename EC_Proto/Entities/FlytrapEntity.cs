using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
	public class FlytrapEntity : TerrainEntity
	{
		public static Texture2D spritesheet;
		static AnimationManager anim = new AnimationManager();

		public FlytrapEntity () {
			Visible = true;
		}

		public FlytrapEntity (Rectangle rect) {
			position.X = rect.X;
			position.Y = rect.Y;
			spriteChoice.texture = spritesheet;
			animState.AnimationName = "alive";
			spriteChoice.rect = anim.GetRectangle (animState);
			Visible = true;
		}

		static public void InitAnimation() {
			anim.AddAnimation ("alive", 0, 0, 20, 20,1);
			anim.AddAnimation ("dying", 20, 0, 20, 20,3);
			anim.AddAnimation ("dead", 60, 0, 20, 20,1);

			anim.AddStateChange ("alive", "fire", "dying", true);
			anim.AddStateChange ("dying","anim_end","dead",true);
		}


		public override void Update (KeyboardState state, GameTime time) {

		}

		public override void AnimationTick ()
		{
			animState = anim.Tick (animState);
			spriteChoice.rect = anim.GetRectangle (animState);

			if (animState.AnimationName == "dead") {
				alive = false;
			}
		}

		override public void CollidedWith (Entity e) {
			if ( e is FireballEntity) {
				animState = anim.Update (animState, "fire");
				Console.Out.WriteLine ("Fire!");
			}
		}
	}
}

