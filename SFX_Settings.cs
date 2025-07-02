using System;
using UnityEngine;

// Token: 0x02000139 RID: 313
[Serializable]
public class SFX_Settings
{
	// Token: 0x0400080D RID: 2061
	[Range(0f, 1f)]
	public float volume = 0.5f;

	// Token: 0x0400080E RID: 2062
	[Range(0f, 1f)]
	[Tooltip("0.2 variation means random between 80% of specified volume and 100% of specified volume")]
	public float volume_Variation = 0.2f;

	// Token: 0x0400080F RID: 2063
	public float pitch = 1f;

	// Token: 0x04000810 RID: 2064
	[Range(0f, 1f)]
	[Tooltip("0.1 variation means random between 95% of specified volume and 105% of specified volume")]
	public float pitch_Variation = 0.1f;

	// Token: 0x04000811 RID: 2065
	[Range(0f, 1f)]
	public float spatialBlend = 1f;

	// Token: 0x04000812 RID: 2066
	[Range(0f, 1f)]
	public float dopplerLevel = 1f;

	// Token: 0x04000813 RID: 2067
	public float range = 150f;

	// Token: 0x04000814 RID: 2068
	public float cooldown = 0.02f;

	// Token: 0x04000815 RID: 2069
	public int maxInstances_NOT_IMPLEMENTED = 5;
}
