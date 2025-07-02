using System;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000042 RID: 66
public class AudioLevelSlider : MonoBehaviour
{
	// Token: 0x06000319 RID: 793 RVA: 0x00013898 File Offset: 0x00011A98
	private void Update()
	{
		Photon.Realtime.Player player = this.player;
	}

	// Token: 0x0600031A RID: 794 RVA: 0x000138A1 File Offset: 0x00011AA1
	private void Awake()
	{
		this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnSliderChanged));
	}

	// Token: 0x0600031B RID: 795 RVA: 0x000138C0 File Offset: 0x00011AC0
	public void Init(Photon.Realtime.Player newPlayer)
	{
		this.player = newPlayer;
		if (this.player == null)
		{
			Debug.Log("Init " + base.gameObject.name + " with null player");
		}
		else
		{
			Debug.Log(string.Concat(new string[]
			{
				"Init ",
				base.gameObject.name,
				" for player ",
				newPlayer.NickName,
				" local: ",
				this.player.IsLocal.ToString()
			}));
		}
		bool flag = this.player != null && !this.player.IsLocal;
		Debug.Log("Setting active: " + flag.ToString());
		base.gameObject.SetActive(flag);
		this.bar.color = this.barGradient.Evaluate(this.slider.value);
		if (flag)
		{
			this.playerName.text = this.player.NickName;
			this.slider.SetValueWithoutNotify(AudioLevels.GetPlayerLevel(this.player.ActorNumber));
		}
		this.percent.text = Mathf.RoundToInt(this.slider.value * 200f).ToString() + "%";
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00013A14 File Offset: 0x00011C14
	private void OnSliderChanged(float newValue)
	{
		if (this.player != null)
		{
			AudioLevels.SetPlayerLevel(this.player.ActorNumber, newValue);
			this.icon.sprite = ((newValue == 0f) ? this.mutedAudioSprite : this.audioSprites[Mathf.FloorToInt(newValue * 2.99f)]);
			this.bar.color = this.barGradient.Evaluate(newValue);
			EventSystem.current.SetSelectedGameObject(null);
			this.percent.text = Mathf.RoundToInt(newValue * 200f).ToString() + "%";
		}
	}

	// Token: 0x040003B6 RID: 950
	public TextMeshProUGUI playerName;

	// Token: 0x040003B7 RID: 951
	public TextMeshProUGUI percent;

	// Token: 0x040003B8 RID: 952
	public Photon.Realtime.Player player;

	// Token: 0x040003B9 RID: 953
	public Slider slider;

	// Token: 0x040003BA RID: 954
	public Image bar;

	// Token: 0x040003BB RID: 955
	public Gradient barGradient;

	// Token: 0x040003BC RID: 956
	public Sprite[] audioSprites;

	// Token: 0x040003BD RID: 957
	public Sprite mutedAudioSprite;

	// Token: 0x040003BE RID: 958
	public Image icon;
}
