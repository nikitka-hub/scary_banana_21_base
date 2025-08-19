using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProximityAudio : MonoBehaviour
{
	public Transform player;

	public float maxHearingDistance = 10f;

	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.spatialBlend = 1f;
	}

	private void Update()
	{
		if (player == null)
		{
			return;
		}
		float num = Vector3.Distance(base.transform.position, player.position);
		if (num < maxHearingDistance)
		{
			float volume = Mathf.Clamp01(1f - num / maxHearingDistance);
			audioSource.volume = volume;
			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
		}
		else if (audioSource.isPlaying)
		{
			audioSource.Stop();
		}
	}
}
