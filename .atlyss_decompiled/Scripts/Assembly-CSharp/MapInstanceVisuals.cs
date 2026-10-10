using System;
using UnityEngine;

[Serializable]
public class MapInstanceVisuals
{
	[Header("Day/Night Cycle Conditions")]
	public bool _isInteriorInstance;

	public Color _forceSkyColor = Color.white;

	public bool _useForceSkyColor;

	[Space]
	public Color _directionalLightColor = Color.white;

	public Color _skySolidColor = Color.white;

	public Color _ambientColor = Color.white;

	[Space]
	public Color _directionalLightNightColor = Color.black;

	public Color _skySolidNightColor = Color.black;

	public Color _ambientNightColor = Color.black;

	[Space]
	public Vector3 _directionalLightNightRotation = Vector3.zero;

	[Header("Fog")]
	public FogMode _fogMode = FogMode.ExponentialSquared;

	public float _skybox_fogDensity = 0.03f;

	public float _mainCam_fogDensity = 0.03f;

	public bool _enableFog = true;

	[Header("Weather Conditions")]
	public ParticleSystem _weatherParticleSystem;

	[Range(0f, 100f)]
	public int _chanceForWeather;

	public float _weatherIntervalBuffer = 250.89f;
}
