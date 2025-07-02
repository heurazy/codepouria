using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000191 RID: 401
[DefaultExecutionOrder(1000001)]
public class BingBongPhysics : MonoBehaviour
{
	// Token: 0x06000AF9 RID: 2809 RVA: 0x0003649A File Offset: 0x0003469A
	private void OnEnable()
	{
		this.bingBongPowers = base.GetComponent<BingBongPowers>();
		this.bingBongPowers.SetTexts("PHYSICS", this.descr);
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x000364BE File Offset: 0x000346BE
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x000364CC File Offset: 0x000346CC
	private void Update()
	{
		this.CheckInuput();
		float cd = this.GetCD();
		bool auto = this.GetAuto();
		this.counter += Time.unscaledDeltaTime;
		if (this.counter < cd)
		{
			return;
		}
		if (auto && !Input.GetKey(KeyCode.Mouse0))
		{
			return;
		}
		if (!auto && !Input.GetKeyDown(KeyCode.Mouse0))
		{
			return;
		}
		this.DoEffect();
		this.counter = 0f;
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x0003653C File Offset: 0x0003473C
	private void DoEffect()
	{
		PhotonNetwork.Instantiate(this.GetEffect().name, base.transform.position, base.transform.rotation, 0, null).GetComponent<PhotonView>().RPC("RPCA_BingBongInitObj", RpcTarget.All, new object[] { this.view.ViewID });
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0003659C File Offset: 0x0003479C
	private GameObject GetEffect()
	{
		if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
		{
			return this.effect_Blow;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
		{
			return this.effect_Suck;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			return this.effect_Push;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
		{
			return this.effect_Push_Gentle;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForceGrab)
		{
			return this.effect_Grab;
		}
		return null;
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x000365F9 File Offset: 0x000347F9
	private bool GetAuto()
	{
		if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
		{
			return true;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
		{
			return true;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			return false;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
		{
			return false;
		}
		BingBongPhysics.PhysicsType physicsType = this.physicsType;
		return true;
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x00036630 File Offset: 0x00034830
	private float GetCD()
	{
		if (this.physicsType == BingBongPhysics.PhysicsType.Blow)
		{
			return 0.25f;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.Suck)
		{
			return 0.25f;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush)
		{
			return 0f;
		}
		if (this.physicsType == BingBongPhysics.PhysicsType.ForcePush_Gentle)
		{
			return 0f;
		}
		BingBongPhysics.PhysicsType physicsType = this.physicsType;
		return 0.25f;
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x00036688 File Offset: 0x00034888
	private void CheckInuput()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			this.SetState(BingBongPhysics.PhysicsType.Blow);
		}
		if (Input.GetKeyDown(KeyCode.T))
		{
			this.SetState(BingBongPhysics.PhysicsType.Suck);
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			this.SetState(BingBongPhysics.PhysicsType.ForceGrab);
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			this.SetState(BingBongPhysics.PhysicsType.ForcePush);
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			this.SetState(BingBongPhysics.PhysicsType.ForcePush_Gentle);
		}
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x000366E5 File Offset: 0x000348E5
	private void SetState(BingBongPhysics.PhysicsType setType)
	{
		this.physicsType = setType;
		this.bingBongPowers.SetTip(setType.ToString(), 0);
	}

	// Token: 0x04000A04 RID: 2564
	public BingBongPhysics.PhysicsType physicsType;

	// Token: 0x04000A05 RID: 2565
	private PhotonView view;

	// Token: 0x04000A06 RID: 2566
	private BingBongPowers bingBongPowers;

	// Token: 0x04000A07 RID: 2567
	private string descr = "Blow: [R]\n\nSuck: [T]\n\nForce Grab: [F]\n\nForce Push: [C]\n\nForce Push Gentle: [V]";

	// Token: 0x04000A08 RID: 2568
	private float counter;

	// Token: 0x04000A09 RID: 2569
	public GameObject effect_Blow;

	// Token: 0x04000A0A RID: 2570
	public GameObject effect_Suck;

	// Token: 0x04000A0B RID: 2571
	public GameObject effect_Push;

	// Token: 0x04000A0C RID: 2572
	public GameObject effect_Push_Gentle;

	// Token: 0x04000A0D RID: 2573
	public GameObject effect_Grab;

	// Token: 0x02000382 RID: 898
	public enum PhysicsType
	{
		// Token: 0x040012F6 RID: 4854
		Blow,
		// Token: 0x040012F7 RID: 4855
		Suck,
		// Token: 0x040012F8 RID: 4856
		ForcePush,
		// Token: 0x040012F9 RID: 4857
		ForcePush_Gentle,
		// Token: 0x040012FA RID: 4858
		ForceGrab
	}
}
