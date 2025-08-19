using Photon.Pun;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	[RequireComponent(typeof(Renderer))]
	[RequireComponent(typeof(PhotonView))]
	public class ChangeColor : MonoBehaviour
	{
		private PhotonView photonView;

		private void Start()
		{
			photonView = GetComponent<PhotonView>();
			if (photonView.IsMine)
			{
				Color color = Random.ColorHSV();
				photonView.RPC("ChangeColour", RpcTarget.AllBuffered, new Vector3(color.r, color.g, color.b));
			}
		}

		[PunRPC]
		private void ChangeColour(Vector3 randomColor)
		{
			GetComponent<Renderer>().material.SetColor("_Color", new Color(randomColor.x, randomColor.y, randomColor.z));
		}
	}
}
