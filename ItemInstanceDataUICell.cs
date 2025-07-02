using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

// Token: 0x020000A6 RID: 166
public class ItemInstanceDataUICell : VisualElement
{
	// Token: 0x060005E6 RID: 1510 RVA: 0x00020E24 File Offset: 0x0001F024
	public ItemInstanceDataUICell(ItemInstanceData data)
	{
		this.data = data;
		this.label = new Label();
		this.label.AddToClassList("info");
		base.Add(this.label);
		this.label.text = data.guid.ToString();
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00020E84 File Offset: 0x0001F084
	public void Update()
	{
		string text = this.data.guid.ToString();
		text += string.Format(" - enteries: {0}", this.data.data.Count);
		foreach (KeyValuePair<DataEntryKey, DataEntryValue> keyValuePair in this.data.data)
		{
			text += string.Format("\n{0} : {1}", keyValuePair.Key, keyValuePair.Value.GetType().Name);
			text += "\n---";
			text = text + "\n" + keyValuePair.Value.ToString();
			text += "\n---";
		}
		this.label.text = text;
	}

	// Token: 0x040005EB RID: 1515
	private ItemInstanceData data;

	// Token: 0x040005EC RID: 1516
	private Label label;
}
