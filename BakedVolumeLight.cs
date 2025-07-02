using System;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class BakedVolumeLight : MonoBehaviour
{
	// Token: 0x06000706 RID: 1798 RVA: 0x00024CF0 File Offset: 0x00022EF0
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = this.color;
		BakedVolumeLight.LightModes lightModes = this.mode;
		if (lightModes == BakedVolumeLight.LightModes.Point)
		{
			Gizmos.DrawWireSphere(base.transform.position, this.radius);
			return;
		}
		if (lightModes != BakedVolumeLight.LightModes.Spot)
		{
			return;
		}
		Vector3 vector = base.transform.position + base.transform.forward * this.radius;
		Gizmos.DrawLine(base.transform.position, vector);
		float num = this.coneSize * 0.034906585f;
		Vector3[] array = new Vector3[]
		{
			vector + base.transform.up * num * this.radius,
			vector + base.transform.right * num * this.radius,
			vector + -base.transform.up * num * this.radius,
			vector + -base.transform.right * num * this.radius
		};
		foreach (Vector3 vector2 in array)
		{
			Gizmos.DrawLine(base.transform.position, vector2);
		}
		Gizmos.DrawLineStrip(array, true);
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x00024E67 File Offset: 0x00023067
	public void Rebake()
	{
		Object.FindAnyObjectByType<LightVolume>().Bake(null);
	}

	// Token: 0x04000698 RID: 1688
	public BakedVolumeLight.LightModes mode;

	// Token: 0x04000699 RID: 1689
	public Color color = Color.white;

	// Token: 0x0400069A RID: 1690
	public float intensity = 1f;

	// Token: 0x0400069B RID: 1691
	public float radius = 10f;

	// Token: 0x0400069C RID: 1692
	[Range(0f, 1f)]
	public float falloff = 0.5f;

	// Token: 0x0400069D RID: 1693
	[Range(0f, 1f)]
	[Tooltip("Percentage width at which the light should be full brightness. 1.0 means the entire cone is full bright, 0.0 means that the fade lerp starts immediately in the center")]
	public float coneFalloff = 0.9f;

	// Token: 0x0400069E RID: 1694
	[Range(0f, 90f)]
	public float coneSize = 30f;

	// Token: 0x02000331 RID: 817
	public enum LightModes
	{
		// Token: 0x040011B8 RID: 4536
		Point,
		// Token: 0x040011B9 RID: 4537
		Spot
	}
}
