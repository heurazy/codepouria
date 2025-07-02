using System;
using UnityEngine;

// Token: 0x02000045 RID: 69
public class BadgeUnlocker : MonoBehaviour
{
	// Token: 0x0600033E RID: 830 RVA: 0x000140EC File Offset: 0x000122EC
	private void Start()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x0600033F RID: 831 RVA: 0x000140FC File Offset: 0x000122FC
	public void Update()
	{
		if (this.useTestBadge)
		{
			int num = GUIManager.instance.mainBadgeManager.badgeData.Length;
			Texture2D texture2D = new Texture2D(num, 1);
			texture2D.filterMode = FilterMode.Point;
			for (int i = 0; i < num; i++)
			{
				texture2D.SetPixel(i, 1, Color.black);
			}
			texture2D.SetPixel(this.testBadge, 1, Color.white);
			texture2D.Apply();
			this.badgeSashRenderer.materials[0].SetTexture("BadgeUnlockTexture", texture2D);
		}
	}

	// Token: 0x06000340 RID: 832 RVA: 0x0001417C File Offset: 0x0001237C
	public static void SetBadges(Character refCharacter, Renderer sashRenderer)
	{
		int num = refCharacter.data.badgeStatus.Length;
		Texture2D texture2D = new Texture2D(num, 1);
		texture2D.filterMode = FilterMode.Point;
		for (int i = 0; i < num; i++)
		{
			if (refCharacter.data.badgeStatus[i])
			{
				texture2D.SetPixel(GUIManager.instance.mainBadgeManager.badgeData[i].visualID, 1, Color.white);
			}
			else
			{
				texture2D.SetPixel(GUIManager.instance.mainBadgeManager.badgeData[i].visualID, 1, Color.black);
			}
		}
		texture2D.Apply();
		if (sashRenderer == null)
		{
			return;
		}
		sashRenderer.materials[0].SetTexture("BadgeUnlockTexture", texture2D);
	}

	// Token: 0x06000341 RID: 833 RVA: 0x0001422A File Offset: 0x0001242A
	public void BadgeUnlockVisual()
	{
		if (!this.character)
		{
			this.character = base.GetComponent<Character>();
		}
		BadgeUnlocker.SetBadges(this.character, this.badgeSashRenderer);
	}

	// Token: 0x040003C8 RID: 968
	public int testBadge;

	// Token: 0x040003C9 RID: 969
	public bool useTestBadge;

	// Token: 0x040003CA RID: 970
	private Character character;

	// Token: 0x040003CB RID: 971
	public Renderer badgeSashRenderer;
}
