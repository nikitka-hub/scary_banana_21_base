using Photon.Pun;
using UnityEngine;

public class FingerColliderFixed : MonoBehaviourPun
{
	[Header("Script by Keo.CS if you need help join the DC")]
	public GameObject Collider;

	public GameObject FingerBone;

	private void Start()
	{
		if (base.photonView.IsMine)
		{
			Collider.SetActive(value: true);
		}
		else
		{
			Collider.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (Collider == null || FingerBone == null)
		{
			Debug.Log("You forgot the Collider or the Finger bone you, so how do you expect this to work hmmmmm");
			return;
		}
		Collider.transform.position = FingerBone.transform.position;
		Collider.transform.rotation = FingerBone.transform.rotation;
	}
}
