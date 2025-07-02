using System;
using TMPro;
using UnityEngine;

// Token: 0x0200014F RID: 335
public class DevMessageUI : MonoBehaviour
{
	// Token: 0x06000994 RID: 2452 RVA: 0x000301FF File Offset: 0x0002E3FF
	private void Start()
	{
		this.service = GameHandler.GetService<NextLevelService>();
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x0003020C File Offset: 0x0002E40C
	private void Update()
	{
		bool flag = this.service.Data.IsSome && !string.IsNullOrEmpty(this.service.Data.Value.DevMessage);
		this.parent.SetActive(flag);
		if (flag)
		{
			TextMeshProUGUI[] array = this.texts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].text = this.service.Data.Value.DevMessage;
			}
		}
	}

	// Token: 0x04000877 RID: 2167
	public GameObject parent;

	// Token: 0x04000878 RID: 2168
	public TextMeshProUGUI[] texts;

	// Token: 0x04000879 RID: 2169
	private NextLevelService service;
}
