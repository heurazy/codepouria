using System;
using UnityEngine;
using WebSocketSharp;

// Token: 0x020001DE RID: 478
public class ItemAudioManager : MonoBehaviour
{
	// Token: 0x06000C9C RID: 3228 RVA: 0x0003EB97 File Offset: 0x0003CD97
	private void Start()
	{
		this.character = base.transform.root.GetComponent<Character>();
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x0003EBB0 File Offset: 0x0003CDB0
	private void Update()
	{
		this.character.refs.animator.SetBool("Eat", false);
		this.character.refs.animator.SetBool("Heal", false);
		this.character.refs.animator.SetBool("Drink", false);
		this.character.refs.animator.SetBool("Antidote", false);
		this.throwCharge.volume = Mathf.Lerp(this.throwCharge.volume, 0f, Time.deltaTime * 5f);
		this.throwCharge.pitch = Mathf.Lerp(this.throwCharge.pitch, 1f, Time.deltaTime * 5f);
		if (!string.IsNullOrEmpty(this.prevUse) && !this.prevUse.IsNullOrEmpty())
		{
			this.character.refs.animator.SetBool(this.prevUse, false);
		}
		if (!this.character.data.currentItem && !string.IsNullOrEmpty(this.prevUse))
		{
			this.character.refs.animator.SetBool(this.prevUse, false);
		}
		if (this.character.refs.animator.GetBool("Consumed Item"))
		{
			this.finishTimer -= Time.deltaTime;
		}
		else
		{
			this.finishTimer = 0.25f;
		}
		if (this.finishTimer <= 0f)
		{
			this.character.refs.animator.SetBool("Consumed Item", false);
		}
		if (this.character.data.currentItem)
		{
			if (this.character.refs.items.throwChargeLevel > 0f)
			{
				this.throwCharge.volume = Mathf.Lerp(this.throwCharge.volume, 0.3f, Time.deltaTime * 10f);
				this.throwCharge.pitch = Mathf.Lerp(this.throwCharge.pitch, 2f + this.character.refs.items.throwChargeLevel * 3f, Time.deltaTime * 10f);
			}
			if (this.prevItem != this.character.data.currentItem)
			{
				for (int i = 0; i < this.switchGeneric.Length; i++)
				{
					this.switchGeneric[i].Play(base.transform.position);
				}
			}
			if (this.character.data.currentItem.GetComponent<ItemUseFeedback>() && this.character.data.currentItem)
			{
				if (this.prevItem != this.character.data.currentItem)
				{
					for (int j = 0; j < this.character.data.currentItem.GetComponent<ItemUseFeedback>().equip.Length; j++)
					{
						this.character.data.currentItem.GetComponent<ItemUseFeedback>().equip[j].Play(base.transform.position);
					}
				}
				string useAnimation = this.character.data.currentItem.GetComponent<ItemUseFeedback>().useAnimation;
				if (!string.IsNullOrEmpty(useAnimation))
				{
					if (this.character.data.currentItem.isUsingPrimary && this.character.data.currentItem.castProgress < 1f)
					{
						this.character.refs.animator.SetBool(useAnimation, true);
					}
					else
					{
						this.character.refs.animator.SetBool(useAnimation, false);
					}
				}
				this.prevUse = useAnimation;
			}
		}
		if (this.prevItem && !this.character.data.currentItem)
		{
			for (int k = 0; k < this.switchGeneric.Length; k++)
			{
				this.switchGeneric[k].Play(base.transform.position);
			}
		}
		this.prevItem = this.character.data.currentItem;
	}

	// Token: 0x04000B9C RID: 2972
	private string prevUse;

	// Token: 0x04000B9D RID: 2973
	private Item prevItem;

	// Token: 0x04000B9E RID: 2974
	public AudioSource throwCharge;

	// Token: 0x04000B9F RID: 2975
	private Character character;

	// Token: 0x04000BA0 RID: 2976
	[HideInInspector]
	public float finishTimer;

	// Token: 0x04000BA1 RID: 2977
	private float increase;

	// Token: 0x04000BA2 RID: 2978
	public SFX_Instance[] switchGeneric;
}
