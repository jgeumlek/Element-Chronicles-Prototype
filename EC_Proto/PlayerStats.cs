using System;
using Microsoft.Xna.Framework;

namespace EC_Proto
{
	public class PlayerStats
	{
		public static float level = 1;
		public static float curHp = 100;
		public static float maxHp = 100;
		public static float curMana = 20;
		public static float maxMana = 20;
		public static float curExp = 0;
		public static float maxExp = 100;

		public static int hpRegenTime = 1000; // in milliseconds
		public static int manaRegenTime = 2000;

		private static TimeSpan hpTimer = new TimeSpan (0, 0, 0, 0, hpRegenTime);
		private static TimeSpan manaTimer = new TimeSpan (0, 0, 0, 0, manaRegenTime);

		public static void Update (GameTime gameTime) {
			if (curHp < 0) {
				curHp = 0;
			}
			if (curMana < 0) {
				curMana = 0;
			}
			if (hpTimer > TimeSpan.Zero) {
				hpTimer -= gameTime.ElapsedGameTime;
			} else {
				if (curHp < maxHp)
					curHp++;
				hpTimer = new TimeSpan (0, 0, 0, 0, hpRegenTime);
			}

			if (manaTimer > TimeSpan.Zero) {
				manaTimer -= gameTime.ElapsedGameTime;
			} else {
				if (curMana < maxMana)
					curMana++;
				manaTimer = new TimeSpan (0, 0, 0, 0, manaRegenTime);
			}
		}

		public static void Respawn () {
			curHp = maxHp;
			curMana = maxMana;
			if (GameScene.player.overlayActive) {
				GameScene.player.DestroyOverlay ();
				SpellManager.activeSpell = "";
			}
		}
	}
}