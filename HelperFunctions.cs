using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000099 RID: 153
public class HelperFunctions : MonoBehaviour
{
	// Token: 0x06000577 RID: 1399 RVA: 0x0001F0B4 File Offset: 0x0001D2B4
	internal static Terrain GetTerrain(Vector3 center)
	{
		RaycastHit raycastHit = HelperFunctions.LineCheck(center + Vector3.up * 1000f, center - Vector3.up * 1000f, HelperFunctions.LayerType.Terrain, 0f, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			return raycastHit.transform.GetComponent<Terrain>();
		}
		return null;
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0001F114 File Offset: 0x0001D314
	public static LayerMask GetMask(HelperFunctions.LayerType layerType)
	{
		if (layerType == HelperFunctions.LayerType.AllPhysical)
		{
			return HelperFunctions.AllPhysical;
		}
		if (layerType == HelperFunctions.LayerType.TerrainMap)
		{
			return HelperFunctions.terrainMapMask;
		}
		if (layerType == HelperFunctions.LayerType.Terrain)
		{
			return HelperFunctions.terrainMask;
		}
		if (layerType == HelperFunctions.LayerType.Default)
		{
			return HelperFunctions.DefaultMask;
		}
		if (layerType == HelperFunctions.LayerType.AllPhysicalExceptCharacter)
		{
			return HelperFunctions.AllPhysicalExceptCharacter;
		}
		return HelperFunctions.MapMask;
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x0001F150 File Offset: 0x0001D350
	public static Vector3 GetGroundPos(Vector3 from, HelperFunctions.LayerType layerType, float radius = 0f)
	{
		Vector3 vector = from;
		RaycastHit raycastHit = HelperFunctions.LineCheck(from, from + Vector3.down * 10000f, layerType, radius, QueryTriggerInteraction.Ignore);
		if (raycastHit.transform)
		{
			vector = raycastHit.point;
		}
		return vector;
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x0001F195 File Offset: 0x0001D395
	public static RaycastHit GetGroundPosRaycast(Vector3 from, HelperFunctions.LayerType layerType, float radius = 0f)
	{
		return HelperFunctions.LineCheck(from, from + Vector3.down * 10000f, layerType, radius, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x0001F1B8 File Offset: 0x0001D3B8
	internal static GameObject InstantiatePrefab(GameObject sourceObj, Transform parent)
	{
		GameObject gameObject = null;
		if (!Application.isEditor)
		{
			gameObject = Object.Instantiate<GameObject>(sourceObj, parent);
		}
		return gameObject;
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x0001F1D7 File Offset: 0x0001D3D7
	public static RaycastHit GetGroundPosRaycast(Vector3 from, HelperFunctions.LayerType layerType, Vector3 gravityDir, float radius = 0f)
	{
		return HelperFunctions.LineCheck(from, from + gravityDir * 10000f, layerType, radius, QueryTriggerInteraction.Ignore);
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x0001F1F4 File Offset: 0x0001D3F4
	public static RaycastHit LineCheck(Vector3 from, Vector3 to, HelperFunctions.LayerType layerType, float radius = 0f, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		RaycastHit raycastHit = default(RaycastHit);
		Ray ray = new Ray(from, to - from);
		if (radius == 0f)
		{
			Physics.Raycast(ray, out raycastHit, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType));
		}
		else
		{
			Physics.SphereCast(ray, radius, out raycastHit, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType));
		}
		return raycastHit;
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x0001F25C File Offset: 0x0001D45C
	public static RaycastHit[] LineCheckAll(Vector3 from, Vector3 to, HelperFunctions.LayerType layerType, float radius = 0f, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
	{
		RaycastHit[] array;
		if (radius == 0f)
		{
			array = Physics.RaycastAll(from, to - from, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType), triggerInteraction);
		}
		else
		{
			array = Physics.SphereCastAll(from, radius, to - from, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType), triggerInteraction);
		}
		return array;
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x0001F2BC File Offset: 0x0001D4BC
	public static RaycastHit LineCheckIgnoreItem(Vector3 from, Vector3 to, HelperFunctions.LayerType layerType, Item ignoreItem)
	{
		RaycastHit raycastHit = default(RaycastHit);
		foreach (RaycastHit raycastHit2 in Physics.RaycastAll(from, to - from, Vector3.Distance(from, to), HelperFunctions.GetMask(layerType)))
		{
			Item componentInParent = raycastHit2.collider.GetComponentInParent<Item>();
			if ((!componentInParent || !(componentInParent == ignoreItem)) && (raycastHit.collider == null || raycastHit.distance > raycastHit2.distance))
			{
				raycastHit = raycastHit2;
			}
		}
		return raycastHit;
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x0001F34C File Offset: 0x0001D54C
	internal static ConfigurableJoint AttachPositionJoint(Rigidbody rig1, Rigidbody rig2, bool useCustomConnection = false, Vector3 customConnectionPoint = default(Vector3))
	{
		ConfigurableJoint configurableJoint = rig1.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.xMotion = ConfigurableJointMotion.Locked;
		configurableJoint.yMotion = ConfigurableJointMotion.Locked;
		configurableJoint.zMotion = ConfigurableJointMotion.Locked;
		configurableJoint.projectionMode = JointProjectionMode.PositionAndRotation;
		configurableJoint.anchor = ((!useCustomConnection) ? rig1.transform.InverseTransformPoint(rig2.position) : rig1.transform.InverseTransformPoint(customConnectionPoint));
		configurableJoint.enableCollision = false;
		configurableJoint.connectedBody = rig2;
		return configurableJoint;
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x0001F3B6 File Offset: 0x0001D5B6
	internal static Joint AttachFixedJoint(Rigidbody rig1, Rigidbody rig2)
	{
		FixedJoint fixedJoint = rig1.gameObject.AddComponent<FixedJoint>();
		fixedJoint.enableCollision = false;
		fixedJoint.connectedBody = rig2;
		return fixedJoint;
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x0001F3D4 File Offset: 0x0001D5D4
	internal static Vector3 RandomOnFlatCircle()
	{
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		return new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x0001F400 File Offset: 0x0001D600
	internal static void DestroyAll(Object[] objects)
	{
		for (int i = objects.Length - 1; i >= 0; i--)
		{
			Object.Destroy(objects[i]);
		}
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x0001F425 File Offset: 0x0001D625
	internal static Vector3 EulerToLook(Vector2 euler)
	{
		return new Vector3(euler.y, -euler.x, 0f);
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x0001F43E File Offset: 0x0001D63E
	internal static Vector3 LookToEuler(Vector2 lookRotationValues)
	{
		return new Vector3(-lookRotationValues.y, lookRotationValues.x, 0f);
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x0001F457 File Offset: 0x0001D657
	internal static Vector3 LookToDirection(Vector3 look, Vector3 targetDir)
	{
		return HelperFunctions.EulerToDirection(HelperFunctions.LookToEuler(look), targetDir);
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x0001F46A File Offset: 0x0001D66A
	internal static Vector3 EulerToDirection(Vector3 euler, Vector3 targetDir)
	{
		return Quaternion.Euler(euler) * targetDir;
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x0001F478 File Offset: 0x0001D678
	internal static Vector3 DirectionToEuler(Vector3 dir)
	{
		return Quaternion.LookRotation(dir, Vector3.up).eulerAngles;
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x0001F498 File Offset: 0x0001D698
	internal static Vector3 DirectionToLook(Vector3 dir)
	{
		Vector3 vector = HelperFunctions.DirectionToEuler(dir);
		while (vector.x > 180f)
		{
			vector.x -= 360f;
		}
		return HelperFunctions.EulerToLook(vector);
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x0001F4D6 File Offset: 0x0001D6D6
	internal static Vector3 GroundDirection(Vector3 planeNormal, Vector3 sideDirection)
	{
		return -Vector3.Cross(sideDirection, planeNormal);
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x0001F4E4 File Offset: 0x0001D6E4
	internal static Vector3 SeparateClamps(Vector3 rotationError, float clamp)
	{
		rotationError.x = Mathf.Clamp(rotationError.x, -clamp, clamp);
		rotationError.y = Mathf.Clamp(rotationError.y, -clamp, clamp);
		rotationError.z = Mathf.Clamp(rotationError.z, -clamp, clamp);
		return rotationError;
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x0001F531 File Offset: 0x0001D731
	internal static float FlatDistance(Vector3 from, Vector3 to)
	{
		return Vector2.Distance(from.XZ(), to.XZ());
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x0001F544 File Offset: 0x0001D744
	internal static void IgnoreConnect(Rigidbody rig1, Rigidbody rig2)
	{
		rig1.gameObject.AddComponent<ConfigurableJoint>().connectedBody = rig2;
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x0001F557 File Offset: 0x0001D757
	internal static RaycastHit[] SortRaycastResults(RaycastHit[] hitsToSort)
	{
		hitsToSort.Sort(new Comparison<RaycastHit>(HelperFunctions.RaycastHitComparer));
		return hitsToSort;
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x0001F56C File Offset: 0x0001D76C
	public static Vector3[] GetCircularDirections(int count)
	{
		Vector3[] array = new Vector3[count];
		float num = 360f / (float)count;
		for (int i = 0; i < count; i++)
		{
			float num2 = (float)i * num;
			float num3 = 0.017453292f * num2;
			array[i] = new Vector3(Mathf.Cos(num3), 0f, Mathf.Sin(num3)).normalized;
		}
		return array;
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x0001F5CC File Offset: 0x0001D7CC
	private static int RaycastHitComparer(RaycastHit x, RaycastHit y)
	{
		if (x.distance < y.distance)
		{
			return -1;
		}
		return 1;
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x0001F5E4 File Offset: 0x0001D7E4
	internal static Quaternion GetRandomRotationWithUp(Vector3 normal)
	{
		Vector3 vector = Random.onUnitSphere;
		vector.y = 0f;
		vector = Vector3.Cross(normal, Vector3.Cross(normal, vector));
		return Quaternion.LookRotation(vector, normal);
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x0001F618 File Offset: 0x0001D818
	public static Bounds GetTotalBounds(GameObject gameObject)
	{
		return HelperFunctions.GetTotalBounds(gameObject.GetComponentsInChildren<MeshRenderer>());
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x0001F628 File Offset: 0x0001D828
	internal static Vector3 GetCenterOfMass(Transform transform)
	{
		Vector3 vector = Vector3.zero;
		float num = 0f;
		for (int i = 0; i < transform.childCount; i++)
		{
			Collider component = transform.GetChild(i).GetComponent<Collider>();
			if (component)
			{
				vector += component.transform.position;
				num += 1f;
			}
		}
		vector /= num;
		return transform.InverseTransformPoint(vector);
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x0001F690 File Offset: 0x0001D890
	public static Bounds GetTotalBounds(IEnumerable<Renderer> rends)
	{
		Bounds bounds = default(Bounds);
		bool flag = true;
		foreach (Renderer renderer in rends)
		{
			if (flag)
			{
				bounds = renderer.bounds;
				flag = false;
			}
			else
			{
				bounds.Encapsulate(renderer.bounds);
			}
		}
		return bounds;
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x0001F6F8 File Offset: 0x0001D8F8
	public static List<Tout> GetComponentListFromComponentArray<Tin, Tout>(IEnumerable<Tin> inComponents) where Tin : Component where Tout : Component
	{
		List<Tout> list = new List<Tout>();
		foreach (Tin tin in inComponents)
		{
			Tout component = tin.GetComponent<Tout>();
			if (component)
			{
				list.Add(component);
			}
		}
		return list;
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x0001F760 File Offset: 0x0001D960
	internal static IEnumerable<T> SortBySiblingIndex<T>(IEnumerable<T> componentsToSort) where T : Component
	{
		List<T> list = new List<T>();
		list.AddRange(componentsToSort);
		list.Sort((T p1, T p2) => p1.transform.GetSiblingIndex().CompareTo(p2.transform.GetSiblingIndex()));
		return list;
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x0001F793 File Offset: 0x0001D993
	internal static float FlatAngle(Vector3 dir1, Vector3 dir2)
	{
		return Vector3.Angle(dir1.Flat(), dir2.Flat());
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x0001F7A8 File Offset: 0x0001D9A8
	internal static void SetChildCollidersLayer(Transform root, int layerID)
	{
		Collider[] componentsInChildren = root.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = layerID;
		}
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x0001F7D8 File Offset: 0x0001D9D8
	internal static void SetJointDrive(ConfigurableJoint joint, float spring, float damper, Rigidbody rig)
	{
		JointDrive angularXDrive = joint.angularXDrive;
		angularXDrive.positionSpring = spring * rig.mass;
		angularXDrive.positionDamper = damper * rig.mass;
		joint.angularXDrive = angularXDrive;
		joint.angularYZDrive = angularXDrive;
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x0001F818 File Offset: 0x0001DA18
	internal static Transform FindChildRecursive(string targetName, Transform root)
	{
		if (root.gameObject.name.ToUpper() == targetName.ToUpper())
		{
			return root;
		}
		for (int i = 0; i < root.childCount; i++)
		{
			Transform transform = HelperFunctions.FindChildRecursive(targetName, root.GetChild(i));
			if (!(transform == null) && transform.gameObject.name.ToUpper() == targetName.ToUpper())
			{
				return transform;
			}
		}
		return null;
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x0001F88C File Offset: 0x0001DA8C
	internal static void PhysicsRotateTowards(Rigidbody rig, Vector3 from, Vector3 to, float force)
	{
		Vector3 vector = Vector3.Cross(from, to).normalized * Vector3.Angle(from, to);
		rig.AddTorque(vector * force, ForceMode.Acceleration);
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x0001F8C3 File Offset: 0x0001DAC3
	internal static Vector3 MultiplyVectors(Vector3 v1, Vector3 v2)
	{
		v1.x *= v2.x;
		v1.y *= v2.y;
		v1.z *= v2.z;
		return v1;
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x0001F8F9 File Offset: 0x0001DAF9
	public static Vector3 CubicBezier(Vector3 Start, Vector3 _P1, Vector3 _P2, Vector3 end, float _t)
	{
		return (1f - _t) * HelperFunctions.QuadraticBezier(Start, _P1, _P2, _t) + _t * HelperFunctions.QuadraticBezier(_P1, _P2, end, _t);
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x0001F928 File Offset: 0x0001DB28
	public static Vector3 QuadraticBezier(Vector3 start, Vector3 _P1, Vector3 end, float _t)
	{
		return (1f - _t) * HelperFunctions.LinearBezier(start, _P1, _t) + _t * HelperFunctions.LinearBezier(_P1, end, _t);
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x0001F951 File Offset: 0x0001DB51
	public static Vector3 LinearBezier(Vector3 start, Vector3 end, float _t)
	{
		return (1f - _t) * start + _t * end;
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x0001F96C File Offset: 0x0001DB6C
	internal static Vector3 GetRandomPositionInBounds(Bounds bounds)
	{
		return new Vector3(Mathf.Lerp(bounds.min.x, bounds.max.x, Random.value), Mathf.Lerp(bounds.min.y, bounds.max.y, Random.value), Mathf.Lerp(bounds.min.z, bounds.max.z, Random.value));
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x0001F9E4 File Offset: 0x0001DBE4
	internal static GameObject SpawnPrefab(GameObject gameObject, Vector3 position, Quaternion rotation, Transform transform)
	{
		GameObject gameObject2 = null;
		if (!Application.isEditor)
		{
			gameObject2 = Object.Instantiate<GameObject>(gameObject);
		}
		gameObject2.transform.SetParent(transform);
		gameObject2.transform.rotation = rotation;
		gameObject2.transform.position = position;
		return gameObject2;
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x0001FA26 File Offset: 0x0001DC26
	internal static Quaternion GetRotationWithUp(Vector3 forward, Vector3 up)
	{
		return Quaternion.LookRotation(Vector3.ProjectOnPlane(forward, up), up);
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x0001FA38 File Offset: 0x0001DC38
	internal static float BoxDistance(Vector3 pos1, Vector3 pos2)
	{
		return Mathf.Max(Mathf.Max(Mathf.Max(0f, Mathf.Abs(pos1.x - pos2.x)), Mathf.Abs(pos1.y - pos2.y)), Mathf.Abs(pos1.z - pos2.z));
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x0001FA90 File Offset: 0x0001DC90
	internal static bool CanSee(Transform looker, Vector3 pos, float maxAngle = 70f)
	{
		return Vector3.Angle(looker.forward, pos - looker.position) <= maxAngle && !HelperFunctions.LineCheck(looker.transform.position, pos, HelperFunctions.LayerType.TerrainMap, 0f, QueryTriggerInteraction.Ignore).transform;
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x0001FAE4 File Offset: 0x0001DCE4
	internal static bool InBoxRange(Vector3 position1, Vector3 position2, int range)
	{
		return Mathf.Abs(position1.x - position2.x) <= (float)range && Mathf.Abs(position1.y - position2.y) <= (float)range && Mathf.Abs(position1.z - position2.z) <= (float)range;
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x0001FB3C File Offset: 0x0001DD3C
	internal static Random.State SetRandomSeedFromWorldPos(Vector3 position, int seed)
	{
		position.x = (float)Mathf.RoundToInt(position.x);
		position.y = (float)Mathf.RoundToInt(position.y);
		position.z = (float)Mathf.RoundToInt(position.z);
		Random.State state = Random.state;
		Debug.Log("Set Seed");
		Random.InitState(Mathf.RoundToInt((float)seed + position.x + position.y * 100f + position.z * 10000f));
		return state;
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x0001FBC0 File Offset: 0x0001DDC0
	public static List<Transform> FindAllChildrenWithTag(string targetTag, Transform target)
	{
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < target.childCount; i++)
		{
			Transform child = target.GetChild(i);
			if (child.name.Contains(targetTag))
			{
				list.Add(child);
			}
			list.AddRange(HelperFunctions.FindAllChildrenWithTag(targetTag, child));
		}
		return list;
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x0001FC10 File Offset: 0x0001DE10
	internal static T[] GridToFlatArray<T>(T[,] grid)
	{
		T[] array = new T[grid.GetLength(0) * grid.GetLength(1)];
		int length = grid.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length; j++)
			{
				int num = i * length + j;
				array[num] = grid[j, i];
			}
		}
		return array;
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x0001FC6C File Offset: 0x0001DE6C
	internal static NativeArray<float> FloatGridToNativeArray(float[,] floats)
	{
		NativeArray<float> nativeArray = new NativeArray<float>(floats.GetLength(0) * floats.GetLength(1), Allocator.TempJob, NativeArrayOptions.ClearMemory);
		int length = floats.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length; j++)
			{
				int num = i * length + j;
				nativeArray[num] = floats[i, j];
			}
		}
		return nativeArray;
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x0001FCCC File Offset: 0x0001DECC
	internal static float[,] NativeArrayToFloatGrid(NativeArray<float> array, int arrayLength)
	{
		float[,] array2 = new float[arrayLength, arrayLength];
		int length = array.Length;
		for (int i = 0; i < length; i++)
		{
			int num = Mathf.FloorToInt((float)(i / arrayLength));
			int num2 = i - num * arrayLength;
			array2[num, num2] = array[i];
		}
		return array2;
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x0001FD18 File Offset: 0x0001DF18
	public static Vector2Int GetIndex_FlatToGrid(int flatIndex, int arrayLength)
	{
		int num = Mathf.FloorToInt((float)(flatIndex / arrayLength));
		int num2 = flatIndex - num * arrayLength;
		return new Vector2Int(num, num2);
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x0001FD3C File Offset: 0x0001DF3C
	public static int GetIndex_GridToFlat(Vector2Int gridIndex, int arrayLength)
	{
		return gridIndex.x * arrayLength + gridIndex.y;
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x0001FD50 File Offset: 0x0001DF50
	internal static List<Vector2Int> GetIndexesInBounds(int xRess, int yRess, Bounds selectionBounds, Bounds totalBounds)
	{
		int num = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.x, totalBounds.max.x, selectionBounds.min.x) * (float)xRess);
		int num2 = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.x, totalBounds.max.x, selectionBounds.max.x) * (float)xRess);
		int num3 = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.z, totalBounds.max.z, selectionBounds.min.z) * (float)xRess);
		int num4 = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.z, totalBounds.max.z, selectionBounds.max.z) * (float)yRess);
		List<Vector2Int> list = new List<Vector2Int>();
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				list.Add(new Vector2Int(i, j));
				HelperFunctions.IDToWorldPos(i, j, xRess, yRess, totalBounds);
			}
		}
		return list;
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x0001FE68 File Offset: 0x0001E068
	public static Vector3 IDToWorldPos(int x, int y, int xRess, int yRess, Bounds totalBounds)
	{
		float num = (float)x / ((float)xRess - 1f);
		float num2 = (float)y / ((float)yRess - 1f);
		return new Vector3(Mathf.Lerp(totalBounds.min.x, totalBounds.max.x, num), 0f, Mathf.Lerp(totalBounds.min.z, totalBounds.max.z, num2));
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x0001FED4 File Offset: 0x0001E0D4
	internal static Vector3 GetRadomPointInBounds(Bounds b)
	{
		Vector3 min = b.min;
		Vector3 max = b.max;
		return new Vector3(Mathf.Lerp(min.x, max.x, Random.value), Mathf.Lerp(min.y, max.y, Random.value), Mathf.Lerp(min.z, max.z, Random.value));
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x0001FF38 File Offset: 0x0001E138
	internal static Camera GetMainCamera()
	{
		if (MainCamera.instance == null)
		{
			MainCamera.instance = Object.FindAnyObjectByType<MainCamera>();
			MainCamera.instance.cam = MainCamera.instance.GetComponent<Camera>();
		}
		return MainCamera.instance.cam;
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x0001FF70 File Offset: 0x0001E170
	internal static Color GetVertexColorAtPoint(Vector3[] verts, Color[] colors, Transform transform, Vector3 point)
	{
		if (colors.Length == 0)
		{
			return Color.black;
		}
		Color color = Color.black;
		float num = 10000000f;
		for (int i = 0; i < verts.Length; i++)
		{
			Vector3 vector = transform.TransformPoint(verts[i]);
			float num2 = Vector3.Distance(point, vector);
			if (num2 < num)
			{
				num = num2;
				color = colors[i];
			}
		}
		return color;
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x0001FFC9 File Offset: 0x0001E1C9
	internal static float GetValue(Color color)
	{
		return Mathf.Max(new float[] { color.r, color.g, color.b });
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x0001FFF4 File Offset: 0x0001E1F4
	public static T RandomSelection<T>(List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			return default(T);
		}
		return list[Random.Range(0, list.Count)];
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x00020028 File Offset: 0x0001E228
	public static bool IsLayerInLayerMask(LayerMask layerMask, int layer)
	{
		return (layerMask.value & (1 << layer)) != 0;
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x0002003B File Offset: 0x0001E23B
	public static bool IsLayerInLayerMask(HelperFunctions.LayerType layerType, int layer)
	{
		return HelperFunctions.IsLayerInLayerMask(HelperFunctions.GetMask(layerType), layer);
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x00020049 File Offset: 0x0001E249
	public static Vector3 ZeroY(Vector3 original)
	{
		return new Vector3(original.x, 0f, original.z);
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x00020064 File Offset: 0x0001E264
	internal static bool AnyPlayerInZRange(float min, float max)
	{
		foreach (Character character in Character.AllCharacters)
		{
			if (!character.isBot && character.Center.z >= min && character.Center.z <= max)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040005BD RID: 1469
	public static LayerMask AllPhysical = LayerMask.GetMask(new string[] { "Terrain", "Map", "Default", "Character", "Rope" });

	// Token: 0x040005BE RID: 1470
	public static LayerMask AllPhysicalExceptCharacter = LayerMask.GetMask(new string[] { "Terrain", "Map", "Default", "Rope" });

	// Token: 0x040005BF RID: 1471
	public static LayerMask terrainMapMask = LayerMask.GetMask(new string[] { "Terrain", "Map" });

	// Token: 0x040005C0 RID: 1472
	public static LayerMask terrainMask = LayerMask.GetMask(new string[] { "Terrain" });

	// Token: 0x040005C1 RID: 1473
	public static LayerMask MapMask = LayerMask.GetMask(new string[] { "Map" });

	// Token: 0x040005C2 RID: 1474
	public static LayerMask DefaultMask = LayerMask.GetMask(new string[] { "Default" });

	// Token: 0x02000320 RID: 800
	public enum LayerType
	{
		// Token: 0x0400116A RID: 4458
		AllPhysical,
		// Token: 0x0400116B RID: 4459
		TerrainMap,
		// Token: 0x0400116C RID: 4460
		Terrain,
		// Token: 0x0400116D RID: 4461
		Map,
		// Token: 0x0400116E RID: 4462
		Default,
		// Token: 0x0400116F RID: 4463
		AllPhysicalExceptCharacter
	}
}
