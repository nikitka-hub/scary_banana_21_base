using Photon.VR.Cosmetics;
using UnityEngine;

namespace Photon.VR.Testing
{
	public class PhotonVRCosmeticsChanger : MonoBehaviour
	{
		public PhotonVRCosmeticsData Cosmetics;

		public void ChangeCosmetics(PhotonVRCosmeticsData Cosmetics)
		{
			PhotonVRManager.SetCosmetics(Cosmetics);
		}
	}
}
