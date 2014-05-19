using TiledMax;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
	public class FireElementalEntity : MonsterEntity
	{
		public static Texture2D spritesheet;
		static AnimationManager anim = new AnimationManager();
		AI ai;
		List<Vector2> nodeList = new List<Vector2>();

		Properties properties;
		GameScene gs;

		public FireElementalEntity ()
		{
			Visible = true;

			health = 15;
			contactDamage = 3;
			fireDefense = 5;
			waterDefense = -1;
			earthDefense = 0;
			airDefense = 1;
		}

		public FireElementalEntity (Rectangle rect, Properties properties, GameScene gs) {
			position.X = rect.X;
			position.Y = rect.Y;

			spriteChoice.texture = spritesheet;
			animState.AnimationName = "alive";
			hitbox = new Rectangle (0, 0, rect.Width, rect.Height);
			spriteChoice.rect = anim.GetRectangle (animState);
			Visible = true;
			inverseMass = 5;

			health = 10;
			contactDamage = 2;
			fireDefense = 10;
			waterDefense = 0;
			earthDefense = 1;
			airDefense = 0;

			this.properties = properties;
			this.gs = gs;

			baseline = hitbox.Bottom;
		}

		static public void InitAnimation() {
			anim.AddAnimation ("alive", 0, 0, 100, 100, 1);
			anim.AddAnimation ("dead", 0, 0, 100, 100, 1);
		}

		public override void Update (KeyboardState state, GameTime time) {
			if (ai == null) {
				CreateNodeList (gs, properties);
				ai = new PatrolPathAI (nodeList);
			}

			ai.update (this, time);
			momentum *= .8f;
			if (momentum.LengthSquared () > 1) {
				momentum.Normalize ();
				momentum *= 1;
			}

			if (health <= 0 && !dying) {
				animState.AnimationName = "dead";
				spriteChoice.rect = anim.GetRectangle (animState);
				dying = true;
			}
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

			if (e is PlayerEntity) {
				Point TrapCenter = this.getHitBox ().Center;
				Point PlayerCenter = e.getHitBox ().Center;
				//Point diff = e.getHitBox ().Center - getHitBox().Center;

				Vector2 dirvec = new Vector2 (PlayerCenter.X - TrapCenter.X, PlayerCenter.Y - TrapCenter.Y);
				dirvec = Entity.align (dirvec); 

				((PlayerEntity)e).Hit (contactDamage);

				momentum *= -1;
				this.Impulse (-dirvec * 5);
				((PlayerEntity)e).Impulse (dirvec * 5);
			}
		}

		private void CreateNodeList(GameScene gs, Properties properties) {
			string nextNodeName = (string)properties["next"];
			string firstNodeName = nextNodeName;

			do {
				foreach (NodeEntity e in gs.nodeEntities) {
					if (e.nodeName == nextNodeName) {
						nodeList.Add (e.position);
						nextNodeName = e.nextNodeName;
						if (nextNodeName == firstNodeName) break;
					}
				}
			} while (nextNodeName != firstNodeName);
		}
	}
}