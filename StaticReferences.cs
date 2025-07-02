using System;
using UnityEngine;
using UnityEngine.Audio;
using Zorro.Core;

// Token: 0x02000142 RID: 322
[CreateAssetMenu(fileName = "StaticReferences", menuName = "Peak/StaticReferences")]
public class StaticReferences : SingletonAsset<StaticReferences>
{
	// Token: 0x04000837 RID: 2103
	public AudioMixerGroup masterMixerGroup;
}
