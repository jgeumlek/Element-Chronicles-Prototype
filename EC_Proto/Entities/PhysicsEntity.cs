using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	abstract public class PhysicsEntity : Entity
	{
		public double inverseMass = 0;
		protected Vector2 momentum;
		public Vector2 Momentum { get { return momentum; } set { momentum = Momentum;}}


		public PhysicsEntity ()
		{
			Collidable = true; //Need to check if we are using this flag elsewhere.
		}

		public void Impulse(Vector2 impulse) {
			momentum += impulse;
		}
	}
}

