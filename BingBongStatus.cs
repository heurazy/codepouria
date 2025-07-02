using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000194 RID: 404
public class BingBongStatus : MonoBehaviour
{
	// Token: 0x06000B10 RID: 2832 RVA: 0x00036B47 File Offset: 0x00034D47
	private void OnEnable()
	{
		this.bingBongPowers = base.GetComponent<BingBongPowers>();
		this.bingBongPowers.SetTexts("STATUS", this.descr);
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x00036B6B File Offset: 0x00034D6B
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x00036B7C File Offset: 0x00034D7C
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			this.allStatusSelected = true;
			this.bingBongPowers.SetTip("Status: All", 2);
		}
		string[] names = Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE));
		int num = names.Length;
		int num2 = (int)this.currentStatusTarget;
		if (Input.GetKeyDown(KeyCode.V))
		{
			this.allStatusSelected = false;
			num2--;
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			this.allStatusSelected = false;
			num2++;
		}
		if (num2 < 0)
		{
			num2 = num - 1;
		}
		if (num2 >= num)
		{
			num2 = 0;
		}
		if (this.currentStatusTarget != (CharacterAfflictions.STATUSTYPE)num2)
		{
			this.currentStatusTarget = (CharacterAfflictions.STATUSTYPE)num2;
			this.bingBongPowers.SetTip(names[num2] ?? "", 2);
		}
		Character target = this.GetTarget();
		if (target)
		{
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				this.view.RPC("RPCA_AddStatusBingBing", RpcTarget.All, new object[]
				{
					target.photonView.ViewID,
					(int)(this.allStatusSelected ? ((CharacterAfflictions.STATUSTYPE)(-1)) : this.currentStatusTarget),
					1
				});
			}
			if (Input.GetKeyDown(KeyCode.Mouse1))
			{
				this.view.RPC("RPCA_AddStatusBingBing", RpcTarget.All, new object[]
				{
					target.photonView.ViewID,
					(int)(this.allStatusSelected ? ((CharacterAfflictions.STATUSTYPE)(-1)) : this.currentStatusTarget),
					-1
				});
			}
		}
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x00036CE8 File Offset: 0x00034EE8
	[PunRPC]
	public void RPCA_AddStatusBingBing(int target, int statusID, int mult)
	{
		Character component = PhotonView.Find(target).GetComponent<Character>();
		if (component.IsLocal)
		{
			if (mult > 0)
			{
				if (statusID == -1)
				{
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, 1f, false);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f, false);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, 1f, false);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Poison, 1f, false);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Curse, 1f, false);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 1f, false);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Weight, 1f, false);
					component.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Hot, 1f, false);
					return;
				}
				component.refs.afflictions.AddStatus(this.currentStatusTarget, 0.2f, false);
				return;
			}
			else
			{
				if (statusID == -1)
				{
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Injury, 1f, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hunger, 1f, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Cold, 1f, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Poison, 1f, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Curse, 1f, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Drowsy, 1f, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Weight, 1f, false);
					component.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Hot, 1f, false);
					return;
				}
				component.refs.afflictions.SubtractStatus(this.currentStatusTarget, 0.2f, false);
			}
		}
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x00036ED8 File Offset: 0x000350D8
	private Character GetTarget()
	{
		Character character = null;
		float num = float.MaxValue;
		foreach (Character character2 in Character.AllCharacters)
		{
			float num2 = Vector3.Angle(MainCamera.instance.transform.forward, character2.Center - MainCamera.instance.transform.position);
			if (num2 < num)
			{
				num = num2;
				character = character2;
			}
		}
		return character;
	}

	// Token: 0x04000A1B RID: 2587
	private BingBongPowers bingBongPowers;

	// Token: 0x04000A1C RID: 2588
	private string descr = "Add status: [LMB]\n\nRemove status: [RMB]\n\nSelect all status: [F]\n\nPrev status: [V]\n\nNext status: [C]\n\n";

	// Token: 0x04000A1D RID: 2589
	private PhotonView view;

	// Token: 0x04000A1E RID: 2590
	private bool allStatusSelected;

	// Token: 0x04000A1F RID: 2591
	private CharacterAfflictions.STATUSTYPE currentStatusTarget;
}
