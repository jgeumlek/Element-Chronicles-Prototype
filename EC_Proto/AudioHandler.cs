using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace EC_Proto
{
	public class AudioHandler
	{
		public static ContentManager content;
		public static SoundEffect bgm;
		public static SoundEffectInstance bgmInstance;

		public enum songs {
			title,
			main,
			water,
			fire,
			earth,
			air
		}

		public static int currentSong = -1;

		public static void Start() {
			StartNewTrack ("TitleSong.wav");
		}

		public static void SetScene (string sceneName) {
			if (sceneName == "game") {
				SwitchBgmTrack ("bgm.wav");
			} else if (sceneName == "logo") {
				if (currentSong == -1) {
					Start ();
				} else {
					SwitchBgmTrack ("TitleSong.wav");
				}
			}
		}

		public static void SetMap (string mapName) {
			if (mapName == "central_hub" && currentSong != (int)songs.main) {
				SwitchBgmTrack ("bgm.wav");
			} else if (mapName == "central_water" && currentSong != (int)songs.water) {
				SwitchBgmTrack ("WaterSong.wav");
			} else if (mapName == "central_earth" && currentSong != (int)songs.earth) {
				SwitchBgmTrack ("EarthSong.wav");
			} else if (mapName == "central_air" && currentSong != (int)songs.air) {
				SwitchBgmTrack ("AirZone.wav");
			} else if (mapName == "central_fire" && currentSong != (int)songs.fire) {
				SwitchBgmTrack ("FireSong.wav");
			}
		}

		public static void SwitchBgmTrack (string trackName) {
			StopCurrentTrack ();
			StartNewTrack (trackName);
		}

		public static void StopCurrentTrack () {
			bgmInstance.Stop ();
			bgmInstance.Dispose ();
		}

		public static void StartNewTrack (string trackName) {
			bgm = content.Load<SoundEffect> (trackName);
			bgmInstance = bgm.CreateInstance ();
			bgmInstance.IsLooped = true;
			bgmInstance.Play ();
			SetCurrentTrack (trackName);
		}

		public static void SetCurrentTrack (string trackName) {
			switch (trackName) {
			case "TitleSong.wav":
				currentSong = (int)songs.title;
				break;
			case "bgm.wav":
				currentSong = (int)songs.main;
				break;
			case "WaterSong.wav":
				currentSong = (int)songs.water;
				break;
			case "EarthSong.wav":
				currentSong = (int)songs.earth;
				break;
			case "AirZone.wav":
				currentSong = (int)songs.air;
				break;
			case "FireSong.wav":
				currentSong = (int)songs.air;
				break;
			}
		}
	}
}

