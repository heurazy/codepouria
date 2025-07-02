using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;

// Token: 0x02000163 RID: 355
public class MenuWindow : MonoBehaviour
{
	// Token: 0x17000082 RID: 130
	// (get) Token: 0x06000A05 RID: 2565 RVA: 0x00031DEB File Offset: 0x0002FFEB
	public virtual bool openOnStart
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x06000A06 RID: 2566 RVA: 0x00031DEE File Offset: 0x0002FFEE
	public virtual bool selectOnOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000084 RID: 132
	// (get) Token: 0x06000A07 RID: 2567 RVA: 0x00031DF1 File Offset: 0x0002FFF1
	public virtual Selectable objectToSelectOnOpen
	{
		get
		{
			return null;
		}
	}

	// Token: 0x17000085 RID: 133
	// (get) Token: 0x06000A08 RID: 2568 RVA: 0x00031DF4 File Offset: 0x0002FFF4
	public virtual bool closeOnPause
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x06000A09 RID: 2569 RVA: 0x00031DF7 File Offset: 0x0002FFF7
	public virtual bool closeOnUICancel
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x06000A0A RID: 2570 RVA: 0x00031DFA File Offset: 0x0002FFFA
	public virtual bool blocksPlayerInput
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x06000A0B RID: 2571 RVA: 0x00031DFD File Offset: 0x0002FFFD
	public virtual bool showCursorWhileOpen
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x06000A0C RID: 2572 RVA: 0x00031E00 File Offset: 0x00030000
	public virtual bool autoHideOnClose
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000A0D RID: 2573 RVA: 0x00031E03 File Offset: 0x00030003
	// (set) Token: 0x06000A0E RID: 2574 RVA: 0x00031E0B File Offset: 0x0003000B
	public bool isOpen { get; private set; }

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x06000A0F RID: 2575 RVA: 0x00031E14 File Offset: 0x00030014
	// (set) Token: 0x06000A10 RID: 2576 RVA: 0x00031E1C File Offset: 0x0003001C
	public bool inputActive { get; private set; }

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x06000A11 RID: 2577 RVA: 0x00031E25 File Offset: 0x00030025
	// (set) Token: 0x06000A12 RID: 2578 RVA: 0x00031E2D File Offset: 0x0003002D
	public bool initialized { get; private set; }

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06000A13 RID: 2579 RVA: 0x00031E36 File Offset: 0x00030036
	public virtual GameObject panel
	{
		get
		{
			return base.gameObject;
		}
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x00031E3E File Offset: 0x0003003E
	protected virtual void Start()
	{
		if (!this.isOpen)
		{
			if (this.openOnStart)
			{
				this.Open();
				return;
			}
			this.StartClosed();
		}
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x00031E5D File Offset: 0x0003005D
	protected virtual void Update()
	{
		this.TestCloseViaInput();
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x00031E68 File Offset: 0x00030068
	private void TestCloseViaInput()
	{
		if (this.inputActive)
		{
			if (this.closeOnPause && Character.localCharacter && Character.localCharacter.input.pauseWasPressed)
			{
				this.Close();
				Character.localCharacter.input.pauseWasPressed = false;
				return;
			}
			if (this.closeOnUICancel && Singleton<UIInputHandler>.Instance.cancelWasPressed)
			{
				this.Close();
				Singleton<UIInputHandler>.Instance.cancelWasPressed = false;
				return;
			}
		}
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x00031EDF File Offset: 0x000300DF
	protected virtual void Initialize()
	{
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x00031EE4 File Offset: 0x000300E4
	internal virtual void Open()
	{
		Debug.Log("opening window", base.gameObject);
		this.isOpen = true;
		if (!MenuWindow.AllActiveWindows.Contains(this))
		{
			MenuWindow.AllActiveWindows.Add(this);
		}
		this.Show();
		if (!this.initialized)
		{
			this.Initialize();
			this.initialized = true;
		}
		this.OnOpen();
		if (this.selectOnOpen)
		{
			this.SelectStartingElement();
		}
		this.SetInputActive(true);
		if (GUIManager.instance != null)
		{
			GUIManager.instance.TriggerMenuWindowOpened(this);
		}
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00031F6E File Offset: 0x0003016E
	protected virtual void OnOpen()
	{
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x00031F70 File Offset: 0x00030170
	private void OnDestroy()
	{
		if (MenuWindow.AllActiveWindows.Contains(this))
		{
			MenuWindow.AllActiveWindows.Remove(this);
		}
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x00031F8C File Offset: 0x0003018C
	public static void CloseAllWindows()
	{
		for (int i = MenuWindow.AllActiveWindows.Count - 1; i >= 0; i--)
		{
			if (MenuWindow.AllActiveWindows[i] != null)
			{
				MenuWindow.AllActiveWindows[i].ForceClose();
			}
		}
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x00031FD3 File Offset: 0x000301D3
	internal void StartClosed()
	{
		this.isOpen = false;
		this.SetInputActive(false);
		this.panel.SetActive(false);
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x00031FF0 File Offset: 0x000301F0
	internal void Close()
	{
		Debug.Log(base.gameObject.name + " closing.");
		this.isOpen = false;
		if (MenuWindow.AllActiveWindows.Contains(this))
		{
			MenuWindow.AllActiveWindows.Remove(this);
		}
		this.OnClose();
		this.SetInputActive(false);
		if (this.autoHideOnClose)
		{
			this.Hide();
		}
		if (GUIManager.instance != null)
		{
			GUIManager.instance.TriggerMenuWindowClosed(this);
		}
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x0003206A File Offset: 0x0003026A
	internal void ForceClose()
	{
		this.Close();
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x00032072 File Offset: 0x00030272
	protected virtual void OnClose()
	{
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x00032074 File Offset: 0x00030274
	public void Show()
	{
		this.panel.SetActive(true);
	}

	// Token: 0x06000A21 RID: 2593 RVA: 0x00032082 File Offset: 0x00030282
	public void Hide()
	{
		this.panel.SetActive(false);
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x00032090 File Offset: 0x00030290
	public void SetInputActive(bool active)
	{
		this.inputActive = active;
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x00032099 File Offset: 0x00030299
	private void SelectStartingElement()
	{
		UIInputHandler.SetSelectedObject((this.objectToSelectOnOpen == null) ? null : this.objectToSelectOnOpen.gameObject);
	}

	// Token: 0x040008F7 RID: 2295
	public static List<MenuWindow> AllActiveWindows = new List<MenuWindow>();
}
