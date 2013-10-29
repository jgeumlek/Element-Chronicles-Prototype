using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
namespace EC_Proto
{
	public class AnimationManager
	{
		Dictionary<String,Rectangle[]> Frames = new Dictionary<String, Rectangle[]>();
		Dictionary<AnimationTransitionSignal, AnimationTransitionResponse> StateChanges = new Dictionary<AnimationTransitionSignal, AnimationTransitionResponse>();

		public AnimationManager ()
		{
			Rectangle[] dummy = new Rectangle[1];
			dummy [0] = new Rectangle (0, 0, 0, 0);
			Frames.Add ("", dummy);
		}
		public void AddFrame (String name, int x, int y, int width, int height) {
			Rectangle[] rect = new Rectangle[1];
			rect [0] = new Rectangle (x, y, width, height);
			Frames.Add (name, rect);
		}
		//Assumes animations are in rows.
		public void AddAnimation(String name, int x, int y, int width, int height, int numFrames) {
			Rectangle[] rects = new Rectangle[numFrames]; 
			//TODO: Handle wrapping around image boundary.
			for (int i = 0; i < numFrames; i++) {
				rects [i] = new Rectangle (x + i * width, y, width, height);
			}
			Frames.Add (name, rects);
		}

		/// <summary>
		/// Adds the state change.
		/// </summary>
		/// <param name="sourceAnimation">Source animation. If empty string, the animation change applies to all states.</param>
		/// <param name="signal">Signal.</param>
		/// <param name="destAnimation">Destination animation.</param>
		/// <param name="resetCount">If set to <c>true</c> reset count.</param>
		public void AddStateChange(String sourceAnimation, String signal, String destAnimation, bool resetCount) {
			AnimationTransitionSignal source = new AnimationTransitionSignal (sourceAnimation, signal);
			AnimationTransitionResponse response = new AnimationTransitionResponse (destAnimation, resetCount);
			StateChanges.Add (source, response);

		}

		public Rectangle GetRectangle(AnimationState state) {
			Rectangle[] rects = Frames [state.AnimationName];
			int frameNum = state.CurrentFrame % rects.Length;
			return rects [frameNum];

		}

		public AnimationState Update(AnimationState currentState, String signal) {

			AnimationTransitionSignal specificTransition = new AnimationTransitionSignal (currentState.AnimationName, signal);
			AnimationTransitionSignal generalTransition = new AnimationTransitionSignal ("", signal);

			if (StateChanges.ContainsKey (specificTransition)) {
				AnimationTransitionResponse response = StateChanges [specificTransition];
				currentState.AnimationName = response.DestAnim;
				if (response.ResetFrameNumber) {
					currentState.CurrentFrame = 0;
				}
			} else if (StateChanges.ContainsKey (generalTransition)) {
				AnimationTransitionResponse response = StateChanges [generalTransition];
				currentState.AnimationName = response.DestAnim;
				if (response.ResetFrameNumber) {
					currentState.CurrentFrame = 0;
				}
			}

			return currentState;
		}

		//increments one frame, and handles animation ending signals. Note that all animations loop by default.
		public AnimationState Tick(AnimationState currentState) {
			currentState.CurrentFrame++;
			if (!Frames.ContainsKey (currentState.AnimationName)) { //Shouldn't happen, let's fail gracefully.
				//TODO: Log this?
				return currentState;
			}

			if (currentState.CurrentFrame == Frames[currentState.AnimationName].Length) { //The current animation just ended and looped!
				currentState.CurrentFrame = 0;
				currentState = Update (currentState, "anim_end"); //Could probably be optimized. Perhaps the current animation stores a next_anim? If none, stop. If it has one, switch. Looping would switching to self.
			}
			return currentState;
		}
	}



	class AnimationTransitionSignal {
		public String sourceAnim;
		public String signal;
		public AnimationTransitionSignal(String sourceAnim, String signal) {
			this.sourceAnim = sourceAnim;
			this.signal = signal;
		
		}

		public override int GetHashCode ()
		{
			return (sourceAnim + signal).GetHashCode ();
		}

		public override bool Equals (object obj)
		{
			if (obj is AnimationTransitionSignal) {
				AnimationTransitionSignal other = (AnimationTransitionSignal)obj;
				return (sourceAnim.Equals (other.sourceAnim) && signal.Equals (other.signal));
			}

			return false;
		}

	}

	class AnimationTransitionResponse {
		public String DestAnim;
		public bool ResetFrameNumber;
		public AnimationTransitionResponse(String destAnim, bool resetFrameNumber) {
			DestAnim = destAnim;
			ResetFrameNumber = resetFrameNumber;

		}


	}

}

