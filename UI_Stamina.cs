using System;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

// Token: 0x02000294 RID: 660
public class UI_Stamina : MonoBehaviour
{
	// Token: 0x06000FBF RID: 4031 RVA: 0x0004FCAF File Offset: 0x0004DEAF
	private void Update()
	{
		this.fill.fillAmount = Character.localCharacter.data.currentStamina;
	}

	// Token: 0x04000ED0 RID: 3792
	public ProceduralImage fill;
}
