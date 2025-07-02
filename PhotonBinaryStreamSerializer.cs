using System;
using Photon.Pun;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x020000FC RID: 252
public abstract class PhotonBinaryStreamSerializer<T> : MonoBehaviourPunCallbacks, IPunObservable where T : struct, IBinarySerializable
{
	// Token: 0x06000775 RID: 1909
	public abstract T GetDataToWrite();

	// Token: 0x06000776 RID: 1910 RVA: 0x00027E58 File Offset: 0x00026058
	protected virtual void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x00027E68 File Offset: 0x00026068
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			if (this.ShouldSendData())
			{
				if (IBinarySerializable.shouldLog)
				{
					Debug.Log(base.gameObject.name + " sending data in type " + base.GetType().Name);
				}
				T dataToWrite = this.GetDataToWrite();
				BinarySerializer binarySerializer = new BinarySerializer(UnsafeUtility.SizeOf<T>(), Allocator.Temp);
				dataToWrite.Serialize(binarySerializer);
				byte[] array = binarySerializer.buffer.ToByteArray();
				NetworkStats.RegisterBytesSent<T>((ulong)((long)array.Length));
				stream.SendNext(array);
				binarySerializer.Dispose();
				return;
			}
		}
		else
		{
			if (IBinarySerializable.shouldLog)
			{
				Debug.Log(base.gameObject.name + " received data in type " + base.GetType().Name);
			}
			BinaryDeserializer binaryDeserializer = new BinaryDeserializer((byte[])stream.ReceiveNext(), Allocator.Temp);
			T t = new T();
			t.Deserialize(binaryDeserializer);
			binaryDeserializer.Dispose();
			this.RemoteValue = Optionable<T>.Some(t);
			this.OnDataReceived(t);
		}
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x00027F66 File Offset: 0x00026166
	public virtual void OnDataReceived(T data)
	{
		this.sinceLastPackage = 0f;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00027F73 File Offset: 0x00026173
	public virtual bool ShouldSendData()
	{
		return true;
	}

	// Token: 0x04000704 RID: 1796
	protected Optionable<T> RemoteValue;

	// Token: 0x04000705 RID: 1797
	protected float sinceLastPackage;

	// Token: 0x04000706 RID: 1798
	protected new PhotonView photonView;
}
