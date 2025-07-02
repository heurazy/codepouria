using System;
using UnityEngine;
using UnityEngine.Networking;
using Zorro.Core;

// Token: 0x02000056 RID: 86
public static class CloudAPI
{
	// Token: 0x060003B5 RID: 949 RVA: 0x0001676C File Offset: 0x0001496C
	public static void CheckVersion(Action<LoginResponse> response)
	{
		GameHandler.AddStatus<QueryingGameTimeStatus>(new QueryingGameTimeStatus());
		BuildVersion buildVersion = new BuildVersion(Application.version);
		string text = "https://peaklogin.azurewebsites.net/api/VersionCheck?version=" + buildVersion.ToMatchmaking();
		Debug.Log("Sending GET Request to: " + text);
		UnityWebRequest request = UnityWebRequest.Get(text);
		request.SendWebRequest().completed += delegate(AsyncOperation _)
		{
			GameHandler.ClearStatus<QueryingGameTimeStatus>();
			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("Got error: " + request.error);
				if (request.result != UnityWebRequest.Result.ConnectionError)
				{
					Action<LoginResponse> response2 = response;
					if (response2 == null)
					{
						return;
					}
					response2(new LoginResponse
					{
						VersionOkay = false
					});
				}
				return;
			}
			string text2 = request.downloadHandler.text;
			LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(text2);
			Debug.Log("Got message: " + text2);
			Action<LoginResponse> response3 = response;
			if (response3 == null)
			{
				return;
			}
			response3(loginResponse);
		};
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x000167E8 File Offset: 0x000149E8
	public static void VerifyLobby(ulong lobbyID, Action<string> response)
	{
		string text = "https://peaklogin.azurewebsites.net/api/VerifyLobby?lobby=" + lobbyID.ToString();
		Debug.Log("Sending GET Request to: " + text);
		UnityWebRequest request = UnityWebRequest.Get(text);
		request.SendWebRequest().completed += delegate(AsyncOperation _)
		{
			GameHandler.ClearStatus<QueryingGameTimeStatus>();
			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("Failed to verify lobby: " + request.error);
				return;
			}
			string text2 = request.downloadHandler.text;
			Debug.Log("Got message: " + text2);
			Action<string> response2 = response;
			if (response2 == null)
			{
				return;
			}
			response2(text2);
		};
	}
}
