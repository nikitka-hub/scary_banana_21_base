using Photon.Voice.Unity.Demos.DemoVoiceUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos
{
	public static class UiExtensions
	{
		public static void SetPosX(this RectTransform rectTransform, float x)
		{
			rectTransform.anchoredPosition3D = new Vector3(x, rectTransform.anchoredPosition3D.y, rectTransform.anchoredPosition3D.z);
		}

		public static void SetHeight(this RectTransform rectTransform, float h)
		{
			rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
		}

		public static void SetValue(this Toggle toggle, bool isOn)
		{
			toggle.SetIsOnWithoutNotify(isOn);
		}

		public static void SetValue(this Slider slider, float v)
		{
			slider.SetValueWithoutNotify(v);
		}

		public static void SetValue(this InputField inputField, string v)
		{
			inputField.SetTextWithoutNotify(v);
		}

		public static void DestroyChildren(this Transform transform)
		{
			if (!(null != transform) || !transform)
			{
				return;
			}
			for (int num = transform.childCount - 1; num >= 0; num--)
			{
				Transform child = transform.GetChild(num);
				if ((bool)child && (bool)child.gameObject)
				{
					Object.Destroy(child.gameObject);
				}
			}
			transform.DetachChildren();
		}

		public static void Hide(this CanvasGroup canvasGroup, bool blockRaycasts = false, bool interactable = false)
		{
			canvasGroup.alpha = 0f;
			canvasGroup.blocksRaycasts = blockRaycasts;
			canvasGroup.interactable = interactable;
		}

		public static void Show(this CanvasGroup canvasGroup, bool blockRaycasts = true, bool interactable = true)
		{
			canvasGroup.alpha = 1f;
			canvasGroup.blocksRaycasts = blockRaycasts;
			canvasGroup.interactable = interactable;
		}

		public static bool IsHidden(this CanvasGroup canvasGroup)
		{
			return canvasGroup.alpha <= 0f;
		}

		public static bool IsShown(this CanvasGroup canvasGroup)
		{
			return canvasGroup.alpha > 0f;
		}

		public static void SetSingleOnClickCallback(this Button button, UnityAction action)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(action);
		}

		public static void SetSingleOnValueChangedCallback(this Toggle toggle, UnityAction<bool> action)
		{
			toggle.onValueChanged.RemoveAllListeners();
			toggle.onValueChanged.AddListener(action);
		}

		public static void SetSingleOnValueChangedCallback(this InputField inputField, UnityAction<string> action)
		{
			inputField.onValueChanged.RemoveAllListeners();
			inputField.onValueChanged.AddListener(action);
		}

		public static void SetSingleOnEndEditCallback(this InputField inputField, UnityAction<string> action)
		{
			inputField.onEndEdit.RemoveAllListeners();
			inputField.onEndEdit.AddListener(action);
		}

		public static void SetSingleOnValueChangedCallback(this Dropdown inputField, UnityAction<int> action)
		{
			inputField.onValueChanged.RemoveAllListeners();
			inputField.onValueChanged.AddListener(action);
		}

		public static void SetSingleOnValueChangedCallback(this Slider slider, UnityAction<float> action)
		{
			slider.onValueChanged.RemoveAllListeners();
			slider.onValueChanged.AddListener(action);
		}

		public static void SetSingleOnValueChangedCallback(this MicrophoneSelector selector, UnityAction<MicType, DeviceInfo> action)
		{
			selector.onValueChanged.RemoveAllListeners();
			selector.onValueChanged.AddListener(action);
		}
	}
}
