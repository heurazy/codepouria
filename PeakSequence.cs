using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200020E RID: 526
public class PeakSequence : MonoBehaviour
{
	// Token: 0x06000D9E RID: 3486 RVA: 0x000448C2 File Offset: 0x00042AC2
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000D9F RID: 3487 RVA: 0x000448D0 File Offset: 0x00042AD0
	private void OnDisable()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.Log("Destroying ropes");
			if (this.ropeAnchorInstance != null)
			{
				PhotonNetwork.Destroy(this.ropeAnchorInstance.photonView);
			}
			if (this.ropeInstance != null)
			{
				PhotonNetwork.Destroy(this.ropeInstance.photonView);
				return;
			}
		}
		else
		{
			this.ropeAnchorInstance.gameObject.SetActive(false);
			this.ropeInstance.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000DA0 RID: 3488 RVA: 0x00044950 File Offset: 0x00042B50
	private void Update()
	{
		if (this.waitTime > this.timeToWait)
		{
			if (!this.spawnedRope)
			{
				if (PhotonNetwork.IsMasterClient)
				{
					this.spawnedRope = true;
					GameObject gameObject = PhotonNetwork.Instantiate(this.ropeAnchorWithRopePref.name, this.ropeSpawnPoint.position, Quaternion.identity, 0, null);
					this.ropeAnchorInstance = gameObject.GetComponent<RopeAnchorWithRope>();
					this.ropeAnchorInstance.ropeSegmentLength = 40f;
					Rope rope = this.ropeAnchorInstance.SpawnRope();
					this.view.RPC("SetRopeToClients", RpcTarget.All, new object[] { rope.GetComponent<PhotonView>() });
				}
			}
			else
			{
				this.CheckGameComplete();
			}
		}
		this.waitTime += Time.deltaTime;
	}

	// Token: 0x06000DA1 RID: 3489 RVA: 0x00044A0C File Offset: 0x00042C0C
	private void CheckGameComplete()
	{
		if (this.endingGame)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			int num = 0;
			List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
			for (int i = allPlayerCharacters.Count - 1; i >= 0; i--)
			{
				if (allPlayerCharacters[i].data.dead)
				{
					allPlayerCharacters.RemoveAt(i);
				}
			}
			List<Character> list = new List<Character>();
			foreach (Character character in allPlayerCharacters)
			{
				if (character.data.fullyConscious)
				{
					list.Add(character);
				}
			}
			for (int j = 0; j < allPlayerCharacters.Count; j++)
			{
				if (Character.CheckWinCondition(allPlayerCharacters[j]))
				{
					num++;
				}
			}
			if (num > 0)
			{
				this.timerElapsed += Time.deltaTime;
				if (this.timerElapsed >= this.lengthOfASecond)
				{
					if (num >= list.Count && this.secondsElapsed < this.totalSeconds - this.totalWinningSeconds)
					{
						this.secondsElapsed = this.totalSeconds - this.totalWinningSeconds;
					}
					this.timerElapsed = 0f;
					this.view.RPC("RPCUpdateTimer", RpcTarget.All, new object[] { this.secondsElapsed });
					this.secondsElapsed++;
					if (this.secondsElapsed > this.totalSeconds)
					{
						this.endingGame = true;
						Character.localCharacter.EndGame();
						return;
					}
				}
			}
			else
			{
				this.secondsElapsed = 0;
				this.timerElapsed = 0f;
				this.view.RPC("RPCUpdateTimer", RpcTarget.All, new object[] { -1 });
			}
		}
	}

	// Token: 0x06000DA2 RID: 3490 RVA: 0x00044BCC File Offset: 0x00042DCC
	[PunRPC]
	public void SetRopeToClients(PhotonView v)
	{
		this.ropeInstance = v.GetComponent<Rope>();
		Debug.Log(string.Format("ROPE AS BEEN SET TO {0}", this.ropeInstance));
	}

	// Token: 0x06000DA3 RID: 3491 RVA: 0x00044BEF File Offset: 0x00042DEF
	[PunRPC]
	private void RPCUpdateTimer(int seconds)
	{
		if (seconds == -1)
		{
			GUIManager.instance.endgame.Disable();
			return;
		}
		GUIManager.instance.endgame.UpdateCounter(this.totalSeconds - seconds);
	}

	// Token: 0x04000CAF RID: 3247
	private PhotonView view;

	// Token: 0x04000CB0 RID: 3248
	public GameObject ropeAnchorWithRopePref;

	// Token: 0x04000CB1 RID: 3249
	public Transform ropeSpawnPoint;

	// Token: 0x04000CB2 RID: 3250
	private float waitTime;

	// Token: 0x04000CB3 RID: 3251
	public float timeToWait = 5f;

	// Token: 0x04000CB4 RID: 3252
	public int totalSeconds = 30;

	// Token: 0x04000CB5 RID: 3253
	public int totalWinningSeconds = 5;

	// Token: 0x04000CB6 RID: 3254
	public float lengthOfASecond = 1.5f;

	// Token: 0x04000CB7 RID: 3255
	private bool spawnedRope;

	// Token: 0x04000CB8 RID: 3256
	public RopeAnchorWithRope ropeAnchorInstance;

	// Token: 0x04000CB9 RID: 3257
	public Rope ropeInstance;

	// Token: 0x04000CBA RID: 3258
	private float timerElapsed;

	// Token: 0x04000CBB RID: 3259
	private int secondsElapsed;

	// Token: 0x04000CBC RID: 3260
	private bool endingGame;
}
