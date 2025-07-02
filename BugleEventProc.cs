using System;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class BugleEventProc : MonoBehaviour
{
	// Token: 0x0600064E RID: 1614 RVA: 0x00022208 File Offset: 0x00020408
	private void Awake()
	{
		this.item = base.GetComponent<Item>();
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Combine(item.OnPrimaryStarted, new Action(this.ThrowBugleEvent));
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x0002223D File Offset: 0x0002043D
	private void OnDestroy()
	{
		Item item = this.item;
		item.OnPrimaryStarted = (Action)Delegate.Remove(item.OnPrimaryStarted, new Action(this.ThrowBugleEvent));
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x00022266 File Offset: 0x00020466
	private void ThrowBugleEvent()
	{
		GlobalEvents.TriggerBugleTooted(this.item);
	}

	// Token: 0x04000626 RID: 1574
	private Item item;
}
