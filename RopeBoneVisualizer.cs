using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000117 RID: 279
public class RopeBoneVisualizer : MonoBehaviour
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x06000828 RID: 2088 RVA: 0x0002B42A File Offset: 0x0002962A
	// (set) Token: 0x06000829 RID: 2089 RVA: 0x0002B432 File Offset: 0x00029632
	public Transform StartTransform { get; set; }

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x0600082A RID: 2090 RVA: 0x0002B43B File Offset: 0x0002963B
	// (set) Token: 0x0600082B RID: 2091 RVA: 0x0002B443 File Offset: 0x00029643
	public Optionable<bool> ManuallyUpdateNextFrame { get; set; }

	// Token: 0x0600082C RID: 2092 RVA: 0x0002B44C File Offset: 0x0002964C
	private void Awake()
	{
		this.view = base.GetComponentInParent<PhotonView>();
		this.rope = base.GetComponent<Rope>();
		this.bones = this.boneRoot.PGetComponentsInChildrenButNotMe(false).ToList<Transform>();
		this.bones.Reverse();
		this.meshRenderer = base.GetComponentInChildren<SkinnedMeshRenderer>();
		this.CheckVisible();
		this.ghostMaterial = Object.Instantiate<Material>(this.ghostMaterial);
		this.ropeMaterial = Object.Instantiate<Material>(this.ropeMaterial);
	}

	// Token: 0x0600082D RID: 2093 RVA: 0x0002B4C7 File Offset: 0x000296C7
	private void OnDestroy()
	{
		Object.Destroy(this.ropeMaterial);
		Object.Destroy(this.ghostMaterial);
	}

	// Token: 0x0600082E RID: 2094 RVA: 0x0002B4E0 File Offset: 0x000296E0
	private void LateUpdate()
	{
		RopeBoneVisualizer.<>c__DisplayClass22_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this.CheckVisible();
		if (this.targetPoints.Count != this.remoteRenderingPoints.Count)
		{
			Debug.LogError("Target points count mismatch");
			return;
		}
		float num = 1f / (float)PhotonNetwork.SerializationRate;
		this.sinceLastPackage += Time.deltaTime;
		if (this.ManuallyUpdateNextFrame.IsSome)
		{
			if (!this.ManuallyUpdateNextFrame.Value)
			{
				return;
			}
			this.ManuallyUpdateNextFrame = Optionable<bool>.Some(false);
		}
		float num2 = this.sinceLastPackage / num;
		for (int i = 0; i < this.remoteRenderingPoints.Count; i++)
		{
			Vector3 vector = Vector3.Lerp(this.remoteRenderingPoints[i].position, this.targetPoints[i].position, num2 * 0.5f);
			Quaternion quaternion = Quaternion.Lerp(this.remoteRenderingPoints[i].rotation, this.targetPoints[i].rotation, num2 * 0.5f);
			this.remoteRenderingPoints[i] = new RopeSyncData.SegmentData
			{
				position = vector,
				rotation = quaternion
			};
		}
		List<RopeSyncData.SegmentData> list;
		if (!this.view.IsMine)
		{
			list = this.remoteRenderingPoints;
		}
		else
		{
			list = (from transform1 in this.rope.GetRopeSegments()
				select new RopeSyncData.SegmentData
				{
					position = transform1.position,
					rotation = transform1.rotation
				}).ToList<RopeSyncData.SegmentData>();
		}
		List<RopeSyncData.SegmentData> list2 = list;
		CS$<>8__locals1.positions = new List<RopeSyncData.SegmentData>();
		if (this.StartTransform != null && this.rope.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			CS$<>8__locals1.positions.Add(new RopeSyncData.SegmentData
			{
				position = this.StartTransform.position,
				rotation = this.StartTransform.rotation
			});
		}
		if (list2.Count > 0)
		{
			CS$<>8__locals1.positions.Add(list2[0]);
		}
		for (int j = list2.Count - 1; j >= 1; j--)
		{
			CS$<>8__locals1.positions.Add(list2[j]);
		}
		CS$<>8__locals1.positions.Reverse();
		this.meshRenderer.sharedMaterial.SetFloat("_RopeCutoff", (Mathf.Floor((float)CS$<>8__locals1.positions.Count) - this.segmentMod) * (1f / (float)(this.bones.Count - 1)));
		if (CS$<>8__locals1.positions.Count == 0)
		{
			return;
		}
		if (this.rope.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			this.<LateUpdate>g__RenderInSpool|22_2(ref CS$<>8__locals1);
		}
		else
		{
			this.<LateUpdate>g__RenderInNotSpool|22_1(ref CS$<>8__locals1);
		}
		this.<LateUpdate>g__RenderInSpool|22_2(ref CS$<>8__locals1);
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x0002B7AC File Offset: 0x000299AC
	public void OnDrawGizmosSelected()
	{
		foreach (Transform transform in this.bones)
		{
			DrawArrow.ForGizmo(transform.position, transform.up, Color.green, 0.25f);
			DrawArrow.ForGizmo(transform.position, transform.forward, Color.blue, 0.25f);
			DrawArrow.ForGizmo(transform.position, transform.right, Color.red, 0.25f);
		}
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x0002B84C File Offset: 0x00029A4C
	private void CheckVisible()
	{
		this.meshRenderer.sharedMaterial = ((this.rope.attachmenState == Rope.ATTACHMENT.inSpool) ? this.ghostMaterial : this.ropeMaterial);
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x0002B878 File Offset: 0x00029A78
	public void SetData(RopeSyncData data)
	{
		if (this.rope.creatorLeft)
		{
			return;
		}
		this.sinceLastPackage = 0f;
		this.targetPoints = data.segments.ToList<RopeSyncData.SegmentData>();
		int num = data.segments.Length;
		int count = this.remoteRenderingPoints.Count;
		if (num < count)
		{
			int num2 = count - num;
			for (int i = 0; i < num2; i++)
			{
				this.remoteRenderingPoints.RemoveLast<RopeSyncData.SegmentData>();
			}
			return;
		}
		if (num > count)
		{
			int num3 = num - count;
			for (int j = 0; j < num3; j++)
			{
				int num4 = count + j;
				this.remoteRenderingPoints.Add(data.segments[num4]);
			}
		}
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x0002B948 File Offset: 0x00029B48
	[CompilerGenerated]
	private void <LateUpdate>g__RenderInNotSpool|22_1(ref RopeBoneVisualizer.<>c__DisplayClass22_0 A_1)
	{
		int num = 0;
		for (int i = 0; i < this.bones.Count; i++)
		{
			Transform transform = this.bones[i];
			if (i > A_1.positions.Count - 1 && i > 0)
			{
				transform.gameObject.name = num.ToString();
				if (num == 0 && this.StartTransform != null)
				{
					transform.position = this.StartTransform.position;
					Vector3 vector = transform.position - this.bones[i - 1].position;
					transform.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up, -vector);
					num++;
				}
				else
				{
					transform.position = this.bones[i - 1].position;
					transform.rotation = this.bones[i - 1].rotation;
					num++;
					transform.localScale = Vector3.zero;
				}
			}
			else
			{
				transform.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up + Vector3.forward * 0.05f, -A_1.positions[i].rotation.GetUp());
				transform.localScale = 1f.xxx();
				transform.position = A_1.positions[i].position.PToV3();
				transform.gameObject.name = i.ToString();
			}
		}
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x0002BACC File Offset: 0x00029CCC
	[CompilerGenerated]
	private void <LateUpdate>g__RenderInSpool|22_2(ref RopeBoneVisualizer.<>c__DisplayClass22_0 A_1)
	{
		int num = 0;
		for (int i = 0; i < this.bones.Count; i++)
		{
			Transform transform = this.bones[i];
			if (i > A_1.positions.Count - 3 && i > 0)
			{
				transform.gameObject.name = num.ToString();
				if (num == 0 && this.StartTransform != null)
				{
					transform.position = this.StartTransform.position;
					if (this.withRotOfStartPos)
					{
						transform.rotation = this.StartTransform.rotation;
					}
					else
					{
						Vector3 vector = transform.position - this.bones[i - 1].position;
						transform.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up, -vector);
					}
					num++;
				}
				else
				{
					transform.position = this.bones[i - 1].position;
					transform.rotation = this.bones[i - 1].rotation;
					num++;
					transform.localScale = Vector3.zero;
				}
			}
			else
			{
				transform.rotation = ExtQuaternion.LookRotationPrioUp(Vector3.up + Vector3.forward * 0.05f, -A_1.positions[i].rotation.GetUp());
				transform.localScale = 1f.xxx();
				transform.position = A_1.positions[i].position.PToV3();
				transform.gameObject.name = i.ToString();
			}
		}
	}

	// Token: 0x040007A6 RID: 1958
	public Material ghostMaterial;

	// Token: 0x040007A7 RID: 1959
	public Material ropeMaterial;

	// Token: 0x040007A8 RID: 1960
	public GameObject boneRoot;

	// Token: 0x040007A9 RID: 1961
	public List<Transform> bones;

	// Token: 0x040007AA RID: 1962
	public float segmentMod = 1f;

	// Token: 0x040007AB RID: 1963
	public bool withRotOfStartPos;

	// Token: 0x040007AC RID: 1964
	private readonly List<RopeSyncData.SegmentData> remoteRenderingPoints = new List<RopeSyncData.SegmentData>();

	// Token: 0x040007AD RID: 1965
	private SkinnedMeshRenderer meshRenderer;

	// Token: 0x040007AE RID: 1966
	private Rope rope;

	// Token: 0x040007AF RID: 1967
	private float sinceLastPackage;

	// Token: 0x040007B0 RID: 1968
	[NonSerialized]
	private List<RopeSyncData.SegmentData> targetPoints = new List<RopeSyncData.SegmentData>();

	// Token: 0x040007B1 RID: 1969
	private PhotonView view;
}
