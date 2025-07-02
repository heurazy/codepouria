using System;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x020002C9 RID: 713
	public class FriendItem : MonoBehaviour
	{
		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060011B3 RID: 4531 RVA: 0x000573DE File Offset: 0x000555DE
		// (set) Token: 0x060011B2 RID: 4530 RVA: 0x000573D0 File Offset: 0x000555D0
		[HideInInspector]
		public string FriendId
		{
			get
			{
				return this.NameLabel.text;
			}
			set
			{
				this.NameLabel.text = value;
			}
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x000573EB File Offset: 0x000555EB
		public void Awake()
		{
			this.Health.text = string.Empty;
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x00057400 File Offset: 0x00055600
		public void OnFriendStatusUpdate(int status, bool gotMessage, object message)
		{
			string text;
			switch (status)
			{
			case 1:
				text = "Invisible";
				break;
			case 2:
				text = "Online";
				break;
			case 3:
				text = "Away";
				break;
			case 4:
				text = "Do not disturb";
				break;
			case 5:
				text = "Looking For Game/Group";
				break;
			case 6:
				text = "Playing";
				break;
			default:
				text = "Offline";
				break;
			}
			this.StatusLabel.text = text;
			if (gotMessage)
			{
				string text2 = string.Empty;
				if (message != null)
				{
					string[] array = message as string[];
					if (array != null && array.Length >= 2)
					{
						text2 = array[1] + "%";
					}
				}
				this.Health.text = text2;
			}
		}

		// Token: 0x04001016 RID: 4118
		public Text NameLabel;

		// Token: 0x04001017 RID: 4119
		public Text StatusLabel;

		// Token: 0x04001018 RID: 4120
		public Text Health;
	}
}
