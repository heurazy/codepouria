using System;
using UnityEngine;

// Token: 0x020001B2 RID: 434
public class CursorAnimation : MonoBehaviour
{
	// Token: 0x06000BF6 RID: 3062 RVA: 0x0003BEBE File Offset: 0x0003A0BE
	private void Start()
	{
		Cursor.SetCursor(this.cursorOpen, this.cursorHotspot, CursorMode.Auto);
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x0003BED2 File Offset: 0x0003A0D2
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Cursor.SetCursor(this.curserClosed, this.cursorHotspot, CursorMode.Auto);
			return;
		}
		if (Input.GetMouseButtonUp(0))
		{
			Cursor.SetCursor(this.cursorOpen, this.cursorHotspot, CursorMode.Auto);
		}
	}

	// Token: 0x04000AD8 RID: 2776
	public Texture2D cursorOpen;

	// Token: 0x04000AD9 RID: 2777
	public Texture2D curserClosed;

	// Token: 0x04000ADA RID: 2778
	private Vector2 cursorHotspot = new Vector2(32f, 32f);
}
