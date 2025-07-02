using System;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;

// Token: 0x0200003B RID: 59
public class AnimatedMouth : MonoBehaviour
{
	// Token: 0x060002E8 RID: 744 RVA: 0x00012B80 File Offset: 0x00010D80
	private void Start()
	{
		this.amplitudePeakLimiter = this.minAmplitudeThreshold;
		this.character = base.GetComponent<Character>();
		if (!this.isGhost && this.character != null && this.character.IsLocal)
		{
			Singleton<MicrophoneRelay>.Instance.RegisterMicListener(new Action<float[]>(this.OnGetMic));
		}
		this.pushToTalkSetting = GameHandler.Instance.SettingsHandler.GetSetting<PushToTalkSetting>();
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x00012BF4 File Offset: 0x00010DF4
	private void OnDestroy()
	{
		if (!this.isGhost && this.character != null && this.character.IsLocal && Singleton<MicrophoneRelay>.Instance)
		{
			Singleton<MicrophoneRelay>.Instance.UnregisterMicListener(new Action<float[]>(this.OnGetMic));
		}
	}

	// Token: 0x060002EA RID: 746 RVA: 0x00012C46 File Offset: 0x00010E46
	public void OnGetMic(float[] buffer)
	{
		this.m_lastSentLocalBuffer = buffer;
	}

	// Token: 0x060002EB RID: 747 RVA: 0x00012C50 File Offset: 0x00010E50
	private void Update()
	{
		float[] array = new float[256];
		this.audioSource.GetSpectrumData(array, 0, FFTWindow.Rectangular);
		if (this.m_lastSentLocalBuffer != null)
		{
			array = this.m_lastSentLocalBuffer;
		}
		this.ProcessMicData(array);
	}

	// Token: 0x060002EC RID: 748 RVA: 0x00012C8C File Offset: 0x00010E8C
	public static float MicrophoneLevelMax(float[] data)
	{
		int num = 128;
		float num2 = 0f;
		for (int i = 0; i < num; i++)
		{
			float num3 = data[i] * data[i];
			if (num2 < num3)
			{
				num2 = num3;
			}
		}
		return num2;
	}

	// Token: 0x060002ED RID: 749 RVA: 0x00012CC0 File Offset: 0x00010EC0
	public static float MicrophoneLevelMaxDecibels(float level)
	{
		return 20f * Mathf.Log10(Mathf.Abs(level));
	}

	// Token: 0x060002EE RID: 750 RVA: 0x00012CD4 File Offset: 0x00010ED4
	private void ProcessMicData(float[] buffer)
	{
		if (!this.audioSource)
		{
			return;
		}
		if (!this.isGhost && this.character != null && (this.character.data.dead || this.character.data.passedOut))
		{
			return;
		}
		float num = AnimatedMouth.MicrophoneLevelMaxDecibels(AnimatedMouth.MicrophoneLevelMax(buffer));
		if (this.character != null && this.character.IsLocal && this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.PushToTalk && !this.character.input.pushToTalkPressed)
		{
			num = -80f;
		}
		float num2 = this.decibelToAmountCurve.Evaluate(num);
		if (num2 > this.amplitudePeakLimiter)
		{
			this.amplitudePeakLimiter = num2;
		}
		if (this.amplitudePeakLimiter > this.minAmplitudeThreshold)
		{
			this.amplitudePeakLimiter -= this.amplitudeHighestDecay * Time.deltaTime;
		}
		this.volume = num2 / this.amplitudePeakLimiter;
		if (this.volume > this.volumePeak)
		{
			this.volumePeak = this.volume;
		}
		this.volumePeak = Mathf.Lerp(this.volumePeak, 0f, Time.deltaTime * this.amplitudeSmoothing);
		if (this.volumePeak > this.talkThreshold)
		{
			this.mouthRenderer.material.SetInt("_UseTalkSprites", 1);
			this.isSpeaking = true;
		}
		else
		{
			this.isSpeaking = false;
			this.mouthRenderer.material.SetInt("_UseTalkSprites", 0);
		}
		this.amplitudeIndex = (int)(Mathf.Clamp01(this.volumePeak * this.amplitudeMult) * (float)(this.mouthTextures.Length - 1));
		this.mouthRenderer.material.SetTexture("_TalkSprite", this.mouthTextures[this.amplitudeIndex]);
	}

	// Token: 0x04000384 RID: 900
	public AnimationCurve decibelToAmountCurve = AnimationCurve.EaseInOut(-80f, 0f, 12f, 1f);

	// Token: 0x04000385 RID: 901
	public bool isSpeaking;

	// Token: 0x04000386 RID: 902
	public AudioSource audioSource;

	// Token: 0x04000387 RID: 903
	public Vector2 BandPassFilter;

	// Token: 0x04000388 RID: 904
	[FormerlySerializedAs("amplitude")]
	[Range(0f, 1f)]
	public float volume;

	// Token: 0x04000389 RID: 905
	[FormerlySerializedAs("amplitudeHighest")]
	public float amplitudePeakLimiter;

	// Token: 0x0400038A RID: 906
	public float minAmplitudeThreshold = 0.5f;

	// Token: 0x0400038B RID: 907
	public float amplitudeHighestDecay = 0.01f;

	// Token: 0x0400038C RID: 908
	public float amplitudeSmoothing = 0.2f;

	// Token: 0x0400038D RID: 909
	public float talkThreshold = 0.1f;

	// Token: 0x0400038E RID: 910
	public float amplitudeMult;

	// Token: 0x0400038F RID: 911
	[HideInInspector]
	public int amplitudeIndex;

	// Token: 0x04000390 RID: 912
	[FormerlySerializedAs("textures")]
	[Header("Mouth Cards")]
	public Texture2D[] mouthTextures;

	// Token: 0x04000391 RID: 913
	public Renderer mouthRenderer;

	// Token: 0x04000392 RID: 914
	public Character character;

	// Token: 0x04000393 RID: 915
	public bool isGhost;

	// Token: 0x04000394 RID: 916
	private float volumePeak;

	// Token: 0x04000395 RID: 917
	private PushToTalkSetting pushToTalkSetting;

	// Token: 0x04000396 RID: 918
	private float[] m_lastSentLocalBuffer;
}
