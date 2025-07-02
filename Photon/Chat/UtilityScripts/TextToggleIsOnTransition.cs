using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Chat.UtilityScripts
{
	// Token: 0x020002CF RID: 719
	[RequireComponent(typeof(Text))]
	public class TextToggleIsOnTransition : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x060011C7 RID: 4551 RVA: 0x00057657 File Offset: 0x00055857
		public void OnEnable()
		{
			this._text = base.GetComponent<Text>();
			this.OnValueChanged(this.toggle.isOn);
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged));
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x00057692 File Offset: 0x00055892
		public void OnDisable()
		{
			this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnValueChanged));
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x000576B0 File Offset: 0x000558B0
		public void OnValueChanged(bool isOn)
		{
			this._text.color = (isOn ? (this.isHover ? this.HoverOnColor : this.HoverOnColor) : (this.isHover ? this.NormalOffColor : this.NormalOffColor));
		}

		// Token: 0x060011CA RID: 4554 RVA: 0x000576EE File Offset: 0x000558EE
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.isHover = true;
			this._text.color = (this.toggle.isOn ? this.HoverOnColor : this.HoverOffColor);
		}

		// Token: 0x060011CB RID: 4555 RVA: 0x0005771D File Offset: 0x0005591D
		public void OnPointerExit(PointerEventData eventData)
		{
			this.isHover = false;
			this._text.color = (this.toggle.isOn ? this.NormalOnColor : this.NormalOffColor);
		}

		// Token: 0x04001020 RID: 4128
		public Toggle toggle;

		// Token: 0x04001021 RID: 4129
		private Text _text;

		// Token: 0x04001022 RID: 4130
		public Color NormalOnColor = Color.white;

		// Token: 0x04001023 RID: 4131
		public Color NormalOffColor = Color.black;

		// Token: 0x04001024 RID: 4132
		public Color HoverOnColor = Color.black;

		// Token: 0x04001025 RID: 4133
		public Color HoverOffColor = Color.black;

		// Token: 0x04001026 RID: 4134
		private bool isHover;
	}
}
