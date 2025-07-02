using System;
using UnityEngine;

// Token: 0x02000290 RID: 656
public class UIPlayerNames : MonoBehaviour
{
	// Token: 0x06000FB1 RID: 4017 RVA: 0x0004F6B4 File Offset: 0x0004D8B4
	public int Init(CharacterInteractible characterInteractable)
	{
		this.indexCounter++;
		this.playerNameText[this.indexCounter - 1].characterInteractable = characterInteractable;
		this.playerNameText[this.indexCounter - 1].text.text = characterInteractable.GetName();
		for (int i = 0; i < this.playerNameText.Length; i++)
		{
			this.playerNameText[i].gameObject.SetActive(false);
		}
		return this.indexCounter - 1;
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x0004F734 File Offset: 0x0004D934
	public void UpdateName(int index, Vector3 position, bool visible, int speakingAmplitude)
	{
		if (index >= this.playerNameText.Length)
		{
			return;
		}
		this.playerNameText[index].transform.position = MainCamera.instance.cam.WorldToScreenPoint(position);
		if (visible)
		{
			this.playerNameText[index].gameObject.SetActive(true);
			this.playerNameText[index].group.alpha = Mathf.MoveTowards(this.playerNameText[index].group.alpha, 1f, Time.deltaTime * 5f);
			if (this.playerNameText[index].characterInteractable && AudioLevels.GetPlayerLevel(this.playerNameText[index].characterInteractable.character.photonView.OwnerActorNr) == 0f)
			{
				this.playerNameText[index].audioImage.sprite = this.mutedAudioSprite;
				return;
			}
			if (speakingAmplitude > 0)
			{
				this.playerNameText[index].audioImage.sprite = this.audioSprites[Mathf.Clamp(speakingAmplitude, 0, this.audioSprites.Length - 1)];
				this.playerNameText[index].audioImageTimeout = this.audioImageTimeoutMax;
				return;
			}
			this.playerNameText[index].audioImageTimeout -= Time.deltaTime;
			if (this.playerNameText[index].audioImageTimeout <= 0f)
			{
				this.playerNameText[index].audioImage.sprite = this.audioSprites[0];
				return;
			}
		}
		else
		{
			this.playerNameText[index].group.alpha = Mathf.MoveTowards(this.playerNameText[index].group.alpha, 0f, Time.deltaTime * 5f);
			if (this.playerNameText[index].group.alpha < 0.01f && this.playerNameText[index].gameObject.activeSelf)
			{
				this.playerNameText[index].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x0004F91E File Offset: 0x0004DB1E
	public void DisableName(int index)
	{
		if (this.playerNameText[index])
		{
			this.playerNameText[index].gameObject.SetActive(false);
		}
	}

	// Token: 0x04000EB8 RID: 3768
	private int indexCounter;

	// Token: 0x04000EB9 RID: 3769
	public PlayerName[] playerNameText;

	// Token: 0x04000EBA RID: 3770
	public Sprite[] audioSprites;

	// Token: 0x04000EBB RID: 3771
	public Sprite mutedAudioSprite;

	// Token: 0x04000EBC RID: 3772
	public float audioImageTimeoutMax = 1f;
}
