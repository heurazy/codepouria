using System;
using UnityEngine;

// Token: 0x02000019 RID: 25
[DefaultExecutionOrder(600)]
public class Interaction : MonoBehaviour
{
	// Token: 0x17000018 RID: 24
	// (get) Token: 0x060001AA RID: 426 RVA: 0x0000D0F2 File Offset: 0x0000B2F2
	// (set) Token: 0x060001AB RID: 427 RVA: 0x0000D0FA File Offset: 0x0000B2FA
	public float currentInteractableHeldTime
	{
		get
		{
			return this._cihf;
		}
		set
		{
			this._cihf = value;
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x060001AC RID: 428 RVA: 0x0000D103 File Offset: 0x0000B303
	public float constantInteractableProgress
	{
		get
		{
			return this.currentInteractableHeldTime / this.currentConstantInteractableTime;
		}
	}

	// Token: 0x060001AD RID: 429 RVA: 0x0000D112 File Offset: 0x0000B312
	private void Awake()
	{
		Interaction.instance = this;
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000D11C File Offset: 0x0000B31C
	private void LateUpdate()
	{
		this.currentHovered = null;
		if (!Character.localCharacter)
		{
			return;
		}
		if (Character.localCharacter.data.passedOut || Character.localCharacter.data.fullyPassedOut || !Character.localCharacter.CanDoInput())
		{
			this.bestInteractable = null;
		}
		else
		{
			this.DoInteractableRaycasts(out this.bestInteractable);
			this.bestCharacter = this.bestInteractable as CharacterInteractible;
			this.DoInteraction(this.bestInteractable);
		}
		this.bestInteractableName = ((this.bestInteractable == null) ? "null" : this.bestInteractable.GetTransform().gameObject.name);
		this.currentHovered = this.bestInteractable;
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x060001AF RID: 431 RVA: 0x0000D1D3 File Offset: 0x0000B3D3
	public bool hasValidTargetCharacter
	{
		get
		{
			return this.bestCharacter != null;
		}
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000D1E4 File Offset: 0x0000B3E4
	private void DoInteraction(IInteractible interactable)
	{
		if (Character.localCharacter.input.interactWasReleased && interactable != null && this.currentHeldInteractible == interactable && this.readyToReleaseInteract)
		{
			IInteractibleConstant interactibleConstant = interactable as IInteractibleConstant;
			if (interactibleConstant != null)
			{
				interactibleConstant.ReleaseInteract(Character.localCharacter);
			}
			this.readyToReleaseInteract = false;
		}
		if (!Character.localCharacter.input.interactIsPressed)
		{
			this.readyToInteract = true;
			this.CancelHeldInteract();
		}
		else
		{
			if (this.readyToInteract && interactable != null)
			{
				this.readyToReleaseInteract = true;
				IInteractibleConstant interactibleConstant2 = interactable as IInteractibleConstant;
				if (interactibleConstant2 != null && interactibleConstant2.IsConstantlyInteractable(Character.localCharacter))
				{
					this.currentHeldInteractible = interactibleConstant2;
					this.currentConstantInteractableTime = interactibleConstant2.GetInteractTime(Character.localCharacter);
				}
				Debug.Log("doing interaction with " + interactable.GetTransform().gameObject.name);
				interactable.Interact(Character.localCharacter);
				this.readyToInteract = false;
				return;
			}
			if (Character.localCharacter.input.interactIsPressed && this.currentHeldInteractible != null)
			{
				if (interactable != this.currentHeldInteractible)
				{
					this.currentHeldInteractible = null;
				}
				else
				{
					this.currentInteractableHeldTime += Time.deltaTime;
					if (this.currentInteractableHeldTime >= this.currentConstantInteractableTime)
					{
						this.currentHeldInteractible.Interact_CastFinished(Character.localCharacter);
						this.readyToReleaseInteract = false;
						if (!this.currentHeldInteractible.holdOnFinish)
						{
							this.CancelHeldInteract();
						}
					}
				}
			}
		}
		if (this.currentHeldInteractible == null)
		{
			this.CancelHeldInteract();
		}
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000D34C File Offset: 0x0000B54C
	private void DoInteractableRaycasts(out IInteractible interactableResult)
	{
		Ray ray = new Ray(MainCamera.instance.transform.position, MainCamera.instance.transform.forward);
		RaycastHit[] array = HelperFunctions.LineCheckAll(ray.origin, ray.origin + ray.direction * this.distance, HelperFunctions.LayerType.AllPhysical, 0f, QueryTriggerInteraction.Collide);
		IInteractible interactible = null;
		RaycastHit raycastHit = default(RaycastHit);
		raycastHit.distance = float.MaxValue;
		foreach (RaycastHit raycastHit2 in array)
		{
			if (raycastHit2.distance < raycastHit.distance && !Character.localCharacter.refs.ragdoll.colliderList.Contains(raycastHit2.collider))
			{
				Item componentInParent = raycastHit2.transform.GetComponentInParent<Item>();
				if (!componentInParent || !(componentInParent == Character.localCharacter.data.currentItem))
				{
					raycastHit = raycastHit2;
				}
			}
		}
		if (raycastHit.collider != null)
		{
			IInteractible componentInParent2 = raycastHit.collider.GetComponentInParent<IInteractible>();
			if (componentInParent2 != null && componentInParent2.IsInteractible(Character.localCharacter))
			{
				interactible = componentInParent2;
			}
		}
		bool flag = interactible == null;
		if (flag)
		{
			float num = float.MaxValue;
			this.sphereCastResults = new RaycastHit[100];
			int num2 = Physics.SphereCastNonAlloc(MainCamera.instance.transform.position + MainCamera.instance.transform.forward * (this.area / 2f), this.area, MainCamera.instance.transform.forward, this.sphereCastResults, Mathf.Min(raycastHit.distance, this.distance), HelperFunctions.GetMask(HelperFunctions.LayerType.AllPhysical), QueryTriggerInteraction.Collide);
			int num3 = 0;
			while (num3 < num2 && num3 < this.sphereCastResults.Length)
			{
				RaycastHit raycastHit3 = this.sphereCastResults[num3];
				Item componentInParent3 = raycastHit3.transform.GetComponentInParent<Item>();
				if (!componentInParent3 || !(componentInParent3 == Character.localCharacter.data.currentItem))
				{
					float num4 = Vector3.Angle(raycastHit3.point - MainCamera.instance.transform.position, MainCamera.instance.transform.forward);
					if (flag && num4 < num)
					{
						IInteractible componentInParent4 = raycastHit3.collider.GetComponentInParent<IInteractible>();
						if (componentInParent4 != null && componentInParent4.IsInteractible(Character.localCharacter))
						{
							Item componentInParent5 = raycastHit3.transform.GetComponentInParent<Item>();
							if (!componentInParent5 || !(componentInParent5 == Character.localCharacter.data.currentItem))
							{
								RaycastHit raycastHit4 = HelperFunctions.LineCheck(ray.origin, raycastHit3.point, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Collide);
								if (raycastHit4.collider != null && raycastHit4.collider.GetComponentInParent<IInteractible>() != componentInParent4)
								{
									Debug.DrawLine(ray.origin, raycastHit3.point, Color.red);
								}
								else
								{
									Debug.DrawLine(ray.origin, raycastHit3.point, Color.green);
									num = num4;
									interactible = componentInParent4;
								}
							}
						}
					}
				}
				num3++;
			}
		}
		interactableResult = interactible;
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000D67E File Offset: 0x0000B87E
	private void CancelHeldInteract()
	{
		if (this.currentHeldInteractible != null)
		{
			this.currentHeldInteractible.CancelCast(Character.localCharacter);
		}
		this.currentInteractableHeldTime = 0f;
		this.currentHeldInteractible = null;
	}

	// Token: 0x040001AB RID: 427
	public float distance = 2f;

	// Token: 0x040001AC RID: 428
	public float area = 0.5f;

	// Token: 0x040001AD RID: 429
	public float maxCharacterInteractAngle = 90f;

	// Token: 0x040001AE RID: 430
	public static Interaction instance;

	// Token: 0x040001AF RID: 431
	public IInteractible currentHovered;

	// Token: 0x040001B0 RID: 432
	public IInteractibleConstant currentHeldInteractible;

	// Token: 0x040001B1 RID: 433
	public float currentConstantInteractableTime;

	// Token: 0x040001B2 RID: 434
	private float _cihf;

	// Token: 0x040001B3 RID: 435
	public RaycastHit[] sphereCastResults = new RaycastHit[100];

	// Token: 0x040001B4 RID: 436
	internal IInteractible bestInteractable;

	// Token: 0x040001B5 RID: 437
	[SerializeField]
	internal CharacterInteractible bestCharacter;

	// Token: 0x040001B6 RID: 438
	[HideInInspector]
	public bool readyToInteract = true;

	// Token: 0x040001B7 RID: 439
	[HideInInspector]
	public bool readyToReleaseInteract = true;

	// Token: 0x040001B8 RID: 440
	[SerializeField]
	private string bestInteractableName;
}
