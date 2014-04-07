using System;
using Microsoft.Xna.Framework;

namespace EC_Proto
{
	public class RandomWalkAI : AI
	{
		static Random rng = new Random();
		private Vector2 impulseDir;

		TimeSpan timer = new TimeSpan (0, 0, 0, 0, 500);

		public RandomWalkAI ()
		{
		}

		public void update (PhysicsEntity actor, GameTime time) {
			if (timer >= TimeSpan.Zero) {
				timer -= time.ElapsedGameTime;
			} else {
				timer = new TimeSpan (0, 0, 0, 0, 500);
				impulseDir = new Vector2 ((float)rng.NextDouble () - 0.5f, (float)rng.NextDouble () - 0.5f);
				impulseDir.Normalize ();
			}
			actor.Impulse (impulseDir);
		}
	}
}
