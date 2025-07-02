using System;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x020002C2 RID: 706
	public static class PhotonDemoExtensions
	{
		// Token: 0x06001152 RID: 4434 RVA: 0x00055D9A File Offset: 0x00053F9A
		public static bool Mute(this Photon.Realtime.Player player)
		{
			return player.SetCustomProperties(new Hashtable(1) { { "mu", true } }, null, null);
		}

		// Token: 0x06001153 RID: 4435 RVA: 0x00055DBB File Offset: 0x00053FBB
		public static bool Unmute(this Photon.Realtime.Player player)
		{
			return player.SetCustomProperties(new Hashtable(1) { { "mu", false } }, null, null);
		}

		// Token: 0x06001154 RID: 4436 RVA: 0x00055DDC File Offset: 0x00053FDC
		public static bool IsMuted(this Photon.Realtime.Player player)
		{
			return player.HasBoolProperty("mu");
		}

		// Token: 0x06001155 RID: 4437 RVA: 0x00055DE9 File Offset: 0x00053FE9
		public static bool SetPhotonVAD(this Photon.Realtime.Player player, bool value)
		{
			return player.SetCustomProperties(new Hashtable(1) { { "pv", value } }, null, null);
		}

		// Token: 0x06001156 RID: 4438 RVA: 0x00055E0A File Offset: 0x0005400A
		public static bool SetWebRTCVAD(this Photon.Realtime.Player player, bool value)
		{
			return player.SetCustomProperties(new Hashtable(1) { { "wv", value } }, null, null);
		}

		// Token: 0x06001157 RID: 4439 RVA: 0x00055E2B File Offset: 0x0005402B
		public static bool SetAEC(this Photon.Realtime.Player player, bool value)
		{
			return player.SetCustomProperties(new Hashtable(1) { { "ec", value } }, null, null);
		}

		// Token: 0x06001158 RID: 4440 RVA: 0x00055E4C File Offset: 0x0005404C
		public static bool SetAGC(this Photon.Realtime.Player player, bool agcEnabled, int gain, int level)
		{
			return player.SetCustomProperties(new Hashtable(1) { 
			{
				"gc",
				new object[] { agcEnabled, gain, level }
			} }, null, null);
		}

		// Token: 0x06001159 RID: 4441 RVA: 0x00055E95 File Offset: 0x00054095
		public static bool SetMic(this Photon.Realtime.Player player, Recorder.MicType type)
		{
			return player.SetCustomProperties(new Hashtable(1) { { "m", type } }, null, null);
		}

		// Token: 0x0600115A RID: 4442 RVA: 0x00055EB6 File Offset: 0x000540B6
		public static bool HasPhotonVAD(this Photon.Realtime.Player player)
		{
			return player.HasBoolProperty("pv");
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x00055EC3 File Offset: 0x000540C3
		public static bool HasWebRTCVAD(this Photon.Realtime.Player player)
		{
			return player.HasBoolProperty("wv");
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x00055ED0 File Offset: 0x000540D0
		public static bool HasAEC(this Photon.Realtime.Player player)
		{
			return player.HasBoolProperty("ec");
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x00055EE0 File Offset: 0x000540E0
		public static bool HasAGC(this Photon.Realtime.Player player)
		{
			object[] array = player.GetObjectProperty("gc") as object[];
			return array != null && array.Length != 0 && (bool)array[0];
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x00055F10 File Offset: 0x00054110
		public static int GetAGCGain(this Photon.Realtime.Player player)
		{
			object[] array = player.GetObjectProperty("gc") as object[];
			if (array == null || array.Length <= 1)
			{
				return 0;
			}
			return (int)array[1];
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x00055F44 File Offset: 0x00054144
		public static int GetAGCLevel(this Photon.Realtime.Player player)
		{
			object[] array = player.GetObjectProperty("gc") as object[];
			if (array == null || array.Length <= 2)
			{
				return 0;
			}
			return (int)array[2];
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x00055F78 File Offset: 0x00054178
		public static Recorder.MicType? GetMic(this Photon.Realtime.Player player)
		{
			Recorder.MicType? micType = null;
			try
			{
				micType = new Recorder.MicType?((Recorder.MicType)player.GetObjectProperty("m"));
			}
			catch
			{
				micType = null;
			}
			return micType;
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x00055FC4 File Offset: 0x000541C4
		private static bool HasBoolProperty(this Photon.Realtime.Player player, string prop)
		{
			object obj;
			return player.CustomProperties.TryGetValue(prop, out obj) && (bool)obj;
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x00055FEC File Offset: 0x000541EC
		private static int? GetIntProperty(this Photon.Realtime.Player player, string prop)
		{
			object obj;
			if (player.CustomProperties.TryGetValue(prop, out obj))
			{
				return new int?((int)obj);
			}
			return null;
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x00056020 File Offset: 0x00054220
		private static object GetObjectProperty(this Photon.Realtime.Player player, string prop)
		{
			object obj;
			if (player.CustomProperties.TryGetValue(prop, out obj))
			{
				return obj;
			}
			return null;
		}

		// Token: 0x04000FE1 RID: 4065
		internal const string MUTED_KEY = "mu";

		// Token: 0x04000FE2 RID: 4066
		internal const string PHOTON_VAD_KEY = "pv";

		// Token: 0x04000FE3 RID: 4067
		internal const string WEBRTC_AEC_KEY = "ec";

		// Token: 0x04000FE4 RID: 4068
		internal const string WEBRTC_VAD_KEY = "wv";

		// Token: 0x04000FE5 RID: 4069
		internal const string WEBRTC_AGC_KEY = "gc";

		// Token: 0x04000FE6 RID: 4070
		internal const string MIC_KEY = "m";
	}
}
