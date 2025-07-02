using System;
using System.Collections;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using Zorro.Core;

// Token: 0x0200015A RID: 346
public class LoadingScreen : MonoBehaviour
{
	// Token: 0x060009E0 RID: 2528 RVA: 0x000316AF File Offset: 0x0002F8AF
	private void Awake()
	{
		this.canvas.enabled = false;
		this.anim = base.GetComponent<Animator>();
		base.transform.SetParent(null, true);
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060009E1 RID: 2529 RVA: 0x000316E1 File Offset: 0x0002F8E1
	public virtual IEnumerator LoadingRoutine(Action runAfter, IEnumerator[] processList)
	{
		PhotonNetwork.IsMessageQueueRunning = false;
		this.canvas.enabled = true;
		float num = 0f;
		if (this.FadeOutAudioCurve != null && this.FadeOutAudioCurve.keys.Length != 0)
		{
			num = this.FadeOutAudioCurve.GetEndTime();
		}
		float extraLoadTime = this.loadStartYieldTime - num;
		if (this.FadeOutAudioCurve != null && this.FadeOutAudioCurve.keys.Length != 0)
		{
			yield return this.FadeOutAudioCurve.YieldForCurve(delegate(float f)
			{
				Debug.Log(string.Format("SETTING FADE: {0}", f));
				if (this.Mixer != null)
				{
					this.Mixer.SetFloat("LoadingFade", math.remap(0f, 1f, -80f, 0f, f));
				}
			}, true, 1f);
		}
		if (extraLoadTime > 0f)
		{
			yield return new WaitForSecondsRealtime(extraLoadTime);
		}
		int num2;
		for (int processIndex = 0; processIndex < processList.Length; processIndex = num2 + 1)
		{
			this.currentProcess = processList[processIndex];
			base.StartCoroutine(this.RunProcess(this.currentProcess));
			while (this.runningProcess)
			{
				yield return null;
			}
			num2 = processIndex;
		}
		if (!PhotonNetwork.IsMessageQueueRunning)
		{
			PhotonNetwork.IsMessageQueueRunning = true;
			Debug.Log("Restarting message queue");
		}
		if (runAfter != null)
		{
			runAfter();
		}
		this.anim.SetTrigger("Finish");
		Debug.Log("Loading finished.");
		if (this.FadeInAudioCurve != null && this.FadeInAudioCurve.keys.Length != 0)
		{
			yield return this.FadeInAudioCurve.YieldForCurve(delegate(float f)
			{
				Debug.Log(string.Format("SETTING FADE OUT: {0}", f));
				if (this.Mixer != null)
				{
					this.Mixer.SetFloat("LoadingFade", math.remap(0f, 1f, -80f, 0f, f));
				}
			}, true, 1f);
		}
		Object.Destroy(base.gameObject, 6f);
		yield break;
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x000316FE File Offset: 0x0002F8FE
	private IEnumerator RunProcess(IEnumerator process)
	{
		Debug.Log("Process Started: process");
		this.runningProcess = true;
		yield return base.StartCoroutine(process);
		this.runningProcess = false;
		Debug.Log("Process Finished: process");
		yield break;
	}

	// Token: 0x040008DA RID: 2266
	public AnimationCurve FadeOutAudioCurve;

	// Token: 0x040008DB RID: 2267
	public AnimationCurve FadeInAudioCurve;

	// Token: 0x040008DC RID: 2268
	public AudioMixer Mixer;

	// Token: 0x040008DD RID: 2269
	public Canvas canvas;

	// Token: 0x040008DE RID: 2270
	private Animator anim;

	// Token: 0x040008DF RID: 2271
	public float loadStartYieldTime = 1.5f;

	// Token: 0x040008E0 RID: 2272
	protected IEnumerator currentProcess;

	// Token: 0x040008E1 RID: 2273
	private bool runningProcess;

	// Token: 0x02000373 RID: 883
	public enum LoadingScreenType
	{
		// Token: 0x040012C8 RID: 4808
		Basic,
		// Token: 0x040012C9 RID: 4809
		Plane
	}
}
