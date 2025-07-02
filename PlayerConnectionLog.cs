using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

// Token: 0x0200016D RID: 365
public class PlayerConnectionLog : MonoBehaviourPunCallbacks
{
	// Token: 0x06000A58 RID: 2648 RVA: 0x000326B8 File Offset: 0x000308B8
	private void RebuildString()
	{
		this.sb.Clear();
		foreach (string text in this.currentLog)
		{
			this.sb.Append(text);
			this.sb.Append("\n");
		}
		this.text.text = this.sb.ToString();
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x00032744 File Offset: 0x00030944
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		if (!newPlayer.IsLocal && newPlayer.NickName == "Bing Bong")
		{
			return;
		}
		this.AddMessage(string.Concat(new string[]
		{
			this.GetColorTag(this.userColor),
			" ",
			newPlayer.NickName,
			"</color>",
			this.GetColorTag(this.joinedColor),
			" joined the expedition</color>"
		}));
		if (this.sfxJoin)
		{
			this.sfxJoin.Play(default(Vector3));
		}
	}

	// Token: 0x06000A5A RID: 2650 RVA: 0x000327E0 File Offset: 0x000309E0
	public override void OnPlayerLeftRoom(Photon.Realtime.Player newPlayer)
	{
		if (!newPlayer.IsLocal)
		{
			if (newPlayer.NickName == "Bing Bong")
			{
				return;
			}
			this.AddMessage(string.Concat(new string[]
			{
				this.GetColorTag(this.userColor),
				newPlayer.NickName,
				"</color>",
				this.GetColorTag(this.leftColor),
				" left the expedition</color>"
			}));
			if (this.sfxLeave)
			{
				this.sfxLeave.Play(default(Vector3));
			}
		}
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x00032871 File Offset: 0x00030A71
	public void TestAddJoin()
	{
		this.AddMessage(this.GetColorTag(this.userColor) + "TestPlayer</color>" + this.GetColorTag(this.joinedColor) + " joined the expedition</color>");
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x000328A0 File Offset: 0x00030AA0
	public void TestAddLeft()
	{
		this.AddMessage(this.GetColorTag(this.userColor) + "TestPlayer</color>" + this.GetColorTag(this.leftColor) + " left the expedition</color>");
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x000328CF File Offset: 0x00030ACF
	private string GetColorTag(Color c)
	{
		return "<color=#" + ColorUtility.ToHtmlStringRGB(c) + ">";
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x000328E6 File Offset: 0x00030AE6
	private void AddMessage(string s)
	{
		this.currentLog.Add(s);
		this.RebuildString();
		base.StartCoroutine(this.TimeoutMessageRoutine());
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x00032907 File Offset: 0x00030B07
	private IEnumerator TimeoutMessageRoutine()
	{
		yield return new WaitForSeconds(8f);
		this.currentLog.RemoveAt(0);
		this.RebuildString();
		yield break;
	}

	// Token: 0x0400091B RID: 2331
	public TextMeshProUGUI text;

	// Token: 0x0400091C RID: 2332
	private List<string> currentLog = new List<string>();

	// Token: 0x0400091D RID: 2333
	private StringBuilder sb = new StringBuilder();

	// Token: 0x0400091E RID: 2334
	public Color joinedColor;

	// Token: 0x0400091F RID: 2335
	public Color leftColor;

	// Token: 0x04000920 RID: 2336
	public Color userColor;

	// Token: 0x04000921 RID: 2337
	public SFX_Instance sfxJoin;

	// Token: 0x04000922 RID: 2338
	public SFX_Instance sfxLeave;
}
