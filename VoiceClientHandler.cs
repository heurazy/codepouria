using System;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class VoiceClientHandler : MonoBehaviour
{
	// Token: 0x06000AAB RID: 2731 RVA: 0x00033E3C File Offset: 0x0003203C
	private void Awake()
	{
		PunVoiceClient component = base.GetComponent<PunVoiceClient>();
		if (component == null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (PunVoiceClient.Instance != component)
		{
			Debug.Log("Already Found VoiceClient, Destroying...");
			Object.Destroy(base.gameObject);
			return;
		}
		base.transform.SetParent(null);
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x00033EA0 File Offset: 0x000320A0
	private void Start()
	{
		VoiceClientHandler.m_VoiceConnection = base.GetComponent<VoiceConnection>();
		if (VoiceClientHandler.m_VoiceConnection.Client.State != ClientState.Joined)
		{
			VoiceClientHandler.m_VoiceConnection.Client.StateChanged += this.OnStateChanged;
			return;
		}
		VoiceClientHandler.InitNetworkVoice();
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x00033EEC File Offset: 0x000320EC
	private void OnStateChanged(ClientState state, ClientState toState)
	{
		if (toState == ClientState.Joined)
		{
			VoiceClientHandler.InitNetworkVoice();
		}
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x00033EF8 File Offset: 0x000320F8
	public static void InitNetworkVoice()
	{
		if (VoiceClientHandler.m_LocalRecorder == null || VoiceClientHandler.m_VoiceConnection == null || VoiceClientHandler.m_VoiceConnection.Client.State != ClientState.Joined)
		{
			return;
		}
		VoiceClientHandler.m_VoiceConnection.Client.LoadBalancingPeer.OpChangeGroups(Array.Empty<byte>(), Array.Empty<byte>());
		VoiceClientHandler.m_LocalRecorder.InterestGroup = 0;
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x00033F5D File Offset: 0x0003215D
	public static void LocalPlayerAssigned(Recorder r)
	{
		VoiceClientHandler.m_LocalRecorder = r;
		VoiceClientHandler.InitNetworkVoice();
	}

	// Token: 0x04000987 RID: 2439
	private static VoiceConnection m_VoiceConnection;

	// Token: 0x04000988 RID: 2440
	private static Recorder m_LocalRecorder;
}
