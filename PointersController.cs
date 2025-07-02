using System;
using Photon.Voice.PUN;
using UnityEngine;

// Token: 0x02000213 RID: 531
[RequireComponent(typeof(PhotonVoiceView))]
public class PointersController : MonoBehaviour
{
	// Token: 0x06000DB0 RID: 3504 RVA: 0x00044F0F File Offset: 0x0004310F
	private void Awake()
	{
		this.photonVoiceView = base.GetComponent<PhotonVoiceView>();
		this.SetActiveSafe(this.pointerUp, false);
		this.SetActiveSafe(this.pointerDown, false);
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x00044F37 File Offset: 0x00043137
	private void Update()
	{
		this.SetActiveSafe(this.pointerDown, this.photonVoiceView.IsSpeaking);
		this.SetActiveSafe(this.pointerUp, this.photonVoiceView.IsRecording);
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x00044F67 File Offset: 0x00043167
	private void SetActiveSafe(GameObject go, bool active)
	{
		if (go != null && go.activeSelf != active)
		{
			go.SetActive(active);
		}
	}

	// Token: 0x04000CC3 RID: 3267
	[SerializeField]
	private GameObject pointerDown;

	// Token: 0x04000CC4 RID: 3268
	[SerializeField]
	private GameObject pointerUp;

	// Token: 0x04000CC5 RID: 3269
	private PhotonVoiceView photonVoiceView;
}
