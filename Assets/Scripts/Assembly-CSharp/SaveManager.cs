using UnityEngine;

public class SaveManager : MonoBehaviour
{
	public NameScript NameScript;

	private void Start()
	{
		PlayerPrefs.GetString("PhtonUsername");
		NameScript.NameVar = PlayerPrefs.GetString("PhotonUsername");
	}

	private void Update()
	{
		PlayerPrefs.SetString("PhotonUsername", NameScript.NameVar);
	}
}
