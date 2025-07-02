using System;

// Token: 0x020000E2 RID: 226
public class OnItemStateChangedAction : ItemActionBase
{
	// Token: 0x060006E8 RID: 1768 RVA: 0x00024286 File Offset: 0x00022486
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnStateChange = (Action<ItemState>)Delegate.Combine(item.OnStateChange, new Action<ItemState>(this.RunAction));
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x000242B0 File Offset: 0x000224B0
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnStateChange = (Action<ItemState>)Delegate.Remove(item.OnStateChange, new Action<ItemState>(this.RunAction));
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x000242DA File Offset: 0x000224DA
	public virtual void RunAction(ItemState state)
	{
	}
}
