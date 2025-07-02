using System;
using UnityEngine;

// Token: 0x020001EC RID: 492
public class LavaRiverSFXPos : MonoBehaviour
{
	// Token: 0x06000CF7 RID: 3319 RVA: 0x000410A0 File Offset: 0x0003F2A0
	private void Update()
	{
		if (MainCamera.instance)
		{
			base.transform.position = new Vector3(MainCamera.instance.transform.position.x, base.transform.position.y, MainCamera.instance.transform.position.z);
			if (base.transform.position.z < 1050f)
			{
				base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y, 1050f);
			}
		}
	}
}
