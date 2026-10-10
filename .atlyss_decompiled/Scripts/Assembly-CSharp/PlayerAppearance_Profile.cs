using System;

[Serializable]
public class PlayerAppearance_Profile
{
	public string _setRaceTag;

	public int _setEye;

	public int _setMouth;

	public int _setHairStyle;

	public int _setEar;

	public int _setTail;

	public int _setMisc;

	public bool _displayBoobs;

	public bool _isLeftHanded;

	public float _voicePitch;

	public int _helmDyeIndex;

	public float _headWidth;

	public float _muzzleWeight;

	public float _heightWeight = 1f;

	public float _widthWeight = 1f;

	public float _torsoWeight;

	public float _armWeight;

	public float _boobWeight;

	public float _bellyWeight;

	public float _bottomWeight;

	public int _setTexture;

	public bool _hideHelm;

	public bool _hideCape;

	public bool _hideChest;

	public bool _hideLeggings;

	public bool _hideVanityHelm;

	public bool _hideVanityCape;

	public bool _hideVanityChest;

	public bool _hideVanityLeggings;

	public ColorAdjustShader_Profile _hairColorProfile;

	public bool _hairIsBodyColor;

	public ColorAdjustShader_Profile _miscColorProfile;

	public ColorAdjustShader_Profile _setSkinColorProfile;
}
