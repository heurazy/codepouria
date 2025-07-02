using System;
using UnityEngine;

// Token: 0x020001A5 RID: 421
public class CharacterInteractible : MonoBehaviour, IInteractible
{
	// Token: 0x06000BA3 RID: 2979 RVA: 0x0003A5A9 File Offset: 0x000387A9
	private void Start()
	{
		this.character = base.GetComponent<Character>();
	}

	// Token: 0x06000BA4 RID: 2980 RVA: 0x0003A5B7 File Offset: 0x000387B7
	public Vector3 Center()
	{
		return this.character.Center;
	}

	// Token: 0x06000BA5 RID: 2981 RVA: 0x0003A5C4 File Offset: 0x000387C4
	public string GetInteractionText()
	{
		if (this.CarriedByLocalCharacter())
		{
			return "Drop " + this.GetName();
		}
		if (this.CanBeCarried())
		{
			return "Carry " + this.GetName();
		}
		return "";
	}

	// Token: 0x06000BA6 RID: 2982 RVA: 0x0003A5FD File Offset: 0x000387FD
	public string GetSecondaryInteractionText()
	{
		if (this.HasItemCanUseOnFriend())
		{
			return this.GetItemPrompt(Character.localCharacter.data.currentItem);
		}
		return "";
	}

	// Token: 0x06000BA7 RID: 2983 RVA: 0x0003A622 File Offset: 0x00038822
	public string GetItemPrompt(Item item)
	{
		return item.UIData.secondaryInteractPrompt.Replace("#targetChar", this.GetName());
	}

	// Token: 0x06000BA8 RID: 2984 RVA: 0x0003A63F File Offset: 0x0003883F
	public string GetName()
	{
		return this.character.characterName;
	}

	// Token: 0x06000BA9 RID: 2985 RVA: 0x0003A64C File Offset: 0x0003884C
	private bool CarriedByLocalCharacter()
	{
		return this.character.data.carrier && this.character.data.carrier == Character.localCharacter;
	}

	// Token: 0x06000BAA RID: 2986 RVA: 0x0003A681 File Offset: 0x00038881
	private bool CanBeCarried()
	{
		return this.character.data.fullyPassedOut && !this.character.data.dead && !this.character.data.carrier;
	}

	// Token: 0x06000BAB RID: 2987 RVA: 0x0003A6C4 File Offset: 0x000388C4
	private bool HasItemCanUseOnFriend()
	{
		return !this.character.data.dead && this.character != Character.localCharacter && Character.localCharacter.data.currentItem && Character.localCharacter.data.currentItem.canUseOnFriend;
	}

	// Token: 0x06000BAC RID: 2988 RVA: 0x0003A721 File Offset: 0x00038921
	public Transform GetTransform()
	{
		return this.character.GetBodypart(BodypartType.Torso).transform;
	}

	// Token: 0x06000BAD RID: 2989 RVA: 0x0003A734 File Offset: 0x00038934
	public void HoverEnter()
	{
	}

	// Token: 0x06000BAE RID: 2990 RVA: 0x0003A736 File Offset: 0x00038936
	public void HoverExit()
	{
	}

	// Token: 0x06000BAF RID: 2991 RVA: 0x0003A738 File Offset: 0x00038938
	public void Interact(Character interactor)
	{
		if (this.CarriedByLocalCharacter())
		{
			interactor.refs.carriying.Drop(this.character);
			return;
		}
		if (this.CanBeCarried())
		{
			interactor.refs.carriying.StartCarry(this.character);
			return;
		}
	}

	// Token: 0x06000BB0 RID: 2992 RVA: 0x0003A778 File Offset: 0x00038978
	public bool IsInteractible(Character interactor)
	{
		return this.IsPrimaryInteractible(interactor) || this.IsSecondaryInteractible(interactor);
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x0003A78C File Offset: 0x0003898C
	public bool IsPrimaryInteractible(Character interactor)
	{
		return this.CarriedByLocalCharacter() || this.CanBeCarried();
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x0003A7A4 File Offset: 0x000389A4
	public bool IsSecondaryInteractible(Character interactor)
	{
		if (!this.HasItemCanUseOnFriend())
		{
			return false;
		}
		if (this.character.data.fullyPassedOut)
		{
			return true;
		}
		Vector3 vector = HelperFunctions.ZeroY(this.character.data.lookDirection);
		Vector3 vector2 = HelperFunctions.ZeroY(interactor.data.lookDirection);
		return Vector3.Angle(vector, -vector2) <= Interaction.instance.maxCharacterInteractAngle;
	}

	// Token: 0x04000A90 RID: 2704
	public Character character;
}
