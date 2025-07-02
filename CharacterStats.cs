using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000010 RID: 16
public class CharacterStats : MonoBehaviour
{
	// Token: 0x06000164 RID: 356 RVA: 0x0000BC8D File Offset: 0x00009E8D
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
	}

	// Token: 0x06000165 RID: 357 RVA: 0x0000BC9B File Offset: 0x00009E9B
	private void Start()
	{
		this.RecordHeight();
		this.Record(false, 0f);
	}

	// Token: 0x06000166 RID: 358 RVA: 0x0000BCB0 File Offset: 0x00009EB0
	public void GetCaughtUp()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		List<EndScreen.TimelineInfo> list = Character.localCharacter.refs.stats.timelineInfo;
		for (int i = 0; i < list.Count; i++)
		{
			EndScreen.TimelineInfo timelineInfo = default(EndScreen.TimelineInfo);
			timelineInfo.time = list[i].time;
			timelineInfo.height = this.heightInUnits;
			this.timelineInfo.Add(timelineInfo);
		}
	}

	// Token: 0x06000167 RID: 359 RVA: 0x0000BD28 File Offset: 0x00009F28
	private void RecordHeight()
	{
		this.heightInUnits = this.character.HipPos().y;
		this.heightInMeters = (float)Mathf.RoundToInt(this.heightInUnits * CharacterStats.unitsToMeters);
		if (this.character.IsLocal && !this.character.data.dead)
		{
			Singleton<AchievementManager>.Instance.RecordMaxHeight(Mathf.RoundToInt(this.heightInMeters));
		}
	}

	// Token: 0x06000168 RID: 360 RVA: 0x0000BD98 File Offset: 0x00009F98
	private void Update()
	{
		this.RecordHeight();
		this.tick += Time.deltaTime;
		if (this.tick > this.tickRate && !this.won && !this.lost)
		{
			this.tick = 0f;
			if (!this.character.IsLocal && this.timelineInfo.Count == 1)
			{
				this.GetCaughtUp();
			}
			this.Record(false, 0f);
		}
	}

	// Token: 0x06000169 RID: 361 RVA: 0x0000BE13 File Offset: 0x0000A013
	public EndScreen.TimelineInfo GetFirstTimelineInfo()
	{
		return this.timelineInfo[0];
	}

	// Token: 0x0600016A RID: 362 RVA: 0x0000BE21 File Offset: 0x0000A021
	public EndScreen.TimelineInfo GetFinalTimelineInfo()
	{
		return this.timelineInfo[this.timelineInfo.Count - 1];
	}

	// Token: 0x0600016B RID: 363 RVA: 0x0000BE3B File Offset: 0x0000A03B
	public static int UnitsToMeters(float units)
	{
		return Mathf.RoundToInt(Mathf.Min(units, CharacterStats.peakHeightInUnits) * CharacterStats.unitsToMeters);
	}

	// Token: 0x0600016C RID: 364 RVA: 0x0000BE54 File Offset: 0x0000A054
	public void Record(bool useOverridePosition = false, float overrideHeight = 0f)
	{
		EndScreen.TimelineInfo timelineInfo = default(EndScreen.TimelineInfo);
		timelineInfo.height = this.heightInUnits;
		if (useOverridePosition)
		{
			timelineInfo.height = overrideHeight;
		}
		if (timelineInfo.height > 2000f)
		{
			return;
		}
		timelineInfo.time = Time.time;
		if (this.justDied)
		{
			this.justDied = false;
			timelineInfo.died = true;
		}
		else if (this.character.data.dead)
		{
			timelineInfo.dead = true;
		}
		if (this.justRevived)
		{
			this.justRevived = false;
			timelineInfo.revived = true;
			Debug.LogError("RECORD REVIVED!");
		}
		else
		{
			if (this.justPassedOut)
			{
				this.justPassedOut = false;
				timelineInfo.justPassedOut = true;
			}
			if (this.character.data.passedOut)
			{
				timelineInfo.passedOut = true;
			}
		}
		this.timelineInfo.Add(timelineInfo);
	}

	// Token: 0x0600016D RID: 365 RVA: 0x0000BF30 File Offset: 0x0000A130
	public void Win()
	{
		this.won = true;
		if (this.character.IsLocal)
		{
			EndScreen.TimelineInfo timelineInfo = this.timelineInfo[this.timelineInfo.Count - 1];
			timelineInfo.won = true;
			GlobalEvents.TriggerLocalCharacterWonRun();
			this.timelineInfo[this.timelineInfo.Count - 1] = timelineInfo;
		}
	}

	// Token: 0x0600016E RID: 366 RVA: 0x0000BF90 File Offset: 0x0000A190
	public void Lose(bool somebodyElseWon)
	{
		this.lost = true;
		this.somebodyElseWon = somebodyElseWon;
	}

	// Token: 0x0400015B RID: 347
	public static float peakHeightInUnits = 1200f;

	// Token: 0x0400015C RID: 348
	private Character character;

	// Token: 0x0400015D RID: 349
	public float heightInUnits;

	// Token: 0x0400015E RID: 350
	public float heightInMeters;

	// Token: 0x0400015F RID: 351
	public static float unitsToMeters = 1.6f;

	// Token: 0x04000160 RID: 352
	private float tick;

	// Token: 0x04000161 RID: 353
	public float tickRate = 1f;

	// Token: 0x04000162 RID: 354
	public List<EndScreen.TimelineInfo> timelineInfo = new List<EndScreen.TimelineInfo>();

	// Token: 0x04000163 RID: 355
	public bool won;

	// Token: 0x04000164 RID: 356
	public bool lost;

	// Token: 0x04000165 RID: 357
	public bool somebodyElseWon;

	// Token: 0x04000166 RID: 358
	public bool justDied;

	// Token: 0x04000167 RID: 359
	public bool justPassedOut;

	// Token: 0x04000168 RID: 360
	public bool justRevived;
}
