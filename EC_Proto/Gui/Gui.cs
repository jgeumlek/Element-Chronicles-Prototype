using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
	public class Gui
	{
		public static String sceneName;

		// HUD elements
		public static Texture2D hpBarBackground; // Black bar behind the health bar, which does not change in length
		public static Texture2D hpBar;
		public static Texture2D manaBarBackground;
		public static Texture2D manaBar;
		public static Texture2D expBarBackground;
		public static Texture2D expBar;

		private static int barLength = 200;

		// Dialog elements
		public static Texture2D textBox;
		public static GuiText dialogText = new GuiText ("You have obtained the spell: Fireball\nPress H to use.", new Vector2 (160, 150));
		public static GuiText dialogQuitText = new GuiText ("Press E to cancel dialog.", new Vector2 (365, 310), Color.Gray);

		public static bool inDialog = false;
		private static Rectangle textBoxPosition = new Rectangle (150, 140, 500, 200);

		public static void Update(GameTime gameTime, KeyboardState state, KeyboardState prevState) {
			if (state.IsKeyDown (Keys.E) && prevState.IsKeyUp (Keys.E)) {
				inDialog = !inDialog;
			}

			if (PlayerStats.curExp >= PlayerStats.maxExp) {
				PlayerStats.level++;
				PlayerStats.maxExp += PlayerStats.maxExp / 10;
				PlayerStats.curExp = 0;
			}
				
		}

		public static void Draw(Matrix screenMatrix, SpriteBatch spriteBatch, GraphicsDeviceManager graphics) {
			spriteBatch.Begin ();

			spriteBatch.Draw (hpBarBackground, new Rectangle (10, 10, barLength, 10), Color.White);
			spriteBatch.Draw (hpBar, new Rectangle (10, 10, (int)(PlayerStats.curHp / PlayerStats.maxHp * barLength), 10), Color.White);
			spriteBatch.Draw (manaBarBackground, new Rectangle (10, 30, barLength, 10), Color.White);
			spriteBatch.Draw (manaBar, new Rectangle (10, 30, (int)(PlayerStats.curMana/PlayerStats.maxMana * barLength), 10), Color.White);
			spriteBatch.Draw (expBarBackground, new Rectangle (0, 470, 800, 10), Color.White);
			spriteBatch.Draw (expBar, new Rectangle (0, 470, (int)(PlayerStats.curExp/PlayerStats.maxExp * 800), 10), Color.White);

			if (inDialog) {
				spriteBatch.Draw (textBox, textBoxPosition, Color.White);
				dialogText.Draw (spriteBatch);
				dialogQuitText.Draw (spriteBatch);
			}

			spriteBatch.End();
		}

		public static void UpdateDialog (string newText) {
			dialogText.text = newText;
			inDialog = true;
		}
	}
}

