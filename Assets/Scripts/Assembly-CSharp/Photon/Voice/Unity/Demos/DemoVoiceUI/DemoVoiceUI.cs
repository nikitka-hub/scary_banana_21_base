using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Voice.Unity.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	[RequireComponent(typeof(UnityVoiceClient), typeof(ConnectAndJoin))]
	public class DemoVoiceUI : MonoBehaviour, IInRoomCallbacks, IMatchmakingCallbacks
	{
		[SerializeField]
		private Text connectionStatusText;

		[SerializeField]
		private Text serverStatusText;

		[SerializeField]
		private Text roomStatusText;

		[SerializeField]
		private Text inputWarningText;

		[SerializeField]
		private Text rttText;

		[SerializeField]
		private Text rttVariationText;

		[SerializeField]
		private Text packetLossWarningText;

		[SerializeField]
		private InputField localNicknameText;

		[SerializeField]
		private Toggle debugEchoToggle;

		[SerializeField]
		private Toggle reliableTransmissionToggle;

		[SerializeField]
		private Toggle encryptionToggle;

		[SerializeField]
		private GameObject webRtcDspGameObject;

		[SerializeField]
		private Toggle aecToggle;

		[SerializeField]
		private Toggle aecHighPassToggle;

		[SerializeField]
		private InputField reverseStreamDelayInputField;

		[SerializeField]
		private Toggle noiseSuppressionToggle;

		[SerializeField]
		private Toggle agcToggle;

		[SerializeField]
		private Slider agcCompressionGainSlider;

		[SerializeField]
		private Slider agcTargetLevelSlider;

		[SerializeField]
		private Toggle vadToggle;

		[SerializeField]
		private Toggle muteToggle;

		[SerializeField]
		private Toggle streamAudioClipToggle;

		[SerializeField]
		private Toggle audioToneToggle;

		[SerializeField]
		private Toggle dspToggle;

		[SerializeField]
		private Toggle highPassToggle;

		[SerializeField]
		private Toggle photonVadToggle;

		[SerializeField]
		private MicrophoneSelector microphoneSelector;

		[SerializeField]
		private GameObject androidMicSettingGameObject;

		[SerializeField]
		private Toggle androidAgcToggle;

		[SerializeField]
		private Toggle androidAecToggle;

		[SerializeField]
		private Toggle androidNsToggle;

		[SerializeField]
		private bool defaultTransmitEnabled;

		[SerializeField]
		private bool fullScreen;

		[SerializeField]
		private InputField roomNameInputField;

		[SerializeField]
		private int rttYellowThreshold = 100;

		[SerializeField]
		private int rttRedThreshold = 160;

		[SerializeField]
		private int rttVariationYellowThreshold = 25;

		[SerializeField]
		private int rttVariationRedThreshold = 50;

		private GameObject compressionGainGameObject;

		private GameObject targetLevelGameObject;

		private Text compressionGainText;

		private Text targetLevelText;

		private GameObject aecOptionsGameObject;

		public Transform RemoteVoicesPanel;

		protected UnityVoiceClient voiceConnection;

		private WebRtcAudioDsp voiceAudioPreprocessor;

		private ConnectAndJoin connectAndJoin;

		private readonly Color warningColor = new Color(0.9f, 0.5f, 0f, 1f);

		private readonly Color okColor = new Color(0f, 0.6f, 0.2f, 1f);

		private readonly Color redColor = new Color(1f, 0f, 0f, 1f);

		private readonly Color defaultColor = new Color(0f, 0f, 0f, 1f);

		private Func<IAudioDesc> toneInputFactory = () => new AudioUtil.ToneAudioReader<float>(null, 440.0, 48000, 2);

		private void Start()
		{
			connectAndJoin = GetComponent<ConnectAndJoin>();
			voiceConnection = GetComponent<UnityVoiceClient>();
			voiceAudioPreprocessor = voiceConnection.PrimaryRecorder.GetComponent<WebRtcAudioDsp>();
			compressionGainGameObject = agcCompressionGainSlider.transform.parent.gameObject;
			compressionGainText = compressionGainGameObject.GetComponentInChildren<Text>();
			targetLevelGameObject = agcTargetLevelSlider.transform.parent.gameObject;
			targetLevelText = targetLevelGameObject.GetComponentInChildren<Text>();
			aecOptionsGameObject = aecHighPassToggle.transform.parent.gameObject;
			SetDefaults();
			InitUiCallbacks();
			GetSavedNickname();
			voiceConnection.PrimaryRecorder.InputFactory = toneInputFactory;
			voiceConnection.SpeakerLinked += OnSpeakerCreated;
			voiceConnection.Client.AddCallbackTarget(this);
		}

		protected virtual void SetDefaults()
		{
			muteToggle.isOn = !defaultTransmitEnabled;
		}

		private void OnDestroy()
		{
			voiceConnection.SpeakerLinked -= OnSpeakerCreated;
			voiceConnection.Client.RemoveCallbackTarget(this);
		}

		private void GetSavedNickname()
		{
			string text = PlayerPrefs.GetString("vNick");
			if (!string.IsNullOrEmpty(text))
			{
				localNicknameText.text = text;
				voiceConnection.Client.NickName = text;
			}
		}

		protected virtual void OnSpeakerCreated(Speaker speaker)
		{
			speaker.gameObject.transform.SetParent(RemoteVoicesPanel, worldPositionStays: false);
			speaker.GetComponent<RemoteSpeakerUI>().Init(voiceConnection);
			speaker.OnRemoteVoiceRemoveAction = (Action<Speaker>)Delegate.Combine(speaker.OnRemoteVoiceRemoveAction, new Action<Speaker>(OnRemoteVoiceRemove));
		}

		private void OnRemoteVoiceRemove(Speaker speaker)
		{
			if (speaker != null)
			{
				UnityEngine.Object.Destroy(speaker.gameObject);
			}
		}

		private void ToggleMute(bool isOn)
		{
			muteToggle.targetGraphic.enabled = !isOn;
			if (isOn)
			{
				voiceConnection.Client.LocalPlayer.Mute();
			}
			else
			{
				voiceConnection.Client.LocalPlayer.Unmute();
			}
		}

		protected virtual void ToggleIsRecording(bool isRecording)
		{
			voiceConnection.PrimaryRecorder.RecordingEnabled = isRecording;
		}

		private void ToggleDebugEcho(bool isOn)
		{
			voiceConnection.PrimaryRecorder.DebugEchoMode = isOn;
		}

		private void ToggleReliable(bool isOn)
		{
			voiceConnection.PrimaryRecorder.ReliableMode = isOn;
		}

		private void ToggleEncryption(bool isOn)
		{
			voiceConnection.PrimaryRecorder.Encrypt = isOn;
		}

		private void ToggleAEC(bool isOn)
		{
			voiceAudioPreprocessor.AEC = isOn;
			aecOptionsGameObject.SetActive(isOn);
			voiceConnection.Client.LocalPlayer.SetAEC(isOn);
		}

		private void ToggleNoiseSuppression(bool isOn)
		{
			voiceAudioPreprocessor.NoiseSuppression = isOn;
		}

		private void ToggleAGC(bool isOn)
		{
			voiceAudioPreprocessor.AGC = isOn;
			compressionGainGameObject.SetActive(isOn);
			targetLevelGameObject.SetActive(isOn);
			voiceConnection.Client.LocalPlayer.SetAGC(isOn, voiceAudioPreprocessor.AgcCompressionGain, voiceAudioPreprocessor.AgcTargetLevel);
		}

		private void ToggleVAD(bool isOn)
		{
			voiceAudioPreprocessor.VAD = isOn;
			voiceConnection.Client.LocalPlayer.SetWebRTCVAD(isOn);
		}

		private void ToggleHighPass(bool isOn)
		{
			voiceAudioPreprocessor.HighPass = isOn;
		}

		private void ToggleDsp(bool isOn)
		{
			voiceAudioPreprocessor.enabled = isOn;
			voiceConnection.PrimaryRecorder.RestartRecording();
			webRtcDspGameObject.SetActive(isOn);
			voiceConnection.Client.LocalPlayer.SetWebRTCVAD(voiceAudioPreprocessor.VAD);
			voiceConnection.Client.LocalPlayer.SetAEC(voiceAudioPreprocessor.AEC);
			voiceConnection.Client.LocalPlayer.SetAGC(voiceAudioPreprocessor.AGC, voiceAudioPreprocessor.AgcCompressionGain, voiceAudioPreprocessor.AgcTargetLevel);
		}

		private void ToggleAudioClipStreaming(bool isOn)
		{
			if (isOn)
			{
				audioToneToggle.SetValue(isOn: false);
				voiceConnection.PrimaryRecorder.SourceType = Recorder.InputSourceType.AudioClip;
			}
			else if (!audioToneToggle.isOn)
			{
				microphoneSelector.SwitchToSelectedMic();
			}
		}

		private void ToggleAudioToneFactory(bool isOn)
		{
			if (isOn)
			{
				streamAudioClipToggle.SetValue(isOn: false);
				voiceConnection.PrimaryRecorder.SourceType = Recorder.InputSourceType.Factory;
				voiceConnection.PrimaryRecorder.InputFactory = toneInputFactory;
			}
			else if (!streamAudioClipToggle.isOn)
			{
				microphoneSelector.SwitchToSelectedMic();
			}
		}

		private void TogglePhotonVAD(bool isOn)
		{
			voiceConnection.PrimaryRecorder.VoiceDetection = isOn;
			voiceConnection.Client.LocalPlayer.SetPhotonVAD(isOn);
		}

		private void ToggleAecHighPass(bool isOn)
		{
			voiceAudioPreprocessor.AecHighPass = isOn;
			voiceConnection.Client.LocalPlayer.SetAEC(isOn);
		}

		private void OnAgcCompressionGainChanged(float agcCompressionGain)
		{
			voiceAudioPreprocessor.AgcCompressionGain = (int)agcCompressionGain;
			compressionGainText.text = "Compression Gain: " + agcCompressionGain;
			voiceConnection.Client.LocalPlayer.SetAGC(voiceAudioPreprocessor.AGC, (int)agcCompressionGain, voiceAudioPreprocessor.AgcTargetLevel);
		}

		private void OnAgcTargetLevelChanged(float agcTargetLevel)
		{
			voiceAudioPreprocessor.AgcTargetLevel = (int)agcTargetLevel;
			targetLevelText.text = "Target Level: " + agcTargetLevel;
			voiceConnection.Client.LocalPlayer.SetAGC(voiceAudioPreprocessor.AGC, voiceAudioPreprocessor.AgcCompressionGain, (int)agcTargetLevel);
		}

		private void OnReverseStreamDelayChanged(string newReverseStreamString)
		{
			if (int.TryParse(newReverseStreamString, out var result) && result > 0)
			{
				voiceAudioPreprocessor.ReverseStreamDelayMs = result;
			}
			else
			{
				reverseStreamDelayInputField.text = voiceAudioPreprocessor.ReverseStreamDelayMs.ToString();
			}
		}

		private void OnMicrophoneChanged(Recorder.MicType micType, DeviceInfo deviceInfo)
		{
			voiceConnection.Client.LocalPlayer.SetMic(micType);
			androidMicSettingGameObject.SetActive(micType == Recorder.MicType.Photon);
		}

		private void OnAndroidMicSettingsChanged(bool isOn)
		{
			voiceConnection.PrimaryRecorder.SetAndroidNativeMicrophoneSettings(androidAecToggle.isOn, androidAgcToggle.isOn, androidNsToggle.isOn);
		}

		private void UpdateSyncedNickname(string nickname)
		{
			nickname = nickname.Trim();
			voiceConnection.Client.LocalPlayer.NickName = nickname;
			PlayerPrefs.SetString("vNick", nickname);
		}

		private void JoinOrCreateRoom(string roomName)
		{
			if (string.IsNullOrEmpty(roomName))
			{
				connectAndJoin.RoomName = string.Empty;
				connectAndJoin.RandomRoom = true;
			}
			else
			{
				connectAndJoin.RoomName = roomName.Trim();
				connectAndJoin.RandomRoom = false;
			}
			if (voiceConnection.Client.InRoom)
			{
				voiceConnection.Client.OpLeaveRoom(becomeInactive: false);
			}
			else if (!voiceConnection.Client.IsConnected)
			{
				voiceConnection.ConnectUsingSettings();
			}
		}

		private void PhotonVoiceCreated(PhotonVoiceCreatedParams p)
		{
			InitUiValues();
		}

		protected virtual void Update()
		{
			connectionStatusText.text = voiceConnection.Client.State.ToString();
			serverStatusText.text = $"{voiceConnection.Client.CloudRegion}/{voiceConnection.Client.CurrentServerAddress}";
			if (voiceConnection.PrimaryRecorder.IsCurrentlyTransmitting)
			{
				float num = voiceConnection.PrimaryRecorder.LevelMeter.CurrentAvgAmp;
				if (num > 1f)
				{
					num /= 32768f;
				}
				if ((double)num > 0.1)
				{
					inputWarningText.text = "Input too loud!";
					inputWarningText.color = warningColor;
				}
				else
				{
					inputWarningText.text = string.Empty;
					ResetTextColor(inputWarningText);
				}
			}
			if (voiceConnection.FramesReceivedPerSecond > 0f)
			{
				packetLossWarningText.text = $"{voiceConnection.FramesLostPercent:0.##}% Packet Loss";
				packetLossWarningText.color = ((voiceConnection.FramesLostPercent > 1f) ? warningColor : okColor);
			}
			else
			{
				packetLossWarningText.text = string.Empty;
				ResetTextColor(packetLossWarningText);
			}
			rttText.text = "RTT:" + voiceConnection.Client.LoadBalancingPeer.RoundTripTime;
			SetTextColor(voiceConnection.Client.LoadBalancingPeer.RoundTripTime, rttText, rttYellowThreshold, rttRedThreshold);
			rttVariationText.text = "VAR:" + voiceConnection.Client.LoadBalancingPeer.RoundTripTimeVariance;
			SetTextColor(voiceConnection.Client.LoadBalancingPeer.RoundTripTimeVariance, rttVariationText, rttVariationYellowThreshold, rttVariationRedThreshold);
		}

		private void SetTextColor(int textValue, Text text, int yellowThreshold, int redThreshold)
		{
			if (textValue > redThreshold)
			{
				text.color = redColor;
			}
			else if (textValue > yellowThreshold)
			{
				text.color = warningColor;
			}
			else
			{
				text.color = okColor;
			}
		}

		private void ResetTextColor(Text text)
		{
			text.color = defaultColor;
		}

		private void InitUiCallbacks()
		{
			muteToggle.SetSingleOnValueChangedCallback(ToggleMute);
			debugEchoToggle.SetSingleOnValueChangedCallback(ToggleDebugEcho);
			reliableTransmissionToggle.SetSingleOnValueChangedCallback(ToggleReliable);
			encryptionToggle.SetSingleOnValueChangedCallback(ToggleEncryption);
			streamAudioClipToggle.SetSingleOnValueChangedCallback(ToggleAudioClipStreaming);
			audioToneToggle.SetSingleOnValueChangedCallback(ToggleAudioToneFactory);
			photonVadToggle.SetSingleOnValueChangedCallback(TogglePhotonVAD);
			vadToggle.SetSingleOnValueChangedCallback(ToggleVAD);
			aecToggle.SetSingleOnValueChangedCallback(ToggleAEC);
			agcToggle.SetSingleOnValueChangedCallback(ToggleAGC);
			dspToggle.SetSingleOnValueChangedCallback(ToggleDsp);
			highPassToggle.SetSingleOnValueChangedCallback(ToggleHighPass);
			aecHighPassToggle.SetSingleOnValueChangedCallback(ToggleAecHighPass);
			noiseSuppressionToggle.SetSingleOnValueChangedCallback(ToggleNoiseSuppression);
			agcCompressionGainSlider.SetSingleOnValueChangedCallback(OnAgcCompressionGainChanged);
			agcTargetLevelSlider.SetSingleOnValueChangedCallback(OnAgcTargetLevelChanged);
			localNicknameText.SetSingleOnEndEditCallback(UpdateSyncedNickname);
			roomNameInputField.SetSingleOnEndEditCallback(JoinOrCreateRoom);
			reverseStreamDelayInputField.SetSingleOnEndEditCallback(OnReverseStreamDelayChanged);
			androidAgcToggle.SetSingleOnValueChangedCallback(OnAndroidMicSettingsChanged);
			androidAecToggle.SetSingleOnValueChangedCallback(OnAndroidMicSettingsChanged);
			androidNsToggle.SetSingleOnValueChangedCallback(OnAndroidMicSettingsChanged);
		}

		private void InitUiValues()
		{
			muteToggle.SetValue(voiceConnection.Client.LocalPlayer.IsMuted());
			debugEchoToggle.SetValue(voiceConnection.PrimaryRecorder.DebugEchoMode);
			reliableTransmissionToggle.SetValue(voiceConnection.PrimaryRecorder.ReliableMode);
			encryptionToggle.SetValue(voiceConnection.PrimaryRecorder.Encrypt);
			streamAudioClipToggle.SetValue(voiceConnection.PrimaryRecorder.SourceType == Recorder.InputSourceType.AudioClip);
			audioToneToggle.SetValue(voiceConnection.PrimaryRecorder.SourceType == Recorder.InputSourceType.Factory && voiceConnection.PrimaryRecorder.InputFactory == toneInputFactory);
			photonVadToggle.SetValue(voiceConnection.PrimaryRecorder.VoiceDetection);
			androidAgcToggle.SetValue(voiceConnection.PrimaryRecorder.AndroidMicrophoneAGC);
			androidAecToggle.SetValue(voiceConnection.PrimaryRecorder.AndroidMicrophoneAEC);
			androidNsToggle.SetValue(voiceConnection.PrimaryRecorder.AndroidMicrophoneNS);
			if (webRtcDspGameObject != null)
			{
				dspToggle.gameObject.SetActive(value: true);
				dspToggle.SetValue(voiceAudioPreprocessor.enabled);
				webRtcDspGameObject.SetActive(dspToggle.isOn);
				aecToggle.SetValue(voiceAudioPreprocessor.AEC);
				aecHighPassToggle.SetValue(voiceAudioPreprocessor.AecHighPass);
				reverseStreamDelayInputField.text = voiceAudioPreprocessor.ReverseStreamDelayMs.ToString();
				aecOptionsGameObject.SetActive(voiceAudioPreprocessor.AEC);
				noiseSuppressionToggle.isOn = voiceAudioPreprocessor.NoiseSuppression;
				agcToggle.SetValue(voiceAudioPreprocessor.AGC);
				agcCompressionGainSlider.SetValue(voiceAudioPreprocessor.AgcCompressionGain);
				agcTargetLevelSlider.SetValue(voiceAudioPreprocessor.AgcTargetLevel);
				compressionGainGameObject.SetActive(voiceAudioPreprocessor.AGC);
				targetLevelGameObject.SetActive(voiceAudioPreprocessor.AGC);
				vadToggle.SetValue(voiceAudioPreprocessor.VAD);
				highPassToggle.SetValue(voiceAudioPreprocessor.HighPass);
			}
			else
			{
				dspToggle.gameObject.SetActive(value: false);
			}
		}

		private void SetRoomDebugText()
		{
			string text = string.Empty;
			if (voiceConnection.Client.InRoom)
			{
				foreach (Player value in voiceConnection.Client.CurrentRoom.Players.Values)
				{
					text += value.ToStringFull();
				}
				roomStatusText.text = $"{voiceConnection.Client.CurrentRoom.Name} {text}";
			}
			else
			{
				roomStatusText.text = string.Empty;
			}
			roomStatusText.text = ((voiceConnection.Client.CurrentRoom == null) ? string.Empty : $"{voiceConnection.Client.CurrentRoom.Name} {text}");
		}

		protected virtual void OnActorPropertiesChanged(Player targetPlayer, Hashtable changedProps)
		{
			if (targetPlayer.IsLocal)
			{
				bool flag = targetPlayer.IsMuted();
				voiceConnection.PrimaryRecorder.TransmitEnabled = !flag;
				muteToggle.SetValue(flag);
			}
			SetRoomDebugText();
		}

		protected void OnApplicationQuit()
		{
			voiceConnection.Client.RemoveCallbackTarget(this);
		}

		void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
		{
			SetRoomDebugText();
		}

		void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
		{
			SetRoomDebugText();
		}

		void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
		{
		}

		void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
		{
			OnActorPropertiesChanged(targetPlayer, changedProps);
		}

		void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
		{
		}

		void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		void IMatchmakingCallbacks.OnCreatedRoom()
		{
		}

		void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
		{
		}

		void IMatchmakingCallbacks.OnJoinedRoom()
		{
			SetRoomDebugText();
			voiceConnection.Client.LocalPlayer.SetMic(voiceConnection.PrimaryRecorder.MicrophoneType);
		}

		void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
		{
		}

		void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
		{
		}

		void IMatchmakingCallbacks.OnLeftRoom()
		{
			SetRoomDebugText();
			SetDefaults();
		}
	}
}
