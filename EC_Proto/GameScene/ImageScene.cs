using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
	public class ImageScene : GameScene
	{
		Texture2D image;
		int count = 0;
		int MaxCount = 1;
		bool TimeLimited;
		String nextScene = "";

		public ImageScene (GameState game, Texture2D image) : base(game)
		{
			this.image = image;
			this.game = game;
			TimeLimited = false;
			SceneWidth = image.Width;
			SceneHeight = image.Height;
		}

		public ImageScene (GameState game, Texture2D image, String nextScene) : this(game,image)
		{
			this.nextScene = nextScene;
		}

		public ImageScene (GameState state, Texture2D image, int maxAnimFrames, String nextScene) : this(state,image,nextScene)
		{
			TimeLimited = true;
			MaxCount = maxAnimFrames;
		}

		public override void Update (GameTime gameTime, Microsoft.Xna.Framework.Input.KeyboardState state, Microsoft.Xna.Framework.Input.KeyboardState prevState)
		{

			if (state.IsKeyDown (Microsoft.Xna.Framework.Input.Keys.Escape) || state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter) || count >= MaxCount) {
				game.LoadScene (nextScene);
			}
		}

		public override void AnimationTick ()
		{
			if (TimeLimited)
				count++;
		}

		public override void Draw (Matrix screenMatrix, SpriteBatch spriteBatch, GraphicsDeviceManager graphics, bool drawHitBoxes)
		{
			spriteBatch.Begin ();
			spriteBatch.Draw (image, new Vector2(0,0), Color.White);
			spriteBatch.End ();
		}
	}
}

