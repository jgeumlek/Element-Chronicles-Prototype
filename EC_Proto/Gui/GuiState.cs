using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
	public class GuiState
	{

		public Texture2D hpBarBackground; // Black bar behind the health bar, which does not change in length
		public Texture2D hpBar;
		public Texture2D manaBarBackground;
		public Texture2D manaBar;
		public Texture2D expBarBackground;
		public Texture2D expBar;

		public void LoadContent() {



		}

		public GuiState ()
		{

		}

		public void Draw(SpriteBatch spriteBatch, GraphicsDeviceManager graphics) {

		}
	}
}

