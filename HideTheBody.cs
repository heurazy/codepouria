using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001D7 RID: 471
public class HideTheBody : MonoBehaviour
{
	// Token: 0x06000C86 RID: 3206 RVA: 0x0003E320 File Offset: 0x0003C520
	private void Start()
	{
		this.character = base.GetComponentInParent<Character>();
		this.Toggle(true);
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x0003E338 File Offset: 0x0003C538
	private void Update()
	{
		bool flag = !this.character.IsLocal || this.character.data.fullyPassedOut || this.character.data.dead || this.isDummy;
		if (!this.character.IsLocal && this.character.data.carrier != null && this.character.data.carrier.IsLocal)
		{
			flag = false;
		}
		if (flag != this.isShowing)
		{
			this.Toggle(flag);
		}
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x0003E3CE File Offset: 0x0003C5CE
	public void Refresh()
	{
		this.Toggle(this.isShowing);
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x0003E3DC File Offset: 0x0003C5DC
	private void Toggle(bool show)
	{
		this.isShowing = show;
		this.shadowCaster.SetActive(!show);
		this.shadowCasterHat.SetActive(!show);
		if (show)
		{
			this.SetShowing(this.body, 0f);
			this.SetShowing(this.headRend, 0f);
			this.SetShowing(this.sash, 0f);
			for (int i = 0; i < this.costumes.Length; i++)
			{
				this.SetShowing(this.costumes[i], 0f);
			}
			Renderer[] componentsInChildren = this.face.GetComponentsInChildren<Renderer>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				this.SetShowing(componentsInChildren[j], 0f);
			}
			for (int k = 0; k < this.refs.playerHats.Length; k++)
			{
				this.SetShowing(this.refs.playerHats[k], 0f);
			}
			return;
		}
		this.SetShowing(this.body, 1f);
		this.SetShowing(this.headRend, 1f);
		this.SetShowing(this.sash, 1f);
		for (int l = 0; l < this.costumes.Length; l++)
		{
			this.SetShowing(this.costumes[l], 1f);
		}
		Renderer[] componentsInChildren2 = this.face.GetComponentsInChildren<Renderer>();
		for (int m = 0; m < componentsInChildren2.Length; m++)
		{
			this.SetShowing(componentsInChildren2[m], 1f);
		}
		for (int n = 0; n < this.refs.playerHats.Length; n++)
		{
			this.SetShowing(this.refs.playerHats[n], 1f);
		}
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x0003E58C File Offset: 0x0003C78C
	public void SetShowing(Renderer r, float x)
	{
		Material[] materials = r.materials;
		Material[] array = materials;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat(this.VERTEXGHOST, x);
		}
		r.materials = materials;
	}

	// Token: 0x04000B7C RID: 2940
	public bool isDummy;

	// Token: 0x04000B7D RID: 2941
	public SkinnedMeshRenderer body;

	// Token: 0x04000B7E RID: 2942
	public Renderer headRend;

	// Token: 0x04000B7F RID: 2943
	public CustomizationRefs refs;

	// Token: 0x04000B80 RID: 2944
	public Transform face;

	// Token: 0x04000B81 RID: 2945
	public GameObject shadowCaster;

	// Token: 0x04000B82 RID: 2946
	public GameObject shadowCasterHat;

	// Token: 0x04000B83 RID: 2947
	public SkinnedMeshRenderer[] costumes;

	// Token: 0x04000B84 RID: 2948
	[FormerlySerializedAs("Sash")]
	public SkinnedMeshRenderer sash;

	// Token: 0x04000B85 RID: 2949
	private bool isShowing = true;

	// Token: 0x04000B86 RID: 2950
	private Character character;

	// Token: 0x04000B87 RID: 2951
	private int VERTEXGHOST = Shader.PropertyToID("_VertexGhost");
}
