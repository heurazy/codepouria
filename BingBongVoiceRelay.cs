using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000048 RID: 72
public class BingBongVoiceRelay : MonoBehaviourPunCallbacks
{
	// Token: 0x0600034A RID: 842 RVA: 0x000143FF File Offset: 0x000125FF
	private void Awake()
	{
		this.m_photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600034B RID: 843 RVA: 0x00014410 File Offset: 0x00012610
	private void LateUpdate()
	{
		bool flag = false;
		if (Singleton<PeakHandler>.Instance != null && Singleton<PeakHandler>.Instance.isPlayingCinematic)
		{
			flag = true;
		}
		BingBong instance = BingBong.Instance;
		Optionable<Vector3> optionable = Optionable<Vector3>.None;
		if (instance == null)
		{
			using (List<Player>.Enumerator enumerator = PlayerHandler.GetAllPlayers().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Player player = enumerator.Current;
					byte b;
					if (player.HasInAnySlot(this.BingBongItem.itemID, out b) && player.character != null)
					{
						optionable = Optionable<Vector3>.Some(player.character.Center);
						break;
					}
				}
				goto IL_00EB;
			}
		}
		float[] array = new float[256];
		this.m_source.GetSpectrumData(array, 0, FFTWindow.Rectangular);
		float num = AnimatedMouth.MicrophoneLevelMaxDecibels(AnimatedMouth.MicrophoneLevelMax(array));
		float num2 = this.dbToOpenCurve.Evaluate(num);
		instance.SetVoiceData(num2);
		optionable = Optionable<Vector3>.Some(instance.transform.position);
		IL_00EB:
		this.m_source.spatialBlend = (flag ? 0f : (optionable.IsSome ? 1f : 0f));
		this.m_source.volume = (optionable.IsSome ? 0.45f : 0f);
		if (optionable.IsSome)
		{
			this.m_source.transform.position = optionable.Value;
		}
	}

	// Token: 0x040003DA RID: 986
	public AnimationCurve dbToOpenCurve;

	// Token: 0x040003DB RID: 987
	public Item BingBongItem;

	// Token: 0x040003DC RID: 988
	public AudioSource m_source;

	// Token: 0x040003DD RID: 989
	private PhotonView m_photonView;
}
