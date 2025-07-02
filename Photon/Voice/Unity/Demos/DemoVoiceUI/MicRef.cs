using System;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x020002C0 RID: 704
	public struct MicRef
	{
		// Token: 0x06001143 RID: 4419 RVA: 0x00055923 File Offset: 0x00053B23
		public MicRef(MicType micType, DeviceInfo device)
		{
			this.MicType = micType;
			this.Device = device;
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x00055933 File Offset: 0x00053B33
		public override string ToString()
		{
			return string.Format("Mic reference: {0}", this.Device.Name);
		}

		// Token: 0x04000FD4 RID: 4052
		public readonly MicType MicType;

		// Token: 0x04000FD5 RID: 4053
		public readonly DeviceInfo Device;
	}
}
