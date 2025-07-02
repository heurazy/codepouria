using System;
using Photon.Voice.Unity.Demos.DemoVoiceUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos
{
	// Token: 0x020002BC RID: 700
	public static class UiExtensions
	{
		// Token: 0x060010F6 RID: 4342 RVA: 0x0005432A File Offset: 0x0005252A
		public static void SetPosX(this RectTransform rectTransform, float x)
		{
			rectTransform.anchoredPosition3D = new Vector3(x, rectTransform.anchoredPosition3D.y, rectTransform.anchoredPosition3D.z);
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x0005434E File Offset: 0x0005254E
		public static void SetHeight(this RectTransform rectTransform, float h)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x00054358 File Offset: 0x00052558
		public static void SetValue(this Toggle toggle, bool isOn)
		{
			toggle.SetIsOnWithoutNotify(isOn);
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x00054361 File Offset: 0x00052561
		public static void SetValue(this Slider slider, float v)
		{
			slider.SetValueWithoutNotify(v);
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x0005436A File Offset: 0x0005256A
		public static void SetValue(this InputField inputField, string v)
		{
			inputField.SetTextWithoutNotify(v);
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x00054374 File Offset: 0x00052574
		public static void DestroyChildren(this Transform transform)
		{
			if (null != transform && transform)
			{
				for (int i = transform.childCount - 1; i >= 0; i--)
				{
					Transform child = transform.GetChild(i);
					if (child && child.gameObject)
					{
						Object.Destroy(child.gameObject);
					}
				}
				transform.DetachChildren();
			}
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x000543D3 File Offset: 0x000525D3
		public static void Hide(this CanvasGroup canvasGroup, bool blockRaycasts = false, bool interactable = false)
		{
			canvasGroup.alpha = 0f;
			canvasGroup.blocksRaycasts = blockRaycasts;
			canvasGroup.interactable = interactable;
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x000543EE File Offset: 0x000525EE
		public static void Show(this CanvasGroup canvasGroup, bool blockRaycasts = true, bool interactable = true)
		{
			canvasGroup.alpha = 1f;
			canvasGroup.blocksRaycasts = blockRaycasts;
			canvasGroup.interactable = interactable;
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x00054409 File Offset: 0x00052609
		public static bool IsHidden(this CanvasGroup canvasGroup)
		{
			return canvasGroup.alpha <= 0f;
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x0005441B File Offset: 0x0005261B
		public static bool IsShown(this CanvasGroup canvasGroup)
		{
			return canvasGroup.alpha > 0f;
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x0005442A File Offset: 0x0005262A
		public static void SetSingleOnClickCallback(this Button button, UnityAction action)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(action);
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x00054443 File Offset: 0x00052643
		public static void SetSingleOnValueChangedCallback(this Toggle toggle, UnityAction<bool> action)
		{
			toggle.onValueChanged.RemoveAllListeners();
			toggle.onValueChanged.AddListener(action);
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x0005445C File Offset: 0x0005265C
		public static void SetSingleOnValueChangedCallback(this InputField inputField, UnityAction<string> action)
		{
			inputField.onValueChanged.RemoveAllListeners();
			inputField.onValueChanged.AddListener(action);
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x00054475 File Offset: 0x00052675
		public static void SetSingleOnEndEditCallback(this InputField inputField, UnityAction<string> action)
		{
			inputField.onEndEdit.RemoveAllListeners();
			inputField.onEndEdit.AddListener(action);
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x0005448E File Offset: 0x0005268E
		public static void SetSingleOnValueChangedCallback(this Dropdown inputField, UnityAction<int> action)
		{
			inputField.onValueChanged.RemoveAllListeners();
			inputField.onValueChanged.AddListener(action);
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x000544A7 File Offset: 0x000526A7
		public static void SetSingleOnValueChangedCallback(this Slider slider, UnityAction<float> action)
		{
			slider.onValueChanged.RemoveAllListeners();
			slider.onValueChanged.AddListener(action);
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x000544C0 File Offset: 0x000526C0
		public static void SetSingleOnValueChangedCallback(this MicrophoneSelector selector, UnityAction<MicType, DeviceInfo> action)
		{
			selector.onValueChanged.RemoveAllListeners();
			selector.onValueChanged.AddListener(action);
		}
	}
}
