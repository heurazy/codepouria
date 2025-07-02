using System;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	// Token: 0x020002CB RID: 715
	[RequireComponent(typeof(ChatGui))]
	public class NamePickGui : MonoBehaviour
	{
		// Token: 0x060011B9 RID: 4537 RVA: 0x000574C4 File Offset: 0x000556C4
		public void Start()
		{
			this.chatNewComponent = Object.FindFirstObjectByType<ChatGui>();
			string @string = PlayerPrefs.GetString("NamePickUserName");
			if (!string.IsNullOrEmpty(@string))
			{
				this.idInput.text = @string;
			}
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x000574FB File Offset: 0x000556FB
		public void EndEditOnEnter()
		{
			if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
			{
				this.StartChat();
			}
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x00057518 File Offset: 0x00055718
		public void StartChat()
		{
			ChatGui chatGui = Object.FindFirstObjectByType<ChatGui>();
			chatGui.UserName = this.idInput.text.Trim();
			chatGui.Connect();
			base.enabled = false;
			PlayerPrefs.SetString("NamePickUserName", chatGui.UserName);
		}

		// Token: 0x04001019 RID: 4121
		private const string UserNamePlayerPref = "NamePickUserName";

		// Token: 0x0400101A RID: 4122
		public ChatGui chatNewComponent;

		// Token: 0x0400101B RID: 4123
		public InputField idInput;
	}
}
