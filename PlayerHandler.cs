using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000109 RID: 265
public class PlayerHandler : GameService<PlayerHandler>, IDisposable
{
	// Token: 0x060007D1 RID: 2001 RVA: 0x000297D0 File Offset: 0x000279D0
	public static List<Character> GetAllPlayerCharacters()
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, Character> keyValuePair in GameService<PlayerHandler>.Instance.m_playerCharacterLookup)
		{
			Photon.Realtime.Player player;
			if (!PhotonNetwork.TryGetPlayer(keyValuePair.Key, out player))
			{
				list.Add(keyValuePair.Key);
			}
			else if (player.IsInactive)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (int num in list)
		{
			GameService<PlayerHandler>.Instance.m_playerCharacterLookup.Remove(num);
			Debug.Log(string.Format("Removing {0} character from list..", num));
		}
		return GameService<PlayerHandler>.Instance.m_playerCharacterLookup.Values.ToList<Character>();
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x000298D0 File Offset: 0x00027AD0
	public static void RegisterPlayer(global::Player player)
	{
		PhotonView component = player.GetComponent<PhotonView>();
		if (GameService<PlayerHandler>.Instance.m_playerLookup.ContainsKey(component.Owner.ActorNumber))
		{
			GameService<PlayerHandler>.Instance.m_playerLookup.Remove(component.Owner.ActorNumber);
			Debug.Log(string.Format("Overwriting player for {0}", component.Owner.ActorNumber));
		}
		GameService<PlayerHandler>.Instance.m_playerLookup.Add(component.Owner.ActorNumber, player);
		Debug.Log(string.Format("Registering Player object for {0} : {1}", component.Owner.NickName, component.Owner.ActorNumber));
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x00029980 File Offset: 0x00027B80
	public static void RegisterCharacter(Character character)
	{
		PhotonView component = character.GetComponent<PhotonView>();
		if (GameService<PlayerHandler>.Instance.m_playerCharacterLookup.ContainsKey(component.Owner.ActorNumber))
		{
			Debug.Log(string.Format("Overwriting character for {0}", component.Owner.ActorNumber));
			Character character2 = GameService<PlayerHandler>.Instance.m_playerCharacterLookup[component.Owner.ActorNumber];
			if (character2 != null)
			{
				character2.gameObject.SetActive(false);
				Debug.LogError("Disabled Old Player....");
			}
			GameService<PlayerHandler>.Instance.m_playerCharacterLookup.Remove(component.Owner.ActorNumber);
		}
		GameService<PlayerHandler>.Instance.m_playerCharacterLookup.Add(component.Owner.ActorNumber, character);
		Debug.Log(string.Format("Registering Character object for {0} : {1}", component.Owner.NickName, component.Owner.ActorNumber));
		Action<Character> onCharacterRegistered = PlayerHandler.OnCharacterRegistered;
		if (onCharacterRegistered == null)
		{
			return;
		}
		onCharacterRegistered(character);
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x00029A79 File Offset: 0x00027C79
	public static global::Player GetPlayer(Photon.Realtime.Player photonPlayer)
	{
		return GameService<PlayerHandler>.Instance.m_playerLookup.GetValueOrDefault(photonPlayer.ActorNumber);
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x00029A90 File Offset: 0x00027C90
	public static global::Player GetPlayer(int actorNumber)
	{
		return GameService<PlayerHandler>.Instance.m_playerLookup.GetValueOrDefault(actorNumber);
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x00029AA2 File Offset: 0x00027CA2
	public static bool TryGetPlayer(int actorNumber, out global::Player player)
	{
		player = PlayerHandler.GetPlayer(actorNumber);
		return player != null;
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x00029AB4 File Offset: 0x00027CB4
	public static Character GetPlayerCharacter(Photon.Realtime.Player photonPlayer)
	{
		return GameService<PlayerHandler>.Instance.m_playerCharacterLookup.GetValueOrDefault(photonPlayer.ActorNumber);
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x00029ACB File Offset: 0x00027CCB
	public static bool HasHadPlayerCharacter(Photon.Realtime.Player photonPlayer)
	{
		return GameService<PlayerHandler>.Instance.m_playerCharacterLookup.ContainsKey(photonPlayer.ActorNumber);
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x00029AE4 File Offset: 0x00027CE4
	public static byte AssignMixerGroup(Character character)
	{
		for (byte b = 0; b < 4; b += 1)
		{
			if (!GameService<PlayerHandler>.Instance.m_assignedVoiceGroups.ContainsKey(b) || !GameService<PlayerHandler>.Instance.m_assignedVoiceGroups[b].UnityObjectExists<Character>())
			{
				GameService<PlayerHandler>.Instance.m_assignedVoiceGroups[b] = character;
				return b;
			}
		}
		return byte.MaxValue;
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x00029B3F File Offset: 0x00027D3F
	public void Dispose()
	{
		Debug.Log("Disposing PlayerHandler");
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x00029B4C File Offset: 0x00027D4C
	public static List<global::Player> GetAllPlayers()
	{
		List<global::Player> list = new List<global::Player>();
		foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
		{
			global::Player player2;
			if (!player.IsInactive && PlayerHandler.TryGetPlayer(player.ActorNumber, out player2))
			{
				list.Add(player2);
			}
		}
		return list;
	}

	// Token: 0x04000756 RID: 1878
	private Dictionary<int, global::Player> m_playerLookup = new Dictionary<int, global::Player>();

	// Token: 0x04000757 RID: 1879
	private Dictionary<int, Character> m_playerCharacterLookup = new Dictionary<int, Character>();

	// Token: 0x04000758 RID: 1880
	private Dictionary<byte, Character> m_assignedVoiceGroups = new Dictionary<byte, Character>();

	// Token: 0x04000759 RID: 1881
	public static Action<Character> OnCharacterRegistered;
}
