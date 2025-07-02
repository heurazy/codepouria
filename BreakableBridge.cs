using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000198 RID: 408
public class BreakableBridge : OnNetworkStart
{
	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x06000B2F RID: 2863 RVA: 0x0003749D File Offset: 0x0003569D
	public bool LocalCharacterOnBridge
	{
		get
		{
			return Time.time - this.localTouchStamp < 0.2f;
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x06000B30 RID: 2864 RVA: 0x000374B2 File Offset: 0x000356B2
	private float DistanceToLocalPlayer
	{
		get
		{
			return Vector3.Distance(Character.localCharacter.Center, base.transform.position);
		}
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x000374D0 File Offset: 0x000356D0
	private void Awake()
	{
		this.jungleVine = base.GetComponent<JungleVine>();
		this.photonView = base.GetComponent<PhotonView>();
		this.source = base.GetComponent<AudioSource>();
		foreach (CollisionModifier collisionModifier in base.GetComponentsInChildren<CollisionModifier>())
		{
			collisionModifier.applyEffects = false;
			collisionModifier.onCollide = (Action<Character, CollisionModifier>)Delegate.Combine(collisionModifier.onCollide, new Action<Character, CollisionModifier>(this.OnBridgeCollision));
		}
		this.rend = base.GetComponentInChildren<Renderer>();
		this.rend.material.SetFloat(BreakableBridge.JitterAmount, 0f);
		this.rend.material.SetFloat(BreakableBridge.AlphaClip, 0.01f);
		if (this.holdsPeople == 0)
		{
			this.holdsPeople = 5;
		}
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x00037590 File Offset: 0x00035790
	public override void NetworkStart()
	{
		this.holdsPeople = Random.Range(1, 5);
		this.photonView.RPC("SyncHoldsPeopleRPC", RpcTarget.All, new object[] { this.holdsPeople });
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x000375C4 File Offset: 0x000357C4
	private void Update()
	{
		if (this.isShaking)
		{
			this.source.pitch += 0.1f * Time.deltaTime;
			this.source.volume += 0.1f * Time.deltaTime;
			this.source.enabled = true;
		}
		if (!this.photonView.IsMine)
		{
			return;
		}
		if (this.isBreaking && !this.isShaking && !this.isFallen)
		{
			this.timeUntilBreak -= Time.deltaTime;
			if (this.timeUntilBreak < 0f)
			{
				this.photonView.RPC("ShakeBridge_Rpc", RpcTarget.All, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x0003767C File Offset: 0x0003587C
	private void FixedUpdate()
	{
		this.peopleOnBridge = 0;
		if (this.debug)
		{
			Debug.Log(string.Format("FixedUpdate: {0}, peopleOnBridge: {1}", Time.frameCount, this.peopleOnBridge));
		}
		this.peopleOnBridge = 0;
		foreach (Character character in this.peopleOnBridgeDict.Keys.ToList<Character>())
		{
			Dictionary<Character, float> dictionary = this.peopleOnBridgeDict;
			Character character2 = character;
			dictionary[character2] += Time.deltaTime;
			if (this.peopleOnBridgeDict[character] < 0.25f)
			{
				this.peopleOnBridge++;
			}
		}
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x0003774C File Offset: 0x0003594C
	private void OnDestroy()
	{
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x0003774E File Offset: 0x0003594E
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient || newPlayer == PhotonNetwork.LocalPlayer)
		{
			return;
		}
		this.photonView.RPC("SyncHoldsPeopleRPC", newPlayer, new object[] { this.holdsPeople });
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x0003778C File Offset: 0x0003598C
	[PunRPC]
	public void SyncHoldsPeopleRPC(int holdsPeople)
	{
		this.holdsPeople = holdsPeople;
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x00037798 File Offset: 0x00035998
	public void AddCollisionModifiers()
	{
		Debug.Log("AddCollisionModifiers");
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		Debug.Log(string.Format("colliers: {0}", componentsInChildren.Length));
		Collider[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.AddComponent<CollisionModifier>();
		}
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x000377EC File Offset: 0x000359EC
	private void OnBridgeCollision(Character character, CollisionModifier collider)
	{
		if (this.isBreaking)
		{
			return;
		}
		if (character == Character.localCharacter)
		{
			this.localTouchStamp = Time.time;
		}
		if (!this.photonView.IsMine)
		{
			return;
		}
		if (!this.peopleOnBridgeDict.TryAdd(character, 0f))
		{
			this.peopleOnBridgeDict[character] = 0f;
		}
		if (this.peopleOnBridge < this.holdsPeople)
		{
			return;
		}
		if (this.isShaking)
		{
			return;
		}
		if (this.holdsPeople >= this.peopleOnBridge)
		{
			return;
		}
		this.isBreaking = true;
		this.timeUntilBreak = Random.Range(2.5f, 7.5f);
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x00037890 File Offset: 0x00035A90
	[PunRPC]
	private void ShakeBridge_Rpc()
	{
		Debug.Log("start shake rock");
		this.isShaking = true;
		this.source.enabled = true;
		this.source.Play();
		if (!this.isShaking)
		{
			this.source.volume = 0.125f;
		}
		if (this.DistanceToLocalPlayer < this.startShakeDistance)
		{
			Debug.Log(string.Format("start shake {0}", this.startShakeAmount));
			GamefeelHandler.instance.AddPerlinShake(this.startShakeAmount, 0.2f, 15f);
		}
		base.StartCoroutine(this.<ShakeBridge_Rpc>g__RockShake|42_0());
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x0003792C File Offset: 0x00035B2C
	[PunRPC]
	private void Fall_Rpc()
	{
		base.StartCoroutine(this.<Fall_Rpc>g__DestroyRoutine|43_0());
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x00037A06 File Offset: 0x00035C06
	[CompilerGenerated]
	private IEnumerator <ShakeBridge_Rpc>g__RockShake|42_0()
	{
		Debug.Log("Start shaking");
		float duration = 0f;
		float timeUntilShake = 0f;
		this.rend.material.SetFloat(BreakableBridge.JitterAmount, 1f);
		while (duration < this.fallTime)
		{
			timeUntilShake -= Time.deltaTime;
			if (this.LocalCharacterOnBridge && timeUntilShake <= 0f)
			{
				GamefeelHandler.instance.AddPerlinShake(this.climbingScreenShake, 0.2f, 15f);
				Debug.Log("Clime shake");
				timeUntilShake = this.screenShakeTickTime;
			}
			Vector3 vector = Vector2.zero;
			vector.x += Mathf.PerlinNoise1D(100f + duration * this.shakeScale) * this.axisMul.x;
			vector.y += Mathf.PerlinNoise1D(10300f + duration * this.shakeScale) * this.axisMul.y;
			vector.z += Mathf.PerlinNoise1D(1340f + duration * this.shakeScale) * this.axisMul.z;
			vector *= this.amount;
			duration += Time.deltaTime;
			yield return null;
		}
		this.rend.material.SetFloat(BreakableBridge.JitterAmount, 0f);
		Debug.Log("Done shaking");
		if (this.isShaking)
		{
			for (int i = 0; i < this.breakSfx.Length; i++)
			{
				this.breakSfx[i].Play(base.transform.position);
			}
		}
		this.isShaking = false;
		this.fullMesh.localPosition = 0.ToVec();
		this.source.volume = 0f;
		this.source.Stop();
		if (this.photonView.IsMine)
		{
			this.photonView.RPC("Fall_Rpc", RpcTarget.All, Array.Empty<object>());
		}
		yield break;
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x00037A15 File Offset: 0x00035C15
	[CompilerGenerated]
	private IEnumerator <Fall_Rpc>g__DestroyRoutine|43_0()
	{
		this.isFallen = true;
		Object.DestroyImmediate(this.jungleVine.colliderRoot.gameObject);
		if (this.breakParticles != null)
		{
			this.breakParticles.Play();
		}
		float normalizedTime = 0f;
		while (normalizedTime < 1f)
		{
			normalizedTime += Time.deltaTime * 0.7f;
			this.rend.material.SetFloat(BreakableBridge.BreakAmount, normalizedTime);
			yield return null;
		}
		Debug.Log(string.Format("Destroy: {0}", base.gameObject), base.gameObject);
		yield return null;
		yield break;
	}

	// Token: 0x04000A36 RID: 2614
	private static readonly int JitterAmount = Shader.PropertyToID("_JitterAmount");

	// Token: 0x04000A37 RID: 2615
	private static readonly int BreakAmount = Shader.PropertyToID("_BreakAmount");

	// Token: 0x04000A38 RID: 2616
	private static readonly int AlphaClip = Shader.PropertyToID("_AlphaClip");

	// Token: 0x04000A39 RID: 2617
	public SFX_Instance[] breakSfx;

	// Token: 0x04000A3A RID: 2618
	[Range(0f, 1f)]
	public float breakPoint = 0.4f;

	// Token: 0x04000A3B RID: 2619
	[Range(0f, 1f)]
	public float breakChance = 0.5f;

	// Token: 0x04000A3C RID: 2620
	public Vector3 axisMul = new Vector3(1f, 1f, 1f);

	// Token: 0x04000A3D RID: 2621
	public float shakeScale = 30f;

	// Token: 0x04000A3E RID: 2622
	public float fallTime = 5f;

	// Token: 0x04000A3F RID: 2623
	public float amount = 1f;

	// Token: 0x04000A40 RID: 2624
	public float startShakeDistance = 10f;

	// Token: 0x04000A41 RID: 2625
	public float startShakeAmount = 400f;

	// Token: 0x04000A42 RID: 2626
	public float climbingScreenShake = 240f;

	// Token: 0x04000A43 RID: 2627
	public float screenShakeTickTime = 0.2f;

	// Token: 0x04000A44 RID: 2628
	public bool debug;

	// Token: 0x04000A45 RID: 2629
	public bool isShaking;

	// Token: 0x04000A46 RID: 2630
	public float localTouchStamp;

	// Token: 0x04000A47 RID: 2631
	public int holdsPeople;

	// Token: 0x04000A48 RID: 2632
	public int peopleOnBridge;

	// Token: 0x04000A49 RID: 2633
	public Transform fullMesh;

	// Token: 0x04000A4A RID: 2634
	public ParticleSystem breakParticles;

	// Token: 0x04000A4B RID: 2635
	private readonly Dictionary<Character, float> peopleOnBridgeDict = new Dictionary<Character, float>();

	// Token: 0x04000A4C RID: 2636
	private new PhotonView photonView;

	// Token: 0x04000A4D RID: 2637
	private Renderer rend;

	// Token: 0x04000A4E RID: 2638
	private AudioSource source;

	// Token: 0x04000A4F RID: 2639
	private JungleVine jungleVine;

	// Token: 0x04000A50 RID: 2640
	private float timeUntilBreak;

	// Token: 0x04000A51 RID: 2641
	private bool isBreaking;

	// Token: 0x04000A52 RID: 2642
	private bool isFallen;
}
