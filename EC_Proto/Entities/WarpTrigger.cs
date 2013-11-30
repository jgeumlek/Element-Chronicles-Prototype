using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EC_Proto
{
		public class WarpTrigger : TerrainEntity
		{
			public static Texture2D texture;
			private GameState game;
			private String mapName;
			private String locationTarget;



			public WarpTrigger (String mapName, String locationTarget, Rectangle rect, GameState game) {
				position.X = rect.X;
				position.Y = rect.Y;
				hitbox = new Rectangle (0, 0, rect.Width, rect.Height);
				spriteChoice.texture = texture;
				spriteChoice.rect = new Rectangle (0, 0, texture.Width, texture.Height);
				Visible = true;
				this.game = game;
				this.mapName = mapName;
				this.locationTarget = locationTarget;
			}





			override public void CollidedWith (Entity e) {
				if ( e is PlayerEntity) {
					game.LoadMap (mapName, locationTarget);
				}
			}
		}



}

