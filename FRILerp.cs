using System;
using UnityEngine;

// Token: 0x020001D3 RID: 467
public class FRILerp : MonoBehaviour
{
	// Token: 0x06000C6C RID: 3180 RVA: 0x0003DFF9 File Offset: 0x0003C1F9
	private void Start()
	{
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x0003DFFB File Offset: 0x0003C1FB
	public static Vector3 Lerp(Vector3 from, Vector3 target, float speed, bool useTimeScale = true)
	{
		return Vector3.Lerp(from, target, 1f - Mathf.Exp(-speed * (useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime)));
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x0003E021 File Offset: 0x0003C221
	public static Vector3 PLerp(Vector3 from, Vector3 target, float speed, float dt)
	{
		return Vector3.Lerp(from, target, 1f - Mathf.Exp(-speed * dt));
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x0003E039 File Offset: 0x0003C239
	public static Quaternion PLerp(Quaternion from, Quaternion target, float speed, float dt)
	{
		return Quaternion.Lerp(from, target, 1f - Mathf.Exp(-speed * dt));
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x0003E051 File Offset: 0x0003C251
	public static float PLerp(float from, float target, float speed, float dt)
	{
		return Mathf.Lerp(from, target, 1f - Mathf.Exp(-speed * dt));
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x0003E069 File Offset: 0x0003C269
	public static Vector3 LerpFixed(Vector3 from, Vector3 target, float speed, bool useTimeScale = true)
	{
		return Vector3.Lerp(from, target, 1f - Mathf.Exp(-speed * (useTimeScale ? Time.fixedDeltaTime : Time.unscaledDeltaTime)));
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x0003E08F File Offset: 0x0003C28F
	public static Vector3 LerpUnclamped(Vector3 from, Vector3 target, float speed)
	{
		return Vector3.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x0003E0AB File Offset: 0x0003C2AB
	public static float Lerp(float from, float target, float speed, bool useTimeScale = true)
	{
		return Mathf.Lerp(from, target, 1f - Mathf.Exp(-speed * (useTimeScale ? Time.fixedDeltaTime : Time.unscaledDeltaTime)));
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x0003E0D1 File Offset: 0x0003C2D1
	public static float LerpUnclamped(float from, float target, float speed)
	{
		return Mathf.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x0003E0ED File Offset: 0x0003C2ED
	public static Vector3 Slerp(Vector3 from, Vector3 target, float speed)
	{
		return Vector3.Slerp(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x0003E109 File Offset: 0x0003C309
	public static Vector3 SlerpUnclamped(Vector3 from, Vector3 target, float speed)
	{
		return Vector3.SlerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x0003E125 File Offset: 0x0003C325
	public static Quaternion Lerp(Quaternion from, Quaternion target, float speed)
	{
		return Quaternion.Lerp(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x0003E141 File Offset: 0x0003C341
	public static Quaternion LerpUnclamped(Quaternion from, Quaternion target, float speed)
	{
		return Quaternion.LerpUnclamped(from, target, 1f - Mathf.Exp(-speed * Time.deltaTime));
	}
}
