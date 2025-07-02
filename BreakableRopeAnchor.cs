using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000199 RID: 409
public class BreakableRopeAnchor : MonoBehaviour
{
	// Token: 0x06000B40 RID: 2880 RVA: 0x00037A24 File Offset: 0x00035C24
	private void Awake()
	{
		this.anchor = base.GetComponent<RopeAnchorWithRope>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x00037A3E File Offset: 0x00035C3E
	private void Start()
	{
		this.willBreakInTime = this.breakableTimeMinMax.PRndRange();
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x00037A54 File Offset: 0x00035C54
	private void Update()
	{
		if (!this.photonView.IsMine)
		{
			return;
		}
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		int num = 0;
		foreach (Character character in allPlayerCharacters)
		{
			if (character.data.isRopeClimbing && character.data.heldRope == this.anchor.rope)
			{
				num++;
			}
		}
		if (num > 0)
		{
			this.willBreakInTime -= Time.deltaTime;
		}
		if (this.willBreakInTime > 0f)
		{
			return;
		}
		if (this.isBreaking)
		{
			return;
		}
		base.StartCoroutine(this.<Update>g__Break|9_0());
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x00037B4B File Offset: 0x00035D4B
	[CompilerGenerated]
	private IEnumerator <Update>g__Break|9_0()
	{
		this.isBreaking = true;
		Debug.Log(string.Format("Break: segments {0}", this.anchor.rope.Segments));
		this.anchor.rope.Segments += this.dropSegments;
		float elapsed = 0f;
		float startSegments = this.anchor.rope.Segments;
		while (elapsed < this.breakAnimTime)
		{
			elapsed += Time.deltaTime;
			this.anchor.rope.Segments = Mathf.Lerp(startSegments, startSegments + 1f, elapsed / 0.5f);
			yield return null;
		}
		Debug.Log("Detach_Rpc");
		this.anchor.rope.photonView.RPC("Detach_Rpc", RpcTarget.AllBuffered, Array.Empty<object>());
		yield break;
	}

	// Token: 0x04000A53 RID: 2643
	public float breakAnimTime = 3f;

	// Token: 0x04000A54 RID: 2644
	public Vector2 breakableTimeMinMax = new Vector2(3f, 8f);

	// Token: 0x04000A55 RID: 2645
	public float dropSegments = 1f;

	// Token: 0x04000A56 RID: 2646
	private float willBreakInTime;

	// Token: 0x04000A57 RID: 2647
	private RopeAnchorWithRope anchor;

	// Token: 0x04000A58 RID: 2648
	private PhotonView photonView;

	// Token: 0x04000A59 RID: 2649
	private bool isBreaking;
}
