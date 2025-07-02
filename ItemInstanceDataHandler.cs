using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class ItemInstanceDataHandler : RetrievableSingleton<ItemInstanceDataHandler>
{
	// Token: 0x060005E1 RID: 1505 RVA: 0x00020D8C File Offset: 0x0001EF8C
	public IEnumerable<ItemInstanceData> GetAllItemInstances()
	{
		return this.m_instanceData.Values;
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x00020D99 File Offset: 0x0001EF99
	protected override void OnCreated()
	{
		base.OnCreated();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x00020DAC File Offset: 0x0001EFAC
	public static void AddInstanceData(ItemInstanceData instanceData)
	{
		if (!RetrievableSingleton<ItemInstanceDataHandler>.Instance.m_instanceData.TryAdd(instanceData.guid, instanceData))
		{
			throw new Exception(string.Format("Adding item instance with duplicate guid: {0}", instanceData.guid));
		}
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x00020DE4 File Offset: 0x0001EFE4
	public static bool TryGetInstanceData(Guid guid, out ItemInstanceData o)
	{
		ItemInstanceData itemInstanceData;
		if (RetrievableSingleton<ItemInstanceDataHandler>.Instance.m_instanceData.TryGetValue(guid, out itemInstanceData))
		{
			o = itemInstanceData;
			return true;
		}
		o = null;
		return false;
	}

	// Token: 0x040005EA RID: 1514
	private Dictionary<Guid, ItemInstanceData> m_instanceData = new Dictionary<Guid, ItemInstanceData>();
}
