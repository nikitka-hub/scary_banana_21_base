using Photon.VR;
using TMPro;
using UnityEngine;

public class NameScript : MonoBehaviour
{
	public string NameVar;

	public TextMeshPro NameText;

	private void Update()
	{
		if (NameVar.Length > 12)
		{
			NameVar = NameVar.Substring(0, 12);
		}
		NameText.text = NameVar;
		PhotonVRManager.SetUsername(NameVar);
	}
}
