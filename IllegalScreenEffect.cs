using System;
using UnityEngine;

// Token: 0x020001DB RID: 475
public class IllegalScreenEffect : MonoBehaviour
{
	// Token: 0x06000C91 RID: 3217 RVA: 0x0003E7F4 File Offset: 0x0003C9F4
	private void Start()
	{
		this.rend = base.GetComponent<MeshRenderer>();
		this.rend.enabled = false;
		this.mat = this.rend.material;
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x0003E820 File Offset: 0x0003CA20
	private void Update()
	{
		if (!this.character)
		{
			if (Character.localCharacter)
			{
				this.character = Character.localCharacter;
				Character character = this.character;
				character.illegalStatusAction = (Action<string, float>)Delegate.Combine(character.illegalStatusAction, new Action<string, float>(this.AddStatus));
			}
			return;
		}
		if (this.character.data.fullyPassedOut || this.character.data.dead)
		{
			this.activeForSeconds = 0f;
		}
		this.activeForSeconds -= Time.deltaTime;
		if (this.activeForSeconds > 0f)
		{
			this.rend.enabled = true;
			float num = Mathf.Clamp01(this.activeForSeconds / 3f);
			float @float = this.mat.GetFloat(this.shaderVarName);
			if (num > @float)
			{
				this.mat.SetFloat(this.shaderVarName, Mathf.Lerp(@float, num, Time.deltaTime));
			}
			else
			{
				this.mat.SetFloat(this.shaderVarName, num);
			}
			this.character.data.isBlind = true;
			return;
		}
		this.rend.enabled = false;
		this.character.data.isBlind = false;
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x0003E95B File Offset: 0x0003CB5B
	private void AddStatus(string status, float duration)
	{
		if (status.ToUpper() != this.statusName.ToUpper())
		{
			return;
		}
		this.activeForSeconds = duration;
	}

	// Token: 0x04000B8E RID: 2958
	public string statusName = "BLIND";

	// Token: 0x04000B8F RID: 2959
	public string shaderVarName = "_Alpha";

	// Token: 0x04000B90 RID: 2960
	private float activeForSeconds;

	// Token: 0x04000B91 RID: 2961
	private Character character;

	// Token: 0x04000B92 RID: 2962
	private MeshRenderer rend;

	// Token: 0x04000B93 RID: 2963
	private Material mat;
}
