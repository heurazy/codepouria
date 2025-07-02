using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using pworld.Scripts.Extensions;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000113 RID: 275
[DefaultExecutionOrder(100000)]
public class Rope : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
	// Token: 0x17000068 RID: 104
	// (get) Token: 0x060007FB RID: 2043 RVA: 0x0002A572 File Offset: 0x00028772
	// (set) Token: 0x060007FC RID: 2044 RVA: 0x0002A57A File Offset: 0x0002877A
	public float Segments
	{
		get
		{
			return this.segments;
		}
		set
		{
			this.segments = Mathf.Clamp(value, 0f, (float)Rope.MaxSegments);
		}
	}

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x060007FD RID: 2045 RVA: 0x0002A593 File Offset: 0x00028793
	public static int MaxSegments
	{
		get
		{
			return 40;
		}
	}

	// Token: 0x1700006A RID: 106
	// (get) Token: 0x060007FE RID: 2046 RVA: 0x0002A597 File Offset: 0x00028797
	public int SegmentCount
	{
		get
		{
			if (base.photonView.IsMine)
			{
				return this.simulationSegments.Count;
			}
			return this.remoteColliderSegments.Count;
		}
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0002A5BD File Offset: 0x000287BD
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.IsMasterClient)
		{
			this.view.RPC("OnRejoinSyncRPC", newPlayer, new object[] { this.attachmenState });
		}
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0002A5F2 File Offset: 0x000287F2
	[PunRPC]
	public void OnRejoinSyncRPC(Rope.ATTACHMENT attachmentState)
	{
		this.attachmenState = attachmentState;
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0002A5FB File Offset: 0x000287FB
	private void Awake()
	{
		this.itemSpool = base.GetComponentInParent<Item>();
		this.climbingAPI = base.GetComponent<RopeClimbingAPI>();
		this.view = base.GetComponent<PhotonView>();
		this.ropeBoneVisualizer = base.GetComponentInChildren<RopeBoneVisualizer>();
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x0002A630 File Offset: 0x00028830
	private void Update()
	{
		bool flag;
		switch (this.attachmenState)
		{
		case Rope.ATTACHMENT.unattached:
			flag = false;
			break;
		case Rope.ATTACHMENT.inSpool:
			flag = false;
			break;
		case Rope.ATTACHMENT.anchored:
			flag = true;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		this.isClimbable = flag;
		if (!base.photonView.IsMine || this.creatorLeft)
		{
			if (this.simulationSegments.Count > 0)
			{
				this.Clear(false);
			}
			return;
		}
		this.timeSinceRemoved += Time.deltaTime;
		int num = Mathf.Clamp(Mathf.FloorToInt(this.Segments), 1, int.MaxValue);
		if (this.simulationSegments.Count > num)
		{
			if (this.simulationSegments.Count > 1)
			{
				this.RemoveSegment();
			}
		}
		else if (this.simulationSegments.Count < num)
		{
			this.AddSegment();
		}
		if (this.simulationSegments.Count > 1)
		{
			float num2 = this.Segments % 1f;
			List<Transform> list = this.simulationSegments;
			ConfigurableJoint component = list[list.Count - 1].GetComponent<ConfigurableJoint>();
			Vector3 vector = Vector3.Lerp(this.startAnchorOf2ndSegment, -this.spacing.oxo(), Mathf.Clamp01(this.timeSinceRemoved / this.slurpTime));
			component.connectedAnchor = Vector3.Lerp(this.spacing.oxo(), vector, num2);
			component.GetComponent<Collider>().enabled = true;
		}
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x0002A784 File Offset: 0x00028984
	private void FixedUpdate()
	{
		if (!base.photonView.IsMine || this.creatorLeft)
		{
			return;
		}
		if (this.antigrav)
		{
			using (List<Transform>.Enumerator enumerator = this.simulationSegments.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Transform transform = enumerator.Current;
					transform.GetComponent<Rigidbody>().AddForce(-Physics.gravity * 2f, ForceMode.Acceleration);
				}
				return;
			}
		}
		foreach (Character character in this.charactersClimbing)
		{
			float ropePercent = character.data.ropePercent;
			this.climbingAPI.GetSegmentFromPercent(ropePercent).GetComponent<Rigidbody>().AddForce(Vector3.down * this.climberGravity, ForceMode.Acceleration);
		}
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x0002A878 File Offset: 0x00028A78
	public override void OnEnable()
	{
		base.OnEnable();
		PhotonNetwork.AddCallbackTarget(this);
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x0002A886 File Offset: 0x00028A86
	public override void OnDisable()
	{
		base.OnDisable();
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x0002A894 File Offset: 0x00028A94
	public List<Transform> GetRopeSegments()
	{
		if (base.photonView.IsMine)
		{
			return this.simulationSegments;
		}
		return this.remoteColliderSegments;
	}

	// Token: 0x06000807 RID: 2055 RVA: 0x0002A8B0 File Offset: 0x00028AB0
	public bool IsActive()
	{
		bool flag = true;
		if (this.itemSpool != null && this.itemSpool.itemState != ItemState.Held)
		{
			flag = false;
		}
		return flag;
	}

	// Token: 0x06000808 RID: 2056 RVA: 0x0002A8E0 File Offset: 0x00028AE0
	[PunRPC]
	public void Detach_Rpc()
	{
		if (this.spool != null)
		{
			this.spool.ropeInstance = null;
			this.spool.rope = null;
			this.spool.Segments = 0f;
			this.spool.ClearRope();
			this.spool.RopeFuel -= this.segments;
		}
		if (this.view.IsMine)
		{
			Object.DestroyImmediate(this.simulationSegments.First<Transform>().GetComponent<ConfigurableJoint>());
		}
		this.spool = null;
		this.attachmenState = Rope.ATTACHMENT.unattached;
		Debug.Log(string.Format("Detach_Rpc: {0}", this.attachmenState));
		this.ropeBoneVisualizer.StartTransform = null;
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x0002A99C File Offset: 0x00028B9C
	public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
	{
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x0002A99E File Offset: 0x00028B9E
	public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		this.creatorLeft = true;
		Debug.Log(string.Format("OnMasterClientSwitched: {0}, isMaster: {1}, frame: {2}", newMasterClient, PhotonNetwork.IsMasterClient, Time.frameCount));
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x0002A9D4 File Offset: 0x00028BD4
	public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
	{
		if (targetView != base.photonView)
		{
			return;
		}
		Debug.Log("Trasfered ownership to me");
		this.creatorLeft = true;
		if (this.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			Debug.Log(string.Format("attached to spool, deleting rope: {0}", this.view));
			PhotonNetwork.Destroy(this.view);
		}
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0002AA2A File Offset: 0x00028C2A
	public void OnOwnershipTransferFailed(PhotonView targetView, Photon.Realtime.Player senderOfFailedRequest)
	{
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x0002AA2C File Offset: 0x00028C2C
	[PunRPC]
	public void AttachToAnchor_Rpc(PhotonView anchorView)
	{
		if (this.ropeBoneVisualizer == null)
		{
			this.ropeBoneVisualizer = base.GetComponentInChildren<RopeBoneVisualizer>();
		}
		if (this.attachmenState == Rope.ATTACHMENT.inSpool)
		{
			this.Detach_Rpc();
		}
		this.attachedToAnchor = anchorView.GetComponent<RopeAnchor>();
		this.attachmenState = Rope.ATTACHMENT.anchored;
		Debug.Log(string.Format("AttachToAnchor_Rpc: {0}", this.attachmenState));
		this.ropeBoneVisualizer.StartTransform = this.attachedToAnchor.anchorPoint;
		if (!base.photonView.IsMine)
		{
			return;
		}
		List<Transform> ropeSegments = this.GetRopeSegments();
		if (ropeSegments.Count > 0)
		{
			ropeSegments[0].GetComponent<RopeSegment>().Tie(this.attachedToAnchor.anchorPoint.position);
		}
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x0002AAE4 File Offset: 0x00028CE4
	public float GetLengthInMeters()
	{
		return Rope.GetLengthInMeters((float)this.GetRopeSegments().Count);
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x0002AAF7 File Offset: 0x00028CF7
	public static float GetLengthInMeters(float segmentCount)
	{
		return segmentCount * 0.25f;
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x0002AB00 File Offset: 0x00028D00
	[PunRPC]
	public void AttachToSpool_Rpc(PhotonView viewSpool)
	{
		this.spool = viewSpool.GetComponent<RopeSpool>();
		if (this.spool == null)
		{
			Debug.LogError("Spool is null");
			return;
		}
		this.spool.ropeInstance = base.gameObject;
		this.spool.rope = this;
		this.ropeBoneVisualizer.StartTransform = this.spool.ropeStart;
		base.transform.position = this.spool.ropeBase.position;
		base.transform.rotation = this.spool.ropeBase.rotation;
		this.attachmenState = Rope.ATTACHMENT.inSpool;
		Debug.Log(string.Format("AttachToSpool_Rpc: {0}", this.attachmenState));
		this.Segments = 0f;
		Physics.SyncTransforms();
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x0002ABCC File Offset: 0x00028DCC
	public void AddSegment()
	{
		bool flag = this.simulationSegments.Count == 0;
		Transform transform = null;
		if (!flag)
		{
			transform = this.simulationSegments[0];
		}
		Vector3 vector = (flag ? base.transform.position : transform.transform.position);
		Quaternion quaternion = (flag ? base.transform.rotation : transform.transform.rotation);
		GameObject gameObject = Object.Instantiate<GameObject>(this.ropeSegmentPrefab, vector, quaternion, base.transform);
		gameObject.gameObject.name = "RopeSegment: " + this.simulationSegments.Count.ToString();
		ConfigurableJoint component = gameObject.GetComponent<ConfigurableJoint>();
		if (flag)
		{
			component.autoConfigureConnectedAnchor = true;
			if (this.spool != null)
			{
				component.transform.position = this.spool.ropeBase.position;
				component.transform.rotation = this.spool.ropeBase.rotation;
				component.autoConfigureConnectedAnchor = true;
				component.connectedBody = this.spool.rig;
				component.angularXMotion = ConfigurableJointMotion.Limited;
				component.angularXLimitSpring = new SoftJointLimitSpring
				{
					spring = 35f,
					damper = 45f
				};
				component.angularYZLimitSpring = new SoftJointLimitSpring
				{
					spring = 35f,
					damper = 45f
				};
				component.angularZMotion = ConfigurableJointMotion.Limited;
			}
		}
		else
		{
			component.connectedBody = transform.GetComponent<Rigidbody>();
		}
		this.simulationSegments.Add(gameObject.transform);
		if (this.simulationSegments.Count > 2)
		{
			List<Transform> list = this.simulationSegments;
			Component component2 = list[list.Count - 2];
			Rigidbody component3 = gameObject.GetComponent<Rigidbody>();
			ConfigurableJoint component4 = component2.GetComponent<ConfigurableJoint>();
			component4.connectedBody = component3;
			this.startAnchorOf2ndSegment = new Vector3(0f, -this.spacing, 0f);
			component4.connectedAnchor = this.startAnchorOf2ndSegment;
		}
	}

	// Token: 0x06000812 RID: 2066 RVA: 0x0002ADC8 File Offset: 0x00028FC8
	private void RemoveSegment()
	{
		List<Transform> list = this.simulationSegments;
		Transform transform = list[list.Count - 1];
		List<Transform> list2 = this.simulationSegments;
		Transform transform2 = list2[list2.Count - 2];
		Transform transform3 = this.simulationSegments[0];
		Object.DestroyImmediate(transform.gameObject);
		this.simulationSegments.RemoveLast<Transform>();
		ConfigurableJoint component = transform2.GetComponent<ConfigurableJoint>();
		if (transform2 == transform3)
		{
			Debug.LogError("Attempting to connect joint to itself");
			return;
		}
		this.timeSinceRemoved = 0f;
		component.connectedBody = transform3.GetComponent<Rigidbody>();
		this.startAnchorOf2ndSegment = transform3.InverseTransformPoint(component.transform.position);
		component.connectedAnchor = this.startAnchorOf2ndSegment;
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x0002AE74 File Offset: 0x00029074
	public RopeSyncData GetSyncData()
	{
		RopeSyncData ropeSyncData = new RopeSyncData
		{
			isVisible = this.isClimbable,
			segments = new RopeSyncData.SegmentData[this.simulationSegments.Count]
		};
		for (int i = 0; i < this.simulationSegments.Count; i++)
		{
			ropeSyncData.segments[i] = new RopeSyncData.SegmentData
			{
				position = this.simulationSegments[i].position,
				rotation = this.simulationSegments[i].rotation
			};
		}
		return ropeSyncData;
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x0002AF10 File Offset: 0x00029110
	public void SetSyncData(RopeSyncData data)
	{
		if (data.updateVisualizerManually)
		{
			this.ropeBoneVisualizer.ManuallyUpdateNextFrame = Optionable<bool>.Some(true);
		}
		if (this.creatorLeft)
		{
			return;
		}
		this.isClimbable = data.isVisible;
		int num = data.segments.Length;
		int count = this.remoteColliderSegments.Count;
		if (num < count)
		{
			int num2 = count - num;
			for (int i = 0; i < num2; i++)
			{
				List<Transform> list = this.remoteColliderSegments;
				Component component = list[list.Count - 1];
				this.remoteColliderSegments.RemoveLast<Transform>();
				Object.Destroy(component.gameObject);
			}
		}
		else if (num > count)
		{
			int num3 = num - count;
			for (int j = 0; j < num3; j++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.remoteSegmentPrefab, Vector3.zero, Quaternion.identity, base.transform);
				gameObject.GetComponent<RopeSegment>().rope = this;
				this.remoteColliderSegments.Add(gameObject.transform);
			}
		}
		if (num != this.remoteColliderSegments.Count)
		{
			Debug.LogError("Remote Segment Logic Failed");
			return;
		}
		for (int k = 0; k < data.segments.Length; k++)
		{
			this.remoteColliderSegments[k].position = data.segments[k].position;
			this.remoteColliderSegments[k].rotation = data.segments[k].rotation;
		}
		this.ropeBoneVisualizer.SetData(data);
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x0002B07D File Offset: 0x0002927D
	public float GetTotalLength()
	{
		return (float)this.SegmentCount * this.spacing;
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x0002B090 File Offset: 0x00029290
	public void Clear(bool alsoRemoveRemote = false)
	{
		Debug.Log("Rope Clear!");
		if (this.simulationSegments.Count > 0)
		{
			for (int i = this.simulationSegments.Count - 1; i >= 0; i--)
			{
				Object.Destroy(this.simulationSegments[i].gameObject);
			}
			this.simulationSegments.Clear();
		}
		if (alsoRemoveRemote)
		{
			for (int j = this.remoteColliderSegments.Count - 1; j >= 0; j--)
			{
				Object.Destroy(this.remoteColliderSegments[j].gameObject);
			}
			this.remoteColliderSegments.Clear();
		}
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x0002B12A File Offset: 0x0002932A
	public void AddCharacterClimbing(Character character)
	{
		this.charactersClimbing.Add(character);
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0002B138 File Offset: 0x00029338
	public void RemoveCharacterClimbing(Character character)
	{
		this.charactersClimbing.Remove(character);
	}

	// Token: 0x0400077D RID: 1917
	public float spacing = 0.75f;

	// Token: 0x0400077E RID: 1918
	public float climberGravity = 1f;

	// Token: 0x0400077F RID: 1919
	public float slurpTime = 10f;

	// Token: 0x04000780 RID: 1920
	public bool antigrav;

	// Token: 0x04000781 RID: 1921
	public bool isHelicopterRope;

	// Token: 0x04000782 RID: 1922
	public GameObject ropeSegmentPrefab;

	// Token: 0x04000783 RID: 1923
	public GameObject remoteSegmentPrefab;

	// Token: 0x04000784 RID: 1924
	public Rope.ATTACHMENT attachmenState;

	// Token: 0x04000785 RID: 1925
	public bool isClimbable;

	// Token: 0x04000786 RID: 1926
	public PhotonView view;

	// Token: 0x04000787 RID: 1927
	private readonly List<Transform> remoteColliderSegments = new List<Transform>();

	// Token: 0x04000788 RID: 1928
	private readonly List<Transform> simulationSegments = new List<Transform>();

	// Token: 0x04000789 RID: 1929
	[NonSerialized]
	public List<Character> charactersClimbing = new List<Character>();

	// Token: 0x0400078A RID: 1930
	[NonSerialized]
	public RopeClimbingAPI climbingAPI;

	// Token: 0x0400078B RID: 1931
	private Item itemSpool;

	// Token: 0x0400078C RID: 1932
	private RopeBoneVisualizer ropeBoneVisualizer;

	// Token: 0x0400078D RID: 1933
	private float segments;

	// Token: 0x0400078E RID: 1934
	private RopeSpool spool;

	// Token: 0x0400078F RID: 1935
	private Vector3 startAnchorOf2ndSegment;

	// Token: 0x04000790 RID: 1936
	private float timeSinceRemoved;

	// Token: 0x04000791 RID: 1937
	public bool creatorLeft;

	// Token: 0x04000792 RID: 1938
	private RopeAnchor attachedToAnchor;

	// Token: 0x0200034A RID: 842
	public enum ATTACHMENT
	{
		// Token: 0x0400121E RID: 4638
		unattached,
		// Token: 0x0400121F RID: 4639
		inSpool,
		// Token: 0x04001220 RID: 4640
		anchored
	}
}
