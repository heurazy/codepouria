using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zorro.UI;
using Zorro.UI.Modal;

// Token: 0x0200015D RID: 349
public class MainMenuPageHandler : UIPageHandler
{
	// Token: 0x060009EF RID: 2543 RVA: 0x00031907 File Offset: 0x0002FB07
	protected override void Start()
	{
		base.Start();
		GameHandler.GetService<ConnectionService>().StateMachine.SwitchState<DefaultConnectionState>(false);
		CloudAPI.CheckVersion(delegate(LoginResponse response)
		{
			GameHandler.GetService<NextLevelService>().NewData(response);
			if (!response.VersionOkay)
			{
				Modal.OpenModal(new DefaultHeaderModalOption("Version out of date", "Close the game, and update the game on steam..."), new ModalButtonsOption(new ModalButtonsOption.Option[]
				{
					new ModalButtonsOption.Option("Okay", null)
				}), new Action(Application.Quit));
				return;
			}
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if (commandLineArgs.Length >= 2)
			{
				int i = 0;
				while (i < commandLineArgs.Length - 1)
				{
					Debug.Log("Parsing arg: " + commandLineArgs[i]);
					if (commandLineArgs[i].ToLower() == "+connect_lobby")
					{
						MainMenuPageHandler.<>c__DisplayClass4_0 CS$<>8__locals1 = new MainMenuPageHandler.<>c__DisplayClass4_0();
						if (ulong.TryParse(commandLineArgs[i + 1], out CS$<>8__locals1.lobbyID) && CS$<>8__locals1.lobbyID > 0UL)
						{
							base.StartCoroutine(CS$<>8__locals1.<Start>g__ConnectSoon|1());
							return;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
		});
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x00031934 File Offset: 0x0002FB34
	private void Update()
	{
		if (this.BackReference.action.WasPerformedThisFrame())
		{
			IHaveParentPage haveParentPage = this.currentPage as IHaveParentPage;
			if (haveParentPage != null)
			{
				ValueTuple<UIPage, PageTransistion> parentPage = haveParentPage.GetParentPage();
				UIPage item = parentPage.Item1;
				PageTransistion item2 = parentPage.Item2;
				base.TransistionToPage(item, item2);
			}
		}
		if ((PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer && !(this.currentPage is MainMenuFirstTimeSetupPage)) || PhotonNetwork.OfflineMode)
		{
			this.IntroAnimation.SetBool(MainMenuPageHandler.Connected, true);
		}
		IsDisconnectingForOfflineMode isDisconnectingForOfflineMode;
		if (!this.disconnected && PhotonNetwork.NetworkClientState == ClientState.Disconnected && !PhotonNetwork.OfflineMode && !GameHandler.TryGetStatus<IsDisconnectingForOfflineMode>(out isDisconnectingForOfflineMode))
		{
			this.disconnected = true;
			Debug.Log("Opening disconnected modal");
			HeaderModalOption headerModalOption = new DefaultHeaderModalOption("Failed to connect to Photon Network", "Try to connect again or play in offline mode?");
			ModalButtonsOption.Option[] array = new ModalButtonsOption.Option[2];
			array[0] = new ModalButtonsOption.Option("Try again", delegate
			{
				PhotonNetwork.OfflineMode = false;
				NetworkConnector.ConnectToPhoton();
				base.StartCoroutine(this.<Update>g__Timeout|6_2());
			});
			array[1] = new ModalButtonsOption.Option("Play offline", delegate
			{
				PhotonNetwork.OfflineMode = true;
			});
			Modal.OpenModal(headerModalOption, new ModalButtonsOption(array), null);
		}
		this.ConnectingInfoText.text = this.GetPrettyStateName();
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00031A5C File Offset: 0x0002FC5C
	private string GetPrettyStateName()
	{
		ClientState networkClientState = PhotonNetwork.NetworkClientState;
		if (networkClientState != ClientState.Authenticating)
		{
			switch (networkClientState)
			{
			case ClientState.ConnectingToMasterServer:
			case ClientState.ConnectingToNameServer:
			case ClientState.ConnectedToNameServer:
				return "Connecting...";
			case ClientState.ConnectedToMasterServer:
				return "";
			}
			return networkClientState.ToString();
		}
		return "Authenticating...";
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00031BB6 File Offset: 0x0002FDB6
	[CompilerGenerated]
	private IEnumerator <Update>g__Timeout|6_2()
	{
		yield return new WaitForSecondsRealtime(5f);
		this.disconnected = false;
		yield break;
	}

	// Token: 0x040008E7 RID: 2279
	private static readonly int Connected = Animator.StringToHash("Connected");

	// Token: 0x040008E8 RID: 2280
	public InputActionReference BackReference;

	// Token: 0x040008E9 RID: 2281
	public Animator IntroAnimation;

	// Token: 0x040008EA RID: 2282
	public TextMeshProUGUI ConnectingInfoText;

	// Token: 0x040008EB RID: 2283
	private bool disconnected;
}
