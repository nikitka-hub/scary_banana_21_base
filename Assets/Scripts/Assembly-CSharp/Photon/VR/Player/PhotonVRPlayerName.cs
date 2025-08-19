using UnityEngine;

namespace Photon.VR.Player
{
	public class PhotonVRPlayerName : MonoBehaviour
	{
		[Tooltip("How high the text should be above the players head")]
		public float Offset = 0.17f;

		public Transform Head;

		private void Update()
		{
			base.transform.position = Head.position + new Vector3(0f, Offset, 0f);
			Vector3 forward = PhotonVRManager.Manager.Head.position - base.transform.position;
			Quaternion b = new Quaternion(0f, Quaternion.LookRotation(forward).y, 0f, Quaternion.LookRotation(forward).w);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, 10f * Time.deltaTime);
		}
	}
}
