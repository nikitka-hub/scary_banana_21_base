using UnityEngine;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos
{
	public class BackgroundMusicController : MonoBehaviour
	{
		[SerializeField]
		private Text volumeText;

		[SerializeField]
		private Slider volumeSlider;

		[SerializeField]
		private AudioSource audioSource;

		[SerializeField]
		private float initialVolume = 0.125f;

		private void Awake()
		{
			volumeSlider.minValue = 0f;
			volumeSlider.maxValue = 1f;
			volumeSlider.SetSingleOnValueChangedCallback(OnVolumeChanged);
			volumeSlider.value = initialVolume;
			OnVolumeChanged(initialVolume);
		}

		private void OnVolumeChanged(float newValue)
		{
			audioSource.volume = newValue;
		}
	}
}
