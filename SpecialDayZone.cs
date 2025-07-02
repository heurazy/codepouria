using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000278 RID: 632
public class SpecialDayZone : MonoBehaviour
{
	// Token: 0x06000F46 RID: 3910 RVA: 0x0004D200 File Offset: 0x0004B400
	private void Start()
	{
		this.bounds.center = base.transform.position;
		this.outerBounds.center = base.transform.position;
		this.outerBounds.size = this.bounds.size + Vector3.one * this.blendSize;
		if (this.specialLight)
		{
			this.specialLight.color = Color.black;
		}
	}

	// Token: 0x06000F47 RID: 3911 RVA: 0x0004D284 File Offset: 0x0004B484
	private void OnDrawGizmosSelected()
	{
		this.bounds.center = base.transform.position;
		this.outerBounds.center = base.transform.position;
		Gizmos.DrawWireCube(this.bounds.center, this.bounds.size);
		Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
		this.outerBounds.size = this.bounds.size + Vector3.one * this.blendSize;
		Gizmos.DrawWireCube(this.outerBounds.center, this.outerBounds.size);
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x0004D33C File Offset: 0x0004B53C
	private void Update()
	{
		if (!Character.localCharacter)
		{
			return;
		}
		if (this.outerBounds.Contains(Character.localCharacter.Center))
		{
			this.inBounds = true;
			return;
		}
		this.inBounds = false;
	}

	// Token: 0x04000E2C RID: 3628
	public Light specialLight;

	// Token: 0x04000E2D RID: 3629
	public Color specialSunColor;

	// Token: 0x04000E2E RID: 3630
	public Color specialTopColor;

	// Token: 0x04000E2F RID: 3631
	public Color specialMidColor;

	// Token: 0x04000E30 RID: 3632
	public Color specialBottomColor;

	// Token: 0x04000E31 RID: 3633
	[FormerlySerializedAs("shaderValsToBlend")]
	public ShaderVal[] globalShaderVals;

	// Token: 0x04000E32 RID: 3634
	private float baseFog;

	// Token: 0x04000E33 RID: 3635
	public float fogDensity = 400f;

	// Token: 0x04000E34 RID: 3636
	public Bounds bounds;

	// Token: 0x04000E35 RID: 3637
	public Bounds outerBounds;

	// Token: 0x04000E36 RID: 3638
	public float blendSize = 50f;

	// Token: 0x04000E37 RID: 3639
	[Header("Debug")]
	public bool inBounds;
}
