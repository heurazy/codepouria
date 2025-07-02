using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000218 RID: 536
public class PlayerGizmos : MonoBehaviour
{
	// Token: 0x06000DCD RID: 3533 RVA: 0x00045C7D File Offset: 0x00043E7D
	private void Start()
	{
		PlayerGizmos.instance = this;
	}

	// Token: 0x06000DCE RID: 3534 RVA: 0x00045C88 File Offset: 0x00043E88
	private void Update()
	{
		for (int i = this.gizmos.Count - 1; i >= 0; i--)
		{
			GizmoInstance gizmoInstance = this.gizmos[i];
			if (gizmoInstance == null)
			{
				this.gizmos.RemoveAt(i);
			}
			else
			{
				gizmoInstance.framesSinceActivated++;
				if (gizmoInstance.framesSinceActivated > 5)
				{
					gizmoInstance.giz.SetActive(false);
					this.gizmos.Remove(gizmoInstance);
				}
			}
		}
	}

	// Token: 0x06000DCF RID: 3535 RVA: 0x00045CFC File Offset: 0x00043EFC
	public void DisplayGizmo(PlayerGizmos.GizmoType gizmoType, Vector3 pos, Vector3 direction)
	{
		GameObject gizmo = this.GetGizmo(gizmoType);
		GizmoInstance gizmoInstance = this.Contains(gizmo);
		if (gizmoInstance != null)
		{
			gizmoInstance.framesSinceActivated = 0;
		}
		else
		{
			this.gizmos.Add(new GizmoInstance
			{
				giz = gizmo,
				framesSinceActivated = 0
			});
		}
		gizmo.SetActive(true);
		gizmo.transform.position = pos;
		gizmo.transform.rotation = Quaternion.LookRotation(direction);
	}

	// Token: 0x06000DD0 RID: 3536 RVA: 0x00045D67 File Offset: 0x00043F67
	private GameObject GetGizmo(PlayerGizmos.GizmoType gizmoType)
	{
		if (gizmoType == PlayerGizmos.GizmoType.Pointer)
		{
			return this.pointer;
		}
		return null;
	}

	// Token: 0x06000DD1 RID: 3537 RVA: 0x00045D74 File Offset: 0x00043F74
	private GizmoInstance Contains(GameObject gizmo)
	{
		foreach (GizmoInstance gizmoInstance in this.gizmos)
		{
			if (gizmoInstance.giz == gizmo)
			{
				return gizmoInstance;
			}
		}
		return null;
	}

	// Token: 0x04000CE1 RID: 3297
	public List<GizmoInstance> gizmos = new List<GizmoInstance>();

	// Token: 0x04000CE2 RID: 3298
	public static PlayerGizmos instance;

	// Token: 0x04000CE3 RID: 3299
	public GameObject pointer;

	// Token: 0x0200039F RID: 927
	public enum GizmoType
	{
		// Token: 0x04001360 RID: 4960
		Pointer
	}
}
