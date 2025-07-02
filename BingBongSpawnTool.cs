using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class BingBongSpawnTool : MonoBehaviour
{
	// Token: 0x06000B0B RID: 2827 RVA: 0x00036900 File Offset: 0x00034B00
	private void Update()
	{
		this.counter += Time.unscaledDeltaTime;
		if (this.counter < this.spawnRate)
		{
			return;
		}
		if (!this.auto && !Input.GetKeyDown(KeyCode.Mouse0))
		{
			return;
		}
		if (this.auto && !Input.GetKey(KeyCode.Mouse0))
		{
			return;
		}
		this.counter = 0f;
		this.Spawn();
	}

	// Token: 0x06000B0C RID: 2828 RVA: 0x0003696C File Offset: 0x00034B6C
	private void Spawn()
	{
		Vector3 position = this.GetPosition();
		Quaternion rotation = this.GetRotation();
		GameObject gameObject = PhotonNetwork.Instantiate(this.folder + this.objectToSpawn.name, position, rotation, 0, null);
		if (this.bingbongInit)
		{
			gameObject.GetComponent<PhotonView>().RPC("RPCA_BingBongInitObj", RpcTarget.AllBuffered, new object[] { base.GetComponentInParent<PhotonView>().ViewID });
		}
	}

	// Token: 0x06000B0D RID: 2829 RVA: 0x000369DC File Offset: 0x00034BDC
	public Vector3 GetPosition()
	{
		Vector3 vector = base.transform.position;
		if (this.pos == BingBongSpawnTool.SpawnPos.RaycastPos)
		{
			RaycastHit raycastHit = HelperFunctions.LineCheck(base.transform.position, base.transform.TransformPoint(Vector3.forward * 5000f), HelperFunctions.LayerType.AllPhysical, 0f, QueryTriggerInteraction.Ignore);
			if (raycastHit.transform)
			{
				vector = raycastHit.point;
				vector += raycastHit.normal * this.normalOffsetPos;
			}
		}
		else if (this.pos == BingBongSpawnTool.SpawnPos.BingBong)
		{
			vector = base.transform.TransformPoint(Vector3.forward * 2f);
		}
		return vector + Random.insideUnitSphere * this.randomPosRadius;
	}

	// Token: 0x06000B0E RID: 2830 RVA: 0x00036A9C File Offset: 0x00034C9C
	public Quaternion GetRotation()
	{
		if (this.rot == BingBongSpawnTool.SpawnRot.BingBongRotation)
		{
			return base.transform.rotation;
		}
		if (this.rot == BingBongSpawnTool.SpawnRot.Random)
		{
			return Random.rotation;
		}
		if (this.rot == BingBongSpawnTool.SpawnRot.RaycastNormal)
		{
			return Quaternion.LookRotation(HelperFunctions.LineCheck(base.transform.position, base.transform.TransformPoint(Vector3.forward * 5000f), HelperFunctions.LayerType.AllPhysical, 0f, QueryTriggerInteraction.Ignore).normal);
		}
		BingBongSpawnTool.SpawnRot spawnRot = this.rot;
		return Quaternion.identity;
	}

	// Token: 0x04000A11 RID: 2577
	public float spawnRate = 0.1f;

	// Token: 0x04000A12 RID: 2578
	public bool auto = true;

	// Token: 0x04000A13 RID: 2579
	public string folder = "0_Items/";

	// Token: 0x04000A14 RID: 2580
	public GameObject objectToSpawn;

	// Token: 0x04000A15 RID: 2581
	public bool bingbongInit;

	// Token: 0x04000A16 RID: 2582
	public BingBongSpawnTool.SpawnPos pos;

	// Token: 0x04000A17 RID: 2583
	public BingBongSpawnTool.SpawnRot rot;

	// Token: 0x04000A18 RID: 2584
	public float randomPosRadius;

	// Token: 0x04000A19 RID: 2585
	public float normalOffsetPos;

	// Token: 0x04000A1A RID: 2586
	private float counter;

	// Token: 0x02000383 RID: 899
	public enum SpawnPos
	{
		// Token: 0x040012FC RID: 4860
		BingBong,
		// Token: 0x040012FD RID: 4861
		RaycastPos
	}

	// Token: 0x02000384 RID: 900
	public enum SpawnRot
	{
		// Token: 0x040012FF RID: 4863
		BingBongRotation,
		// Token: 0x04001300 RID: 4864
		Random,
		// Token: 0x04001301 RID: 4865
		RaycastNormal,
		// Token: 0x04001302 RID: 4866
		Identity
	}
}
