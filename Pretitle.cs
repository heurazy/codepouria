using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Token: 0x02000220 RID: 544
public class Pretitle : MonoBehaviour
{
	// Token: 0x06000DE6 RID: 3558 RVA: 0x00046346 File Offset: 0x00044546
	private void Start()
	{
		base.StartCoroutine(this.PreloadScene());
		base.StartCoroutine(this.LoadTitle());
	}

	// Token: 0x06000DE7 RID: 3559 RVA: 0x00046362 File Offset: 0x00044562
	private IEnumerator PreloadScene()
	{
		AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("Title", LoadSceneMode.Single);
		loadSceneAsync.allowSceneActivation = false;
		while (!this.allowedToSwitch)
		{
			yield return null;
		}
		loadSceneAsync.allowSceneActivation = true;
		yield break;
	}

	// Token: 0x06000DE8 RID: 3560 RVA: 0x00046371 File Offset: 0x00044571
	private IEnumerator LoadTitle()
	{
		yield return new WaitForSecondsRealtime(this.loadWait);
		this.allowedToSwitch = true;
		yield break;
	}

	// Token: 0x06000DE9 RID: 3561 RVA: 0x00046380 File Offset: 0x00044580
	private void Update()
	{
		bool flag = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape);
		if (!flag)
		{
			InputActionReference[] array = this.skipKeys;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].action.WasPressedThisFrame())
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			this.allowedToSwitch = true;
		}
	}

	// Token: 0x04000D03 RID: 3331
	public InputActionReference[] skipKeys;

	// Token: 0x04000D04 RID: 3332
	public float loadWait = 11f;

	// Token: 0x04000D05 RID: 3333
	private bool allowedToSwitch;
}
