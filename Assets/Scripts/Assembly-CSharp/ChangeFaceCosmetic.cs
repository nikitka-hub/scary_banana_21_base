using Photon.VR;
using Photon.VR.Cosmetics;
using UnityEngine;

public class ChangeFaceCosmetic : MonoBehaviour
{
	public string Face;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("HandTag"))
		{
			PhotonVRManager.SetCosmetic(CosmeticType.Face, Face);
		}
	}
}
