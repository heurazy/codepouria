using System;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200021D RID: 541
public class PointPing : MonoBehaviour
{
	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x06000DD8 RID: 3544 RVA: 0x00045EA4 File Offset: 0x000440A4
	public Vector3 PingerForward
	{
		get
		{
			return (base.transform.position - this.pointPinger.character.Head).normalized;
		}
	}

	// Token: 0x06000DD9 RID: 3545 RVA: 0x00045ED9 File Offset: 0x000440D9
	private void Awake()
	{
		this.material = this.renderer.material;
	}

	// Token: 0x06000DDA RID: 3546 RVA: 0x00045EEC File Offset: 0x000440EC
	private void Start()
	{
		this.camera = Camera.main;
		this.Go();
		this.pingSound.Play(base.transform.position);
	}

	// Token: 0x06000DDB RID: 3547 RVA: 0x00045F15 File Offset: 0x00044115
	public void Update()
	{
		this.Go();
	}

	// Token: 0x06000DDC RID: 3548 RVA: 0x00045F20 File Offset: 0x00044120
	private void Go()
	{
		float num = this.camera.SizeOfFrustumAtDistance(Vector3.Distance(Character.localCharacter.Center, base.transform.position));
		num = this.minMaxScale.PClampFloat(num);
		base.transform.localScale = (num * this.sizeOfFrustum).xxx();
		Vector3 vector = base.transform.position - this.camera.transform.position;
		float num2 = Vector3.Angle(this.PingerForward, vector);
		Vector3 vector2 = Vector3.Lerp(-this.hitNormal, this.PingerForward, num2.Remap(0f, this.angleThing, 0f, 1f));
		float num3 = Vector3.Angle(vector2, vector);
		Vector3 vector3 = Vector3.Lerp(-Vector3.up, vector2, num3.Remap(0f, this.angleThing, 0f, 1f));
		base.transform.rotation = Quaternion.LookRotation(vector3, Vector3.up);
	}

	// Token: 0x04000CEF RID: 3311
	public float sizeOfFrustum = 0.1f;

	// Token: 0x04000CF0 RID: 3312
	public Vector2 minMaxScale = new Vector2(0.2f, 3f);

	// Token: 0x04000CF1 RID: 3313
	public Vector2 visibilityFullNoneNoLos = new Vector2(30f, 50f);

	// Token: 0x04000CF2 RID: 3314
	public float NoLosVisibilityMul = 0.5f;

	// Token: 0x04000CF3 RID: 3315
	public float angleThing = 90f;

	// Token: 0x04000CF4 RID: 3316
	public MeshRenderer renderer;

	// Token: 0x04000CF5 RID: 3317
	public SpriteRenderer ringRenderer;

	// Token: 0x04000CF6 RID: 3318
	public SFX_Instance pingSound;

	// Token: 0x04000CF7 RID: 3319
	public Material material;

	// Token: 0x04000CF8 RID: 3320
	public Vector3 hitNormal;

	// Token: 0x04000CF9 RID: 3321
	public PointPinger pointPinger;

	// Token: 0x04000CFA RID: 3322
	private Camera camera;
}
