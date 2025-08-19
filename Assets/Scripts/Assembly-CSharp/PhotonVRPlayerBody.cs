using UnityEngine;

public class PhotonVRPlayerBody : MonoBehaviour
{
	public Transform Head;

	public float Offset;

	private void Update()
	{
		base.transform.rotation = new Quaternion(0f, Head.rotation.y, 0f, Head.rotation.w);
		base.transform.position = new Vector3(Head.position.x, Head.position.y + Offset, Head.position.z);
	}
}
