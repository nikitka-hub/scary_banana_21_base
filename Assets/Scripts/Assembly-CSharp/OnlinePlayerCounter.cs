using Photon.Pun;
using TMPro;
using UnityEngine;

public class OnlinePlayerCounter : MonoBehaviour
{
	private TMP_Text playerCountText;

	private void Start()
	{
		playerCountText = GetComponent<TMP_Text>();
	}

	private void Update()
	{
		if (PhotonNetwork.IsConnected)
		{
			int countOfPlayers = PhotonNetwork.CountOfPlayers;
			playerCountText.text = "Online Players: " + countOfPlayers;
		}
	}
}
