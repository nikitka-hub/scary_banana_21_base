using Photon.Voice.PUN;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	[RequireComponent(typeof(PhotonVoiceView))]
	public class PointersController : MonoBehaviour
	{
		[SerializeField]
		private GameObject pointerDown;

		[SerializeField]
		private GameObject pointerUp;

		private PhotonVoiceView photonVoiceView;

		private void Awake()
		{
			photonVoiceView = GetComponent<PhotonVoiceView>();
			SetActiveSafe(pointerUp, active: false);
			SetActiveSafe(pointerDown, active: false);
		}

		private void Update()
		{
			SetActiveSafe(pointerDown, photonVoiceView.IsSpeaking);
			SetActiveSafe(pointerUp, photonVoiceView.IsRecording);
		}

		private void SetActiveSafe(GameObject go, bool active)
		{
			if (go != null && go.activeSelf != active)
			{
				go.SetActive(active);
			}
		}
	}
}
