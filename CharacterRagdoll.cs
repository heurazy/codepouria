using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x0200000F RID: 15
[DefaultExecutionOrder(-99)]
public class CharacterRagdoll : MonoBehaviour
{
	// Token: 0x06000152 RID: 338 RVA: 0x0000B3AC File Offset: 0x000095AC
	private void Awake()
	{
		this.character = base.GetComponentInParent<Character>();
		Bodypart[] componentsInChildren = base.GetComponentsInChildren<Bodypart>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.RegisterBodypart(componentsInChildren[i]);
		}
	}

	// Token: 0x06000153 RID: 339 RVA: 0x0000B3E4 File Offset: 0x000095E4
	private void SetPhysicsMats()
	{
		foreach (Bodypart bodypart in this.partList)
		{
			bodypart.SetPhysicsMaterial(this.GetFrictionType(), this.slipperyMat, this.normalMat);
		}
	}

	// Token: 0x06000154 RID: 340 RVA: 0x0000B448 File Offset: 0x00009648
	public void ToggleCollision(bool enableCollision)
	{
		for (int i = 0; i < this.colliderList.Count; i++)
		{
			this.colliderList[i].enabled = enableCollision;
		}
	}

	// Token: 0x06000155 RID: 341 RVA: 0x0000B480 File Offset: 0x00009680
	public void ToggleKinematic(bool enableKinematic)
	{
		this.character.data.isKinecmatic = enableKinematic;
		for (int i = 0; i < this.rigidbodies.Count; i++)
		{
			this.rigidbodies[i].isKinematic = enableKinematic;
		}
	}

	// Token: 0x06000156 RID: 342 RVA: 0x0000B4C6 File Offset: 0x000096C6
	private Bodypart.FrictionType GetFrictionType()
	{
		if (this.character.data.currentRagdollControll < 0.9f)
		{
			return Bodypart.FrictionType.Grippy;
		}
		return Bodypart.FrictionType.Slippery;
	}

	// Token: 0x06000157 RID: 343 RVA: 0x0000B4E4 File Offset: 0x000096E4
	private void Start()
	{
		this.rotationBefore = this.character.refs.rigCreator.transform.rotation;
		if (this.character.refs.ikRigBuilder)
		{
			this.character.refs.ikRigBuilder.Build(this.character.refs.animator.playableGraph);
		}
		this.character.refs.animator.playableGraph.Evaluate(0f);
		this.character.refs.animator.playableGraph.Stop();
		this.character.refs.animator.playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
	}

	// Token: 0x06000158 RID: 344 RVA: 0x0000B5B0 File Offset: 0x000097B0
	private void OnDestroy()
	{
		if (this.m_PlayableGraph.IsValid())
		{
			this.m_PlayableGraph.Destroy();
		}
	}

	// Token: 0x06000159 RID: 345 RVA: 0x0000B5CC File Offset: 0x000097CC
	private void RegisterBodypart(Bodypart bodypart)
	{
		this.partList.Add(bodypart);
		this.partDict.Add(bodypart.partType, bodypart);
		this.rigidbodies.Add(bodypart.Rig);
		bodypart.Rig.mass *= this.massMultiplier;
	}

	// Token: 0x0600015A RID: 346 RVA: 0x0000B620 File Offset: 0x00009820
	public void FixedUpdate()
	{
		this.SetPhysicsMats();
		if (this.firstFrame)
		{
			this.firstFrame = false;
			return;
		}
		if (this.character.data.currentItem)
		{
			this.character.refs.animations.PrepIK();
		}
		this.RotateCharacter();
		this.character.refs.ikRigBuilder.SyncLayers();
		this.character.refs.ikRigBuilder.Evaluate(Time.fixedDeltaTime);
		this.character.refs.animator.playableGraph.Evaluate(Time.fixedDeltaTime);
		this.character.refs.animations.ConfigureIK();
		for (int i = 0; i < this.partList.Count; i++)
		{
			this.partList[i].SaveAnimationData();
			this.DrawLines(this.partList[i].jointParent, this.partList[i]);
		}
		this.SaveAdditionalTransformPositions();
		this.ResetRotation();
		for (int j = 0; j < this.partList.Count; j++)
		{
			this.partList[j].ResetTransform();
		}
	}

	// Token: 0x0600015B RID: 347 RVA: 0x0000B758 File Offset: 0x00009958
	public void SnapToAnimation()
	{
		for (int i = 0; i < this.character.refs.ragdoll.partList.Count; i++)
		{
			this.character.refs.ragdoll.partList[i].SnapToAnim();
		}
	}

	// Token: 0x0600015C RID: 348 RVA: 0x0000B7AC File Offset: 0x000099AC
	private void DrawLines(Bodypart parent, Bodypart part)
	{
		if (parent)
		{
			Debug.DrawLine(this.character.GetAnimationRelativePosition(part.transform.position), this.character.GetAnimationRelativePosition(parent.transform.position), Color.white);
			Debug.DrawLine(this.character.GetAnimationRelativePosition(part.transform.position), part.Rig.position, Color.red);
		}
	}

	// Token: 0x0600015D RID: 349 RVA: 0x0000B822 File Offset: 0x00009A22
	private void RotateCharacter()
	{
		this.rotationBefore = this.character.refs.rigCreator.transform.rotation;
		this.character.SetRotation();
	}

	// Token: 0x0600015E RID: 350 RVA: 0x0000B84F File Offset: 0x00009A4F
	private void ResetRotation()
	{
		this.character.refs.rigCreator.transform.rotation = this.rotationBefore;
	}

	// Token: 0x0600015F RID: 351 RVA: 0x0000B874 File Offset: 0x00009A74
	private void SaveAdditionalTransformPositions()
	{
		Bodypart bodypart = this.character.GetBodypart(BodypartType.Head);
		Vector3 vector = bodypart.transform.position - this.character.refs.rigCreator.transform.position;
		this.character.data.targetHeadHeight = vector.y;
		this.character.refs.animationHeadTransform.position = bodypart.transform.position;
		this.character.refs.animationHeadTransform.rotation = bodypart.transform.rotation;
		this.character.refs.animationLookTransform.position = bodypart.transform.position;
		this.character.refs.animationLookTransform.rotation = Quaternion.Euler(-this.character.data.lookValues.y * 0.5f, this.character.data.lookValues.x, 0f);
		Bodypart bodypart2 = this.character.GetBodypart(BodypartType.Hip);
		Vector3 vector2 = bodypart2.transform.position - this.character.refs.rigCreator.transform.position;
		this.character.data.targetHipHeight = vector2.y;
		this.character.refs.animationHipTransform.position = bodypart2.transform.position;
		this.character.refs.animationHipTransform.rotation = bodypart2.transform.rotation;
		if (this.character.data.currentItem)
		{
			this.character.refs.animationItemTransform.position = this.character.refs.animationLookTransform.TransformPoint(this.character.data.currentItem.defaultPos);
			Vector3 vector3 = this.character.data.lookDirection * this.character.data.currentItem.defaultForward.z;
			vector3 += this.character.data.lookDirection_Right * this.character.data.currentItem.defaultForward.x;
			vector3 += this.character.data.lookDirection_Up * this.character.data.currentItem.defaultForward.y;
			this.character.refs.animationItemTransform.rotation = Quaternion.LookRotation(vector3);
		}
	}

	// Token: 0x06000160 RID: 352 RVA: 0x0000BB24 File Offset: 0x00009D24
	public void HaltBodyVelocity()
	{
		foreach (Rigidbody rigidbody in this.rigidbodies)
		{
			rigidbody.linearVelocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}
	}

	// Token: 0x06000161 RID: 353 RVA: 0x0000BB84 File Offset: 0x00009D84
	public void MoveAllRigsInDirection(Vector3 delta)
	{
		foreach (Rigidbody rigidbody in this.rigidbodies)
		{
			rigidbody.MovePosition(rigidbody.position + delta);
		}
	}

	// Token: 0x06000162 RID: 354 RVA: 0x0000BBE0 File Offset: 0x00009DE0
	internal void SetInterpolation(bool interpolateEnabled)
	{
		foreach (Bodypart bodypart in this.partList)
		{
			bodypart.Rig.interpolation = (interpolateEnabled ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
		}
	}

	// Token: 0x04000150 RID: 336
	public float massMultiplier = 1f;

	// Token: 0x04000151 RID: 337
	public List<Bodypart> partList = new List<Bodypart>();

	// Token: 0x04000152 RID: 338
	public Dictionary<BodypartType, Bodypart> partDict = new Dictionary<BodypartType, Bodypart>();

	// Token: 0x04000153 RID: 339
	private List<Rigidbody> rigidbodies = new List<Rigidbody>();

	// Token: 0x04000154 RID: 340
	private Character character;

	// Token: 0x04000155 RID: 341
	public PhysicsMaterial slipperyMat;

	// Token: 0x04000156 RID: 342
	public PhysicsMaterial normalMat;

	// Token: 0x04000157 RID: 343
	internal List<Collider> colliderList = new List<Collider>();

	// Token: 0x04000158 RID: 344
	private PlayableGraph m_PlayableGraph;

	// Token: 0x04000159 RID: 345
	private bool firstFrame = true;

	// Token: 0x0400015A RID: 346
	private Quaternion rotationBefore;
}
