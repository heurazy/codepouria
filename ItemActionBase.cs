using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000D9 RID: 217
public class ItemActionBase : MonoBehaviourPun
{
	// Token: 0x1700005A RID: 90
	// (get) Token: 0x0600069C RID: 1692 RVA: 0x0002328F File Offset: 0x0002148F
	[SerializeField]
	protected Character character
	{
		get
		{
			return this.item.holderCharacter;
		}
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x0002329C File Offset: 0x0002149C
	public virtual void RunAction()
	{
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x0002329E File Offset: 0x0002149E
	protected virtual void OnEnable()
	{
		this.Init();
		this.Subscribe();
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x000232AC File Offset: 0x000214AC
	protected virtual void Start()
	{
		this.Unsubscribe();
		this.Subscribe();
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x000232BA File Offset: 0x000214BA
	public void OnDisable()
	{
		this.Unsubscribe();
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x000232C2 File Offset: 0x000214C2
	protected virtual void Subscribe()
	{
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x000232C4 File Offset: 0x000214C4
	protected virtual void Unsubscribe()
	{
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x000232C6 File Offset: 0x000214C6
	private void Init()
	{
		this.item = base.GetComponent<Item>();
	}

	// Token: 0x04000651 RID: 1617
	protected Item item;
}
