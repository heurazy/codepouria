using System;
using Photon.Pun;
using Sirenix.Utilities;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000217 RID: 535
public class PlayerGhost : MonoBehaviour
{
	// Token: 0x06000DC6 RID: 3526 RVA: 0x00045943 File Offset: 0x00043B43
	private void Awake()
	{
		this.m_view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x00045954 File Offset: 0x00043B54
	[PunRPC]
	public void RPCA_InitGhost(PhotonView character, PhotonView t)
	{
		this.m_owner = character.GetComponent<Character>();
		this.m_owner.Ghost = this;
		this.RPCA_SetTarget(t);
		PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(this.m_owner.photonView.Owner);
		this.animatedMouth.audioSource = character.GetComponent<AnimatedMouth>().audioSource;
		this.CustomizeGhost(playerData.customizationData);
		if (character.IsMine)
		{
			this.PlayerRenderers.ForEach(delegate(Renderer r)
			{
				r.enabled = false;
			});
			this.EyeRenderers.ForEach(delegate(Renderer r)
			{
				r.enabled = false;
			});
			this.mouthRenderer.enabled = false;
			this.accessoryRenderer.enabled = false;
		}
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x00045A34 File Offset: 0x00043C34
	private void CustomizeGhost(CharacterCustomizationData data)
	{
		int currentSkin = data.currentSkin;
		if (currentSkin > Singleton<Customization>.Instance.skins.Length)
		{
			return;
		}
		for (int i = 0; i < this.PlayerRenderers.Length; i++)
		{
			this.PlayerRenderers[i].material.SetColor("_PlayerColor", Singleton<Customization>.Instance.skins[currentSkin].color);
		}
		for (int j = 0; j < this.EyeRenderers.Length; j++)
		{
			this.EyeRenderers[j].material.SetColor(PlayerGhost.SkinColor, Singleton<Customization>.Instance.skins[currentSkin].color);
		}
		for (int k = 0; k < this.EyeRenderers.Length; k++)
		{
			this.EyeRenderers[k].material.SetTexture(PlayerGhost.MainTex, Singleton<Customization>.Instance.eyes[data.currentEyes].texture);
		}
		this.accessoryRenderer.material.SetTexture(PlayerGhost.MainTex, Singleton<Customization>.Instance.accessories[data.currentAccessory].texture);
		this.mouthRenderer.material.SetTexture(PlayerGhost.MainTex, Singleton<Customization>.Instance.mouths[data.currentMouth].texture);
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x00045B64 File Offset: 0x00043D64
	[PunRPC]
	public void RPCA_SetTarget(PhotonView t)
	{
		this.m_target = t.GetComponent<Character>();
	}

	// Token: 0x06000DCA RID: 3530 RVA: 0x00045B74 File Offset: 0x00043D74
	private void Update()
	{
		Vector3 vector = this.m_target.Center;
		base.transform.rotation = Quaternion.LookRotation(this.m_owner.data.lookDirection);
		vector += base.transform.forward * -1f * this.m_owner.data.spectateZoom;
		vector += base.transform.up * 0.5f;
		base.transform.position = Vector3.Lerp(base.transform.position, vector, Time.deltaTime * 3f);
		base.transform.rotation = Quaternion.LookRotation(MainCamera.instance.cam.transform.position - base.transform.position);
	}

	// Token: 0x04000CD7 RID: 3287
	private static readonly int MainTex = Shader.PropertyToID("_MainTex");

	// Token: 0x04000CD8 RID: 3288
	private static readonly int SkinColor = Shader.PropertyToID("_SkinColor");

	// Token: 0x04000CD9 RID: 3289
	public Character m_target;

	// Token: 0x04000CDA RID: 3290
	public Character m_owner;

	// Token: 0x04000CDB RID: 3291
	public PhotonView m_view;

	// Token: 0x04000CDC RID: 3292
	[Header("Customization Refrences")]
	public Renderer[] PlayerRenderers;

	// Token: 0x04000CDD RID: 3293
	public Renderer[] EyeRenderers;

	// Token: 0x04000CDE RID: 3294
	public Renderer mouthRenderer;

	// Token: 0x04000CDF RID: 3295
	public Renderer accessoryRenderer;

	// Token: 0x04000CE0 RID: 3296
	public AnimatedMouth animatedMouth;
}
