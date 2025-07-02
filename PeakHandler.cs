using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.Core;

// Token: 0x0200020D RID: 525
public class PeakHandler : Singleton<PeakHandler>
{
	// Token: 0x06000D95 RID: 3477 RVA: 0x00044552 File Offset: 0x00042752
	public void SummonHelicopter()
	{
		this.peakSequence.SetActive(true);
		this.summonedHelicopter = true;
	}

	// Token: 0x06000D96 RID: 3478 RVA: 0x00044568 File Offset: 0x00042768
	public void EndCutscene()
	{
		this.isPlayingCinematic = true;
		List<Character> allCharacters = Character.AllCharacters;
		foreach (Character character in allCharacters)
		{
			character.refs.animator.gameObject.SetActive(false);
		}
		MainCamera.instance.gameObject.SetActive(false);
		MenuWindow.CloseAllWindows();
		this.peakSequence.SetActive(false);
		GUIManager.instance.letterboxCanvas.gameObject.SetActive(true);
		GUIManager.instance.hudCanvas.enabled = false;
		this.endCutscene.SetActive(true);
		this.SetCosmetics(allCharacters);
		base.StartCoroutine(this.<EndCutscene>g__OpenEndscreen|12_0());
	}

	// Token: 0x06000D97 RID: 3479 RVA: 0x00044638 File Offset: 0x00042838
	private void SetCosmetics(List<Character> characters)
	{
		Singleton<MicrophoneRelay>.Instance.RegisterMicListener(new Action<float[]>(this.OnGetLocalMic));
		characters = characters.Where((Character character) => character.refs.stats.won).ToList<Character>();
		characters.Sort((Character c1, Character c2) => c1.photonView.ViewID.CompareTo(c2.photonView.ViewID));
		characters[0].refs.customization.SetCustomizationForRef(this.firstCutsceneScout);
		this.firstCutsceneScout.GetComponent<AnimatedMouth>().audioSource = characters[0].GetComponent<AnimatedMouth>().audioSource;
		this.localMouths.Add(this.firstCutsceneScout.GetComponent<AnimatedMouth>());
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			if (i >= characters.Count)
			{
				this.cutsceneScoutRefs[i].gameObject.SetActive(false);
			}
			else
			{
				characters[i].refs.customization.SetCustomizationForRef(this.cutsceneScoutRefs[num]);
				BadgeUnlocker.SetBadges(characters[i], this.cutsceneScoutRefs[num].sashRenderer);
				this.cutsceneScoutRefs[num].GetComponent<AnimatedMouth>().audioSource = characters[i].GetComponent<AnimatedMouth>().audioSource;
				if (characters[i].IsLocal)
				{
					this.localMouths.Add(this.cutsceneScoutRefs[num].GetComponent<AnimatedMouth>());
				}
				num++;
			}
		}
		if (characters.Count <= 1)
		{
			this.cutsceneScoutAnims[0].alone = true;
		}
		if (characters.Count <= 2)
		{
			this.cutsceneScoutAnims[1].alone = true;
		}
	}

	// Token: 0x06000D98 RID: 3480 RVA: 0x000447E8 File Offset: 0x000429E8
	private void OnGetLocalMic(float[] buffer)
	{
		foreach (AnimatedMouth animatedMouth in this.localMouths)
		{
			animatedMouth.OnGetMic(buffer);
		}
	}

	// Token: 0x06000D99 RID: 3481 RVA: 0x0004483C File Offset: 0x00042A3C
	public void EndScreenComplete()
	{
		Singleton<GameOverHandler>.Instance.ForceEveryPlayerDoneWithEndScreen();
		this.endScreenComplete = true;
		base.StartCoroutine(PeakHandler.<EndScreenComplete>g__CreditsLogic|15_0());
	}

	// Token: 0x06000D9A RID: 3482 RVA: 0x0004485B File Offset: 0x00042A5B
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (this.isPlayingCinematic && Singleton<MicrophoneRelay>.Instance)
		{
			Singleton<MicrophoneRelay>.Instance.UnregisterMicListener(new Action<float[]>(this.OnGetLocalMic));
		}
	}

	// Token: 0x06000D9C RID: 3484 RVA: 0x000448AB File Offset: 0x00042AAB
	[CompilerGenerated]
	private IEnumerator <EndCutscene>g__OpenEndscreen|12_0()
	{
		yield return new WaitForSeconds(this.secondsUntilEndscreen);
		GUIManager.instance.endScreen.Open();
		while (!this.endScreenComplete)
		{
			yield return null;
		}
		this.endCutsceneAnimator.SetBool("Next", true);
		GUIManager.instance.endScreen.Close();
		yield break;
	}

	// Token: 0x06000D9D RID: 3485 RVA: 0x000448BA File Offset: 0x00042ABA
	[CompilerGenerated]
	internal static IEnumerator <EndScreenComplete>g__CreditsLogic|15_0()
	{
		yield return new WaitForSecondsRealtime(20f);
		InputAction anyKeyAction = InputSystem.actions.FindAction("AnyKey", false);
		bool skipped = false;
		float creditsLength = 60f;
		float t = 0f;
		while (t < creditsLength && !skipped)
		{
			Debug.Log("Waiting for input....");
			if (anyKeyAction != null && anyKeyAction.WasPerformedThisFrame())
			{
				skipped = true;
			}
			t += Time.unscaledDeltaTime;
			yield return null;
		}
		Debug.Log("Local player is done with credits!");
		Singleton<GameOverHandler>.Instance.LoadAirport();
		yield break;
	}

	// Token: 0x04000CA4 RID: 3236
	public bool summonedHelicopter;

	// Token: 0x04000CA5 RID: 3237
	public GameObject peakSequence;

	// Token: 0x04000CA6 RID: 3238
	public GameObject endCutscene;

	// Token: 0x04000CA7 RID: 3239
	public Animator endCutsceneAnimator;

	// Token: 0x04000CA8 RID: 3240
	public float secondsUntilEndscreen = 13f;

	// Token: 0x04000CA9 RID: 3241
	public CustomizationRefs firstCutsceneScout;

	// Token: 0x04000CAA RID: 3242
	public CustomizationRefs[] cutsceneScoutRefs;

	// Token: 0x04000CAB RID: 3243
	public EndCutsceneScoutHelper[] cutsceneScoutAnims;

	// Token: 0x04000CAC RID: 3244
	private List<AnimatedMouth> localMouths = new List<AnimatedMouth>();

	// Token: 0x04000CAD RID: 3245
	public bool isPlayingCinematic;

	// Token: 0x04000CAE RID: 3246
	private bool endScreenComplete;
}
