using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000155 RID: 341
public class EndScreenScoutWindow : MonoBehaviour
{
	// Token: 0x060009C6 RID: 2502 RVA: 0x00030BA0 File Offset: 0x0002EDA0
	public void Init(Character character)
	{
		if (character != null)
		{
			if (character.IsLocal)
			{
				this.scoutName.fontStyle = FontStyles.Underline;
			}
			this.scoutName.text = character.characterName;
			Color playerColor = character.refs.customization.PlayerColor;
			playerColor.a = this.panelAlpha;
			this.panel.color = playerColor;
			this.altitude.text = "0m";
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x00030C22 File Offset: 0x0002EE22
	public void UpdateAltitude(int m)
	{
		this.altitude.text = m.ToString() + "m";
	}

	// Token: 0x040008B6 RID: 2230
	public TMP_Text scoutName;

	// Token: 0x040008B7 RID: 2231
	public TMP_Text altitude;

	// Token: 0x040008B8 RID: 2232
	public float panelAlpha = 0.25f;

	// Token: 0x040008B9 RID: 2233
	public Image panel;
}
