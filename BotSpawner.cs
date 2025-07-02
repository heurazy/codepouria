using System;
using System.Runtime.CompilerServices;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200004C RID: 76
public class BotSpawner : MonoBehaviour
{
	// Token: 0x06000371 RID: 881 RVA: 0x0001503C File Offset: 0x0001323C
	private void Go()
	{
		this.SpawnBot(PatrolBoss.me.transform.position);
	}

	// Token: 0x06000372 RID: 882 RVA: 0x00015054 File Offset: 0x00013254
	public void SpawnBot(Vector3 spawnPosition)
	{
		bool flag = false;
		for (int i = 0; i < 10; i++)
		{
			if (this.<SpawnBot>g__TrySpawnBot|2_0(spawnPosition + ExtMath.RandInsideUnitCircle().xoy() * 2f))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Debug.LogWarning("Could not spawn troop");
		}
	}

	// Token: 0x06000374 RID: 884 RVA: 0x000150AC File Offset: 0x000132AC
	[CompilerGenerated]
	private bool <SpawnBot>g__TrySpawnBot|2_0(Vector3 spawnPosition)
	{
		foreach (Collider collider in Physics.OverlapSphere(spawnPosition, 2f))
		{
			if (collider.gameObject.layer != LayerMask.NameToLayer("Terrain") && collider.gameObject.layer != LayerMask.NameToLayer("Prop"))
			{
				return false;
			}
		}
		Object.Instantiate<GameObject>(this.botPrefab, spawnPosition, Quaternion.identity);
		Debug.Log("Spawn Bot");
		return true;
	}

	// Token: 0x040003FF RID: 1023
	public GameObject botPrefab;
}
