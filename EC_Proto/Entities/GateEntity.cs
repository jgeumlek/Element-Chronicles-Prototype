using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TiledMax;


namespace EC_Proto
{
	public class GateEntity : TerrainEntity
	{

		public static Texture2D gateTex;
		static AnimationManager anim = new AnimationManager();
		List<String> require = new List<String>();
		List<String> forbid = new List<String>();
		List<String> any = new List<String>();
		GameScene gs;


		public GateEntity (Rectangle rect, Properties properties, GameScene gs)
		{
			position.X = rect.X;
			position.Y = rect.Y;
			spriteChoice.texture = gateTex;
			animState.AnimationName = "closed";
			Collidable = true;
			Visible = true;

			if (properties.ContainsKey ("any")) {
				foreach (String identifier in ((String)properties["any"]).Split(',')) {
					any.Add (identifier);
				}

			}

			if (properties.ContainsKey ("require")) {
				foreach (String identifier in ((String)properties["require"]).Split(',')) {
					require.Add (identifier);
				}


			}
			if (properties.ContainsKey ("forbid")) {
				foreach (String identifier in ((String)properties["forbid"]).Split(',')) {
					forbid.Add (identifier);
				}
			}




			hitbox = new Rectangle(0,0,100,100);
			Visible = true;
			this.gs = gs;
		}

		public override bool Activated() {
			return AnySatisfied () || (RequireSatisfied () && ForbidSatisfied ());
		}

		private bool AnySatisfied() {
			foreach (String ident in any) {
				if (gs.NamedEntities.ContainsKey (ident) && gs.NamedEntities [ident].Activated ()) {
					return true;
				}
			}
			return false;
		}

		private bool RequireSatisfied() {
			if (require.Count == 0)
				return false;
			foreach (String ident in require) {
				if (gs.NamedEntities.ContainsKey (ident) && !gs.NamedEntities [ident].Activated ()) {
					return false;
				}
			}
			return true;
		}

		private bool ForbidSatisfied() {
			if (forbid.Count == 0)
				return true;
			foreach (String ident in forbid) {
				if (gs.NamedEntities.ContainsKey (ident) && gs.NamedEntities [ident].Activated ()) {
					return false;
				}
			}
			return true;
		}


		public override void Update (KeyboardState state, GameTime time) {

		}

		public override void AnimationTick ()
		{
			animState = anim.Tick (animState);
			spriteChoice.rect = anim.GetRectangle (animState);

			if (animState.AnimationName == "open") {
				Collidable = false;
			} else {
				Collidable = true;
			}


			if (animState.AnimationName.Equals("closed") && Activated ()) {
				animState = anim.Update (animState, "open");
			} else if (animState.AnimationName == "open" && !Activated ()) {
				animState = anim.Update (animState, "close");
			}
		}

		static public void InitAnimation() {
			anim.AddAnimation ("closed", 0, 0, 100, 100,1);
			anim.AddAnimation ("opening", 0, 0, 100, 100,1);
			anim.AddAnimation ("open", 100, 0, 100, 100,1);
			anim.AddAnimation ("closing", 100, 0, 100, 100,1);

			anim.AddStateChange ("closed", "open", "opening", true);
			anim.AddStateChange ("open", "close", "closing", true);
			anim.AddStateChange ("opening", "anim_end", "open", true);
			anim.AddStateChange ("closing", "anim_end", "closed", true);
		}
	}
}

