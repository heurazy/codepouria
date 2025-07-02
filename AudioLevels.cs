using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000041 RID: 65
public class AudioLevels : MonoBehaviour
{
	// Token: 0x06000311 RID: 785 RVA: 0x00013664 File Offset: 0x00011864
	public static void Reset()
	{
		AudioLevels.PlayerAudioLevels.Clear();
		GlobalEvents.TriggerCharacterAudioLevelsUpdated();
	}

	// Token: 0x06000312 RID: 786 RVA: 0x00013675 File Offset: 0x00011875
	public static float GetPlayerLevel(int playerID)
	{
		if (!AudioLevels.PlayerAudioLevels.ContainsKey(playerID))
		{
			return 0.5f;
		}
		return AudioLevels.PlayerAudioLevels[playerID];
	}

	// Token: 0x06000313 RID: 787 RVA: 0x00013695 File Offset: 0x00011895
	public static void SetPlayerLevel(int playerID, float f)
	{
		if (!AudioLevels.PlayerAudioLevels.ContainsKey(playerID))
		{
			AudioLevels.PlayerAudioLevels.Add(playerID, 1f);
		}
		AudioLevels.PlayerAudioLevels[playerID] = f;
		GlobalEvents.TriggerCharacterAudioLevelsUpdated();
	}

	// Token: 0x06000314 RID: 788 RVA: 0x000136C8 File Offset: 0x000118C8
	public void OnEnable()
	{
		Photon.Realtime.Player[] playerList = PhotonNetwork.PlayerList;
		int i = 0;
		Debug.Log("There are " + playerList.Length.ToString() + " Players.");
		for (int j = 0; j < playerList.Length; j++)
		{
			if (this.sliders.Count > j)
			{
				this.sliders[j].Init(playerList[j]);
			}
			i = j + 1;
		}
		while (i < this.sliders.Count)
		{
			this.sliders[i].Init(null);
			i++;
		}
		this.InitNavigation();
	}

	// Token: 0x06000315 RID: 789 RVA: 0x0001375C File Offset: 0x0001195C
	private void InitNavigation()
	{
		if (!this.mainPage)
		{
			return;
		}
		for (int i = 0; i < this.sliders.Count; i++)
		{
			Slider slider = this.sliders[i].slider;
			bool flag = i == 0 || !this.sliders[i - 1].gameObject.activeInHierarchy;
			bool flag2 = i == this.sliders.Count - 1 || !this.sliders[i + 1].gameObject.activeInHierarchy;
			Selectable selectable = (flag ? null : this.sliders[i - 1].slider);
			Selectable selectable2 = (flag2 ? this.mainPage.resumeButton : this.sliders[i + 1].slider);
			this.SetSliderSelection(slider, selectable, selectable2);
		}
	}

	// Token: 0x06000316 RID: 790 RVA: 0x00013840 File Offset: 0x00011A40
	private void SetSliderSelection(Selectable obj, Selectable prev, Selectable next)
	{
		obj.navigation = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnUp = prev,
			selectOnDown = next,
			selectOnLeft = null,
			selectOnRight = null
		};
	}

	// Token: 0x040003B3 RID: 947
	public static Dictionary<int, float> PlayerAudioLevels = new Dictionary<int, float>();

	// Token: 0x040003B4 RID: 948
	public List<AudioLevelSlider> sliders;

	// Token: 0x040003B5 RID: 949
	public PauseOptionsMenu mainPage;
}
