using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000151 RID: 337
public class EmoteWheel : UIWheel
{
	// Token: 0x060009A1 RID: 2465 RVA: 0x00030333 File Offset: 0x0002E533
	public void OnEnable()
	{
		this.InitWheel();
	}

	// Token: 0x060009A2 RID: 2466 RVA: 0x0003033B File Offset: 0x0002E53B
	public void OnDisable()
	{
		this.Choose();
	}

	// Token: 0x060009A3 RID: 2467 RVA: 0x00030344 File Offset: 0x0002E544
	public void InitWheel()
	{
		this.chosenEmoteData = null;
		for (int i = 0; i < this.slices.Length; i++)
		{
			this.slices[i].Init(this.data[i], this);
		}
		this.selectedEmoteName.text = "";
	}

	// Token: 0x060009A4 RID: 2468 RVA: 0x00030391 File Offset: 0x0002E591
	public void Choose()
	{
		if (this.chosenEmoteData != null)
		{
			Character.localCharacter.refs.animations.PlayEmote(this.chosenEmoteData.anim);
		}
	}

	// Token: 0x060009A5 RID: 2469 RVA: 0x000303C0 File Offset: 0x0002E5C0
	public void Hover(EmoteWheelData emoteWheelData)
	{
		this.selectedEmoteName.text = emoteWheelData.emoteName;
		this.chosenEmoteData = emoteWheelData;
	}

	// Token: 0x060009A6 RID: 2470 RVA: 0x000303DA File Offset: 0x0002E5DA
	public void Dehover(EmoteWheelData emoteWheelData)
	{
		if (this.chosenEmoteData == emoteWheelData)
		{
			this.selectedEmoteName.text = "";
			this.chosenEmoteData = null;
		}
	}

	// Token: 0x060009A7 RID: 2471 RVA: 0x00030404 File Offset: 0x0002E604
	protected override void TestSelectSliceGamepad(Vector2 gamepadVector)
	{
		float num = 0f;
		EmoteWheelSlice emoteWheelSlice = null;
		if (gamepadVector.sqrMagnitude >= 0.5f)
		{
			for (int i = 0; i < this.slices.Length; i++)
			{
				float num2 = Vector3.Angle(gamepadVector, this.slices[i].GetUpVector());
				if (emoteWheelSlice == null || num2 < num)
				{
					emoteWheelSlice = this.slices[i];
					num = num2;
				}
			}
		}
		if (emoteWheelSlice != null)
		{
			EventSystem.current.SetSelectedGameObject(emoteWheelSlice.button.gameObject);
			emoteWheelSlice.Hover();
			return;
		}
		EventSystem.current.SetSelectedGameObject(null);
		this.Dehover(this.chosenEmoteData);
	}

	// Token: 0x0400087E RID: 2174
	public EmoteWheelSlice[] slices;

	// Token: 0x0400087F RID: 2175
	public EmoteWheelData[] data;

	// Token: 0x04000880 RID: 2176
	public TextMeshProUGUI selectedEmoteName;

	// Token: 0x04000881 RID: 2177
	private EmoteWheelData chosenEmoteData;
}
