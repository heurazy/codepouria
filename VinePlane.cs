using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000297 RID: 663
public class VinePlane : MonoBehaviour
{
	// Token: 0x06000FC7 RID: 4039 RVA: 0x0004FE96 File Offset: 0x0004E096
	private void Start()
	{
		this.UpdateCollider();
	}

	// Token: 0x06000FC8 RID: 4040 RVA: 0x0004FE9E File Offset: 0x0004E09E
	private void OnValidate()
	{
		if (!Application.isPlaying && this.liveEdit)
		{
			this.Blast();
		}
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x0004FEB8 File Offset: 0x0004E0B8
	public void Blast()
	{
		this.meshCollider.enabled = false;
		this.RestoreDefaults();
		for (int i = 0; i < this.bonesParent.childCount; i++)
		{
			Transform child = this.bonesParent.GetChild(i);
			Vector3 vector = child.transform.position + child.transform.up * this.raycastStartLength;
			Vector3 vector2 = child.transform.position - child.transform.up * (this.raycastStartLength + this.raycastEndLength);
			RaycastHit raycastHit;
			if (Physics.Linecast(vector, vector2, out raycastHit, this.mask.value, QueryTriggerInteraction.Ignore))
			{
				if (child.gameObject.activeSelf)
				{
					child.transform.position = raycastHit.point + base.transform.up * this.lift * this.GetDistanceFromCorner(i);
				}
				else
				{
					child.transform.position = raycastHit.point;
				}
				Plane plane = new Plane(base.transform.up, base.transform.position);
				if (child.gameObject.activeSelf)
				{
					float num = Mathf.Pow(Mathf.Clamp01(Mathf.Abs(plane.GetDistanceToPoint(child.transform.position) / this.raycastEndLength)), this.planeLiftPow);
					child.transform.position += base.transform.up * num * this.planeLiftAmount;
				}
			}
		}
		if (!this.liveEdit)
		{
			this.Bake();
			return;
		}
		this.skinnedMeshRenderer.material = this.editingMaterial;
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x00050074 File Offset: 0x0004E274
	public void Bake()
	{
		this.meshCollider.enabled = true;
		this.liveEdit = false;
		this.UpdateCollider();
		if (this.vineType == VinePlane.VineType.Normal)
		{
			this.skinnedMeshRenderer.material = this.vineMatNormal;
		}
		else if (this.vineType == VinePlane.VineType.Thorns)
		{
			this.skinnedMeshRenderer.material = this.vineMatThorns;
		}
		else if (this.vineType == VinePlane.VineType.Poison)
		{
			this.skinnedMeshRenderer.material = this.vineMatPoison;
		}
		this.skinnedMeshRendererLeaves.material = this.skinnedMeshRenderer.material;
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x00050101 File Offset: 0x0004E301
	private float GetDistanceFromCorner(int index)
	{
		return Mathf.InverseLerp(this.distanceToCorner, 0f, Vector3.Distance(this.bonesParent.GetChild(index).position, this.centerBone.position));
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x00050134 File Offset: 0x0004E334
	private void RestoreDefaultsButton()
	{
		this.RestoreDefaults();
		this.Bake();
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x00050144 File Offset: 0x0004E344
	private void RestoreDefaults()
	{
		for (int i = 0; i < this.bonesParent.childCount; i++)
		{
			this.bonesParent.GetChild(i).localPosition = this.defaultPositions[i];
			this.bonesParent.GetChild(i).localRotation = this.defaultRotations[i];
		}
		for (int j = 0; j < this.bonesParent.childCount; j++)
		{
			if (Mathf.Abs(this.bonesParent.GetChild(j).localPosition.y) > 3.9f)
			{
				this.bonesParent.GetChild(j).gameObject.SetActive(false);
			}
			else
			{
				this.bonesParent.GetChild(j).gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x0005020C File Offset: 0x0004E40C
	private void SetDefaultsBECAREFUL()
	{
		this.defaultPositions.Clear();
		this.defaultRotations.Clear();
		for (int i = 0; i < this.bonesParent.childCount; i++)
		{
			this.defaultPositions.Add(this.bonesParent.GetChild(i).localPosition);
			this.defaultRotations.Add(this.bonesParent.GetChild(i).localRotation);
		}
		this.Bake();
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x00050284 File Offset: 0x0004E484
	private void UpdateCollider()
	{
		this.skinnedMeshRenderer.ResetBounds();
		this.bakedMesh = new Mesh();
		this.skinnedMeshRenderer.BakeMesh(this.bakedMesh, true);
		this.meshCollider.sharedMesh = null;
		this.meshCollider.sharedMesh = this.bakedMesh;
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x000502D8 File Offset: 0x0004E4D8
	private void OnDrawGizmos()
	{
		Plane plane = new Plane(base.transform.up, base.transform.position);
		for (int i = 0; i < this.bonesParent.childCount; i++)
		{
			if (this.bonesParent.GetChild(i).gameObject.activeSelf)
			{
				float num = Mathf.Abs(plane.GetDistanceToPoint(this.bonesParent.GetChild(i).transform.position));
				Gizmos.color = new Color(num, num, num);
				Gizmos.DrawSphere(this.bonesParent.GetChild(i).transform.position, 0.1f);
			}
		}
	}

	// Token: 0x04000ED4 RID: 3796
	public SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x04000ED5 RID: 3797
	public SkinnedMeshRenderer skinnedMeshRendererLeaves;

	// Token: 0x04000ED6 RID: 3798
	public MeshCollider meshCollider;

	// Token: 0x04000ED7 RID: 3799
	private Mesh bakedMesh;

	// Token: 0x04000ED8 RID: 3800
	public Transform bonesParent;

	// Token: 0x04000ED9 RID: 3801
	public float raycastStartLength = 1f;

	// Token: 0x04000EDA RID: 3802
	public float raycastEndLength = 5f;

	// Token: 0x04000EDB RID: 3803
	public LayerMask mask;

	// Token: 0x04000EDC RID: 3804
	public float distanceToCorner = 5f;

	// Token: 0x04000EDD RID: 3805
	public Transform centerBone;

	// Token: 0x04000EDE RID: 3806
	public Material vineMatNormal;

	// Token: 0x04000EDF RID: 3807
	public Material vineMatPoison;

	// Token: 0x04000EE0 RID: 3808
	public Material vineMatThorns;

	// Token: 0x04000EE1 RID: 3809
	public Material editingMaterial;

	// Token: 0x04000EE2 RID: 3810
	public VinePlane.VineType vineType;

	// Token: 0x04000EE3 RID: 3811
	public float lift = 0.1f;

	// Token: 0x04000EE4 RID: 3812
	public float planeLiftAmount = 0.5f;

	// Token: 0x04000EE5 RID: 3813
	public float planeLiftPow = 5f;

	// Token: 0x04000EE6 RID: 3814
	public bool liveEdit;

	// Token: 0x04000EE7 RID: 3815
	public List<Vector3> defaultPositions = new List<Vector3>();

	// Token: 0x04000EE8 RID: 3816
	public List<Quaternion> defaultRotations = new List<Quaternion>();

	// Token: 0x020003C1 RID: 961
	public enum VineType
	{
		// Token: 0x040013DC RID: 5084
		Normal,
		// Token: 0x040013DD RID: 5085
		Poison,
		// Token: 0x040013DE RID: 5086
		Thorns
	}
}
