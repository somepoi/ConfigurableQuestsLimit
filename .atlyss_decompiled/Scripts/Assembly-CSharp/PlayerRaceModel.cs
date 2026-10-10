using UnityEngine;

public class PlayerRaceModel : MonoBehaviour
{
	private Player _player;

	public RaceModelEquipDisplay _equipDisplay;

	private static readonly int _HUE = Shader.PropertyToID("_Hue");

	private static readonly int _SATURATION = Shader.PropertyToID("_Saturation");

	private static readonly int _BRIGHTNESS = Shader.PropertyToID("_Brightness");

	private static readonly int _CONTRAST = Shader.PropertyToID("_Contrast");

	private static readonly int _TINT = Shader.PropertyToID("_ColorTint");

	public ScriptablePlayerRace _scriptablePlayerRace;

	public bool _isCharacterCreationDisplay;

	public Material _baseHeadMaterial;

	public Material _baseLegMaterial;

	private Material[] _cachedBaseBodyMaterials;

	private Material[] _cachedArmorBodyMaterials;

	[Header("Base Display")]
	public Transform _armatureTransform;

	public Transform _headBoneTransform;

	public SkinnedMeshRenderer _baseBodyMesh;

	public SkinnedMeshRenderer _baseArmMesh;

	public SkinnedMeshRenderer _baseBoobMesh;

	public SkinnedMeshRenderer _baseFeetMesh;

	public Vector3 _originHeadScale = Vector3.one;

	public bool _displayBoobs;

	private bool _restBlink;

	private bool _isIdleAnim = true;

	public bool _canAltIdleAnim = true;

	public bool _headCulled;

	public bool _legsCulled;

	private int _setTextureIndex;

	public ColorAdjustShader_Profile _setSkinColorProfile;

	[Header("Animator Params")]
	public float _baseMovementMultiplier = 1f;

	public float _ledgeClimbOffset = 1f;

	public Animator _raceAnimator;

	public bool _hasAltMovementAnim;

	[Header("Hands")]
	public Transform _leftHandBone;

	public Transform _rightHandBone;

	[Header("Blendshape Weights")]
	public float _headWidth = 1f;

	public float _muzzleWeight;

	public float _heightWeight = 1f;

	public float _widthWeight = 1f;

	public float _torsoWeight;

	public float _armWeight;

	public float _boobWeight;

	public float _bellyWeight;

	public float _bottomWeight;

	[Header("Dynamic Bones")]
	public DynamicBone[] _boobDynamicBones = new DynamicBone[2];

	public DynamicBone[] _assDynamicBones = new DynamicBone[2];

	public DynamicBone[] _bellyDynamicBone;

	[Header("Eyes / Mouth")]
	public SkinnedMeshRenderer _faceMesh;

	private int _setEyeIndex;

	private int _setMouthIndex;

	private EyeCondition _currentEyeCondition;

	private MouthCondition _currentMouthCondition;

	private float _eyeConditionBuffer;

	private float _mouthConditionBuffer;

	private float _currentEyeConditionBuffer;

	private float _currentMouthConditionBuffer;

	[HideInInspector]
	public bool _eyeConditionIsInfinite;

	[HideInInspector]
	public bool _mouthConditionIsInfinite;

	[Header("Ears")]
	[SerializeField]
	private Transform _earDisplayContainer;

	public SkinnedMeshRenderer[] _earDisplays;

	public bool _hideEars;

	private int _setEarIndex;

	[Header("HairStyles")]
	public MeshFilter _hairStyleMeshFilter;

	public MeshRenderer _hairStyleMeshRenderer;

	public bool _useHelmHair;

	public ColorAdjustShader_Profile _setHairColorProfile;

	public bool _hairHueIsBodyHue;

	public bool _muzzleWeightIsHead;

	private int _setHairIndex;

	[Header("Misc Objects")]
	public Transform _miscDisplayContainer;

	public MeshRenderer[] _miscDisplays;

	public SkinnedMeshRenderer[] _miscSkinnedDisplays;

	public ColorAdjustShader_Profile _setMiscColorProfile;

	public bool _miscDisplayIsMouth;

	public bool _copyBodyColorProfile;

	private int _setMiscIndex;

	[Header("Tails")]
	[SerializeField]
	private Transform _tailDisplayContainer;

	public SkinnedMeshRenderer[] _tailDisplays;

	[SerializeField]
	private bool _tailIsBodyColor;

	private int _setTailIndex;

	public int SetTextureIndex
	{
		get
		{
			return _setTextureIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _scriptablePlayerRace._skinTextureGroups.Length - 1;
			}
			if (value >= _scriptablePlayerRace._skinTextureGroups.Length)
			{
				value = 0;
			}
			_setTextureIndex = value;
		}
	}

	public int SetEyeIndex
	{
		get
		{
			return _setEyeIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _scriptablePlayerRace._eyeTextures.Length - 1;
			}
			if (value >= _scriptablePlayerRace._eyeTextures.Length)
			{
				value = 0;
			}
			_setEyeIndex = value;
		}
	}

	public int SetMouthIndex
	{
		get
		{
			return _setMouthIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _scriptablePlayerRace._mouthTextures.Length - 1;
			}
			if (value >= _scriptablePlayerRace._mouthTextures.Length)
			{
				value = 0;
			}
			_setMouthIndex = value;
		}
	}

	public int SetEarIndex
	{
		get
		{
			return _setEarIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _earDisplays.Length - 1;
			}
			if (value >= _earDisplays.Length)
			{
				value = 0;
			}
			_setEarIndex = value;
		}
	}

	public int SetHairIndex
	{
		get
		{
			return _setHairIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _scriptablePlayerRace._hairStyleMeshes.Length - 1;
			}
			if (value >= _scriptablePlayerRace._hairStyleMeshes.Length)
			{
				value = 0;
			}
			_setHairIndex = value;
		}
	}

	public int SetMiscIndex
	{
		get
		{
			return _setMiscIndex;
		}
		set
		{
			if (value < 0)
			{
				if (_miscDisplays.Length != 0)
				{
					value = _miscDisplays.Length - 1;
				}
				if (_miscSkinnedDisplays.Length != 0)
				{
					value = _miscSkinnedDisplays.Length - 1;
				}
			}
			if (_miscDisplays.Length != 0 && value >= _miscDisplays.Length)
			{
				value = 0;
			}
			if (_miscSkinnedDisplays.Length != 0 && value >= _miscSkinnedDisplays.Length)
			{
				value = 0;
			}
			_setMiscIndex = value;
		}
	}

	public int SetTailIndex
	{
		get
		{
			return _setTailIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _tailDisplays.Length - 1;
			}
			if (value >= _tailDisplays.Length)
			{
				value = 0;
			}
			_setTailIndex = value;
		}
	}

	private void Awake()
	{
		_player = GetComponentInParent<Player>();
		_equipDisplay = GetComponent<RaceModelEquipDisplay>();
		_earDisplays = _earDisplayContainer.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		_tailDisplays = _tailDisplayContainer.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		_miscDisplays = _miscDisplayContainer.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
		_miscSkinnedDisplays = _miscDisplayContainer.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		_raceAnimator = GetComponentInChildren<Animator>();
		_baseHeadMaterial = _baseBodyMesh.material;
		_baseLegMaterial = _baseBodyMesh.materials[_scriptablePlayerRace._lowerLegMaterialIndex];
		_cachedBaseBodyMaterials = new Material[0];
		_cachedArmorBodyMaterials = new Material[0];
	}

	private void Update()
	{
		Handle_CharacterBlinkEffect();
		Handle_CharacterAltIdle();
		Handle_CharacterExpression();
		_baseBoobMesh.enabled = _displayBoobs;
		if (_displayBoobs)
		{
			_torsoWeight = 0f;
		}
		else
		{
			_boobWeight = 0f;
		}
		if ((bool)SettingsManager._current)
		{
			if (SettingsManager._current._jiggleBonesToggle.isOn)
			{
				_assDynamicBones[0].enabled = false;
				_assDynamicBones[1].enabled = false;
				_bellyDynamicBone[0].enabled = false;
				_boobDynamicBones[0].enabled = false;
				_boobDynamicBones[1].enabled = false;
			}
			else
			{
				Set_DynamicBoneParams();
			}
		}
	}

	private void Handle_CharacterExpression()
	{
		Material[] materials = _faceMesh.materials;
		switch (_currentEyeCondition)
		{
		case EyeCondition.Center:
			materials[0].mainTexture = _scriptablePlayerRace._eyeTextures[_setEyeIndex]._center;
			break;
		case EyeCondition.Up:
			materials[0].mainTexture = _scriptablePlayerRace._eyeTextures[_setEyeIndex]._up;
			break;
		case EyeCondition.Down:
			materials[0].mainTexture = _scriptablePlayerRace._eyeTextures[_setEyeIndex]._down;
			break;
		case EyeCondition.Left:
			materials[0].mainTexture = _scriptablePlayerRace._eyeTextures[_setEyeIndex]._left;
			break;
		case EyeCondition.Right:
			materials[0].mainTexture = _scriptablePlayerRace._eyeTextures[_setEyeIndex]._right;
			break;
		case EyeCondition.Hurt:
			materials[0].mainTexture = _scriptablePlayerRace._eyeTextures[_setEyeIndex]._hurt;
			break;
		case EyeCondition.Pissed:
			materials[0].mainTexture = _scriptablePlayerRace._eyeTextures[_setEyeIndex]._pissed;
			break;
		case EyeCondition.Closed:
			materials[0].mainTexture = _scriptablePlayerRace._eyeTextures[_setEyeIndex]._closed;
			break;
		}
		switch (_currentMouthCondition)
		{
		case MouthCondition.Closed:
			materials[1].mainTexture = _scriptablePlayerRace._mouthTextures[_setMouthIndex]._closed;
			if (_miscDisplayIsMouth)
			{
				_miscSkinnedDisplays[_setMiscIndex].SetBlendShapeWeight(0, 0f);
			}
			break;
		case MouthCondition.Open:
			materials[1].mainTexture = _scriptablePlayerRace._mouthTextures[_setMouthIndex]._open;
			if (_miscDisplayIsMouth)
			{
				_miscSkinnedDisplays[_setMiscIndex].SetBlendShapeWeight(0, 100f);
			}
			break;
		}
		if (_currentEyeConditionBuffer < _eyeConditionBuffer && !_eyeConditionIsInfinite)
		{
			_currentEyeConditionBuffer += Time.deltaTime;
		}
		if (_currentMouthConditionBuffer < _mouthConditionBuffer && !_mouthConditionIsInfinite)
		{
			_currentMouthConditionBuffer += Time.deltaTime;
		}
		if (!_eyeConditionIsInfinite && _currentEyeConditionBuffer >= _eyeConditionBuffer && _currentEyeCondition != EyeCondition.Center)
		{
			_currentEyeCondition = EyeCondition.Center;
		}
		if (!_mouthConditionIsInfinite && _currentMouthConditionBuffer >= _mouthConditionBuffer && _currentMouthCondition != MouthCondition.Closed)
		{
			_currentMouthCondition = MouthCondition.Closed;
		}
	}

	private void ResetRestBlink()
	{
		_restBlink = false;
	}

	private void Handle_CharacterBlinkEffect()
	{
		if (!_isCharacterCreationDisplay && (bool)GetComponentInParent<Player>() && GetComponentInParent<Player>()._currentPlayerCondition == PlayerCondition.DEAD)
		{
			_faceMesh.SetBlendShapeWeight(0, 1f);
			Set_EyeCondition(EyeCondition.Closed, 1f);
			return;
		}
		if (!_restBlink && Random.Range(0, 1200) >= 1150)
		{
			_faceMesh.SetBlendShapeWeight(0, 100f);
			Invoke("ResetRestBlink", Random.Range(0.1f, 4.85f));
			_restBlink = true;
		}
		_faceMesh.SetBlendShapeWeight(0, Mathf.Lerp(_faceMesh.GetBlendShapeWeight(0), 0f, 9.5f * Time.deltaTime));
	}

	private void ResetIdleAlt()
	{
		_isIdleAnim = true;
	}

	private void Handle_CharacterAltIdle()
	{
		if ((bool)_player)
		{
			PlayerVisual pVisual = _player._pVisual;
			PlayerCombat pCombat = _player._pCombat;
			PlayerCasting pCasting = _player._pCasting;
			if ((pVisual.isLocalPlayer && Player._mainPlayer._currentPlayerAction != PlayerAction.IDLE) || pCasting._currentCastSkill != null || pCombat._isAirAttacked)
			{
				return;
			}
		}
		if (_isIdleAnim && _canAltIdleAnim && Random.Range(0, 2800) <= 10 && !_raceAnimator.GetBool("_inMovement"))
		{
			_raceAnimator.CrossFade("Idle_02", 0.12f, 0);
			_isIdleAnim = false;
			Invoke("ResetIdleAlt", Random.Range(23.85f, 40.25f));
		}
	}

	public void Init_CullHead(bool _isTrue)
	{
		if ((_headCulled & _isTrue) || (!_headCulled && !_isTrue))
		{
			return;
		}
		if (_cachedBaseBodyMaterials.Length == 0)
		{
			_cachedBaseBodyMaterials = _baseBodyMesh.materials;
		}
		if (_isTrue)
		{
			_cachedBaseBodyMaterials[_scriptablePlayerRace._headMaterialIndex] = _equipDisplay._maskMaterial;
			_baseBodyMesh.materials = _cachedBaseBodyMaterials;
			_hairStyleMeshRenderer.enabled = false;
			_faceMesh.enabled = false;
			if (_miscSkinnedDisplays != null && _miscSkinnedDisplays.Length != 0)
			{
				_miscSkinnedDisplays[_setMiscIndex].enabled = false;
			}
		}
		else
		{
			if (_cachedBaseBodyMaterials[_scriptablePlayerRace._headMaterialIndex] != _baseHeadMaterial)
			{
				_cachedBaseBodyMaterials[_scriptablePlayerRace._headMaterialIndex] = _baseHeadMaterial;
			}
			_baseBodyMesh.materials = _cachedBaseBodyMaterials;
			_hairStyleMeshRenderer.enabled = true;
			_faceMesh.enabled = true;
			if (_miscSkinnedDisplays != null && _miscSkinnedDisplays.Length != 0)
			{
				_miscSkinnedDisplays[_setMiscIndex].enabled = true;
			}
			Apply_CharacterDisplay();
		}
		_headCulled = _isTrue;
	}

	public void Init_CullLegs(bool _isTrue)
	{
		if (!(_legsCulled & _isTrue) && (_legsCulled || _isTrue))
		{
			if (_cachedBaseBodyMaterials.Length == 0)
			{
				_cachedBaseBodyMaterials = _baseBodyMesh.materials;
			}
			if (_cachedArmorBodyMaterials.Length == 0)
			{
				_cachedArmorBodyMaterials = _equipDisplay._armorBodyMesh.materials;
			}
			if (_isTrue)
			{
				_cachedBaseBodyMaterials[_scriptablePlayerRace._upperLegMaterialIndex] = _equipDisplay._maskMaterial;
				_cachedBaseBodyMaterials[_scriptablePlayerRace._lowerLegMaterialIndex] = _equipDisplay._maskMaterial;
				_cachedArmorBodyMaterials[_scriptablePlayerRace._upperLegMaterialIndex] = _equipDisplay._maskMaterial;
				_cachedArmorBodyMaterials[_scriptablePlayerRace._lowerLegMaterialIndex] = _equipDisplay._maskMaterial;
				_baseBodyMesh.materials = _cachedBaseBodyMaterials;
				_equipDisplay._armorBodyMesh.materials = _cachedArmorBodyMaterials;
			}
			else
			{
				_cachedBaseBodyMaterials[_scriptablePlayerRace._upperLegMaterialIndex] = _equipDisplay._upperLegMaterial;
				_cachedBaseBodyMaterials[_scriptablePlayerRace._lowerLegMaterialIndex] = _equipDisplay._lowerLegMaterial;
				_cachedArmorBodyMaterials[_scriptablePlayerRace._upperLegMaterialIndex] = _equipDisplay._upperLegArmorMaterial;
				_cachedArmorBodyMaterials[_scriptablePlayerRace._lowerLegMaterialIndex] = _equipDisplay._lowerLegArmorMaterial;
				_baseBodyMesh.materials = _cachedBaseBodyMaterials;
				_equipDisplay._armorBodyMesh.materials = _cachedArmorBodyMaterials;
			}
			_legsCulled = _isTrue;
		}
	}

	public void Apply_CharacterDisplay()
	{
		Material[] materials = _baseBodyMesh.materials;
		materials[_scriptablePlayerRace._headMaterialIndex].mainTexture = _scriptablePlayerRace._skinTextureGroups[_setTextureIndex]._headTexture;
		materials[_scriptablePlayerRace._bodyMaterialIndex].mainTexture = _scriptablePlayerRace._skinTextureGroups[_setTextureIndex]._bodyTexture;
		materials[_scriptablePlayerRace._upperLegMaterialIndex].mainTexture = _scriptablePlayerRace._skinTextureGroups[_setTextureIndex]._legTexture;
		materials[_scriptablePlayerRace._lowerLegMaterialIndex].mainTexture = _scriptablePlayerRace._skinTextureGroups[_setTextureIndex]._legTexture;
		_baseArmMesh.materials[_scriptablePlayerRace._armMaterialIndex].mainTexture = _scriptablePlayerRace._skinTextureGroups[_setTextureIndex]._bodyTexture;
		_baseFeetMesh.material.mainTexture = _scriptablePlayerRace._skinTextureGroups[_setTextureIndex]._legTexture;
		Material[] array = materials;
		foreach (Material obj in array)
		{
			obj.SetFloat(_HUE, _setSkinColorProfile._hue);
			obj.SetFloat(_BRIGHTNESS, _setSkinColorProfile._brightness);
			obj.SetFloat(_CONTRAST, _setSkinColorProfile._contrast);
			obj.SetFloat(_SATURATION, _setSkinColorProfile._saturation);
		}
		_baseBodyMesh.materials = materials;
		Material[] materials2 = _baseArmMesh.materials;
		materials2[_scriptablePlayerRace._armMaterialIndex].SetFloat(_HUE, _baseBodyMesh.material.GetFloat(_HUE));
		materials2[_scriptablePlayerRace._armMaterialIndex].SetFloat(_BRIGHTNESS, _baseBodyMesh.material.GetFloat(_BRIGHTNESS));
		materials2[_scriptablePlayerRace._armMaterialIndex].SetFloat(_CONTRAST, _baseBodyMesh.material.GetFloat(_CONTRAST));
		materials2[_scriptablePlayerRace._armMaterialIndex].SetFloat(_SATURATION, _baseBodyMesh.material.GetFloat(_SATURATION));
		if (_scriptablePlayerRace._useArmMaterialIndex_2)
		{
			materials2[_scriptablePlayerRace._armMaterialIndex_2].SetFloat(_HUE, _baseBodyMesh.material.GetFloat(_HUE));
			materials2[_scriptablePlayerRace._armMaterialIndex_2].SetFloat(_BRIGHTNESS, _baseBodyMesh.material.GetFloat(_BRIGHTNESS));
			materials2[_scriptablePlayerRace._armMaterialIndex_2].SetFloat(_CONTRAST, _baseBodyMesh.material.GetFloat(_CONTRAST));
			materials2[_scriptablePlayerRace._armMaterialIndex_2].SetFloat(_SATURATION, _baseBodyMesh.material.GetFloat(_SATURATION));
		}
		_baseArmMesh.materials = materials2;
		_baseFeetMesh.material.SetFloat(_HUE, _baseBodyMesh.material.GetFloat(_HUE));
		_baseFeetMesh.material.SetFloat(_BRIGHTNESS, _baseBodyMesh.material.GetFloat(_BRIGHTNESS));
		_baseFeetMesh.material.SetFloat(_CONTRAST, _baseBodyMesh.material.GetFloat(_CONTRAST));
		_baseFeetMesh.material.SetFloat(_SATURATION, _baseBodyMesh.material.GetFloat(_SATURATION));
		if (_scriptablePlayerRace._useFeetMaterialIndex)
		{
			Material[] materials3 = _baseFeetMesh.materials;
			materials3[_scriptablePlayerRace._feetMaterialIndex].SetFloat(_HUE, _baseBodyMesh.material.GetFloat(_HUE));
			materials3[_scriptablePlayerRace._feetMaterialIndex].SetFloat(_BRIGHTNESS, _baseBodyMesh.material.GetFloat(_BRIGHTNESS));
			materials3[_scriptablePlayerRace._feetMaterialIndex].SetFloat(_CONTRAST, _baseBodyMesh.material.GetFloat(_CONTRAST));
			materials3[_scriptablePlayerRace._feetMaterialIndex].SetFloat(_SATURATION, _baseBodyMesh.material.GetFloat(_SATURATION));
			_baseFeetMesh.materials = materials3;
		}
		Set_CharacterBlendshapeWeights();
		Set_CharacterBoobDisplay();
		Set_EarDisplay();
		Set_HairStyleDisplay();
		Set_MiscDisplay();
		Set_TailDisplay();
		void Set_CharacterBlendshapeWeights()
		{
			if (!_isCharacterCreationDisplay)
			{
				_baseBodyMesh.SetBlendShapeWeight(0, _bellyWeight);
				_baseBodyMesh.SetBlendShapeWeight(1, _bottomWeight);
				_baseBodyMesh.SetBlendShapeWeight(2, _torsoWeight);
				_muzzleWeightIsHead = _scriptablePlayerRace._muzzleWeightIsHead;
				if (_headCulled)
				{
					_baseBodyMesh.SetBlendShapeWeight(3, 0f);
				}
				else if (_muzzleWeightIsHead && _baseBodyMesh.GetBlendShapeWeight(4) != 0f)
				{
					_baseBodyMesh.SetBlendShapeWeight(3, 0f);
				}
				else
				{
					_baseBodyMesh.SetBlendShapeWeight(3, _muzzleWeight);
				}
				int index = 4;
				if (_equipDisplay._hasHeadBlendWeights)
				{
					index = 5;
				}
				if (_bottomWeight < 0f)
				{
					_baseBodyMesh.SetBlendShapeWeight(1, 0f);
					_baseBodyMesh.SetBlendShapeWeight(index, _bottomWeight * -1f);
					if (_scriptablePlayerRace._feetUseThighWeights)
					{
						_baseFeetMesh.SetBlendShapeWeight(0, _bottomWeight * -1f);
					}
				}
				else
				{
					_baseBodyMesh.SetBlendShapeWeight(index, 0f);
					if (_scriptablePlayerRace._feetUseThighWeights)
					{
						_baseFeetMesh.SetBlendShapeWeight(0, 0f);
					}
				}
				_faceMesh.SetBlendShapeWeight(1, _muzzleWeight);
				_baseArmMesh.SetBlendShapeWeight(0, _armWeight);
				_baseBoobMesh.SetBlendShapeWeight(0, _boobWeight);
			}
		}
		void Set_CharacterBoobDisplay()
		{
			if (_displayBoobs)
			{
				_baseBoobMesh.material.mainTexture = _scriptablePlayerRace._skinTextureGroups[_setTextureIndex]._bodyTexture;
				_baseBoobMesh.material.SetFloat(_HUE, _baseBodyMesh.material.GetFloat(_HUE));
				_baseBoobMesh.material.SetFloat(_BRIGHTNESS, _baseBodyMesh.material.GetFloat(_BRIGHTNESS));
				_baseBoobMesh.material.SetFloat(_CONTRAST, _baseBodyMesh.material.GetFloat(_CONTRAST));
				_baseBoobMesh.material.SetFloat(_SATURATION, _baseBodyMesh.material.GetFloat(_SATURATION));
			}
		}
		void Set_EarDisplay()
		{
			if (_earDisplays.Length != 0)
			{
				for (int j = 0; j < _earDisplays.Length; j++)
				{
					if (j != SetEarIndex)
					{
						_earDisplays[j].transform.parent.gameObject.SetActive(value: false);
					}
					else
					{
						_earDisplays[j].transform.parent.gameObject.SetActive(value: true);
					}
				}
				SkinnedMeshRenderer skinnedMeshRenderer = _earDisplays[_setEarIndex];
				if ((bool)skinnedMeshRenderer)
				{
					skinnedMeshRenderer.enabled = !_hideEars;
					if ((bool)_scriptablePlayerRace._skinTextureGroups[_setTextureIndex]._earTexture)
					{
						skinnedMeshRenderer.material.mainTexture = _scriptablePlayerRace._skinTextureGroups[_setTextureIndex]._earTexture;
					}
					skinnedMeshRenderer.material.SetFloat(_HUE, _baseBodyMesh.material.GetFloat(_HUE));
					skinnedMeshRenderer.material.SetFloat(_BRIGHTNESS, _baseBodyMesh.material.GetFloat(_BRIGHTNESS));
					skinnedMeshRenderer.material.SetFloat(_CONTRAST, _baseBodyMesh.material.GetFloat(_CONTRAST));
					skinnedMeshRenderer.material.SetFloat(_SATURATION, _baseBodyMesh.material.GetFloat(_SATURATION));
				}
			}
		}
		void Set_HairStyleDisplay()
		{
			if (_scriptablePlayerRace._hairStyleMeshes != null)
			{
				_hairStyleMeshRenderer.material.mainTexture = _scriptablePlayerRace._skinTextureGroups[_setTextureIndex]._hairTexture;
				if (_useHelmHair)
				{
					if (_scriptablePlayerRace._hairStyleMeshes[_setHairIndex] != null)
					{
						_hairStyleMeshFilter.mesh = _scriptablePlayerRace._helmHairMesh;
					}
				}
				else
				{
					_hairStyleMeshFilter.mesh = _scriptablePlayerRace._hairStyleMeshes[_setHairIndex];
				}
				if (_hairHueIsBodyHue)
				{
					_hairStyleMeshRenderer.material.SetFloat(_HUE, _baseBodyMesh.material.GetFloat(_HUE));
					_hairStyleMeshRenderer.material.SetFloat(_BRIGHTNESS, _baseBodyMesh.material.GetFloat(_BRIGHTNESS));
					_hairStyleMeshRenderer.material.SetFloat(_CONTRAST, _baseBodyMesh.material.GetFloat(_CONTRAST));
					_hairStyleMeshRenderer.material.SetFloat(_SATURATION, _baseBodyMesh.material.GetFloat(_SATURATION));
				}
				else
				{
					_hairStyleMeshRenderer.material.SetFloat(_HUE, _setHairColorProfile._hue);
					_hairStyleMeshRenderer.material.SetFloat(_BRIGHTNESS, _setHairColorProfile._brightness);
					_hairStyleMeshRenderer.material.SetFloat(_CONTRAST, _setHairColorProfile._contrast);
					_hairStyleMeshRenderer.material.SetFloat(_SATURATION, _setHairColorProfile._saturation);
				}
			}
		}
		void Set_MiscDisplay()
		{
			if (_miscDisplays.Length != 0)
			{
				MeshRenderer obj2 = _miscDisplays[_setMiscIndex];
				obj2.material.SetFloat(_HUE, _setMiscColorProfile._hue);
				obj2.material.SetFloat(_BRIGHTNESS, _setMiscColorProfile._brightness);
				obj2.material.SetFloat(_CONTRAST, _setMiscColorProfile._contrast);
				obj2.material.SetFloat(_SATURATION, _setMiscColorProfile._saturation);
				for (int j = 0; j < _miscDisplays.Length; j++)
				{
					if (j != _setMiscIndex)
					{
						_miscDisplays[j].gameObject.SetActive(value: false);
					}
					else
					{
						_miscDisplays[j].gameObject.SetActive(value: true);
					}
				}
			}
			if (_miscSkinnedDisplays.Length != 0)
			{
				SkinnedMeshRenderer obj3 = _miscSkinnedDisplays[_setMiscIndex];
				obj3.material.SetFloat(_HUE, _setMiscColorProfile._hue);
				obj3.material.SetFloat(_BRIGHTNESS, _setMiscColorProfile._brightness);
				obj3.material.SetFloat(_CONTRAST, _setMiscColorProfile._contrast);
				obj3.material.SetFloat(_SATURATION, _setMiscColorProfile._saturation);
				for (int k = 0; k < _miscSkinnedDisplays.Length; k++)
				{
					if (k != _setMiscIndex)
					{
						_miscSkinnedDisplays[k].gameObject.SetActive(value: false);
					}
					else
					{
						_miscSkinnedDisplays[k].gameObject.SetActive(value: true);
					}
				}
			}
		}
		void Set_TailDisplay()
		{
			if (_tailDisplays.Length != 0)
			{
				for (int j = 0; j < _tailDisplays.Length; j++)
				{
					if (j != _setTailIndex)
					{
						_tailDisplays[j].transform.parent.gameObject.SetActive(value: false);
					}
					else
					{
						_tailDisplays[j].transform.parent.gameObject.SetActive(value: true);
					}
				}
				SkinnedMeshRenderer skinnedMeshRenderer = _tailDisplays[_setTailIndex];
				if ((bool)_scriptablePlayerRace._skinTextureGroups[_setTextureIndex]._tailTexture)
				{
					skinnedMeshRenderer.material.mainTexture = _scriptablePlayerRace._skinTextureGroups[_setTextureIndex]._tailTexture;
				}
				if (_tailIsBodyColor)
				{
					skinnedMeshRenderer.material.SetFloat(_HUE, _baseBodyMesh.material.GetFloat(_HUE));
					skinnedMeshRenderer.material.SetFloat(_BRIGHTNESS, _baseBodyMesh.material.GetFloat(_BRIGHTNESS));
					skinnedMeshRenderer.material.SetFloat(_CONTRAST, _baseBodyMesh.material.GetFloat(_CONTRAST));
					skinnedMeshRenderer.material.SetFloat(_SATURATION, _baseBodyMesh.material.GetFloat(_SATURATION));
				}
			}
		}
	}

	private void Set_DynamicBoneParams()
	{
		_boobDynamicBones[0].enabled = !_equipDisplay._jiggleLockArmor;
		_boobDynamicBones[1].enabled = !_equipDisplay._jiggleLockArmor;
		_assDynamicBones[0].enabled = _bottomWeight >= 35f;
		_assDynamicBones[1].enabled = _bottomWeight >= 35f;
		_bellyDynamicBone[0].enabled = _bellyWeight >= 35f;
	}

	public void Reset_Expression()
	{
		_eyeConditionIsInfinite = false;
		_mouthConditionIsInfinite = false;
		_currentEyeCondition = EyeCondition.Center;
		_currentMouthCondition = MouthCondition.Closed;
	}

	public void Set_EyeCondition(EyeCondition _eyeCondition, float _time)
	{
		if (_time <= 0f)
		{
			_time = 0f;
		}
		_eyeConditionIsInfinite = _time <= 0f;
		_currentEyeCondition = _eyeCondition;
		_eyeConditionBuffer = _time;
		_currentEyeConditionBuffer = 0f;
	}

	public void Set_MouthCondition(MouthCondition _mouthCondition, float _time)
	{
		if (_time <= 0f)
		{
			_time = 0f;
		}
		_mouthConditionIsInfinite = _time <= 0f;
		_currentMouthCondition = _mouthCondition;
		_mouthConditionBuffer = _time;
		_currentMouthConditionBuffer = 0f;
	}
}
