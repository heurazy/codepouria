using System;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200004F RID: 79
public class PatrolBoss : MonoBehaviour
{
	// Token: 0x0600037E RID: 894 RVA: 0x000152C7 File Offset: 0x000134C7
	public void Awake()
	{
		PatrolBoss.me = this;
	}

	// Token: 0x0600037F RID: 895 RVA: 0x000152D0 File Offset: 0x000134D0
	public Vector3 GetPoint()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(this.point.transform.position + ExtMath.RandInsideUnitCircle().xoy() * 10f, Vector3.down, out raycastHit, 1000f, HelperFunctions.GetMask(HelperFunctions.LayerType.TerrainMap)))
		{
			return raycastHit.point;
		}
		Debug.LogError("This wrong");
		return Vector3.positiveInfinity;
	}

	// Token: 0x04000405 RID: 1029
	public static PatrolBoss me;

	// Token: 0x04000406 RID: 1030
	public GameObject point;
}
