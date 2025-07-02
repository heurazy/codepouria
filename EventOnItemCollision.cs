using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200008B RID: 139
public class EventOnItemCollision : MonoBehaviour
{
	// Token: 0x060004DB RID: 1243 RVA: 0x0001C25C File Offset: 0x0001A45C
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x0001C26C File Offset: 0x0001A46C
	private void OnCollisionEnter(Collision collision)
	{
		if (this.onlyOnce && this.triggered)
		{
			return;
		}
		if (this.onlyWhenImKinematic && this.rb != null && !this.rb.isKinematic)
		{
			return;
		}
		Item componentInParent = collision.gameObject.GetComponentInParent<Item>();
		if (componentInParent == null || componentInParent.itemState != ItemState.Ground)
		{
			return;
		}
		Debug.Log(string.Format("{0} collided with {1} at velocity {2}", base.gameObject.name, componentInParent.gameObject.name, collision.relativeVelocity.magnitude));
		if (collision.relativeVelocity.magnitude > this.minCollisionVelocity)
		{
			this.TriggerEvent();
		}
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x0001C320 File Offset: 0x0001A520
	private void TriggerEvent()
	{
		if (this.onlyOnce && this.triggered)
		{
			return;
		}
		this.triggered = true;
		UnityEvent unityEvent = this.eventOnCollided;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04000515 RID: 1301
	public bool onlyWhenImKinematic;

	// Token: 0x04000516 RID: 1302
	public UnityEvent eventOnCollided;

	// Token: 0x04000517 RID: 1303
	private Rigidbody rb;

	// Token: 0x04000518 RID: 1304
	public float minCollisionVelocity;

	// Token: 0x04000519 RID: 1305
	public bool onlyOnce;

	// Token: 0x0400051A RID: 1306
	private bool triggered;
}
