using System;
using System.Text;
using AOT;
using Steamworks;
using UnityEngine;

// Token: 0x02000143 RID: 323
[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
	// Token: 0x17000074 RID: 116
	// (get) Token: 0x0600093B RID: 2363 RVA: 0x0002EC23 File Offset: 0x0002CE23
	public static SteamManager Instance
	{
		get
		{
			return SteamManager.s_instance;
		}
	}

	// Token: 0x17000075 RID: 117
	// (get) Token: 0x0600093C RID: 2364 RVA: 0x0002EC2A File Offset: 0x0002CE2A
	public static bool Initialized
	{
		get
		{
			return SteamManager.Instance.m_bInitialized;
		}
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x0002EC36 File Offset: 0x0002CE36
	[MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
	protected static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText);
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x0002EC3E File Offset: 0x0002CE3E
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void InitOnPlayMode()
	{
		SteamManager.s_EverInitialized = false;
		SteamManager.s_instance = null;
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x0002EC4C File Offset: 0x0002CE4C
	protected virtual void Awake()
	{
		if (SteamManager.s_instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		SteamManager.s_instance = this;
		if (SteamManager.s_EverInitialized)
		{
			throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
		}
		Object.DontDestroyOnLoad(base.gameObject);
		if (!Packsize.Test())
		{
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}
		if (!DllCheck.Test())
		{
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		try
		{
			if (SteamAPI.RestartAppIfNecessary(new AppId_t(3527290U)))
			{
				Debug.Log("[Steamworks.NET] Shutting down because RestartAppIfNecessary returned true. Steam will restart the application.");
				Application.Quit();
				return;
			}
		}
		catch (DllNotFoundException ex)
		{
			string text = "[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n";
			DllNotFoundException ex2 = ex;
			Debug.LogError(text + ((ex2 != null) ? ex2.ToString() : null), this);
			Application.Quit();
			return;
		}
		this.m_bInitialized = SteamAPI.Init();
		if (!this.m_bInitialized)
		{
			Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
			return;
		}
		SteamManager.s_EverInitialized = true;
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x0002ED38 File Offset: 0x0002CF38
	protected virtual void OnEnable()
	{
		if (SteamManager.s_instance == null)
		{
			SteamManager.s_instance = this;
		}
		if (!this.m_bInitialized)
		{
			return;
		}
		if (this.m_SteamAPIWarningMessageHook == null)
		{
			this.m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamManager.SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(this.m_SteamAPIWarningMessageHook);
		}
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x0002ED86 File Offset: 0x0002CF86
	protected virtual void OnDestroy()
	{
		if (SteamManager.s_instance != this)
		{
			return;
		}
		SteamManager.s_instance = null;
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.Shutdown();
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x0002EDAA File Offset: 0x0002CFAA
	protected virtual void Update()
	{
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.RunCallbacks();
	}

	// Token: 0x04000838 RID: 2104
	protected static bool s_EverInitialized;

	// Token: 0x04000839 RID: 2105
	protected static SteamManager s_instance;

	// Token: 0x0400083A RID: 2106
	protected bool m_bInitialized;

	// Token: 0x0400083B RID: 2107
	protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
}
