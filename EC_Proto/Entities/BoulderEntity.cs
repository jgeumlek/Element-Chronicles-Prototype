using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class BoulderEntity : TerrainEntity
	{
		static public Texture2D texture;
		private Vector2 resetPosition = new Vector2 (0, 0);
		bool collidedWithTerrain = false;

		public BoulderEntity ()
		{
			Visible = true;
		}

		public BoulderEntity (Rectangle rect) {
			position.X = rect.X;
			position.Y = rect.Y;
			spriteChoice.texture = texture;
			spriteChoice.rect = texture.Bounds;
			hitbox = texture.Bounds;
			Visible = true;
		}

		public override void Update (KeyboardState state, GameTime time) {
			if (!collidedWithTerrain) { //Really lazy collsion resolution. Needs work.
				SetResetPosition (position);
			} else {
				collidedWithTerrain = false;
			}
		}

		override public void CollidedWith (Entity e) {
			if (e is TerrainEntity) {
				collidedWithTerrain = true;
				ResetWarp ();
			}
			if (e is PlayerEntity) {
				position += 2 * Entity.dirVector (((PlayerEntity)e).direction); // ((PlayerEntity)e).getCurrentSpeed ()
			}
		}

		//Set where boulder goes when out of bounds/in a pit/underwater/etc.
		public void SetResetPosition(Vector2 resetPosition) {
			this.resetPosition.X = resetPosition.X;
			this.resetPosition.Y = resetPosition.Y;
		}

		//Warp to the player's reset position.
		public void ResetWarp() {

			position.X = resetPosition.X;
			position.Y = resetPosition.Y;
		}
	}
}