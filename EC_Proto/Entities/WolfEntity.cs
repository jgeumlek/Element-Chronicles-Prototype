using TiledMax;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
	public class WolfEntity : MonsterEntity
	{
		public static Texture2D spritesheet;
		static AnimationManager anim = new AnimationManager();
		AI ai;
		List<Vector2> nodeList = new List<Vector2>();

		public bool seePlayer;
		List<Vector2> positionList = new List<Vector2>();
		private TimeSpan positionPushTimer;
		private Vector2 playerDetectedNode;

		Properties properties;
		GameScene gs;

		private bool backtracking;

		public WolfEntity () {
			Visible = true;

			health = 10;
			contactDamage = 2;
			fireDefense = 0;
			waterDefense = 0;
			earthDefense = 1;
			airDefense = 0;

			seePlayer = false;
			backtracking = false;
		}

		public WolfEntity (Rectangle rect, Properties properties, GameScene gs) {
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
			fireDefense = 0;
			waterDefense = 0;
			earthDefense = 1;
			airDefense = 0;

			this.properties = properties;
			this.gs = gs;

			baseline = hitbox.Bottom;
			seePlayer = false;
			positionPushTimer = new TimeSpan(0,0,0,0,1000);

			backtracking = false;

		}

		static public void InitAnimation() {
			anim.AddAnimation ("alive", 0, 0, 100, 100, 1);
			anim.AddAnimation ("dead", 0, 0, 100, 100, 1);
		}

		public override void Update (KeyboardState state, GameTime time) {
		//---HANDLE AI SWITCHING---
			// If the monster has no AI, assign it a PatrolPathAI with nodeList
			if (ai == null) {
				CreateNodeList (gs, properties);
				ai = new PatrolPathAI (nodeList);
			}

			// If wolf sees player, set seePlayer to true
			if (!backtracking && !seePlayer && GameState.Scene ().LineOfSight (getHitBox ().Center, GameScene.player.getHitBox ().Center)) {
				seePlayer = true;
			}
			// If wolf does not see player, set seePlayer to false
			else if (seePlayer && !(GameState.Scene ().LineOfSight (getHitBox ().Center, GameScene.player.getHitBox ().Center))) {
				seePlayer = false;
			}

			if (seePlayer) {
				if (ai is PatrolPathAI) {
					ai = new SeePlayerAI ();
				}
			} else {
				if (ai is SeePlayerAI) {
					ai = new PatrolPathAI (nodeList);
				}
			}

//			if (seePlayer) {
//
//				if (ai is PatrolPathAI) {
//					ai = new SeePlayerAI ();
//					playerDetectedNode = position;
//					positionList.Add (playerDetectedNode);
//				}
//
//				if (ai is SeePlayerAI) {
//					positionPushTimer -= time.ElapsedGameTime;
//					if (positionPushTimer <= TimeSpan.Zero) {
//						if (Math.Abs (position.X - positionList [positionList.Count - 1].X) > 10 || Math.Abs (position.Y - positionList [positionList.Count - 1].Y) > 10) {
//							positionList.Add (position);
//						}
//						positionPushTimer = new TimeSpan (0, 0, 0, 0, 1000);
//					}
//
//					if (Math.Abs (position.X - playerDetectedNode.X) > 500 || Math.Abs (position.Y - playerDetectedNode.Y) > 500) {
//						seePlayer = false;
//						ai = new PatrolPathAI (positionList);
//						backtracking = true;
//					}
//				}
//			}
//
//			if (!seePlayer) {
//				if (ai is SeePlayerAI) { // If wolf loses sight of player and is still a SeePlayerAI
//					positionList.Reverse (); // Reverse the list so that we backtrack node-by-node
//					ai = new PatrolPathAI (positionList);
//					backtracking = true;
//				} else if (ai is PatrolPathAI) {
//					if (backtracking) {
//						if ((position - playerDetectedNode).LengthSquared () < 2) { // If on top of position where player was detected
//							ai = new PatrolPathAI (nodeList);
//							positionList.Clear ();
//							backtracking = false;
//						}
//					}
//				}
//			}

			ai.update (this, time);
		//---END HANDLE AI SWITCHING---

			momentum *= .8f;
			if (momentum.LengthSquared () > .5f) {
				momentum.Normalize ();
				momentum *= (float)Math.Sqrt(.5f);
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