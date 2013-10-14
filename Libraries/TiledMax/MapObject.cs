using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace TiledMax
{
    public class MapObject
    {
        public string Name { get; set; }  // The name of the object. An arbitrary string.
        public string Type { get; set; }  // The type of the object. An arbitrary string.
        public int X { get; set; }        // The x coordinate of the object in pixels.
        public int Y { get; set; }        // The y coordinate of the object in pixels.
        public int Width { get; set; }    // The width of the object in pixels.
        public int Height { get; set; }   // The height of the object in pixels.
        public int Gid { get; set; }      // An reference to a tile (optional).
        public Properties Properties { get; set; }
        public Collection<Image> Images { get; set; }

        public MapObject()
        {
            Properties = new Properties();
            Images = new Collection<Image>();
        }
    }
}
