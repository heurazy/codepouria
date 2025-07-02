using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001F9 RID: 505
public class DemoScript : MonoBehaviour
{
	// Token: 0x06000D16 RID: 3350 RVA: 0x00041984 File Offset: 0x0003FB84
	private void Start()
	{
		this.originalRotation = base.transform.localRotation;
		Renderer component = this.LightBulb.GetComponent<Renderer>();
		if (Application.isPlaying)
		{
			component.sharedMaterial = component.material;
		}
		this.lightBulbMaterial = component.sharedMaterial;
	}

	// Token: 0x06000D17 RID: 3351 RVA: 0x000419CD File Offset: 0x0003FBCD
	private void Update()
	{
		this.RotateMirror();
		this.MoveLightBulb();
		this.UpdateMouseLook();
		this.UpdateMovement();
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x000419E7 File Offset: 0x0003FBE7
	public void MirrorRecursionToggled()
	{
		this.ChangeMirrorRecursion();
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x000419F0 File Offset: 0x0003FBF0
	public void ChangeMirrorRecursion()
	{
		foreach (GameObject gameObject in this.Mirrors)
		{
			gameObject.GetComponent<MirrorScript>().MirrorRecursion = this.RecursionToggle.isOn;
		}
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x00041A50 File Offset: 0x0003FC50
	private void UpdateMovement()
	{
		float num = 4f * Time.deltaTime;
		if (Input.GetKey(KeyCode.W))
		{
			base.transform.Translate(0f, 0f, num);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			base.transform.Translate(0f, 0f, -num);
		}
		if (Input.GetKey(KeyCode.A))
		{
			base.transform.Translate(-num, 0f, 0f);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			base.transform.Translate(num, 0f, 0f);
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			this.RecursionToggle.isOn = !this.RecursionToggle.isOn;
		}
	}

	// Token: 0x06000D1B RID: 3355 RVA: 0x00041B10 File Offset: 0x0003FD10
	private void RotateMirror()
	{
		GameObject gameObject = this.Mirrors[0];
		float num = gameObject.transform.rotation.eulerAngles.y;
		if (num > 65f && num < 100f)
		{
			this.rotationModifier = -this.rotationModifier;
			num -= 65f;
			gameObject.transform.Rotate(0f, -num, 0f);
			return;
		}
		if (num > 100f && num < 295f)
		{
			this.rotationModifier = -this.rotationModifier;
			num = 295f - num;
			gameObject.transform.Rotate(0f, num, 0f);
			return;
		}
		gameObject.transform.Rotate(0f, this.rotationModifier * Time.deltaTime * 20f, 0f);
	}

	// Token: 0x06000D1C RID: 3356 RVA: 0x00041BE4 File Offset: 0x0003FDE4
	private void MoveLightBulb()
	{
		float num = this.LightBulb.transform.position.x;
		if (num > 5f)
		{
			this.moveModifier = -this.moveModifier;
			num = 5f;
		}
		else if (num < -5f)
		{
			this.moveModifier = -this.moveModifier;
			num = -5f;
		}
		else
		{
			num += Time.deltaTime * this.moveModifier;
		}
		Light component = this.LightBulb.GetComponent<Light>();
		this.LightBulb.transform.position = new Vector3(num, this.LightBulb.transform.position.y, this.LightBulb.transform.position.z);
		float num2 = Mathf.Min(1f, component.intensity);
		this.lightBulbMaterial.SetColor("_EmissionColor", new Color(num2, num2, num2));
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x00041CC8 File Offset: 0x0003FEC8
	private void UpdateMouseLook()
	{
		if (this.axes == DemoScript.RotationAxes.MouseXAndY)
		{
			this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
			this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
			this.rotationX = DemoScript.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
			this.rotationY = DemoScript.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
			Quaternion quaternion = Quaternion.AngleAxis(this.rotationX, Vector3.up);
			Quaternion quaternion2 = Quaternion.AngleAxis(this.rotationY, -Vector3.right);
			base.transform.localRotation = this.originalRotation * quaternion * quaternion2;
			return;
		}
		if (this.axes == DemoScript.RotationAxes.MouseX)
		{
			this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
			this.rotationX = DemoScript.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
			Quaternion quaternion3 = Quaternion.AngleAxis(this.rotationX, Vector3.up);
			base.transform.localRotation = this.originalRotation * quaternion3;
			return;
		}
		this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
		this.rotationY = DemoScript.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
		Quaternion quaternion4 = Quaternion.AngleAxis(-this.rotationY, Vector3.right);
		base.transform.localRotation = this.originalRotation * quaternion4;
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x00041E6C File Offset: 0x0004006C
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	// Token: 0x04000C30 RID: 3120
	public List<GameObject> Mirrors;

	// Token: 0x04000C31 RID: 3121
	public GameObject LightBulb;

	// Token: 0x04000C32 RID: 3122
	public Toggle RecursionToggle;

	// Token: 0x04000C33 RID: 3123
	private float rotationModifier = -1f;

	// Token: 0x04000C34 RID: 3124
	private float moveModifier = 1f;

	// Token: 0x04000C35 RID: 3125
	private Material lightBulbMaterial;

	// Token: 0x04000C36 RID: 3126
	private DemoScript.RotationAxes axes;

	// Token: 0x04000C37 RID: 3127
	private float sensitivityX = 15f;

	// Token: 0x04000C38 RID: 3128
	private float sensitivityY = 15f;

	// Token: 0x04000C39 RID: 3129
	private float minimumX = -360f;

	// Token: 0x04000C3A RID: 3130
	private float maximumX = 360f;

	// Token: 0x04000C3B RID: 3131
	private float minimumY = -60f;

	// Token: 0x04000C3C RID: 3132
	private float maximumY = 60f;

	// Token: 0x04000C3D RID: 3133
	private float rotationX;

	// Token: 0x04000C3E RID: 3134
	private float rotationY;

	// Token: 0x04000C3F RID: 3135
	private Quaternion originalRotation;

	// Token: 0x02000393 RID: 915
	private enum RotationAxes
	{
		// Token: 0x04001334 RID: 4916
		MouseXAndY,
		// Token: 0x04001335 RID: 4917
		MouseX,
		// Token: 0x04001336 RID: 4918
		MouseY
	}
}
