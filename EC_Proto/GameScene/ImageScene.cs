using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


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

		public override void Update (GameTime gameTime, KeyboardState state, KeyboardState prevState)
		{

			if ((state.IsKeyDown (Keys.Escape) && prevState.IsKeyUp(Keys.Escape)) || (state.IsKeyDown(Keys.Enter) && prevState.IsKeyUp(Keys.Enter)) || count >= MaxCount) {
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
			spriteBatch.Draw (image, new Rectangle(0,0,game.ViewWidth,game.ViewHeight), Color.White);
			spriteBatch.End ();
		}
	}
}

