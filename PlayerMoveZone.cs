using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000087 RID: 135
public class PlayerMoveZone : MonoBehaviour
{
	// Token: 0x060004AF RID: 1199 RVA: 0x0001B5F0 File Offset: 0x000197F0
	private void Awake()
	{
		this.zoneBounds.center = base.transform.position;
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x0001B608 File Offset: 0x00019808
	private void Start()
	{
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x0001B60A File Offset: 0x0001980A
	private void Update()
	{
		if (Character.observedCharacter == null)
		{
			return;
		}
		this.characterInsideBounds = this.zoneBounds.Contains(Character.observedCharacter.Center);
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x0001B635 File Offset: 0x00019835
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

	// Token: 0x060004B3 RID: 1203 RVA: 0x0001B664 File Offset: 0x00019864
	private void AddForceToCharacter()
	{
		Character.localCharacter.AddForce(this.forceDirection * this.Force, 0.5f, 1f);
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0001B68C File Offset: 0x0001988C
	private void OnDrawGizmosSelected()
	{
		this.zoneBounds.center = base.transform.position;
		Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
		Gizmos.DrawCube(this.zoneBounds.center, this.zoneBounds.extents * 2f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.position, base.transform.position + this.forceDirection * this.Force);
	}

	// Token: 0x040004EF RID: 1263
	[FormerlySerializedAs("windZoneBounds")]
	public Bounds zoneBounds;

	// Token: 0x040004F0 RID: 1264
	public Vector3 forceDirection;

	// Token: 0x040004F1 RID: 1265
	[FormerlySerializedAs("windForce")]
	[SerializeField]
	private float Force;

	// Token: 0x040004F2 RID: 1266
	public bool characterInsideBounds;
}
