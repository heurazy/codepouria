using System;
using System.Collections;
using UnityEngine;

// Token: 0x020001D2 RID: 466
public class FootStepPlayer : MonoBehaviour
{
	// Token: 0x06000C68 RID: 3176 RVA: 0x0003DA5E File Offset: 0x0003BC5E
	private void Start()
	{
		this.character = base.transform.root.GetComponent<Character>();
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x0003DA78 File Offset: 0x0003BC78
	private void Update()
	{
		this.doStep = 0;
		using (IEnumerator enumerator = base.transform.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (((Transform)enumerator.Current).gameObject.activeSelf)
				{
					this.doStep++;
				}
			}
		}
		if (this.doStep == 0)
		{
			this.t = false;
		}
		if (this.doStep > 0 && !this.t)
		{
			this.PlayStep();
		}
		if (this.character.data.sinceGrounded <= 0f && !this.onGround.active)
		{
			this.onGround.SetActive(true);
			this.offGround.SetActive(false);
			this.PlayStep();
		}
		if (this.character.data.sinceGrounded > 0.25f && !this.offGround.active)
		{
			this.offGround.SetActive(true);
			this.onGround.SetActive(false);
			this.PlayStep();
		}
	}

	// Token: 0x06000C6A RID: 3178 RVA: 0x0003DB94 File Offset: 0x0003BD94
	private void PlayStep()
	{
		if (Physics.Linecast(base.transform.position, base.transform.position + Vector3.down * 100f, out this.hit, this.floorLayer))
		{
			MeshRenderer component = this.hit.collider.GetComponent<MeshRenderer>();
			if (component)
			{
				if (component.material.name == this.beachSand.name + " (Instance)")
				{
					this.surfaceLookup.PlayStep(base.transform.position, 1);
					this.t = true;
					return;
				}
				if (component.material.name == this.beachRock.name + " (Instance)")
				{
					this.surfaceLookup.PlayStep(base.transform.position, 2);
					this.t = true;
					return;
				}
				foreach (Material material in this.jungleGrass)
				{
					if (component.material.name == material.name + " (Instance)")
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 3);
						}
						this.t = true;
					}
				}
				if (component.material.name == this.jungleRock.name + " (Instance)")
				{
					this.surfaceLookup.PlayStep(base.transform.position, 4);
					this.t = true;
					return;
				}
				if (component.material.name == this.iceRock.name + " (Instance)")
				{
					if (this.ambience)
					{
						this.ambience.naturelessTerrain = 30f;
					}
					this.surfaceLookup.PlayStep(base.transform.position, 5);
					this.t = true;
					return;
				}
				if (component.material.name == this.iceSnow.name + " (Instance)")
				{
					if (this.ambience)
					{
						this.ambience.naturelessTerrain = 30f;
					}
					this.surfaceLookup.PlayStep(base.transform.position, 6);
					this.t = true;
					return;
				}
				if (component.material.name == this.volcanoRock.name + " (Instance)")
				{
					if (this.ambience)
					{
						this.ambience.naturelessTerrain = 30f;
						this.ambience.vulcanoT = 10f;
					}
					this.surfaceLookup.PlayStep(base.transform.position, 9);
					this.t = true;
					return;
				}
				foreach (Material material2 in this.metal)
				{
					if (component.material.name == material2.name + " (Instance)")
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 7);
						}
						this.t = true;
					}
				}
				foreach (Material material3 in this.wood)
				{
					if (component.material.name == material3.name + " (Instance)")
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 8);
						}
						this.t = true;
					}
					if (component.material.name == material3.name + " (Instance) (Instance)")
					{
						if (!this.t)
						{
							this.surfaceLookup.PlayStep(base.transform.position, 8);
						}
						this.t = true;
					}
				}
				if (!this.t)
				{
					this.surfaceLookup.PlayStep(base.transform.position, 0);
					this.t = true;
				}
			}
			else
			{
				this.surfaceLookup.PlayStep(base.transform.position, 0);
				this.t = true;
			}
		}
		else
		{
			this.surfaceLookup.PlayStep(base.transform.position, 0);
			this.t = true;
		}
		this.t = true;
	}

	// Token: 0x04000B65 RID: 2917
	private Character character;

	// Token: 0x04000B66 RID: 2918
	public LayerMask floorLayer;

	// Token: 0x04000B67 RID: 2919
	public StepSoundCollection surfaceLookup;

	// Token: 0x04000B68 RID: 2920
	private int doStep;

	// Token: 0x04000B69 RID: 2921
	private bool t;

	// Token: 0x04000B6A RID: 2922
	public Material beachSand;

	// Token: 0x04000B6B RID: 2923
	public Material beachRock;

	// Token: 0x04000B6C RID: 2924
	public Material[] jungleGrass;

	// Token: 0x04000B6D RID: 2925
	public Material jungleRock;

	// Token: 0x04000B6E RID: 2926
	public Material iceSnow;

	// Token: 0x04000B6F RID: 2927
	public Material iceRock;

	// Token: 0x04000B70 RID: 2928
	public Material volcanoRock;

	// Token: 0x04000B71 RID: 2929
	public Material[] metal;

	// Token: 0x04000B72 RID: 2930
	public Material[] wood;

	// Token: 0x04000B73 RID: 2931
	private RaycastHit hit;

	// Token: 0x04000B74 RID: 2932
	public GameObject onGround;

	// Token: 0x04000B75 RID: 2933
	public GameObject offGround;

	// Token: 0x04000B76 RID: 2934
	public AmbienceAudio ambience;
}
