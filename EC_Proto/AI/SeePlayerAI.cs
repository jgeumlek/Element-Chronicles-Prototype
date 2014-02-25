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

		public void update(PhysicsEntity actor) {
			if (GameState.Scene ().LineOfSight (actor.getHitBox ().Center, GameScene.player.getHitBox ().Center)) {
				Point diff = GameScene.player.getHitBox ().Center - actor.getHitBox ().Center;
				Console.Out.WriteLine (diff);
				Vector2 dir = new Vector2 (diff.X, diff.Y);
				dir.Normalize ();
				Console.Out.WriteLine (dir);
				actor.Impulse (dir);

			}
		}
	}
}

