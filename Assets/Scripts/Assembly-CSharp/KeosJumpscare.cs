using System.Collections;
using UnityEngine;

public class KeosJumpscare : MonoBehaviour
{
	[Header("This was made by Keo.CS")]
	public GameObject gorillaPlayer;

	public Transform teleportPoint;

	public string handTag = "HandTag";

	public string PlayerTag = "Player";

	public float disableDuration = 0.5f;

	public float gravityDisableTime = 1f;

	public float jumpscareDuration = 2f;

	public GameObject Jumpscare;

	private Rigidbody gorillaRigidbody;

	private Collider[] allColliders;

	private void Start()
	{
		gorillaRigidbody = gorillaPlayer.GetComponent<Rigidbody>();
		allColliders = Object.FindObjectsOfType<Collider>();
		Jumpscare.SetActive(value: false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(handTag) || other.CompareTag(PlayerTag))
		{
			Collider[] array = allColliders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
			TeleportGorilla();
		}
	}

	private void TeleportGorilla()
	{
		gorillaRigidbody.useGravity = false;
		Invoke("EnableGravity", gravityDisableTime);
		gorillaPlayer.transform.position = teleportPoint.position;
		Invoke("EnableColliders", disableDuration);
		Jumpscare.SetActive(value: true);
		StartCoroutine(JumpscareEnd(jumpscareDuration));
	}

	private void EnableGravity()
	{
		gorillaRigidbody.useGravity = true;
	}

	private void EnableColliders()
	{
		Collider[] array = allColliders;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = true;
		}
	}

	private IEnumerator JumpscareEnd(float duration)
	{
		yield return new WaitForSeconds(duration);
		Jumpscare.SetActive(value: false);
	}
}
