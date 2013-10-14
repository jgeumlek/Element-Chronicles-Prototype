using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace TiledMax
{
    /// <summary>
    /// Represents an image in a tiled map.
    /// </summary>
    public class Image
    {
        public string Source { get; set; }
        public Color TransColor { get; set; }
        public bool UseTransColor { get; set; }
    }
}
