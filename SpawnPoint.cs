using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200002F RID: 47
public class SpawnPoint : MonoBehaviour
{
	// Token: 0x06000295 RID: 661 RVA: 0x000117FD File Offset: 0x0000F9FD
	private void Awake()
	{
		SpawnPoint.allSpawnPoints.Add(this);
	}

	// Token: 0x06000296 RID: 662 RVA: 0x0001180A File Offset: 0x0000FA0A
	private void OnDestroy()
	{
		SpawnPoint.allSpawnPoints.Remove(this);
	}

	// Token: 0x04000317 RID: 791
	public int index;

	// Token: 0x04000318 RID: 792
	public bool startPassedOut;

	// Token: 0x04000319 RID: 793
	public static List<SpawnPoint> allSpawnPoints = new List<SpawnPoint>();
}
