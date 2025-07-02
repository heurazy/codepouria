using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200029E RID: 670
public class WarpCompassVFX : ItemVFX
{
	// Token: 0x06000FF8 RID: 4088 RVA: 0x00051049 File Offset: 0x0004F249
	private new void Start()
	{
		base.Start();
		GameUtils instance = GameUtils.instance;
		instance.OnUpdatedFeedData = (Action)Delegate.Combine(instance.OnUpdatedFeedData, new Action(this.OnUpdatedFeedData));
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x00051077 File Offset: 0x0004F277
	private void OnDestroy()
	{
		GameUtils instance = GameUtils.instance;
		instance.OnUpdatedFeedData = (Action)Delegate.Remove(instance.OnUpdatedFeedData, new Action(this.OnUpdatedFeedData));
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x000510A0 File Offset: 0x0004F2A0
	protected override void Update()
	{
		base.Update();
		float num = this.item.castProgress;
		if ((!this.item.isUsingPrimary || this.item.finishedCast) && this.timeStartedBeingUsedOnMe == 0f)
		{
			num = 0f;
		}
		else if (this.timeStartedBeingUsedOnMe > 0f)
		{
			num = (Time.time - this.timeStartedBeingUsedOnMe) / this.item.totalSecondaryUsingTime;
		}
		this.warpPost.enabled = this.warpPost.weight > 0.01f;
		this.warpPost2.enabled = this.warpPost2.weight > 0.01f;
		if (this.warpPost2.weight >= 1f)
		{
			this.warpPost.weight = 0f;
		}
		else
		{
			this.warpPost.weight = Mathf.Lerp(this.warpPost.weight, num, Time.deltaTime * 10f);
		}
		this.warpPost2.weight = this.warpPost2Curve.Evaluate(this.warpPost.weight);
		this.compassPointer.speedMultiplier = 1f + this.warpPost.weight * 4f;
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x000511DC File Offset: 0x0004F3DC
	protected override void Shake()
	{
		GamefeelHandler.instance.AddPerlinShake(this.warpPost.weight * this.shakeAmount * Time.deltaTime * 100f, 0.2f, 15f);
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x00051210 File Offset: 0x0004F410
	private void OnUpdatedFeedData()
	{
		bool flag = false;
		using (List<FeedData>.Enumerator enumerator = GameUtils.instance.GetFeedDataForReceiver(Character.localCharacter.photonView.ViewID).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.itemID == this.item.itemID)
				{
					flag = true;
					if (this.timeStartedBeingUsedOnMe == 0f)
					{
						this.timeStartedBeingUsedOnMe = Time.time;
					}
				}
			}
		}
		if (!flag)
		{
			this.timeStartedBeingUsedOnMe = 0f;
		}
	}

	// Token: 0x04000F03 RID: 3843
	public Volume warpPost;

	// Token: 0x04000F04 RID: 3844
	public Volume warpPost2;

	// Token: 0x04000F05 RID: 3845
	public float maxCastProgress = 1.1f;

	// Token: 0x04000F06 RID: 3846
	public AnimationCurve warpPost2Curve;

	// Token: 0x04000F07 RID: 3847
	public CompassPointer compassPointer;

	// Token: 0x04000F08 RID: 3848
	public float timeStartedBeingUsedOnMe;
}
