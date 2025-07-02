using System;

// Token: 0x02000267 RID: 615
[Serializable]
public class StringAndFloat
{
	// Token: 0x06000EDD RID: 3805 RVA: 0x0004ACBB File Offset: 0x00048EBB
	public StringAndFloat(string name, float value)
	{
		this.name = name;
		this.value = value;
	}

	// Token: 0x04000DB7 RID: 3511
	public string name;

	// Token: 0x04000DB8 RID: 3512
	public float value;
}
