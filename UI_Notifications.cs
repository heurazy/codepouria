using System;
using TMPro;
using UnityEngine;

// Token: 0x02000292 RID: 658
public class UI_Notifications : MonoBehaviour
{
	// Token: 0x06000FB9 RID: 4025 RVA: 0x0004F9C0 File Offset: 0x0004DBC0
	public void AddNotification(string text)
	{
		Transform child = base.transform.GetChild(0);
		Object.Instantiate<GameObject>(this.prefab, child.position, child.rotation, child).GetComponentInChildren<TextMeshProUGUI>().text = text;
	}

	// Token: 0x04000EBF RID: 3775
	public GameObject prefab;
}
