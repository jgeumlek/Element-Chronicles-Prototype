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

		public PlayerEntity ()
		{
			hitbox = new Rectangle (0, 16, 32, 16);
			hurtbox = new Rectangle (0, 0, 32, 32);
		}
		//Need to clean up constructors, and use base class better
		public PlayerEntity(Vector2 position, Texture2D texture) : this()
		{
			this.position = position;
			spriteChoice.texture = texture;
		}

		public Vector2 getCurrentSpeed () {
			return currentSpeed;
		}
		
		public override void Update (KeyboardState keyboard, GameTime gameTime) {
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
	}
}

