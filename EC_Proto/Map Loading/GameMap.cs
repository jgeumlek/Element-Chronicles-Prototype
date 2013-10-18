using System;
using TiledMax;

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Content;
using System.Collections.ObjectModel;


namespace EC_Proto
{
	public class GameMap
	{
		private String mapFile;
		public Dictionary<int,GameTile> Tiles = new Dictionary<int,GameTile> ();
		public Collection<Layer> Layers;
		public int TileWidth = 0;
		public int TileHeight = 0;

		public GameMap (String mapFile)
		{
			this.mapFile = mapFile;
			GameTile empty = new GameTile ();
			Tiles.Add (0, empty);
		}

		public void Load(ContentManager content, List<TerrainEntity> blocks) {
			//This process can be sped up by editing the TiledMax files to parse directly into our data structures.
			TiledMax.Map map = TiledMax.Map.Open (mapFile);
			Layers = map.Layers;
			int tileID = 1;

			//Read in textures and set up convenient structures for drawing tiles.
			foreach (TiledMax.TileSet ts in map.TileSets) {
				Texture2D tileSheet = content.Load<Texture2D> (ts.Images [0].Source);
				TileWidth = ts.TileWidth;
				TileHeight = ts.TileHeight; //Multiple tileset sizes? How to handle? Take the max?


				//TODO: Account for margins and spacing here!
				int rowCount = tileSheet.Width / TileWidth;
				int colCount = tileSheet.Height / TileHeight;
				int tileCount = rowCount * colCount;
				int currentRow = 0;
				int currentCol = 0;

				//Note: tileID is held outside of loop to handle multiple tilesheets; each starts off where the last left.
				//The code below should be rewritten so that tile.ID does not require addition, and instead the loop
				//boundaries take on the additional math.

				for (int i = 0; i < tileCount; i++) {
					GameTile tile = new GameTile ();
					tile.display.texture = tileSheet;
					tile.display.rect = new Rectangle (currentCol * TileWidth, currentRow * TileHeight, TileWidth, TileHeight); //Rectangle with this tile's texture on the sheet.
					tile.ID = tileID + i;
					if (ts.TileProperties.ContainsKey (tileID + i - 1)) {//Need to actually check what properties these are! This is a placeholder.
						TiledMax.Properties props = ts.TileProperties [tileID + i - 1];
						if (props.ContainsKey ("solid")) 
							tile.solid = true;
						if (props.ContainsKey ("spawn"))
							tile.SpawnType = (String)props ["spawn"];
					}
					Tiles.Add (tileID + i, tile);
					currentCol++;
					if (currentCol > colCount) {
						currentCol = 0;
						currentRow++;
					}
				}
				tileID += tileCount;
			}
			//Add in block entities for each solid tile
			foreach (TiledMax.Layer layer in map.Layers) {
				int width = layer.Width;
				int height = layer.Height;
				for (int y = 0; y < layer.Height; y++) {
					for (int x = 0; x < layer.Width; x++) {
						Rectangle destination = new Rectangle (x * map.TileWidth, y * map.TileHeight, map.TileWidth, map.TileHeight);
						tileID = layer.Data [x, y] - 1;
						if (tileID > 0) {
							GameTile tile = Tiles [tileID];
							if (tile.solid) {
								blocks.Add (new TerrainEntity (destination));
							}
						}
					}
				}
			}
		}

		//Should make a Unload method to clean up?

	}
}
