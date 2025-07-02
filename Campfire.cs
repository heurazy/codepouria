using System;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

// Token: 0x0200007E RID: 126
public class Campfire : MonoBehaviour, IInteractibleConstant, IInteractible
{
	// Token: 0x17000049 RID: 73
	// (get) Token: 0x06000466 RID: 1126 RVA: 0x00019D65 File Offset: 0x00017F65
	public bool Lit
	{
		get
		{
			return this.state == Campfire.FireState.Lit;
		}
	}

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x06000467 RID: 1127 RVA: 0x00019D70 File Offset: 0x00017F70
	// (set) Token: 0x06000468 RID: 1128 RVA: 0x00019D78 File Offset: 0x00017F78
	public int FireWoodCount
	{
		get
		{
			return this.fireWoodCount;
		}
		set
		{
			this.view.RPC("SetFireWoodCount", RpcTarget.All, new object[] { value });
		}
	}

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x06000469 RID: 1129 RVA: 0x00019D9A File Offset: 0x00017F9A
	public float LitProgress
	{
		get
		{
			return (this.beenBurningFor / this.burnsFor).Clamp01();
		}
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x00019DB0 File Offset: 0x00017FB0
	private void Awake()
	{
		this.view = base.GetComponent<PhotonView>();
		this.mainRenderer = base.GetComponentInChildren<Renderer>();
		this.startRot = this.fireParticles.emission.rateOverTime.constant;
		this.startSize = new Vector2(this.fireParticles.main.startSize.constantMin, this.fireParticles.main.startSize.constantMax);
		this.SetFireWoodCount(3);
		this.UpdateLit();
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x00019E44 File Offset: 0x00018044
	private void Update()
	{
		if (this.Lit)
		{
			this.beenBurningFor += Time.deltaTime;
			ParticleSystem.MainModule main = this.fireParticles.main;
			ParticleSystem.MinMaxCurve minMaxCurve = main.startSize;
			minMaxCurve.constantMin = Mathf.Lerp(this.startSize.x, this.endSize.x, this.LitProgress);
			minMaxCurve.constantMax = Mathf.Lerp(this.startSize.y, this.endSize.y, this.LitProgress);
			main.startSize = minMaxCurve;
			ParticleSystem.EmissionModule emission = this.fireParticles.emission;
			ParticleSystem.MinMaxCurve rateOverTime = emission.rateOverTime;
			rateOverTime.constant = Mathf.Lerp(this.startRot, this.endRot, this.LitProgress);
			emission.rateOverTime = rateOverTime;
			if (!this.t)
			{
				if (!this.isPyre && MoraleBoost.SpawnMoraleBoost(base.transform.position, this.moraleBoostRadius, this.moraleBoostBaseline, this.moraleBoostPerAdditionalScout, false, 2))
				{
					for (int i = 0; i < this.moraleBoost.Length; i++)
					{
						this.moraleBoost[i].Play(base.transform.position);
					}
					Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.MoraleBoosts, 1);
				}
				if (Character.localCharacter != null && Vector3.Distance(base.transform.position, Character.localCharacter.Center) <= this.moraleBoostRadius)
				{
					Character.localCharacter.refs.afflictions.AdjustStatus(CharacterAfflictions.STATUSTYPE.Injury, -0.2f, false);
				}
				for (int j = 0; j < this.fireStart.Length; j++)
				{
					this.fireStart[j].Play(base.transform.position);
				}
				this.t = true;
			}
			if (this.view.IsMine && this.beenBurningFor > this.burnsFor && !this.isPyre)
			{
				this.view.RPC("Extinguish_Rpc", RpcTarget.AllBuffered, Array.Empty<object>());
			}
		}
		else if (this.t)
		{
			for (int k = 0; k < this.extinguish.Length; k++)
			{
				this.extinguish[k].Play(base.transform.position);
			}
			this.t = false;
		}
		this.StupidTextUpdate();
		this.UpdateAudioLoop();
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x0001A090 File Offset: 0x00018290
	private void StupidTextUpdate()
	{
		if (GUIManager.instance.currentInteractable == this)
		{
			GUIManager.instance.RefreshInteractablePrompt();
		}
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x0001A0A9 File Offset: 0x000182A9
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.moraleBoostRadius);
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x0001A0CC File Offset: 0x000182CC
	public Vector3 Center()
	{
		return this.mainRenderer.bounds.center;
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x0001A0EC File Offset: 0x000182EC
	public string GetInteractionText()
	{
		if (this.FireWoodCount >= this.requiredFireWoods && !this.Lit)
		{
			string text;
			if (!this.EveryoneInRange(out text))
			{
				return text;
			}
			return "light";
		}
		else
		{
			if (this.Lit)
			{
				return "cook";
			}
			return "";
		}
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x0001A134 File Offset: 0x00018334
	public string GetName()
	{
		if (!string.IsNullOrEmpty(this.nameOverride))
		{
			return this.nameOverride;
		}
		if (!this.isPyre)
		{
			return "campfire";
		}
		return "pyre";
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x0001A15D File Offset: 0x0001835D
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x0001A165 File Offset: 0x00018365
	public void HoverEnter()
	{
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x0001A167 File Offset: 0x00018367
	public void HoverExit()
	{
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x0001A16C File Offset: 0x0001836C
	public void Interact(Character interactor)
	{
		if (!this.Lit && this.FireWoodCount < this.requiredFireWoods && interactor.data.currentItem != null && interactor.data.currentItem.GetComponent<FireWoodItem>())
		{
			interactor.data.currentItem.StartCoroutine(interactor.data.currentItem.ConsumeDelayed(true));
			int num = this.FireWoodCount;
			this.FireWoodCount = num + 1;
		}
		if (this.Lit && interactor.data.currentItem != null && interactor.data.currentItem.cooking.canBeCooked)
		{
			this.currentlyCookingItem = interactor.data.currentItem;
			interactor.data.currentItem.GetComponent<ItemCooking>().StartCookingVisuals();
		}
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x0001A244 File Offset: 0x00018444
	public void Interact_CastFinished(Character interactor)
	{
		Debug.Log("Interact_CastFinished");
		string text;
		if (this.Lit)
		{
			if (this.currentlyCookingItem)
			{
				if (this.currentlyCookingItem.GetData<IntItemData>(DataEntryKey.CookedAmount).Value == 0)
				{
					Singleton<AchievementManager>.Instance.IncrementSteamStat(STEAMSTATTYPE.MealsCooked, 1);
				}
				this.currentlyCookingItem.GetComponent<ItemCooking>().FinishCooking();
				return;
			}
		}
		else if (this.EveryoneInRange(out text))
		{
			this.view.RPC("Light_Rpc", RpcTarget.AllBuffered, Array.Empty<object>());
		}
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x0001A2C0 File Offset: 0x000184C0
	public void CancelCast(Character interactor)
	{
		if (this.currentlyCookingItem)
		{
			this.currentlyCookingItem.GetComponent<ItemCooking>().CancelCookingVisuals();
		}
		this.currentlyCookingItem = null;
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x0001A2E6 File Offset: 0x000184E6
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x06000478 RID: 1144 RVA: 0x0001A2E8 File Offset: 0x000184E8
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x0001A2EB File Offset: 0x000184EB
	public bool IsInteractible(Character interactor)
	{
		return this.state == Campfire.FireState.Off || (this.state != Campfire.FireState.Spent && interactor.data.currentItem != null);
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x0001A314 File Offset: 0x00018514
	public bool EveryoneInRange(out string printout)
	{
		bool flag = true;
		printout = "";
		foreach (Character character in PlayerHandler.GetAllPlayerCharacters())
		{
			float num = Vector3.Distance(base.transform.position, character.Center);
			if (num > 15f && !character.data.dead)
			{
				flag = false;
				printout += string.Format("\n{0} {1}m", character.photonView.Owner.NickName, Mathf.RoundToInt(num * CharacterStats.unitsToMeters));
			}
		}
		if (!flag)
		{
			printout = "can't light campfire with friends missing!\n" + printout;
		}
		return flag;
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x0001A3DC File Offset: 0x000185DC
	public bool IsConstantlyInteractable(Character interactor)
	{
		if (this.state == Campfire.FireState.Off)
		{
			return this.FireWoodCount >= this.requiredFireWoods;
		}
		return this.state != Campfire.FireState.Spent && interactor.data.currentItem != null;
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x0001A414 File Offset: 0x00018614
	public float GetInteractTime(Character interactor)
	{
		return this.cookTime;
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x0001A41C File Offset: 0x0001861C
	public void DebugLight()
	{
		this.fireWoodCount = this.requiredFireWoods;
		this.view.RPC("Light_Rpc", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x0001A440 File Offset: 0x00018640
	[PunRPC]
	private void SetFireWoodCount(int count)
	{
		this.HideLogs();
		this.fireWoodCount = count;
		for (int i = 0; i < Mathf.Min(this.logRoot.childCount, this.fireWoodCount); i++)
		{
			this.logRoot.GetChild(i).gameObject.SetActive(true);
		}
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x0001A494 File Offset: 0x00018694
	private void UpdateAudioLoop()
	{
		if (this.loop)
		{
			float num = (this.Lit ? 0.5f : 0f);
			this.loop.volume = Mathf.Lerp(this.loop.volume, num, Time.deltaTime * 5f);
		}
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x0001A4EC File Offset: 0x000186EC
	private void HideLogs()
	{
		foreach (object obj in this.logRoot)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x0001A548 File Offset: 0x00018748
	[PunRPC]
	private void Light_Rpc()
	{
		this.state = Campfire.FireState.Lit;
		this.UpdateLit();
		this.smokeParticlesOff.Stop();
		this.smokeParticlesLit.Play();
		GUIManager.instance.RefreshInteractablePrompt();
		if (this.advanceToSegment != Segment.TheKiln)
		{
			Singleton<MapHandler>.Instance.GoToSegment(this.advanceToSegment);
		}
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x0001A59C File Offset: 0x0001879C
	[PunRPC]
	private void Extinguish_Rpc()
	{
		this.beenBurningFor = 0f;
		this.state = Campfire.FireState.Spent;
		this.FireWoodCount = 0;
		this.HideLogs();
		this.UpdateLit();
		this.smokeParticlesOff.Stop();
		this.smokeParticlesLit.Stop();
		this.fireParticles.Stop();
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x0001A5F0 File Offset: 0x000187F0
	private void UpdateLit()
	{
		if (this.enableWhenLit)
		{
			this.enableWhenLit.SetActive(this.state == Campfire.FireState.Lit);
		}
		if (this.disableWhenLit)
		{
			this.disableWhenLit.SetActive(this.state == Campfire.FireState.Off || this.state == Campfire.FireState.Spent);
		}
	}

	// Token: 0x040004A3 RID: 1187
	public Segment advanceToSegment;

	// Token: 0x040004A4 RID: 1188
	public Campfire.FireState state;

	// Token: 0x040004A5 RID: 1189
	public GameObject enableWhenLit;

	// Token: 0x040004A6 RID: 1190
	public GameObject disableWhenLit;

	// Token: 0x040004A7 RID: 1191
	[FormerlySerializedAs("litTime")]
	public float burnsFor = 180f;

	// Token: 0x040004A8 RID: 1192
	public float cookTime = 5f;

	// Token: 0x040004A9 RID: 1193
	public Transform logRoot;

	// Token: 0x040004AA RID: 1194
	public int requiredFireWoods = 3;

	// Token: 0x040004AB RID: 1195
	public Vector2 endSize = new Vector2(0.1f, 0.2f);

	// Token: 0x040004AC RID: 1196
	public float endRot = 3f;

	// Token: 0x040004AD RID: 1197
	[FormerlySerializedAs("litTimeElapsed")]
	public float beenBurningFor;

	// Token: 0x040004AE RID: 1198
	public ParticleSystem fireParticles;

	// Token: 0x040004AF RID: 1199
	public ParticleSystem smokeParticlesOff;

	// Token: 0x040004B0 RID: 1200
	public ParticleSystem smokeParticlesLit;

	// Token: 0x040004B1 RID: 1201
	public float moraleBoostRadius;

	// Token: 0x040004B2 RID: 1202
	public float moraleBoostBaseline;

	// Token: 0x040004B3 RID: 1203
	public float moraleBoostPerAdditionalScout;

	// Token: 0x040004B4 RID: 1204
	public float injuryReduction = 0.2f;

	// Token: 0x040004B5 RID: 1205
	public SFX_Instance[] fireStart;

	// Token: 0x040004B6 RID: 1206
	public SFX_Instance[] extinguish;

	// Token: 0x040004B7 RID: 1207
	public SFX_Instance[] moraleBoost;

	// Token: 0x040004B8 RID: 1208
	public AudioSource loop;

	// Token: 0x040004B9 RID: 1209
	public bool isPyre;

	// Token: 0x040004BA RID: 1210
	public string nameOverride;

	// Token: 0x040004BB RID: 1211
	private Item currentlyCookingItem;

	// Token: 0x040004BC RID: 1212
	private int fireWoodCount;

	// Token: 0x040004BD RID: 1213
	private Renderer mainRenderer;

	// Token: 0x040004BE RID: 1214
	private float startRot;

	// Token: 0x040004BF RID: 1215
	private Vector2 startSize;

	// Token: 0x040004C0 RID: 1216
	private bool t;

	// Token: 0x040004C1 RID: 1217
	private PhotonView view;

	// Token: 0x02000309 RID: 777
	public enum FireState
	{
		// Token: 0x04001129 RID: 4393
		Off,
		// Token: 0x0400112A RID: 4394
		Lit,
		// Token: 0x0400112B RID: 4395
		Spent
	}
}
