using System;
using UnityEngine;

// Token: 0x02000013 RID: 19
[Serializable]
public class FeedData
{
	// Token: 0x06000180 RID: 384 RVA: 0x0000C3EF File Offset: 0x0000A5EF
	public void PrintDescription()
	{
		Debug.Log(this.GetDescription());
	}

	// Token: 0x06000181 RID: 385 RVA: 0x0000C3FC File Offset: 0x0000A5FC
	public string GetDescription()
	{
		Character character;
		bool characterWithPhotonID = Character.GetCharacterWithPhotonID(this.giverID, out character);
		Character character2;
		Character.GetCharacterWithPhotonID(this.receiverID, out character2);
		Item item;
		bool flag = ItemDatabase.TryGetItem(this.itemID, out item);
		string text = (characterWithPhotonID ? character.characterName : "An unknown scout");
		string text2 = (characterWithPhotonID ? character.characterName : "an unknown scout");
		string text3 = (flag ? item.GetItemName(null) : "an unknown item");
		return string.Concat(new string[] { text, " is feeding ", text2, " a ", text3, "..." });
	}

	// Token: 0x04000180 RID: 384
	public int giverID;

	// Token: 0x04000181 RID: 385
	public int receiverID;

	// Token: 0x04000182 RID: 386
	public ushort itemID;

	// Token: 0x04000183 RID: 387
	public float totalItemTime;
}
