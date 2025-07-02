using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000DF RID: 223
public class MagicBean : ItemComponent
{
	// Token: 0x060006D2 RID: 1746 RVA: 0x00023A44 File Offset: 0x00021C44
	public void Update()
	{
		if (this.photonView.IsMine)
		{
			if (this.item.itemState == ItemState.Held)
			{
				base.GetData<OptionableBoolItemData>(DataEntryKey.Used).HasData = true;
				return;
			}
			if (PhotonNetwork.IsMasterClient && this.isPlanted)
			{
				this.timeToPlant -= Time.deltaTime;
				if (this.timeToPlant <= 0f)
				{
					float vineDistance = this.GetVineDistance(base.transform.position, this.averageNormal);
					this.photonView.RPC("GrowVineRPC", RpcTarget.All, new object[]
					{
						base.transform.position,
						this.averageNormal,
						vineDistance
					});
					this.GrowVineRPC(base.transform.position, this.averageNormal, vineDistance);
					PhotonNetwork.Destroy(base.gameObject);
				}
			}
		}
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x00023B2D File Offset: 0x00021D2D
	private void DebugValue()
	{
		if (base.HasData(DataEntryKey.Used))
		{
			Debug.Log(base.GetData<BoolItemData>(DataEntryKey.Used).Value);
			return;
		}
		Debug.Log("No data");
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x00023B59 File Offset: 0x00021D59
	public override void OnInstanceDataSet()
	{
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x00023B5C File Offset: 0x00021D5C
	private void OnCollisionEnter(Collision collision)
	{
		if (this.photonView.IsMine && this.item.itemState == ItemState.Ground && base.GetData<OptionableBoolItemData>(DataEntryKey.Used).HasData && HelperFunctions.IsLayerInLayerMask(HelperFunctions.LayerType.TerrainMap, collision.gameObject.layer))
		{
			this.item.SetKinematicNetworked(true, this.item.transform.position, this.item.transform.rotation);
			this.DoNormalRaycasts(collision.contacts[0].point, collision.contacts[0].normal);
			this.isPlanted = true;
		}
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x00023C04 File Offset: 0x00021E04
	private float GetVineDistance(Vector3 startPos, Vector3 direction)
	{
		RaycastHit[] array = HelperFunctions.LineCheckAll(startPos, startPos + direction * this.plantPrefab.maxLength, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		float num = this.plantPrefab.maxLength;
		foreach (RaycastHit raycastHit in array)
		{
			if (raycastHit.distance > 0.7f && raycastHit.distance < num)
			{
				num = raycastHit.distance;
			}
		}
		return num;
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x00023C79 File Offset: 0x00021E79
	[PunRPC]
	protected void GrowVineRPC(Vector3 pos, Vector3 direction, float maxLength)
	{
		MagicBeanVine magicBeanVine = Object.Instantiate<MagicBeanVine>(this.plantPrefab, pos, Quaternion.identity);
		magicBeanVine.transform.up = direction;
		magicBeanVine.maxLength = maxLength;
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x00023CA0 File Offset: 0x00021EA0
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		foreach (Vector3 vector in this.raycastSpotsTest)
		{
			Gizmos.DrawSphere(vector, 0.1f);
			Gizmos.DrawLine(base.transform.position, base.transform.position + this.averageNormal);
		}
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00023D28 File Offset: 0x00021F28
	private void TestRaycast()
	{
		this.DoNormalRaycasts(base.transform.position, Vector3.up);
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00023D40 File Offset: 0x00021F40
	private void DoNormalRaycasts(Vector3 centralHit, Vector3 centralNormal)
	{
		this.raycastSpotsTest.Clear();
		List<Vector3> list = new List<Vector3>();
		float num = 0.2f;
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i != 0 || j != 0)
				{
					Vector3 vector = Vector3.ProjectOnPlane(new Vector3((float)i, 0f, (float)j), centralNormal).normalized * num;
					Vector3 vector2 = centralHit + vector + centralNormal;
					this.raycastSpotsTest.Add(vector2);
					this.raycastResult = HelperFunctions.LineCheck(vector2, vector2 - centralNormal * 2f, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
					if (this.raycastResult.collider != null)
					{
						list.Add(this.raycastResult.normal);
					}
				}
			}
			Vector3 vector3 = centralNormal;
			foreach (Vector3 vector4 in list)
			{
				vector3 += vector4;
			}
			this.averageNormal = vector3.normalized;
			if (Vector3.Angle(this.averageNormal, Vector3.up) < this.snapToVerticalAngle)
			{
				this.averageNormal = Vector3.up;
			}
		}
	}

	// Token: 0x04000666 RID: 1638
	private bool isPlanted;

	// Token: 0x04000667 RID: 1639
	public float timeToPlant;

	// Token: 0x04000668 RID: 1640
	public MagicBeanVine plantPrefab;

	// Token: 0x04000669 RID: 1641
	public float snapToVerticalAngle = 15f;

	// Token: 0x0400066A RID: 1642
	private List<Vector3> raycastSpotsTest = new List<Vector3>();

	// Token: 0x0400066B RID: 1643
	private RaycastHit raycastResult;

	// Token: 0x0400066C RID: 1644
	private Vector3 averageNormal;
}
