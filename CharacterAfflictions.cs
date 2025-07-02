using System;
using System.Collections.Generic;
using Peak.Afflictions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Zorro.Core;
using Zorro.Core.CLI;
using Zorro.Core.Serizalization;

// Token: 0x02000008 RID: 8
[ConsoleClassCustomizer("Afflictions")]
public class CharacterAfflictions : MonoBehaviourPunCallbacks
{
	// Token: 0x0600009C RID: 156 RVA: 0x000051DC File Offset: 0x000033DC
	private void Awake()
	{
		this.character = base.GetComponent<Character>();
		this.InitStatusArrays();
		this.m_inAirport = SceneManager.GetActiveScene().name == "Airport";
	}

	// Token: 0x0600009D RID: 157 RVA: 0x00005218 File Offset: 0x00003418
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		this.PushStatuses(newPlayer);
	}

	// Token: 0x0600009E RID: 158 RVA: 0x00005228 File Offset: 0x00003428
	private void InitStatusArrays()
	{
		this.currentStatuses = new float[Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE)).Length];
		this.currentIncrementalStatuses = new float[this.currentStatuses.Length];
		this.currentDecrementalStatuses = new float[this.currentStatuses.Length];
		this.lastAddedStatus = new float[this.currentStatuses.Length];
		this.lastAddedIncrementalStatus = new float[this.currentStatuses.Length];
	}

	// Token: 0x0600009F RID: 159 RVA: 0x000052A0 File Offset: 0x000034A0
	private void Update()
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		for (int i = this.afflictionList.Count - 1; i >= 0; i--)
		{
			if (this.afflictionList[i].Tick())
			{
				this.character.refs.afflictions.RemoveAffliction(this.afflictionList[i], false);
			}
		}
		this.UpdateNormalStatuses();
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x00005310 File Offset: 0x00003510
	internal void UpdateWeight()
	{
		int num = 0;
		for (int i = 0; i < this.character.player.itemSlots.Length; i++)
		{
			ItemSlot itemSlot = this.character.player.itemSlots[i];
			if (itemSlot.prefab != null)
			{
				num += itemSlot.prefab.CarryWeight;
			}
		}
		BackpackSlot backpackSlot = this.character.player.backpackSlot;
		BackpackData backpackData;
		if (!backpackSlot.IsEmpty() && backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
		{
			for (int j = 0; j < backpackData.itemSlots.Length; j++)
			{
				ItemSlot itemSlot2 = backpackData.itemSlots[j];
				if (!itemSlot2.IsEmpty())
				{
					num += itemSlot2.prefab.CarryWeight;
				}
			}
		}
		if (this.character.data.carriedPlayer != null)
		{
			num += 8;
		}
		this.SetStatus(CharacterAfflictions.STATUSTYPE.Weight, 0.025f * (float)num);
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x000053FC File Offset: 0x000035FC
	private void UpdateNormalStatuses()
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		if (Ascents.isNightCold && Singleton<MountainProgressHandler>.Instance && Singleton<MountainProgressHandler>.Instance.maxProgressPointReached < 3 && DayNightManager.instance != null && DayNightManager.instance.isDay < 0.5f)
		{
			this.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, Time.deltaTime * (1f - DayNightManager.instance.isDay) * Ascents.nightColdRate, false);
		}
		if (this.character.data.fullyConscious)
		{
			this.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, Time.deltaTime * this.hungerPerSecond * Ascents.hungerRateMultiplier, false);
		}
		if (this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Poison) > 0f && Time.time - this.LastAddedStatus(CharacterAfflictions.STATUSTYPE.Poison) > this.poisonReductionCooldown)
		{
			this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Poison, this.poisonReductionPerSecond * Time.deltaTime, false);
		}
		if (this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Drowsy) > 0f && Time.time - this.LastAddedStatus(CharacterAfflictions.STATUSTYPE.Drowsy) > this.drowsyReductionCooldown)
		{
			this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Drowsy, this.drowsyReductionPerSecond * Time.deltaTime, false);
		}
		if (this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hot) > 0f && Time.time - this.LastAddedStatus(CharacterAfflictions.STATUSTYPE.Hot) > this.hotReductionCooldown)
		{
			this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hot, this.hotReductionPerSecond * Time.deltaTime, false);
		}
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x0000554C File Offset: 0x0000374C
	public void AddAffliction(Affliction affliction, bool fromRPC = false)
	{
		if (this.character.data.carriedPlayer)
		{
			return;
		}
		if (affliction == null)
		{
			Debug.LogError("Trying to add null affliction");
			return;
		}
		if (!this.character.IsLocal && !fromRPC)
		{
			return;
		}
		if (affliction == null)
		{
			Debug.LogError("Tried to apply null affliction! This is probably a big problem!");
			return;
		}
		Affliction affliction2 = affliction.Copy();
		Affliction affliction3;
		if (this.HasAfflictionType(this.afflictionList, affliction2.GetAfflictionType(), out affliction3))
		{
			affliction3.Stack(affliction2);
		}
		else
		{
			this.afflictionList.Add(affliction2);
			affliction2.character = this.character;
			affliction2.OnApplied();
			Debug.Log(string.Format("Added {0} to {1}", affliction2.GetAfflictionType(), this.character.gameObject.name));
		}
		if (!fromRPC && this.character.IsLocal)
		{
			this.PushAfflictions(null);
		}
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00005624 File Offset: 0x00003824
	public void RemoveAffliction(Affliction affliction, bool fromRPC = false)
	{
		if (!this.character.IsLocal && !fromRPC)
		{
			return;
		}
		this.afflictionList.Remove(affliction);
		affliction.OnRemoved();
		Debug.Log(string.Format("Removed {0} to {1}", affliction.GetAfflictionType(), this.character.gameObject.name));
		if (!fromRPC && this.character.IsLocal)
		{
			this.PushAfflictions(null);
		}
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00005698 File Offset: 0x00003898
	public float GetCurrentStatus(CharacterAfflictions.STATUSTYPE statusType)
	{
		if (this.currentStatuses.WithinRange((int)statusType))
		{
			return this.currentStatuses[(int)statusType];
		}
		return 0f;
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x000056C4 File Offset: 0x000038C4
	public float GetIncrementalStatus(CharacterAfflictions.STATUSTYPE statusType)
	{
		return this.currentIncrementalStatuses[(int)statusType];
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x000056DC File Offset: 0x000038DC
	public float LastAddedStatus(CharacterAfflictions.STATUSTYPE statusType)
	{
		return this.lastAddedStatus[(int)statusType];
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x000056F4 File Offset: 0x000038F4
	public float LastAddedIncrementalStatus(CharacterAfflictions.STATUSTYPE statusType)
	{
		return this.lastAddedIncrementalStatus[(int)statusType];
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x060000A8 RID: 168 RVA: 0x0000570C File Offset: 0x0000390C
	public float statusSum
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < this.currentStatuses.Length; i++)
			{
				num += this.currentStatuses[i];
			}
			return num;
		}
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x00005740 File Offset: 0x00003940
	public void SetStatus(CharacterAfflictions.STATUSTYPE statusType, float amount)
	{
		if (this.character.isBot)
		{
			return;
		}
		if (!this.character.IsLocal)
		{
			return;
		}
		Mathf.FloorToInt(amount / 0.025f);
		this.currentStatuses[(int)statusType] = amount;
		this.currentStatuses[(int)statusType] = Mathf.Clamp(this.currentStatuses[(int)statusType], 0f, this.GetStatusCap(statusType));
		this.currentIncrementalStatuses[(int)statusType] = 0f;
		this.currentDecrementalStatuses[(int)statusType] = 0f;
		this.character.ClampStamina();
		GUIManager.instance.bar.ChangeBar();
		this.PushStatuses(null);
	}

	// Token: 0x060000AA RID: 170 RVA: 0x000057DD File Offset: 0x000039DD
	public void AdjustStatus(CharacterAfflictions.STATUSTYPE statusType, float amount, bool fromRPC = false)
	{
		if (amount > 0f)
		{
			this.AddStatus(statusType, amount, fromRPC);
			return;
		}
		if (amount < 0f)
		{
			this.SubtractStatus(statusType, Mathf.Abs(amount), fromRPC);
		}
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00005808 File Offset: 0x00003A08
	public bool AddStatus(CharacterAfflictions.STATUSTYPE statusType, float amount, bool fromRPC = false)
	{
		if (this.character.isBot)
		{
			return false;
		}
		if (this.character.statusesLocked)
		{
			return false;
		}
		if (amount == 0f)
		{
			return false;
		}
		if (this.m_inAirport)
		{
			return false;
		}
		float num = 2f - this.statusSum;
		if (!this.character.IsLocal && !fromRPC)
		{
			return false;
		}
		float currentStatus = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Hot);
		float currentStatus2 = this.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Cold);
		if (statusType == CharacterAfflictions.STATUSTYPE.Cold && currentStatus > 0f)
		{
			this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hot, amount, fromRPC);
			amount -= currentStatus;
			if (amount <= 0f)
			{
				return false;
			}
		}
		else if (statusType == CharacterAfflictions.STATUSTYPE.Hot && currentStatus2 > 0f)
		{
			this.SubtractStatus(CharacterAfflictions.STATUSTYPE.Cold, amount, fromRPC);
			amount -= currentStatus2;
			if (amount <= 0f)
			{
				return false;
			}
		}
		this.currentIncrementalStatuses[(int)statusType] += amount;
		this.lastAddedIncrementalStatus[(int)statusType] = Time.time;
		Action<CharacterAfflictions.STATUSTYPE, float> onAddedIncrementalStatus = this.OnAddedIncrementalStatus;
		if (onAddedIncrementalStatus != null)
		{
			onAddedIncrementalStatus(statusType, amount);
		}
		if (this.currentIncrementalStatuses[(int)statusType] >= 0.025f)
		{
			float num2 = (float)Mathf.FloorToInt(this.currentIncrementalStatuses[(int)statusType] / 0.025f) * 0.025f;
			num2 = Mathf.Min(num2, num);
			this.currentStatuses[(int)statusType] += num2;
			this.currentStatuses[(int)statusType] = Mathf.Clamp(this.currentStatuses[(int)statusType], 0f, this.GetStatusCap(statusType));
			Action<CharacterAfflictions.STATUSTYPE, float> onAddedStatus = this.OnAddedStatus;
			if (onAddedStatus != null)
			{
				onAddedStatus(statusType, num2);
			}
			this.currentIncrementalStatuses[(int)statusType] = 0f;
			this.character.ClampStamina();
			GUIManager.instance.bar.ChangeBar();
			this.StatusSFX(statusType, amount);
			if (this.character.IsLocal && this.character == Character.observedCharacter)
			{
				GUIManager.instance.AddStatusFX(statusType, amount);
			}
			this.PlayParticle(statusType);
			this.lastAddedStatus[(int)statusType] = Time.time;
			this.PushStatuses(null);
		}
		return true;
	}

	// Token: 0x060000AC RID: 172 RVA: 0x000059E8 File Offset: 0x00003BE8
	public void SubtractStatus(CharacterAfflictions.STATUSTYPE statusType, float amount, bool fromRPC = false)
	{
		if (this.character.isBot)
		{
			return;
		}
		if (this.character.statusesLocked)
		{
			return;
		}
		if (!this.character.IsLocal && !fromRPC)
		{
			return;
		}
		if (this.currentStatuses[(int)statusType] == 0f)
		{
			this.currentDecrementalStatuses[(int)statusType] = 0f;
			return;
		}
		this.currentDecrementalStatuses[(int)statusType] += amount;
		if (this.currentDecrementalStatuses[(int)statusType] >= 0.025f)
		{
			float num = (float)Mathf.FloorToInt(this.currentDecrementalStatuses[(int)statusType] / 0.025f) * 0.025f;
			Debug.Log(string.Format("Removing status chunk: {0}", statusType));
			this.currentStatuses[(int)statusType] -= num;
			this.currentStatuses[(int)statusType] = Mathf.Clamp(this.currentStatuses[(int)statusType], 0f, this.GetStatusCap(statusType));
			if (statusType == CharacterAfflictions.STATUSTYPE.Hunger)
			{
				this.currentIncrementalStatuses[(int)statusType] = 0f;
			}
			this.currentDecrementalStatuses[(int)statusType] = 0f;
			this.character.ClampStamina();
			GUIManager.instance.bar.ChangeBar();
			this.PushStatuses(null);
		}
	}

	// Token: 0x060000AD RID: 173 RVA: 0x00005B08 File Offset: 0x00003D08
	private void StatusSFX(CharacterAfflictions.STATUSTYPE sT, float ammount)
	{
		if (sT == CharacterAfflictions.STATUSTYPE.Injury)
		{
			if (ammount > 0f && this.injurySmall)
			{
				this.injurySmall.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
			}
			if (ammount > 0.4f && this.injuryMid)
			{
				this.injuryMid.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
			}
			if (ammount > 0.75f && this.injuryHeavy)
			{
				this.injuryHeavy.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
				return;
			}
		}
		else if (sT == CharacterAfflictions.STATUSTYPE.Hunger)
		{
			if (this.injuryHunger)
			{
				this.injuryHunger.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
				return;
			}
		}
		else if (sT == CharacterAfflictions.STATUSTYPE.Cold)
		{
			if (this.injuryIce)
			{
				this.injuryIce.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
				return;
			}
		}
		else if (sT == CharacterAfflictions.STATUSTYPE.Hot)
		{
			if (this.injuryFire)
			{
				this.injuryFire.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
				return;
			}
		}
		else if (sT == CharacterAfflictions.STATUSTYPE.Poison && this.injuryPoison)
		{
			this.injuryPoison.Play(this.character.GetBodypartRig(BodypartType.Hip).transform.position);
		}
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00005C95 File Offset: 0x00003E95
	public void PlayDebugParticle()
	{
		this.PlayParticle(this.debugStatusType);
	}

	// Token: 0x060000AF RID: 175 RVA: 0x00005CA4 File Offset: 0x00003EA4
	public void PlayParticle(CharacterAfflictions.STATUSTYPE statusType)
	{
		switch (statusType)
		{
		case CharacterAfflictions.STATUSTYPE.Injury:
			this.character.refs.customization.PulseStatus(this.colorInjury);
			return;
		case CharacterAfflictions.STATUSTYPE.Hunger:
		case CharacterAfflictions.STATUSTYPE.Weight:
			break;
		case CharacterAfflictions.STATUSTYPE.Cold:
			this.character.refs.customization.PulseStatus(this.colorCold);
			return;
		case CharacterAfflictions.STATUSTYPE.Poison:
			this.character.refs.customization.PulseStatus(this.colorPoison);
			return;
		case CharacterAfflictions.STATUSTYPE.Crab:
			this.character.refs.customization.PulseStatus(this.colorCrab);
			return;
		case CharacterAfflictions.STATUSTYPE.Curse:
			this.character.refs.customization.PulseStatus(this.colorCurse);
			return;
		case CharacterAfflictions.STATUSTYPE.Drowsy:
			this.character.refs.customization.PulseStatus(this.colorDrowsy);
			return;
		case CharacterAfflictions.STATUSTYPE.Hot:
			this.character.refs.customization.PulseStatus(this.colorHot);
			break;
		default:
			return;
		}
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00005DA0 File Offset: 0x00003FA0
	public void PushStatuses(Photon.Realtime.Player specificPlayer = null)
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		byte[] array = IBinarySerializable.ToManagedArray<StatusSyncData>(new StatusSyncData
		{
			statusList = new List<float>(this.currentStatuses)
		});
		if (specificPlayer == null)
		{
			this.character.photonView.RPC("SyncStatusesRPC", RpcTarget.Others, new object[] { array });
			return;
		}
		this.character.photonView.RPC("SyncStatusesRPC", specificPlayer, new object[] { array });
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x00005E20 File Offset: 0x00004020
	[PunRPC]
	private void SyncStatusesRPC(byte[] data)
	{
		if (this.character.IsLocal)
		{
			return;
		}
		float[] array = IBinarySerializable.GetFromManagedArray<StatusSyncData>(data).statusList.ToArray();
		this.ApplyStatusesFromFloatArrayRPC(array);
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x00005E54 File Offset: 0x00004054
	[PunRPC]
	public void ApplyStatusesFromFloatArrayRPC(float[] deserializedData)
	{
		if (deserializedData.Length != this.currentStatuses.Length)
		{
			string text = "Deserialized data length for " + this.character.gameObject.name + " does not match current status length!!!\ndeserialized data:";
			for (int i = 0; i < deserializedData.Length; i++)
			{
				text = text + deserializedData[i].ToString() + ", ";
			}
			text += "\nlocal data:";
			for (int j = 0; j < this.currentStatuses.Length; j++)
			{
				text = text + this.currentStatuses[j].ToString() + ", ";
			}
			return;
		}
		for (int k = 0; k < deserializedData.Length; k++)
		{
			float num = deserializedData[k] - this.currentStatuses[k];
			if (num > 0f)
			{
				this.AddStatus((CharacterAfflictions.STATUSTYPE)k, num, true);
			}
			if (num < 0f)
			{
				this.SubtractStatus((CharacterAfflictions.STATUSTYPE)k, -num, true);
			}
		}
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x00005F38 File Offset: 0x00004138
	public void PushAfflictions(Photon.Realtime.Player specificPlayer = null)
	{
		if (!this.character.IsLocal)
		{
			return;
		}
		byte[] array = IBinarySerializable.ToManagedArray<AfflictionSyncData>(new AfflictionSyncData
		{
			afflictions = new List<Affliction>(this.afflictionList)
		});
		if (specificPlayer == null)
		{
			this.character.photonView.RPC("SyncAfflictionsRPC", RpcTarget.Others, new object[] { array });
			return;
		}
		this.character.photonView.RPC("SyncAfflictionsRPC", specificPlayer, new object[] { array });
	}

	// Token: 0x060000B4 RID: 180 RVA: 0x00005FB8 File Offset: 0x000041B8
	[PunRPC]
	private void SyncAfflictionsRPC(byte[] data)
	{
		if (this.character.IsLocal)
		{
			return;
		}
		Affliction[] array = IBinarySerializable.GetFromManagedArray<AfflictionSyncData>(data).afflictions.ToArray();
		for (int i = this.afflictionList.Count - 1; i >= 0; i--)
		{
			Affliction affliction = this.afflictionList[i];
			Affliction affliction2;
			if (!this.HasAfflictionType(array, affliction.GetAfflictionType(), out affliction2))
			{
				Debug.Log(string.Format("{0} removed old affliction: {1}", base.gameObject.name, affliction.GetAfflictionType()));
				this.RemoveAffliction(affliction, true);
			}
		}
		foreach (Affliction affliction3 in array)
		{
			Affliction affliction4;
			if (this.HasAfflictionType(this.afflictionList, affliction3.GetAfflictionType(), out affliction4))
			{
				Debug.Log(string.Format("{0} stacked affliction: {1}", base.gameObject.name, affliction3.GetAfflictionType()));
				affliction4.Stack(affliction3);
			}
			else
			{
				Debug.Log(string.Format("{0} added new affliction: {1}", base.gameObject.name, affliction3.GetAfflictionType()));
				this.AddAffliction(affliction3, true);
			}
		}
	}

	// Token: 0x060000B5 RID: 181 RVA: 0x000060DC File Offset: 0x000042DC
	private bool HasAfflictionType(IEnumerable<Affliction> afflictionList, Affliction.AfflictionType type, out Affliction affliction)
	{
		foreach (Affliction affliction2 in afflictionList)
		{
			if (affliction2.GetAfflictionType() == type)
			{
				affliction = affliction2;
				return true;
			}
		}
		affliction = null;
		return false;
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x00006134 File Offset: 0x00004334
	public float GetStatusCap(CharacterAfflictions.STATUSTYPE type)
	{
		if (this.statusCaps.ContainsKey(type))
		{
			return this.statusCaps[type];
		}
		return 2f;
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x00006156 File Offset: 0x00004356
	[ConsoleCommand]
	public static void Starve()
	{
		Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f, false);
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x00006174 File Offset: 0x00004374
	[ContextMenu("Test Poison over Time")]
	public void AddPoisonOverTime()
	{
		this.AddAffliction(new Affliction_PoisonOverTime(10f, 0f, 0.05f), false);
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x00006191 File Offset: 0x00004391
	[ConsoleCommand]
	public static void ClearAllAilments()
	{
		Character.localCharacter.refs.afflictions.ClearAllStatus(false);
	}

	// Token: 0x060000BA RID: 186 RVA: 0x000061A8 File Offset: 0x000043A8
	public void ClearAllStatus(bool excludeCurse = true)
	{
		int num = Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE)).Length;
		for (int i = 0; i < num; i++)
		{
			CharacterAfflictions.STATUSTYPE statustype = (CharacterAfflictions.STATUSTYPE)i;
			Debug.Log("Clearing status: " + statustype.ToString());
			if ((!excludeCurse || statustype != CharacterAfflictions.STATUSTYPE.Curse) && statustype != CharacterAfflictions.STATUSTYPE.Crab)
			{
				Debug.Log(string.Format("Current: {0}, amount {1}", statustype, this.character.refs.afflictions.GetCurrentStatus(statustype)));
				Debug.Log(string.Format("SetStatus status: {0}", statustype));
				this.character.refs.afflictions.SetStatus(statustype, 0f);
			}
		}
	}

	// Token: 0x060000BB RID: 187 RVA: 0x00006262 File Offset: 0x00004462
	[ConsoleCommand]
	public static void ClearHunger()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Hunger, 0f);
	}

	// Token: 0x060000BC RID: 188 RVA: 0x0000627E File Offset: 0x0000447E
	[ConsoleCommand]
	public static void ClearDrowsy()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 0f);
	}

	// Token: 0x060000BD RID: 189 RVA: 0x0000629A File Offset: 0x0000449A
	[ConsoleCommand]
	public static void ClearInjury()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Injury, 0f);
	}

	// Token: 0x060000BE RID: 190 RVA: 0x000062B6 File Offset: 0x000044B6
	[ConsoleCommand]
	public static void ClearCurse()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Curse, 0f);
	}

	// Token: 0x060000BF RID: 191 RVA: 0x000062D2 File Offset: 0x000044D2
	[ConsoleCommand]
	public static void ClearCold()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Cold, 0f);
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x000062EE File Offset: 0x000044EE
	[ConsoleCommand]
	public static void ClearPoison()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Poison, 0f);
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x0000630A File Offset: 0x0000450A
	[ConsoleCommand]
	public static void ClearHot()
	{
		Character.localCharacter.refs.afflictions.SetStatus(CharacterAfflictions.STATUSTYPE.Hot, 0f);
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x00006326 File Offset: 0x00004526
	[ConsoleCommand]
	public static void ClearAll()
	{
		Character.localCharacter.refs.afflictions.ClearAllStatus(false);
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00006340 File Offset: 0x00004540
	public void ClearPoisonAfflictions()
	{
		List<Affliction> list = new List<Affliction>();
		foreach (Affliction affliction in this.afflictionList)
		{
			if (affliction is Affliction_PoisonOverTime)
			{
				list.Add(affliction);
			}
			else
			{
				Affliction_AdjustStatusOverTime affliction_AdjustStatusOverTime = affliction as Affliction_AdjustStatusOverTime;
				if (affliction_AdjustStatusOverTime != null && affliction_AdjustStatusOverTime.statusType == CharacterAfflictions.STATUSTYPE.Poison && affliction_AdjustStatusOverTime.statusPerSecond > 0f)
				{
					list.Add(affliction);
				}
			}
		}
		foreach (Affliction affliction2 in list)
		{
			this.RemoveAffliction(affliction2, false);
		}
	}

	// Token: 0x060000C4 RID: 196 RVA: 0x0000640C File Offset: 0x0000460C
	[ContextMenu("Test Full Drowsy")]
	[ConsoleCommand]
	public static void AddDrowsy()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 0.2f, false);
	}

	// Token: 0x060000C5 RID: 197 RVA: 0x0000642F File Offset: 0x0000462F
	[ContextMenu("Test Curse")]
	[ConsoleCommand]
	public static void AddCurse()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 0.2f, false);
	}

	// Token: 0x060000C6 RID: 198 RVA: 0x00006452 File Offset: 0x00004652
	[ContextMenu("Test Death")]
	[ConsoleCommand]
	public static void Die()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 1f, false);
	}

	// Token: 0x060000C7 RID: 199 RVA: 0x00006475 File Offset: 0x00004675
	[ContextMenu("Add Poison")]
	[ConsoleCommand]
	public static void AddPoison()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.2f, false);
	}

	// Token: 0x060000C8 RID: 200 RVA: 0x00006498 File Offset: 0x00004698
	[ContextMenu("Test Cold")]
	[ConsoleCommand]
	public static void AddCold()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, 0.2f, false);
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x000064BB File Offset: 0x000046BB
	[ContextMenu("Test Hot")]
	[ConsoleCommand]
	public static void AddHot()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, 0.2f, false);
	}

	// Token: 0x060000CA RID: 202 RVA: 0x000064DE File Offset: 0x000046DE
	[ContextMenu("Test Injury")]
	[ConsoleCommand]
	public static void AddInjury()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 0.2f, false);
	}

	// Token: 0x060000CB RID: 203 RVA: 0x00006501 File Offset: 0x00004701
	[ContextMenu("Test Hunger")]
	[ConsoleCommand]
	public static void AddHunger()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 0.2f, false);
	}

	// Token: 0x060000CC RID: 204 RVA: 0x00006524 File Offset: 0x00004724
	[ContextMenu("Test Crab")]
	public static void TestCrab()
	{
		PlayerHandler.GetPlayerCharacter(PhotonNetwork.LocalPlayer).refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Crab, 0.2f, false);
	}

	// Token: 0x04000043 RID: 67
	private Dictionary<CharacterAfflictions.STATUSTYPE, float> statusCaps = new Dictionary<CharacterAfflictions.STATUSTYPE, float> { 
	{
		CharacterAfflictions.STATUSTYPE.Injury,
		1f
	} };

	// Token: 0x04000044 RID: 68
	[SerializeField]
	public float[] currentStatuses;

	// Token: 0x04000045 RID: 69
	private float[] currentIncrementalStatuses;

	// Token: 0x04000046 RID: 70
	private float[] currentDecrementalStatuses;

	// Token: 0x04000047 RID: 71
	private float[] lastAddedStatus;

	// Token: 0x04000048 RID: 72
	private float[] lastAddedIncrementalStatus;

	// Token: 0x04000049 RID: 73
	public float poisonReductionPerSecond;

	// Token: 0x0400004A RID: 74
	public float poisonReductionCooldown;

	// Token: 0x0400004B RID: 75
	public float drowsyReductionPerSecond;

	// Token: 0x0400004C RID: 76
	public float drowsyReductionCooldown;

	// Token: 0x0400004D RID: 77
	public float hotReductionPerSecond;

	// Token: 0x0400004E RID: 78
	public float hotReductionCooldown;

	// Token: 0x0400004F RID: 79
	public float hungerPerSecond = 0.0005f;

	// Token: 0x04000050 RID: 80
	public float nightColdPerSecond = 0.002f;

	// Token: 0x04000051 RID: 81
	public Character character;

	// Token: 0x04000052 RID: 82
	[SerializeReference]
	public List<Affliction> afflictionList = new List<Affliction>();

	// Token: 0x04000053 RID: 83
	[FormerlySerializedAs("headVFX")]
	public Transform headVfxTransform;

	// Token: 0x04000054 RID: 84
	[ColorUsage(false, true)]
	public Color colorInjury;

	// Token: 0x04000055 RID: 85
	[ColorUsage(false, true)]
	public Color colorCold;

	// Token: 0x04000056 RID: 86
	[ColorUsage(false, true)]
	public Color colorCrab;

	// Token: 0x04000057 RID: 87
	[ColorUsage(false, true)]
	public Color colorPoison;

	// Token: 0x04000058 RID: 88
	[ColorUsage(false, true)]
	public Color colorCurse;

	// Token: 0x04000059 RID: 89
	[ColorUsage(false, true)]
	public Color colorDrowsy;

	// Token: 0x0400005A RID: 90
	[ColorUsage(false, true)]
	public Color colorHot;

	// Token: 0x0400005B RID: 91
	public SFX_Instance injurySmall;

	// Token: 0x0400005C RID: 92
	public SFX_Instance injuryMid;

	// Token: 0x0400005D RID: 93
	public SFX_Instance injuryHeavy;

	// Token: 0x0400005E RID: 94
	public SFX_Instance injuryIce;

	// Token: 0x0400005F RID: 95
	public SFX_Instance injuryFire;

	// Token: 0x04000060 RID: 96
	public SFX_Instance injuryPoison;

	// Token: 0x04000061 RID: 97
	public SFX_Instance injuryHunger;

	// Token: 0x04000062 RID: 98
	public Action<CharacterAfflictions.STATUSTYPE, float> OnAddedStatus;

	// Token: 0x04000063 RID: 99
	public Action<CharacterAfflictions.STATUSTYPE, float> OnAddedIncrementalStatus;

	// Token: 0x04000064 RID: 100
	private bool m_inAirport;

	// Token: 0x04000065 RID: 101
	private float lastAddedPoison;

	// Token: 0x04000066 RID: 102
	public const float STATUS_INCREMENT = 0.025f;

	// Token: 0x04000067 RID: 103
	public const float MAX_TOTAL_STATUS = 2f;

	// Token: 0x04000068 RID: 104
	public CharacterAfflictions.STATUSTYPE debugStatusType;

	// Token: 0x020002E8 RID: 744
	public enum STATUSTYPE
	{
		// Token: 0x040010A0 RID: 4256
		Injury,
		// Token: 0x040010A1 RID: 4257
		Hunger,
		// Token: 0x040010A2 RID: 4258
		Cold,
		// Token: 0x040010A3 RID: 4259
		Poison,
		// Token: 0x040010A4 RID: 4260
		Crab,
		// Token: 0x040010A5 RID: 4261
		Curse,
		// Token: 0x040010A6 RID: 4262
		Drowsy,
		// Token: 0x040010A7 RID: 4263
		Weight,
		// Token: 0x040010A8 RID: 4264
		Hot
	}
}
