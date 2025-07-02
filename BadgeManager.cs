using System;
using TMPro;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class BadgeManager : MonoBehaviour
{
	// Token: 0x1700007C RID: 124
	// (get) Token: 0x0600097E RID: 2430 RVA: 0x0002FDCB File Offset: 0x0002DFCB
	// (set) Token: 0x0600097F RID: 2431 RVA: 0x0002FDD4 File Offset: 0x0002DFD4
	public BadgeUI selectedBadge
	{
		get
		{
			return this._selectedBadge;
		}
		set
		{
			this._selectedBadge = value;
			if (this._selectedBadge != null && this._selectedBadge.data != null)
			{
				if (this._selectedBadge.data.IsLocked)
				{
					this.badgePopupName.text = "???";
					this.badgePopupDescription.text = this._selectedBadge.data.description;
				}
				else
				{
					this.badgePopupName.text = this._selectedBadge.data.displayName + " Badge";
					this.badgePopupDescription.text = this._selectedBadge.data.description;
				}
			}
			else
			{
				this.badgePopupName.text = "???";
				this.badgePopupDescription.text = "You don't have this badge yet!";
			}
			this.badgePopupAnim.Play("Popup", 0, 0f);
		}
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x0002FEC6 File Offset: 0x0002E0C6
	public void InheritData(BadgeManager other)
	{
		this.badgeData = new BadgeData[other.badgeData.Length];
		other.badgeData.CopyTo(this.badgeData, 0);
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x0002FEED File Offset: 0x0002E0ED
	private void OnEnable()
	{
		this.selectedBadge = null;
		if (this.initBadgesOnEnable)
		{
			this.InitBadges();
		}
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x0002FF04 File Offset: 0x0002E104
	public BadgeData GetBadgeData(ACHIEVEMENTTYPE achievementType)
	{
		foreach (BadgeData badgeData in this.badgeData)
		{
			if (badgeData.linkedAchievement == achievementType)
			{
				return badgeData;
			}
		}
		return null;
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x0002FF38 File Offset: 0x0002E138
	private void InitBadges()
	{
		this.badges = base.GetComponentsInChildren<BadgeUI>();
		for (int i = 0; i < this.badges.Length; i++)
		{
			if (i < this.badgeData.Length)
			{
				this.badges[i].Init(this.badgeData[i]);
			}
			else
			{
				this.badges[i].Init(null);
			}
		}
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x0002FF94 File Offset: 0x0002E194
	private void Update()
	{
		this.badgePopup.SetActive(this.selectedBadge != null);
		if (this.selectedBadge)
		{
			this.badgePopup.transform.position = this.selectedBadge.transform.position;
		}
	}

	// Token: 0x04000866 RID: 2150
	private BadgeUI _selectedBadge;

	// Token: 0x04000867 RID: 2151
	public GameObject badgePopup;

	// Token: 0x04000868 RID: 2152
	public Animator badgePopupAnim;

	// Token: 0x04000869 RID: 2153
	public TextMeshProUGUI badgePopupName;

	// Token: 0x0400086A RID: 2154
	public TextMeshProUGUI badgePopupDescription;

	// Token: 0x0400086B RID: 2155
	public BadgeData[] badgeData;

	// Token: 0x0400086C RID: 2156
	private BadgeUI[] badges;

	// Token: 0x0400086D RID: 2157
	public bool initBadgesOnEnable;
}
