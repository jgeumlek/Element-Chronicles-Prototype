using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


/// <summary>
/// TODO Find out why overlays aren't disappearing.
/// </summary>

namespace EC_Proto
{
	public class SpellManager
	{

		public static string activeSpell = "";
		private static TimeSpan drainTimer = new TimeSpan(0,0,1);

		public static Dictionary<string,bool> spells = new Dictionary<string,bool> 
		{
			{"fireball",true},
			{"frostbreath",false},
			{"earthen shield",false},
			{"windwalk",true}
		};

		public static void Update (GameTime gameTime, KeyboardState state, KeyboardState prevState) {
			if (!GameScene.player.projectileLaunched) {
				if (spells ["fireball"] && state.IsKeyDown (Keys.H) && prevState.IsKeyUp (Keys.H) && GameScene.player.HasEnoughMana (2)) {
					GameScene.player.Fireball ();
				} else if (spells ["frostbreath"] && state.IsKeyDown (Keys.J) && prevState.IsKeyUp (Keys.J) && GameScene.player.HasEnoughMana (1)) {
					GameScene.player.FrostBreath ();
				}
			}

			if (spells ["earthen shield"]) {
				if (state.IsKeyDown (Keys.K) && prevState.IsKeyUp (Keys.K)) {
					if (activeSpell == "") {
						GameScene.player.EarthenShield ();
					} else {
						GameScene.player.DestroyOverlay ();
						if (activeSpell != "earthen shield") {
							if (GameScene.player.HasEnoughMana (1)) {
								GameScene.player.EarthenShield ();
							}
						} else {
							activeSpell = "";
						}
					}
				}
			}
			if (spells ["windwalk"]) {
				if (state.IsKeyDown (Keys.L) && prevState.IsKeyUp (Keys.L)) {
					if (activeSpell == "") {
						GameScene.player.WindWalk ();
					} else {
						GameScene.player.DestroyOverlay ();
						if (activeSpell != "windwalk") {
							if (GameScene.player.HasEnoughMana (1)) {
								GameScene.player.WindWalk ();
							}
						} else {
							activeSpell = "";
						}
					}
				}
			}
			if (activeSpell != "") {
				drainTimer -= gameTime.ElapsedGameTime;
				if (drainTimer.Milliseconds <= 0) {
					DrainMana ();
				}
			}
		}

		private static void DrainMana () {
			GameScene.player.ConsumeMana (1);
			if (!GameScene.player.HasEnoughMana (1)) {
				GameScene.player.DestroyOverlay ();
				activeSpell = "";
			}
			drainTimer = new TimeSpan (0, 0, 1);
		}
	}
}

