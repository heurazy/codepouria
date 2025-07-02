using System;
using UnityEngine;

// Token: 0x02000089 RID: 137
public class WaterZone : MonoBehaviour
{
	// Token: 0x060004C7 RID: 1223 RVA: 0x0001BD35 File Offset: 0x00019F35
	private void Awake()
	{
		this.zoneBounds.center = base.transform.position;
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x0001BD4D File Offset: 0x00019F4D
	private void Start()
	{
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x0001BD4F File Offset: 0x00019F4F
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		this.characterInsideBounds = this.zoneBounds.Contains(Character.observedCharacter.Center);
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x0001BD7A File Offset: 0x00019F7A
	private void FixedUpdate()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		if (this.characterInsideBounds && Character.observedCharacter == Character.localCharacter)
		{
			this.AddForceToCharacter();
		}
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x0001BDA9 File Offset: 0x00019FA9
	private void AddForceToCharacter()
	{
		Character.localCharacter.AddForce(-Character.localCharacter.data.worldMovementInput * 0.5f, 1f, 1f);
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x0001BDE0 File Offset: 0x00019FE0
	private void OnDrawGizmosSelected()
	{
		this.zoneBounds.center = base.transform.position;
		Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
		Gizmos.DrawCube(this.zoneBounds.center, this.zoneBounds.extents * 2f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.position, base.transform.position + this.forceDirection * this.Force);
	}

	// Token: 0x04000501 RID: 1281
	public Bounds zoneBounds;

	// Token: 0x04000502 RID: 1282
	public Vector3 forceDirection;

	// Token: 0x04000503 RID: 1283
	[SerializeField]
	private float Force;

	// Token: 0x04000504 RID: 1284
	public bool characterInsideBounds;
}
