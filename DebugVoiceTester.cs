using System;
using UnityEngine;

// Token: 0x020001B9 RID: 441
public class DebugVoiceTester : MonoBehaviour
{
	// Token: 0x06000C13 RID: 3091 RVA: 0x0003C7E0 File Offset: 0x0003A9E0
	private void Start()
	{
		this.audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 44100);
		this.audioSource.loop = true;
		while (Microphone.GetPosition(null) <= 0)
		{
		}
		this.audioSource.Play();
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x0003C82C File Offset: 0x0003AA2C
	private void Update()
	{
	}

	// Token: 0x04000B16 RID: 2838
	public AudioSource audioSource;
}
