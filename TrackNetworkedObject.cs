using System;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class TrackNetworkedObject : MonoBehaviour
{
	// Token: 0x06000958 RID: 2392 RVA: 0x0002F16D File Offset: 0x0002D36D
	private void OnEnable()
	{
		TrackableNetworkObject.OnTrackableObjectCreated = (Action<int>)Delegate.Combine(TrackableNetworkObject.OnTrackableObjectCreated, new Action<int>(this.TryReattachToTrackedObject));
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x0002F18F File Offset: 0x0002D38F
	private void OnDisable()
	{
		TrackableNetworkObject.OnTrackableObjectCreated = (Action<int>)Delegate.Remove(TrackableNetworkObject.OnTrackableObjectCreated, new Action<int>(this.TryReattachToTrackedObject));
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x0002F1B1 File Offset: 0x0002D3B1
	private void TryReattachToTrackedObject(int ID)
	{
		this.TryGetTrackedObject();
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x0002F1BC File Offset: 0x0002D3BC
	private void TryGetTrackedObject()
	{
		if (this.trackedObjectID == -1)
		{
			Debug.LogError("TrackNetworkObject has a value of -1. This should never happen.");
			base.enabled = false;
			return;
		}
		TrackableNetworkObject trackableObject = TrackableNetworkObject.GetTrackableObject(this.trackedObjectID);
		if (trackableObject != null)
		{
			this.SetObject(trackableObject);
			this.lostTrackableTick = 0;
			return;
		}
		this.lostTrackableTick++;
		if (this.lostTrackableTick > 20)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0600095C RID: 2396 RVA: 0x0002F22C File Offset: 0x0002D42C
	public void SetObject(TrackableNetworkObject trackableObject)
	{
		this.trackedObject = trackableObject;
		this.trackedObjectID = trackableObject.instanceID;
		this.trackedObject.currentTracker = this;
		Debug.Log(string.Format("Object {0} Reconnected to trackable object {1} with photon ID {2}", base.gameObject.GetHashCode(), this.trackedObjectID, trackableObject.photonView.ViewID));
	}

	// Token: 0x0600095D RID: 2397 RVA: 0x0002F294 File Offset: 0x0002D494
	private void LateUpdate()
	{
		if (this.trackedObject == null)
		{
			this.TryGetTrackedObject();
		}
		if (this.trackedObject != null)
		{
			base.transform.rotation = this.trackedObject.transform.rotation;
			base.transform.position = this.trackedObject.transform.TransformPoint(this.offset);
		}
	}

	// Token: 0x04000847 RID: 2119
	public int trackedObjectID = -1;

	// Token: 0x04000848 RID: 2120
	public TrackableNetworkObject trackedObject;

	// Token: 0x04000849 RID: 2121
	public Vector3 offset;

	// Token: 0x0400084A RID: 2122
	private int lostTrackableTick;
}
