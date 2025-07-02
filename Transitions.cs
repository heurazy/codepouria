using System;
using UnityEngine;

// Token: 0x0200028A RID: 650
public class Transitions : MonoBehaviour
{
	// Token: 0x06000F9C RID: 3996 RVA: 0x0004F352 File Offset: 0x0004D552
	private void Awake()
	{
		Transitions.instance = this;
		this.transitions = base.GetComponentsInChildren<Transition>(true);
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x0004F368 File Offset: 0x0004D568
	public void PlayTransition(TransitionType transitionType, Action action, float transitionInSpeed = 1f, float transitionOutSpeed = 1f)
	{
		Transitions.<>c__DisplayClass3_0 CS$<>8__locals1 = new Transitions.<>c__DisplayClass3_0();
		CS$<>8__locals1.transitionInSpeed = transitionInSpeed;
		CS$<>8__locals1.action = action;
		CS$<>8__locals1.transitionOutSpeed = transitionOutSpeed;
		CS$<>8__locals1.transition = this.GetTransition(transitionType);
		base.StartCoroutine(CS$<>8__locals1.<PlayTransition>g__IPlayTransition|0());
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x0004F3AC File Offset: 0x0004D5AC
	private Transition GetTransition(TransitionType transitionType)
	{
		for (int i = 0; i < this.transitions.Length; i++)
		{
			if (this.transitions[i].transitionType == transitionType)
			{
				return this.transitions[i];
			}
		}
		return null;
	}

	// Token: 0x04000E9F RID: 3743
	private Transition[] transitions;

	// Token: 0x04000EA0 RID: 3744
	public static Transitions instance;
}
