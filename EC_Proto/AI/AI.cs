using System;
using Microsoft.Xna.Framework;

namespace EC_Proto
{
	public interface AI
	{
		void update (PhysicsEntity actor, GameTime time);
	}
}

