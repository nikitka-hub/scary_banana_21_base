using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	public static class PhotonDemoExtensions
	{
		internal const string MUTED_KEY = "mu";

		internal const string PHOTON_VAD_KEY = "pv";

		internal const string WEBRTC_AEC_KEY = "ec";

		internal const string WEBRTC_VAD_KEY = "wv";

		internal const string WEBRTC_AGC_KEY = "gc";

		internal const string MIC_KEY = "m";

		public static bool Mute(this Player player)
		{
			return player.SetCustomProperties(new Hashtable(1) { { "mu", true } });
		}

		public static bool Unmute(this Player player)
		{
			return player.SetCustomProperties(new Hashtable(1) { { "mu", false } });
		}

		public static bool IsMuted(this Player player)
		{
			return player.HasBoolProperty("mu");
		}

		public static bool SetPhotonVAD(this Player player, bool value)
		{
			return player.SetCustomProperties(new Hashtable(1) { { "pv", value } });
		}

		public static bool SetWebRTCVAD(this Player player, bool value)
		{
			return player.SetCustomProperties(new Hashtable(1) { { "wv", value } });
		}

		public static bool SetAEC(this Player player, bool value)
		{
			return player.SetCustomProperties(new Hashtable(1) { { "ec", value } });
		}

		public static bool SetAGC(this Player player, bool agcEnabled, int gain, int level)
		{
			return player.SetCustomProperties(new Hashtable(1) { 
			{
				"gc",
				new object[3] { agcEnabled, gain, level }
			} });
		}

		public static bool SetMic(this Player player, Recorder.MicType type)
		{
			return player.SetCustomProperties(new Hashtable(1) { { "m", type } });
		}

		public static bool HasPhotonVAD(this Player player)
		{
			return player.HasBoolProperty("pv");
		}

		public static bool HasWebRTCVAD(this Player player)
		{
			return player.HasBoolProperty("wv");
		}

		public static bool HasAEC(this Player player)
		{
			return player.HasBoolProperty("ec");
		}

		public static bool HasAGC(this Player player)
		{
			if (!(player.GetObjectProperty("gc") is object[] array) || array.Length == 0)
			{
				return false;
			}
			return (bool)array[0];
		}

		public static int GetAGCGain(this Player player)
		{
			if (!(player.GetObjectProperty("gc") is object[] array) || array.Length <= 1)
			{
				return 0;
			}
			return (int)array[1];
		}

		public static int GetAGCLevel(this Player player)
		{
			if (!(player.GetObjectProperty("gc") is object[] array) || array.Length <= 2)
			{
				return 0;
			}
			return (int)array[2];
		}

		public static Recorder.MicType? GetMic(this Player player)
		{
			Recorder.MicType? micType = null;
			try
			{
				return (Recorder.MicType)player.GetObjectProperty("m");
			}
			catch
			{
				micType = null;
			}
			return micType;
		}

		private static bool HasBoolProperty(this Player player, string prop)
		{
			if (player.CustomProperties.TryGetValue(prop, out var value))
			{
				return (bool)value;
			}
			return false;
		}

		private static int? GetIntProperty(this Player player, string prop)
		{
			if (player.CustomProperties.TryGetValue(prop, out var value))
			{
				return (int)value;
			}
			return null;
		}

		private static object GetObjectProperty(this Player player, string prop)
		{
			if (player.CustomProperties.TryGetValue(prop, out var value))
			{
				return value;
			}
			return null;
		}
	}
}
