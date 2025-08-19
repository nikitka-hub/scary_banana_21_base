using Photon.Pun;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	[RequireComponent(typeof(PhotonView))]
	public class ChangeName : MonoBehaviour
	{
		private void Start()
		{
			PhotonView component = GetComponent<PhotonView>();
			base.name = $"ActorNumber {component.OwnerActorNr}";
		}
	}
}
