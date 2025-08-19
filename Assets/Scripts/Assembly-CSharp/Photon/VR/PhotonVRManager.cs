using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.VR.Cosmetics;
using Photon.VR.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Photon.VR
{
	public class PhotonVRManager : MonoBehaviourPunCallbacks
	{
		[Header("Photon")]
		public string AppId;

		public string VoiceAppId;

		[Tooltip("Please read https://doc.photonengine.com/en-us/pun/current/connection-and-authentication/regions for more information")]
		public string Region = "eu";

		[Header("Player")]
		public Transform Head;

		public Transform LeftHand;

		public Transform RightHand;

		public Color Colour;

		[Header("Networking")]
		public string DefaultQueue = "Default";

		public int DefaultRoomLimit = 16;

		[Header("Other")]
		[Tooltip("If the user shall connect when this object has awoken")]
		public bool ConnectOnAwake = true;

		[Tooltip("If the user shall join a room when they connect")]
		public bool JoinRoomOnConnect = true;

		[NonSerialized]
		public PhotonVRPlayer LocalPlayer;

		private RoomOptions options;

		private ConnectionState State;

		public static PhotonVRManager Manager { get; private set; }

		public PhotonVRCosmeticsData Cosmetics { get; private set; } = new PhotonVRCosmeticsData();

		private void Start()
		{
			if (Manager == null)
			{
				Manager = this;
			}
			else
			{
				Debug.LogError("There can't be multiple PhotonVRManagers in a scene");
				Application.Quit();
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (ConnectOnAwake)
			{
				Connect();
			}
			if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Colour")))
			{
				Colour = JsonUtility.FromJson<Color>(PlayerPrefs.GetString("Colour"));
			}
			if (!string.IsNullOrEmpty(PlayerPrefs.GetString("Cosmetics")))
			{
				Cosmetics = JsonUtility.FromJson<PhotonVRCosmeticsData>(PlayerPrefs.GetString("Cosmetics"));
			}
		}

		public static bool Connect()
		{
			if (string.IsNullOrEmpty(Manager.AppId) || string.IsNullOrEmpty(Manager.VoiceAppId))
			{
				Debug.LogError("Please input an app id");
				return false;
			}
			PhotonNetwork.AuthValues = null;
			Manager.State = ConnectionState.Connecting;
			PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = Manager.AppId;
			PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice = Manager.VoiceAppId;
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = Manager.Region;
			PhotonNetwork.ConnectUsingSettings();
			Debug.Log("Connecting - AppId: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime + " VoiceAppId: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice);
			return true;
		}

		public static bool ConnectAuthenticated(string username, string token)
		{
			if (string.IsNullOrEmpty(Manager.AppId) || string.IsNullOrEmpty(Manager.VoiceAppId))
			{
				Debug.LogError("Please input an app id");
				return false;
			}
			AuthenticationValues authenticationValues = new AuthenticationValues();
			authenticationValues.AuthType = CustomAuthenticationType.Custom;
			authenticationValues.AddAuthParameter("username", username);
			authenticationValues.AddAuthParameter("token", token);
			PhotonNetwork.AuthValues = authenticationValues;
			Manager.State = ConnectionState.Connecting;
			PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = Manager.AppId;
			PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice = Manager.VoiceAppId;
			PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = Manager.Region;
			PhotonNetwork.ConnectUsingSettings();
			Debug.Log("Connecting - AppId: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime + " VoiceAppId: " + PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice);
			return true;
		}

		public void Disconnect()
		{
			PhotonNetwork.Disconnect();
		}

		public static void ChangeServers(string Id, string VoiceId)
		{
			PhotonNetwork.Disconnect();
			Manager.AppId = Id;
			Manager.VoiceAppId = VoiceId;
			Connect();
		}

		public static void ChangeServersAuthenticated(string Id, string VoiceId, string username, string token)
		{
			PhotonNetwork.Disconnect();
			Manager.AppId = Id;
			Manager.VoiceAppId = VoiceId;
			ConnectAuthenticated(username, token);
		}

		public static void SetUsername(string Name)
		{
			PhotonNetwork.LocalPlayer.NickName = Name;
			PlayerPrefs.SetString("Username", Name);
			if (PhotonNetwork.InRoom && Manager.LocalPlayer != null)
			{
				Manager.LocalPlayer.RefreshPlayerValues();
			}
		}

		public static void SetColour(Color PlayerColour)
		{
			Manager.Colour = PlayerColour;
			Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
			customProperties["Colour"] = JsonUtility.ToJson(PlayerColour);
			PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
			PlayerPrefs.SetString("Colour", JsonUtility.ToJson(PlayerColour));
			if (PhotonNetwork.InRoom && Manager.LocalPlayer != null)
			{
				Manager.LocalPlayer.RefreshPlayerValues();
			}
		}

		public static void SetCosmetics(PhotonVRCosmeticsData PlayerCosmetics)
		{
			Manager.Cosmetics = PlayerCosmetics;
			Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
			customProperties["Cosmetics"] = JsonUtility.ToJson(PlayerCosmetics);
			PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
			PlayerPrefs.SetString("Cosmetics", JsonUtility.ToJson(PlayerCosmetics));
			if (PhotonNetwork.InRoom && Manager.LocalPlayer != null)
			{
				Manager.LocalPlayer.RefreshPlayerValues();
			}
		}

		public static void SetCosmetic(CosmeticType Type, string CosmeticId)
		{
			PhotonVRCosmeticsData cosmetics = Manager.Cosmetics;
			switch (Type)
			{
			case CosmeticType.Head:
				cosmetics.Head = CosmeticId;
				break;
			case CosmeticType.Face:
				cosmetics.Face = CosmeticId;
				break;
			case CosmeticType.Body:
				cosmetics.Body = CosmeticId;
				break;
			case CosmeticType.BothHands:
				cosmetics.LeftHand = CosmeticId;
				cosmetics.RightHand = CosmeticId;
				break;
			case CosmeticType.LeftHand:
				cosmetics.LeftHand = CosmeticId;
				break;
			case CosmeticType.RightHand:
				cosmetics.RightHand = CosmeticId;
				break;
			}
			Manager.Cosmetics = cosmetics;
			Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
			customProperties["Cosmetics"] = JsonUtility.ToJson(cosmetics);
			PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
			PlayerPrefs.SetString("Cosmetics", JsonUtility.ToJson(cosmetics));
			if (PhotonNetwork.InRoom && Manager.LocalPlayer != null)
			{
				Manager.LocalPlayer.RefreshPlayerValues();
			}
		}

		public override void OnConnectedToMaster()
		{
			State = ConnectionState.Connected;
			Debug.Log("Connected");
			PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("Username");
			PhotonNetwork.LocalPlayer.CustomProperties["Colour"] = JsonUtility.ToJson(Colour);
			PhotonNetwork.LocalPlayer.CustomProperties["Cosmetics"] = JsonUtility.ToJson(Cosmetics);
			if (JoinRoomOnConnect)
			{
				JoinRandomRoom(DefaultQueue, DefaultRoomLimit);
			}
		}

		public static ConnectionState GetConnectionState()
		{
			return Manager.State;
		}

		public static void SwitchScenes(int SceneIndex, int MaxPlayers)
		{
			SceneManager.LoadScene(SceneIndex);
			JoinRandomRoom(SceneIndex.ToString(), MaxPlayers);
		}

		public static void SwitchScenes(int SceneIndex)
		{
			SceneManager.LoadScene(SceneIndex);
			JoinRandomRoom(SceneIndex.ToString(), Manager.DefaultRoomLimit);
		}

		public static void JoinRandomRoom(string Queue, int MaxPlayers)
		{
			_JoinRandomRoom(Queue, MaxPlayers);
		}

		public static void JoinRandomRoom(string Queue)
		{
			_JoinRandomRoom(Queue, Manager.DefaultRoomLimit);
		}

		private static void _JoinRandomRoom(string Queue, int MaxPlayers)
		{
			Manager.State = ConnectionState.JoiningRoom;
			Hashtable hashtable = new Hashtable();
			hashtable.Add("queue", Queue);
			hashtable.Add("version", Application.version);
			RoomOptions roomOptions = new RoomOptions();
			roomOptions.MaxPlayers = (byte)MaxPlayers;
			roomOptions.IsVisible = true;
			roomOptions.IsOpen = true;
			roomOptions.CustomRoomProperties = hashtable;
			roomOptions.CustomRoomPropertiesForLobby = new string[2] { "queue", "version" };
			Manager.options = roomOptions;
			PhotonNetwork.JoinRandomRoom(hashtable, (byte)roomOptions.MaxPlayers, MatchmakingMode.RandomMatching, null, null);
			Debug.Log(string.Format("Joining random with type {0}", hashtable["queue"]));
		}

		public static void JoinPrivateRoom(string RoomId, int MaxPlayers)
		{
			_JoinPrivateRoom(RoomId, MaxPlayers);
		}

		public static void JoinPrivateRoom(string RoomId)
		{
			_JoinPrivateRoom(RoomId, Manager.DefaultRoomLimit);
		}

		public static void _JoinPrivateRoom(string RoomId, int MaxPlayers)
		{
			PhotonNetwork.JoinOrCreateRoom(RoomId, new RoomOptions
			{
				IsVisible = false,
				IsOpen = true,
				MaxPlayers = (byte)MaxPlayers
			}, null);
			Debug.Log("Joining a private room: " + RoomId);
		}

		public override void OnJoinedRoom()
		{
			Debug.Log("Joined a room");
			State = ConnectionState.InRoom;
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
			base.OnDisconnected(cause);
			State = ConnectionState.Disconnected;
			Debug.Log("Disconnected from server");
		}

		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			HandleJoinError();
		}

		private void HandleJoinError()
		{
			Debug.Log("Failed to join room - creating a new one");
			string text = CreateRoomCode();
			Debug.Log("Joining " + text);
			PhotonNetwork.CreateRoom(text, options);
		}

		public string CreateRoomCode()
		{
			return new System.Random().Next(99999).ToString();
		}
	}
}
