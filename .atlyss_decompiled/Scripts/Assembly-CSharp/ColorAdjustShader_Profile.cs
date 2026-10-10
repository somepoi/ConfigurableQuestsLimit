using System;
using UnityEngine;

[Serializable]
public class ColorAdjustShader_Profile
{
	[Range(-360f, 360f)]
	public float _hue;

	[Range(0f, 2f)]
	public float _saturation;

	[Range(-1f, 1f)]
	public float _brightness;

	[Range(0f, 2f)]
	public float _contrast;
}
