using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x02000154 RID: 340
public class EndScreen : MenuWindow
{
	// Token: 0x1700007E RID: 126
	// (get) Token: 0x060009B0 RID: 2480 RVA: 0x0003055C File Offset: 0x0002E75C
	public override bool openOnStart
	{
		get
		{
			return false;
		}
	}

	// Token: 0x1700007F RID: 127
	// (get) Token: 0x060009B1 RID: 2481 RVA: 0x0003055F File Offset: 0x0002E75F
	public override Selectable objectToSelectOnOpen
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000080 RID: 128
	// (get) Token: 0x060009B2 RID: 2482 RVA: 0x00030562 File Offset: 0x0002E762
	public override bool selectOnOpen
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x00030565 File Offset: 0x0002E765
	private void Awake()
	{
		EndScreen.instance = this;
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x0003056D File Offset: 0x0002E76D
	protected override void Start()
	{
		base.Start();
		base.StartCoroutine(this.EndSequenceRoutine());
	}

	// Token: 0x060009B5 RID: 2485 RVA: 0x00030584 File Offset: 0x0002E784
	protected override void Initialize()
	{
		this.nextButton.onClick.AddListener(new UnityAction(this.Next));
		this.cosmeticNextButton.onClick.AddListener(new UnityAction(this.PopupNext));
		this.ascentsNextButton.onClick.AddListener(new UnityAction(this.PopupNext));
		this.promotionNextButton.onClick.AddListener(new UnityAction(this.PopupNext));
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x00030601 File Offset: 0x0002E801
	private void Next()
	{
		this.WaitingForPlayersUI.gameObject.SetActive(true);
		Singleton<GameOverHandler>.Instance.LocalPlayerHasClosedEndScreen();
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x0003061E File Offset: 0x0002E81E
	private IEnumerator EndSequenceRoutine()
	{
		UIInputHandler.SetSelectedObject(null);
		this.canvasGroup.alpha = 0f;
		this.canvasGroup.DOFade(1f, 1f);
		List<Character> allCharacters = Character.AllCharacters;
		for (int i = 0; i < this.scoutWindows.Length; i++)
		{
			if (i < allCharacters.Count)
			{
				this.scoutWindows[i].gameObject.SetActive(true);
				this.scoutWindows[i].Init(allCharacters[i]);
			}
			else
			{
				this.scoutWindows[i].gameObject.SetActive(false);
			}
		}
		this.endTime.gameObject.SetActive(false);
		this.buttons.SetActive(false);
		this.peakBanner.SetActive(Character.localCharacter.refs.stats.won);
		this.yourFriendsWonBanner.SetActive(!Character.localCharacter.refs.stats.won && Character.localCharacter.refs.stats.somebodyElseWon);
		this.deadBanner.SetActive(!Character.localCharacter.refs.stats.won && !Character.localCharacter.refs.stats.somebodyElseWon);
		this.cosmeticUnlockObject.SetActive(false);
		yield return new WaitForSeconds(2f);
		try
		{
			this.endTime.text = this.GetTimeString(Character.localCharacter.refs.stats.timelineInfo[Character.localCharacter.refs.stats.timelineInfo.Count - 1].time - Character.localCharacter.refs.stats.timelineInfo[0].time);
			this.endTime.gameObject.SetActive(true);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
		}
		yield return new WaitForSeconds(1f);
		yield return base.StartCoroutine(this.TimelineRoutine(allCharacters));
		yield return new WaitForSeconds(0.25f);
		List<int> completedAscentsThisRun = Singleton<AchievementManager>.Instance.completedAscentsThisRun;
		yield return base.StartCoroutine(this.AscentRoutine(completedAscentsThisRun));
		yield return new WaitForSeconds(0.25f);
		this.selectedBadge = false;
		yield return base.StartCoroutine(this.BadgeRoutine());
		this.buttons.SetActive(true);
		if (!this.selectedBadge)
		{
			UIInputHandler.SetSelectedObject(this.returnToAirportButton.gameObject);
		}
		yield break;
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x00030630 File Offset: 0x0002E830
	private string GetTimeString(float totalSeconds)
	{
		int num = Mathf.FloorToInt(totalSeconds);
		int num2 = num / 3600;
		int num3 = num % 3600 / 60;
		int num4 = num % 60;
		return string.Format("{0}:{1:00}:{2:00}", num2, num3, num4);
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x00030676 File Offset: 0x0002E876
	private IEnumerator TimelineRoutine(List<Character> allCharacters)
	{
		for (int j = 0; j < this.scouts.Length; j++)
		{
			this.scouts[j].gameObject.SetActive(false);
			this.scoutsAtPeak[j].gameObject.SetActive(false);
		}
		if (this.debug)
		{
			for (int k = 0; k < this.scouts.Length; k++)
			{
				this.scouts[k].color = this.debugColors[k];
				this.scoutsAtPeak[k].color = this.debugColors[k];
			}
		}
		else
		{
			for (int l = 0; l < allCharacters.Count; l++)
			{
				Color playerColor = allCharacters[l].refs.customization.PlayerColor;
				playerColor.a = 1f;
				this.scouts[l].color = playerColor;
				this.scoutsAtPeak[l].color = this.scouts[l].color;
			}
		}
		yield return new WaitForSeconds(0.1f);
		List<List<EndScreen.TimelineInfo>> timelineInfos = new List<List<EndScreen.TimelineInfo>>();
		if (this.debug)
		{
			timelineInfos.Add(new List<EndScreen.TimelineInfo>());
			timelineInfos.Add(new List<EndScreen.TimelineInfo>());
			timelineInfos.Add(new List<EndScreen.TimelineInfo>());
			timelineInfos.Add(new List<EndScreen.TimelineInfo>());
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			int num5 = Random.Range(10, this.pipCount - 10);
			for (int m = 0; m < this.pipCount; m++)
			{
				float num6 = (float)m / ((float)this.pipCount - 1f);
				EndScreen.TimelineInfo timelineInfo = default(EndScreen.TimelineInfo);
				num += this.GetRandom(num6) * 0.15f * num6;
				timelineInfo.height = Mathf.Clamp01(num6 + num);
				timelineInfo.time = num6;
				timelineInfos[0].Add(timelineInfo);
				EndScreen.TimelineInfo timelineInfo2 = default(EndScreen.TimelineInfo);
				num2 += this.GetRandom(num6) * 0.15f * num6;
				timelineInfo2.height = Mathf.Clamp01(num6 + num2);
				timelineInfo2.time = num6;
				timelineInfos[1].Add(timelineInfo2);
				EndScreen.TimelineInfo timelineInfo3 = default(EndScreen.TimelineInfo);
				num3 += this.GetRandom(num6) * 0.15f * num6;
				timelineInfo3.height = Mathf.Clamp01(num6 + num3);
				timelineInfo3.time = num6;
				timelineInfos[2].Add(timelineInfo3);
				EndScreen.TimelineInfo timelineInfo4 = default(EndScreen.TimelineInfo);
				num4 += this.GetRandom(num6) * 0.15f * num6;
				timelineInfo4.height = Mathf.Clamp01(num6 + num4);
				timelineInfo4.time = num6;
				if (m == num5)
				{
					timelineInfo4.died = true;
				}
				if (m > num5)
				{
					timelineInfo4.dead = true;
				}
				timelineInfos[3].Add(timelineInfo4);
			}
		}
		else
		{
			for (int n = 0; n < allCharacters.Count; n++)
			{
				if (allCharacters[n] != null)
				{
					timelineInfos.Add(allCharacters[n].refs.stats.timelineInfo);
				}
			}
		}
		for (int num7 = 0; num7 < timelineInfos.Count; num7++)
		{
			this.scouts[num7].gameObject.SetActive(true);
		}
		int longestCount = 1;
		for (int num8 = 0; num8 < timelineInfos.Count; num8++)
		{
			if (timelineInfos[num8].Count > longestCount)
			{
				longestCount = timelineInfos[num8].Count;
			}
		}
		float startTime = 100000f;
		float maxTime = 0f;
		maxTime = Character.localCharacter.refs.stats.GetFinalTimelineInfo().time;
		startTime = Character.localCharacter.refs.stats.GetFirstTimelineInfo().time;
		maxTime -= startTime;
		if (maxTime == 0f)
		{
			maxTime = 1f;
		}
		float yieldTime = Mathf.Min(this.waitTime * Time.deltaTime / (float)longestCount, 0.2f);
		int num10;
		for (int i = 0; i < longestCount; i = num10 + 1)
		{
			for (int num9 = 0; num9 < timelineInfos.Count; num9++)
			{
				if (i < timelineInfos[num9].Count)
				{
					this.DrawPip(num9, timelineInfos[num9][i], maxTime, startTime, this.scouts[num9].color);
					if (!timelineInfos[num9][i].dead && !timelineInfos[num9][i].died)
					{
						this.scoutWindows[num9].UpdateAltitude(CharacterStats.UnitsToMeters(timelineInfos[num9][i].height));
					}
				}
			}
			yield return new WaitForSeconds(yieldTime * 0.33f);
			num10 = i;
		}
		for (int i = 0; i < timelineInfos.Count; i = num10 + 1)
		{
			this.CheckPeak(i, timelineInfos[i][timelineInfos[i].Count - 1]);
			yield return new WaitForSeconds(0.25f);
			num10 = i;
		}
		yield break;
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x0003068C File Offset: 0x0002E88C
	private List<BadgeData> GetBadgeUnlocks()
	{
		List<BadgeData> list = new List<BadgeData>();
		foreach (ACHIEVEMENTTYPE achievementtype in Singleton<AchievementManager>.Instance.achievementsEarnedThisRun)
		{
			BadgeData badgeData = GUIManager.instance.mainBadgeManager.GetBadgeData(achievementtype);
			if (badgeData != null)
			{
				list.Add(badgeData);
			}
		}
		return list;
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x00030704 File Offset: 0x0002E904
	private IEnumerator AscentRoutine(List<int> completedAscentsThisRun)
	{
		if (completedAscentsThisRun.Count > 0 && completedAscentsThisRun[0] == 0)
		{
			yield return this.AscentsUnlockRoutine();
		}
		int num;
		for (int i = 0; i < completedAscentsThisRun.Count; i = num + 1)
		{
			yield return new WaitForSeconds(0.5f);
			yield return this.PromotionUnlockRoutine(completedAscentsThisRun[i]);
			num = i;
		}
		yield return null;
		yield break;
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x0003071A File Offset: 0x0002E91A
	private IEnumerator BadgeRoutine()
	{
		BadgeManager bm = base.GetComponent<BadgeManager>();
		bm.InheritData(GUIManager.instance.mainBadgeManager);
		List<BadgeData> badgeUnlocks = this.GetBadgeUnlocks();
		int num;
		for (int i = 0; i < badgeUnlocks.Count; i = num + 1)
		{
			BadgeUI newBadge = Object.Instantiate<BadgeUI>(this.badge, this.badgeParentTF);
			newBadge.manager = bm;
			newBadge.Init(badgeUnlocks[i]);
			newBadge.canvasGroup.DOFade(1f, 0.2f);
			newBadge.transform.localScale = Vector3.one * 1.5f;
			newBadge.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
			CustomizationOption cosmetic;
			if (Singleton<Customization>.Instance.TryGetUnlockedCosmetic(badgeUnlocks[i], out cosmetic))
			{
				yield return new WaitForSeconds(0.5f);
				yield return this.CosmeticUnlockRoutine(cosmetic);
			}
			if (i == 0)
			{
				UIInputHandler.SetSelectedObject(newBadge.gameObject);
				this.selectedBadge = true;
			}
			yield return new WaitForSeconds(0.5f);
			newBadge = null;
			cosmetic = null;
			num = i;
		}
		yield break;
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x00030729 File Offset: 0x0002E929
	public void PopupNext()
	{
		this.inPopupView = false;
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x00030732 File Offset: 0x0002E932
	private IEnumerator CosmeticUnlockRoutine(CustomizationOption cosmetic)
	{
		this.cosmeticUnlockObject.SetActive(true);
		string text = "NEW HAT!";
		if (cosmetic.type == Customization.Type.Accessory || cosmetic.type == Customization.Type.Eyes)
		{
			text = "NEW LOOK!";
		}
		if (cosmetic.type == Customization.Type.Fit)
		{
			text = "NEW FIT!";
		}
		this.cosmeticUnlockTitle.text = text;
		this.cosmeticUnlockIcon.texture = cosmetic.texture;
		Shadow component = this.cosmeticUnlockIcon.GetComponent<Shadow>();
		if (component)
		{
			component.enabled = cosmetic.type == Customization.Type.Eyes;
		}
		this.cosmeticUnlockIcon.material = ((cosmetic.type == Customization.Type.Eyes) ? this.eyesMaterial : null);
		this.inPopupView = true;
		while (this.inPopupView)
		{
			UIInputHandler.SetSelectedObject(this.cosmeticNextButton.gameObject);
			yield return null;
		}
		this.cosmeticUnlockAnimator.Play("Done", 0, 0f);
		yield return new WaitForSeconds(0.25f);
		this.cosmeticUnlockObject.SetActive(false);
		yield break;
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x00030748 File Offset: 0x0002E948
	private IEnumerator AscentsUnlockRoutine()
	{
		this.ascentsUnlockObject.SetActive(true);
		this.inPopupView = true;
		while (this.inPopupView)
		{
			UIInputHandler.SetSelectedObject(this.ascentsNextButton.gameObject);
			yield return null;
		}
		this.ascentsUnlockAnimator.Play("Done", 0, 0f);
		yield return new WaitForSeconds(0.25f);
		this.ascentsUnlockObject.SetActive(false);
		yield break;
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x00030757 File Offset: 0x0002E957
	private IEnumerator PromotionUnlockRoutine(int ascent)
	{
		this.promotionUnlockObject.SetActive(true);
		string titleReward = this.ascentData.ascents[ascent + 1].titleReward;
		this.promotionUnlockTitle.text = titleReward;
		if (ascent < this.ascentData.ascents.Count - 2)
		{
			this.promotionNextAscentUnlockText.text = this.ascentData.ascents[ascent + 2].title + " UNLOCKED!";
		}
		else
		{
			this.promotionNextAscentUnlockText.text = "";
		}
		this.promotionUnlockIcon.sprite = this.ascentData.ascents[ascent + 1].sashSprite;
		this.inPopupView = true;
		while (this.inPopupView)
		{
			UIInputHandler.SetSelectedObject(this.promotionNextButton.gameObject);
			yield return null;
		}
		this.promotionUnlockAnimator.Play("Done", 0, 0f);
		yield return new WaitForSeconds(0.25f);
		this.promotionUnlockObject.SetActive(false);
		yield break;
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x0003076D File Offset: 0x0002E96D
	private float GetRandom(float nudge)
	{
		return Random.Range(-1f + nudge, 0f + nudge);
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x00030784 File Offset: 0x0002E984
	public void DrawPip(int playerIndex, EndScreen.TimelineInfo heightTime, float maxTime, float startTime, Color color)
	{
		if (heightTime.dead)
		{
			return;
		}
		Image image = Object.Instantiate<Image>(heightTime.revived ? this.revivedPip : (heightTime.justPassedOut ? this.passedOutPip : (heightTime.died ? this.deadPip : this.pip)), this.scoutLines[playerIndex]);
		image.color = color;
		image.transform.GetChild(0).GetComponent<Image>().color = image.color;
		float num = CharacterStats.peakHeightInUnits;
		if (this.debug)
		{
			num = 1f;
		}
		image.transform.localPosition = new Vector3(this.timelinePanel.sizeDelta.x * Mathf.Clamp01((heightTime.time - startTime) / maxTime), this.timelinePanel.sizeDelta.y * heightTime.height / num, 0f);
		image.transform.localPosition += Vector3.up * (float)playerIndex * 2f;
		this.scouts[playerIndex].transform.localPosition = image.transform.localPosition;
		if (this.oldPip[playerIndex])
		{
			image.transform.right = this.oldPip[playerIndex].transform.position - image.transform.position;
			image.rectTransform.sizeDelta = new Vector2(Vector3.Distance(image.transform.position, this.oldPip[playerIndex].transform.position) / this.timelinePanel.lossyScale.x, 1.5f);
		}
		if (heightTime.died)
		{
			this.scouts[playerIndex].gameObject.SetActive(false);
			image.transform.GetChild(2).GetComponent<Image>().color = image.color;
			image.transform.GetChild(2).transform.rotation = Quaternion.identity;
		}
		if (heightTime.justPassedOut)
		{
			image.transform.GetChild(2).GetComponent<Image>().color = image.color;
			image.transform.GetChild(2).transform.rotation = Quaternion.identity;
		}
		else if (heightTime.passedOut)
		{
			image.transform.GetChild(0).GetComponent<Image>().material = this.passedOutMaterial;
		}
		if (heightTime.revived)
		{
			image.transform.GetChild(2).GetComponent<Image>().color = image.color;
			image.transform.GetChild(2).transform.rotation = Quaternion.identity;
			image.transform.GetChild(0).gameObject.SetActive(false);
			this.scouts[playerIndex].gameObject.SetActive(true);
		}
		this.oldPip[playerIndex] = image;
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x00030A5C File Offset: 0x0002EC5C
	public void CheckPeak(int playerIndex, EndScreen.TimelineInfo timelineInfo)
	{
		if (timelineInfo.time >= 0.99f && timelineInfo.height >= 1f && !this.scoutsAtPeak[playerIndex].gameObject.activeSelf && !timelineInfo.dead && timelineInfo.won)
		{
			this.scouts[playerIndex].gameObject.SetActive(false);
			this.scoutsAtPeak[playerIndex].gameObject.SetActive(true);
			this.scoutsAtPeak[playerIndex].transform.SetSiblingIndex(1);
			this.scoutsAtPeak[playerIndex].rectTransform.sizeDelta = Vector3.zero;
			this.scoutsAtPeak[playerIndex].rectTransform.DOSizeDelta(Vector3.one * 15f, 0.25f, false).SetEase(Ease.OutBack);
		}
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x00030B40 File Offset: 0x0002ED40
	public void ReturnToAirport()
	{
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", true, true, 3f) });
	}

	// Token: 0x04000888 RID: 2184
	public static EndScreen instance;

	// Token: 0x04000889 RID: 2185
	public CanvasGroup canvasGroup;

	// Token: 0x0400088A RID: 2186
	public AscentData ascentData;

	// Token: 0x0400088B RID: 2187
	public bool debug;

	// Token: 0x0400088C RID: 2188
	public TMP_Text endTime;

	// Token: 0x0400088D RID: 2189
	public EndScreenScoutWindow[] scoutWindows;

	// Token: 0x0400088E RID: 2190
	public Color[] debugColors;

	// Token: 0x0400088F RID: 2191
	public BadgeData[] debugBadgeUnlocks;

	// Token: 0x04000890 RID: 2192
	public BadgeUI badge;

	// Token: 0x04000891 RID: 2193
	public Transform badgeParentTF;

	// Token: 0x04000892 RID: 2194
	public Transform[] scoutLines;

	// Token: 0x04000893 RID: 2195
	public Image[] scouts;

	// Token: 0x04000894 RID: 2196
	public Image[] scoutsAtPeak;

	// Token: 0x04000895 RID: 2197
	public int pipCount = 100;

	// Token: 0x04000896 RID: 2198
	public float waitTime = 5f;

	// Token: 0x04000897 RID: 2199
	public RectTransform timelinePanel;

	// Token: 0x04000898 RID: 2200
	public Image pip;

	// Token: 0x04000899 RID: 2201
	public Image deadPip;

	// Token: 0x0400089A RID: 2202
	public Image passedOutPip;

	// Token: 0x0400089B RID: 2203
	public Image revivedPip;

	// Token: 0x0400089C RID: 2204
	public Material passedOutMaterial;

	// Token: 0x0400089D RID: 2205
	public GameObject peakBanner;

	// Token: 0x0400089E RID: 2206
	public GameObject deadBanner;

	// Token: 0x0400089F RID: 2207
	public GameObject yourFriendsWonBanner;

	// Token: 0x040008A0 RID: 2208
	public GameObject buttons;

	// Token: 0x040008A1 RID: 2209
	public WaitingForPlayersUI WaitingForPlayersUI;

	// Token: 0x040008A2 RID: 2210
	public Button nextButton;

	// Token: 0x040008A3 RID: 2211
	public Button returnToAirportButton;

	// Token: 0x040008A4 RID: 2212
	public Material eyesMaterial;

	// Token: 0x040008A5 RID: 2213
	private bool selectedBadge;

	// Token: 0x040008A6 RID: 2214
	public GameObject cosmeticUnlockObject;

	// Token: 0x040008A7 RID: 2215
	public Animator cosmeticUnlockAnimator;

	// Token: 0x040008A8 RID: 2216
	public TMP_Text cosmeticUnlockTitle;

	// Token: 0x040008A9 RID: 2217
	public Button cosmeticNextButton;

	// Token: 0x040008AA RID: 2218
	public RawImage cosmeticUnlockIcon;

	// Token: 0x040008AB RID: 2219
	public GameObject ascentsUnlockObject;

	// Token: 0x040008AC RID: 2220
	public Animator ascentsUnlockAnimator;

	// Token: 0x040008AD RID: 2221
	public Button ascentsNextButton;

	// Token: 0x040008AE RID: 2222
	public GameObject promotionUnlockObject;

	// Token: 0x040008AF RID: 2223
	public Animator promotionUnlockAnimator;

	// Token: 0x040008B0 RID: 2224
	public TMP_Text promotionUnlockTitle;

	// Token: 0x040008B1 RID: 2225
	public TMP_Text promotionNextAscentUnlockText;

	// Token: 0x040008B2 RID: 2226
	public Button promotionNextButton;

	// Token: 0x040008B3 RID: 2227
	public Image promotionUnlockIcon;

	// Token: 0x040008B4 RID: 2228
	private bool inPopupView;

	// Token: 0x040008B5 RID: 2229
	private Image[] oldPip = new Image[4];

	// Token: 0x0200036B RID: 875
	public struct TimelineInfo
	{
		// Token: 0x04001299 RID: 4761
		public float height;

		// Token: 0x0400129A RID: 4762
		public float time;

		// Token: 0x0400129B RID: 4763
		public bool died;

		// Token: 0x0400129C RID: 4764
		public bool dead;

		// Token: 0x0400129D RID: 4765
		public bool revived;

		// Token: 0x0400129E RID: 4766
		public bool justPassedOut;

		// Token: 0x0400129F RID: 4767
		public bool passedOut;

		// Token: 0x040012A0 RID: 4768
		public bool won;
	}
}
