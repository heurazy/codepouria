using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000AC RID: 172
public class Action_ApplyMassAffliction : Action_ApplyAffliction
{
	// Token: 0x060005FA RID: 1530 RVA: 0x000211C9 File Offset: 0x0001F3C9
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x000211EB File Offset: 0x0001F3EB
	public override void RunAction()
	{
		if (this.affliction == null)
		{
			Debug.LogError("Your affliction is null bro");
			return;
		}
		this.item.photonView.RPC("TryAddAfflictionToLocalCharacter", RpcTarget.All, Array.Empty<object>());
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x0002121C File Offset: 0x0001F41C
	[PunRPC]
	public void TryAddAfflictionToLocalCharacter()
	{
		if (this.ignoreCaster && this.item.holderCharacter == Character.localCharacter)
		{
			return;
		}
		if (Vector3.Distance(Character.localCharacter.Center, base.transform.position) <= this.radius)
		{
			Character.localCharacter.refs.afflictions.AddAffliction(this.affliction, false);
		}
	}

	// Token: 0x040005F3 RID: 1523
	public float radius;

	// Token: 0x040005F4 RID: 1524
	public bool ignoreCaster;
}
