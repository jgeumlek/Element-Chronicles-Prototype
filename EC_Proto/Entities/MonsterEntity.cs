using System;

namespace EC_Proto
{
	abstract public class MonsterEntity : TerrainEntity
	{
		protected int health;
		protected int contactDamage;
		protected int fireDefense;
		protected int waterDefense;
		protected int earthDefense;
		protected int airDefense;

		public MonsterEntity ()
		{
		}
	}
}

