using System.Collections.Generic;
using POpusCodec.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	public class CodecSettingsUI : MonoBehaviour
	{
		[SerializeField]
		private Dropdown frameDurationDropdown;

		[SerializeField]
		private Dropdown samplingRateDropdown;

		[SerializeField]
		private InputField bitrateInputField;

		[SerializeField]
		private Recorder recorder;

		private static readonly List<string> frameDurationOptions = new List<string> { "2.5ms", "5ms", "10ms", "20ms", "40ms", "60ms" };

		private static readonly List<string> samplingRateOptions = new List<string> { "8kHz", "12kHz", "16kHz", "24kHz", "48kHz" };

		private void Awake()
		{
			frameDurationDropdown.ClearOptions();
			frameDurationDropdown.AddOptions(frameDurationOptions);
			InitFrameDuration();
			frameDurationDropdown.SetSingleOnValueChangedCallback(OnFrameDurationChanged);
			samplingRateDropdown.ClearOptions();
			samplingRateDropdown.AddOptions(samplingRateOptions);
			InitSamplingRate();
			samplingRateDropdown.SetSingleOnValueChangedCallback(OnSamplingRateChanged);
			bitrateInputField.SetSingleOnValueChangedCallback(OnBitrateChanged);
			InitBitrate();
		}

		private void Update()
		{
			InitFrameDuration();
			InitSamplingRate();
			InitBitrate();
		}

		private void OnBitrateChanged(string newBitrateString)
		{
			if (int.TryParse(newBitrateString, out var result))
			{
				recorder.Bitrate = result;
			}
		}

		private void OnFrameDurationChanged(int index)
		{
			OpusCodec.FrameDuration frameDuration = recorder.FrameDuration;
			switch (index)
			{
			case 0:
				frameDuration = OpusCodec.FrameDuration.Frame2dot5ms;
				break;
			case 1:
				frameDuration = OpusCodec.FrameDuration.Frame5ms;
				break;
			case 2:
				frameDuration = OpusCodec.FrameDuration.Frame10ms;
				break;
			case 3:
				frameDuration = OpusCodec.FrameDuration.Frame20ms;
				break;
			case 4:
				frameDuration = OpusCodec.FrameDuration.Frame40ms;
				break;
			case 5:
				frameDuration = OpusCodec.FrameDuration.Frame60ms;
				break;
			}
			recorder.FrameDuration = frameDuration;
		}

		private void OnSamplingRateChanged(int index)
		{
			SamplingRate samplingRate = recorder.SamplingRate;
			switch (index)
			{
			case 0:
				samplingRate = SamplingRate.Sampling08000;
				break;
			case 1:
				samplingRate = SamplingRate.Sampling12000;
				break;
			case 2:
				samplingRate = SamplingRate.Sampling16000;
				break;
			case 3:
				samplingRate = SamplingRate.Sampling24000;
				break;
			case 4:
				samplingRate = SamplingRate.Sampling48000;
				break;
			}
			recorder.SamplingRate = samplingRate;
		}

		private void InitFrameDuration()
		{
			int value = 0;
			switch (recorder.FrameDuration)
			{
			case OpusCodec.FrameDuration.Frame5ms:
				value = 1;
				break;
			case OpusCodec.FrameDuration.Frame10ms:
				value = 2;
				break;
			case OpusCodec.FrameDuration.Frame20ms:
				value = 3;
				break;
			case OpusCodec.FrameDuration.Frame40ms:
				value = 4;
				break;
			case OpusCodec.FrameDuration.Frame60ms:
				value = 5;
				break;
			}
			frameDurationDropdown.value = value;
		}

		private void InitSamplingRate()
		{
			int value = 0;
			switch (recorder.SamplingRate)
			{
			case SamplingRate.Sampling12000:
				value = 1;
				break;
			case SamplingRate.Sampling16000:
				value = 2;
				break;
			case SamplingRate.Sampling24000:
				value = 3;
				break;
			case SamplingRate.Sampling48000:
				value = 4;
				break;
			}
			samplingRateDropdown.value = value;
		}

		private void InitBitrate()
		{
			bitrateInputField.text = recorder.Bitrate.ToString();
		}
	}
}
