using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	private TextMeshPro fpsText;

	private void Start()
	{
		fpsText = GetComponent<TextMeshPro>();
		if (fpsText == null)
		{
			Debug.LogError("FPSDisplay: No TextMeshPro component found.");
		}
		timeleft = updateInterval;
	}

	private void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			float num = accum / (float)frames;
			fpsText.text = $"{num:F2} FPS";
			if (num < 30f)
			{
				fpsText.color = Color.yellow;
			}
			else if (num < 10f)
			{
				fpsText.color = Color.red;
			}
			else
			{
				fpsText.color = Color.green;
			}
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}
}
