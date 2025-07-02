using System;
using TMPro;
using UnityEngine;

// Token: 0x02000291 RID: 657
public class UI_Interaction : MonoBehaviour
{
	// Token: 0x06000FB5 RID: 4021 RVA: 0x0004F955 File Offset: 0x0004DB55
	private void Start()
	{
		this.text = base.GetComponentInChildren<TextMeshProUGUI>();
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x0004F963 File Offset: 0x0004DB63
	private void Update()
	{
		this.OnChange();
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x0004F96C File Offset: 0x0004DB6C
	private void OnChange()
	{
		this.current = Interaction.instance.currentHovered;
		if (this.current != null)
		{
			this.text.text = this.current.GetInteractionText();
			return;
		}
		this.text.text = "";
	}

	// Token: 0x04000EBD RID: 3773
	private TextMeshProUGUI text;

	// Token: 0x04000EBE RID: 3774
	private IInteractible current;
}
