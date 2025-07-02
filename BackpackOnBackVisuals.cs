using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000043 RID: 67
public class BackpackOnBackVisuals : BackpackVisuals, IInteractibleConstant, IInteractible
{
	// Token: 0x0600031E RID: 798 RVA: 0x00013ABE File Offset: 0x00011CBE
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
		this.InitRenderers();
	}

	// Token: 0x0600031F RID: 799 RVA: 0x00013AD2 File Offset: 0x00011CD2
	private void OnEnable()
	{
		this.RefreshCooking();
	}

	// Token: 0x06000320 RID: 800 RVA: 0x00013ADC File Offset: 0x00011CDC
	private void InitRenderers()
	{
		this.renderers = base.GetComponentsInChildren<MeshRenderer>();
		this.defaultTints = new Color[this.renderers.Length];
		for (int i = 0; i < this.renderers.Length; i++)
		{
			this.defaultTints[i] = this.renderers[i].material.GetColor("_Tint");
		}
	}

	// Token: 0x06000321 RID: 801 RVA: 0x00013B40 File Offset: 0x00011D40
	private void RefreshCooking()
	{
		IntItemData intItemData;
		if (this.character.player.backpackSlot.data.TryGetDataEntry<IntItemData>(DataEntryKey.CookedAmount, out intItemData))
		{
			this.CookVisually(intItemData.Value);
		}
	}

	// Token: 0x06000322 RID: 802 RVA: 0x00013B78 File Offset: 0x00011D78
	protected virtual void CookVisually(int cookedAmount)
	{
		Debug.Log("Cooking backpack visually");
		if (this.renderers == null)
		{
			this.InitRenderers();
		}
		for (int i = 0; i < this.renderers.Length; i++)
		{
			if (cookedAmount > 0)
			{
				Debug.Log(string.Format("Cooked amount is {0}", cookedAmount));
				this.renderers[i].material.SetColor("_Tint", this.defaultTints[i] * ItemCooking.GetCookColor(cookedAmount));
			}
		}
	}

	// Token: 0x06000323 RID: 803 RVA: 0x00013BF8 File Offset: 0x00011DF8
	public override BackpackData GetBackpackData()
	{
		BackpackData backpackData;
		if (!this.character.player.backpackSlot.data.TryGetDataEntry<BackpackData>(DataEntryKey.BackpackData, out backpackData))
		{
			this.character.player.backpackSlot.data.RegisterNewEntry<BackpackData>(DataEntryKey.BackpackData);
		}
		return backpackData;
	}

	// Token: 0x06000324 RID: 804 RVA: 0x00013C41 File Offset: 0x00011E41
	protected override void PutItemInBackpack(GameObject visual, byte slotID)
	{
		visual.GetComponent<PhotonView>().RPC("PutInBackpackRPC", RpcTarget.All, new object[]
		{
			slotID,
			BackpackReference.GetFromEquippedBackpack(this.character)
		});
	}

	// Token: 0x06000325 RID: 805 RVA: 0x00013C78 File Offset: 0x00011E78
	public bool IsInteractible(Character interactor)
	{
		Vector3 vector = HelperFunctions.ZeroY(interactor.data.lookDirection);
		Vector3 vector2 = HelperFunctions.ZeroY(base.transform.forward);
		return Vector3.Angle(vector, vector2) < 110f;
	}

	// Token: 0x06000326 RID: 806 RVA: 0x00013CB3 File Offset: 0x00011EB3
	public void Interact(Character interactor)
	{
	}

	// Token: 0x06000327 RID: 807 RVA: 0x00013CB8 File Offset: 0x00011EB8
	public void HoverEnter()
	{
		MeshRenderer componentInChildren = base.GetComponentInChildren<MeshRenderer>();
		if (componentInChildren)
		{
			componentInChildren.material.SetFloat(BackpackOnBackVisuals.Interactable, 1f);
		}
	}

	// Token: 0x06000328 RID: 808 RVA: 0x00013CEC File Offset: 0x00011EEC
	public void HoverExit()
	{
		MeshRenderer componentInChildren = base.GetComponentInChildren<MeshRenderer>();
		if (componentInChildren)
		{
			componentInChildren.material.SetFloat(BackpackOnBackVisuals.Interactable, 0f);
		}
	}

	// Token: 0x06000329 RID: 809 RVA: 0x00013D1D File Offset: 0x00011F1D
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x0600032A RID: 810 RVA: 0x00013D2A File Offset: 0x00011F2A
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x0600032B RID: 811 RVA: 0x00013D32 File Offset: 0x00011F32
	public string GetInteractionText()
	{
		return "open";
	}

	// Token: 0x0600032C RID: 812 RVA: 0x00013D39 File Offset: 0x00011F39
	public string GetName()
	{
		return this.character.characterName + "'s backpack";
	}

	// Token: 0x0600032D RID: 813 RVA: 0x00013D50 File Offset: 0x00011F50
	public bool IsConstantlyInteractable(Character interactor)
	{
		return this.IsInteractible(interactor);
	}

	// Token: 0x0600032E RID: 814 RVA: 0x00013D59 File Offset: 0x00011F59
	public float GetInteractTime(Character interactor)
	{
		return this.openRadialMenuTime;
	}

	// Token: 0x0600032F RID: 815 RVA: 0x00013D61 File Offset: 0x00011F61
	public void Interact_CastFinished(Character interactor)
	{
		GUIManager.instance.OpenBackpackWheel(BackpackReference.GetFromEquippedBackpack(this.character));
	}

	// Token: 0x06000330 RID: 816 RVA: 0x00013D78 File Offset: 0x00011F78
	public void CancelCast(Character interactor)
	{
	}

	// Token: 0x06000331 RID: 817 RVA: 0x00013D7A File Offset: 0x00011F7A
	public void ReleaseInteract(Character interactor)
	{
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x06000332 RID: 818 RVA: 0x00013D7C File Offset: 0x00011F7C
	public bool holdOnFinish
	{
		get
		{
			return false;
		}
	}

	// Token: 0x040003BF RID: 959
	private static readonly int Interactable = Shader.PropertyToID("_Interactable");

	// Token: 0x040003C0 RID: 960
	public Character character;

	// Token: 0x040003C1 RID: 961
	public float openRadialMenuTime = 0.25f;

	// Token: 0x040003C2 RID: 962
	private MeshRenderer[] renderers;

	// Token: 0x040003C3 RID: 963
	private Color[] defaultTints;
}
