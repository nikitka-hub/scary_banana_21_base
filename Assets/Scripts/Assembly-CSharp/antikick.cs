using Photon.Pun;
using UnityEngine;

public class antikick : MonoBehaviour
{
	public PhotonView ptView;

	public Collider collider;

	private void Update()
	{
		if (ptView.IsMine)
		{
			collider.enabled = false;
		}
		else
		{
			collider.enabled = true;
		}
	}
}
