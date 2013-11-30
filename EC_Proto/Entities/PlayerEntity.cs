using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class PlayerEntity : Entity
	{
		private float playerspeed = 5;
		private Vector2 currentSpeed = new Vector2(0,0); //Used for adding momentum to projectiles.
		private Vector2 resetPosition = new Vector2 (0, 0);
		bool collidedWithTerrain = false; //For really hack-ish collision resolution! Needs to be reworked.
		static AnimationManager anim = new AnimationManager();
		public static Texture2D texture; 
		public bool strength = false; // Able to push boulders?

		static public void InitAnimation() {
			anim.AddAnimation ("south", 230, 0, 80, 150,1); //TODO: Fix spritesheet alignment! The others seem to be multiples of 80.
			anim.AddAnimation ("west", 320, 0, 80, 150,1);
			anim.AddAnimation ("east", 400, 0, 80, 150,1);
			anim.AddAnimation ("north", 80, 0, 80, 150,1);

			anim.AddStateChange ("", "west", "west", false);
			anim.AddStateChange ("", "south", "south", false);
			anim.AddStateChange ("", "east", "east", false);
			anim.AddStateChange ("", "north", "north", false);
		}

		public PlayerEntity ()
		{
			spriteChoice.texture = texture;
			hitbox = new Rectangle (10, 110, 60, 27);
			hurtbox = new Rectangle (10, 5, 60, 140);
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

		public Vector2 Center() {
		return position + new Vector2 (40, 75);
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


			if (keyboard.IsKeyDown (Keys.A)) {
				newDirection = Direction.West;
				if (direction == newDirection) { changedirection = false; } //Already heading this way; don't turn, just move diagonally.
				moveDirection += Entity.dirVector (newDirection);
			}
			if (keyboard.IsKeyDown (Keys.D)) {

				newDirection = Direction.East;
				if (direction == newDirection) { changedirection = false; }
				moveDirection += Entity.dirVector (newDirection);
			}
			if (keyboard.IsKeyDown (Keys.W)) {

				newDirection = Direction.North;				
				if (direction == newDirection) { changedirection = false; }

				moveDirection += Entity.dirVector (newDirection);
			}
			if (keyboard.IsKeyDown (Keys.S)) {

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
		//Move to the center of a rectangle, and update the reset positions as well.
		public void MoveToRect(Rectangle location) {
			Point center = location.Center;
			SetResetPosition (new Vector2 (center.X - 40, center.Y - 90)); //HACK: subtract to reach top left corner! Should probably clean up player entity interface.
			ResetWarp ();
                }
	}
}
