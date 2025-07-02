using System;

// Token: 0x020000B6 RID: 182
public class Action_GuidebookScroll : ItemActionBase
{
	// Token: 0x06000612 RID: 1554 RVA: 0x00021458 File Offset: 0x0001F658
	private void Awake()
	{
		this.guidebook = base.GetComponent<Guidebook>();
	}

	// Token: 0x06000613 RID: 1555 RVA: 0x00021468 File Offset: 0x0001F668
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Combine(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollButtonLeft = (Action)Delegate.Combine(item2.OnScrollButtonLeft, new Action(this.ScrollLeft));
		Item item3 = this.item;
		item3.OnScrollButtonRight = (Action)Delegate.Combine(item3.OnScrollButtonRight, new Action(this.ScrollRight));
	}

	// Token: 0x06000614 RID: 1556 RVA: 0x000214EC File Offset: 0x0001F6EC
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnScrolledMouseOnly = (Action<float>)Delegate.Remove(item.OnScrolledMouseOnly, new Action<float>(this.Scrolled));
		Item item2 = this.item;
		item2.OnScrollButtonLeft = (Action)Delegate.Remove(item2.OnScrollButtonLeft, new Action(this.ScrollLeft));
		Item item3 = this.item;
		item3.OnScrollButtonRight = (Action)Delegate.Remove(item3.OnScrollButtonRight, new Action(this.ScrollRight));
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x0002156E File Offset: 0x0001F76E
	private void ScrollLeft()
	{
		this.Scrolled(-1f);
	}

	// Token: 0x06000616 RID: 1558 RVA: 0x0002157B File Offset: 0x0001F77B
	private void ScrollRight()
	{
		this.Scrolled(1f);
	}

	// Token: 0x06000617 RID: 1559 RVA: 0x00021588 File Offset: 0x0001F788
	private void Scrolled(float value)
	{
		if (this.guidebook && this.guidebook.isOpen)
		{
			if (value < 0f)
			{
				this.guidebook.FlipPageLeft();
				return;
			}
			if (value > 0f)
			{
				this.guidebook.FlipPageRight();
			}
		}
	}

	// Token: 0x040005FE RID: 1534
	private Guidebook guidebook;
}
