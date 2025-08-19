using Photon.VR;
using Photon.VR.Cosmetics;
using UnityEngine;

public class ChangeBodyCosmetic : MonoBehaviour
{
	public string Body;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("HandTag"))
		{
			PhotonVRManager.SetCosmetic(CosmeticType.Body, Body);
		}
	}
}
