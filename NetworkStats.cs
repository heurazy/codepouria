using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000100 RID: 256
public class NetworkStats : Singleton<NetworkStats>
{
	// Token: 0x06000795 RID: 1941 RVA: 0x00028640 File Offset: 0x00026840
	private void Update()
	{
		this.m_timer += Time.deltaTime;
		if (this.m_timer > 1f)
		{
			this.m_timer -= 1f;
			this.m_lastRecievedDelta = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesIn - this.m_bytesReceivedLastSecond;
			this.m_bytesReceivedLastSecond = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesIn;
			this.m_lastSentDelta = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesOut - this.m_bytesSentLastSecond;
			this.m_bytesSentLastSecond = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesOut;
			foreach (KeyValuePair<string, ulong> keyValuePair in this.m_binaryStreamsByType)
			{
				string key = keyValuePair.Key;
				ulong value = keyValuePair.Value;
				this.<Update>g__UpdateEntry|8_0(key, value);
			}
			this.<Update>g__UpdateEntry|8_0("VoiceData", (ulong)PhotonVoiceStats.bytesSent);
		}
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x0002874C File Offset: 0x0002694C
	public static void RegisterBytesSent<T>(ulong bytesSent)
	{
		Type typeFromHandle = typeof(T);
		if (!Singleton<NetworkStats>.Instance.m_binaryStreamsByType.ContainsKey(typeFromHandle.Name))
		{
			Singleton<NetworkStats>.Instance.m_binaryStreamsByType.Add(typeFromHandle.Name, 0UL);
		}
		Dictionary<string, ulong> binaryStreamsByType = Singleton<NetworkStats>.Instance.m_binaryStreamsByType;
		string name = typeFromHandle.Name;
		binaryStreamsByType[name] += bytesSent;
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x000287B4 File Offset: 0x000269B4
	public List<ValueTuple<string, ulong>> GetBytesSent()
	{
		return this.m_binaryStreamsByType.Select((KeyValuePair<string, ulong> pair) => new ValueTuple<string, ulong>(pair.Key, pair.Value)).ToList<ValueTuple<string, ulong>>();
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x000287E5 File Offset: 0x000269E5
	public List<ValueTuple<string, ulong>> GetBytesDeltaSent()
	{
		return this.m_binaryStreamsByTypeDelta.Select((KeyValuePair<string, ulong> pair) => new ValueTuple<string, ulong>(pair.Key, pair.Value)).ToList<ValueTuple<string, ulong>>();
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x00028840 File Offset: 0x00026A40
	[CompilerGenerated]
	private void <Update>g__UpdateEntry|8_0(string key, ulong value)
	{
		if (this.m_binaryStreamsByTypeSecond.ContainsKey(key))
		{
			ulong num = this.m_binaryStreamsByTypeSecond[key];
			ulong num2 = value - num;
			this.m_binaryStreamsByTypeDelta[key] = num2;
		}
		this.m_binaryStreamsByTypeSecond[key] = value;
	}

	// Token: 0x04000711 RID: 1809
	public long m_bytesReceivedLastSecond;

	// Token: 0x04000712 RID: 1810
	public long m_lastRecievedDelta;

	// Token: 0x04000713 RID: 1811
	public long m_bytesSentLastSecond;

	// Token: 0x04000714 RID: 1812
	public long m_lastSentDelta;

	// Token: 0x04000715 RID: 1813
	private float m_timer;

	// Token: 0x04000716 RID: 1814
	private Dictionary<string, ulong> m_binaryStreamsByType = new Dictionary<string, ulong>();

	// Token: 0x04000717 RID: 1815
	private Dictionary<string, ulong> m_binaryStreamsByTypeSecond = new Dictionary<string, ulong>();

	// Token: 0x04000718 RID: 1816
	private Dictionary<string, ulong> m_binaryStreamsByTypeDelta = new Dictionary<string, ulong>();
}
