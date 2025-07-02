using System;
using UnityEngine;

// Token: 0x0200000B RID: 11
public class CharacterHeatEmission : MonoBehaviour
{
	// Token: 0x060000ED RID: 237 RVA: 0x000076A8 File Offset: 0x000058A8
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
	}

	// Token: 0x060000EE RID: 238 RVA: 0x000076B8 File Offset: 0x000058B8
	public void Update()
	{
		base.transform.position = this.character.refs.hip.transform.position;
		if (this.character.data.sinceAddedCold < 3f)
		{
			return;
		}
		this.counter += Time.deltaTime;
		if (this.counter < this.rate)
		{
			return;
		}
		this.counter = 0f;
		foreach (Character character in Character.AllCharacters)
		{
			if (Vector3.Distance(base.transform.position, character.Center) < this.radius)
			{
				character.refs.afflictions.SubtractStatus(CharacterAfflictions.STATUSTYPE.Cold, this.heatAmount, false);
			}
		}
	}

	// Token: 0x060000EF RID: 239 RVA: 0x000077A4 File Offset: 0x000059A4
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x040000D2 RID: 210
	public float radius = 1f;

	// Token: 0x040000D3 RID: 211
	public float heatAmount = 0.05f;

	// Token: 0x040000D4 RID: 212
	public float rate = 0.5f;

	// Token: 0x040000D5 RID: 213
	private float counter;

	// Token: 0x040000D6 RID: 214
	private Character character;
}
