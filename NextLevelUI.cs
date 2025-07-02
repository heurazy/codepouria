using System;
using TMPro;
using UnityEngine;

// Token: 0x02000161 RID: 353
public class NextLevelUI : MonoBehaviour
{
	// Token: 0x060009FF RID: 2559 RVA: 0x00031D03 File Offset: 0x0002FF03
	private void Start()
	{
		this.nextLevelService = GameHandler.GetService<NextLevelService>();
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x00031D10 File Offset: 0x0002FF10
	private void Update()
	{
		if (this.nextLevelService.Data.IsSome)
		{
			this.timer.text = this.ParseSeconds(this.nextLevelService.Data.Value.SecondsLeft);
			return;
		}
		this.timer.text = "NO DATA";
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x00031D6C File Offset: 0x0002FF6C
	public string ParseSeconds(int seconds)
	{
		if (seconds < 0)
		{
			return "-- -- --";
		}
		int num = Mathf.FloorToInt((float)seconds / 3600f);
		int num2 = Mathf.FloorToInt((float)(seconds - num * 3600) / 60f);
		float num3 = (float)(seconds - (num * 3600 + num2 * 60));
		return string.Format("{0}h {1}m {2}s", num, num2, num3);
	}

	// Token: 0x040008F1 RID: 2289
	public TextMeshProUGUI timer;

	// Token: 0x040008F2 RID: 2290
	private NextLevelService nextLevelService;
}
