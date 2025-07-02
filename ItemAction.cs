using System;
using UnityEngine;

// Token: 0x020000DA RID: 218
public class ItemAction : ItemActionBase
{
	// Token: 0x060006A5 RID: 1701 RVA: 0x000232DC File Offset: 0x000214DC
	protected override void Subscribe()
	{
		if (this.OnPressed)
		{
			Item item = this.item;
			item.OnPrimaryStarted = (Action)Delegate.Combine(item.OnPrimaryStarted, new Action(this.RunAction));
		}
		if (this.OnHeld)
		{
			Item item2 = this.item;
			item2.OnPrimaryHeld = (Action)Delegate.Combine(item2.OnPrimaryHeld, new Action(this.RunAction));
		}
		if (this.OnCastFinished)
		{
			Item item3 = this.item;
			item3.OnPrimaryFinishedCast = (Action)Delegate.Combine(item3.OnPrimaryFinishedCast, new Action(this.RunAction));
		}
		if (this.OnCancelled)
		{
			Item item4 = this.item;
			item4.OnPrimaryCancelled = (Action)Delegate.Combine(item4.OnPrimaryCancelled, new Action(this.RunAction));
		}
		if (this.OnConsumed)
		{
			Item item5 = this.item;
			item5.OnConsumed = (Action)Delegate.Combine(item5.OnConsumed, new Action(this.RunAction));
		}
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x000233DC File Offset: 0x000215DC
	protected override void Unsubscribe()
	{
		if (this.OnPressed)
		{
			Item item = this.item;
			item.OnPrimaryStarted = (Action)Delegate.Remove(item.OnPrimaryStarted, new Action(this.RunAction));
		}
		if (this.OnHeld)
		{
			Item item2 = this.item;
			item2.OnPrimaryHeld = (Action)Delegate.Remove(item2.OnPrimaryHeld, new Action(this.RunAction));
		}
		if (this.OnCastFinished)
		{
			Item item3 = this.item;
			item3.OnPrimaryFinishedCast = (Action)Delegate.Remove(item3.OnPrimaryFinishedCast, new Action(this.RunAction));
		}
		if (this.OnCancelled)
		{
			Item item4 = this.item;
			item4.OnPrimaryCancelled = (Action)Delegate.Remove(item4.OnPrimaryCancelled, new Action(this.RunAction));
		}
		if (this.OnConsumed)
		{
			Item item5 = this.item;
			item5.OnConsumed = (Action)Delegate.Remove(item5.OnConsumed, new Action(this.RunAction));
		}
	}

	// Token: 0x04000652 RID: 1618
	[SerializeField]
	public bool OnPressed;

	// Token: 0x04000653 RID: 1619
	[SerializeField]
	public bool OnHeld;

	// Token: 0x04000654 RID: 1620
	[SerializeField]
	public bool OnReleased;

	// Token: 0x04000655 RID: 1621
	[SerializeField]
	public bool OnCastFinished;

	// Token: 0x04000656 RID: 1622
	[SerializeField]
	public bool OnCancelled;

	// Token: 0x04000657 RID: 1623
	public bool OnConsumed;
}
