using Photon.Pun;
using UnityEngine;

public class kick : MonoBehaviour
{
	public PhotonView ptView;

	private void OnTriggerEnter(Collider other)
	{
		if (!ptView.IsMine)
		{
			Application.Quit();
		}
	}
}
