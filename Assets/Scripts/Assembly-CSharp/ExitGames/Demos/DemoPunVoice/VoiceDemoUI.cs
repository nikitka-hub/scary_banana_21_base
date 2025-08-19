using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice
{
	public class VoiceDemoUI : MonoBehaviour
	{
		public delegate void OnDebugToggle(bool debugMode);

		[SerializeField]
		private Text punState;

		[SerializeField]
		private Text voiceState;

		private PunVoiceClient punVoiceClient;

		private Canvas canvas;

		[SerializeField]
		private Button punSwitch;

		private Text punSwitchText;

		[SerializeField]
		private Button voiceSwitch;

		private Text voiceSwitchText;

		[SerializeField]
		private Button calibrateButton;

		private Text calibrateText;

		[SerializeField]
		private Text voiceDebugText;

		private PhotonVoiceView recorder;

		[SerializeField]
		private GameObject inGameSettings;

		[SerializeField]
		private GameObject globalSettings;

		[SerializeField]
		private Text devicesInfoText;

		private GameObject debugGO;

		private bool debugMode;

		private float volumeBeforeMute;

		private DebugLevel previousDebugLevel;

		[SerializeField]
		private int calibrationMilliSeconds = 2000;

		public bool DebugMode
		{
			get
			{
				return debugMode;
			}
			set
			{
				debugMode = value;
				debugGO.SetActive(debugMode);
				voiceDebugText.text = string.Empty;
				if (debugMode)
				{
					previousDebugLevel = punVoiceClient.Client.LoadBalancingPeer.DebugOut;
					punVoiceClient.Client.LoadBalancingPeer.DebugOut = DebugLevel.ALL;
				}
				else
				{
					punVoiceClient.Client.LoadBalancingPeer.DebugOut = previousDebugLevel;
				}
				if (VoiceDemoUI.DebugToggled != null)
				{
					VoiceDemoUI.DebugToggled(debugMode);
				}
			}
		}

		public static event OnDebugToggle DebugToggled;

		private void Awake()
		{
			punVoiceClient = PunVoiceClient.Instance;
			Debug.LogWarning("VoiceDemoUI selected a punVoiceClient.Instance", punVoiceClient);
		}

		private void OnDestroy()
		{
			ChangePOV.CameraChanged -= OnCameraChanged;
			BetterToggle.ToggleValueChanged -= BetterToggle_ToggleValueChanged;
			CharacterInstantiation.CharacterInstantiated -= CharacterInstantiation_CharacterInstantiated;
			punVoiceClient.Client.StateChanged -= VoiceClientStateChanged;
			PhotonNetwork.NetworkingClient.StateChanged -= PunClientStateChanged;
		}

		private void CharacterInstantiation_CharacterInstantiated(GameObject character)
		{
			PhotonVoiceView component = character.GetComponent<PhotonVoiceView>();
			if (component != null)
			{
				recorder = component;
			}
		}

		private void InitToggles(Toggle[] toggles)
		{
			if (toggles == null)
			{
				return;
			}
			foreach (Toggle toggle in toggles)
			{
				switch (toggle.name)
				{
				case "Mute":
					toggle.isOn = AudioListener.volume <= 0.001f;
					break;
				case "VoiceDetection":
					if (recorder != null && recorder.RecorderInUse != null)
					{
						toggle.isOn = recorder.RecorderInUse.VoiceDetection;
					}
					break;
				case "DebugVoice":
					toggle.isOn = DebugMode;
					break;
				case "Transmit":
					if (recorder != null && recorder.RecorderInUse != null)
					{
						toggle.isOn = recorder.RecorderInUse.TransmitEnabled;
					}
					break;
				case "DebugEcho":
					if (recorder != null && recorder.RecorderInUse != null)
					{
						toggle.isOn = recorder.RecorderInUse.DebugEchoMode;
					}
					break;
				case "AutoConnectAndJoin":
					toggle.isOn = punVoiceClient.AutoConnectAndJoin;
					break;
				}
			}
		}

		private void BetterToggle_ToggleValueChanged(Toggle toggle)
		{
			switch (toggle.name)
			{
			case "Mute":
				if (toggle.isOn)
				{
					volumeBeforeMute = AudioListener.volume;
					AudioListener.volume = 0f;
				}
				else
				{
					AudioListener.volume = volumeBeforeMute;
					volumeBeforeMute = 0f;
				}
				break;
			case "Transmit":
				if ((bool)recorder.RecorderInUse)
				{
					recorder.RecorderInUse.TransmitEnabled = toggle.isOn;
				}
				break;
			case "VoiceDetection":
				if ((bool)recorder.RecorderInUse)
				{
					recorder.RecorderInUse.VoiceDetection = toggle.isOn;
				}
				break;
			case "DebugEcho":
				if ((bool)recorder.RecorderInUse)
				{
					recorder.RecorderInUse.DebugEchoMode = toggle.isOn;
				}
				break;
			case "DebugVoice":
				DebugMode = toggle.isOn;
				break;
			case "AutoConnectAndJoin":
				punVoiceClient.AutoConnectAndJoin = toggle.isOn;
				break;
			}
		}

		private void OnCameraChanged(Camera newCamera)
		{
			canvas.worldCamera = newCamera;
		}

		private void Start()
		{
			ChangePOV.CameraChanged += OnCameraChanged;
			BetterToggle.ToggleValueChanged += BetterToggle_ToggleValueChanged;
			CharacterInstantiation.CharacterInstantiated += CharacterInstantiation_CharacterInstantiated;
			punVoiceClient.Client.StateChanged += VoiceClientStateChanged;
			PhotonNetwork.NetworkingClient.StateChanged += PunClientStateChanged;
			canvas = GetComponentInChildren<Canvas>();
			if (punSwitch != null)
			{
				punSwitchText = punSwitch.GetComponentInChildren<Text>();
				punSwitch.onClick.AddListener(PunSwitchOnClick);
			}
			if (voiceSwitch != null)
			{
				voiceSwitchText = voiceSwitch.GetComponentInChildren<Text>();
				voiceSwitch.onClick.AddListener(VoiceSwitchOnClick);
			}
			if (calibrateButton != null)
			{
				calibrateButton.onClick.AddListener(CalibrateButtonOnClick);
				calibrateText = calibrateButton.GetComponentInChildren<Text>();
			}
			if (punState != null)
			{
				debugGO = punState.transform.parent.gameObject;
			}
			volumeBeforeMute = AudioListener.volume;
			previousDebugLevel = punVoiceClient.Client.LoadBalancingPeer.DebugOut;
			if (globalSettings != null)
			{
				globalSettings.SetActive(value: true);
				InitToggles(globalSettings.GetComponentsInChildren<Toggle>());
			}
			if (devicesInfoText != null)
			{
				using AudioInEnumerator source = new AudioInEnumerator(punVoiceClient.Logger);
				using IDeviceEnumerator source2 = Platform.CreateAudioInEnumerator(punVoiceClient.Logger);
				if (source.Count() + source2.Count() == 0)
				{
					devicesInfoText.enabled = true;
					devicesInfoText.color = Color.red;
					devicesInfoText.text = "No microphone device detected!";
				}
				else
				{
					devicesInfoText.text = "Mic Unity: " + string.Join(", ", source.Select((DeviceInfo x) => x.ToString()));
					Text text = devicesInfoText;
					text.text = text.text + "\nMic Photon: " + string.Join(", ", source2.Select((DeviceInfo x) => x.ToString()));
				}
			}
			VoiceClientStateChanged(ClientState.PeerCreated, punVoiceClient.ClientState);
			PunClientStateChanged(ClientState.PeerCreated, PhotonNetwork.NetworkingClient.State);
		}

		private void PunSwitchOnClick()
		{
			if (PhotonNetwork.NetworkClientState == ClientState.Joined)
			{
				PhotonNetwork.Disconnect();
			}
			else if (PhotonNetwork.NetworkClientState == ClientState.Disconnected || PhotonNetwork.NetworkClientState == ClientState.PeerCreated)
			{
				PhotonNetwork.ConnectUsingSettings();
			}
		}

		private void VoiceSwitchOnClick()
		{
			if (punVoiceClient.ClientState == ClientState.Joined)
			{
				punVoiceClient.Disconnect();
			}
			else if (punVoiceClient.ClientState == ClientState.PeerCreated || punVoiceClient.ClientState == ClientState.Disconnected)
			{
				punVoiceClient.ConnectAndJoinRoom();
			}
		}

		private void CalibrateButtonOnClick()
		{
			if ((bool)recorder.RecorderInUse && !recorder.RecorderInUse.VoiceDetectorCalibrating)
			{
				recorder.RecorderInUse.VoiceDetectorCalibrate(calibrationMilliSeconds);
			}
		}

		private void Update()
		{
			if (recorder != null && recorder.RecorderInUse != null && recorder.RecorderInUse.LevelMeter != null)
			{
				voiceDebugText.text = $"Amp: avg. {recorder.RecorderInUse.LevelMeter.CurrentAvgAmp:0.000000}, peak {recorder.RecorderInUse.LevelMeter.CurrentPeakAmp:0.000000}";
			}
		}

		private void PunClientStateChanged(ClientState fromState, ClientState toState)
		{
			punState.text = $"PUN: {toState}";
			switch (toState)
			{
			case ClientState.PeerCreated:
			case ClientState.Disconnected:
				punSwitch.interactable = true;
				punSwitchText.text = "PUN Connect";
				break;
			case ClientState.Joined:
				punSwitch.interactable = true;
				punSwitchText.text = "PUN Disconnect";
				break;
			default:
				punSwitch.interactable = false;
				punSwitchText.text = "PUN busy";
				break;
			}
			UpdateUiBasedOnVoiceState(punVoiceClient.ClientState);
		}

		private void VoiceClientStateChanged(ClientState fromState, ClientState toState)
		{
			UpdateUiBasedOnVoiceState(toState);
		}

		private void UpdateUiBasedOnVoiceState(ClientState voiceClientState)
		{
			voiceState.text = $"PhotonVoice: {voiceClientState}";
			switch (voiceClientState)
			{
			case ClientState.Joined:
				voiceSwitch.interactable = true;
				inGameSettings.SetActive(value: true);
				voiceSwitchText.text = "Voice Disconnect";
				InitToggles(inGameSettings.GetComponentsInChildren<Toggle>());
				if (recorder != null && recorder.RecorderInUse != null)
				{
					calibrateButton.interactable = !recorder.RecorderInUse.VoiceDetectorCalibrating;
					calibrateText.text = (recorder.RecorderInUse.VoiceDetectorCalibrating ? "Calibrating" : $"Calibrate ({calibrationMilliSeconds / 1000}s)");
				}
				else
				{
					calibrateButton.interactable = false;
					calibrateText.text = "Unavailable";
				}
				break;
			case ClientState.PeerCreated:
			case ClientState.Disconnected:
				if (PhotonNetwork.InRoom)
				{
					voiceSwitch.interactable = true;
					voiceSwitchText.text = "Voice Connect";
					voiceDebugText.text = string.Empty;
				}
				else
				{
					voiceSwitch.interactable = false;
					voiceSwitchText.text = "Voice N/A";
					voiceDebugText.text = string.Empty;
				}
				calibrateButton.interactable = false;
				voiceSwitchText.text = "Voice Connect";
				calibrateText.text = "Unavailable";
				inGameSettings.SetActive(value: false);
				break;
			default:
				voiceSwitch.interactable = false;
				voiceSwitchText.text = "Voice busy";
				break;
			}
		}

		protected void OnApplicationQuit()
		{
			punVoiceClient.Client.StateChanged -= VoiceClientStateChanged;
			PhotonNetwork.NetworkingClient.StateChanged -= PunClientStateChanged;
		}
	}
}
