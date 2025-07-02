using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001F0 RID: 496
[ExecuteAlways]
public class LoadingScreenAnimation : MonoBehaviour
{
	// Token: 0x06000D01 RID: 3329 RVA: 0x00041204 File Offset: 0x0003F404
	private void Update()
	{
		this.barFill.fillAmount = Mathf.Lerp(this.barFillMinMax.x, this.barFillMinMax.y, this.fillAmount);
		this.planeRotation.localEulerAngles = new Vector3(0f, 0f, Mathf.Lerp(this.planeRotationMinMax.x, this.planeRotationMinMax.y, this.fillAmount));
		this.loadingText.text = this.loadingString.Substring(0, Mathf.RoundToInt((float)this.loadingString.Length * this.fillAmount));
	}

	// Token: 0x04000BFA RID: 3066
	public Image barFill;

	// Token: 0x04000BFB RID: 3067
	public Transform planeRotation;

	// Token: 0x04000BFC RID: 3068
	public TMP_Text loadingText;

	// Token: 0x04000BFD RID: 3069
	[Range(0f, 1f)]
	public float fillAmount;

	// Token: 0x04000BFE RID: 3070
	public Vector2 barFillMinMax;

	// Token: 0x04000BFF RID: 3071
	public Vector2 planeRotationMinMax;

	// Token: 0x04000C00 RID: 3072
	public string loadingString;

	// Token: 0x04000C01 RID: 3073
	public float maxFill;
}
