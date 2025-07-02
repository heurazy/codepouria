using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200007F RID: 127
public class Capybara : MonoBehaviour
{
	// Token: 0x06000485 RID: 1157 RVA: 0x0001A6A7 File Offset: 0x000188A7
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.serenadeDistance);
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x0001A6C9 File Offset: 0x000188C9
	private void OnEnable()
	{
		GlobalEvents.OnBugleTooted = (Action<Item>)Delegate.Combine(GlobalEvents.OnBugleTooted, new Action<Item>(this.TestBugleTooted));
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x0001A6EB File Offset: 0x000188EB
	private void OnDisable()
	{
		GlobalEvents.OnBugleTooted = (Action<Item>)Delegate.Remove(GlobalEvents.OnBugleTooted, new Action<Item>(this.TestBugleTooted));
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x0001A710 File Offset: 0x00018910
	private void TestBugleTooted(Item bugle)
	{
		if (Vector3.Distance(base.transform.position, bugle.transform.position) < this.serenadeDistance && bugle.holderCharacter && bugle.holderCharacter.IsLocal)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(ACHIEVEMENTTYPE.AnimalSerenadingBadge);
		}
	}

	// Token: 0x040004C2 RID: 1218
	public float serenadeDistance;
}
