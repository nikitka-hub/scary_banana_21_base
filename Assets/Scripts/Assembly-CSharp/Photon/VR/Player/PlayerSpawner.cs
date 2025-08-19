using Photon.Pun;
using UnityEngine;

namespace Photon.VR.Player
{
	public class PlayerSpawner : MonoBehaviourPunCallbacks
	{
		[Tooltip("The location of the player prefab")]
		public string PrefabLocation = "PhotonVR/Player";

		private GameObject playerTemp;

		private void Awake()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}

		public override void OnJoinedRoom()
		{
			playerTemp = PhotonNetwork.Instantiate(PrefabLocation, Vector3.zero, Quaternion.identity, 0);
		}

		public override void OnLeftRoom()
		{
			PhotonNetwork.Destroy(playerTemp);
		}
	}
}
