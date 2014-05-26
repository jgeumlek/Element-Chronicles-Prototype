using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
	public class GuiText
	{
		public static SpriteFont dialogFont;

		public string text { get; set; }
		public Vector2 position { get; set; }
		public Color color { get; set; }

		public GuiText ()
		{
			this.text = "";
			this.position = new Vector2 (0, 0);
			this.color = Color.White;
		}

		public GuiText (string text)
		{
			this.text = text;
			this.position = new Vector2 (0, 0);
			this.color = Color.White;
		}

		public GuiText (Vector2 position)
		{
			this.text = "";
			this.position = position;
			this.color = Color.White;
		}

		public GuiText (Color color)
		{
			this.text = "";
			this.position = new Vector2 (0, 0);
			this.color = color;
		}

		public GuiText (string text, Vector2 position)
		{
			this.text = text;
			this.position = position;
			this.color = Color.White;
		}

		public GuiText (string text, Color color)
		{
			this.text = text;
			this.position = new Vector2 (0, 0);
			this.color = color;
		}

		public GuiText (string text, Vector2 position, Color color)
		{
			this.text = text;
			this.position = position;
			this.color = color;
		}

		public void Update(GameTime gameTime) {

		}

		public void Draw(SpriteBatch spriteBatch) {
			spriteBatch.DrawString(dialogFont, text, position, color);
		}
	}
}

