using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x02000192 RID: 402
[DefaultExecutionOrder(1000000)]
public class BingBongPowers : MonoBehaviour
{
	// Token: 0x06000B03 RID: 2819 RVA: 0x0003671A File Offset: 0x0003491A
	private void Start()
	{
		this.SetGodCamStyle();
		base.GetComponentInChildren<Canvas>().enabled = base.GetComponent<PhotonView>().IsMine;
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x00036738 File Offset: 0x00034938
	private void SetGodCamStyle()
	{
		MainCameraMovement component = MainCamera.instance.GetComponent<MainCameraMovement>();
		component.godcam.lookSens = 20f;
		component.godcam.lookDrag = 5f;
		component.godcam.force = 15f;
		component.godcam.drag = 3f;
		component.godcam.canOrbit = false;
	}

	// Token: 0x06000B05 RID: 2821 RVA: 0x0003679A File Offset: 0x0003499A
	private void LateUpdate()
	{
		this.TogglePowers();
		base.transform.position = MainCamera.instance.transform.position;
		base.transform.rotation = MainCamera.instance.transform.rotation;
	}

	// Token: 0x06000B06 RID: 2822 RVA: 0x000367D6 File Offset: 0x000349D6
	private void TogglePowers()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			this.ToggleID(0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			this.ToggleID(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			this.ToggleID(2);
		}
	}

	// Token: 0x06000B07 RID: 2823 RVA: 0x00036808 File Offset: 0x00034A08
	private void ToggleID(int id)
	{
		base.GetComponent<BingBongPhysics>().enabled = false;
		base.GetComponent<BingBongTimeControl>().enabled = false;
		base.GetComponent<BingBongStatus>().enabled = false;
		if (id == 0)
		{
			base.GetComponent<BingBongPhysics>().enabled = true;
		}
		if (id == 1)
		{
			base.GetComponent<BingBongTimeControl>().enabled = true;
		}
		if (id == 2)
		{
			base.GetComponent<BingBongStatus>().enabled = true;
		}
		for (int i = 0; i < this.tooltipBar.childCount; i++)
		{
			if (i == id)
			{
				this.tooltipBar.GetChild(i).GetComponent<CanvasGroup>().alpha = 1f;
			}
			else
			{
				this.tooltipBar.GetChild(i).GetComponent<CanvasGroup>().alpha = 0.5f;
			}
		}
	}

	// Token: 0x06000B08 RID: 2824 RVA: 0x000368BA File Offset: 0x00034ABA
	public void SetTexts(string titleDescr, string description)
	{
		this.titleText.text = titleDescr;
		this.descriptionText.text = description;
	}

	// Token: 0x06000B09 RID: 2825 RVA: 0x000368D4 File Offset: 0x00034AD4
	public void SetTip(string tip, int toolID)
	{
		this.tooltipBar.GetChild(toolID).Find("Tip").GetComponent<TextMeshProUGUI>()
			.text = tip;
	}

	// Token: 0x04000A0E RID: 2574
	public TextMeshProUGUI titleText;

	// Token: 0x04000A0F RID: 2575
	public TextMeshProUGUI descriptionText;

	// Token: 0x04000A10 RID: 2576
	public Transform tooltipBar;
}
