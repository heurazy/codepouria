using System;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x02000265 RID: 613
public class SetCharacterPosition : MonoBehaviour
{
	// Token: 0x06000ED7 RID: 3799 RVA: 0x0004AC48 File Offset: 0x00048E48
	private void Go()
	{
		this.characterPrefab.transform.position = base.transform.position;
		PExt.SaveObj(this.characterPrefab);
	}

	// Token: 0x06000ED8 RID: 3800 RVA: 0x0004AC70 File Offset: 0x00048E70
	private void Start()
	{
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x0004AC72 File Offset: 0x00048E72
	private void Update()
	{
	}

	// Token: 0x04000DB5 RID: 3509
	public GameObject characterPrefab;
}
