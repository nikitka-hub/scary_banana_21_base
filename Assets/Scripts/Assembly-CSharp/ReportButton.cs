using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ReportButton : MonoBehaviour
{
	[SerializeField]
	public int ButtonNumber;

	[SerializeField]
	public LeaderBoard LB;

	[SerializeField]
	public string HandTag = "HandTag";

	public Material PressedMaterial;

	private Material UnPressedMaterial;

	private Renderer rend;

	private Player ReportedPlayer;

	private bool reported;

	private void Start()
	{
		rend = GetComponent<Renderer>();
		UnPressedMaterial = rend.material;
	}

	private void Update()
	{
		if (ButtonNumber > 0 && ButtonNumber <= PhotonNetwork.PlayerList.Length && PhotonNetwork.PlayerList[ButtonNumber - 1] != ReportedPlayer && reported)
		{
			reported = false;
			rend.material = UnPressedMaterial;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (ButtonNumber > 0 && ButtonNumber <= PhotonNetwork.PlayerList.Length && other.CompareTag(HandTag) && !reported)
		{
			LB.Report(ButtonNumber);
			rend.material = PressedMaterial;
			ReportedPlayer = PhotonNetwork.PlayerList[ButtonNumber - 1];
			reported = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag(HandTag))
		{
			rend.material = UnPressedMaterial;
		}
	}
}
