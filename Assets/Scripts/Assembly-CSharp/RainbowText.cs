using TMPro;
using UnityEngine;

public class RainbowText : MonoBehaviour
{
	[Header("Script made by Golonka")]
	[Header("If it is possible give credits")]
	public float colorChangeSpeed = 0.1f;

	private TextMeshPro textMeshPro;

	private float hueValue;

	private void Start()
	{
		textMeshPro = GetComponent<TextMeshPro>();
		if (textMeshPro == null)
		{
			textMeshPro = GetComponentInChildren<TextMeshPro>();
		}
		if (textMeshPro == null)
		{
			Debug.LogError("TextMeshPro component not found on the GameObject or its children.");
		}
	}

	private void Update()
	{
		if (!(textMeshPro == null))
		{
			hueValue += colorChangeSpeed * Time.deltaTime;
			if (hueValue >= 1f)
			{
				hueValue -= 1f;
			}
			Color color = Color.HSVToRGB(hueValue, 1f, 1f);
			textMeshPro.color = color;
		}
	}
}
