using System;
using Photon.Voice;
using Photon.Voice.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using Zorro.Core;

// Token: 0x02000011 RID: 17
public class CharacterVoiceHandler : MonoBehaviour
{
	// Token: 0x17000016 RID: 22
	// (get) Token: 0x06000171 RID: 369 RVA: 0x0000BFD4 File Offset: 0x0000A1D4
	// (set) Token: 0x06000172 RID: 370 RVA: 0x0000BFDC File Offset: 0x0000A1DC
	internal AudioSource audioSource { get; private set; }

	// Token: 0x06000173 RID: 371 RVA: 0x0000BFE5 File Offset: 0x0000A1E5
	private void OnEnable()
	{
		GlobalEvents.OnCharacterAudioLevelsUpdated = (Action)Delegate.Combine(GlobalEvents.OnCharacterAudioLevelsUpdated, new Action(this.UpdateAudioLevel));
	}

	// Token: 0x06000174 RID: 372 RVA: 0x0000C007 File Offset: 0x0000A207
	private void OnDisable()
	{
		GlobalEvents.OnCharacterAudioLevelsUpdated = (Action)Delegate.Remove(GlobalEvents.OnCharacterAudioLevelsUpdated, new Action(this.UpdateAudioLevel));
	}

	// Token: 0x06000175 RID: 373 RVA: 0x0000C02C File Offset: 0x0000A22C
	private void UpdateAudioLevel()
	{
		if (AudioLevels.PlayerAudioLevels.ContainsKey(this.m_character.photonView.OwnerActorNr))
		{
			float num = AudioLevels.PlayerAudioLevels[this.m_character.photonView.OwnerActorNr];
			this.audioLevel = num;
			Debug.Log(string.Format("{0} set audio levels to {1}", this.m_character.characterName, num));
			return;
		}
		this.audioLevel = 0.5f;
	}

	// Token: 0x06000176 RID: 374 RVA: 0x0000C0A4 File Offset: 0x0000A2A4
	private void Start()
	{
		this.m_Recorder = base.GetComponent<Recorder>();
		this.m_character = base.GetComponentInParent<Character>();
		this.microphoneSetting = GameHandler.Instance.SettingsHandler.GetSetting<MicrophoneSetting>();
		this.pushToTalkSetting = GameHandler.Instance.SettingsHandler.GetSetting<PushToTalkSetting>();
		this.audioSource = base.GetComponent<AudioSource>();
		this.m_source = base.GetComponent<AudioSource>();
		if (this.m_character.IsLocal)
		{
			return;
		}
		byte b = PlayerHandler.AssignMixerGroup(this.m_character);
		if (b != 255)
		{
			this.m_source.outputAudioMixerGroup = this.GetMixerGroup(b);
			this.m_parameter = this.GetMixerGroupParameter(b);
		}
	}

	// Token: 0x06000177 RID: 375 RVA: 0x0000C14C File Offset: 0x0000A34C
	private AudioMixerGroup GetMixerGroup(byte group)
	{
		AudioMixerGroup audioMixerGroup;
		switch (group)
		{
		case 0:
			audioMixerGroup = this.m_mixerGroup1;
			break;
		case 1:
			audioMixerGroup = this.m_mixerGroup2;
			break;
		case 2:
			audioMixerGroup = this.m_mixerGroup3;
			break;
		case 3:
			audioMixerGroup = this.m_mixerGroup4;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		return audioMixerGroup;
	}

	// Token: 0x06000178 RID: 376 RVA: 0x0000C19C File Offset: 0x0000A39C
	private string GetMixerGroupParameter(byte group)
	{
		return "Voice" + ((int)(group + 1)).ToString() + "Effects";
	}

	// Token: 0x06000179 RID: 377 RVA: 0x0000C1C4 File Offset: 0x0000A3C4
	private void Update()
	{
		this.m_source.volume = (this.m_character.data.fullyConscious ? this.audioLevel : ((this.m_character.Ghost != null) ? this.audioLevel : 0f));
		this.PushToTalk();
		if (this.m_character.IsLocal && !this.m_character.isBot)
		{
			string id = this.microphoneSetting.Value.id;
			if (id != this.m_setMicrophoneDevice && !string.IsNullOrEmpty(id))
			{
				this.m_setMicrophoneDevice = id;
				this.m_Recorder.MicrophoneDevice = new DeviceInfo(id, null);
				Debug.Log("Setting microphone to " + id);
			}
		}
	}

	// Token: 0x0600017A RID: 378 RVA: 0x0000C288 File Offset: 0x0000A488
	private void PushToTalk()
	{
		bool flag = this.pushToTalkSetting.Value == PushToTalkSetting.PushToTalkType.VoiceActivation || this.m_character.input.pushToTalkPressed;
		if (flag != this.m_currentlyTransmitting || this.firstTime)
		{
			this.firstTime = false;
			this.m_currentlyTransmitting = flag;
			this.m_Recorder.TransmitEnabled = flag;
		}
	}

	// Token: 0x0600017B RID: 379 RVA: 0x0000C2E4 File Offset: 0x0000A4E4
	private void LateUpdate()
	{
		bool flag = false;
		if (Singleton<PeakHandler>.Instance != null && Singleton<PeakHandler>.Instance.isPlayingCinematic)
		{
			flag = true;
		}
		this.m_source.spatialBlend = (float)(flag ? 0 : 1);
		if (this.m_character.IsLocal)
		{
			return;
		}
		Vector3 vector = this.m_character.refs.head.transform.position;
		if (this.m_character.Ghost != null)
		{
			vector = this.m_character.Ghost.transform.position;
		}
		base.transform.position = vector;
		float num = math.saturate(LightVolume.Instance().SamplePositionAlpha(vector));
		num = math.saturate(1f - math.remap(0f, 0.3f, 0f, 1f, num));
		if (flag)
		{
		}
	}

	// Token: 0x04000169 RID: 361
	private Character m_character;

	// Token: 0x0400016A RID: 362
	[SerializeField]
	private AudioMixer m_mixer;

	// Token: 0x0400016C RID: 364
	[SerializeField]
	private AudioMixerGroup m_mixerGroup1;

	// Token: 0x0400016D RID: 365
	[SerializeField]
	private AudioMixerGroup m_mixerGroup2;

	// Token: 0x0400016E RID: 366
	[SerializeField]
	private AudioMixerGroup m_mixerGroup3;

	// Token: 0x0400016F RID: 367
	[SerializeField]
	private AudioMixerGroup m_mixerGroup4;

	// Token: 0x04000170 RID: 368
	private AudioSource m_source;

	// Token: 0x04000171 RID: 369
	private string m_parameter;

	// Token: 0x04000172 RID: 370
	private MicrophoneSetting microphoneSetting;

	// Token: 0x04000173 RID: 371
	private PushToTalkSetting pushToTalkSetting;

	// Token: 0x04000174 RID: 372
	private string m_setMicrophoneDevice;

	// Token: 0x04000175 RID: 373
	private Recorder m_Recorder;

	// Token: 0x04000176 RID: 374
	private bool m_currentlyTransmitting;

	// Token: 0x04000177 RID: 375
	private float audioLevel = 0.5f;

	// Token: 0x04000178 RID: 376
	private bool firstTime;

	// Token: 0x04000179 RID: 377
	public const float DEFAULT_VOICE_VOLUME = 0.5f;
}
