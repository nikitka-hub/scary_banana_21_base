using UnityEngine;

public class SaveColor : MonoBehaviour
{
	private float RedKey;

	private float GreenKey;

	private float BlueKey;

	public ColorScript ColorScript;

	private void Start()
	{
		RedKey = PlayerPrefs.GetFloat("RedKey");
		GreenKey = PlayerPrefs.GetFloat("GreenKey");
		BlueKey = PlayerPrefs.GetFloat("BlueKey");
		ColorScript.Red = RedKey;
		ColorScript.Green = GreenKey;
		ColorScript.Blue = BlueKey;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.CompareTag("HandTag"))
		{
			RedKey = ColorScript.Red;
			PlayerPrefs.SetFloat("RedKey", RedKey);
			GreenKey = ColorScript.Green;
			PlayerPrefs.SetFloat("GreenKey", GreenKey);
			BlueKey = ColorScript.Blue;
			PlayerPrefs.SetFloat("BlueKey", BlueKey);
		}
	}
}
