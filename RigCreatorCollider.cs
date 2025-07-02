using System;
using UnityEngine;

// Token: 0x0200002B RID: 43
[ExecuteInEditMode]
public class RigCreatorCollider : MonoBehaviour
{
	// Token: 0x0600027B RID: 635 RVA: 0x000113C1 File Offset: 0x0000F5C1
	private void Start()
	{
		if (this.disableOnStart)
		{
			this.Col().enabled = false;
			return;
		}
		base.GetComponentInParent<CharacterRagdoll>().colliderList.Add(this.Col());
	}

	// Token: 0x0600027C RID: 636 RVA: 0x000113EE File Offset: 0x0000F5EE
	private void Awake()
	{
		if (this.IsEditor())
		{
			this.SetValues();
			return;
		}
		this.RegisterCollider();
		this.Col();
	}

	// Token: 0x0600027D RID: 637 RVA: 0x0001140C File Offset: 0x0000F60C
	private void RegisterCollider()
	{
		base.transform.parent.GetComponent<Bodypart>().RegisterCollider(this);
	}

	// Token: 0x0600027E RID: 638 RVA: 0x00011424 File Offset: 0x0000F624
	private bool IsEditor()
	{
		return Application.isEditor && !Application.isPlaying;
	}

	// Token: 0x0600027F RID: 639 RVA: 0x00011437 File Offset: 0x0000F637
	private void OnDestroy()
	{
		if (!this.IsEditor())
		{
			return;
		}
		if (!this.RigCreator())
		{
			return;
		}
		this.RigCreator().RemoveCollider(this);
	}

	// Token: 0x06000280 RID: 640 RVA: 0x0001145C File Offset: 0x0000F65C
	private CapsuleCollider Col()
	{
		if (!this.col)
		{
			this.col = base.GetComponent<CapsuleCollider>();
		}
		return this.col;
	}

	// Token: 0x06000281 RID: 641 RVA: 0x0001147D File Offset: 0x0000F67D
	private RigCreator RigCreator()
	{
		if (!this.rigCreator)
		{
			this.rigCreator = base.GetComponentInParent<RigCreator>();
		}
		return this.rigCreator;
	}

	// Token: 0x06000282 RID: 642 RVA: 0x0001149E File Offset: 0x0000F69E
	private void Update()
	{
		if (this.IsEditor())
		{
			this.CheckEditorDataChanged();
		}
	}

	// Token: 0x06000283 RID: 643 RVA: 0x000114B0 File Offset: 0x0000F6B0
	private void CheckEditorDataChanged()
	{
		if (this.position != base.transform.localPosition || this.rotation != base.transform.localRotation || this.scale != base.transform.localScale || this.height != this.Col().height || this.radius != this.Col().radius)
		{
			this.RigCreator().ColliderChanged(this, base.transform.localPosition, base.transform.localRotation, base.transform.localScale, this.height, this.radius);
			this.SetValues();
		}
	}

	// Token: 0x06000284 RID: 644 RVA: 0x0001156C File Offset: 0x0000F76C
	private void SetValues()
	{
		this.position = base.transform.localPosition;
		this.rotation = base.transform.localRotation;
		this.scale = base.transform.localScale;
		this.height = this.Col().height;
		this.radius = this.Col().radius;
	}

	// Token: 0x04000306 RID: 774
	internal CapsuleCollider col;

	// Token: 0x04000307 RID: 775
	internal Vector3 position;

	// Token: 0x04000308 RID: 776
	internal Quaternion rotation;

	// Token: 0x04000309 RID: 777
	internal Vector3 scale;

	// Token: 0x0400030A RID: 778
	internal float height;

	// Token: 0x0400030B RID: 779
	internal float radius;

	// Token: 0x0400030C RID: 780
	public bool disableOnStart;

	// Token: 0x0400030D RID: 781
	internal RigCreator rigCreator;
}
