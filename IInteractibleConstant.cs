using System;

// Token: 0x02000018 RID: 24
public interface IInteractibleConstant : IInteractible
{
	// Token: 0x060001A4 RID: 420
	bool IsConstantlyInteractable(Character interactor);

	// Token: 0x060001A5 RID: 421
	float GetInteractTime(Character interactor);

	// Token: 0x060001A6 RID: 422
	void Interact_CastFinished(Character interactor);

	// Token: 0x060001A7 RID: 423
	void CancelCast(Character interactor);

	// Token: 0x060001A8 RID: 424
	void ReleaseInteract(Character interactor);

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x060001A9 RID: 425
	bool holdOnFinish { get; }
}
