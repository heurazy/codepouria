using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000168 RID: 360
public class PassportButton : MonoBehaviour
{
	// Token: 0x06000A49 RID: 2633 RVA: 0x00032464 File Offset: 0x00030664
	public void SetButton(CustomizationOption option, int index)
	{
		if (option != null)
		{
			base.gameObject.SetActive(true);
			if (option.IsLocked && !this.manager.testUnlockAll)
			{
				this.lockedIcon.gameObject.SetActive(true);
				this.icon.gameObject.SetActive(false);
			}
			else
			{
				this.lockedIcon.gameObject.SetActive(false);
				this.icon.gameObject.SetActive(true);
				this.icon.texture = option.texture;
				if (option.type == Customization.Type.Skin)
				{
					this.icon.color = option.color;
				}
				else
				{
					this.icon.color = Color.white;
				}
				if (option.type == Customization.Type.Eyes)
				{
					this.icon.material = this.eyeMaterial;
				}
				else
				{
					this.icon.material = null;
				}
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
		this.currentOption = option;
		this.currentIndex = index;
	}

	// Token: 0x06000A4A RID: 2634 RVA: 0x00032568 File Offset: 0x00030768
	public void Click()
	{
		if (!this.currentOption.IsLocked || this.manager.testUnlockAll)
		{
			this.manager.SetOption(this.currentOption, this.currentIndex);
		}
		EventSystem.current.SetSelectedGameObject(null);
	}

	// Token: 0x0400090D RID: 2317
	public Button button;

	// Token: 0x0400090E RID: 2318
	public PassportManager manager;

	// Token: 0x0400090F RID: 2319
	public RawImage icon;

	// Token: 0x04000910 RID: 2320
	public RawImage lockedIcon;

	// Token: 0x04000911 RID: 2321
	public Image border;

	// Token: 0x04000912 RID: 2322
	private CustomizationOption currentOption;

	// Token: 0x04000913 RID: 2323
	private int currentIndex;

	// Token: 0x04000914 RID: 2324
	public Material eyeMaterial;
}
