using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200007B RID: 123
public class AirportCheckInKiosk : MonoBehaviourPun, IInteractibleConstant, IInteractible
{
	// Token: 0x0600044B RID: 1099 RVA: 0x00019722 File Offset: 0x00017922
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x00019725 File Offset: 0x00017925
	public void Awake()
	{
		this.mpb = new MaterialPropertyBlock();
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x00019734 File Offset: 0x00017934
	private void Start()
	{
		if (GameHandler.GetService<NextLevelService>().Data.IsSome)
		{
			Debug.Log(string.Format("seconds left until next map... {0}", GameHandler.GetService<NextLevelService>().Data.Value.SecondsLeft));
		}
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x0001977D File Offset: 0x0001797D
	public void Interact(Character interactor)
	{
	}

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x0600044F RID: 1103 RVA: 0x0001977F File Offset: 0x0001797F
	// (set) Token: 0x06000450 RID: 1104 RVA: 0x000197AD File Offset: 0x000179AD
	private MeshRenderer[] meshRenderers
	{
		get
		{
			if (this._mr == null)
			{
				this._mr = base.GetComponentsInChildren<MeshRenderer>();
				MonoBehaviour.print(this._mr.Length);
			}
			return this._mr;
		}
		set
		{
			this._mr = value;
		}
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x000197B8 File Offset: 0x000179B8
	public void HoverEnter()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				if (this.meshRenderers[i] != null)
				{
					this.meshRenderers[i].SetPropertyBlock(this.mpb);
				}
			}
		}
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x00019818 File Offset: 0x00017A18
	public void HoverExit()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			for (int i = 0; i < this.meshRenderers.Length; i++)
			{
				this.meshRenderers[i].SetPropertyBlock(this.mpb);
			}
		}
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x00019868 File Offset: 0x00017A68
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000454 RID: 1108 RVA: 0x00019875 File Offset: 0x00017A75
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x0001987D File Offset: 0x00017A7D
	public string GetInteractionText()
	{
		return "Board Flight";
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x00019884 File Offset: 0x00017A84
	public string GetName()
	{
		return "Gate Kiosk";
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x0001988B File Offset: 0x00017A8B
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.IsInteractible(interactor);
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x00019894 File Offset: 0x00017A94
	public float GetInteractTime(Character interactor)
	{
		return this.interactTime;
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x0001989C File Offset: 0x00017A9C
	public void Interact_CastFinished(Character interactor)
	{
		GUIManager.instance.boardingPass.Open();
		GUIManager.instance.boardingPass.kiosk = this;
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x000198BD File Offset: 0x00017ABD
	public void StartGame(int ascent)
	{
		base.photonView.RPC("LoadIslandMaster", RpcTarget.MasterClient, new object[] { ascent });
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x000198DF File Offset: 0x00017ADF
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x000198E1 File Offset: 0x00017AE1
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x000198E4 File Offset: 0x00017AE4
	[PunRPC]
	public void LoadIslandMaster(int ascent)
	{
		MenuWindow.CloseAllWindows();
		if (!PhotonNetwork.IsMasterClient)
		{
			return;
		}
		Debug.Log("Loading scene as master.");
		NextLevelService service = GameHandler.GetService<NextLevelService>();
		string text = "WilIsland";
		if (service.Data.IsSome)
		{
			text = SingletonAsset<MapBaker>.Instance.GetLevel(service.Data.Value.CurrentLevelIndex);
		}
		else if (PhotonNetwork.OfflineMode)
		{
			text = SingletonAsset<MapBaker>.Instance.GetLevel(0);
		}
		if (string.IsNullOrEmpty(text))
		{
			text = "WilIsland";
		}
		base.photonView.RPC("BeginIslandLoadRPC", RpcTarget.All, new object[] { text, ascent });
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x00019984 File Offset: 0x00017B84
	[PunRPC]
	public void BeginIslandLoadRPC(string sceneName, int ascent)
	{
		GameHandler.AddStatus<SceneSwitchingStatus>(new SceneSwitchingStatus());
		Debug.Log("Begin scene load RPC: " + sceneName);
		Ascents.currentAscent = ascent;
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Plane, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess(sceneName, true, true, 0f) });
	}

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x0600045F RID: 1119 RVA: 0x000199D8 File Offset: 0x00017BD8
	public bool holdOnFinish { get; }

	// Token: 0x0400049B RID: 1179
	public float interactTime;

	// Token: 0x0400049C RID: 1180
	private MaterialPropertyBlock mpb;

	// Token: 0x0400049D RID: 1181
	private MeshRenderer[] _mr;
}
