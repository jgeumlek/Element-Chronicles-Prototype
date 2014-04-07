using System;
using Microsoft.Xna.Framework;
namespace EC_Proto
{
	public class SeePlayerAI : AI
	{
		float speed = 5;
		public SeePlayerAI ()
		{
		}

		public SeePlayerAI (float speed)
		{
			this.speed = speed;
		}

		public void update(PhysicsEntity actor, GameTime time) {
			if (GameState.Scene ().LineOfSight (actor.getHitBox ().Center, GameScene.player.getHitBox ().Center)) {
				Console.Out.WriteLine ("I see you.");
				Vector2 dir = new Vector2 (GameScene.player.getHitBox().Center.X - actor.getHitBox().Center.X, GameScene.player.getHitBox().Center.Y - actor.getHitBox().Center.Y);
				dir.Normalize ();
				actor.Impulse (dir);

			}
		}
	}
}

