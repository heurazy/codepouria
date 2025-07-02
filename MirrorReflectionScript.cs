using System;
using UnityEngine;

// Token: 0x020001FB RID: 507
public class MirrorReflectionScript : MonoBehaviour
{
	// Token: 0x06000D2B RID: 3371 RVA: 0x0004283B File Offset: 0x00040A3B
	private void Start()
	{
		this.childScript = base.gameObject.transform.parent.gameObject.GetComponentInChildren<MirrorCameraScript>();
		if (this.childScript == null)
		{
			Debug.LogError("Child script (MirrorCameraScript) should be in sibling object");
		}
	}

	// Token: 0x06000D2C RID: 3372 RVA: 0x00042875 File Offset: 0x00040A75
	private void OnWillRenderObject()
	{
		this.childScript.RenderMirror();
	}

	// Token: 0x04000C4A RID: 3146
	private MirrorCameraScript childScript;
}
