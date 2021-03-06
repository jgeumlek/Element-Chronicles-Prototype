﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace TiledMax
{
    public class Layer
    {
        public string Name { get; set; }           // The name of the layer.
        public int X { get; set; }                 // The x coordinate of the layer in tiles. Defaults to 0 and can no longer be changed in Tiled Qt.
        public int Y { get; set; }                 // The y coordinate of the layer in tiles. Defaults to 0 and can no longer be changed in Tiled Qt.
        public int Width { get; set; }             // The width of the layer in tiles. Traditionally required, but as of Tiled Qt always the same as the map width.
        public int Height { get; set; }            // The height of the layer in tiles. Traditionally required, but as of Tiled Qt always the same as the map height.
        public double Opacity { get; set; }        // The opacity of the layer as a value from 0 to 1. Defaults to 1.
        public bool Visible { get; set; }          // Whether the layer is shown (1) or hidden (0). Defaults to 1.
        public Properties Properties { get; set; }
        public int[,] Data { get; set; }

        public Layer(int width, int height)
        {
            Properties = new Properties();
            Width = width;
            Height = height;
            Data = new int[Height, Width];
        }
    }
}
