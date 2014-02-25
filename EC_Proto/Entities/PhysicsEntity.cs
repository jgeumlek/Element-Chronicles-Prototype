using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	abstract public class PhysicsEntity : Entity
	{
		public double inverseMass = 0;
		public Vector2 momentum;
		public Vector2 Momentum { get { return momentum; } set { }}


		public PhysicsEntity ()
		{
			Collidable = true; //Need to check if we are using this flag elsewhere.
		}

		public void Impulse(Vector2 impulse) {
			momentum += impulse;
		}
	}
}

