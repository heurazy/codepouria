using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x020002C6 RID: 710
	public class ChannelSelector : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x0600118C RID: 4492 RVA: 0x000566E5 File Offset: 0x000548E5
		public void SetChannel(string channel)
		{
			this.Channel = channel;
			base.GetComponentInChildren<Text>().text = this.Channel;
		}

		// Token: 0x0600118D RID: 4493 RVA: 0x000566FF File Offset: 0x000548FF
		public void OnPointerClick(PointerEventData eventData)
		{
			Object.FindFirstObjectByType<ChatGui>().ShowChannel(this.Channel);
		}

		// Token: 0x04000FFB RID: 4091
		public string Channel;
	}
}
