using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class DialogueEffect : MonoBehaviour
{
	// Token: 0x06000997 RID: 2455 RVA: 0x00030295 File Offset: 0x0002E495
	private void Awake()
	{
		this.m_TextComponent = base.GetComponent<TMP_Text>();
		this.DTanimator = new DOTweenTMPAnimator(this.m_TextComponent);
	}

	// Token: 0x06000998 RID: 2456 RVA: 0x000302B4 File Offset: 0x0002E4B4
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06000999 RID: 2457 RVA: 0x000302BC File Offset: 0x0002E4BC
	private void OnEnable()
	{
	}

	// Token: 0x0600099A RID: 2458 RVA: 0x000302BE File Offset: 0x0002E4BE
	private void OnDisable()
	{
		this.TryDestroy();
	}

	// Token: 0x0600099B RID: 2459 RVA: 0x000302C6 File Offset: 0x0002E4C6
	public virtual void Init()
	{
	}

	// Token: 0x0600099C RID: 2460 RVA: 0x000302C8 File Offset: 0x0002E4C8
	private void TryDestroy()
	{
		this.destroyed = true;
		Object.Destroy(this);
	}

	// Token: 0x0600099D RID: 2461 RVA: 0x000302D7 File Offset: 0x0002E4D7
	private void LateUpdate()
	{
		if (!this.destroyed)
		{
			this.EffectRoutine();
		}
	}

	// Token: 0x0600099E RID: 2462 RVA: 0x000302E8 File Offset: 0x0002E4E8
	protected virtual void EffectRoutine()
	{
		this.textInfo = this.m_TextComponent.textInfo;
		int characterCount = this.textInfo.characterCount;
		if (characterCount == 0)
		{
			return;
		}
		for (int i = 0; i < characterCount; i++)
		{
			this.UpdateCharacter(i);
		}
	}

	// Token: 0x0600099F RID: 2463 RVA: 0x00030329 File Offset: 0x0002E529
	public virtual void UpdateCharacter(int index)
	{
	}

	// Token: 0x0400087A RID: 2170
	protected TMP_Text m_TextComponent;

	// Token: 0x0400087B RID: 2171
	protected TMP_TextInfo textInfo;

	// Token: 0x0400087C RID: 2172
	public DOTweenTMPAnimator DTanimator;

	// Token: 0x0400087D RID: 2173
	private bool destroyed;
}
