using UnityEngine;

public class WaterCamera : MonoBehaviour
{
	public Material Wobble;

	public Color underwaterColor;

	public BlendMode Blend;

	[Header("Shaders")]
	[Space]
	public Shader multiply;

	public Shader overlay;

	public Shader screen;

	public bool effectActive;

	private void Update()
	{
		switch (Blend)
		{
		case BlendMode.Multiply:
			Wobble.shader = multiply;
			break;
		case BlendMode.Overlay:
			Wobble.shader = overlay;
			break;
		case BlendMode.Screen:
			Wobble.shader = screen;
			break;
		}
	}

	public void SetBlend(int mode)
	{
		Blend = (BlendMode)mode;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (Wobble == null)
		{
			Graphics.Blit(source, destination);
		}
		else if (effectActive)
		{
			Wobble.SetColor("_Color", underwaterColor);
			Graphics.Blit(source, destination, Wobble);
		}
		else
		{
			Wobble.SetColor("_Color", Color.white);
			Graphics.Blit(source, destination);
		}
	}
}
