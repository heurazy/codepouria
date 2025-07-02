using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zorro.Settings;

// Token: 0x02000172 RID: 370
public class SharedSettingsMenu : MonoBehaviour
{
	// Token: 0x06000A69 RID: 2665 RVA: 0x00032AA6 File Offset: 0x00030CA6
	private void OnEnable()
	{
		this.RefreshSettings();
		if (this.m_tabs.selectedButton != null)
		{
			this.m_tabs.Select(this.m_tabs.selectedButton);
		}
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x00032AD7 File Offset: 0x00030CD7
	private void RefreshSettings()
	{
		if (GameHandler.Instance != null)
		{
			this.settings = GameHandler.Instance.SettingsHandler.GetSettingsThatImplements<IExposedSetting>();
		}
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x00032AFC File Offset: 0x00030CFC
	public void ShowSettings(SettingsCategory category)
	{
		if (this.m_fadeInCoroutine != null)
		{
			base.StopCoroutine(this.m_fadeInCoroutine);
			this.m_fadeInCoroutine = null;
		}
		foreach (SettingsUICell settingsUICell in this.m_spawnedCells)
		{
			Object.Destroy(settingsUICell.gameObject);
		}
		this.m_spawnedCells.Clear();
		this.RefreshSettings();
		foreach (IExposedSetting exposedSetting in this.settings.Where((IExposedSetting setting) => setting.GetCategory() == category.ToString()).Where(delegate(IExposedSetting setting)
		{
			IConditionalSetting conditionalSetting = setting as IConditionalSetting;
			return conditionalSetting == null || conditionalSetting.ShouldShow();
		}))
		{
			SettingsUICell component = Object.Instantiate<GameObject>(this.m_settingsCellPrefab, this.m_settingsContentParent).GetComponent<SettingsUICell>();
			this.m_spawnedCells.Add(component);
			component.Setup<Setting>(exposedSetting as Setting);
		}
		this.m_fadeInCoroutine = base.StartCoroutine(this.FadeInCells());
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x00032C38 File Offset: 0x00030E38
	private IEnumerator FadeInCells()
	{
		int i = 0;
		foreach (SettingsUICell settingsUICell in this.m_spawnedCells)
		{
			settingsUICell.FadeIn();
			yield return new WaitForSecondsRealtime(0.05f);
			int num = i;
			i = num + 1;
		}
		List<SettingsUICell>.Enumerator enumerator = default(List<SettingsUICell>.Enumerator);
		this.m_fadeInCoroutine = null;
		yield break;
		yield break;
	}

	// Token: 0x0400092F RID: 2351
	[SerializeField]
	private SettingsTABS m_tabs;

	// Token: 0x04000930 RID: 2352
	public GameObject m_settingsCellPrefab;

	// Token: 0x04000931 RID: 2353
	public Transform m_settingsContentParent;

	// Token: 0x04000932 RID: 2354
	private List<IExposedSetting> settings;

	// Token: 0x04000933 RID: 2355
	private readonly List<SettingsUICell> m_spawnedCells = new List<SettingsUICell>();

	// Token: 0x04000934 RID: 2356
	private Coroutine m_fadeInCoroutine;
}
