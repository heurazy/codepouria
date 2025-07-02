using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000108 RID: 264
public class PlayerEyeLook : MonoBehaviour
{
	// Token: 0x060007CD RID: 1997 RVA: 0x0002933B File Offset: 0x0002753B
	private void Start()
	{
		this.localCharacter = base.GetComponent<Character>();
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0002934C File Offset: 0x0002754C
	private void Update()
	{
		this.characters = Character.AllCharacters;
		this.distance = float.PositiveInfinity;
		for (int i = 0; i < this.characters.Count; i++)
		{
			float num = Vector3.Distance(this.characters[i].Center, this.localCharacter.Center);
			if (num < this.distance && this.characters[i] != this.localCharacter)
			{
				this.distance = num;
				this.character = this.characters[i];
			}
			AnimatedMouth component = this.characters[i].GetComponent<AnimatedMouth>();
			if (num < this.listenRange && component.isSpeaking && this.characters[i] != this.localCharacter)
			{
				this.distance = num;
				this.character = this.characters[i];
			}
		}
		if (this.character != null)
		{
			this.lookDir = (this.character.Head - this.localCharacter.Head).normalized;
			this.lookDelta = this.localCharacter.GetBodypart(BodypartType.Head).transform.forward - this.lookDir;
			base.transform.InverseTransformDirection(this.lookDelta);
			this.UpDelta = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.up, this.lookDelta);
			this.RightDelta = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.right, this.lookDelta);
			this.lookAngle = Vector3.Angle(this.localCharacter.data.lookDirection, this.lookDir);
		}
		if (this.character != null && this.distance < this.lookRange && this.lookAngle < this.lookAngleMax)
		{
			this.eyeTarget = new Vector2(this.RightDelta * -this.XMax, this.UpDelta * this.YMax);
			this.lookingAtCharacter = true;
		}
		else
		{
			this.lookingAtCharacter = false;
			Vector3 forward = this.localCharacter.GetBodypart(BodypartType.Hip).transform.forward;
			forward.y = 0f;
			Vector3 vector = this.localCharacter.data.lookDirection - forward;
			float num2 = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.right, vector);
			float num3 = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.up, vector);
			this.eyeTarget = new Vector2(num2 * this.XMax, num3 * -this.YMax);
		}
		float num4 = 1f;
		if (this.character != this.lastCharacter)
		{
			num4 = 0.3f;
		}
		this.eyePos = Vector2.Lerp(this.eyePos, this.eyeTarget, Time.deltaTime * this.lookSmoothing * num4);
		for (int j = 0; j < this.eyeRenderers.Length; j++)
		{
			this.eyeRenderers[j].material.SetVector("_EyePosition", this.eyePos);
		}
		if (Vector3.Distance(this.lastViewDir, this.localCharacter.GetBodypart(BodypartType.Head).transform.forward) > this.xLookThreshold)
		{
			this.lastViewDir = this.localCharacter.GetBodypart(BodypartType.Head).transform.forward;
		}
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x000296E0 File Offset: 0x000278E0
	private void OnDrawGizmosSelected()
	{
		if (this.lookingAtCharacter)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(this.localCharacter.Head, this.lookDir * this.lookRange);
		}
		else
		{
			Gizmos.color = Color.yellow;
			Vector3 forward = this.localCharacter.GetBodypart(BodypartType.Head).transform.forward;
			forward.y = 0f;
			Gizmos.DrawRay(this.localCharacter.Head, forward * this.lookRange);
		}
		Gizmos.color = Color.red;
		Gizmos.DrawRay(this.localCharacter.Head, this.localCharacter.GetBodypart(BodypartType.Head).transform.forward * this.lookRange);
	}

	// Token: 0x04000740 RID: 1856
	public bool lookingAtCharacter;

	// Token: 0x04000741 RID: 1857
	private List<Character> characters = new List<Character>();

	// Token: 0x04000742 RID: 1858
	public float distance;

	// Token: 0x04000743 RID: 1859
	public float lookRange;

	// Token: 0x04000744 RID: 1860
	public float listenRange;

	// Token: 0x04000745 RID: 1861
	private Character lastCharacter;

	// Token: 0x04000746 RID: 1862
	public float lookSmoothing;

	// Token: 0x04000747 RID: 1863
	public Character character;

	// Token: 0x04000748 RID: 1864
	public Renderer[] eyeRenderers;

	// Token: 0x04000749 RID: 1865
	private Vector3 lookDir;

	// Token: 0x0400074A RID: 1866
	public float lookAngleMax;

	// Token: 0x0400074B RID: 1867
	private Vector3 lookDelta;

	// Token: 0x0400074C RID: 1868
	private float RightDelta;

	// Token: 0x0400074D RID: 1869
	private float UpDelta;

	// Token: 0x0400074E RID: 1870
	public float lookAngle;

	// Token: 0x0400074F RID: 1871
	public float xLookThreshold;

	// Token: 0x04000750 RID: 1872
	[FormerlySerializedAs("leftRightMax")]
	public float XMax;

	// Token: 0x04000751 RID: 1873
	[FormerlySerializedAs("upDownMax")]
	public float YMax;

	// Token: 0x04000752 RID: 1874
	private Character localCharacter;

	// Token: 0x04000753 RID: 1875
	private Vector2 eyePos = Vector2.zero;

	// Token: 0x04000754 RID: 1876
	private Vector2 eyeTarget = Vector2.zero;

	// Token: 0x04000755 RID: 1877
	private Vector3 lastViewDir;
}
