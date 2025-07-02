using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x0200017F RID: 383
public class WaitingForPlayersUI : MonoBehaviour
{
	// Token: 0x06000AB1 RID: 2737 RVA: 0x00033F74 File Offset: 0x00032174
	private void Update()
	{
		List<Player> allPlayers = PlayerHandler.GetAllPlayers();
		for (int i = 0; i < this.scoutImages.Length; i++)
		{
			this.scoutImages[i].gameObject.SetActive(false);
		}
		int num = 0;
		foreach (Player player in allPlayers)
		{
			bool hasClosedEndScreen = player.hasClosedEndScreen;
			PersistentPlayerData playerData = GameHandler.GetService<PersistentPlayerDataService>().GetPlayerData(player.photonView.Owner);
			Color color = Singleton<Customization>.Instance.skins[playerData.customizationData.currentSkin].color;
			this.scoutImages[num].gameObject.SetActive(true);
			this.scoutImages[num].color = (hasClosedEndScreen ? color : this.notReadyColor);
			num++;
		}
	}

	// Token: 0x04000989 RID: 2441
	public Image[] scoutImages;

	// Token: 0x0400098A RID: 2442
	public Color notReadyColor;
}
