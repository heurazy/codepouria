using System;
using UnityEngine;

// Token: 0x02000046 RID: 70
public class BingBong : MonoBehaviour
{
	// Token: 0x06000343 RID: 835 RVA: 0x0001425E File Offset: 0x0001245E
	private void Start()
	{
		BingBong.Instance = this;
	}

	// Token: 0x06000344 RID: 836 RVA: 0x00014266 File Offset: 0x00012466
	public void SetVoiceData(float open)
	{
		this.BingBongsVisuals.mouthOpen = open;
	}

	// Token: 0x040003CC RID: 972
	public static BingBong Instance;

	// Token: 0x040003CD RID: 973
	public BingBongsVisuals BingBongsVisuals;
}
