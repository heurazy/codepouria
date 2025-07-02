using System;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class DefaultConnectionState : ConnectionState
{
	// Token: 0x060003D3 RID: 979 RVA: 0x00016B6F File Offset: 0x00014D6F
	public override void Enter()
	{
		base.Enter();
		if (Time.frameCount > 3)
		{
			GameHandler.GetService<SteamLobbyHandler>().LeaveLobby();
		}
	}
}
