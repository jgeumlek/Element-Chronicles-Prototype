using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class PlayerEntity : PhysicsEntity
	{
		// Physics
		private const float DEFAULT_FRICTION = .9f;
		private float maxSpeed = 5.5f;
		private float acceleration = 2;
		public float FrictionFactor = .9f;
		private Vector2 resetPosition = new Vector2 (0, 0);
		bool collidedWithTerrain = false; //For really hack-ish collision resolution! Needs to be reworked.

		KeyboardState prevKeyboardState;

		// Knockback
		private const int KNOCKBACK_DISTANCE = -50;
		private const int FLINCH_TIME = 500; // how long the player flinches for in milliseconds
		public TimeSpan flinchTime;

		// Spells
		public bool projectileLaunched;
		private TimeSpan timer;
		public Entity overlay; // A spell that displays an animation in front of the player creates an overlay
		public bool overlayActive;

		// Animation
		public static Texture2D spritesheet;
		static AnimationManager anim = new AnimationManager();
		public static Texture2D texture; 

		static public void InitAnimation() {
			anim.AddAnimation ("eastidle", 0, 0, 80, 150,1);
			anim.AddAnimation ("westidle", 0, 150, 80, 150,1);
			anim.AddAnimation ("southidle", 0, 300, 80, 150,1);
			anim.AddAnimation ("northidle", 0, 450, 80, 150,1);

			anim.AddAnimation ("east", 0, 0, 80, 150, 7);
			anim.AddAnimation ("west", 0, 150, 80, 150, 7);
			anim.AddAnimation ("south", 0, 300, 80, 150, 7);
			anim.AddAnimation ("north", 0, 450, 80, 150, 7);


			anim.AddStateChange ("", "west", "west", true);
			anim.AddStateChange ("", "south", "south", true);
			anim.AddStateChange ("", "east", "east", true);
			anim.AddStateChange ("", "north", "north", true);

			anim.AddStateChange ("west", "undefined", "westidle", false);
			anim.AddStateChange ("south", "undefined", "southidle", false);
			anim.AddStateChange ("east", "undefined", "eastidle", false);
			anim.AddStateChange ("north", "undefined", "northidle", false);

			anim.AddStateChange ("westidle", "west", "west", false);
			anim.AddStateChange ("southidle", "south", "south", false);
			anim.AddStateChange ("eastidle", "east", "east", false);
			anim.AddStateChange ("northidle", "north", "north", false);
		}

		public PlayerEntity () {
			spriteChoice.texture = texture;
			hitbox = new Rectangle (10, 110, 60, 27);
			hurtbox = new Rectangle (10, 5, 60, 140);
			direction = Direction.South;
			animState.AnimationName = "southidle";

			inverseMass = 1;


			projectileLaunched = false;
			timer = new TimeSpan (0, 0, 0, 0, 500);
			baseline = hitbox.Bottom;

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
			bool startwalking = false;

			Vector2 moveDirection = new Vector2 (0, 0);
			Direction newDirection = Direction.Undefined;

			if (keyboard.IsKeyDown (Keys.R)) { //strafing. Don't change directions.
				changedirection = false;
			}

			if (keyboard.IsKeyDown (Keys.A)) {
				if (prevKeyboardState.IsKeyUp (Keys.A)) {
					startwalking = true;
				}

				newDirection = Direction.West;
				if (direction == newDirection) {
					changedirection = false;
				} //Already heading this way; don't turn, just move diagonally.
				moveDirection += Entity.dirVector (newDirection);
			}
			if (keyboard.IsKeyDown (Keys.D)) {
				if (prevKeyboardState.IsKeyUp (Keys.D)) {
					startwalking = true;
				}

				newDirection = Direction.East;
				if (direction == newDirection) {
					changedirection = false;
				}
				moveDirection += Entity.dirVector (newDirection);
			}
			if (keyboard.IsKeyDown (Keys.W)) {
				if (prevKeyboardState.IsKeyUp (Keys.W)) {
					startwalking = true;
				}

				newDirection = Direction.North;				
				if (direction == newDirection) {
					changedirection = false;
				}
				moveDirection += Entity.dirVector (newDirection);
			}
			if (keyboard.IsKeyDown (Keys.S)) {
				if (prevKeyboardState.IsKeyUp (Keys.S)) {
					startwalking = true;
				}

				newDirection = Direction.South;
				if (direction == newDirection) {
					changedirection = false;
				}
				moveDirection += Entity.dirVector (newDirection);
			}


			if (startwalking) {
				direction = newDirection;
				animState = anim.Update (animState, Entity.dirName(newDirection));
			}
//			if (changedirection && newDirection != Direction.Undefined) {
//				direction = newDirection; //We aren't still heading the same way, and we are moving. We should change direction.
//				animState = anim.Update (animState, Entity.dirName (newDirection));
//			}
			else if (keyboard.IsKeyUp (Keys.W) && keyboard.IsKeyUp (Keys.A) && keyboard.IsKeyUp (Keys.S) && keyboard.IsKeyUp (Keys.D)) {
				animState = anim.Update (animState, "undefined");
			}



			if (projectileLaunched) {
				if (timer > TimeSpan.Zero) {
					timer -= gameTime.ElapsedGameTime;
				} else {
					projectileLaunched = false;
					timer = new TimeSpan (0, 0, 0, 0, 500);
				}
			}

			// Knockback flinch
			if (flinchTime <= TimeSpan.Zero) {
				momentum += moveDirection * acceleration;
			} else if (flinchTime > TimeSpan.Zero) {
				flinchTime -= gameTime.ElapsedGameTime;
			}

			//moveOffset (momentum);
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

			momentum *= FrictionFactor;
			
			//Set up friction factor for next frame.
			FrictionFactor = DEFAULT_FRICTION;

			prevKeyboardState = keyboard;
		}

		//Collision rules.
		override public void CollidedWith(Entity e) {
			if (e is ScrollEntity) {
				string spellName = (string)((ScrollEntity)e).properties ["spell"];
				SpellManager.spells [spellName] = true;

				if (spellName == "fireball") {
					Gui.UpdateDialog ("You learned the spell Fireball.\nPress H to use.");
				} else if (spellName == "frostbreath") {
					Gui.UpdateDialog ("You learned the spell Frost Breath.\nPress J to use.");
				} else if (spellName == "earthen shield") {
					Gui.UpdateDialog ("You learned the spell Earthen Strength.\nPress K to toggle.");
				} else if (spellName == "windwalk") {
					Gui.UpdateDialog ("You learned the spell Wind Walk.\nPress L to toggle.");
				}

				e.Destroy ();
			} else if (e is WarpTrigger) {
				if (overlayActive) {
					DestroyOverlay ();
					SpellManager.activeSpell = "";
				}
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

		public void Fireball () {
			FireballEntity fireball = new FireballEntity (GameScene.player.Center(), GameScene.player.direction, GameScene.player.getCurrentSpeed());
			GameScene.AddSpellEntity (fireball);
			projectileLaunched = true;
			GameScene.player.ConsumeMana (2);
		}

		public void FrostBreath () {
			FrostEntity frost = new FrostEntity (GameScene.player.Center() + new Vector2(7,10), GameScene.player.direction, GameScene.player.getCurrentSpeed());
			GameScene.AddSpellEntity (frost);
			projectileLaunched = true;
			GameScene.player.ConsumeMana (1);
		}

        public void EarthenStrength () {
			overlay = new PlayerRocksEntity (GameScene.player.position);
			GameScene.AddSpellEntity (overlay);
			overlayActive = true;
			ConsumeMana (1);
			SpellManager.activeSpell = "earthen shield";
        }

		public void WindWalk () {
			overlay = new PlayerWindEntity (GameScene.player.position);
			GameScene.AddSpellEntity (overlay);
			overlayActive = true;
			ConsumeMana (1);
			SpellManager.activeSpell = "windwalk";
		}

		public void DestroyOverlay () {
			overlayActive = false;
			overlay.Destroy ();
		}

		//Move to the center of a rectangle, and update the reset positions as well.
		public void MoveToRect(Rectangle location) {
			Point center = location.Center;
			SetResetPosition (new Vector2 (center.X - 40, center.Y - 90)); //HACK: subtract to reach top left corner! Should probably clean up player entity interface.
			ResetWarp ();
		}

		public void KnockBack () {
			position += KNOCKBACK_DISTANCE * Entity.dirVector (direction);
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

		public bool HasEnoughMana (float requiredMana) {
			if (PlayerStats.curMana >= requiredMana)
				return true;
			else
				return false;
		}
	}
}
