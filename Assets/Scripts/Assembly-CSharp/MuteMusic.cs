using System.Collections;
using UnityEngine;

public class MuteMusic : MonoBehaviour
{
	public AudioSource[] music;

	public float delay = 1f;

	private bool isProcessing;

	private bool isMuted;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("HandTag") && !isProcessing)
		{
			StartCoroutine(ToggleMuteWithDelay());
		}
	}

	private IEnumerator ToggleMuteWithDelay()
	{
		isProcessing = true;
		AudioSource[] array = music;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].mute = !isMuted;
		}
		isMuted = !isMuted;
		yield return new WaitForSeconds(delay);
		isProcessing = false;
	}
}
