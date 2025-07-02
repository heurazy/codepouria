using System;
using UnityEngine;

// Token: 0x020001DC RID: 476
public class InjurySphere : MonoBehaviour
{
	// Token: 0x06000C95 RID: 3221 RVA: 0x0003E99B File Offset: 0x0003CB9B
	private void Start()
	{
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x0003E9A0 File Offset: 0x0003CBA0
	private void Update()
	{
		if (Vector3.Distance(Character.localCharacter.data.groundPos, base.transform.position) < base.transform.localScale.x / 2f)
		{
			if (this.isHealing)
			{
				Character.localCharacter.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Injury, Time.deltaTime * 0.2f, false);
				return;
			}
			Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, Time.deltaTime * 0.2f, false);
		}
	}

	// Token: 0x04000B94 RID: 2964
	public bool isHealing;
}
