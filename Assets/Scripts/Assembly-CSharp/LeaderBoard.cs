using System.Collections;
using System.Text;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.VR.Player;
using Photon.Voice.PUN;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PhotonView))]
public class LeaderBoard : MonoBehaviour
{
	[SerializeField]
	public TMP_Text[] displaySpot;

	[SerializeField]
	public Renderer[] ColorSpot;

	[SerializeField]
	public string WebHookURL;

	[SerializeField]
	public Playfablogin playfablogin;

	private bool hashed;

	private bool Kicked;

	private void Start()
	{
		if (GetComponent<PhotonView>().OwnershipTransfer != OwnershipOption.Takeover)
		{
			GetComponent<PhotonView>().OwnershipTransfer = OwnershipOption.Takeover;
		}
	}

	private void Update()
	{
		if (PhotonNetwork.IsConnected && !hashed)
		{
			ExitGames.Client.Photon.Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
			customProperties["PlayfabID"] = playfablogin.MyPlayFabID;
			PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
			hashed = true;
		}
		for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
		{
			if (!Kicked)
			{
				displaySpot[i].text = PhotonNetwork.PlayerList[i].NickName;
				PhotonVRPlayer[] array = Object.FindObjectsOfType<PhotonVRPlayer>();
				foreach (PhotonVRPlayer photonVRPlayer in array)
				{
					if (photonVRPlayer.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.PlayerList[i])
					{
						ColorSpot[i].material.color = JsonUtility.FromJson<Color>((string)photonVRPlayer.gameObject.GetComponent<PhotonView>().Owner.CustomProperties["Colour"]);
					}
				}
			}
			else
			{
				if (PhotonNetwork.IsConnected)
				{
					PhotonNetwork.Disconnect();
				}
				displaySpot[i].color = Color.red;
				displaySpot[i].text = "You have been Kicked";
			}
		}
		for (int k = 0; k < displaySpot.Length; k++)
		{
			if (k > PhotonNetwork.PlayerList.Length)
			{
				displaySpot[k].text = null;
				ColorSpot[k].material.color = Color.white;
			}
		}
	}

	public void MutePress(int ButtonNumber)
	{
		if (PhotonNetwork.PlayerList.Length < ButtonNumber - 1)
		{
			return;
		}
		PhotonVRPlayer[] array = Object.FindObjectsOfType<PhotonVRPlayer>();
		foreach (PhotonVRPlayer photonVRPlayer in array)
		{
			if (photonVRPlayer.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.PlayerList[ButtonNumber - 1])
			{
				AudioSource component = photonVRPlayer.gameObject.GetComponent<PhotonVoiceView>().SpeakerInUse.gameObject.GetComponent<AudioSource>();
				component.mute = !component.mute;
				break;
			}
		}
	}

	public void KickPress(int ButtonNumber)
	{
		if (PhotonNetwork.PlayerList.Length < ButtonNumber - 1)
		{
			return;
		}
		PhotonVRPlayer[] array = Object.FindObjectsOfType<PhotonVRPlayer>();
		foreach (PhotonVRPlayer photonVRPlayer in array)
		{
			if (photonVRPlayer.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.PlayerList[ButtonNumber - 1])
			{
				GetComponent<PhotonView>().RequestOwnership();
				GetComponent<PhotonView>().RPC("KickPlayer", photonVRPlayer.gameObject.GetComponent<PhotonView>().Owner);
			}
		}
	}

	[PunRPC]
	private void KickPlayer()
	{
		Kicked = true;
	}

	public void Report(int ButtonNumber)
	{
		_ = playfablogin.MyPlayFabID;
		if (PhotonNetwork.PlayerList.Length < ButtonNumber - 1)
		{
			return;
		}
		PhotonVRPlayer[] array = Object.FindObjectsOfType<PhotonVRPlayer>();
		foreach (PhotonVRPlayer photonVRPlayer in array)
		{
			if (photonVRPlayer.gameObject.GetComponent<PhotonView>().Owner == PhotonNetwork.PlayerList[ButtonNumber - 1])
			{
				SendtoWebhook(PhotonNetwork.PlayerList[ButtonNumber - 1].NickName + " " + (string)photonVRPlayer.gameObject.GetComponent<PhotonView>().Owner.CustomProperties["PlayfabID"] + " was reported by " + PlayerPrefs.GetString("Username", null) + playfablogin.MyPlayFabID);
			}
		}
	}

	public void SendtoWebhook(string message)
	{
		StartCoroutine(PostToDiscord(message));
	}

	private IEnumerator PostToDiscord(string message)
	{
		string s = "{\"content\": \"" + message + "\"}";
		UnityWebRequest www = new UnityWebRequest(WebHookURL, "POST");
		byte[] bytes = new UTF8Encoding().GetBytes(s);
		www.uploadHandler = new UploadHandlerRaw(bytes);
		www.downloadHandler = new DownloadHandlerBuffer();
		www.SetRequestHeader("Content-Type", "application/json");
		yield return www.SendWebRequest();
		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError("Reporting Webhook Error: " + www.error);
		}
	}
}
