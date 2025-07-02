using System;

// Token: 0x020000A7 RID: 167
public class Actions_Binoculars : ItemActionBase
{
	// Token: 0x060005E8 RID: 1512 RVA: 0x00020F7C File Offset: 0x0001F17C
	protected override void Subscribe()
	{
		Item item = this.item;
		item.OnScrolled = (Action<float>)Delegate.Combine(item.OnScrolled, new Action<float>(this.Scrolled));
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x00020FA5 File Offset: 0x0001F1A5
	protected override void Unsubscribe()
	{
		Item item = this.item;
		item.OnScrolled = (Action<float>)Delegate.Remove(item.OnScrolled, new Action<float>(this.Scrolled));
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x00020FCE File Offset: 0x0001F1CE
	private void Scrolled(float value)
	{
		this.cameraOverride.AdjustFOV(-value * this.scrollSpeed);
	}

	// Token: 0x040005ED RID: 1517
	public CameraOverride_Binoculars cameraOverride;

	// Token: 0x040005EE RID: 1518
	public float scrollSpeed = 2f;
}
