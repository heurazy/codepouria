using System;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x020001E7 RID: 487
public class SplineKnotPosition : MonoBehaviour
{
	// Token: 0x06000CD8 RID: 3288 RVA: 0x00040048 File Offset: 0x0003E248
	private void Start()
	{
		if (this.splineContainer == null || this.splineContainer.Splines.Count == 0)
		{
			Debug.LogError("SplineContainer is missing or empty.");
			return;
		}
		Spline spline = this.splineContainer.Splines[0];
		float normalizedInterpolation = SplineUtility.GetNormalizedInterpolation<Spline>(this.splineContainer.Spline, this.f, PathIndexUnit.Knot);
		Debug.Log(string.Format("Knot {0} is at {1}% along the spline.", this.knotIndex, normalizedInterpolation * 100f));
	}

	// Token: 0x04000BD0 RID: 3024
	public SplineContainer splineContainer;

	// Token: 0x04000BD1 RID: 3025
	public int knotIndex;

	// Token: 0x04000BD2 RID: 3026
	public float f;
}
