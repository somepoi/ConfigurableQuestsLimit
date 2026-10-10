using System;
using UnityEngine;

[Serializable]
public class CharacterParamsGroup
{
	[Header("Color")]
	public Sprite _bodyHueSliderSprite;

	public Sprite _miscHueSliderSprite;

	[Space]
	public Vector2 _bodyHueRange = new Vector2(0f, 360f);

	public Vector2 _bodyBrightnessRange = new Vector2(-0.35f, 0.1f);

	public Vector2 _bodyContrastRange = new Vector2(0.75f, 1.45f);

	public Vector2 _bodySaturationRange = new Vector2(0f, 2f);

	[Header("Weights")]
	public Vector2 _headWidthRange = new Vector2(0.95f, 1.15f);

	public Vector2 _headModRange = new Vector2(0f, 100f);

	public Vector2 _heightRange = new Vector2(0.95f, 1.055f);

	public Vector2 _widthRange = new Vector2(0.95f, 1.15f);

	public Vector2 _torsoRange = new Vector2(-10f, 105f);

	public Vector2 _boobRange = new Vector2(-20f, 100f);

	public Vector2 _armRange = new Vector2(-5f, 105f);

	public Vector2 _bellyRange = new Vector2(-20f, 120f);

	public Vector2 _bottomRange = new Vector2(-50f, 100f);

	[Header("Audio")]
	public Vector2 _pitchRange = new Vector2(0.6f, 1.25f);
}
