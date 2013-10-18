using System.Collections.ObjectModel;
using System.Drawing;
using System;
using System.IO;
using System.Collections.Generic;

namespace TiledMax
{
    /// <summary>
    /// A tileset that provides methods for reading tiles associated with this map.
    /// </summary>
    public class TileSet
    {
        public int FirstGid { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int Spacing { get; set; }
        public int Margin { get; set; }
        public Collection<Image> Images { get; set; }
        public Collection<Tile> Tiles { get; set; }
        public Collection<Bitmap> Bitmaps { get; set; }

		//This dictionary is hacked-on addition to TiledMax.
		public Dictionary<int, Properties> TileProperties = new Dictionary<int, Properties>();

        public TileSet()
        {
            Images = new Collection<Image>();
            Tiles = new Collection<Tile>();
            Bitmaps = new Collection<Bitmap>();
        }

        /// <summary>
        /// This method cuts the image (if loaded) into tiles, and places them in the Bitmaps array.
        /// </summary>
        public void ReadBitmaps(string base_path)
        {
            // Crop the image margin.
            Bitmap CropImage = null;

            if (Images.Count > 0)
            {
                try
                {
                    Bitmaps.Clear();

                    string filename = Path.Combine(base_path, Images[0].Source);

                    Bitmap SourceImage = new Bitmap(filename);
                    Rectangle SourceCrop = new Rectangle(Margin, Margin, SourceImage.Width - Margin * 2, SourceImage.Height - Margin * 2);
                    Rectangle DestCrop = new Rectangle(0, 0, SourceCrop.Width, SourceCrop.Height);

                    CropImage = new Bitmap(SourceCrop.Width, SourceCrop.Height);
                    using (Graphics g = Graphics.FromImage(CropImage))
                    {
                        g.DrawImage(SourceImage, DestCrop, SourceCrop, GraphicsUnit.Pixel);
                    }

                    if (Images[0].UseTransColor)
                    {
                        CropImage.MakeTransparent(Images[0].TransColor);
                    }

                    int nextX = 0;

                    Rectangle ScanRect = new Rectangle(0, 0, TileWidth, TileHeight); // This rectangle moves.
                    Rectangle DestRect = new Rectangle(0, 0, TileWidth, TileHeight);
                    for (int i = 0; i < 1000; i++)
                    {
                        Bitmap newTile = new Bitmap(TileWidth, TileHeight);
                        using (Graphics g = Graphics.FromImage(newTile))
                        {
                            g.DrawImage(CropImage, DestRect, ScanRect, GraphicsUnit.Pixel);
                        }
                        Bitmaps.Add(newTile);

                        nextX = ScanRect.Left + TileWidth + Spacing;
                        if (nextX < CropImage.Width + 7)
                        {
                            ScanRect.X = nextX;
                        }
                        else
                        {
                            ScanRect.Y += TileHeight + Spacing;
                            ScanRect.X = 0;
                            if (ScanRect.Bottom > CropImage.Height) { break; }
                        }
                    }
                } 
                catch (FileNotFoundException ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception("No tileset images were found.");
            }
        }
    }
}
