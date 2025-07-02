using System;
using UnityEngine;

// Token: 0x02000090 RID: 144
public static class GlobalEvents
{
	// Token: 0x0600050B RID: 1291 RVA: 0x0001CC78 File Offset: 0x0001AE78
	public static void TriggerItemRequested(Item interactor, Character character)
	{
		try
		{
			if (GlobalEvents.OnItemRequested != null)
			{
				GlobalEvents.OnItemRequested(interactor, character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x0001CCB4 File Offset: 0x0001AEB4
	public static void TriggerItemConsumed(Item interactor, Character character)
	{
		try
		{
			if (GlobalEvents.OnItemConsumed != null)
			{
				GlobalEvents.OnItemConsumed(interactor, character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x0001CCF0 File Offset: 0x0001AEF0
	public static void TriggerRespawnChestOpened(RespawnChest chest, Character character)
	{
		try
		{
			if (GlobalEvents.OnRespawnChestOpened != null)
			{
				GlobalEvents.OnRespawnChestOpened(chest, character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x0001CD2C File Offset: 0x0001AF2C
	public static void TriggerLuggageOpened(Luggage luggage, Character character)
	{
		try
		{
			if (GlobalEvents.OnLuggageOpened != null)
			{
				GlobalEvents.OnLuggageOpened(luggage, character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0001CD68 File Offset: 0x0001AF68
	public static void TriggerLocalCharacterWonRun()
	{
		try
		{
			if (GlobalEvents.OnLocalCharacterWonRun != null)
			{
				GlobalEvents.OnLocalCharacterWonRun();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x0001CDA0 File Offset: 0x0001AFA0
	public static void TriggerSomeoneWonRun()
	{
		try
		{
			if (GlobalEvents.OnSomeoneWonRun != null)
			{
				GlobalEvents.OnSomeoneWonRun();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x0001CDD8 File Offset: 0x0001AFD8
	public static void TriggerCharacterPassedOut(Character character)
	{
		try
		{
			if (GlobalEvents.OnCharacterPassedOut != null)
			{
				GlobalEvents.OnCharacterPassedOut(character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x0001CE10 File Offset: 0x0001B010
	public static void TriggerRunEnded()
	{
		try
		{
			if (GlobalEvents.OnRunEnded != null)
			{
				GlobalEvents.OnRunEnded();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000513 RID: 1299 RVA: 0x0001CE48 File Offset: 0x0001B048
	public static void TriggerBugleTooted(Item bugle)
	{
		try
		{
			if (GlobalEvents.OnBugleTooted != null)
			{
				GlobalEvents.OnBugleTooted(bugle);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000514 RID: 1300 RVA: 0x0001CE80 File Offset: 0x0001B080
	public static void TriggerCharacterSpawned(Character character)
	{
		try
		{
			if (GlobalEvents.OnCharacterSpawned != null)
			{
				GlobalEvents.OnCharacterSpawned(character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000515 RID: 1301 RVA: 0x0001CEB8 File Offset: 0x0001B0B8
	public static void TriggerCharacterDestroyed(Character character)
	{
		try
		{
			if (GlobalEvents.OnCharacterOwnerDisconnected != null)
			{
				GlobalEvents.OnCharacterOwnerDisconnected(character);
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000516 RID: 1302 RVA: 0x0001CEF0 File Offset: 0x0001B0F0
	public static void TriggerCharacterAudioLevelsUpdated()
	{
		try
		{
			if (GlobalEvents.OnCharacterAudioLevelsUpdated != null)
			{
				GlobalEvents.OnCharacterAudioLevelsUpdated();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x0400052C RID: 1324
	public static Action<Item, Character> OnItemRequested;

	// Token: 0x0400052D RID: 1325
	public static Action<Item, Character> OnItemConsumed;

	// Token: 0x0400052E RID: 1326
	public static Action<RespawnChest, Character> OnRespawnChestOpened;

	// Token: 0x0400052F RID: 1327
	public static Action<Luggage, Character> OnLuggageOpened;

	// Token: 0x04000530 RID: 1328
	public static Action OnLocalCharacterWonRun;

	// Token: 0x04000531 RID: 1329
	public static Action OnSomeoneWonRun;

	// Token: 0x04000532 RID: 1330
	public static Action<Character> OnCharacterPassedOut;

	// Token: 0x04000533 RID: 1331
	public static Action OnRunEnded;

	// Token: 0x04000534 RID: 1332
	public static Action<Item> OnBugleTooted;

	// Token: 0x04000535 RID: 1333
	public static Action<Character> OnCharacterSpawned;

	// Token: 0x04000536 RID: 1334
	public static Action<Character> OnCharacterOwnerDisconnected;

	// Token: 0x04000537 RID: 1335
	public static Action OnCharacterAudioLevelsUpdated;
}
