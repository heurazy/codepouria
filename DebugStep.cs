using System;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class DebugStep : MonoBehaviour
{
	// Token: 0x06000C10 RID: 3088 RVA: 0x0003C7B8 File Offset: 0x0003A9B8
	private void FixedUpdate()
	{
		if (this.stepType == DebugStep.StepType.FixedUpdate)
		{
			Debug.Break();
		}
	}

	// Token: 0x06000C11 RID: 3089 RVA: 0x0003C7C8 File Offset: 0x0003A9C8
	private void Update()
	{
		if (this.stepType == DebugStep.StepType.Update)
		{
			Debug.Break();
		}
	}

	// Token: 0x04000B15 RID: 2837
	public DebugStep.StepType stepType;

	// Token: 0x02000389 RID: 905
	public enum StepType
	{
		// Token: 0x04001314 RID: 4884
		Update,
		// Token: 0x04001315 RID: 4885
		FixedUpdate
	}
}
