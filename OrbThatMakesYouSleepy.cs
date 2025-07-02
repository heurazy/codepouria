using System;
using UnityEngine;
using Zorro.Core;

// Token: 0x020000E3 RID: 227
public class OrbThatMakesYouSleepy : MonoBehaviour
{
	// Token: 0x060006EC RID: 1772 RVA: 0x000242E4 File Offset: 0x000224E4
	private void Start()
	{
		this.anim.speed = this.animSpeed;
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x000242F7 File Offset: 0x000224F7
	public void Tick()
	{
		this.UpdateHypnosis(false);
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00024300 File Offset: 0x00022500
	private void LateUpdate()
	{
		if (this.napBerry != null)
		{
			this.napBerry.transform.localPosition = new Vector3(-0.013f, -0.22f, 0.008f);
			this.napBerry.transform.localEulerAngles = Vector3.zero;
			return;
		}
		this.anim.speed = 0f;
		this.ambientParticles.gameObject.SetActive(false);
		base.enabled = false;
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x0002437D File Offset: 0x0002257D
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(this.orb.transform.position, this.orbRadius);
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x000243A4 File Offset: 0x000225A4
	private void UpdateCastPositions()
	{
		this.castPositions[0] = this.orb.transform.position;
		this.castPositions[1] = this.orb.transform.position + MainCamera.instance.cam.transform.right * this.orbRadius;
		this.castPositions[2] = this.orb.transform.position - MainCamera.instance.cam.transform.right * this.orbRadius;
		this.castPositions[3] = this.orb.transform.position + MainCamera.instance.cam.transform.up * this.orbRadius;
		this.castPositions[4] = this.orb.transform.position - MainCamera.instance.cam.transform.up * this.orbRadius;
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x000244CD File Offset: 0x000226CD
	private void DebugHypnosis()
	{
		this.UpdateHypnosis(true);
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x000244D8 File Offset: 0x000226D8
	private void UpdateHypnosis(bool debug = false)
	{
		if (!base.enabled)
		{
			return;
		}
		if (!Character.localCharacter.UnityObjectExists<Character>() || !Character.localCharacter.data.fullyConscious)
		{
			return;
		}
		Vector3 vector = Character.localCharacter.Center - this.orb.transform.position;
		if (debug)
		{
			Debug.Log("distance to character: " + vector.magnitude.ToString());
		}
		if (vector.magnitude > this.maxDistance)
		{
			return;
		}
		if (!GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(MainCamera.instance.cam), new Bounds(this.orb.transform.position, Vector3.one * 0.5f)))
		{
			if (debug)
			{
				Debug.Log("Not inside view frustum");
			}
			return;
		}
		int num = 0;
		this.UpdateCastPositions();
		for (int i = 0; i < this.castPositions.Length; i++)
		{
			if (debug)
			{
				Debug.Log(string.Format("testing cast {0}", i));
			}
			Collider collider = HelperFunctions.LineCheck(this.castPositions[i], MainCamera.instance.cam.transform.position, HelperFunctions.LayerType.AllPhysical, 0f, QueryTriggerInteraction.Ignore).collider;
			if (collider == null)
			{
				if (debug)
				{
					Debug.Log("Hit nothing");
				}
				num++;
			}
			else if (collider.gameObject.GetComponentInParent<Character>() == Character.localCharacter)
			{
				if (debug)
				{
					Debug.Log("Hit our own character");
				}
				num++;
			}
		}
		if (num == 0)
		{
			if (debug)
			{
				Debug.Log("Blocked");
			}
			return;
		}
		float num2 = Vector3.Angle(-MainCamera.instance.cam.transform.forward, vector);
		float num3 = Mathf.InverseLerp(this.maxDistance, 2f, vector.magnitude);
		if (debug)
		{
			Debug.Log(string.Format("factor 1: {0}", num3));
		}
		float num4 = Mathf.Lerp(10f, 110f, num3);
		if (debug)
		{
			Debug.Log(string.Format("max angle: {0}", num4));
		}
		float num5 = Mathf.InverseLerp(num4, num4 / 2f, num2);
		if (debug)
		{
			Debug.Log(string.Format("factor 2 {0}", num5));
		}
		float num6 = Mathf.Lerp(this.minDrowsyPerTick, this.maxDrowsyPerTick, Mathf.Min(num3, num5));
		if (num <= 2)
		{
			num6 *= 0.5f;
		}
		if (debug)
		{
			Debug.Log(string.Format("Adding Status: {0}", num6));
		}
		Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Drowsy, num6, false);
		this.particle.Play();
	}

	// Token: 0x0400067C RID: 1660
	public Transform orb;

	// Token: 0x0400067D RID: 1661
	public float orbRadius;

	// Token: 0x0400067E RID: 1662
	public float maxDistance;

	// Token: 0x0400067F RID: 1663
	public float minDrowsyPerSecond;

	// Token: 0x04000680 RID: 1664
	public float maxDrowsyPerSecond;

	// Token: 0x04000681 RID: 1665
	public float minDrowsyPerTick;

	// Token: 0x04000682 RID: 1666
	public float maxDrowsyPerTick;

	// Token: 0x04000683 RID: 1667
	private Vector3[] castPositions = new Vector3[5];

	// Token: 0x04000684 RID: 1668
	public ParticleSystem particle;

	// Token: 0x04000685 RID: 1669
	public Animator anim;

	// Token: 0x04000686 RID: 1670
	public float animSpeed = 1f;

	// Token: 0x04000687 RID: 1671
	public GameObject napBerry;

	// Token: 0x04000688 RID: 1672
	public ParticleSystem ambientParticles;

	// Token: 0x04000689 RID: 1673
	private Plane[] planes = new Plane[6];
}
