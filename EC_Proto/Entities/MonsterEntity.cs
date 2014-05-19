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
		protected bool dying = false;

		public MonsterEntity ()
		{
			health = 10;
			contactDamage = 0;
			fireDefense = 0;
			waterDefense = 0;
			earthDefense = 0;
			airDefense = 0;
		}

		public void Hit(ProjectileEntity projectile) {
			//TODO: Rethink these damage calculations.
			Console.Out.WriteLine (health);
			this.health -= (int)projectile.fireDamage/(fireDefense+1);
			this.health -= (int)projectile.waterDamage/(waterDefense+1);
			this.health -= (int)projectile.earthDamage/(earthDefense+1);
			this.health -= (int)projectile.airDamage/(airDefense+1);
		}

		public override bool Activated ()
		{
			return dying;
		}
	}
}
