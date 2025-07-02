using System;
using UnityEngine;

// Token: 0x020001D4 RID: 468
public class GamefeelHandler : MonoBehaviour
{
	// Token: 0x06000C7A RID: 3194 RVA: 0x0003E165 File Offset: 0x0003C365
	private void Awake()
	{
		GamefeelHandler.instance = this;
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x0003E170 File Offset: 0x0003C370
	public Vector3 GetRotation()
	{
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			vector += base.transform.GetChild(i).localEulerAngles;
		}
		return vector;
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x0003E1B2 File Offset: 0x0003C3B2
	public void AddRotationShake_Local_Stiff(Vector3 force)
	{
		this.stiff.AddForce(force);
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x0003E1C0 File Offset: 0x0003C3C0
	public void AddRotationShake_Local_Loose(Vector3 force)
	{
		this.loose.AddForce(force);
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x0003E1CE File Offset: 0x0003C3CE
	public void AddPerlinShake(float amount = 1f, float duration = 0.2f, float scale = 15f)
	{
		this.perlin.AddShake(amount, duration, scale);
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x0003E1E0 File Offset: 0x0003C3E0
	public void AddPerlinShakeProximity(Vector3 position, float amount = 1f, float duration = 0.2f, float scale = 15f, float maxProximity = 10f)
	{
		float num = 1f;
		if (Character.observedCharacter)
		{
			num = 1f - Mathf.Clamp01(Vector3.Distance(Character.observedCharacter.Center, position) / maxProximity);
		}
		this.perlin.AddShake(amount * num, duration, scale);
	}

	// Token: 0x04000B77 RID: 2935
	public static GamefeelHandler instance;

	// Token: 0x04000B78 RID: 2936
	public RotationSpring stiff;

	// Token: 0x04000B79 RID: 2937
	public RotationSpring loose;

	// Token: 0x04000B7A RID: 2938
	public PerlinShake perlin;
}
