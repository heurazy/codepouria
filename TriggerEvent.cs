using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200028E RID: 654
public class TriggerEvent : MonoBehaviour
{
	// Token: 0x06000FA8 RID: 4008 RVA: 0x0004F557 File Offset: 0x0004D757
	private void Start()
	{
		this.view = base.GetComponentInParent<PhotonView>();
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x0004F568 File Offset: 0x0004D768
	private void OnTriggerEnter(Collider other)
	{
		TriggerEvent.<>c__DisplayClass7_0 CS$<>8__locals1 = new TriggerEvent.<>c__DisplayClass7_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.player = other.GetComponentInParent<Character>();
		if (!CS$<>8__locals1.player)
		{
			return;
		}
		if (this.hits.Contains(CS$<>8__locals1.player))
		{
			return;
		}
		base.StartCoroutine(CS$<>8__locals1.<OnTriggerEnter>g__IHoldHit|0());
		this.TriggerEntered();
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x0004F5C4 File Offset: 0x0004D7C4
	public void TriggerEntered()
	{
		if (this.onlyOnce && this.hasActivated)
		{
			return;
		}
		if (!this.view.IsMine)
		{
			return;
		}
		if (this.triggerChance < Random.value)
		{
			return;
		}
		this.view.RPC("RPCA_Trigger", RpcTarget.All, new object[] { base.transform.GetSiblingIndex() });
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x0004F628 File Offset: 0x0004D828
	public void Trigger()
	{
		if (this.onlyOnce && this.hasActivated)
		{
			return;
		}
		this.triggerEvent.Invoke();
		this.hasActivated = true;
	}

	// Token: 0x04000EB1 RID: 3761
	[Range(0f, 1f)]
	public float triggerChance = 1f;

	// Token: 0x04000EB2 RID: 3762
	public bool onlyOnce;

	// Token: 0x04000EB3 RID: 3763
	public UnityEvent triggerEvent;

	// Token: 0x04000EB4 RID: 3764
	private PhotonView view;

	// Token: 0x04000EB5 RID: 3765
	private bool hasActivated;

	// Token: 0x04000EB6 RID: 3766
	private List<Character> hits = new List<Character>();
}
