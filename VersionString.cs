using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x0200017D RID: 381
public class VersionString : MonoBehaviour
{
	// Token: 0x06000AA8 RID: 2728 RVA: 0x00033D8A File Offset: 0x00031F8A
	private void Start()
	{
		this.m_text = base.GetComponent<TextMeshProUGUI>();
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x00033D98 File Offset: 0x00031F98
	private void Update()
	{
		this.m_text.text = "v" + Application.version;
		if (PhotonNetwork.InRoom)
		{
			ConnectionService service = GameHandler.GetService<ConnectionService>();
			if (service != null)
			{
				InRoomState inRoomState = service.StateMachine.CurrentState as InRoomState;
				if (inRoomState != null && !string.IsNullOrEmpty(inRoomState.verifiedLobby))
				{
					TextMeshProUGUI text = this.m_text;
					text.text = string.Concat(new string[]
					{
						text.text,
						" - ",
						PhotonNetwork.CloudRegion,
						" - ",
						inRoomState.verifiedLobby
					});
				}
			}
		}
	}

	// Token: 0x04000986 RID: 2438
	private TextMeshProUGUI m_text;
}
