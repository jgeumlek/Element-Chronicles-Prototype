using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class PlayerEntity : Entity
	{


		private float playerspeed = 2;
		private Vector2 currentSpeed = new Vector2(0,0); //Used for adding momentum to projectiles.
		private Vector2 resetPosition = new Vector2 (0, 0);
		bool collidedWithTerrain = false; //For really hack-ish collision resolution! Needs to be reworked.

		public PlayerEntity ()
		{
			hitbox = new Rectangle (10, 24, 12, 8);
			hurtbox = new Rectangle (0, 0, 32, 32);

		}
		//Need to clean up constructors, and use base class better
		public PlayerEntity(Vector2 position, Texture2D texture) : this()
		{
			this.position = position;
			spriteChoice.texture = texture;
			resetPosition.X = position.X;
			resetPosition.Y = position.Y;
		}

		public Vector2 getCurrentSpeed () {
			return currentSpeed;
		}
		
		public override void Update (KeyboardState keyboard, GameTime gameTime) {

			if (!collidedWithTerrain) { //Really lazy collsion resolution. Needs work.
				SetResetPosition (position);
			} else {
				collidedWithTerrain = false;
			}

			bool changedirection = true; //Should we let the player character change directions?
			Vector2 moveDirection = new Vector2 (0, 0);
			Direction newDirection = Direction.Undefined;

			if (keyboard.IsKeyDown (Keys.R)) { //strafing. Don't change directions.
				changedirection = false;
			}


			if (keyboard.IsKeyDown (Keys.Left)) {

				newDirection = Direction.West;
				if (direction == newDirection) { changedirection = false; } //Already heading this way; don't turn, just move diagonally.
				moveDirection += Entity.dirVector (newDirection);

			}
			if (keyboard.IsKeyDown (Keys.Right)) {

				newDirection = Direction.East;
				if (direction == newDirection) { changedirection = false; }
				moveDirection += Entity.dirVector (newDirection);
			}
			if (keyboard.IsKeyDown (Keys.Up)) {

				newDirection = Direction.North;				
				if (direction == newDirection) { changedirection = false; }

				moveDirection += Entity.dirVector (newDirection);
			}
			if (keyboard.IsKeyDown (Keys.Down)) {

				newDirection = Direction.South;
				if (direction == newDirection) { changedirection = false; }

				moveDirection += Entity.dirVector (newDirection);
			}

			if (changedirection && newDirection != Direction.Undefined) {
				direction = newDirection; //We aren't still heading the same way, and we are moving. We should change direction.
			}

			currentSpeed = moveDirection * playerspeed;
			moveOffset (currentSpeed);


		}

		//Collision rules.
		override public void CollidedWith(Entity e) {
			if (e is TerrainEntity) {
				collidedWithTerrain = true;
				ResetWarp ();
			}
		}

		//Set where player goes when out of bounds/in a pit/drowned/etc.
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

