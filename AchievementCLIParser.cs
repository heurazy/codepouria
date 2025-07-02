using System;
using System.Collections.Generic;
using Zorro.Core.CLI;

// Token: 0x02000037 RID: 55
[TypeParser(typeof(ACHIEVEMENTTYPE))]
public class AchievementCLIParser : CLITypeParser
{
	// Token: 0x060002CE RID: 718 RVA: 0x000126B4 File Offset: 0x000108B4
	public override object Parse(string str)
	{
		ACHIEVEMENTTYPE achievementtype;
		if (Enum.TryParse<ACHIEVEMENTTYPE>(str, out achievementtype))
		{
			return achievementtype;
		}
		return ACHIEVEMENTTYPE.NONE;
	}

	// Token: 0x060002CF RID: 719 RVA: 0x000126D8 File Offset: 0x000108D8
	public override List<ParameterAutocomplete> FindAutocomplete(string parameterText)
	{
		List<ParameterAutocomplete> list = new List<ParameterAutocomplete>();
		foreach (ACHIEVEMENTTYPE achievementtype in (ACHIEVEMENTTYPE[])Enum.GetValues(typeof(ACHIEVEMENTTYPE)))
		{
			list.Add(new ParameterAutocomplete(achievementtype.ToString()));
		}
		return list;
	}
}
