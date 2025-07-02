using System;
using UnityEngine;

// Token: 0x02000047 RID: 71
public class BingBongsVisuals : MonoBehaviour
{
	// Token: 0x06000346 RID: 838 RVA: 0x0001427C File Offset: 0x0001247C
	private void Start()
	{
		this.eyeRightStartRot = this.eyeRight.localRotation;
		this.eyeLeftStartRot = this.eyeLeft.localRotation;
	}

	// Token: 0x06000347 RID: 839 RVA: 0x000142A0 File Offset: 0x000124A0
	private void Update()
	{
		this.animator.SetFloat(BingBongsVisuals.MouthOpen, this.mouthOpen);
		if (!this.useEyeLook)
		{
			this.eyeLeft.localRotation = Quaternion.Slerp(this.eyeLeft.localRotation, this.eyeLeftStartRot, Time.deltaTime * this.eyeLerpSpeed);
			this.eyeRight.localRotation = Quaternion.Slerp(this.eyeRight.localRotation, this.eyeRightStartRot, Time.deltaTime * this.eyeLerpSpeed);
			return;
		}
		if (Character.observedCharacter == null)
		{
			return;
		}
		Vector3 vector = base.transform.position - Character.observedCharacter.transform.position;
		Vector3.Dot(vector, base.transform.forward);
		Quaternion quaternion = Quaternion.LookRotation(vector, Vector3.up);
		this.eyeRightTarget = quaternion;
		this.eyeLeftTarget = quaternion;
		this.eyeLeft.rotation = Quaternion.Slerp(this.eyeLeft.rotation, this.eyeLeftTarget, Time.deltaTime * this.eyeLerpSpeed);
		this.eyeRight.rotation = Quaternion.Slerp(this.eyeRight.rotation, this.eyeRightTarget, Time.deltaTime * this.eyeLerpSpeed);
	}

	// Token: 0x040003CE RID: 974
	private static readonly int MouthOpen = Animator.StringToHash("Mouth Blend");

	// Token: 0x040003CF RID: 975
	public Animator animator;

	// Token: 0x040003D0 RID: 976
	[Range(0f, 1f)]
	public float mouthOpen;

	// Token: 0x040003D1 RID: 977
	public Transform eyeRight;

	// Token: 0x040003D2 RID: 978
	public Transform eyeLeft;

	// Token: 0x040003D3 RID: 979
	public float eyeLookMaxAngle;

	// Token: 0x040003D4 RID: 980
	public float eyeLerpSpeed = 1f;

	// Token: 0x040003D5 RID: 981
	private Quaternion eyeRightStartRot;

	// Token: 0x040003D6 RID: 982
	private Quaternion eyeLeftStartRot;

	// Token: 0x040003D7 RID: 983
	private Quaternion eyeLeftTarget;

	// Token: 0x040003D8 RID: 984
	private Quaternion eyeRightTarget;

	// Token: 0x040003D9 RID: 985
	public bool useEyeLook;
}
