using TMPro;
using UnityEngine;

public class ServerScript : MonoBehaviour
{
	[Header("THIS SCRIPT WAS MADE BY FLIMCYVR. IT IS NOT YOURS.")]
	[Header("Distributing This Script Will Lead To A Permanent Ban and MORE!")]
	[Header("If you make a video on this script")]
	[Header("credit me with my discord and youtube")]
	public JoinRoom joinRoom;

	public string Letter;

	public TextMeshPro ServerText;

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "HandTag" && joinRoom.name.Length < 12)
		{
			joinRoom.name += Letter;
			ServerText.text = joinRoom.name;
		}
	}

	private void FixedUpdate()
	{
		ServerText.text = joinRoom.name;
	}
}
