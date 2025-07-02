using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class RigCreator : MonoBehaviour
{
	// Token: 0x06000259 RID: 601 RVA: 0x00010913 File Offset: 0x0000EB13
	public void StartClear()
	{
		this.aboutToClear = true;
	}

	// Token: 0x0600025A RID: 602 RVA: 0x0001091C File Offset: 0x0000EB1C
	public void ClearNo()
	{
		this.ClearStates();
	}

	// Token: 0x0600025B RID: 603 RVA: 0x00010924 File Offset: 0x0000EB24
	public void ClearYes()
	{
		this.ClearStates();
		this.ClearDataAndRig();
	}

	// Token: 0x0600025C RID: 604 RVA: 0x00010932 File Offset: 0x0000EB32
	public void AutoGenerate()
	{
		this.FindParts();
		this.GenerateData();
	}

	// Token: 0x0600025D RID: 605 RVA: 0x00010940 File Offset: 0x0000EB40
	private void ClearStates()
	{
		this.aboutToClear = false;
	}

	// Token: 0x0600025E RID: 606 RVA: 0x0001094C File Offset: 0x0000EB4C
	private void GenerateData()
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].justCreated)
			{
				this.InitPart(this.parts[i]);
			}
			else
			{
				this.ApplyPartData(this.parts[i]);
			}
			this.parts[i].justCreated = false;
		}
	}

	// Token: 0x0600025F RID: 607 RVA: 0x000109BA File Offset: 0x0000EBBA
	private void InitPart(RigPart part)
	{
		this.AutoGenerateCollidersForPart(part);
		this.AddRigidbodyToPart(part);
		this.AddJointToPart(part);
		this.AddBodyPartScript(part);
	}

	// Token: 0x06000260 RID: 608 RVA: 0x000109D8 File Offset: 0x0000EBD8
	private void ApplyPartData(RigPart rigPart)
	{
		this.SyncCollidersFromData(rigPart);
		this.SyncRigidbodyFromData(rigPart);
		this.SyncJointFromData(rigPart);
		this.SyncBodypartScript(rigPart);
	}

	// Token: 0x06000261 RID: 609 RVA: 0x000109F6 File Offset: 0x0000EBF6
	private void SyncBodypartScript(RigPart rigPart)
	{
		if (!rigPart.transform.GetComponent<Bodypart>())
		{
			this.AddBodyPartScript(rigPart);
		}
	}

	// Token: 0x06000262 RID: 610 RVA: 0x00010A11 File Offset: 0x0000EC11
	private void SyncJointFromData(RigPart rigPart)
	{
		if (rigPart.joint == null)
		{
			this.AddJointToPart(rigPart);
		}
	}

	// Token: 0x06000263 RID: 611 RVA: 0x00010A28 File Offset: 0x0000EC28
	private void SyncRigidbodyFromData(RigPart rigPart)
	{
		if (rigPart.rig == null)
		{
			this.AddRigidbodyToPart(rigPart);
		}
	}

	// Token: 0x06000264 RID: 612 RVA: 0x00010A40 File Offset: 0x0000EC40
	private void AddRigidbodyToPart(RigPart rigPart)
	{
		Rigidbody rigidbody = rigPart.transform.gameObject.AddComponent<Rigidbody>();
		rigidbody.mass = rigPart.mass;
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		RigCreatorRigidbody rigCreatorRigidbody = rigPart.transform.gameObject.AddComponent<RigCreatorRigidbody>();
		rigCreatorRigidbody.mass = rigPart.mass;
		rigPart.rig = rigidbody;
		rigPart.rigHandler = rigCreatorRigidbody;
	}

	// Token: 0x06000265 RID: 613 RVA: 0x00010A9C File Offset: 0x0000EC9C
	private void SyncCollidersFromData(RigPart rigPart)
	{
		for (int i = 0; i < rigPart.colliders.Count; i++)
		{
			if (rigPart.colliders[i].colliderObject == null)
			{
				rigPart.colliders[i] = this.CreateColliderObject(rigPart.colliders[i].colliderPosition, rigPart.colliders[i].colliderRotation, rigPart.colliders[i].colliderScale, rigPart.transform, rigPart.colliders[i].height, rigPart.colliders[i].radius, false);
			}
		}
	}

	// Token: 0x06000266 RID: 614 RVA: 0x00010B4C File Offset: 0x0000ED4C
	private RigCreatorColliderData CreateColliderObject(Vector3 position, Quaternion rotation, Vector3 scale, Transform parent, float height, float radius, bool isWorldSpace = true)
	{
		GameObject gameObject = new GameObject("RigCollider");
		if (isWorldSpace)
		{
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
		}
		gameObject.transform.SetParent(parent);
		if (!isWorldSpace)
		{
			gameObject.transform.localPosition = position;
			gameObject.transform.localRotation = rotation;
			gameObject.transform.localScale = scale;
		}
		CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
		capsuleCollider.direction = 2;
		capsuleCollider.radius = radius;
		capsuleCollider.height = height;
		RigCreatorColliderData rigCreatorColliderData = new RigCreatorColliderData();
		rigCreatorColliderData.colliderPosition = capsuleCollider.transform.position;
		rigCreatorColliderData.colliderRotation = capsuleCollider.transform.rotation;
		rigCreatorColliderData.colliderScale = capsuleCollider.transform.localScale;
		rigCreatorColliderData.radius = capsuleCollider.radius;
		rigCreatorColliderData.height = capsuleCollider.height;
		RigCreatorCollider rigCreatorCollider = gameObject.AddComponent<RigCreatorCollider>();
		rigCreatorColliderData.colliderObject = rigCreatorCollider;
		return rigCreatorColliderData;
	}

	// Token: 0x06000267 RID: 615 RVA: 0x00010C31 File Offset: 0x0000EE31
	private void AddBodyPartScript(RigPart rigPart)
	{
		rigPart.transform.gameObject.AddComponent<Bodypart>().InitBodypart(rigPart.partType);
	}

	// Token: 0x06000268 RID: 616 RVA: 0x00010C50 File Offset: 0x0000EE50
	private void AddJointToPart(RigPart rigPart)
	{
		Rigidbody componentInParent = rigPart.transform.parent.GetComponentInParent<Rigidbody>();
		if (!componentInParent)
		{
			return;
		}
		ConfigurableJoint configurableJoint = this.SpawnJoint(rigPart.rig, componentInParent, rigPart.spring);
		rigPart.joint = configurableJoint;
		rigPart.jointHandler = rigPart.transform.gameObject.AddComponent<RigCreatorJoint>();
		rigPart.jointHandler.spring = rigPart.spring;
		rigPart.jointHandler.SetSpring(rigPart.spring);
	}

	// Token: 0x06000269 RID: 617 RVA: 0x00010CCC File Offset: 0x0000EECC
	internal ConfigurableJoint SpawnJoint(Rigidbody ownRig, Rigidbody otherRig, float spring)
	{
		ConfigurableJoint configurableJoint = ownRig.gameObject.AddComponent<ConfigurableJoint>();
		SoftJointLimit softJointLimit = configurableJoint.lowAngularXLimit;
		softJointLimit.limit = -177f;
		configurableJoint.lowAngularXLimit = softJointLimit;
		softJointLimit = configurableJoint.highAngularXLimit;
		softJointLimit.limit = 177f;
		configurableJoint.highAngularXLimit = softJointLimit;
		softJointLimit = configurableJoint.angularYLimit;
		softJointLimit.limit = 177f;
		configurableJoint.angularYLimit = softJointLimit;
		softJointLimit = configurableJoint.angularZLimit;
		softJointLimit.limit = 177f;
		configurableJoint.angularZLimit = softJointLimit;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Limited;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Limited;
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Locked;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		configurableJoint.projectionMode = JointProjectionMode.PositionAndRotation;
		configurableJoint.connectedBody = otherRig;
		return configurableJoint;
	}

	// Token: 0x0600026A RID: 618 RVA: 0x00010D84 File Offset: 0x0000EF84
	private void AutoGenerateCollidersForPart(RigPart rigPart)
	{
		Transform transform = null;
		float num = 0f;
		for (int i = rigPart.transform.childCount - 1; i >= 0; i--)
		{
			float num2 = Vector3.Distance(rigPart.transform.GetChild(i).position, rigPart.transform.position);
			if (num2 > num)
			{
				num = num2;
				transform = rigPart.transform.GetChild(i);
			}
		}
		Vector3 vector = Vector3.Lerp(rigPart.transform.position, transform.position, 0.5f);
		Quaternion quaternion = Quaternion.LookRotation(transform.position - rigPart.transform.position);
		float num3 = Vector3.Distance(transform.position, rigPart.transform.position);
		rigPart.colliders.Add(this.CreateColliderObject(vector, quaternion, Vector3.one, rigPart.transform, num3, 0.1f, true));
	}

	// Token: 0x0600026B RID: 619 RVA: 0x00010E68 File Offset: 0x0000F068
	private void FindParts()
	{
		for (int i = 0; i < 179; i++)
		{
			if (this.Contains((BodypartType)i))
			{
				BodypartType bodypartType = (BodypartType)i;
				Transform transform = HelperFunctions.FindChildRecursive(bodypartType.ToString(), base.transform);
				if (transform)
				{
					this.GetPartFromPartType((BodypartType)i).transform = transform;
				}
			}
			else
			{
				BodypartType bodypartType = (BodypartType)i;
				Transform transform2 = HelperFunctions.FindChildRecursive(bodypartType.ToString(), base.transform);
				if (transform2)
				{
					RigPart rigPart = new RigPart();
					rigPart.transform = transform2;
					rigPart.partType = (BodypartType)i;
					rigPart.justCreated = true;
					this.parts.Add(rigPart);
				}
			}
		}
	}

	// Token: 0x0600026C RID: 620 RVA: 0x00010F18 File Offset: 0x0000F118
	private RigPart GetPartFromPartType(BodypartType partType)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].partType == partType)
			{
				return this.parts[i];
			}
		}
		return null;
	}

	// Token: 0x0600026D RID: 621 RVA: 0x00010F60 File Offset: 0x0000F160
	private bool Contains(BodypartType targetType)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].partType == targetType)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600026E RID: 622 RVA: 0x00010F9C File Offset: 0x0000F19C
	private void ClearDataAndRig()
	{
		for (int i = this.parts.Count - 1; i >= 0; i--)
		{
			for (int j = this.parts[i].colliders.Count - 1; j >= 0; j--)
			{
				Object.DestroyImmediate(this.parts[i].colliders[j].colliderObject.gameObject);
			}
			this.parts[i].colliders.Clear();
			Bodypart component = this.parts[i].transform.GetComponent<Bodypart>();
			if (component)
			{
				Object.DestroyImmediate(component);
			}
			Object.DestroyImmediate(this.parts[i].joint);
			if (this.parts[i].jointHandler)
			{
				Object.DestroyImmediate(this.parts[i].jointHandler);
			}
			Object.DestroyImmediate(this.parts[i].rig);
			Object.DestroyImmediate(this.parts[i].rigHandler);
		}
		this.parts.Clear();
	}

	// Token: 0x0600026F RID: 623 RVA: 0x000110C8 File Offset: 0x0000F2C8
	private RigPart GetPartFromJointObject(RigCreatorJoint jointObject)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].jointHandler == jointObject)
			{
				return this.parts[i];
			}
		}
		return null;
	}

	// Token: 0x06000270 RID: 624 RVA: 0x00011114 File Offset: 0x0000F314
	private RigPart GetPartFromRigObject(RigCreatorRigidbody rigObject)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].rigHandler == rigObject)
			{
				return this.parts[i];
			}
		}
		return null;
	}

	// Token: 0x06000271 RID: 625 RVA: 0x00011160 File Offset: 0x0000F360
	private RigPart GetPartFromColliderObject(RigCreatorCollider colliderObject)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			for (int j = this.parts[i].colliders.Count - 1; j >= 0; j--)
			{
				if (this.parts[i].colliders[j].colliderObject == colliderObject)
				{
					return this.parts[i];
				}
			}
		}
		return null;
	}

	// Token: 0x06000272 RID: 626 RVA: 0x000111D8 File Offset: 0x0000F3D8
	private RigCreatorColliderData GetColliderDataFromColliderObject(RigCreatorCollider colliderObject)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			for (int j = this.parts[i].colliders.Count - 1; j >= 0; j--)
			{
				if (this.parts[i].colliders[j].colliderObject == colliderObject)
				{
					return this.parts[i].colliders[j];
				}
			}
		}
		return null;
	}

	// Token: 0x06000273 RID: 627 RVA: 0x0001125C File Offset: 0x0000F45C
	internal void RemoveCollider(RigCreatorCollider rigCreatorCollider)
	{
		RigCreatorColliderData colliderDataFromColliderObject = this.GetColliderDataFromColliderObject(rigCreatorCollider);
		if (colliderDataFromColliderObject != null)
		{
			RigPart partFromColliderObject = this.GetPartFromColliderObject(rigCreatorCollider);
			if (partFromColliderObject != null)
			{
				partFromColliderObject.colliders.Remove(colliderDataFromColliderObject);
			}
		}
	}

	// Token: 0x06000274 RID: 628 RVA: 0x0001128C File Offset: 0x0000F48C
	internal void ColliderChanged(RigCreatorCollider rigCreatorCollider, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, float height, float radius)
	{
		RigCreatorColliderData rigCreatorColliderData = this.GetColliderDataFromColliderObject(rigCreatorCollider);
		if (rigCreatorColliderData == null)
		{
			RigPart rigPart = this.GetPartFromColliderObject(rigCreatorCollider);
			if (rigPart == null)
			{
				rigPart = this.FindPartFromName(rigCreatorCollider.transform.parent.name);
			}
			if (rigPart == null)
			{
				return;
			}
			rigCreatorColliderData = new RigCreatorColliderData();
			rigCreatorColliderData.colliderObject = rigCreatorCollider;
			rigPart.colliders.Add(rigCreatorColliderData);
		}
		rigCreatorColliderData.colliderPosition = localPosition;
		rigCreatorColliderData.colliderRotation = localRotation;
		rigCreatorColliderData.colliderScale = localScale;
		rigCreatorColliderData.height = height;
		rigCreatorColliderData.radius = radius;
	}

	// Token: 0x06000275 RID: 629 RVA: 0x00011309 File Offset: 0x0000F509
	internal void RigidbodyChanged(RigCreatorRigidbody rigObject, float mass)
	{
		this.GetPartFromRigObject(rigObject).mass = mass;
	}

	// Token: 0x06000276 RID: 630 RVA: 0x00011318 File Offset: 0x0000F518
	internal void JointChanged(RigCreatorJoint jointObject, float spring)
	{
		this.GetPartFromJointObject(jointObject).spring = spring;
	}

	// Token: 0x06000277 RID: 631 RVA: 0x00011328 File Offset: 0x0000F528
	private RigPart FindPartFromName(string targetName)
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			if (this.parts[i].partType.ToString() == targetName)
			{
				return this.parts[i];
			}
		}
		return null;
	}

	// Token: 0x04000237 RID: 567
	[HideInInspector]
	public bool aboutToClear;

	// Token: 0x04000238 RID: 568
	public float springMultiplier = 1f;

	// Token: 0x04000239 RID: 569
	public List<RigPart> parts;
}
