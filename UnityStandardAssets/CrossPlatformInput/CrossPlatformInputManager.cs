using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput.PlatformSpecific;

namespace UnityStandardAssets.CrossPlatformInput
{
	// Token: 0x020002A9 RID: 681
	public static class CrossPlatformInputManager
	{
		// Token: 0x06001035 RID: 4149 RVA: 0x00051C6D File Offset: 0x0004FE6D
		public static void SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod activeInputMethod)
		{
			if (activeInputMethod == CrossPlatformInputManager.ActiveInputMethod.Hardware)
			{
				CrossPlatformInputManager.activeInput = CrossPlatformInputManager.s_HardwareInput;
				return;
			}
			if (activeInputMethod != CrossPlatformInputManager.ActiveInputMethod.Touch)
			{
				return;
			}
			CrossPlatformInputManager.activeInput = CrossPlatformInputManager.s_TouchInput;
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x00051C8C File Offset: 0x0004FE8C
		public static bool AxisExists(string name)
		{
			return CrossPlatformInputManager.activeInput.AxisExists(name);
		}

		// Token: 0x06001037 RID: 4151 RVA: 0x00051C99 File Offset: 0x0004FE99
		public static bool ButtonExists(string name)
		{
			return CrossPlatformInputManager.activeInput.ButtonExists(name);
		}

		// Token: 0x06001038 RID: 4152 RVA: 0x00051CA6 File Offset: 0x0004FEA6
		public static void RegisterVirtualAxis(CrossPlatformInputManager.VirtualAxis axis)
		{
			CrossPlatformInputManager.activeInput.RegisterVirtualAxis(axis);
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x00051CB3 File Offset: 0x0004FEB3
		public static void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
		{
			CrossPlatformInputManager.activeInput.RegisterVirtualButton(button);
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x00051CC0 File Offset: 0x0004FEC0
		public static void UnRegisterVirtualAxis(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			CrossPlatformInputManager.activeInput.UnRegisterVirtualAxis(name);
		}

		// Token: 0x0600103B RID: 4155 RVA: 0x00051CDB File Offset: 0x0004FEDB
		public static void UnRegisterVirtualButton(string name)
		{
			CrossPlatformInputManager.activeInput.UnRegisterVirtualButton(name);
		}

		// Token: 0x0600103C RID: 4156 RVA: 0x00051CE8 File Offset: 0x0004FEE8
		public static CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
		{
			return CrossPlatformInputManager.activeInput.VirtualAxisReference(name);
		}

		// Token: 0x0600103D RID: 4157 RVA: 0x00051CF5 File Offset: 0x0004FEF5
		public static float GetAxis(string name)
		{
			return CrossPlatformInputManager.GetAxis(name, false);
		}

		// Token: 0x0600103E RID: 4158 RVA: 0x00051CFE File Offset: 0x0004FEFE
		public static float GetAxisRaw(string name)
		{
			return CrossPlatformInputManager.GetAxis(name, true);
		}

		// Token: 0x0600103F RID: 4159 RVA: 0x00051D07 File Offset: 0x0004FF07
		private static float GetAxis(string name, bool raw)
		{
			return CrossPlatformInputManager.activeInput.GetAxis(name, raw);
		}

		// Token: 0x06001040 RID: 4160 RVA: 0x00051D15 File Offset: 0x0004FF15
		public static bool GetButton(string name)
		{
			return CrossPlatformInputManager.activeInput.GetButton(name);
		}

		// Token: 0x06001041 RID: 4161 RVA: 0x00051D22 File Offset: 0x0004FF22
		public static bool GetButtonDown(string name)
		{
			return CrossPlatformInputManager.activeInput.GetButtonDown(name);
		}

		// Token: 0x06001042 RID: 4162 RVA: 0x00051D2F File Offset: 0x0004FF2F
		public static bool GetButtonUp(string name)
		{
			return CrossPlatformInputManager.activeInput.GetButtonUp(name);
		}

		// Token: 0x06001043 RID: 4163 RVA: 0x00051D3C File Offset: 0x0004FF3C
		public static void SetButtonDown(string name)
		{
			CrossPlatformInputManager.activeInput.SetButtonDown(name);
		}

		// Token: 0x06001044 RID: 4164 RVA: 0x00051D49 File Offset: 0x0004FF49
		public static void SetButtonUp(string name)
		{
			CrossPlatformInputManager.activeInput.SetButtonUp(name);
		}

		// Token: 0x06001045 RID: 4165 RVA: 0x00051D56 File Offset: 0x0004FF56
		public static void SetAxisPositive(string name)
		{
			CrossPlatformInputManager.activeInput.SetAxisPositive(name);
		}

		// Token: 0x06001046 RID: 4166 RVA: 0x00051D63 File Offset: 0x0004FF63
		public static void SetAxisNegative(string name)
		{
			CrossPlatformInputManager.activeInput.SetAxisNegative(name);
		}

		// Token: 0x06001047 RID: 4167 RVA: 0x00051D70 File Offset: 0x0004FF70
		public static void SetAxisZero(string name)
		{
			CrossPlatformInputManager.activeInput.SetAxisZero(name);
		}

		// Token: 0x06001048 RID: 4168 RVA: 0x00051D7D File Offset: 0x0004FF7D
		public static void SetAxis(string name, float value)
		{
			CrossPlatformInputManager.activeInput.SetAxis(name, value);
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06001049 RID: 4169 RVA: 0x00051D8B File Offset: 0x0004FF8B
		public static Vector3 mousePosition
		{
			get
			{
				return CrossPlatformInputManager.activeInput.MousePosition();
			}
		}

		// Token: 0x0600104A RID: 4170 RVA: 0x00051D97 File Offset: 0x0004FF97
		public static void SetVirtualMousePositionX(float f)
		{
			CrossPlatformInputManager.activeInput.SetVirtualMousePositionX(f);
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x00051DA4 File Offset: 0x0004FFA4
		public static void SetVirtualMousePositionY(float f)
		{
			CrossPlatformInputManager.activeInput.SetVirtualMousePositionY(f);
		}

		// Token: 0x0600104C RID: 4172 RVA: 0x00051DB1 File Offset: 0x0004FFB1
		public static void SetVirtualMousePositionZ(float f)
		{
			CrossPlatformInputManager.activeInput.SetVirtualMousePositionZ(f);
		}

		// Token: 0x04000F27 RID: 3879
		private static VirtualInput activeInput = CrossPlatformInputManager.s_HardwareInput;

		// Token: 0x04000F28 RID: 3880
		private static VirtualInput s_TouchInput = new MobileInput();

		// Token: 0x04000F29 RID: 3881
		private static VirtualInput s_HardwareInput = new StandaloneInput();

		// Token: 0x020003C3 RID: 963
		public enum ActiveInputMethod
		{
			// Token: 0x040013E5 RID: 5093
			Hardware,
			// Token: 0x040013E6 RID: 5094
			Touch
		}

		// Token: 0x020003C4 RID: 964
		public class VirtualAxis
		{
			// Token: 0x17000162 RID: 354
			// (get) Token: 0x060014F7 RID: 5367 RVA: 0x000612DC File Offset: 0x0005F4DC
			// (set) Token: 0x060014F8 RID: 5368 RVA: 0x000612E4 File Offset: 0x0005F4E4
			public string name { get; private set; }

			// Token: 0x17000163 RID: 355
			// (get) Token: 0x060014F9 RID: 5369 RVA: 0x000612ED File Offset: 0x0005F4ED
			// (set) Token: 0x060014FA RID: 5370 RVA: 0x000612F5 File Offset: 0x0005F4F5
			public bool matchWithInputManager { get; private set; }

			// Token: 0x060014FB RID: 5371 RVA: 0x000612FE File Offset: 0x0005F4FE
			public VirtualAxis(string name)
				: this(name, true)
			{
			}

			// Token: 0x060014FC RID: 5372 RVA: 0x00061308 File Offset: 0x0005F508
			public VirtualAxis(string name, bool matchToInputSettings)
			{
				this.name = name;
				this.matchWithInputManager = matchToInputSettings;
			}

			// Token: 0x060014FD RID: 5373 RVA: 0x0006131E File Offset: 0x0005F51E
			public void Remove()
			{
				CrossPlatformInputManager.UnRegisterVirtualAxis(this.name);
			}

			// Token: 0x060014FE RID: 5374 RVA: 0x0006132B File Offset: 0x0005F52B
			public void Update(float value)
			{
				this.m_Value = value;
			}

			// Token: 0x17000164 RID: 356
			// (get) Token: 0x060014FF RID: 5375 RVA: 0x00061334 File Offset: 0x0005F534
			public float GetValue
			{
				get
				{
					return this.m_Value;
				}
			}

			// Token: 0x17000165 RID: 357
			// (get) Token: 0x06001500 RID: 5376 RVA: 0x0006133C File Offset: 0x0005F53C
			public float GetValueRaw
			{
				get
				{
					return this.m_Value;
				}
			}

			// Token: 0x040013E8 RID: 5096
			private float m_Value;
		}

		// Token: 0x020003C5 RID: 965
		public class VirtualButton
		{
			// Token: 0x17000166 RID: 358
			// (get) Token: 0x06001501 RID: 5377 RVA: 0x00061344 File Offset: 0x0005F544
			// (set) Token: 0x06001502 RID: 5378 RVA: 0x0006134C File Offset: 0x0005F54C
			public string name { get; private set; }

			// Token: 0x17000167 RID: 359
			// (get) Token: 0x06001503 RID: 5379 RVA: 0x00061355 File Offset: 0x0005F555
			// (set) Token: 0x06001504 RID: 5380 RVA: 0x0006135D File Offset: 0x0005F55D
			public bool matchWithInputManager { get; private set; }

			// Token: 0x06001505 RID: 5381 RVA: 0x00061366 File Offset: 0x0005F566
			public VirtualButton(string name)
				: this(name, true)
			{
			}

			// Token: 0x06001506 RID: 5382 RVA: 0x00061370 File Offset: 0x0005F570
			public VirtualButton(string name, bool matchToInputSettings)
			{
				this.name = name;
				this.matchWithInputManager = matchToInputSettings;
			}

			// Token: 0x06001507 RID: 5383 RVA: 0x00061396 File Offset: 0x0005F596
			public void Pressed()
			{
				if (this.m_Pressed)
				{
					return;
				}
				this.m_Pressed = true;
				this.m_LastPressedFrame = Time.frameCount;
			}

			// Token: 0x06001508 RID: 5384 RVA: 0x000613B3 File Offset: 0x0005F5B3
			public void Released()
			{
				this.m_Pressed = false;
				this.m_ReleasedFrame = Time.frameCount;
			}

			// Token: 0x06001509 RID: 5385 RVA: 0x000613C7 File Offset: 0x0005F5C7
			public void Remove()
			{
				CrossPlatformInputManager.UnRegisterVirtualButton(this.name);
			}

			// Token: 0x17000168 RID: 360
			// (get) Token: 0x0600150A RID: 5386 RVA: 0x000613D4 File Offset: 0x0005F5D4
			public bool GetButton
			{
				get
				{
					return this.m_Pressed;
				}
			}

			// Token: 0x17000169 RID: 361
			// (get) Token: 0x0600150B RID: 5387 RVA: 0x000613DC File Offset: 0x0005F5DC
			public bool GetButtonDown
			{
				get
				{
					return this.m_LastPressedFrame - Time.frameCount == -1;
				}
			}

			// Token: 0x1700016A RID: 362
			// (get) Token: 0x0600150C RID: 5388 RVA: 0x000613ED File Offset: 0x0005F5ED
			public bool GetButtonUp
			{
				get
				{
					return this.m_ReleasedFrame == Time.frameCount - 1;
				}
			}

			// Token: 0x040013EC RID: 5100
			private int m_LastPressedFrame = -5;

			// Token: 0x040013ED RID: 5101
			private int m_ReleasedFrame = -5;

			// Token: 0x040013EE RID: 5102
			private bool m_Pressed;
		}
	}
}
