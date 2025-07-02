using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x020001EB RID: 491
public class LavaRiver : CustomSpawnCondition
{
	// Token: 0x06000CEB RID: 3307 RVA: 0x0004096A File Offset: 0x0003EB6A
	public override bool CheckCondition(PropSpawner.SpawnData data)
	{
		this.Spawn();
		return true;
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x00040974 File Offset: 0x0003EB74
	private void OnDrawGizmosSelected()
	{
		for (int i = 0; i < this.frames.Count; i++)
		{
			Gizmos.color = Color.Lerp(Color.blue, Color.red, (float)i / (float)this.frames.Count);
			Gizmos.DrawSphere(this.frames[i].position, 0.1f);
			Gizmos.DrawLine(this.frames[i].position, this.frames[i].position + this.frames[i].up * 0.5f);
		}
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x00040A22 File Offset: 0x0003EC22
	public void Spawn()
	{
		this.GenerateData();
		this.Apply();
		this.AddLights();
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x00040A38 File Offset: 0x0003EC38
	private void AddLights()
	{
		Transform transform = base.transform.Find("BakedLight");
		if (transform == null)
		{
			return;
		}
		GameObject gameObject = transform.gameObject;
		Transform transform2 = base.transform.Find("BakedLights");
		for (int i = transform2.childCount - 1; i >= 0; i--)
		{
			Object.DestroyImmediate(transform2.GetChild(i).gameObject);
		}
		for (int j = 0; j < this.frames.Count; j += 3)
		{
			Object.Instantiate<GameObject>(gameObject, this.frames[j].position + this.frames[j].up * 4f, Quaternion.identity, transform2).SetActive(true);
		}
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x00040AFB File Offset: 0x0003ECFB
	private void GenerateData()
	{
		this.Simulate();
		this.Simplify();
		this.SmoothUps();
		this.SmoothUps();
		this.SmoothUps();
		this.SmoothUps();
		this.SmoothUps();
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x00040B28 File Offset: 0x0003ED28
	public void Apply()
	{
		SplineContainer componentInChildren = base.GetComponentInChildren<SplineContainer>();
		componentInChildren.transform.position = Vector3.zero;
		componentInChildren.transform.rotation = Quaternion.identity;
		for (int i = componentInChildren.Splines.Count - 1; i >= 0; i--)
		{
			componentInChildren.RemoveSplineAt(i);
		}
		Spline spline = new Spline();
		foreach (LavaRiver.LavaRiverFrame lavaRiverFrame in this.frames)
		{
			spline.Add(new BezierKnot(lavaRiverFrame.position, Vector3.zero, Vector3.zero, HelperFunctions.GetRotationWithUp(lavaRiverFrame.forward, lavaRiverFrame.up)), TangentMode.AutoSmooth);
		}
		componentInChildren.AddSpline(spline);
		SplineExtrude component = componentInChildren.GetComponent<SplineExtrude>();
		component.GetComponent<MeshFilter>().mesh = new Mesh();
		component.Capped = true;
		component.Rebuild();
		this.endRock.transform.position = this.frames[this.frames.Count - 1].position;
		this.endRock.transform.rotation = Random.rotation;
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x00040C74 File Offset: 0x0003EE74
	private void Simulate()
	{
		LavaRiver.<>c__DisplayClass17_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		this.frames = new List<LavaRiver.LavaRiverFrame>();
		this.steps = this.maxSteps;
		CS$<>8__locals1.vel = base.transform.forward * this.spawnVel;
		CS$<>8__locals1.pos = base.transform.position + base.transform.up * 0.1f + base.transform.forward * 0.1f;
		CS$<>8__locals1.up = base.transform.up;
		CS$<>8__locals1.lastPos = CS$<>8__locals1.pos;
		while (this.<Simulate>g__SimulationStep|17_0(ref CS$<>8__locals1))
		{
		}
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x00040D2C File Offset: 0x0003EF2C
	public void SmoothUps()
	{
		for (int i = 1; i < this.frames.Count - 1; i++)
		{
			LavaRiver.LavaRiverFrame lavaRiverFrame = this.frames[i - 1];
			LavaRiver.LavaRiverFrame lavaRiverFrame2 = this.frames[i];
			LavaRiver.LavaRiverFrame lavaRiverFrame3 = this.frames[i + 1];
			Vector3 normalized = (lavaRiverFrame.up + lavaRiverFrame2.up + lavaRiverFrame3.up).normalized;
			lavaRiverFrame2.up = normalized;
		}
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x00040DA8 File Offset: 0x0003EFA8
	public void Simplify()
	{
		for (int i = 1; i < this.frames.Count; i++)
		{
			LavaRiver.LavaRiverFrame lavaRiverFrame = this.frames[i - 1];
			LavaRiver.LavaRiverFrame lavaRiverFrame2 = this.frames[i];
			if (Vector3.Distance(lavaRiverFrame.position, lavaRiverFrame2.position) < this.prefDistancePerFrame)
			{
				this.frames.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x00040E10 File Offset: 0x0003F010
	public void Clear()
	{
		this.frames.Clear();
		SplineContainer componentInChildren = base.GetComponentInChildren<SplineContainer>();
		componentInChildren.transform.position = Vector3.zero;
		componentInChildren.transform.rotation = Quaternion.identity;
		for (int i = componentInChildren.Splines.Count - 1; i >= 0; i--)
		{
			componentInChildren.RemoveSplineAt(i);
		}
		componentInChildren.GetComponent<SplineExtrude>().Rebuild();
		this.endRock.transform.position = base.transform.position;
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x00040F04 File Offset: 0x0003F104
	[CompilerGenerated]
	private bool <Simulate>g__SimulationStep|17_0(ref LavaRiver.<>c__DisplayClass17_0 A_1)
	{
		this.steps--;
		if (this.steps < 0)
		{
			return false;
		}
		if (A_1.vel.magnitude < 0.01f)
		{
			return false;
		}
		if (Vector3.Distance(base.transform.position, A_1.pos) > this.maxLength)
		{
			return false;
		}
		A_1.vel += Vector3.down * this.gravity * this.timeStep;
		A_1.vel += A_1.up * -this.wallStick * this.timeStep;
		A_1.vel *= this.drag;
		Vector3 vector = A_1.pos + A_1.vel * this.timeStep;
		RaycastHit raycastHit = HelperFunctions.LineCheck(A_1.lastPos, vector, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			A_1.up = raycastHit.normal;
			vector = raycastHit.point + raycastHit.normal * 0.05f;
			A_1.vel = Vector3.ProjectOnPlane(A_1.vel, raycastHit.normal);
		}
		A_1.pos = vector;
		this.frames.Add(new LavaRiver.LavaRiverFrame
		{
			position = A_1.pos,
			up = A_1.up,
			forward = A_1.vel.normalized
		});
		A_1.lastPos = A_1.pos;
		return true;
	}

	// Token: 0x04000BED RID: 3053
	public float spawnVel = 5f;

	// Token: 0x04000BEE RID: 3054
	public float gravity = 10f;

	// Token: 0x04000BEF RID: 3055
	public float wallStick;

	// Token: 0x04000BF0 RID: 3056
	public float drag = 0.8f;

	// Token: 0x04000BF1 RID: 3057
	public float timeStep = 0.02f;

	// Token: 0x04000BF2 RID: 3058
	public int maxSteps = 1000;

	// Token: 0x04000BF3 RID: 3059
	public float maxLength = 500f;

	// Token: 0x04000BF4 RID: 3060
	private int steps;

	// Token: 0x04000BF5 RID: 3061
	public float prefDistancePerFrame = 0.3f;

	// Token: 0x04000BF6 RID: 3062
	public GameObject endRock;

	// Token: 0x04000BF7 RID: 3063
	public List<LavaRiver.LavaRiverFrame> frames = new List<LavaRiver.LavaRiverFrame>();

	// Token: 0x0200038F RID: 911
	[Serializable]
	public class LavaRiverFrame
	{
		// Token: 0x04001324 RID: 4900
		public Vector3 position;

		// Token: 0x04001325 RID: 4901
		public Vector3 up;

		// Token: 0x04001326 RID: 4902
		public Vector3 forward;
	}
}
