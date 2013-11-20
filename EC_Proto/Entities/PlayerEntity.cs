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
		static AnimationManager anim = new AnimationManager();
		public static Texture2D texture; 
		public bool strength = true;

		static public void InitAnimation() {
			anim.AddAnimation ("south", 0, 0, 32, 32,4);
			anim.AddAnimation ("west", 0, 32, 32, 32,4);
			anim.AddAnimation ("east", 0, 64, 32, 32,4);
			anim.AddAnimation ("north", 0, 96, 32, 32,4);

			anim.AddStateChange ("", "west", "west", false);
			anim.AddStateChange ("", "south", "south", false);
			anim.AddStateChange ("", "east", "east", false);
			anim.AddStateChange ("", "north", "north", false);
		}

		public PlayerEntity ()
		{
			spriteChoice.texture = texture;
			hitbox = new Rectangle (10, 24, 12, 8);
			hurtbox = new Rectangle (0, 0, 32, 32);
			direction = Direction.South;
			animState.AnimationName = "south";

		}
		//Need to clean up constructors, and use base class better
		public PlayerEntity(Vector2 position) : this()
		{
			this.position = position;

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
				animState = anim.Update (animState,Entity.dirName(newDirection));
			}

			currentSpeed = moveDirection * playerspeed;
			moveOffset (currentSpeed);
			spriteChoice.rect = anim.GetRectangle (animState);


		}

		//Collision rules.
		override public void CollidedWith(Entity e) {
			if (e is TerrainEntity && e.Collidable) {
				collidedWithTerrain = true;
				ResetWarp ();
			}
		}

		public override void AnimationTick ()
		{
			if (currentSpeed.LengthSquared() > 2) animState = anim.Tick(animState);
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

		public void EarthenShield () {
			strength = true;
		}
	}
}