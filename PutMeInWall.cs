using System;
using System.Linq;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200024F RID: 591
public class PutMeInWall : MonoBehaviour
{
	// Token: 0x06000E66 RID: 3686 RVA: 0x000482E7 File Offset: 0x000464E7
	private void Go()
	{
		this.PutInTheWall();
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x000482F0 File Offset: 0x000464F0
	public bool PutInTheWall()
	{
		Vector3 vector = base.transform.position - Vector3.forward * 50f;
		RaycastHit[] array = Physics.RaycastAll(vector, Vector3.forward, 500f);
		Debug.DrawLine(vector, vector + Vector3.forward * 100f, Color.red, 10f);
		Debug.Log(string.Format("hits: {0}", array.Length));
		Debug.Log(string.Format("list{0}", array));
		array = array.OrderBy((RaycastHit h) => h.distance).ToArray<RaycastHit>();
		RaycastHit raycastHit = array.First((RaycastHit h) => h.collider.gameObject != this.gameObject);
		Vector3 vector2 = raycastHit.point + Vector3.forward * this.penetrationRnage.PRndRange();
		Collider component = base.GetComponent<Collider>();
		if (this.angle > 0f && Vector2.Angle(raycastHit.normal, Vector2.up) <= this.angle)
		{
			return false;
		}
		if (this.checkBelow)
		{
			RaycastHit[] array2 = Physics.SphereCastAll(vector2, component.bounds.extents.magnitude, Vector3.down, component.bounds.extents.magnitude * this.belowMargin);
			Debug.Log(string.Format("belowHits: {0}", array2.Length));
			array2 = array2.Where((RaycastHit hit) => hit.collider.gameObject != this.gameObject && hit.collider.gameObject != raycastHit.collider.gameObject).ToArray<RaycastHit>();
			Debug.Log(string.Format("belowHits2: {0}", array2.Length));
			if (array2.Length != 0)
			{
				foreach (RaycastHit raycastHit2 in array2)
				{
					Debug.Log(string.Format("hit: {0}", raycastHit2.collider.gameObject));
				}
				Debug.DrawLine(vector2, vector2 + Vector3.down * (component.bounds.extents.magnitude * this.belowMargin + component.bounds.extents.magnitude), Color.red, 10f);
				return false;
			}
			Debug.DrawLine(vector2, vector2 + Vector3.down * (component.bounds.extents.magnitude * this.belowMargin + component.bounds.extents.magnitude), Color.green, 10f);
		}
		Debug.Log(raycastHit.collider.gameObject, raycastHit.collider.gameObject);
		base.transform.position = vector2;
		return true;
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x000485E8 File Offset: 0x000467E8
	public Vector3? GetWallPosition2(Vector3 startCast, float maxDistance = 100f)
	{
		Vector3 vector = startCast - Vector3.forward * 50f;
		maxDistance += 50f;
		RaycastHit[] array = Physics.RaycastAll(vector, Vector3.forward, maxDistance);
		Debug.DrawLine(vector, vector + Vector3.forward * maxDistance, Color.red, 10f);
		Debug.Log(string.Format("hits: {0}", array.Length));
		Debug.Log(string.Format("list{0}", array));
		array = array.OrderBy((RaycastHit h) => h.distance).ToArray<RaycastHit>();
		RaycastHit raycastHit = array.First((RaycastHit h) => h.collider.gameObject != this.gameObject);
		Vector3 vector2 = raycastHit.point + Vector3.forward * this.penetrationRnage.PRndRange();
		Collider component = base.GetComponent<Collider>();
		if (this.angle > 0f && Vector2.Angle(raycastHit.normal, Vector2.up) <= this.angle)
		{
			return null;
		}
		if (this.checkBelow)
		{
			if ((from hit in Physics.SphereCastAll(vector2, component.bounds.extents.magnitude, Vector3.down, component.bounds.extents.magnitude * this.belowMargin)
				where hit.collider.gameObject != this.gameObject && hit.collider.gameObject != raycastHit.collider.gameObject
				select hit).ToArray<RaycastHit>().Length != 0)
			{
				Debug.DrawLine(vector2, vector2 + Vector3.down * (component.bounds.extents.magnitude * this.belowMargin + component.bounds.extents.magnitude), Color.red, 10f);
				return null;
			}
			Debug.DrawLine(vector2, vector2 + Vector3.down * (component.bounds.extents.magnitude * this.belowMargin + component.bounds.extents.magnitude), Color.green, 10f);
		}
		Debug.Log(raycastHit.collider.gameObject, raycastHit.collider.gameObject);
		return new Vector3?(vector2);
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x0004886C File Offset: 0x00046A6C
	public Vector3? GetWallPosition(Vector3 startCast, float maxDistance = 100f)
	{
		Vector3 vector = startCast - Vector3.forward * 50f;
		maxDistance += 50f;
		RaycastHit[] array = Physics.RaycastAll(vector, Vector3.forward, maxDistance, HelperFunctions.GetMask(HelperFunctions.LayerType.Terrain));
		if (this.angle > 0f)
		{
			array = array.Where((RaycastHit h) => Vector2.Angle(h.normal, Vector2.up) > this.angle).ToArray<RaycastHit>();
		}
		array = array.OrderBy((RaycastHit h) => h.distance).ToArray<RaycastHit>();
		Debug.DrawLine(vector, vector + Vector3.up * maxDistance, Color.green, 10f);
		Debug.DrawLine(vector, vector + Vector3.forward * maxDistance, Color.red, 10f);
		Debug.Log(string.Format("hits: {0}", array.Length));
		Debug.Log(string.Format("list{0}", array));
		RaycastHit[] array2 = array;
		int num = 0;
		if (num >= array2.Length)
		{
			return null;
		}
		RaycastHit raycastHit = array2[num];
		return new Vector3?(raycastHit.point + Vector3.forward * this.penetrationRnage.PRndRange());
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x000489AC File Offset: 0x00046BAC
	public void RandomRotation()
	{
		base.transform.rotation = Quaternion.Euler((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360));
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x000489E2 File Offset: 0x00046BE2
	public void RandomScale()
	{
		base.transform.localScale *= this.scaleRange.PRndRange();
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x00048A05 File Offset: 0x00046C05
	private void Start()
	{
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x00048A07 File Offset: 0x00046C07
	private void Update()
	{
	}

	// Token: 0x04000D6A RID: 3434
	public Vector2 penetrationRnage;

	// Token: 0x04000D6B RID: 3435
	public Vector2 scaleRange = new Vector2(1f, 1f);

	// Token: 0x04000D6C RID: 3436
	public bool checkBelow;

	// Token: 0x04000D6D RID: 3437
	public float belowMargin = 1f;

	// Token: 0x04000D6E RID: 3438
	public float angle = -1f;
}
