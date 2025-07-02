using System;
using UnityEngine;

// Token: 0x02000266 RID: 614
public class SetGlobalVariables : MonoBehaviour
{
	// Token: 0x06000EDB RID: 3803 RVA: 0x0004AC7C File Offset: 0x00048E7C
	private void Start()
	{
		foreach (StringAndFloat stringAndFloat in this.globalVariables)
		{
			PlayerPrefs.SetFloat(stringAndFloat.name, stringAndFloat.value);
		}
	}

	// Token: 0x04000DB6 RID: 3510
	public StringAndFloat[] globalVariables;
}
