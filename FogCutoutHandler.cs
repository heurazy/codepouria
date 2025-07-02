using System;
using UnityEngine;

// Token: 0x0200008D RID: 141
public class FogCutoutHandler : MonoBehaviour
{
	// Token: 0x060004E3 RID: 1251 RVA: 0x0001C4FA File Offset: 0x0001A6FA
	public void debugCurrentCutoutZone()
	{
		this.setFogCutoutZone(this.index);
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x0001C508 File Offset: 0x0001A708
	private void OnEnable()
	{
		this.setFogCutoutZone(0);
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x0001C514 File Offset: 0x0001A714
	private void Update()
	{
		if (Character.localCharacter && Character.localCharacter.Center.z > this.currentCutoutZone.transform.position.z + this.currentCutoutZone.transitionPoint && this.index < this.cutoutZones.Length)
		{
			this.setFogCutoutZone(this.index);
			this.index++;
		}
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x0001C58C File Offset: 0x0001A78C
	public void setFogCutoutZone(int zone)
	{
		FogCutoutHandler.<>c__DisplayClass7_0 CS$<>8__locals1 = new FogCutoutHandler.<>c__DisplayClass7_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.zone = zone;
		base.StartCoroutine(CS$<>8__locals1.<setFogCutoutZone>g__changeZoneRoutine|0());
		this.currentCutoutZone = this.cutoutZones[CS$<>8__locals1.zone];
	}

	// Token: 0x04000522 RID: 1314
	public FogCutoutZone[] cutoutZones;

	// Token: 0x04000523 RID: 1315
	private FogCutoutZone currentCutoutZone;

	// Token: 0x04000524 RID: 1316
	public int index;

	// Token: 0x04000525 RID: 1317
	public float fadeTime = 1f;
}
