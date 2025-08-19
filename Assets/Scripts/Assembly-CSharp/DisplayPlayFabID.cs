using TMPro;
using UnityEngine;

public class DisplayPlayFabID : MonoBehaviour
{
	public Playfablogin playfabloginscript;

	public TextMeshPro IDDisplayText;

	private void Update()
	{
		IDDisplayText.text = "Your ID is: " + playfabloginscript.MyPlayFabID;
	}
}
