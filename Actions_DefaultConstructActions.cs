using System;
using UnityEngine;

// Token: 0x020000A8 RID: 168
[RequireComponent(typeof(Constructable))]
public class Actions_DefaultConstructActions : ItemActionBase
{
	// Token: 0x060005EC RID: 1516 RVA: 0x00020FF7 File Offset: 0x0001F1F7
	private void Awake()
	{
		this.constructable = base.GetComponent<Constructable>();
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x00021008 File Offset: 0x0001F208
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Combine(item.OnPrimaryStarted, new Action(this.StartConstruction));
		Item item2 = this.item;
		item2.OnPrimaryFinishedCast = (Action)Delegate.Combine(item2.OnPrimaryFinishedCast, new Action(this.RunAction));
		Item item3 = this.item;
		item3.OnPrimaryCancelled = (Action)Delegate.Combine(item3.OnPrimaryCancelled, new Action(this.CancelConstruction));
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00021090 File Offset: 0x0001F290
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Remove(item.OnPrimaryStarted, new Action(this.StartConstruction));
		Item item2 = this.item;
		item2.OnPrimaryFinishedCast = (Action)Delegate.Remove(item2.OnPrimaryFinishedCast, new Action(this.RunAction));
		Item item3 = this.item;
		item3.OnPrimaryCancelled = (Action)Delegate.Remove(item3.OnPrimaryCancelled, new Action(this.CancelConstruction));
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x00021115 File Offset: 0x0001F315
	public virtual void StartConstruction()
	{
		this.constructable.StartConstruction();
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x00021122 File Offset: 0x0001F322
	public virtual void CancelConstruction()
	{
		this.constructable.DestroyPreview();
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x0002112F File Offset: 0x0001F32F
	public override void RunAction()
	{
		this.constructable.FinishConstruction();
	}

	// Token: 0x040005EF RID: 1519
	public Constructable constructable;
}
