using System;
using UnityEngine;

// Token: 0x02000254 RID: 596
public class RemoveMusic : MonoBehaviour
{
	// Token: 0x06000E79 RID: 3705 RVA: 0x00048BB8 File Offset: 0x00046DB8
	private void Start()
	{
		this.musics = GameObject.FindGameObjectsWithTag("Music");
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x00048BCC File Offset: 0x00046DCC
	private void Update()
	{
		for (int i = 0; i < this.musics.Length; i++)
		{
			if (this.musics[i] != null)
			{
				this.musics[i].GetComponent<AudioSource>().volume /= 1.01f;
			}
		}
	}

	// Token: 0x04000D77 RID: 3447
	public GameObject[] musics;
}
