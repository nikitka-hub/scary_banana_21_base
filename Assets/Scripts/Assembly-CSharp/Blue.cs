using UnityEngine;

public class Blue : MonoBehaviour
{
	public float value;

	public ColorScript colors;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter()
	{
		colors.Blue = value;
	}
}
