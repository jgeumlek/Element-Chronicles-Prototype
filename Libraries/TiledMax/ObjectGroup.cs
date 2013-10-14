using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace TiledMax
{
    public class ObjectGroup : Collection<MapObject>
    {
        public string Name; // The name of the object group.
        public int X; // The x coordinate of the object group in tiles. Defaults to 0 and can no longer be changed in Tiled Qt.
        public int Y; // The y coordinate of the object group in tiles. Defaults to 0 and can no longer be changed in Tiled Qt.
        public int Width; // The width of the object group in tiles. Meaningless.
        public int Height; // The height of the object group in tiles. Meaningless.
    }
}
