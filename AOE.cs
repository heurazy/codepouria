using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000188 RID: 392
public class AOE : MonoBehaviour
{
	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x06000ACF RID: 2767 RVA: 0x00035154 File Offset: 0x00033354
	private bool hasStatus
	{
		get
		{
			return Mathf.Abs(this.statusAmount) > 0f;
		}
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x00035168 File Offset: 0x00033368
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(base.transform.position, this.range);
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x00035180 File Offset: 0x00033380
	private void Start()
	{
		if (this.auto)
		{
			this.Explode();
		}
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x00035190 File Offset: 0x00033390
	public void Explode()
	{
		if (this.range == 0f)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, this.range, HelperFunctions.GetMask(this.mask));
		List<Character> list = new List<Character>();
		for (int i = 0; i < array.Length; i++)
		{
			Character componentInParent = array[i].GetComponentInParent<Character>();
			if (componentInParent != null && !list.Contains(componentInParent))
			{
				float num = Vector3.Distance(base.transform.position, componentInParent.Center);
				if (num <= this.range)
				{
					float factor = this.GetFactor(num);
					if (factor >= this.minFactor)
					{
						list.Add(componentInParent);
						Vector3 normalized = (componentInParent.Center - base.transform.position).normalized;
						if (Mathf.Abs(this.statusAmount) > 0f)
						{
							if (this.illegalStatus != "")
							{
								componentInParent.AddIllegalStatus(this.illegalStatus, this.statusAmount * factor);
							}
							else
							{
								Debug.Log(string.Format("Adding status {0} with amount {1} to player {2}", this.statusType, this.statusAmount * factor, componentInParent.name));
								componentInParent.refs.afflictions.AdjustStatus(this.statusType, this.statusAmount * factor, false);
							}
						}
						componentInParent.AddForce(normalized * factor * this.knockback, 0.7f, 1.3f);
						if (this.fallTime > 0f && componentInParent.IsLocal)
						{
							componentInParent.Fall(factor * this.fallTime);
						}
					}
				}
			}
			else if (this.canLaunchItems)
			{
				Item componentInParent2 = array[i].GetComponentInParent<Item>();
				if (componentInParent2 != null && componentInParent2.photonView.IsMine)
				{
					float num2 = Vector3.Distance(base.transform.position, componentInParent2.Center());
					if (num2 <= this.range)
					{
						float factor2 = this.GetFactor(num2);
						if (factor2 >= this.minFactor)
						{
							Vector3 normalized2 = (componentInParent2.Center() - base.transform.position).normalized;
							componentInParent2.rig.AddForce(normalized2 * factor2 * this.knockback * this.itemKnockbackMultiplier, ForceMode.Impulse);
						}
					}
				}
			}
		}
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x00035405 File Offset: 0x00033605
	private float GetFactor(float dist)
	{
		return Mathf.Pow(1f - dist / this.range, this.factorPow);
	}

	// Token: 0x040009D5 RID: 2517
	public HelperFunctions.LayerType mask;

	// Token: 0x040009D6 RID: 2518
	public bool auto = true;

	// Token: 0x040009D7 RID: 2519
	public float range = 5f;

	// Token: 0x040009D8 RID: 2520
	public float fallTime = 0.5f;

	// Token: 0x040009D9 RID: 2521
	public float knockback = 25f;

	// Token: 0x040009DA RID: 2522
	public float minFactor = 0.2f;

	// Token: 0x040009DB RID: 2523
	public float factorPow = 1f;

	// Token: 0x040009DC RID: 2524
	public bool canLaunchItems;

	// Token: 0x040009DD RID: 2525
	public float itemKnockbackMultiplier = 1f;

	// Token: 0x040009DE RID: 2526
	public float statusAmount;

	// Token: 0x040009DF RID: 2527
	public CharacterAfflictions.STATUSTYPE statusType;

	// Token: 0x040009E0 RID: 2528
	public string illegalStatus = "";
}
