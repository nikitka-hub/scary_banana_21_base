using UnityEngine;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos
{
	public class SidebarToggle : MonoBehaviour
	{
		[SerializeField]
		private Button sidebarButton;

		[SerializeField]
		private RectTransform panelsHolder;

		private float sidebarWidth = 300f;

		private bool sidebarOpen = true;

		private void Awake()
		{
			sidebarButton.onClick.RemoveAllListeners();
			sidebarButton.onClick.AddListener(ToggleSidebar);
			ToggleSidebar(sidebarOpen);
		}

		[ContextMenu("ToggleSidebar")]
		private void ToggleSidebar()
		{
			sidebarOpen = !sidebarOpen;
			ToggleSidebar(sidebarOpen);
		}

		private void ToggleSidebar(bool open)
		{
			if (!open)
			{
				panelsHolder.SetPosX(0f);
			}
			else
			{
				panelsHolder.SetPosX(sidebarWidth);
			}
		}
	}
}
