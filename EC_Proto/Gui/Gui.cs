using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
	public class Gui
	{
		static public String sceneName;

		static public Texture2D hpBarBackground; // Black bar behind the health bar, which does not change in length
		static public Texture2D hpBar;
		static public Texture2D manaBarBackground;
		static public Texture2D manaBar;
		static public Texture2D expBarBackground;
		static public Texture2D expBar;

		private int barLength = 200;

		public Gui ()
		{

		}

		public void Update(GameTime gameTime) {
			if (PlayerStats.curExp >= PlayerStats.maxExp) {
				PlayerStats.level++;
				PlayerStats.maxExp += PlayerStats.maxExp / 10;
				PlayerStats.curExp = 0;
			}
		}

		public void Draw(Matrix screenMatrix, SpriteBatch spriteBatch, GraphicsDeviceManager graphics) {
			spriteBatch.Begin ();

			spriteBatch.Draw (hpBarBackground, new Rectangle (10, 10, barLength, 10), Color.White);
			spriteBatch.Draw (hpBar, new Rectangle (10, 10, (int)(PlayerStats.curHp / PlayerStats.maxHp * barLength), 10), Color.White);
			spriteBatch.Draw (manaBarBackground, new Rectangle (10, 30, barLength, 10), Color.White);
			spriteBatch.Draw (manaBar, new Rectangle (10, 30, (int)(PlayerStats.curMana/PlayerStats.maxMana * barLength), 10), Color.White);
			spriteBatch.Draw (expBarBackground, new Rectangle (0, 470, 800, 10), Color.White);
			spriteBatch.Draw (expBar, new Rectangle (0, 470, (int)(PlayerStats.curExp/PlayerStats.maxExp * 800), 10), Color.White);

			spriteBatch.End();
		}
	}
}

