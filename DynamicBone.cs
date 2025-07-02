using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000075 RID: 117
[AddComponentMenu("Dynamic Bone/Dynamic Bone")]
public class DynamicBone : MonoBehaviour
{
	// Token: 0x06000423 RID: 1059 RVA: 0x00017D68 File Offset: 0x00015F68
	private void Start()
	{
		if (!this.m_Root)
		{
			this.m_Root = base.transform;
		}
		this.SetupParticles();
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x00017D89 File Offset: 0x00015F89
	private void FixedUpdate()
	{
		if (this.m_UpdateMode == DynamicBone.UpdateMode.AnimatePhysics)
		{
			this.PreUpdate();
		}
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x00017D9A File Offset: 0x00015F9A
	private void Update()
	{
		if (this.m_UpdateMode != DynamicBone.UpdateMode.AnimatePhysics)
		{
			this.PreUpdate();
		}
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x00017DAC File Offset: 0x00015FAC
	private void LateUpdate()
	{
		if (this.m_DistantDisable)
		{
			this.CheckDistance();
		}
		if (this.m_Weight > 0f && (!this.m_DistantDisable || !this.m_DistantDisabled))
		{
			float num = ((this.m_UpdateMode == DynamicBone.UpdateMode.UnscaledTime) ? Time.unscaledDeltaTime : Time.deltaTime);
			this.UpdateDynamicBones(num);
		}
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x00017E01 File Offset: 0x00016001
	private void PreUpdate()
	{
		if (this.m_Weight > 0f && (!this.m_DistantDisable || !this.m_DistantDisabled))
		{
			this.InitTransforms();
		}
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x00017E28 File Offset: 0x00016028
	private void CheckDistance()
	{
		Transform transform = this.m_ReferenceObject;
		if (transform == null && Camera.main != null)
		{
			transform = Camera.main.transform;
		}
		if (transform != null)
		{
			bool flag = (transform.position - base.transform.position).sqrMagnitude > this.m_DistanceToObject * this.m_DistanceToObject;
			if (flag != this.m_DistantDisabled)
			{
				if (!flag)
				{
					this.ResetParticlesPosition();
				}
				this.m_DistantDisabled = flag;
			}
		}
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x00017EAD File Offset: 0x000160AD
	private void OnEnable()
	{
		this.ResetParticlesPosition();
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x00017EB5 File Offset: 0x000160B5
	private void OnDisable()
	{
		this.InitTransforms();
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x00017EC0 File Offset: 0x000160C0
	private void OnValidate()
	{
		this.m_UpdateRate = Mathf.Max(this.m_UpdateRate, 0f);
		this.m_Damping = Mathf.Clamp01(this.m_Damping);
		this.m_Elasticity = Mathf.Clamp01(this.m_Elasticity);
		this.m_Stiffness = Mathf.Clamp01(this.m_Stiffness);
		this.m_Inert = Mathf.Clamp01(this.m_Inert);
		this.m_Friction = Mathf.Clamp01(this.m_Friction);
		this.m_Radius = Mathf.Max(this.m_Radius, 0f);
		if (Application.isEditor && Application.isPlaying)
		{
			this.InitTransforms();
			this.SetupParticles();
		}
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x00017F68 File Offset: 0x00016168
	private void OnDrawGizmosSelected()
	{
		if (!base.enabled || this.m_Root == null)
		{
			return;
		}
		if (Application.isEditor && !Application.isPlaying && base.transform.hasChanged)
		{
			this.InitTransforms();
			this.SetupParticles();
		}
		Gizmos.color = Color.white;
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_ParentIndex >= 0)
			{
				DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
				Gizmos.DrawLine(particle.m_Position, particle2.m_Position);
			}
			if (particle.m_Radius > 0f)
			{
				Gizmos.DrawWireSphere(particle.m_Position, particle.m_Radius * this.m_ObjectScale);
			}
		}
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x00018031 File Offset: 0x00016231
	public void SetWeight(float w)
	{
		if (this.m_Weight != w)
		{
			if (w == 0f)
			{
				this.InitTransforms();
			}
			else if (this.m_Weight == 0f)
			{
				this.ResetParticlesPosition();
			}
			this.m_Weight = w;
		}
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x00018066 File Offset: 0x00016266
	public float GetWeight()
	{
		return this.m_Weight;
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x00018070 File Offset: 0x00016270
	private void UpdateDynamicBones(float t)
	{
		if (this.m_Root == null)
		{
			return;
		}
		this.m_ObjectScale = Mathf.Abs(base.transform.lossyScale.x);
		this.m_ObjectMove = base.transform.position - this.m_ObjectPrevPosition;
		this.m_ObjectPrevPosition = base.transform.position;
		int num = 1;
		float num2 = 1f;
		if (this.m_UpdateMode == DynamicBone.UpdateMode.Default)
		{
			if (this.m_UpdateRate > 0f)
			{
				num2 = Time.deltaTime * this.m_UpdateRate;
			}
			else
			{
				num2 = Time.deltaTime;
			}
		}
		else if (this.m_UpdateRate > 0f)
		{
			float num3 = 1f / this.m_UpdateRate;
			this.m_Time += t;
			num = 0;
			while (this.m_Time >= num3)
			{
				this.m_Time -= num3;
				if (++num >= 3)
				{
					this.m_Time = 0f;
					break;
				}
			}
		}
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				this.UpdateParticles1(num2);
				this.UpdateParticles2(num2);
				this.m_ObjectMove = Vector3.zero;
			}
		}
		else
		{
			this.SkipUpdateParticles();
		}
		this.ApplyParticlesToTransforms();
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x00018198 File Offset: 0x00016398
	public void SetupParticles()
	{
		this.m_Particles.Clear();
		if (this.m_Root == null)
		{
			return;
		}
		this.m_LocalGravity = this.m_Root.InverseTransformDirection(this.m_Gravity);
		this.m_ObjectScale = Mathf.Abs(base.transform.lossyScale.x);
		this.m_ObjectPrevPosition = base.transform.position;
		this.m_ObjectMove = Vector3.zero;
		this.m_BoneTotalLength = 0f;
		this.AppendParticles(this.m_Root, -1, 0f);
		this.UpdateParameters();
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x00018230 File Offset: 0x00016430
	private void AppendParticles(Transform b, int parentIndex, float boneLength)
	{
		DynamicBone.Particle particle = new DynamicBone.Particle();
		particle.m_Transform = b;
		particle.m_ParentIndex = parentIndex;
		if (b != null)
		{
			particle.m_Position = (particle.m_PrevPosition = b.position);
			particle.m_InitLocalPosition = b.localPosition;
			particle.m_InitLocalRotation = b.localRotation;
		}
		else
		{
			Transform transform = this.m_Particles[parentIndex].m_Transform;
			if (this.m_EndLength > 0f)
			{
				Transform parent = transform.parent;
				if (parent != null)
				{
					particle.m_EndOffset = transform.InverseTransformPoint(transform.position * 2f - parent.position) * this.m_EndLength;
				}
				else
				{
					particle.m_EndOffset = new Vector3(this.m_EndLength, 0f, 0f);
				}
			}
			else
			{
				particle.m_EndOffset = transform.InverseTransformPoint(base.transform.TransformDirection(this.m_EndOffset) + transform.position);
			}
			particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
		}
		if (parentIndex >= 0)
		{
			boneLength += (this.m_Particles[parentIndex].m_Transform.position - particle.m_Position).magnitude;
			particle.m_BoneLength = boneLength;
			this.m_BoneTotalLength = Mathf.Max(this.m_BoneTotalLength, boneLength);
		}
		int count = this.m_Particles.Count;
		this.m_Particles.Add(particle);
		if (b != null)
		{
			for (int i = 0; i < b.childCount; i++)
			{
				Transform child = b.GetChild(i);
				bool flag = false;
				if (this.m_Exclusions != null)
				{
					flag = this.m_Exclusions.Contains(child);
				}
				if (!flag)
				{
					this.AppendParticles(child, count, boneLength);
				}
				else if (this.m_EndLength > 0f || this.m_EndOffset != Vector3.zero)
				{
					this.AppendParticles(null, count, boneLength);
				}
			}
			if (b.childCount == 0 && (this.m_EndLength > 0f || this.m_EndOffset != Vector3.zero))
			{
				this.AppendParticles(null, count, boneLength);
			}
		}
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00018464 File Offset: 0x00016664
	public void UpdateParameters()
	{
		if (this.m_Root == null)
		{
			return;
		}
		this.m_LocalGravity = this.m_Root.InverseTransformDirection(this.m_Gravity);
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			particle.m_Damping = this.m_Damping;
			particle.m_Elasticity = this.m_Elasticity;
			particle.m_Stiffness = this.m_Stiffness;
			particle.m_Inert = this.m_Inert;
			particle.m_Friction = this.m_Friction;
			particle.m_Radius = this.m_Radius;
			if (this.m_BoneTotalLength > 0f)
			{
				float num = particle.m_BoneLength / this.m_BoneTotalLength;
				if (this.m_DampingDistrib != null && this.m_DampingDistrib.keys.Length != 0)
				{
					particle.m_Damping *= this.m_DampingDistrib.Evaluate(num);
				}
				if (this.m_ElasticityDistrib != null && this.m_ElasticityDistrib.keys.Length != 0)
				{
					particle.m_Elasticity *= this.m_ElasticityDistrib.Evaluate(num);
				}
				if (this.m_StiffnessDistrib != null && this.m_StiffnessDistrib.keys.Length != 0)
				{
					particle.m_Stiffness *= this.m_StiffnessDistrib.Evaluate(num);
				}
				if (this.m_InertDistrib != null && this.m_InertDistrib.keys.Length != 0)
				{
					particle.m_Inert *= this.m_InertDistrib.Evaluate(num);
				}
				if (this.m_FrictionDistrib != null && this.m_FrictionDistrib.keys.Length != 0)
				{
					particle.m_Friction *= this.m_FrictionDistrib.Evaluate(num);
				}
				if (this.m_RadiusDistrib != null && this.m_RadiusDistrib.keys.Length != 0)
				{
					particle.m_Radius *= this.m_RadiusDistrib.Evaluate(num);
				}
			}
			particle.m_Damping = Mathf.Clamp01(particle.m_Damping);
			particle.m_Elasticity = Mathf.Clamp01(particle.m_Elasticity);
			particle.m_Stiffness = Mathf.Clamp01(particle.m_Stiffness);
			particle.m_Inert = Mathf.Clamp01(particle.m_Inert);
			particle.m_Friction = Mathf.Clamp01(particle.m_Friction);
			particle.m_Radius = Mathf.Max(particle.m_Radius, 0f);
		}
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x000186AC File Offset: 0x000168AC
	private void InitTransforms()
	{
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_Transform != null)
			{
				particle.m_Transform.localPosition = particle.m_InitLocalPosition;
				particle.m_Transform.localRotation = particle.m_InitLocalRotation;
			}
		}
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x0001870C File Offset: 0x0001690C
	private void ResetParticlesPosition()
	{
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_Transform != null)
			{
				particle.m_Position = (particle.m_PrevPosition = particle.m_Transform.position);
			}
			else
			{
				Transform transform = this.m_Particles[particle.m_ParentIndex].m_Transform;
				particle.m_Position = (particle.m_PrevPosition = transform.TransformPoint(particle.m_EndOffset));
			}
			particle.m_isCollide = false;
		}
		this.m_ObjectPrevPosition = base.transform.position;
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x000187B4 File Offset: 0x000169B4
	private void UpdateParticles1(float timeVar)
	{
		Vector3 vector = this.m_Gravity;
		Vector3 normalized = this.m_Gravity.normalized;
		Vector3 vector2 = this.m_Root.TransformDirection(this.m_LocalGravity);
		Vector3 vector3 = normalized * Mathf.Max(Vector3.Dot(vector2, normalized), 0f);
		vector -= vector3;
		vector = (vector + this.m_Force) * (this.m_ObjectScale * timeVar);
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_ParentIndex >= 0)
			{
				Vector3 vector4 = particle.m_Position - particle.m_PrevPosition;
				Vector3 vector5 = this.m_ObjectMove * particle.m_Inert;
				particle.m_PrevPosition = particle.m_Position + vector5;
				float num = particle.m_Damping;
				if (particle.m_isCollide)
				{
					num += particle.m_Friction;
					if (num > 1f)
					{
						num = 1f;
					}
					particle.m_isCollide = false;
				}
				particle.m_Position += vector4 * (1f - num) + vector + vector5;
			}
			else
			{
				particle.m_PrevPosition = particle.m_Position;
				particle.m_Position = particle.m_Transform.position;
			}
		}
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x00018924 File Offset: 0x00016B24
	private void UpdateParticles2(float timeVar)
	{
		Plane plane = default(Plane);
		for (int i = 1; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
			float num;
			if (particle.m_Transform != null)
			{
				num = (particle2.m_Transform.position - particle.m_Transform.position).magnitude;
			}
			else
			{
				num = particle2.m_Transform.localToWorldMatrix.MultiplyVector(particle.m_EndOffset).magnitude;
			}
			float num2 = Mathf.Lerp(1f, particle.m_Stiffness, this.m_Weight);
			if (num2 > 0f || particle.m_Elasticity > 0f)
			{
				Matrix4x4 localToWorldMatrix = particle2.m_Transform.localToWorldMatrix;
				localToWorldMatrix.SetColumn(3, particle2.m_Position);
				Vector3 vector;
				if (particle.m_Transform != null)
				{
					vector = localToWorldMatrix.MultiplyPoint3x4(particle.m_Transform.localPosition);
				}
				else
				{
					vector = localToWorldMatrix.MultiplyPoint3x4(particle.m_EndOffset);
				}
				Vector3 vector2 = vector - particle.m_Position;
				particle.m_Position += vector2 * (particle.m_Elasticity * timeVar);
				if (num2 > 0f)
				{
					vector2 = vector - particle.m_Position;
					float magnitude = vector2.magnitude;
					float num3 = num * (1f - num2) * 2f;
					if (magnitude > num3)
					{
						particle.m_Position += vector2 * ((magnitude - num3) / magnitude);
					}
				}
			}
			if (this.m_Colliders != null)
			{
				float num4 = particle.m_Radius * this.m_ObjectScale;
				for (int j = 0; j < this.m_Colliders.Count; j++)
				{
					DynamicBoneColliderBase dynamicBoneColliderBase = this.m_Colliders[j];
					if (dynamicBoneColliderBase != null && dynamicBoneColliderBase.enabled)
					{
						particle.m_isCollide |= dynamicBoneColliderBase.Collide(ref particle.m_Position, num4);
					}
				}
			}
			if (this.m_FreezeAxis != DynamicBone.FreezeAxis.None)
			{
				switch (this.m_FreezeAxis)
				{
				case DynamicBone.FreezeAxis.X:
					plane.SetNormalAndPosition(particle2.m_Transform.right, particle2.m_Position);
					break;
				case DynamicBone.FreezeAxis.Y:
					plane.SetNormalAndPosition(particle2.m_Transform.up, particle2.m_Position);
					break;
				case DynamicBone.FreezeAxis.Z:
					plane.SetNormalAndPosition(particle2.m_Transform.forward, particle2.m_Position);
					break;
				}
				particle.m_Position -= plane.normal * plane.GetDistanceToPoint(particle.m_Position);
			}
			Vector3 vector3 = particle2.m_Position - particle.m_Position;
			float magnitude2 = vector3.magnitude;
			if (magnitude2 > 0f)
			{
				particle.m_Position += vector3 * ((magnitude2 - num) / magnitude2);
			}
		}
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x00018C30 File Offset: 0x00016E30
	private void SkipUpdateParticles()
	{
		for (int i = 0; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			if (particle.m_ParentIndex >= 0)
			{
				particle.m_PrevPosition += this.m_ObjectMove;
				particle.m_Position += this.m_ObjectMove;
				DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
				float num;
				if (particle.m_Transform != null)
				{
					num = (particle2.m_Transform.position - particle.m_Transform.position).magnitude;
				}
				else
				{
					num = particle2.m_Transform.localToWorldMatrix.MultiplyVector(particle.m_EndOffset).magnitude;
				}
				float num2 = Mathf.Lerp(1f, particle.m_Stiffness, this.m_Weight);
				if (num2 > 0f)
				{
					Matrix4x4 localToWorldMatrix = particle2.m_Transform.localToWorldMatrix;
					localToWorldMatrix.SetColumn(3, particle2.m_Position);
					Vector3 vector;
					if (particle.m_Transform != null)
					{
						vector = localToWorldMatrix.MultiplyPoint3x4(particle.m_Transform.localPosition);
					}
					else
					{
						vector = localToWorldMatrix.MultiplyPoint3x4(particle.m_EndOffset);
					}
					Vector3 vector2 = vector - particle.m_Position;
					float magnitude = vector2.magnitude;
					float num3 = num * (1f - num2) * 2f;
					if (magnitude > num3)
					{
						particle.m_Position += vector2 * ((magnitude - num3) / magnitude);
					}
				}
				Vector3 vector3 = particle2.m_Position - particle.m_Position;
				float magnitude2 = vector3.magnitude;
				if (magnitude2 > 0f)
				{
					particle.m_Position += vector3 * ((magnitude2 - num) / magnitude2);
				}
			}
			else
			{
				particle.m_PrevPosition = particle.m_Position;
				particle.m_Position = particle.m_Transform.position;
			}
		}
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x00018E35 File Offset: 0x00017035
	private static Vector3 MirrorVector(Vector3 v, Vector3 axis)
	{
		return v - axis * (Vector3.Dot(v, axis) * 2f);
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x00018E50 File Offset: 0x00017050
	private void ApplyParticlesToTransforms()
	{
		for (int i = 1; i < this.m_Particles.Count; i++)
		{
			DynamicBone.Particle particle = this.m_Particles[i];
			DynamicBone.Particle particle2 = this.m_Particles[particle.m_ParentIndex];
			if (particle2.m_Transform.childCount <= 1)
			{
				Vector3 vector;
				if (particle.m_Transform != null)
				{
					vector = particle.m_Transform.localPosition;
				}
				else
				{
					vector = particle.m_EndOffset;
				}
				Vector3 vector2 = particle.m_Position - particle2.m_Position;
				Quaternion quaternion = Quaternion.FromToRotation(particle2.m_Transform.TransformDirection(vector), vector2);
				particle2.m_Transform.rotation = quaternion * particle2.m_Transform.rotation;
			}
			if (particle.m_Transform != null)
			{
				particle.m_Transform.position = particle.m_Position;
			}
		}
	}

	// Token: 0x04000474 RID: 1140
	[Tooltip("The root of the transform hierarchy to apply physics.")]
	public Transform m_Root;

	// Token: 0x04000475 RID: 1141
	[Tooltip("Internal physics simulation rate.")]
	public float m_UpdateRate = 60f;

	// Token: 0x04000476 RID: 1142
	public DynamicBone.UpdateMode m_UpdateMode = DynamicBone.UpdateMode.Default;

	// Token: 0x04000477 RID: 1143
	[Tooltip("How much the bones slowed down.")]
	[Range(0f, 1f)]
	public float m_Damping = 0.1f;

	// Token: 0x04000478 RID: 1144
	public AnimationCurve m_DampingDistrib;

	// Token: 0x04000479 RID: 1145
	[Tooltip("How much the force applied to return each bone to original orientation.")]
	[Range(0f, 1f)]
	public float m_Elasticity = 0.1f;

	// Token: 0x0400047A RID: 1146
	public AnimationCurve m_ElasticityDistrib;

	// Token: 0x0400047B RID: 1147
	[Tooltip("How much bone's original orientation are preserved.")]
	[Range(0f, 1f)]
	public float m_Stiffness = 0.1f;

	// Token: 0x0400047C RID: 1148
	public AnimationCurve m_StiffnessDistrib;

	// Token: 0x0400047D RID: 1149
	[Tooltip("How much character's position change is ignored in physics simulation.")]
	[Range(0f, 1f)]
	public float m_Inert;

	// Token: 0x0400047E RID: 1150
	public AnimationCurve m_InertDistrib;

	// Token: 0x0400047F RID: 1151
	[Tooltip("How much the bones slowed down when collide.")]
	public float m_Friction;

	// Token: 0x04000480 RID: 1152
	public AnimationCurve m_FrictionDistrib;

	// Token: 0x04000481 RID: 1153
	[Tooltip("Each bone can be a sphere to collide with colliders. Radius describe sphere's size.")]
	public float m_Radius;

	// Token: 0x04000482 RID: 1154
	public AnimationCurve m_RadiusDistrib;

	// Token: 0x04000483 RID: 1155
	[Tooltip("If End Length is not zero, an extra bone is generated at the end of transform hierarchy.")]
	public float m_EndLength;

	// Token: 0x04000484 RID: 1156
	[Tooltip("If End Offset is not zero, an extra bone is generated at the end of transform hierarchy.")]
	public Vector3 m_EndOffset = Vector3.zero;

	// Token: 0x04000485 RID: 1157
	[Tooltip("The force apply to bones. Partial force apply to character's initial pose is cancelled out.")]
	public Vector3 m_Gravity = Vector3.zero;

	// Token: 0x04000486 RID: 1158
	[Tooltip("The force apply to bones.")]
	public Vector3 m_Force = Vector3.zero;

	// Token: 0x04000487 RID: 1159
	[Tooltip("Collider objects interact with the bones.")]
	public List<DynamicBoneColliderBase> m_Colliders;

	// Token: 0x04000488 RID: 1160
	[Tooltip("Bones exclude from physics simulation.")]
	public List<Transform> m_Exclusions;

	// Token: 0x04000489 RID: 1161
	[Tooltip("Constrain bones to move on specified plane.")]
	public DynamicBone.FreezeAxis m_FreezeAxis;

	// Token: 0x0400048A RID: 1162
	[Tooltip("Disable physics simulation automatically if character is far from camera or player.")]
	public bool m_DistantDisable;

	// Token: 0x0400048B RID: 1163
	public Transform m_ReferenceObject;

	// Token: 0x0400048C RID: 1164
	public float m_DistanceToObject = 20f;

	// Token: 0x0400048D RID: 1165
	private Vector3 m_LocalGravity = Vector3.zero;

	// Token: 0x0400048E RID: 1166
	private Vector3 m_ObjectMove = Vector3.zero;

	// Token: 0x0400048F RID: 1167
	private Vector3 m_ObjectPrevPosition = Vector3.zero;

	// Token: 0x04000490 RID: 1168
	private float m_BoneTotalLength;

	// Token: 0x04000491 RID: 1169
	private float m_ObjectScale = 1f;

	// Token: 0x04000492 RID: 1170
	private float m_Time;

	// Token: 0x04000493 RID: 1171
	private float m_Weight = 1f;

	// Token: 0x04000494 RID: 1172
	private bool m_DistantDisabled;

	// Token: 0x04000495 RID: 1173
	private List<DynamicBone.Particle> m_Particles = new List<DynamicBone.Particle>();

	// Token: 0x02000304 RID: 772
	public enum UpdateMode
	{
		// Token: 0x04001109 RID: 4361
		Normal,
		// Token: 0x0400110A RID: 4362
		AnimatePhysics,
		// Token: 0x0400110B RID: 4363
		UnscaledTime,
		// Token: 0x0400110C RID: 4364
		Default
	}

	// Token: 0x02000305 RID: 773
	public enum FreezeAxis
	{
		// Token: 0x0400110E RID: 4366
		None,
		// Token: 0x0400110F RID: 4367
		X,
		// Token: 0x04001110 RID: 4368
		Y,
		// Token: 0x04001111 RID: 4369
		Z
	}

	// Token: 0x02000306 RID: 774
	private class Particle
	{
		// Token: 0x04001112 RID: 4370
		public Transform m_Transform;

		// Token: 0x04001113 RID: 4371
		public int m_ParentIndex = -1;

		// Token: 0x04001114 RID: 4372
		public float m_Damping;

		// Token: 0x04001115 RID: 4373
		public float m_Elasticity;

		// Token: 0x04001116 RID: 4374
		public float m_Stiffness;

		// Token: 0x04001117 RID: 4375
		public float m_Inert;

		// Token: 0x04001118 RID: 4376
		public float m_Friction;

		// Token: 0x04001119 RID: 4377
		public float m_Radius;

		// Token: 0x0400111A RID: 4378
		public float m_BoneLength;

		// Token: 0x0400111B RID: 4379
		public bool m_isCollide;

		// Token: 0x0400111C RID: 4380
		public Vector3 m_Position = Vector3.zero;

		// Token: 0x0400111D RID: 4381
		public Vector3 m_PrevPosition = Vector3.zero;

		// Token: 0x0400111E RID: 4382
		public Vector3 m_EndOffset = Vector3.zero;

		// Token: 0x0400111F RID: 4383
		public Vector3 m_InitLocalPosition = Vector3.zero;

		// Token: 0x04001120 RID: 4384
		public Quaternion m_InitLocalRotation = Quaternion.identity;
	}
}
