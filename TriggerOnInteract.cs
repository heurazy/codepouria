using System;
using UnityEngine;

// Token: 0x02000081 RID: 129
public class TriggerOnInteract : MonoBehaviour, IInteractible
{
	// Token: 0x0600048E RID: 1166 RVA: 0x0001A962 File Offset: 0x00018B62
	private void Awake()
	{
		this.mpb = new MaterialPropertyBlock();
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x0001A96F File Offset: 0x00018B6F
	public bool IsInteractible(Character interactor)
	{
		return true;
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x0001A972 File Offset: 0x00018B72
	public void Interact(Character interactor)
	{
		this.triggerEvent.TriggerEntered();
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x0001A97F File Offset: 0x00018B7F
	public void HoverEnter()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 1f);
			base.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(this.mpb);
		}
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x0001A9AF File Offset: 0x00018BAF
	public void HoverExit()
	{
		if (this.mpb != null)
		{
			this.mpb.SetFloat(Item.PROPERTY_INTERACTABLE, 0f);
			base.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(this.mpb);
		}
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x0001A9DF File Offset: 0x00018BDF
	public Vector3 Center()
	{
		return base.transform.position;
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x0001A9EC File Offset: 0x00018BEC
	public Transform GetTransform()
	{
		return base.transform;
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x0001A9F4 File Offset: 0x00018BF4
	public string GetInteractionText()
	{
		return "pick up";
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x0001A9FB File Offset: 0x00018BFB
	public string GetName()
	{
		return this.interactableName;
	}

	// Token: 0x040004CB RID: 1227
	private MaterialPropertyBlock mpb;

	// Token: 0x040004CC RID: 1228
	public string interactText;

	// Token: 0x040004CD RID: 1229
	public TriggerEvent triggerEvent;

	// Token: 0x040004CE RID: 1230
	public string interactableName;
}
