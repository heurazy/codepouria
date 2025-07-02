using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class SlipperyJellyfish : MonoBehaviour
{
	// Token: 0x06000F3E RID: 3902 RVA: 0x0004CFF5 File Offset: 0x0004B1F5
	private void Start()
	{
		this.relay = base.GetComponentInParent<TriggerRelay>();
	}

	// Token: 0x06000F3F RID: 3903 RVA: 0x0004D003 File Offset: 0x0004B203
	private void Update()
	{
		this.counter += Time.deltaTime;
	}

	// Token: 0x06000F40 RID: 3904 RVA: 0x0004D018 File Offset: 0x0004B218
	public void OnTriggerEnter(Collider other)
	{
		if (this.counter < 3f)
		{
			return;
		}
		Character componentInParent = other.GetComponentInParent<Character>();
		if (!componentInParent)
		{
			return;
		}
		if (!componentInParent.IsLocal)
		{
			return;
		}
		this.counter = 0f;
		this.relay.view.RPC("RPCA_TriggerWithTarget", RpcTarget.All, new object[]
		{
			base.transform.GetSiblingIndex(),
			Character.localCharacter.refs.view.ViewID
		});
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x0004D0A4 File Offset: 0x0004B2A4
	public void Trigger(int targetID)
	{
		Character component = PhotonView.Find(targetID).GetComponent<Character>();
		if (component == null)
		{
			return;
		}
		Rigidbody bodypartRig = component.GetBodypartRig(BodypartType.Foot_R);
		Rigidbody bodypartRig2 = component.GetBodypartRig(BodypartType.Foot_L);
		Rigidbody bodypartRig3 = component.GetBodypartRig(BodypartType.Hip);
		Rigidbody bodypartRig4 = component.GetBodypartRig(BodypartType.Head);
		component.RPCA_Fall(2f);
		bodypartRig.AddForce((component.data.lookDirection_Flat + Vector3.up) * 200f, ForceMode.Impulse);
		bodypartRig2.AddForce((component.data.lookDirection_Flat + Vector3.up) * 200f, ForceMode.Impulse);
		bodypartRig3.AddForce(Vector3.up * 1500f, ForceMode.Impulse);
		bodypartRig4.AddForce(component.data.lookDirection_Flat * -300f, ForceMode.Impulse);
		component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 0.05f, true);
		for (int i = 0; i < this.slipSFX.Length; i++)
		{
			this.slipSFX[i].Play(base.transform.position);
		}
	}

	// Token: 0x04000E27 RID: 3623
	private float counter = 2.5f;

	// Token: 0x04000E28 RID: 3624
	private TriggerRelay relay;

	// Token: 0x04000E29 RID: 3625
	public SFX_Instance[] slipSFX;
}
