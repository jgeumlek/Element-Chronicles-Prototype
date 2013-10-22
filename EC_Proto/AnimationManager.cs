using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
namespace EC_Proto
{
	public class AnimationManager
	{
		Dictionary<String,Rectangle[]> frames = new Dictionary<String, Rectangle[]>();
		int currentFrame = 0;
		String currentAnim = "";
		public AnimationManager ()
		{
			Rectangle[] dummy = new Rectangle[1];
			dummy [0] = new Rectangle (0, 0, 0, 0);
			frames.Add ("", dummy);
		}
		public void AddFrame (String name, int x, int y, int width, int height) {
			Rectangle[] rect = new Rectangle[1];
			rect [0] = new Rectangle (x, y, width, height);
			frames.Add (name, rect);
		}
		//Assumes animations are in rows.
		public void AddAnimation(String name, int x, int y, int width, int height, int numFrames) {
			Rectangle[] rects = new Rectangle[numFrames]; 
			//TODO: Handle wrapping around image boundary.
			for (int i = 0; i < numFrames; i++) {
				rects [i] = new Rectangle (x + i * width, y, width, height);
			}
			frames.Add (name, rects);
		}

		public void StartAnimation (String name) {
			currentAnim = name;
			currentFrame = 0;
		}

		public void SwitchAnimation(String name) {
			if (!frames.ContainsKey (name)) {
			}
			currentAnim = name;
		}

		public Rectangle GetRectangle() {
			Rectangle[] rects = frames [currentAnim];
			int frameNum = currentFrame % rects.Length;
			return rects [frameNum];
		}
	}
}

