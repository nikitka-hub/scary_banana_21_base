namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	public struct MicRef
	{
		public readonly MicType MicType;

		public readonly DeviceInfo Device;

		public MicRef(MicType micType, DeviceInfo device)
		{
			MicType = micType;
			Device = device;
		}

		public override string ToString()
		{
			return $"Mic reference: {Device.Name}";
		}
	}
}
