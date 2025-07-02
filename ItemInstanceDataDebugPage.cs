using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Zorro.Core.CLI;

// Token: 0x020000A4 RID: 164
public class ItemInstanceDataDebugPage : DebugPage
{
	// Token: 0x060005DF RID: 1503 RVA: 0x00020C5D File Offset: 0x0001EE5D
	public ItemInstanceDataDebugPage()
	{
		this.ScrollView = new ScrollView();
		base.Add(this.ScrollView);
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00020C88 File Offset: 0x0001EE88
	public override void Update()
	{
		base.Update();
		foreach (ItemInstanceData itemInstanceData in RetrievableSingleton<ItemInstanceDataHandler>.Instance.GetAllItemInstances())
		{
			if (!this.m_spawnedCells.ContainsKey(itemInstanceData.guid))
			{
				DataEntryValue dataEntryValue;
				if (itemInstanceData.data.Count == 1 && itemInstanceData.data.TryGetValue(DataEntryKey.ItemUses, out dataEntryValue))
				{
					OptionableIntItemData optionableIntItemData = dataEntryValue as OptionableIntItemData;
					if (optionableIntItemData != null && !optionableIntItemData.HasData)
					{
						continue;
					}
				}
				ItemInstanceDataUICell itemInstanceDataUICell = new ItemInstanceDataUICell(itemInstanceData);
				this.ScrollView.Add(itemInstanceDataUICell);
				this.m_spawnedCells.Add(itemInstanceData.guid, itemInstanceDataUICell);
			}
		}
		foreach (KeyValuePair<Guid, ItemInstanceDataUICell> keyValuePair in this.m_spawnedCells)
		{
			keyValuePair.Value.Update();
		}
	}

	// Token: 0x040005E8 RID: 1512
	private Dictionary<Guid, ItemInstanceDataUICell> m_spawnedCells = new Dictionary<Guid, ItemInstanceDataUICell>();

	// Token: 0x040005E9 RID: 1513
	private ScrollView ScrollView;
}
