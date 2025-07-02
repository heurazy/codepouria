using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zorro.Core;
using Zorro.UI;

// Token: 0x0200015E RID: 350
public class MainMenuPlayPage : UIPage, IHaveParentPage
{
	// Token: 0x060009F7 RID: 2551 RVA: 0x00031BC5 File Offset: 0x0002FDC5
	private void Start()
	{
		this.m_playButton.onClick.AddListener(new UnityAction(this.PlayClicked));
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x00031BE3 File Offset: 0x0002FDE3
	public ValueTuple<UIPage, PageTransistion> GetParentPage()
	{
		return new ValueTuple<UIPage, PageTransistion>(this.pageHandler.GetPage<MainMenuMainPage>(), new SetActivePageTransistion());
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x00031BFC File Offset: 0x0002FDFC
	public void PlayClicked()
	{
		if (string.IsNullOrEmpty(this.m_usernameField.text))
		{
			Debug.LogError("Failed to get username field...");
			return;
		}
		if (string.IsNullOrEmpty(this.m_roomField.text))
		{
			Debug.LogError("Failed to get room name field...");
			return;
		}
		JoinSpecificRoomState joinSpecificRoomState = GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<JoinSpecificRoomState>(false);
		joinSpecificRoomState.RoomName = this.m_roomField.text.ToLower();
		joinSpecificRoomState.RegionToJoin = "eu";
		RetrievableResourceSingleton<LoadingScreenHandler>.Instance.Load(LoadingScreen.LoadingScreenType.Basic, null, new IEnumerator[] { RetrievableResourceSingleton<LoadingScreenHandler>.Instance.LoadSceneProcess("Airport", false, true, 3f) });
	}

	// Token: 0x040008EC RID: 2284
	[SerializeField]
	private Button m_playButton;

	// Token: 0x040008ED RID: 2285
	[SerializeField]
	private TMP_InputField m_usernameField;

	// Token: 0x040008EE RID: 2286
	[SerializeField]
	private TMP_InputField m_roomField;
}
