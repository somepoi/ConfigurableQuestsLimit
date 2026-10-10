using System;
using UnityEngine;

[Serializable]
public struct PlayerAppearanceStruct
{
	public string _setRaceTag;

	public int _eyeID;

	public int _mouthID;

	public int _hairStyleID;

	public int _earID;

	public int _tailID;

	public int _miscID;

	public bool _displayBoobs;

	public bool _isLeftHanded;

	[Space]
	public int _dyeIndex;

	public float _headWidth;

	public float _muzzleWeight;

	public float _voicePitch;

	[Space]
	public float _heightWeight;

	public float _widthWeight;

	public float _torsoWeight;

	public float _armWeight;

	public float _boobWeight;

	public float _bellyWeight;

	public float _bottomWeight;

	[Space]
	public int _textureID;

	public float _bodyColorHue;

	public float _bodyColorBrightness;

	public float _bodyColorContrast;

	public float _bodyColorSaturation;

	[Space]
	public float _hairStyleHue;

	public float _hairStyleBrightness;

	public float _hairStyleContrast;

	public float _hairStyleSaturation;

	public bool _hairIsBodyColor;

	[Space]
	public float _miscHue;

	public float _miscBrightness;

	public float _miscContrast;

	public float _miscSaturation;

	[Space]
	public bool _hideHelm;

	public bool _hideCape;

	public bool _hideChest;

	public bool _hideLeggings;

	[Space]
	public bool _hideVanityHelm;

	public bool _hideVanityCape;

	public bool _hideVanityChest;

	public bool _hideVanityLeggings;
}
