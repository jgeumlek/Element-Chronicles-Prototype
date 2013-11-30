using System;
using Microsoft.Xna.Framework;

namespace EC_Proto
{
	public class CameraManager
	{
		public CameraManager ()
		{
		}

		static public Matrix LookAtPoint(Point point, int viewWidth, int viewHeight, float zoomLevel, int mapWidth, int mapHeight) {
			Matrix screenMatrix = Matrix.Identity;

			//These calculations requirement significant refinement! They are pretty naive in several ways.
			screenMatrix = Matrix.CreateTranslation (-point.X, -point.Y, 1);

			screenMatrix *= Matrix.CreateScale (zoomLevel, zoomLevel, 1);
			screenMatrix *= Matrix.CreateTranslation (300,200,0);

			Matrix invScreen = Matrix.Invert (screenMatrix);
			Vector2 topLeft = Vector2.Transform(new Vector2(0,0), invScreen);
			Vector2 bottomRight = Vector2.Transform (new Vector2 (viewWidth, viewHeight), invScreen);

			//Clip with map boundaries! If a corner coordinate is valid, no translation needed.
			topLeft.X = topLeft.X < 0 ? topLeft.X : 0;
			topLeft.Y = topLeft.Y < 0 ? topLeft.Y : 0;

			bottomRight.X = bottomRight.X > mapWidth  ? bottomRight.X - mapWidth  : 0;
			bottomRight.Y = bottomRight.Y > mapHeight ? bottomRight.Y - mapHeight : 0;

			screenMatrix *= Matrix.CreateTranslation (zoomLevel*topLeft.X,zoomLevel*topLeft.Y,0); //Fix scrolling too far left/up
			screenMatrix *= Matrix.CreateTranslation (zoomLevel*bottomRight.X,zoomLevel*bottomRight.Y,0); //Fix scrolling too far right/down

			return screenMatrix;
		}
	}
}

