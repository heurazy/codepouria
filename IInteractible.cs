using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public interface IInteractible
{
	// Token: 0x0600019C RID: 412
	bool IsInteractible(Character interactor);

	// Token: 0x0600019D RID: 413
	void Interact(Character interactor);

	// Token: 0x0600019E RID: 414
	void HoverEnter();

	// Token: 0x0600019F RID: 415
	void HoverExit();

	// Token: 0x060001A0 RID: 416
	Vector3 Center();

	// Token: 0x060001A1 RID: 417
	Transform GetTransform();

	// Token: 0x060001A2 RID: 418
	string GetInteractionText();

	// Token: 0x060001A3 RID: 419
	string GetName();
}
