using Photon.VR;
using Photon.VR.Cosmetics;
using UnityEngine;

public class ChangeLeftCosmetic : MonoBehaviour
{
	public string Left;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("HandTag"))
		{
			PhotonVRManager.SetCosmetic(CosmeticType.LeftHand, Left);
		}
	}
}
