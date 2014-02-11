using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
	public class GuiTexture
	{
		Texture2D texture;
		
		public GuiTexture (Texture2D texture, Vector2 position, Vector2 size)
		{
			this.texture = texture;
			//this.position = position;
			//this.size = size;
		}
	}
}

