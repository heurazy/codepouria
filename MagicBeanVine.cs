using System;
using UnityEngine;

// Token: 0x020000E0 RID: 224
public class MagicBeanVine : MonoBehaviour
{
	// Token: 0x060006DC RID: 1756 RVA: 0x00023EBC File Offset: 0x000220BC
	private void Awake()
	{
		this.currentLength = this.initialLength;
		float num = this.currentLength / this.maxLength;
		float num2 = this.xzScaleCurve.Evaluate(num) * this.maxWidth;
		this.vineOriginTransform.transform.localScale = new Vector3(num2, this.currentLength, num2);
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x00023F14 File Offset: 0x00022114
	private void FixedUpdate()
	{
		if (this.currentLength < this.maxLength)
		{
			this.currentLength = Mathf.MoveTowards(this.currentLength, this.maxLength, this.growingSpeed * Time.fixedDeltaTime);
			float num = this.currentLength / this.maxLength;
			float num2 = this.xzScaleCurve.Evaluate(num) * this.maxWidth;
			this.vineOriginTransform.transform.localScale = new Vector3(num2, this.currentLength, num2);
			this.vineOriginTransform.transform.Rotate(0f, this.rotationSpeed * this.rotationSpeedCurve.Evaluate(num), 0f);
		}
	}

	// Token: 0x0400066D RID: 1645
	public Transform vineOriginTransform;

	// Token: 0x0400066E RID: 1646
	public float maxWidth = 1.5f;

	// Token: 0x0400066F RID: 1647
	public float maxLength = 20f;

	// Token: 0x04000670 RID: 1648
	public float initialLength = 0.5f;

	// Token: 0x04000671 RID: 1649
	private float currentLength = 0.01f;

	// Token: 0x04000672 RID: 1650
	public float growingSpeed = 1f;

	// Token: 0x04000673 RID: 1651
	public float rotationSpeed = 10f;

	// Token: 0x04000674 RID: 1652
	public AnimationCurve xzScaleCurve;

	// Token: 0x04000675 RID: 1653
	public AnimationCurve rotationSpeedCurve;
}
