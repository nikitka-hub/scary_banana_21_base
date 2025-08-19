using UnityEngine;

public class Red : MonoBehaviour
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
		colors.Red = value;
	}
}
