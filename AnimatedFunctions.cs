using System;
using UnityEngine;

// Token: 0x02000184 RID: 388
public class AnimatedFunctions : MonoBehaviour
{
	// Token: 0x06000AC3 RID: 2755 RVA: 0x00034F3B File Offset: 0x0003313B
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x00034F49 File Offset: 0x00033149
	private void Start()
	{
		this.left = this.character.GetBodypart(BodypartType.Foot_L).GetComponentInChildren<RigCreatorCollider>().GetComponent<Collider>();
		this.right = this.character.GetBodypart(BodypartType.Foot_R).GetComponentInChildren<RigCreatorCollider>().GetComponent<Collider>();
	}

	// Token: 0x040009CC RID: 2508
	private Collider left;

	// Token: 0x040009CD RID: 2509
	private Collider right;

	// Token: 0x040009CE RID: 2510
	private Character character;
}
