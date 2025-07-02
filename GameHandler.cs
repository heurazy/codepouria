using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zorro.ControllerSupport;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000063 RID: 99
[DefaultExecutionOrder(-100)]
public class GameHandler : MonoBehaviour
{
	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060003D7 RID: 983 RVA: 0x00016BB9 File Offset: 0x00014DB9
	public static GameHandler Instance
	{
		get
		{
			return GameHandler._instance;
		}
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060003D8 RID: 984 RVA: 0x00016BC0 File Offset: 0x00014DC0
	// (set) Token: 0x060003D9 RID: 985 RVA: 0x00016BC8 File Offset: 0x00014DC8
	public SettingsHandler SettingsHandler { get; private set; }

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060003DA RID: 986 RVA: 0x00016BD1 File Offset: 0x00014DD1
	public static bool Initialized
	{
		get
		{
			return GameHandler.Instance != null && GameHandler.Instance.m_initialized;
		}
	}

	// Token: 0x060003DB RID: 987 RVA: 0x00016BEC File Offset: 0x00014DEC
	public void Initialize()
	{
		Debug.Log("Game Handler Initialized");
		GameHandler._instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060003DC RID: 988 RVA: 0x00016C09 File Offset: 0x00014E09
	private void OnDestroy()
	{
		Debug.Log("Game Handler Destroying...");
	}

	// Token: 0x060003DD RID: 989 RVA: 0x00016C18 File Offset: 0x00014E18
	private async void Awake()
	{
		this.m_gameStatus = new Dictionary<Type, GameStatus>();
		this.m_gameServices = new Dictionary<Type, object>();
		List<ConsoleCommand> list = ConsoleHandler.ScanForConsoleCommands();
		Dictionary<Type, CLITypeParser> dictionary = ConsoleHandler.ScanForTypeParsers();
		CustomTypeRPCSerialization.Initialize();
		ConsoleHandler.Initialize(list, dictionary);
		RetrievableResourceSingleton<InputHandler>.Instance.Initialize(() => false, () => !DebugUIHandler.IsOpen);
		NetworkStats networkStats = base.gameObject.AddComponent<NetworkStats>();
		this.RegisterService<PlayerHandler>(new PlayerHandler());
		this.RegisterService<ConnectionService>(new ConnectionService());
		this.RegisterService<SteamLobbyHandler>(new SteamLobbyHandler());
		this.RegisterService<PersistentPlayerDataService>(new PersistentPlayerDataService());
		this.RegisterService<NextLevelService>(new NextLevelService());
		Singleton<DebugUIHandler>.Instance.RegisterPage("Network Stats", () => new NetworkStatsPage(networkStats));
		Singleton<DebugUIHandler>.Instance.RegisterPage("Item Instance Datas", () => new ItemInstanceDataDebugPage());
		base.gameObject.AddComponent<SteamManager>();
		Debug.Log("Added SteamManager");
		this.SettingsHandler = new SettingsHandler();
		this.m_initialized = true;
	}

	// Token: 0x060003DE RID: 990 RVA: 0x00016C50 File Offset: 0x00014E50
	private void RegisterService<T>(T service) where T : GameService<T>
	{
		Type type = service.GetType();
		this.m_gameServices[type] = service;
	}

	// Token: 0x060003DF RID: 991 RVA: 0x00016C7B File Offset: 0x00014E7B
	public static T GetService<T>() where T : GameService<T>
	{
		return GameHandler.Instance.m_gameServices[typeof(T)] as T;
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00016CA0 File Offset: 0x00014EA0
	public static async Awaitable WaitForInitialization()
	{
		while (!GameHandler.Instance.m_initialized)
		{
			await Awaitable.NextFrameAsync(default(CancellationToken));
		}
	}

	// Token: 0x060003E1 RID: 993 RVA: 0x00016CDC File Offset: 0x00014EDC
	public static T RestartService<T>(T service) where T : GameService<T>, IDisposable
	{
		Type type = service.GetType();
		if (GameHandler.Instance.m_gameServices.ContainsKey(type))
		{
			((T)((object)GameHandler.Instance.m_gameServices[type])).Dispose();
		}
		GameHandler.Instance.m_gameServices[type] = service;
		return service;
	}

	// Token: 0x060003E2 RID: 994 RVA: 0x00016D40 File Offset: 0x00014F40
	public static void AddStatus<T>(GameStatus status) where T : GameStatus
	{
		Type type = status.GetType();
		GameHandler.Instance.m_gameStatus[type] = status;
		Debug.Log(string.Format("Add status: {0}", type));
	}

	// Token: 0x060003E3 RID: 995 RVA: 0x00016D78 File Offset: 0x00014F78
	public static bool TryGetStatus<T>(out T status) where T : GameStatus
	{
		Type typeFromHandle = typeof(T);
		GameStatus gameStatus;
		bool flag = GameHandler.Instance.m_gameStatus.TryGetValue(typeFromHandle, out gameStatus);
		status = default(T);
		if (flag)
		{
			status = gameStatus as T;
		}
		return flag;
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x00016DC0 File Offset: 0x00014FC0
	public static void ClearStatus<T>() where T : GameStatus
	{
		Type typeFromHandle = typeof(T);
		if (GameHandler.Instance.m_gameStatus.ContainsKey(typeFromHandle))
		{
			GameHandler.Instance.m_gameStatus.Remove(typeFromHandle);
			Debug.Log(string.Format("Clear status: {0}", typeFromHandle));
		}
	}

	// Token: 0x060003E5 RID: 997 RVA: 0x00016E0B File Offset: 0x0001500B
	public static void ClearAllStatuses()
	{
		GameHandler.Instance.m_gameStatus.Clear();
		Debug.Log("Clearing all statuses!");
	}

	// Token: 0x060003E6 RID: 998 RVA: 0x00016E26 File Offset: 0x00015026
	private void Update()
	{
		this.SettingsHandler.Update();
		Debug.ClearDeveloperConsole();
	}

	// Token: 0x04000442 RID: 1090
	private static GameHandler _instance;

	// Token: 0x04000443 RID: 1091
	private Dictionary<Type, object> m_gameServices;

	// Token: 0x04000445 RID: 1093
	private bool m_initialized;

	// Token: 0x04000446 RID: 1094
	private Dictionary<Type, GameStatus> m_gameStatus;
}
