using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	[RequireComponent(typeof(Toggle))]
	[DisallowMultipleComponent]
	public class BetterToggle : MonoBehaviour
	{
		public delegate void OnToggle(Toggle toggle);

		private Toggle toggle;

		public static event OnToggle ToggleValueChanged;

		private void Start()
		{
			toggle = GetComponent<Toggle>();
			toggle.onValueChanged.AddListener(delegate
			{
				OnToggleValueChanged();
			});
		}

		public void OnToggleValueChanged()
		{
			if (BetterToggle.ToggleValueChanged != null)
			{
				BetterToggle.ToggleValueChanged(toggle);
			}
		}
	}
}
