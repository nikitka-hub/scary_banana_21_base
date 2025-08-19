using Photon.VR;
using Photon.VR.Cosmetics;
using UnityEngine;

public class ChangeRightCosmetic : MonoBehaviour
{
	public string Right;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("HandTag"))
		{
			PhotonVRManager.SetCosmetic(CosmeticType.RightHand, Right);
		}
	}
}
