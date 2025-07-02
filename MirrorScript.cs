using System;
using UnityEngine;

// Token: 0x020001FC RID: 508
public class MirrorScript : MonoBehaviour
{
	// Token: 0x04000C4B RID: 3147
	[Tooltip("Maximum number of per pixel lights that will show in the mirrored image")]
	public int MaximumPerPixelLights = 2;

	// Token: 0x04000C4C RID: 3148
	[Tooltip("Texture size for the mirror, depending on how close the player can get to the mirror, this will need to be larger")]
	public int TextureSize = 768;

	// Token: 0x04000C4D RID: 3149
	[Tooltip("Subtracted from the near plane of the mirror")]
	public float ClipPlaneOffset = 0.07f;

	// Token: 0x04000C4E RID: 3150
	[Tooltip("Far clip plane for mirro camera")]
	public float FarClipPlane = 1000f;

	// Token: 0x04000C4F RID: 3151
	[Tooltip("What layers will be reflected?")]
	public LayerMask ReflectLayers = -1;

	// Token: 0x04000C50 RID: 3152
	[Tooltip("Add a flare layer to the reflection camera?")]
	public bool AddFlareLayer;

	// Token: 0x04000C51 RID: 3153
	[Tooltip("For quads, the normal points forward (true). For planes, the normal points up (false)")]
	public bool NormalIsForward = true;

	// Token: 0x04000C52 RID: 3154
	[Tooltip("Aspect ratio (width / height). Set to 0 to use default.")]
	public float AspectRatio;

	// Token: 0x04000C53 RID: 3155
	[Tooltip("Set to true if you have multiple mirrors facing each other to get an infinite effect, otherwise leave as false for a more realistic mirror effect.")]
	public bool MirrorRecursion;
}
