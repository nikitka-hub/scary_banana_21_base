using UnityEngine;

public class AddLetter : MonoBehaviour
{
	public NameScript nameScript;

	public string Handtag;

	public string Letter;

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == Handtag)
		{
			nameScript.NameVar += Letter;
		}
	}
}
