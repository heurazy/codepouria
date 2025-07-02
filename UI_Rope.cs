using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000293 RID: 659
public class UI_Rope : MonoBehaviour
{
	// Token: 0x06000FBB RID: 4027 RVA: 0x0004FA05 File Offset: 0x0004DC05
	private void OnEnable()
	{
		this.segments = 1;
		this.ropeLength = 1f;
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x0004FA1C File Offset: 0x0004DC1C
	private void Update()
	{
		this.ropeLength = Mathf.Lerp(this.ropeLength, (float)this.segments, Time.deltaTime * 5f);
		float num = (Mathf.Max(this.ropeLength, 0f) + this.ropeLengthOffset) * this.ropeLengthMult;
		this.rope.sizeDelta = new Vector2(num, this.rope.sizeDelta.y);
		for (int i = 0; i < this.ropeImages.Length; i++)
		{
			this.ropeImages[i].color = new Color(this.ropeImages[i].color.r, this.ropeImages[i].color.g, this.ropeImages[i].color.b, num * this.ropeLengthAlphaMult - Mathf.Floor(num * this.ropeLengthAlphaMult) + 0.01f);
		}
		bool flag = false;
		for (int j = 0; j < 3; j++)
		{
			this.ropeImages[j].fillAmount = this.ropeSpinA - (this.ropeLength * this.ropeSpinB / this.maxRopeLength - (float)j);
			if (this.ropeImages[j].fillAmount > 0f && !flag)
			{
				flag = true;
				this.ropeEnd.position = this.ropeImages[j].transform.position;
				this.ropeEnd.eulerAngles = this.ropeImages[j].transform.eulerAngles + new Vector3(0f, 0f, this.ropeImages[j].fillAmount * 360f + this.ropeEndOffset);
				this.ropeEndImage.color = new Color(this.ropeImages[j].color.r, this.ropeImages[j].color.g, this.ropeImages[j].color.b, 1f);
			}
		}
		string text = "m";
		int num2 = Mathf.RoundToInt(this.ropeLength * 100f * 0.25f);
		this.ropeLengthText.text = (num2 / 100).ToString() + "." + (num2 % 100).ToString() + text;
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x0004FC67 File Offset: 0x0004DE67
	public void UpdateRope(int newSegments)
	{
		this.segments = newSegments;
	}

	// Token: 0x04000EC0 RID: 3776
	public RectTransform rope;

	// Token: 0x04000EC1 RID: 3777
	public float maxRopeLength = 40f;

	// Token: 0x04000EC2 RID: 3778
	public float ropeLength = 40f;

	// Token: 0x04000EC3 RID: 3779
	public float ropeLengthOffset;

	// Token: 0x04000EC4 RID: 3780
	public float ropeLengthMult = 20f;

	// Token: 0x04000EC5 RID: 3781
	public float ropeLengthAlphaMult;

	// Token: 0x04000EC6 RID: 3782
	public Image[] ropeImages;

	// Token: 0x04000EC7 RID: 3783
	private const string M = "m";

	// Token: 0x04000EC8 RID: 3784
	private const string FT = "ft";

	// Token: 0x04000EC9 RID: 3785
	public TextMeshProUGUI ropeLengthText;

	// Token: 0x04000ECA RID: 3786
	private int segments;

	// Token: 0x04000ECB RID: 3787
	public Transform ropeEnd;

	// Token: 0x04000ECC RID: 3788
	public Image ropeEndImage;

	// Token: 0x04000ECD RID: 3789
	public float ropeSpinA = 2f;

	// Token: 0x04000ECE RID: 3790
	public float ropeSpinB = 3f;

	// Token: 0x04000ECF RID: 3791
	public float ropeEndOffset;
}
