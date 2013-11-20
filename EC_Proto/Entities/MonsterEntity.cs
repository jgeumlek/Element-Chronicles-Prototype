using System;

namespace EC_Proto
{
	abstract public class MonsterEntity : Entity
	{
		protected float health;
		protected float contactDamage;
		protected float fireDefense;
		protected float waterDefense;
		protected float earthDefense;
		protected float airDefense;

		public MonsterEntity ()
		{
		}
	}
}

