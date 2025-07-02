using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Chat.UtilityScripts
{
	// Token: 0x020002CE RID: 718
	[RequireComponent(typeof(Text))]
	public class TextButtonTransition : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x060011C1 RID: 4545 RVA: 0x000575A9 File Offset: 0x000557A9
		public void Awake()
		{
			this._text = base.GetComponent<Text>();
		}

		// Token: 0x060011C2 RID: 4546 RVA: 0x000575B7 File Offset: 0x000557B7
		public void OnEnable()
		{
			this._text.color = this.NormalColor;
		}

		// Token: 0x060011C3 RID: 4547 RVA: 0x000575CA File Offset: 0x000557CA
		public void OnDisable()
		{
			this._text.color = this.NormalColor;
		}

		// Token: 0x060011C4 RID: 4548 RVA: 0x000575DD File Offset: 0x000557DD
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.Selectable == null || this.Selectable.IsInteractable())
			{
				this._text.color = this.HoverColor;
			}
		}

		// Token: 0x060011C5 RID: 4549 RVA: 0x0005760B File Offset: 0x0005580B
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.Selectable == null || this.Selectable.IsInteractable())
			{
				this._text.color = this.NormalColor;
			}
		}

		// Token: 0x0400101C RID: 4124
		private Text _text;

		// Token: 0x0400101D RID: 4125
		public Selectable Selectable;

		// Token: 0x0400101E RID: 4126
		public Color NormalColor = Color.white;

		// Token: 0x0400101F RID: 4127
		public Color HoverColor = Color.black;
	}
}
