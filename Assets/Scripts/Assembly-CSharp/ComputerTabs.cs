using System.Collections.Generic;
using UnityEngine;

public class ComputerTabs : MonoBehaviour
{
	public List<GameObject> OldTabs = new List<GameObject>();

	public GameObject NewTab;

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("HandTag"))
		{
			return;
		}
		foreach (GameObject oldTab in OldTabs)
		{
			oldTab.SetActive(value: false);
		}
		NewTab.SetActive(value: true);
	}
}
