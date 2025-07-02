using System;
using UnityEngine;
using Zorro.Core;
using Zorro.UI.Modal;

// Token: 0x02000101 RID: 257
public class NextLevelService : GameService<NextLevelService>
{
	// Token: 0x0600079B RID: 1947 RVA: 0x00028888 File Offset: 0x00026A88
	public void NewData(LoginResponse response)
	{
		this.Data = Optionable<NextLevelService.NextLevelData>.Some(new NextLevelService.NextLevelData(response));
		Debug.Log("Setting new NextLevelData: " + this.Data.Value.ToString());
	}

	// Token: 0x04000719 RID: 1817
	public Optionable<NextLevelService.NextLevelData> Data;

	// Token: 0x02000346 RID: 838
	public struct NextLevelData
	{
		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06001361 RID: 4961 RVA: 0x0005CAE8 File Offset: 0x0005ACE8
		public int SecondsLeft
		{
			get
			{
				float num = Time.realtimeSinceStartup - this.StartupTimeWhenQueried;
				float num2 = this.SecondsLeftFromQueryTime - num;
				QueryingGameTimeStatus queryingGameTimeStatus;
				if (num2 < 0f && !GameHandler.TryGetStatus<QueryingGameTimeStatus>(out queryingGameTimeStatus))
				{
					CloudAPI.CheckVersion(delegate(LoginResponse response)
					{
						GameHandler.GetService<NextLevelService>().NewData(response);
						if (!response.VersionOkay)
						{
							Modal.OpenModal(new DefaultHeaderModalOption("Version out of date", "Close the game, and update the game on steam..."), new ModalButtonsOption(new ModalButtonsOption.Option[]
							{
								new ModalButtonsOption.Option("Okay", null)
							}), new Action(Application.Quit));
						}
					});
				}
				return Mathf.RoundToInt(num2);
			}
		}

		// Token: 0x06001362 RID: 4962 RVA: 0x0005CB44 File Offset: 0x0005AD44
		public NextLevelData(LoginResponse login)
		{
			this.CurrentLevelIndex = login.LevelIndex;
			this.StartupTimeWhenQueried = Time.realtimeSinceStartup;
			float num = (float)(login.HoursUntilLevel * 60 * 60 + login.MinutesUntilLevel * 60 + login.SecondsUntilLevel);
			this.SecondsLeftFromQueryTime = num;
			this.DevMessage = login.Message;
		}

		// Token: 0x06001363 RID: 4963 RVA: 0x0005CB9A File Offset: 0x0005AD9A
		public override string ToString()
		{
			return string.Format("CurrentIndex: {0}, seconds left {1}", this.CurrentLevelIndex, this.SecondsLeft);
		}

		// Token: 0x04001213 RID: 4627
		public int CurrentLevelIndex;

		// Token: 0x04001214 RID: 4628
		public float StartupTimeWhenQueried;

		// Token: 0x04001215 RID: 4629
		public float SecondsLeftFromQueryTime;

		// Token: 0x04001216 RID: 4630
		public string DevMessage;
	}
}
