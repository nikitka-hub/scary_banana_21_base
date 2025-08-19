using Photon.VR;
using UnityEngine;

public class ColorScript : MonoBehaviour
{
	public float Red;

	public float Blue;

	public float Green;

	private float TrueRed;

	private float TrueBlue;

	private float TrueGreen;

	private void Start()
	{
	}

	private void Update()
	{
		TrueRed = Red / 10f;
		TrueBlue = Blue / 10f;
		TrueGreen = Green / 10f;
		PhotonVRManager.SetColour(new Color(TrueRed, TrueBlue, TrueGreen));
	}
}
