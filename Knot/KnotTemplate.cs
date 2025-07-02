using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using pworld.Scripts.Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Knot
{
	// Token: 0x020002D2 RID: 722
	[ExecuteInEditMode]
	public class KnotTemplate : MonoBehaviour, ISerializationCallbackReceiver
	{
		// Token: 0x060011E0 RID: 4576 RVA: 0x0005810A File Offset: 0x0005630A
		private void Awake()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			this.SplineToLineRenderer();
			this.LineRendererToMeshColliders();
		}

		// Token: 0x060011E1 RID: 4577 RVA: 0x00058120 File Offset: 0x00056320
		private void Update()
		{
		}

		// Token: 0x060011E2 RID: 4578 RVA: 0x00058122 File Offset: 0x00056322
		public void OnBeforeSerialize()
		{
		}

		// Token: 0x060011E3 RID: 4579 RVA: 0x00058124 File Offset: 0x00056324
		public void OnAfterDeserialize()
		{
			this.Register();
		}

		// Token: 0x060011E4 RID: 4580 RVA: 0x0005812C File Offset: 0x0005632C
		public void SplineToLineRenderer()
		{
			if (this.splineContainer == null)
			{
				return;
			}
			List<Vector3> list = new List<Vector3>();
			float num = 1f / (float)this.lr.positionCount;
			List<Keyframe> list2 = new List<Keyframe>();
			this.lr.transform.localPosition = Vector3.zero;
			this.lr.transform.localRotation = Quaternion.identity;
			for (int i = 0; i < this.lr.positionCount; i++)
			{
				float num2 = num * (float)i;
				float3 @float = this.splineContainer.Spline.EvaluatePosition(num2);
				float num3 = @float.z + this.splineContainer.transform.localPosition.z;
				num3 *= num3;
				float magnitude = this.splineContainer.transform.TransformVector(Vector3.one).magnitude;
				num3 *= magnitude;
				@float = this.splineContainer.transform.TransformPoint(@float.PToV3());
				@float = this.lr.transform.InverseTransformPoint(@float);
				list.Add(@float.PToV3().xyn(-num2 * 0.1f));
				list2.Add(new Keyframe(num2, Mathf.Max(this.minWidth, num3 * this.widthMul)));
			}
			this.lr.widthCurve = new AnimationCurve
			{
				keys = list2.ToArray()
			};
			this.lr.SetPositions(list.ToArray());
		}

		// Token: 0x060011E5 RID: 4581 RVA: 0x000582BC File Offset: 0x000564BC
		private void LineRendererToMeshColliders()
		{
			Debug.Log("LineRendererToMeshColliders");
			this.lineMesh = new Mesh();
			this.lr.BakeMesh(this.lineMesh, Camera.main, true);
			this.colliderRoot.KillAllChildren(true, false, false);
			int num = 0;
			while (num < Mathf.FloorToInt((float)this.lineMesh.triangles.Length / 3f) / 2 && num <= this.lineMesh.triangles.Length)
			{
				GameObject gameObject = new GameObject(string.Format("{0}", num));
				List<int> list = new List<int>();
				List<Vector3> list2 = new List<Vector3>();
				gameObject.transform.parent = this.colliderRoot;
				gameObject.transform.localPosition = 0.ToVec();
				for (int i = 0; i < 2; i++)
				{
					int num2 = num * 2 + i;
					for (int j = 0; j < 3; j++)
					{
						Vector3 vector = this.lineMesh.vertices[this.lineMesh.triangles[num2 * 3 + j]];
						list2.Add(vector);
						list.Add(list2.Count - 1);
					}
				}
				Mesh mesh = new Mesh();
				mesh.vertices = list2.ToArray();
				mesh.triangles = list.ToArray();
				mesh.RecalculateAll();
				gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
				num++;
			}
		}

		// Token: 0x060011E6 RID: 4582 RVA: 0x0005841E File Offset: 0x0005661E
		private void Register()
		{
			if (this.registered)
			{
				return;
			}
			this.registered = true;
		}

		// Token: 0x060011E8 RID: 4584 RVA: 0x00058450 File Offset: 0x00056650
		[CompilerGenerated]
		private void <Register>g__EditorSplineUtilityOnAfterSplineWasModified|16_0(Spline spline)
		{
			try
			{
				if (base.gameObject == null)
				{
					return;
				}
			}
			catch (Exception)
			{
				return;
			}
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (spline != this.splineContainer.Spline)
			{
				return;
			}
			this.SplineToLineRenderer();
		}

		// Token: 0x04001038 RID: 4152
		public float widthMul = 0.1f;

		// Token: 0x04001039 RID: 4153
		public float minWidth = 0.001f;

		// Token: 0x0400103A RID: 4154
		public LineRenderer lr;

		// Token: 0x0400103B RID: 4155
		public SplineContainer splineContainer;

		// Token: 0x0400103C RID: 4156
		public Transform colliderRoot;

		// Token: 0x0400103D RID: 4157
		private Mesh lineMesh;

		// Token: 0x0400103E RID: 4158
		private float counter;

		// Token: 0x0400103F RID: 4159
		[NonSerialized]
		private bool registered;

		// Token: 0x04001040 RID: 4160
		public bool editorRefresh;

		// Token: 0x04001041 RID: 4161
		private float timeToRefresh;
	}
}
