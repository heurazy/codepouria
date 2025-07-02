using System;
using System.Collections.Generic;
using Zorro.Core;
using Zorro.Core.CLI;

// Token: 0x02000058 RID: 88
[TypeParser(typeof(Item))]
public class ItemCLIParser : CLITypeParser
{
	// Token: 0x060003B8 RID: 952 RVA: 0x00016854 File Offset: 0x00014A54
	public override object Parse(string str)
	{
		return ObjectDatabaseAsset<ItemDatabase, Item>.GetObjectFromString(str);
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x0001685C File Offset: 0x00014A5C
	public override List<ParameterAutocomplete> FindAutocomplete(string parameterText)
	{
		return SingletonAsset<ItemDatabase>.Instance.GetAutocompleteOptions(parameterText);
	}
}
