using System;
using Microsoft.Xna.Framework;

namespace EC_Proto
{
	public class RandomWalkAI : AI
	{
		static Random rng = new Random();
		public RandomWalkAI ()
		{


		}

		public void update (PhysicsEntity actor) {
			actor.Impulse (new Vector2 ((float)rng.NextDouble () - 0.5f, (float)rng.NextDouble () - 0.5f));
		}
	}
}

