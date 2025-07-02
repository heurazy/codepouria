using System;

// Token: 0x02000064 RID: 100
public abstract class GameService<T> where T : GameService<T>
{
	// Token: 0x17000041 RID: 65
	// (get) Token: 0x060003E8 RID: 1000 RVA: 0x00016E40 File Offset: 0x00015040
	protected static T Instance
	{
		get
		{
			return GameHandler.GetService<T>();
		}
	}
}
