using System;
using UnityEngine;

// Token: 0x020001DD RID: 477
public class IsLookedAt : MonoBehaviour
{
	// Token: 0x06000C98 RID: 3224 RVA: 0x0003EA38 File Offset: 0x0003CC38
	private void Start()
	{
		if (this.characterInteractible.character == Character.localCharacter)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.index = GUIManager.instance.playerNames.Init(this.characterInteractible);
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x0003EA84 File Offset: 0x0003CC84
	private void Update()
	{
		bool flag = false;
		float num = Vector3.Distance(MainCamera.instance.transform.position, base.transform.position);
		float num2 = Vector3.Angle(MainCamera.instance.transform.forward, base.transform.position - MainCamera.instance.transform.position);
		if (num < this.visibleDistance && num2 < this.visibleAngle + (this.visibleDistance - num) / this.visibleDistance * this.angleDistRatio)
		{
			flag = true;
		}
		if (this.mouth.character.data.isBlind)
		{
			flag = false;
		}
		GUIManager.instance.playerNames.UpdateName(this.index, this.playerNamePos.position, flag, this.mouth.amplitudeIndex);
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x0003EB57 File Offset: 0x0003CD57
	private void OnDisable()
	{
		GUIManager.instance.playerNames.DisableName(this.index);
	}

	// Token: 0x04000B95 RID: 2965
	public AnimatedMouth mouth;

	// Token: 0x04000B96 RID: 2966
	public CharacterInteractible characterInteractible;

	// Token: 0x04000B97 RID: 2967
	public float visibleDistance = 8f;

	// Token: 0x04000B98 RID: 2968
	public float visibleAngle = 45f;

	// Token: 0x04000B99 RID: 2969
	public float angleDistRatio = 45f;

	// Token: 0x04000B9A RID: 2970
	public Transform playerNamePos;

	// Token: 0x04000B9B RID: 2971
	private int index;
}
