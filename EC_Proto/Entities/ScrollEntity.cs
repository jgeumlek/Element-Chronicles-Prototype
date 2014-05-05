using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledMax;

namespace EC_Proto
{
	public class ScrollEntity : Entity
	{
		public static Texture2D texture;

		public Properties properties;
		private GameScene gameScene;

		public ScrollEntity (Rectangle rect, Properties properties, GameScene gameScene)
		{
			position.X = rect.X;
			position.Y = rect.Y;

			spriteChoice.texture = texture;
			spriteChoice.rect = texture.Bounds;
			hitbox = texture.Bounds;

			this.properties = properties;
			this.gameScene = gameScene;

			Visible = true;
		}

		public override void Update (KeyboardState keyboard, GameTime gameTime) {

		}
	}
}

