using UnityEngine;

[CreateAssetMenu(fileName = "playerRace_", menuName = "ScriptableObjects/Game Logic/Playable Race", order = 0)]
public class ScriptablePlayerRace : ScriptableObject
{
	[Header("Scriptable Player Race")]
	public string _raceName;

	public Sprite _raceAvatar;

	public Sprite _raceEmblemIcon;

	[TextArea(3, 8)]
	public string _raceDescription;

	[Header("Inventory")]
	public ScriptableChestpiece _startingChestpiece;

	public ScriptableLeggings _startingLeggings;

	[Header("Racial Skill")]
	public ScriptableSkill _racialSkill;

	[Header("Material")]
	public int _headMaterialIndex;

	public int _bodyMaterialIndex;

	[Space]
	public int _armMaterialIndex;

	public int _armMaterialIndex_2;

	public bool _useArmMaterialIndex_2;

	[Space]
	public int _upperLegMaterialIndex;

	public int _lowerLegMaterialIndex;

	public int _feetMaterialIndex;

	public bool _useFeetMaterialIndex;

	[Header("Display")]
	public GameObject _raceModel;

	public CharacterParamsGroup _raceDisplayParams;

	[Space]
	public Mesh[] _hairStyleMeshes;

	public Mesh _helmHairMesh;

	public bool _muzzleWeightIsHead;

	public bool _feetUseThighWeights;

	[Space]
	public SkinTextureGroup[] _skinTextureGroups;

	public EyeTextureGroup[] _eyeTextures;

	public MouthTextureGroup[] _mouthTextures;

	[Space]
	public PlayerAppearanceStruct[] _appearanceTemplates = new PlayerAppearanceStruct[0];

	[Header("Character Creation Parameters")]
	public string _miscName = "Misc";

	public bool _disableEarSelector;

	public bool _disableMouthSelector;

	public bool _randomizeHairStylesOnCC;

	public bool _defaultHairColorToBodyValue;

	public bool _defaultMiscColorValues;

	public Vector3 _headFocusPosition = new Vector3(0f, 2f, 3f);

	[Header("Audio")]
	[Range(-3f, 3f)]
	public float _footstepPitch = 1f;

	public AudioClip[] _chatVoiceClips;

	public PlayerAppearance_Profile Init_ParamsCheck(PlayerAppearance_Profile _aP)
	{
		float _bodyHue = _aP._setSkinColorProfile._hue;
		float _bodyBrightness = _aP._setSkinColorProfile._brightness;
		float _bodyContrast = _aP._setSkinColorProfile._contrast;
		float _bodySaturation = _aP._setSkinColorProfile._saturation;
		float _hairHue = _aP._hairColorProfile._hue;
		float _hairBrightness = _aP._hairColorProfile._brightness;
		float _hairContrast = _aP._hairColorProfile._contrast;
		float _hairSaturation = _aP._hairColorProfile._saturation;
		float _miscHue = _aP._miscColorProfile._hue;
		float _miscBrightness = _aP._miscColorProfile._brightness;
		float _miscContrast = _aP._miscColorProfile._contrast;
		float _miscSaturation = _aP._miscColorProfile._saturation;
		Init_ClampCharacterColorParams();
		float _setHeadWidthWeight = _aP._headWidth;
		float _setMuzzleWeight = _aP._muzzleWeight;
		float _setHeightWeight = _aP._heightWeight;
		float _setWidthWeight = _aP._widthWeight;
		float _setTorsoWeight = _aP._torsoWeight;
		float _setBoobWeight = _aP._boobWeight;
		float _setArmWeight = _aP._armWeight;
		float _setBellyWeight = _aP._bellyWeight;
		float _setBottomWeight = _aP._bottomWeight;
		float _setVoicePitch = _aP._voicePitch;
		Init_ClampCharacterParams();
		return _aP;
		void Init_ClampCharacterColorParams()
		{
			if (_bodyHue < _raceDisplayParams._bodyHueRange.x)
			{
				_bodyHue = _raceDisplayParams._bodyHueRange.x;
			}
			else if (_bodyHue > _raceDisplayParams._bodyHueRange.y)
			{
				_bodyHue = _raceDisplayParams._bodyHueRange.y;
			}
			if (_bodyBrightness < _raceDisplayParams._bodyBrightnessRange.x)
			{
				_bodyBrightness = _raceDisplayParams._bodyBrightnessRange.x;
			}
			else if (_bodyBrightness > _raceDisplayParams._bodyBrightnessRange.y)
			{
				_bodyBrightness = _raceDisplayParams._bodyBrightnessRange.y;
			}
			if (_bodyContrast < _raceDisplayParams._bodyContrastRange.x)
			{
				_bodyContrast = _raceDisplayParams._bodyContrastRange.x;
			}
			else if (_bodyContrast > _raceDisplayParams._bodyContrastRange.y)
			{
				_bodyContrast = _raceDisplayParams._bodyContrastRange.y;
			}
			if (_bodySaturation < _raceDisplayParams._bodySaturationRange.x)
			{
				_bodySaturation = _raceDisplayParams._bodySaturationRange.x;
			}
			else if (_bodySaturation > _raceDisplayParams._bodySaturationRange.y)
			{
				_bodySaturation = _raceDisplayParams._bodySaturationRange.y;
			}
			if (_hairHue < _raceDisplayParams._bodyHueRange.x)
			{
				_hairHue = _raceDisplayParams._bodyHueRange.x;
			}
			else if (_hairHue > _raceDisplayParams._bodyHueRange.y)
			{
				_hairHue = _raceDisplayParams._bodyHueRange.y;
			}
			if (_hairBrightness < _raceDisplayParams._bodyBrightnessRange.x)
			{
				_hairBrightness = _raceDisplayParams._bodyBrightnessRange.x;
			}
			else if (_hairBrightness > _raceDisplayParams._bodyBrightnessRange.y)
			{
				_hairBrightness = _raceDisplayParams._bodyBrightnessRange.y;
			}
			if (_hairContrast < _raceDisplayParams._bodyContrastRange.x)
			{
				_hairContrast = _raceDisplayParams._bodyContrastRange.x;
			}
			else if (_hairContrast > _raceDisplayParams._bodyContrastRange.y)
			{
				_hairContrast = _raceDisplayParams._bodyContrastRange.y;
			}
			if (_hairSaturation < _raceDisplayParams._bodySaturationRange.x)
			{
				_hairSaturation = _raceDisplayParams._bodySaturationRange.x;
			}
			else if (_hairSaturation > _raceDisplayParams._bodySaturationRange.y)
			{
				_hairSaturation = _raceDisplayParams._bodySaturationRange.y;
			}
			if (_miscHue < _raceDisplayParams._bodyHueRange.x)
			{
				_miscHue = _raceDisplayParams._bodyHueRange.x;
			}
			else if (_miscHue > _raceDisplayParams._bodyHueRange.y)
			{
				_miscHue = _raceDisplayParams._bodyHueRange.y;
			}
			if (_miscBrightness < _raceDisplayParams._bodyBrightnessRange.x)
			{
				_miscBrightness = _raceDisplayParams._bodyBrightnessRange.x;
			}
			else if (_miscBrightness > _raceDisplayParams._bodyBrightnessRange.y)
			{
				_miscBrightness = _raceDisplayParams._bodyBrightnessRange.y;
			}
			if (_miscContrast < _raceDisplayParams._bodyContrastRange.x)
			{
				_miscContrast = _raceDisplayParams._bodyContrastRange.x;
			}
			else if (_miscContrast > _raceDisplayParams._bodyContrastRange.y)
			{
				_miscContrast = _raceDisplayParams._bodyContrastRange.y;
			}
			if (_miscSaturation < _raceDisplayParams._bodySaturationRange.x)
			{
				_miscSaturation = _raceDisplayParams._bodySaturationRange.x;
			}
			else if (_miscSaturation > _raceDisplayParams._bodySaturationRange.y)
			{
				_miscSaturation = _raceDisplayParams._bodySaturationRange.y;
			}
			_bodyHue = GameManager._current.ConvertFloat_ToN2(_bodyHue);
			_bodyBrightness = GameManager._current.ConvertFloat_ToN2(_bodyBrightness);
			_bodyContrast = GameManager._current.ConvertFloat_ToN2(_bodyContrast);
			_bodySaturation = GameManager._current.ConvertFloat_ToN2(_bodySaturation);
			_hairHue = GameManager._current.ConvertFloat_ToN2(_hairHue);
			_hairBrightness = GameManager._current.ConvertFloat_ToN2(_hairBrightness);
			_hairContrast = GameManager._current.ConvertFloat_ToN2(_hairContrast);
			_hairSaturation = GameManager._current.ConvertFloat_ToN2(_hairSaturation);
			_miscHue = GameManager._current.ConvertFloat_ToN2(_miscHue);
			_miscBrightness = GameManager._current.ConvertFloat_ToN2(_miscBrightness);
			_miscContrast = GameManager._current.ConvertFloat_ToN2(_miscContrast);
			_miscSaturation = GameManager._current.ConvertFloat_ToN2(_miscSaturation);
			_aP._setSkinColorProfile._hue = _bodyHue;
			_aP._setSkinColorProfile._brightness = _bodyBrightness;
			_aP._setSkinColorProfile._contrast = _bodyContrast;
			_aP._setSkinColorProfile._saturation = _bodySaturation;
			_aP._hairColorProfile._hue = _hairHue;
			_aP._hairColorProfile._brightness = _hairBrightness;
			_aP._hairColorProfile._contrast = _hairContrast;
			_aP._hairColorProfile._saturation = _hairSaturation;
			_aP._miscColorProfile._hue = _miscHue;
			_aP._miscColorProfile._brightness = _miscBrightness;
			_aP._miscColorProfile._contrast = _miscContrast;
			_aP._miscColorProfile._saturation = _miscSaturation;
		}
		void Init_ClampCharacterParams()
		{
			if (_setHeadWidthWeight < _raceDisplayParams._headWidthRange.x)
			{
				_setHeadWidthWeight = _raceDisplayParams._headWidthRange.x;
			}
			else if (_setHeadWidthWeight > _raceDisplayParams._headWidthRange.y)
			{
				_setHeadWidthWeight = _raceDisplayParams._headWidthRange.y;
			}
			if (_setMuzzleWeight < _raceDisplayParams._headModRange.x)
			{
				_setMuzzleWeight = _raceDisplayParams._headModRange.x;
			}
			else if (_setMuzzleWeight > _raceDisplayParams._headModRange.y)
			{
				_setMuzzleWeight = _raceDisplayParams._headModRange.y;
			}
			if (_setHeightWeight < _raceDisplayParams._heightRange.x)
			{
				_setHeightWeight = _raceDisplayParams._heightRange.x;
			}
			else if (_setHeightWeight > _raceDisplayParams._heightRange.y)
			{
				_setHeightWeight = _raceDisplayParams._heightRange.y;
			}
			if (_setWidthWeight < _raceDisplayParams._widthRange.x)
			{
				_setWidthWeight = _raceDisplayParams._widthRange.x;
			}
			else if (_setWidthWeight > _raceDisplayParams._widthRange.y)
			{
				_setWidthWeight = _raceDisplayParams._widthRange.y;
			}
			if (_setTorsoWeight < _raceDisplayParams._torsoRange.x)
			{
				_setTorsoWeight = _raceDisplayParams._torsoRange.x;
			}
			else if (_setTorsoWeight > _raceDisplayParams._torsoRange.y)
			{
				_setTorsoWeight = _raceDisplayParams._torsoRange.y;
			}
			if (_aP._displayBoobs)
			{
				_setTorsoWeight = 0f;
			}
			else
			{
				_setBoobWeight = 0f;
			}
			if (_setBoobWeight < _raceDisplayParams._boobRange.x)
			{
				_setBoobWeight = _raceDisplayParams._boobRange.x;
			}
			else if (_setBoobWeight > _raceDisplayParams._boobRange.y)
			{
				_setBoobWeight = _raceDisplayParams._boobRange.y;
			}
			if (_setArmWeight < _raceDisplayParams._armRange.x)
			{
				_setArmWeight = _raceDisplayParams._armRange.x;
			}
			else if (_setArmWeight > _raceDisplayParams._armRange.y)
			{
				_setArmWeight = _raceDisplayParams._armRange.y;
			}
			if (_setBellyWeight < _raceDisplayParams._bellyRange.x)
			{
				_setBellyWeight = _raceDisplayParams._bellyRange.x;
			}
			else if (_setBellyWeight > _raceDisplayParams._bellyRange.y)
			{
				_setBellyWeight = _raceDisplayParams._bellyRange.y;
			}
			if (_setBottomWeight < _raceDisplayParams._bottomRange.x)
			{
				_setBottomWeight = _raceDisplayParams._bottomRange.x;
			}
			else if (_setBottomWeight > _raceDisplayParams._bottomRange.y)
			{
				_setBottomWeight = _raceDisplayParams._bottomRange.y;
			}
			if (_setVoicePitch < _raceDisplayParams._pitchRange.x)
			{
				_setVoicePitch = _raceDisplayParams._pitchRange.x;
			}
			else if (_setVoicePitch > _raceDisplayParams._pitchRange.y)
			{
				_setVoicePitch = _raceDisplayParams._pitchRange.y;
			}
			_setHeadWidthWeight = GameManager._current.ConvertFloat_ToN2(_setHeadWidthWeight);
			_setMuzzleWeight = GameManager._current.ConvertFloat_ToN2(_setMuzzleWeight);
			_setHeightWeight = GameManager._current.ConvertFloat_ToN2(_setHeightWeight);
			_setWidthWeight = GameManager._current.ConvertFloat_ToN2(_setWidthWeight);
			_setTorsoWeight = GameManager._current.ConvertFloat_ToN2(_setTorsoWeight);
			_setBoobWeight = GameManager._current.ConvertFloat_ToN2(_setBoobWeight);
			_setArmWeight = GameManager._current.ConvertFloat_ToN2(_setArmWeight);
			_setBellyWeight = GameManager._current.ConvertFloat_ToN2(_setBellyWeight);
			_setBottomWeight = GameManager._current.ConvertFloat_ToN2(_setBottomWeight);
			_setVoicePitch = GameManager._current.ConvertFloat_ToN2(_setVoicePitch);
			_aP._headWidth = _setHeadWidthWeight;
			_aP._muzzleWeight = _setMuzzleWeight;
			_aP._heightWeight = _setHeightWeight;
			_aP._widthWeight = _setWidthWeight;
			_aP._torsoWeight = _setTorsoWeight;
			_aP._boobWeight = _setBoobWeight;
			_aP._armWeight = _setArmWeight;
			_aP._bellyWeight = _setBellyWeight;
			_aP._bottomWeight = _setBottomWeight;
			_aP._voicePitch = _setVoicePitch;
		}
	}
}
