using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x02000214 RID: 532
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ProximityVoiceTrigger : VoiceComponent
{
	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000DB4 RID: 3508 RVA: 0x00044F8A File Offset: 0x0004318A
	public byte TargetInterestGroup
	{
		get
		{
			if (this.photonView != null)
			{
				return (byte)this.photonView.OwnerActorNr;
			}
			return 0;
		}
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x00044FA8 File Offset: 0x000431A8
	protected override void Awake()
	{
		this.photonVoiceView = base.GetComponentInParent<PhotonVoiceView>();
		this.photonView = base.GetComponentInParent<PhotonView>();
		base.GetComponent<Collider>().isTrigger = true;
		this.IsLocalCheck();
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x00044FD8 File Offset: 0x000431D8
	private void ToggleTransmission()
	{
		if (this.photonVoiceView.RecorderInUse != null)
		{
			byte targetInterestGroup = this.TargetInterestGroup;
			if (this.photonVoiceView.RecorderInUse.InterestGroup != targetInterestGroup)
			{
				base.Logger.Log(LogLevel.Info, "Setting RecorderInUse's InterestGroup to {0}", new object[] { targetInterestGroup });
				this.photonVoiceView.RecorderInUse.InterestGroup = targetInterestGroup;
			}
			this.photonVoiceView.RecorderInUse.RecordingEnabled = true;
		}
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x00045054 File Offset: 0x00043254
	private void OnTriggerEnter(Collider other)
	{
		if (this.IsLocalCheck())
		{
			ProximityVoiceTrigger component = other.GetComponent<ProximityVoiceTrigger>();
			if (component != null)
			{
				byte targetInterestGroup = component.TargetInterestGroup;
				base.Logger.Log(LogLevel.Debug, "OnTriggerEnter {0}", new object[] { targetInterestGroup });
				if (targetInterestGroup == this.TargetInterestGroup)
				{
					return;
				}
				if (targetInterestGroup == 0)
				{
					return;
				}
				if (!this.groupsToAdd.Contains(targetInterestGroup))
				{
					this.groupsToAdd.Add(targetInterestGroup);
				}
			}
		}
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x000450C8 File Offset: 0x000432C8
	private void OnTriggerExit(Collider other)
	{
		if (this.IsLocalCheck())
		{
			ProximityVoiceTrigger component = other.GetComponent<ProximityVoiceTrigger>();
			if (component != null)
			{
				byte targetInterestGroup = component.TargetInterestGroup;
				base.Logger.Log(LogLevel.Debug, "OnTriggerExit {0}", new object[] { targetInterestGroup });
				if (targetInterestGroup == this.TargetInterestGroup)
				{
					return;
				}
				if (targetInterestGroup == 0)
				{
					return;
				}
				if (this.groupsToAdd.Contains(targetInterestGroup))
				{
					this.groupsToAdd.Remove(targetInterestGroup);
				}
				if (!this.groupsToRemove.Contains(targetInterestGroup))
				{
					this.groupsToRemove.Add(targetInterestGroup);
				}
			}
		}
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x00045158 File Offset: 0x00043358
	protected void Update()
	{
		if (!PunVoiceClient.Instance.Client.InRoom)
		{
			this.subscribedGroups = null;
			return;
		}
		if (this.IsLocalCheck())
		{
			if (this.groupsToAdd.Count > 0 || this.groupsToRemove.Count > 0)
			{
				byte[] array = null;
				byte[] array2 = null;
				if (this.groupsToAdd.Count > 0)
				{
					array = this.groupsToAdd.ToArray();
				}
				if (this.groupsToRemove.Count > 0)
				{
					array2 = this.groupsToRemove.ToArray();
				}
				base.Logger.Log(LogLevel.Info, "client of actor number {0} trying to change groups, to_be_removed#={1} to_be_added#={2}", new object[]
				{
					this.TargetInterestGroup,
					this.groupsToRemove.Count,
					this.groupsToAdd.Count
				});
				if (PunVoiceClient.Instance.Client.OpChangeGroups(array2, array))
				{
					if (this.subscribedGroups != null)
					{
						List<byte> list = new List<byte>();
						for (int i = 0; i < this.subscribedGroups.Length; i++)
						{
							list.Add(this.subscribedGroups[i]);
						}
						for (int j = 0; j < this.groupsToRemove.Count; j++)
						{
							if (list.Contains(this.groupsToRemove[j]))
							{
								list.Remove(this.groupsToRemove[j]);
							}
						}
						for (int k = 0; k < this.groupsToAdd.Count; k++)
						{
							if (!list.Contains(this.groupsToAdd[k]))
							{
								list.Add(this.groupsToAdd[k]);
							}
						}
						this.subscribedGroups = list.ToArray();
					}
					else
					{
						this.subscribedGroups = array;
					}
					this.groupsToAdd.Clear();
					this.groupsToRemove.Clear();
				}
				else
				{
					base.Logger.Log(LogLevel.Error, "Error changing groups", Array.Empty<object>());
				}
			}
			this.ToggleTransmission();
		}
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x00045344 File Offset: 0x00043544
	private bool IsLocalCheck()
	{
		if (this.photonView.IsMine)
		{
			return true;
		}
		if (base.enabled)
		{
			base.Logger.Log(LogLevel.Info, "Disabling ProximityVoiceTrigger as does not belong to local player, actor number {0}", new object[] { this.TargetInterestGroup });
			base.enabled = false;
		}
		return false;
	}

	// Token: 0x04000CC6 RID: 3270
	private List<byte> groupsToAdd = new List<byte>();

	// Token: 0x04000CC7 RID: 3271
	private List<byte> groupsToRemove = new List<byte>();

	// Token: 0x04000CC8 RID: 3272
	[SerializeField]
	private byte[] subscribedGroups;

	// Token: 0x04000CC9 RID: 3273
	private PhotonVoiceView photonVoiceView;

	// Token: 0x04000CCA RID: 3274
	private PhotonView photonView;
}
