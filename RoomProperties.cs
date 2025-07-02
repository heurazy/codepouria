using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using Zorro.Core.CLI;

// Token: 0x020000FD RID: 253
public class RoomProperties : OnNetworkStart
{
	// Token: 0x0600077B RID: 1915 RVA: 0x00027F7E File Offset: 0x0002617E
	public void Awake()
	{
		RoomProperties.me = this;
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00027F88 File Offset: 0x00026188
	public bool IsReconnecting()
	{
		Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
		bool flag = customProperties.ContainsKey(this.DUI) && (bool)customProperties[this.DUI];
		Debug.Log("Reconnecting? " + flag.ToString());
		customProperties[this.DUI] = true;
		customProperties[PhotonNetwork.LocalPlayer.UserId] = this.DUI;
		PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties, null, null);
		return flag;
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x00028010 File Offset: 0x00026210
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			this.ttPositionLog -= Time.deltaTime;
			if (this.ttPositionLog <= 0f)
			{
				this.SaveReconnectData();
				this.ttPositionLog = 1f;
			}
		}
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00028064 File Offset: 0x00026264
	public bool GetReconnectPosition(out Vector3 position)
	{
		position = Vector3.zero;
		Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
		if (!this.HasReconnected(customProperties))
		{
			return false;
		}
		ReconnectData reconnectData = ReconnectData.Deserialize(customProperties[this.ReconnectDataKey] as byte[]);
		position = reconnectData.position;
		return true;
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x000280B8 File Offset: 0x000262B8
	public void SaveReconnectData()
	{
		Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
		ReconnectData reconnectData = ReconnectData.CreateFromCharacter(Character.localCharacter);
		customProperties[this.ReconnectDataKey] = reconnectData.Serialize();
		PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties, null, null);
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x000280FC File Offset: 0x000262FC
	public bool IsLocallyReconnecting()
	{
		Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
		return customProperties[this.ReconnectDataKey] != null && this.HasReconnected(customProperties);
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x0002812C File Offset: 0x0002632C
	public void Reconnect()
	{
		RoomProperties.<>c__DisplayClass8_0 CS$<>8__locals1 = new RoomProperties.<>c__DisplayClass8_0();
		Debug.Log("Checking reconnect");
		Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
		object obj = customProperties[this.ReconnectDataKey];
		if (obj == null)
		{
			return;
		}
		CS$<>8__locals1.reconnectData = ReconnectData.Deserialize(obj as byte[]);
		if (!this.HasReconnected(customProperties))
		{
			return;
		}
		Debug.Log("Warping because reconnect...");
		base.StartCoroutine(CS$<>8__locals1.<Reconnect>g__warp|0());
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x00028197 File Offset: 0x00026397
	public bool HasReconnected(Hashtable properties)
	{
		return properties.ContainsKey(this.DUI) && ReconnectData.Deserialize(properties[this.DUI] as byte[]).isValid;
	}

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x06000783 RID: 1923 RVA: 0x000281C4 File Offset: 0x000263C4
	private string DUI
	{
		get
		{
			return SystemInfo.deviceUniqueIdentifier + RoomProperties.GetPlayerNumber();
		}
	}

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x06000784 RID: 1924 RVA: 0x000281D5 File Offset: 0x000263D5
	private string ReconnectDataKey
	{
		get
		{
			return this.DUI + "_ReconnectData";
		}
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x000281E8 File Offset: 0x000263E8
	[ConsoleCommand]
	public static void ReconnectPassedOut()
	{
		Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
		ReconnectData reconnectData = ReconnectData.CreateFromCharacter(Character.localCharacter);
		reconnectData.fullyPassedOut = true;
		reconnectData.deathTimer = 0.5f;
		customProperties[RoomProperties.me.ReconnectDataKey] = reconnectData.Serialize();
		PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties, null, null);
		RoomProperties.me.Reconnect();
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x00028250 File Offset: 0x00026450
	[ConsoleCommand]
	public static void ReconnectDead()
	{
		Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
		ReconnectData reconnectData = ReconnectData.CreateFromCharacter(Character.localCharacter);
		reconnectData.fullyPassedOut = true;
		reconnectData.deathTimer = 1f;
		reconnectData.dead = true;
		customProperties[RoomProperties.me.ReconnectDataKey] = reconnectData.Serialize();
		PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties, null, null);
		RoomProperties.me.Reconnect();
	}

	// Token: 0x06000787 RID: 1927 RVA: 0x000282C0 File Offset: 0x000264C0
	public void PrintData()
	{
		ReconnectData.Deserialize(PhotonNetwork.CurrentRoom.CustomProperties[this.ReconnectDataKey] as byte[]).PrintData();
	}

	// Token: 0x06000788 RID: 1928 RVA: 0x000282F4 File Offset: 0x000264F4
	public static string GetPlayerNumber()
	{
		return "";
	}

	// Token: 0x06000789 RID: 1929 RVA: 0x000282FB File Offset: 0x000264FB
	private void OnDestroy()
	{
		if (RoomProperties.me == this)
		{
			RoomProperties.me = null;
		}
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x00028310 File Offset: 0x00026510
	public override void NetworkStart()
	{
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x00028314 File Offset: 0x00026514
	public void Clear()
	{
		Hashtable customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
		object obj = customProperties[this.ReconnectDataKey];
		if (obj == null)
		{
			return;
		}
		ReconnectData reconnectData = ReconnectData.Deserialize(obj as byte[]);
		reconnectData.isValid = false;
		customProperties[this.ReconnectDataKey] = reconnectData.Serialize();
		PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties, null, null);
		Debug.Log("Clearing reconnect...");
	}

	// Token: 0x04000707 RID: 1799
	public static RoomProperties me;

	// Token: 0x04000708 RID: 1800
	private float ttPositionLog = 5f;
}
