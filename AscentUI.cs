using System;
using TMPro;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000040 RID: 64
public class AscentUI : MonoBehaviour
{
	// Token: 0x0600030F RID: 783 RVA: 0x00013614 File Offset: 0x00011814
	private void Update()
	{
		int currentAscent = Ascents._currentAscent;
		this.text.text = SingletonAsset<AscentData>.Instance.ascents[currentAscent + 1].title;
		if (currentAscent == 0)
		{
			this.text.text = "";
		}
	}

	// Token: 0x040003B2 RID: 946
	public TextMeshProUGUI text;
}
