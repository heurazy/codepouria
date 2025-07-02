using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x02000197 RID: 407
public class BoardingPass : MenuWindow
{
	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x06000B1D RID: 2845 RVA: 0x00037114 File Offset: 0x00035314
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x06000B1E RID: 2846 RVA: 0x00037117 File Offset: 0x00035317
	public override bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x06000B1F RID: 2847 RVA: 0x0003711A File Offset: 0x0003531A
	public override bool closeOnPause
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x06000B20 RID: 2848 RVA: 0x0003711D File Offset: 0x0003531D
	public override bool closeOnUICancel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x06000B21 RID: 2849 RVA: 0x00037120 File Offset: 0x00035320
	public override bool autoHideOnClose
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x06000B22 RID: 2850 RVA: 0x00037123 File Offset: 0x00035323
	// (set) Token: 0x06000B23 RID: 2851 RVA: 0x0003712B File Offset: 0x0003532B
	public int ascentIndex
	{
		get
		{
			return this._ascentIndex;
		}
		set
		{
			this._ascentIndex = value;
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x06000B24 RID: 2852 RVA: 0x00037134 File Offset: 0x00035334
	public override Selectable objectToSelectOnOpen
	{
		get
		{
			return this.startGameButton;
		}
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x0003713C File Offset: 0x0003533C
	protected override void Initialize()
	{
		this.incrementAscentButton.onClick.AddListener(new UnityAction(this.IncrementAscent));
		this.decrementAscentButton.onClick.AddListener(new UnityAction(this.DecrementAscent));
		this.startGameButton.onClick.AddListener(new UnityAction(this.StartGame));
		this.closeButton.onClick.AddListener(new UnityAction(base.Close));
		this.UpdateAscent();
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x000371BF File Offset: 0x000353BF
	private void InitMaxAscent()
	{
		this.maxUnlockedAscent = 0;
		Singleton<AchievementManager>.Instance.GetSteamStatInt(STEAMSTATTYPE.MaxAscent, out this.maxUnlockedAscent);
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x000371DC File Offset: 0x000353DC
	protected override void OnOpen()
	{
		this.playerName.text = "PASSENGER:\n<size=170%>" + Character.localCharacter.characterName;
		List<Character> allCharacters = Character.AllCharacters;
		for (int i = 0; i < this.players.Length; i++)
		{
			if (allCharacters.Count > i)
			{
				this.players[i].gameObject.SetActive(true);
				this.players[i].color = allCharacters[i].refs.customization.PlayerColor;
			}
			else
			{
				this.players[i].gameObject.SetActive(false);
			}
		}
		this.canvasGroup.alpha = 0f;
		this.canvasGroup.DOFade(1f, 0.5f);
		this.UpdateAscent();
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x000372A1 File Offset: 0x000354A1
	protected override void OnClose()
	{
		this.canvasGroup.DOFade(0f, 0.2f);
		base.Invoke("HideIt", 0.2f);
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x000372C9 File Offset: 0x000354C9
	private void HideIt()
	{
		base.Hide();
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x000372D4 File Offset: 0x000354D4
	private void UpdateAscent()
	{
		this.maxUnlockedAscent = Singleton<AchievementManager>.Instance.GetMaxAscent();
		int num = Mathf.Min(this.maxAscent, this.maxUnlockedAscent);
		this.incrementAscentButton.interactable = this.ascentIndex < num;
		this.decrementAscentButton.interactable = this.ascentIndex > -1;
		this.ascentTitle.text = this.ascentData.ascents[this.ascentIndex + 1].title;
		this.ascentDesc.text = this.ascentData.ascents[this.ascentIndex + 1].description;
		if (this.ascentIndex >= 2)
		{
			TMP_Text tmp_Text = this.ascentDesc;
			tmp_Text.text += "\n\n<alpha=#CC><size=70%>And all previous modifiers.";
		}
		if (this.ascentIndex == this.maxUnlockedAscent && this.ascentIndex > -1 && this.ascentIndex < 8)
		{
			this.reward.gameObject.SetActive(true);
			this.rewardText.text = this.ascentData.ascents[this.ascentIndex + 1].titleReward;
			this.rewardImage.color = this.ascentData.ascents[this.ascentIndex + 1].color;
			return;
		}
		this.reward.gameObject.SetActive(false);
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x00037434 File Offset: 0x00035634
	public void IncrementAscent()
	{
		int ascentIndex = this.ascentIndex;
		this.ascentIndex = ascentIndex + 1;
		this.UpdateAscent();
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x00037458 File Offset: 0x00035658
	public void DecrementAscent()
	{
		int ascentIndex = this.ascentIndex;
		this.ascentIndex = ascentIndex - 1;
		this.UpdateAscent();
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x0003747B File Offset: 0x0003567B
	public void StartGame()
	{
		this.kiosk.StartGame(this.ascentIndex);
	}

	// Token: 0x04000A25 RID: 2597
	public TMP_Text playerName;

	// Token: 0x04000A26 RID: 2598
	public TMP_Text ascentTitle;

	// Token: 0x04000A27 RID: 2599
	public TMP_Text ascentDesc;

	// Token: 0x04000A28 RID: 2600
	public GameObject reward;

	// Token: 0x04000A29 RID: 2601
	public Image rewardImage;

	// Token: 0x04000A2A RID: 2602
	public TextMeshProUGUI rewardText;

	// Token: 0x04000A2B RID: 2603
	public Image[] players;

	// Token: 0x04000A2C RID: 2604
	private int _ascentIndex;

	// Token: 0x04000A2D RID: 2605
	private int maxAscent = 7;

	// Token: 0x04000A2E RID: 2606
	private int maxUnlockedAscent;

	// Token: 0x04000A2F RID: 2607
	public AirportCheckInKiosk kiosk;

	// Token: 0x04000A30 RID: 2608
	public Button incrementAscentButton;

	// Token: 0x04000A31 RID: 2609
	public Button decrementAscentButton;

	// Token: 0x04000A32 RID: 2610
	public Button startGameButton;

	// Token: 0x04000A33 RID: 2611
	public Button closeButton;

	// Token: 0x04000A34 RID: 2612
	public AscentData ascentData;

	// Token: 0x04000A35 RID: 2613
	public CanvasGroup canvasGroup;
}
