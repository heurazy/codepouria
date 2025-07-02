using System;
using System.Linq;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000200 RID: 512
public class MountainProgressHandler : Singleton<MountainProgressHandler>
{
	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x06000D44 RID: 3396 RVA: 0x00043075 File Offset: 0x00041275
	// (set) Token: 0x06000D45 RID: 3397 RVA: 0x0004307D File Offset: 0x0004127D
	public int maxProgressPointReached { get; private set; }

	// Token: 0x06000D46 RID: 3398 RVA: 0x00043086 File Offset: 0x00041286
	public void SetSegmentComplete(int segment)
	{
		this.progressPoints[segment].Reached = true;
		this.TriggerReached(this.progressPoints[segment]);
		if (segment > this.maxProgressPointReached)
		{
			this.maxProgressPointReached = segment;
		}
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x000430B4 File Offset: 0x000412B4
	private void Update()
	{
		this.CheckProgress(true);
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x000430C0 File Offset: 0x000412C0
	public void CheckProgress(bool playAnimation = true)
	{
		for (int i = 0; i < this.progressPoints.Length; i++)
		{
			if (!this.progressPoints[i].Reached)
			{
				if (this.progressPoints[i].transform != null)
				{
					this.progressPoints[i].Reached = this.CheckReached(this.progressPoints[i].transform);
				}
				if (playAnimation && this.progressPoints[i].Reached)
				{
					this.TriggerReached(this.progressPoints[i]);
				}
			}
		}
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x00043144 File Offset: 0x00041344
	private void TriggerReached(MountainProgressHandler.ProgressPoint progressPoint)
	{
		if (Time.time > 2f)
		{
			this.CheckAreaAchievement(progressPoint);
			GUIManager.instance.SetHeroTitle(progressPoint.title, progressPoint.clip);
		}
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x0004316F File Offset: 0x0004136F
	public bool IsAtPeak(Transform tf)
	{
		return this.IsAtPeak(tf.position);
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x0004317D File Offset: 0x0004137D
	public bool IsAtPeak(Vector3 position)
	{
		return this.progressPoints != null && this.progressPoints.Length != 0 && position.z > this.progressPoints.Last<MountainProgressHandler.ProgressPoint>().transform.position.z;
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x000431B4 File Offset: 0x000413B4
	private bool CheckReached(Transform tf)
	{
		return Character.localCharacter && (Character.localCharacter.Center.z > tf.position.z && !Character.localCharacter.data.dead);
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x000431F4 File Offset: 0x000413F4
	private void CheckAreaAchievement(MountainProgressHandler.ProgressPoint point)
	{
		if (point.achievement != ACHIEVEMENTTYPE.NONE)
		{
			Singleton<AchievementManager>.Instance.ThrowAchievement(point.achievement);
		}
	}

	// Token: 0x04000C75 RID: 3189
	public MountainProgressHandler.ProgressPoint[] progressPoints;

	// Token: 0x02000395 RID: 917
	[Serializable]
	public class ProgressPoint
	{
		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06001464 RID: 5220 RVA: 0x0005FD89 File Offset: 0x0005DF89
		// (set) Token: 0x06001465 RID: 5221 RVA: 0x0005FD91 File Offset: 0x0005DF91
		public bool Reached { get; set; }

		// Token: 0x04001339 RID: 4921
		public Transform transform;

		// Token: 0x0400133A RID: 4922
		public string title;

		// Token: 0x0400133B RID: 4923
		public AudioClip clip;

		// Token: 0x0400133C RID: 4924
		public ACHIEVEMENTTYPE achievement;
	}
}
