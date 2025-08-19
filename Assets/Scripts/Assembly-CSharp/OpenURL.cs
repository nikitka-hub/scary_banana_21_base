using UnityEngine;

public class OpenURL : MonoBehaviour
{
	public string URL;

	private void OnTriggerEnter(Collider other)
	{
		Application.OpenURL(URL);
	}
}
