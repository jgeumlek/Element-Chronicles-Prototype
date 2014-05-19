using TiledMax;

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace EC_Proto
{
	public class TmxScene : GameScene
	{
		private GameMap map; //Game state, scene handling, level changes still need work.
		RenderTarget2D mapTex = null;

		public TmxScene (GameState state) : base(state)
		{
		}

		public void LoadMap(String mapfile, String locationTarget, ContentManager content) {
			map = new GameMap (mapfile);
			map.Load (content,this);
			mapTex = null;

			SceneWidth = map.MapWidth;
			SceneHeight = map.MapHeight;

			foreach (String location in map.Locations.Keys) {
			}
			if (map.Locations.ContainsKey (locationTarget)) {
				player.MoveToRect (map.Locations [locationTarget]);
			}
		}

		override public void SpawnEntity(String entityType, Rectangle position, Properties properties) {
			//Ideally use EntitySpawners dictionary, and add entity to appropriate list based on type!
			if (entityType == "player") {
				map.Locations.Add ("default", position);
			}
			base.SpawnEntity (entityType, position, properties);

		}

		override public void Draw(Matrix screenMatrix, SpriteBatch spriteBatch, GraphicsDeviceManager graphics, bool drawHitBoxes) {
			//TODO: Screen transformations, like camera translation.
			if (mapTex == null) {
				mapTex = new RenderTarget2D (graphics.GraphicsDevice, map.MapWidth, map.MapHeight);

				graphics.GraphicsDevice.SetRenderTarget (mapTex);


				spriteBatch.Begin (SpriteSortMode.Deferred, null, null, null, null, null);
				foreach (TiledMax.Layer layer in map.Layers) {

					int width = layer.Width;
					int height = layer.Height;
					for (int y = 0; y < layer.Height; y++) {
						for (int x = 0; x < layer.Width; x++) {
							Rectangle destination = new Rectangle (x * map.TileWidth, y * map.TileHeight, map.TileWidth, map.TileHeight);
							int tileID = layer.Data [y, x] - 1;
							if (tileID > 0) {
								GameTile tile = map.Tiles [tileID];
								if (!EntitySpawners.ContainsKey (tile.SpawnType))
									spriteBatch.Draw (tile.display.texture, destination, tile.display.rect, Color.White);
							}
						}
					}
				}

				spriteBatch.End ();

				graphics.GraphicsDevice.SetRenderTarget (null);
			}

			spriteBatch.Begin(SpriteSortMode.Deferred,null, null, null, null, null,screenMatrix);
			GameTile tiles = map.Tiles[0];
			//spriteBatch.Draw(tiles.display.texture, new Vector2(0,0));
			spriteBatch.Draw (mapTex, new Vector2 (0, 0), Color.White);
			spriteBatch.End ();
			base.Draw (screenMatrix, spriteBatch, graphics, drawHitBoxes);
		}
	}
}

