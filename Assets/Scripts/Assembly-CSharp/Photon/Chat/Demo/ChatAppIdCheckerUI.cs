using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Chat.Demo
{
	[ExecuteInEditMode]
	public class ChatAppIdCheckerUI : MonoBehaviour
	{
		public Text Description;

		public bool WizardOpenedOnce;

		public void Update()
		{
			string text = string.Empty;
			if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
			{
				text = "<Color=Red>WARNING:</Color>\nPlease setup a Chat AppId in the PhotonServerSettings file.";
			}
			Description.text = text;
		}
	}
}
