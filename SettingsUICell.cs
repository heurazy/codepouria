using System;
using TMPro;
using UnityEngine;
using Zorro.Settings;

// Token: 0x02000171 RID: 369
public class SettingsUICell : MonoBehaviour
{
	// Token: 0x06000A65 RID: 2661 RVA: 0x000329B8 File Offset: 0x00030BB8
	public void Setup<T>(T setting) where T : Setting
	{
		this.m_canvasGroup = base.GetComponent<CanvasGroup>();
		this.m_canvasGroup.alpha = 0f;
		IExposedSetting exposedSetting = setting as IExposedSetting;
		if (exposedSetting != null)
		{
			string displayName = exposedSetting.GetDisplayName();
			this.m_text.text = displayName;
		}
		Object.Instantiate<GameObject>(setting.GetSettingUICell(), this.m_settingsContentParent).GetComponent<SettingInputUICell>().Setup(setting, GameHandler.Instance.SettingsHandler);
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x00032A34 File Offset: 0x00030C34
	public void FadeIn()
	{
		this.m_fadeIn = true;
		if (this.fadeInSFX)
		{
			this.fadeInSFX.Play(default(Vector3));
		}
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x00032A69 File Offset: 0x00030C69
	private void Update()
	{
		if (this.m_fadeIn)
		{
			this.m_canvasGroup.alpha = Mathf.Lerp(this.m_canvasGroup.alpha, 1f, Time.unscaledDeltaTime * 10f);
		}
	}

	// Token: 0x0400092A RID: 2346
	public Transform m_settingsContentParent;

	// Token: 0x0400092B RID: 2347
	public TextMeshProUGUI m_text;

	// Token: 0x0400092C RID: 2348
	private bool m_fadeIn;

	// Token: 0x0400092D RID: 2349
	private CanvasGroup m_canvasGroup;

	// Token: 0x0400092E RID: 2350
	public SFX_Instance fadeInSFX;
}
