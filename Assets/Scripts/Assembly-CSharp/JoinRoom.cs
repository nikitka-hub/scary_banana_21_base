using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class JoinRoom : MonoBehaviourPunCallbacks
{
	[Header("THIS SCRIPT WAS MADE BY FLIMCYVR. IT IS NOT YOURS.")]
	[Header("Distributing This Script Will Lead To A Permanent Ban and MORE!")]
	[Header("If you make a video on this script")]
	[Header("credit me with my discord and youtube")]
	public new string name = "";

	public bool pressed;

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			if (PhotonNetwork.InRoom)
			{
				Debug.Log("farded");
				PhotonNetwork.LeaveRoom();
			}
			Debug.Log(name);
			pressed = true;
		}
	}

	private void FixedUpdate()
	{
		if (pressed && PhotonNetwork.IsConnectedAndReady)
		{
			pressed = false;
			OnLeftRoom();
		}
	}

	public override void OnConnectedToMaster()
	{
		if (pressed && PhotonNetwork.IsConnectedAndReady)
		{
			pressed = false;
			OnLeftRoom();
		}
	}

	private new void OnLeftRoom()
	{
		PhotonNetwork.JoinOrCreateRoom(name, new RoomOptions
		{
			MaxPlayers = 10,
			IsVisible = false,
			IsOpen = true
		}, null);
	}
}
