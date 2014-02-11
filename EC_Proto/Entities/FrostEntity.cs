using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EC_Proto
{
	public class FrostEntity : ProjectileEntity
	{
		private const float FROST_SPEED = 1;
		private const int FROST_LIFESPAN = 40;
		private const float FROST_DAMAGE = 3;
		public static Texture2D texture;
		static AnimationManager anim = new AnimationManager();
		static AnimationManager animHitBox = new AnimationManager();

		public FrostEntity () {
			speed = FROST_SPEED;
			lifespan = FROST_LIFESPAN;
			//fireDamage = FROST_DAMAGE;
			hitbox = new Rectangle (20, 20, 60, 60);
			spriteChoice.texture = texture;
			spriteChoice.rect = new Rectangle (0, 0, 32, 32);
		}

		static public void InitAnimation() {
			anim.AddAnimation ("south", 100, 0, 100, 100,1); 
			anim.AddAnimation ("west", 100, 100, 100, 100,1);
			anim.AddAnimation ("east", 0, 0, 100, 100,1);
			anim.AddAnimation ("north", 0, 100, 100, 100,1);

			animHitBox.AddAnimation ("south", 40, 70, 25, 20, 1); 
			animHitBox.AddAnimation ("west", 10, 40, 20, 25,1);
			animHitBox.AddAnimation ("east", 70, 40, 20, 25,1);
			animHitBox.AddAnimation ("north", 30, 10, 25, 20,1);
		}

		public FrostEntity (Vector2 position, Direction direction, Vector2 momentum) : this()
		{
			this.position = position - new Vector2(50,50); //Subtract to fix top left corner. TODO: Clean up entity interface to handle this.
			this.direction = direction;
			movement = momentum + speed * Entity.dirVector (direction);
			animState.AnimationName = Entity.dirName (direction);
			spriteChoice.rect = anim.GetRectangle (animState);
			hitbox = animHitBox.GetRectangle (animState);
		}

		public override void Update (KeyboardState keyboard, GameTime gameTime) {
			moveOffset(movement);
			lifespan--;

			if (lifespan < 0) {
				alive = false;
			}
		}

		override public void CollidedWith(Entity e) {
			if (e is TerrainEntity && e.Collidable) {
				alive = false;
			}
		}
	}
}
