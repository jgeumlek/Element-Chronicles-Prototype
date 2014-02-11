using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class PlayerEntity : PhysicsEntity
	{
		private const int KNOCKBACK_DISTANCE = -50;
		private const int FLINCH_TIME = 500; // how long the player flinches for in milliseconds
		private const float DEFAULT_FRICTION = .9f;
		//private float playerspeed = 5;
		//private Vector2 currentSpeed = new Vector2(0,0); //Used for adding momentum to projectiles.
		private float maxSpeed = 5;
		private float acceleration = 2;
		public float FrictionFactor = .9f;


		private Vector2 resetPosition = new Vector2 (0, 0);
		bool collidedWithTerrain = false; //For really hack-ish collision resolution! Needs to be reworked.
		static AnimationManager anim = new AnimationManager();
		public static Texture2D texture; 
		public bool strength = false; // Able to push boulders?
		public TimeSpan flinchTime;

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

		public PlayerEntity () {
			spriteChoice.texture = texture;
			hitbox = new Rectangle (10, 110, 60, 27);
			hurtbox = new Rectangle (10, 5, 60, 140);
			direction = Direction.South;
			animState.AnimationName = "south";
			inverseMass = 5;
		}

		//Need to clean up constructors, and use base class better
		public PlayerEntity(Vector2 position) : this()
		{
			this.position = position;

			resetPosition.X = position.X;
			resetPosition.Y = position.Y;
		}

		public Vector2 getCurrentSpeed () {
			return momentum;
		}

		public Vector2 Center() {
		return position + new Vector2 (40, 75);
		}

		public override void Update (KeyboardState keyboard, GameTime gameTime) {



			if (!collidedWithTerrain) { //Really lazy collsion resolution. Needs work.
				//SetResetPosition (position);
			} else {
				collidedWithTerrain = false;
			}

			bool changedirection = true; //Should we let the player character change directions?
			Vector2 moveDirection = new Vector2 (0, 0);
			Direction newDirection = Direction.Undefined;

			if (keyboard.IsKeyDown (Keys.R)) { //strafing. Don't change directions.
				changedirection = false;
			}

			if (flinchTime <= TimeSpan.Zero) {
				if (keyboard.IsKeyDown (Keys.A)) {
					newDirection = Direction.West;
					if (direction == newDirection) {
						changedirection = false;
					} //Already heading this way; don't turn, just move diagonally.
					moveDirection += Entity.dirVector (newDirection);
				}
				if (keyboard.IsKeyDown (Keys.D)) {
					newDirection = Direction.East;
					if (direction == newDirection) {
						changedirection = false;
					}
					moveDirection += Entity.dirVector (newDirection);
				}
				if (keyboard.IsKeyDown (Keys.W)) {
					newDirection = Direction.North;				
					if (direction == newDirection) {
						changedirection = false;
					}
					moveDirection += Entity.dirVector (newDirection);
				}
				if (keyboard.IsKeyDown (Keys.S)) {
					newDirection = Direction.South;
					if (direction == newDirection) {
						changedirection = false;
					}

					moveDirection += Entity.dirVector (newDirection);
				}

				if (changedirection && newDirection != Direction.Undefined) {
					direction = newDirection; //We aren't still heading the same way, and we are moving. We should change direction.
					animState = anim.Update (animState, Entity.dirName (newDirection));
				}

				momentum += moveDirection * acceleration;
			


			} else if (flinchTime > TimeSpan.Zero) {
				flinchTime -= gameTime.ElapsedGameTime;
			}


				moveOffset (momentum);
				spriteChoice.rect = anim.GetRectangle (animState);
				momentum *= FrictionFactor;
				if (momentum.LengthSquared() > maxSpeed*maxSpeed) {
					momentum.Normalize ();
					momentum *= maxSpeed;
				}
				if (momentum.LengthSquared() < .5) {
					momentum.X = 0;
					momentum.Y = 0;
				}
				Console.Out.WriteLine (momentum);
				momentum *= FrictionFactor;
				Console.Out.WriteLine ("after" + momentum.ToString());
				//Set up friction factor for next frame.
				FrictionFactor = DEFAULT_FRICTION;
		}

		//Collision rules.
		override public void CollidedWith(Entity e) {
			if (e is WaterEntity && e.Collidable) {

			}
		}

		public override void AnimationTick ()
		{
			if (momentum.LengthSquared() > 2) animState = anim.Tick(animState);
		}

		//Set where player goes when out of bounds/in a pit/drowned/etc.
		public void SetResetPosition(Vector2 resetPosition) {
			this.resetPosition.X = resetPosition.X;
			this.resetPosition.Y = resetPosition.Y;
		}

		//Warp to the player's reset position.
		public void ResetWarp () {
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

		public void KnockBack () {
			//position += KNOCKBACK_DISTANCE * Entity.dirVector (direction);
			flinchTime = new TimeSpan (0, 0, 0, 0, FLINCH_TIME);
		}

		public void Hit (int damage) {
			PlayerStats.curHp -= damage;
		}

		public void ConsumeMana (int mana) {
			PlayerStats.curMana -= mana;
		}

		public void PlusExp (int exp) {
			PlayerStats.curExp += exp;
		}
	}
}
