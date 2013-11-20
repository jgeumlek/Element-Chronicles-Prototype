using System;
using Microsoft.Xna.Framework;

namespace EC_Proto
{
	abstract public class ProjectileEntity : Entity
	{
		protected float speed;
		protected Vector2 movement = new Vector2 (0, 0);
		protected int lifespan;
		public float fireDamage;
		public float waterDamage;
		public float airDamage;
		public float earthDamage;
		public enum faction {
			player,
			enemy
		}
		public ProjectileEntity () {
		}
	}
}

