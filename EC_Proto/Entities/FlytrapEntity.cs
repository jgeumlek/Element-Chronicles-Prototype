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
			inverseMass = 5;
		}

		static public void InitAnimation() {
			anim.AddAnimation ("alive", 0, 0, 100, 100,1);
			anim.AddAnimation ("dying1", 100, 0, 100, 100,2);
			anim.AddAnimation ("dying2", 200, 0, 100, 100,2);
			anim.AddAnimation ("dying3", 100, 0, 100, 100,2);
			anim.AddAnimation ("dead", 300, 0, 100, 100,1);

			anim.AddStateChange ("alive", "fire", "dying1", true);
			anim.AddStateChange ("dying1","anim_end","dying2",true);
			anim.AddStateChange ("dying2","anim_end","dying3",true);
			anim.AddStateChange ("dying3","anim_end","dead",true);


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
				spriteChoice.rect = anim.GetRectangle (animState);
			}

			if (e is PlayerEntity) {
				Point diff = e.getHitBox ().Center - getHitBox().Center;
				Vector2 dirvec = new Vector2 (diff.X, diff.Y);
				dirvec = Entity.align (dirvec); 
				((PlayerEntity)e).Impulse (dirvec * 50);
				((PlayerEntity)e).KnockBack ();


			}
		}
	}
}

