using UnityEngine;

public class CameraFogDensity : MonoBehaviour
{
	public float fogDensity;

	private float previousFogDensity;

	private void OnPreRender()
	{
		previousFogDensity = fogDensity;
	}

	private void OnPostRender()
	{
		RenderSettings.fogDensity = previousFogDensity;
	}
}
