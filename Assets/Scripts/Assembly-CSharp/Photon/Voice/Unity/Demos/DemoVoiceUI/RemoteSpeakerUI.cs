using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	public class RemoteSpeakerUI : MonoBehaviour, IInRoomCallbacks
	{
		[SerializeField]
		private Text nameText;

		[SerializeField]
		protected Image remoteIsMuting;

		[SerializeField]
		private Image remoteIsTalking;

		[SerializeField]
		private InputField playDelayInputField;

		[SerializeField]
		private Text bufferLagText;

		[SerializeField]
		private Slider volumeSlider;

		[SerializeField]
		private Text photonVad;

		[SerializeField]
		private Text webrtcVad;

		[SerializeField]
		private Text aec;

		[SerializeField]
		private Text agc;

		[SerializeField]
		private Text mic;

		protected Speaker speaker;

		private AudioSource audioSource;

		protected VoiceConnection voiceConnection;

		protected LoadBalancingClient loadBalancingClient;

		private int smoothedLag;

		protected Player Actor
		{
			get
			{
				if (loadBalancingClient == null || loadBalancingClient.CurrentRoom == null)
				{
					return null;
				}
				return loadBalancingClient.CurrentRoom.GetPlayer(speaker.RemoteVoice.PlayerId);
			}
		}

		protected virtual void Start()
		{
			speaker = GetComponent<Speaker>();
			audioSource = GetComponent<AudioSource>();
			playDelayInputField.text = speaker.PlayDelay.ToString();
			playDelayInputField.SetSingleOnEndEditCallback(OnPlayDelayChanged);
			SetNickname();
			SetMutedState();
			SetProperties();
			volumeSlider.minValue = 0f;
			volumeSlider.maxValue = 1f;
			volumeSlider.SetSingleOnValueChangedCallback(OnVolumeChanged);
			volumeSlider.value = 1f;
			OnVolumeChanged(1f);
		}

		private void OnVolumeChanged(float newValue)
		{
			audioSource.volume = newValue;
		}

		private void OnPlayDelayChanged(string str)
		{
			if (int.TryParse(str, out var result))
			{
				speaker.PlayDelay = result;
				return;
			}
			Debug.LogErrorFormat("Failed to parse {0}", str);
		}

		private void Update()
		{
			remoteIsTalking.enabled = speaker.IsPlaying;
			if (speaker.IsPlaying)
			{
				int lag = speaker.Lag;
				smoothedLag = (lag + smoothedLag * 99) / 100;
				bufferLagText.text = "Buffer Lag: " + smoothedLag + "/" + lag;
			}
			else
			{
				bufferLagText.text = "Buffer Lag: " + smoothedLag + "/-";
			}
		}

		private void OnDestroy()
		{
			if (loadBalancingClient != null)
			{
				loadBalancingClient.RemoveCallbackTarget(this);
			}
		}

		private void SetNickname()
		{
			string text = speaker.name;
			if (Actor != null)
			{
				text = Actor.NickName;
				if (string.IsNullOrEmpty(text))
				{
					text = "user " + Actor.ActorNumber;
				}
			}
			nameText.text = text;
		}

		private void SetMutedState()
		{
			SetMutedState(Actor.IsMuted());
		}

		private void SetProperties()
		{
			photonVad.enabled = Actor.HasPhotonVAD();
			webrtcVad.enabled = Actor.HasWebRTCVAD();
			aec.enabled = Actor.HasAEC();
			agc.enabled = Actor.HasAGC();
			agc.text = "AGC Gain: " + Actor.GetAGCGain() + " Level: " + Actor.GetAGCLevel();
			Recorder.MicType? micType = Actor.GetMic();
			mic.enabled = micType.HasValue;
			mic.text = ((!micType.HasValue) ? "" : ((micType == Recorder.MicType.Unity) ? "Unity MIC" : "Photon MIC"));
		}

		protected virtual void SetMutedState(bool isMuted)
		{
			remoteIsMuting.enabled = isMuted;
		}

		protected virtual void OnActorPropertiesChanged(Player targetPlayer, Hashtable changedProps)
		{
			if (speaker != null && speaker.RemoteVoice != null && targetPlayer.ActorNumber == speaker.RemoteVoice.PlayerId)
			{
				SetMutedState();
				SetNickname();
				SetProperties();
			}
		}

		public virtual void Init(VoiceConnection vC)
		{
			voiceConnection = vC;
			loadBalancingClient = voiceConnection.Client;
			loadBalancingClient.AddCallbackTarget(this);
		}

		void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
		{
		}

		void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
		{
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
	}
}
