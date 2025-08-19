using Photon.VR;
using Photon.VR.Cosmetics;
using UnityEngine;

public class ChangeHeadCosmetic : MonoBehaviour
{
	public string Head;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("HandTag"))
		{
			PhotonVRManager.SetCosmetic(CosmeticType.Head, Head);
		}
	}
}
