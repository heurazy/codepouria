using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000082 RID: 130
public class GiantTreeAchievementZone : MonoBehaviour
{
	// Token: 0x06000498 RID: 1176 RVA: 0x0001AA0B File Offset: 0x00018C0B
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Character") && other.GetComponentInParent<Character>().IsLocal)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.ArboristBadge);
		}
	}
}
