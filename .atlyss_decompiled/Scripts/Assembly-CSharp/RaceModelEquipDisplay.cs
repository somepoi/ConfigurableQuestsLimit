using System.Collections;
using UnityEngine;

public class RaceModelEquipDisplay : MonoBehaviour
{
	private static readonly int _HUE = Shader.PropertyToID("_Hue");

	private static readonly int _SATURATION = Shader.PropertyToID("_Saturation");

	private static readonly int _BRIGHTNESS = Shader.PropertyToID("_Brightness");

	private static readonly int _CONTRAST = Shader.PropertyToID("_Contrast");

	private static readonly int _TINT = Shader.PropertyToID("_ColorTint");

	private Player _player;

	private PlayerEquipment _pEquip;

	private PlayerRaceModel _playerRaceModel;

	private PlayerCombat _pCombat;

	private PlayerCasting _pCasting;

	private RenderModify _renderModify;

	public bool _displayOnly;

	public bool _disableVanity;

	public float _displayBuffer;

	[Header("Armor Overlay / Meshes")]
	public SkinnedMeshRenderer _armorBodyMesh;

	public SkinnedMeshRenderer _armorArmMesh;

	public SkinnedMeshRenderer _armorBoobMesh;

	public SkinnedMeshRenderer _armorFeetMesh;

	[Space]
	public Material _maskMaterial;

	public Texture _clearTexture;

	[Space]
	private Material[] _bodyMats = new Material[0];

	private Material[] _legOverlayMats = new Material[0];

	private Material[] _armorBodymats = new Material[0];

	private Material[] _armorFeetMats = new Material[0];

	private Material[] _armorBoobMats = new Material[0];

	private Material[] _armMats = new Material[0];

	[Header("Underwear Texture")]
	public Texture _underwearTexture_boob;

	public Texture _underwearTexture_bottom;

	public Texture _underwearTexture_bottomMidWeight;

	public Texture _underwearTexutre_bottomLowWeight;

	[Header("Helm Mesh")]
	public MeshRenderer _helmRender;

	public MeshFilter _helmFilter;

	public bool _hasHeadBlendWeights;

	public bool _hideHelmVisual;

	public bool _hideVanityHelm;

	[Header("Chestpiece Meshes")]
	public SkinnedMeshRenderer _chestPieceMeshRender;

	public SkinnedMeshRenderer _chestPieceHipMeshRender;

	public SkinnedMeshRenderer _chestPieceLegRender;

	public SkinnedMeshRenderer _robeSkirtMeshRender;

	public bool _jiggleLockArmor;

	public bool _hideChestVisual;

	public bool _hideVanityChest;

	private bool _usingBoobLockArmor;

	private bool _usingBoobOverrideChestMesh;

	[Header("Collar Mesh")]
	public MeshRenderer _neckCollarRender;

	public MeshFilter _neckCollarFilter;

	[Header("Armcuff Mesh")]
	public SkinnedMeshRenderer _armCuffMeshRender;

	public bool _hasModArmWeights;

	[Header("Shoulderpad Meshes")]
	[SerializeField]
	private MeshRenderer[] _shoulderpadRenders = new MeshRenderer[2];

	[SerializeField]
	private MeshFilter[] _shoulderpadFilters = new MeshFilter[2];

	[Header("Legging Meshes")]
	public SkinnedMeshRenderer _legPieceMeshRender_01;

	public SkinnedMeshRenderer _legPieceMeshRender_02;

	public SkinnedMeshRenderer _legPieceMeshRender_03;

	public SkinnedMeshRenderer _legPieceMeshRender_04;

	public bool _hideLeggingsVisual;

	public bool _hideVanityLeggings;

	public Material _upperLegMaterial;

	public Material _lowerLegMaterial;

	public Material _upperLegArmorMaterial;

	public Material _lowerLegArmorMaterial;

	[Header("Cape Mesh")]
	[SerializeField]
	private GameObject _capeObject;

	[SerializeField]
	private SkinnedMeshRenderer _capeMeshRender;

	public bool _hideCapeVisual;

	public bool _hideVanityCape;

	[Header("Armor Dye")]
	private ScriptableArmorDye _setArmorDye;

	private int _dyeIndex;

	[Header("Weapon Meshes")]
	public Transform _backWeaponRenderContainer;

	public Transform _rightHandWeaponRenderContainer;

	public Transform _leftHandWeaponRenderContainer;

	[HideInInspector]
	public ScriptableWeaponType _scriptWeaponType;

	[Space]
	public MeshRenderer _rightHandWeaponRender;

	public MeshFilter _rightHandWeaponFilter;

	[Space]
	public MeshRenderer _rightHandWeaponGlowRender;

	public MeshFilter _rightHandWeaponGlowFilter;

	[Space]
	public MeshRenderer _leftHandWeaponRender;

	public MeshFilter _leftHandWeaponFilter;

	[Space]
	public MeshRenderer _leftHandWeaponGlowRender;

	public MeshFilter _leftHandWeaponGlowFilter;

	[Space]
	[SerializeField]
	private MeleeWeaponTrail _wepTrail;

	[Space]
	[SerializeField]
	private MeshRenderer[] _backWeaponRenders;

	[SerializeField]
	private MeshRenderer[] _backWeaponGlowRenders;

	[Space]
	[SerializeField]
	private MeshFilter[] _backWeaponFilters;

	[SerializeField]
	private MeshFilter[] _backWeaponGlowFilters;

	[Space]
	public float _hideTakenOutWeaponBuffer;

	public float _hidePutAwayWeaponBuffer;

	[Header("Shield Meshes")]
	[SerializeField]
	private MeshRenderer _putAwayShieldRender;

	[SerializeField]
	private MeshRenderer _takenOutShieldRender;

	[Space]
	[SerializeField]
	private MeshRenderer _putAwayShieldGlowRender;

	[SerializeField]
	private MeshRenderer _takenOutShieldGlowRender;

	public ScriptableShield _cachedScriptShield;

	public int DyeIndex
	{
		get
		{
			return _dyeIndex;
		}
		set
		{
			if (value < 0)
			{
				value = GameManager._current._statLogics._armorDyes.Length - 1;
			}
			if (value > GameManager._current._statLogics._armorDyes.Length - 1)
			{
				value = 0;
			}
			_dyeIndex = value;
		}
	}

	private void Awake()
	{
		_playerRaceModel = GetComponent<PlayerRaceModel>();
		_renderModify = GetComponent<RenderModify>();
		for (int i = 0; i < _backWeaponRenders.Length; i++)
		{
			_backWeaponRenders[i].gameObject.SetActive(value: true);
		}
		for (int j = 0; j < _backWeaponGlowRenders.Length; j++)
		{
			_backWeaponGlowRenders[j].gameObject.SetActive(value: true);
		}
		_rightHandWeaponRender.gameObject.SetActive(value: true);
		_leftHandWeaponRender.gameObject.SetActive(value: true);
		_rightHandWeaponGlowRender.gameObject.SetActive(value: true);
		_leftHandWeaponGlowRender.gameObject.SetActive(value: true);
		_helmRender.gameObject.SetActive(value: true);
		_neckCollarRender.gameObject.SetActive(value: true);
		_armorBodyMesh.gameObject.SetActive(value: true);
		_armorArmMesh.gameObject.SetActive(value: true);
		_armorBoobMesh.gameObject.SetActive(value: true);
		_armorFeetMesh.gameObject.SetActive(value: true);
		_chestPieceMeshRender.gameObject.SetActive(value: true);
		_chestPieceHipMeshRender.gameObject.SetActive(value: true);
		_chestPieceLegRender.gameObject.SetActive(value: true);
		_robeSkirtMeshRender.gameObject.SetActive(value: true);
		_armCuffMeshRender.gameObject.SetActive(value: true);
		_shoulderpadFilters[0].gameObject.SetActive(value: true);
		_shoulderpadFilters[1].gameObject.SetActive(value: true);
		_capeObject.SetActive(value: true);
		_capeMeshRender.gameObject.SetActive(value: true);
		_legPieceMeshRender_01.gameObject.SetActive(value: true);
		_legPieceMeshRender_02.gameObject.SetActive(value: true);
		_legPieceMeshRender_03.gameObject.SetActive(value: true);
		_legPieceMeshRender_04.gameObject.SetActive(value: true);
		StartCoroutine(Init_CacheData());
	}

	private IEnumerator Init_CacheData()
	{
		_displayBuffer = 20.5f;
		yield return new WaitForSeconds(0.6f);
		if (!_playerRaceModel._isCharacterCreationDisplay)
		{
			while (!_player)
			{
				if (_displayOnly)
				{
					_player = Player._mainPlayer;
				}
				else
				{
					_player = GetComponentInParent<Player>();
				}
				yield return null;
			}
			_pCombat = _player._pCombat;
			_pCasting = _player._pCasting;
			_pEquip = _player._pEquipment;
			yield return new WaitForSeconds(1.5f);
		}
		Apply_WeaponMeshesToRenderModifier();
		void Apply_WeaponMeshesToRenderModifier()
		{
			_renderModify._dynamicObjRenders.Add(_rightHandWeaponRender);
			_renderModify._dynamicObjRenders.Add(_leftHandWeaponRender);
			_renderModify._dynamicObjRenders.Add(_takenOutShieldRender);
			_renderModify._dynamicObjRenders.Add(_putAwayShieldRender);
			MeshRenderer[] backWeaponRenders = _backWeaponRenders;
			foreach (MeshRenderer item in backWeaponRenders)
			{
				_renderModify._dynamicObjRenders.Add(item);
			}
			_takenOutShieldRender.gameObject.SetActive(value: false);
			_putAwayShieldRender.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		Apply_EquipmentVisualDisplay();
	}

	public void Apply_EquipmentVisualDisplay()
	{
		if ((bool)_player && _player._bufferingStatus)
		{
			_displayBuffer = 30f;
		}
		if (_displayBuffer > 0f)
		{
			_displayBuffer -= Time.deltaTime;
		}
		else
		{
			_displayBuffer = 0f;
		}
		if (_hideTakenOutWeaponBuffer > 0f)
		{
			_hideTakenOutWeaponBuffer -= Time.deltaTime;
		}
		else
		{
			_hideTakenOutWeaponBuffer = 0f;
		}
		if (_hidePutAwayWeaponBuffer > 0f)
		{
			_hidePutAwayWeaponBuffer -= Time.deltaTime;
		}
		else
		{
			_hidePutAwayWeaponBuffer = 0f;
		}
		if (_withinBounds())
		{
			Handle_InGameEquipDisplay();
			Handle_AnimatorParams();
			Handle_BlendWeights();
		}
		void Apply_LegPieceWeights()
		{
			if (_legPieceMeshRender_01.enabled)
			{
				Set_LeggingRenderParams(_legPieceMeshRender_01);
			}
			if (_legPieceMeshRender_02.enabled)
			{
				Set_LeggingRenderParams(_legPieceMeshRender_02);
			}
			if (_legPieceMeshRender_03.enabled)
			{
				Set_LeggingRenderParams(_legPieceMeshRender_03);
			}
			if (_legPieceMeshRender_04.enabled)
			{
				Set_LeggingRenderParams(_legPieceMeshRender_04);
			}
		}
		void Handle_AnimatorParams()
		{
			bool displayBoobs = _playerRaceModel._displayBoobs;
			_armorBoobMesh.enabled = displayBoobs;
			AnimatorStateInfo currentAnimatorStateInfo = _playerRaceModel._raceAnimator.GetCurrentAnimatorStateInfo(2);
			AnimatorStateInfo currentAnimatorStateInfo2 = _playerRaceModel._raceAnimator.GetCurrentAnimatorStateInfo(6);
			if (_usingBoobLockArmor)
			{
				if (currentAnimatorStateInfo2.IsName("Empty"))
				{
					_playerRaceModel._raceAnimator.CrossFade("Boob_Armor", 0f, 6);
				}
			}
			else if ((bool)_pCombat && _scriptWeaponType._lockBoobOnTakeOut && _pCombat._currentSheathCondition == WeaponSheatheCondition.TAKEN_OUT)
			{
				if (currentAnimatorStateInfo2.IsName("Empty"))
				{
					_playerRaceModel._raceAnimator.CrossFade("Boob_Armor", 0f, 6);
				}
			}
			else if (!_pCombat && !currentAnimatorStateInfo.IsName("Hold_None") && _scriptWeaponType != null && _scriptWeaponType._lockBoobOnTakeOut)
			{
				if (currentAnimatorStateInfo2.IsName("Empty"))
				{
					_playerRaceModel._raceAnimator.CrossFade("Boob_Armor", 0f, 6);
				}
			}
			else if (currentAnimatorStateInfo2.IsName("Boob_Armor"))
			{
				_playerRaceModel._raceAnimator.CrossFade("Empty", 0f, 6);
			}
		}
		void Handle_BlendWeights()
		{
			if (!_playerRaceModel._isCharacterCreationDisplay && Check_DisplayBuffering())
			{
				int index = 4;
				if (_hasHeadBlendWeights)
				{
					index = 5;
				}
				if (_playerRaceModel._headCulled)
				{
					_armorBodyMesh.SetBlendShapeWeight(3, 0f);
				}
				else
				{
					_armorBodyMesh.SetBlendShapeWeight(3, _playerRaceModel._muzzleWeight);
				}
				_armorBodyMesh.SetBlendShapeWeight(0, _playerRaceModel._bellyWeight);
				_armorBodyMesh.SetBlendShapeWeight(1, _playerRaceModel._bottomWeight);
				_armorBodyMesh.SetBlendShapeWeight(2, _playerRaceModel._torsoWeight);
				_armorArmMesh.SetBlendShapeWeight(0, _playerRaceModel._armWeight);
				_armorBoobMesh.SetBlendShapeWeight(0, _playerRaceModel._boobWeight);
				if ((bool)_armCuffMeshRender.sharedMesh)
				{
					_armCuffMeshRender.SetBlendShapeWeight(0, _playerRaceModel._armWeight);
				}
				if ((bool)_chestPieceLegRender.sharedMesh)
				{
					_chestPieceLegRender.SetBlendShapeWeight(0, _playerRaceModel._bellyWeight);
					_chestPieceLegRender.SetBlendShapeWeight(1, _playerRaceModel._bottomWeight);
					_chestPieceLegRender.SetBlendShapeWeight(2, _playerRaceModel._torsoWeight);
					_chestPieceLegRender.SetBlendShapeWeight(3, _playerRaceModel._muzzleWeight);
					if (_playerRaceModel._headCulled)
					{
						_chestPieceLegRender.SetBlendShapeWeight(3, 0f);
					}
					else
					{
						_chestPieceLegRender.SetBlendShapeWeight(3, _playerRaceModel._muzzleWeight);
					}
				}
				if ((bool)_robeSkirtMeshRender.sharedMesh)
				{
					if (_robeSkirtMeshRender.GetBlendShapeWeight(0) != _playerRaceModel._bottomWeight)
					{
						_robeSkirtMeshRender.SetBlendShapeWeight(0, _playerRaceModel._bottomWeight);
					}
					if (_robeSkirtMeshRender.GetBlendShapeWeight(1) != _playerRaceModel._bellyWeight)
					{
						_robeSkirtMeshRender.SetBlendShapeWeight(1, _playerRaceModel._bellyWeight);
					}
				}
				if (_playerRaceModel._bottomWeight < 0f)
				{
					_armorBodyMesh.SetBlendShapeWeight(1, 0f);
					_armorBodyMesh.SetBlendShapeWeight(index, _playerRaceModel._bottomWeight * -1f);
					_chestPieceLegRender.SetBlendShapeWeight(1, 0f);
					_chestPieceLegRender.SetBlendShapeWeight(index, _playerRaceModel._bottomWeight * -1f);
					_robeSkirtMeshRender.SetBlendShapeWeight(0, 0f);
					_robeSkirtMeshRender.SetBlendShapeWeight(2, _playerRaceModel._bottomWeight * -1f);
					if (_playerRaceModel._scriptablePlayerRace._feetUseThighWeights)
					{
						_armorFeetMesh.SetBlendShapeWeight(0, _playerRaceModel._bottomWeight * -1f);
					}
				}
				else
				{
					_armorBodyMesh.SetBlendShapeWeight(index, 0f);
					_chestPieceLegRender.SetBlendShapeWeight(index, 0f);
					_robeSkirtMeshRender.SetBlendShapeWeight(2, 0f);
					if (_playerRaceModel._scriptablePlayerRace._feetUseThighWeights)
					{
						_armorFeetMesh.SetBlendShapeWeight(0, 0f);
					}
				}
				if ((bool)_chestPieceMeshRender.sharedMesh)
				{
					if (!_usingBoobOverrideChestMesh)
					{
						_chestPieceMeshRender.SetBlendShapeWeight(0, _playerRaceModel._bellyWeight);
						if (_playerRaceModel._bottomWeight > 0f)
						{
							_chestPieceMeshRender.SetBlendShapeWeight(1, _playerRaceModel._bottomWeight);
						}
						else
						{
							_chestPieceMeshRender.SetBlendShapeWeight(1, 0f);
						}
						_chestPieceMeshRender.SetBlendShapeWeight(2, _playerRaceModel._torsoWeight);
					}
					else
					{
						float num = _playerRaceModel._boobWeight;
						if (num < 0f)
						{
							num = 0f;
						}
						_chestPieceMeshRender.SetBlendShapeWeight(0, num);
					}
				}
				Apply_LegPieceWeights();
			}
		}
		void Handle_InGameEquipDisplay()
		{
			if ((bool)_pEquip && (bool)_pCombat)
			{
				Set_HelmDisplay();
				Set_ChestpieceDisplay();
				Set_LeggingsDisplay();
				Set_CapeDisplay();
				Set_WeaponDisplay();
				Set_ShieldDisplay();
			}
		}
		void Set_CapeDisplay()
		{
			bool num = (!_pEquip._equips["Cape"]._scriptableEquip && !_pEquip._equips["Cape"]._vanityEquip) || (_hideCapeVisual && _hideVanityCape);
			if ((bool)_pEquip._equips["Cape"]._vanityEquip && !_hideVanityCape)
			{
				ScriptableArmor scriptArmor = (ScriptableArmor)_pEquip._equips["Cape"]._vanityEquip;
				Apply_ArmorDisplay(scriptArmor);
			}
			else if ((bool)_pEquip._equips["Cape"]._scriptableEquip)
			{
				if (_hideCapeVisual)
				{
					Remove_CapeDisplay();
				}
				else
				{
					ScriptableArmor scriptArmor2 = (ScriptableArmor)_pEquip._equips["Cape"]._scriptableEquip;
					Apply_ArmorDisplay(scriptArmor2);
				}
			}
			else
			{
				Remove_CapeDisplay();
			}
			if (num)
			{
				Remove_CapeDisplay();
			}
		}
		void Set_ChestpieceDisplay()
		{
			bool num = (!_pEquip._equips["Chest"]._scriptableEquip && !_pEquip._equips["Chest"]._vanityEquip) || (_hideChestVisual && _hideVanityChest);
			if ((bool)_pEquip._equips["Chest"]._vanityEquip && !_hideVanityChest)
			{
				ScriptableArmor scriptArmor = (ScriptableArmor)_pEquip._equips["Chest"]._vanityEquip;
				Apply_ArmorDisplay(scriptArmor);
			}
			else if ((bool)_pEquip._equips["Chest"]._scriptableEquip)
			{
				if (_hideChestVisual)
				{
					Remove_ChestPieceDisplay();
				}
				else
				{
					ScriptableArmor scriptArmor2 = (ScriptableArmor)_pEquip._equips["Chest"]._scriptableEquip;
					Apply_ArmorDisplay(scriptArmor2);
				}
			}
			else
			{
				Remove_ChestPieceDisplay();
			}
			if (num)
			{
				Remove_ChestPieceDisplay();
			}
		}
		void Set_HelmDisplay()
		{
			bool num = (!_pEquip._equips["Helm"]._scriptableEquip && !_pEquip._equips["Helm"]._vanityEquip) || (_hideHelmVisual && _hideVanityHelm);
			if ((bool)_pEquip._equips["Helm"]._vanityEquip && !_hideVanityHelm)
			{
				ScriptableArmor scriptArmor = (ScriptableArmor)_pEquip._equips["Helm"]._vanityEquip;
				Apply_ArmorDisplay(scriptArmor);
			}
			else if ((bool)_pEquip._equips["Helm"]._scriptableEquip)
			{
				if (_hideHelmVisual)
				{
					Remove_HelmDisplay();
				}
				else
				{
					ScriptableArmor scriptArmor2 = (ScriptableArmor)_pEquip._equips["Helm"]._scriptableEquip;
					Apply_ArmorDisplay(scriptArmor2);
				}
			}
			else
			{
				Remove_HelmDisplay();
			}
			if (num)
			{
				Remove_HelmDisplay();
			}
		}
		void Set_LeggingRenderParams(SkinnedMeshRenderer _meshRend)
		{
			if ((bool)_meshRend.sharedMesh)
			{
				int blendShapeIndex = _meshRend.sharedMesh.GetBlendShapeIndex("_thigh");
				if (_playerRaceModel._bottomWeight >= 0f)
				{
					_meshRend.SetBlendShapeWeight(0, _playerRaceModel._bottomWeight);
					if (blendShapeIndex != -1)
					{
						_meshRend.SetBlendShapeWeight(2, 0f);
					}
				}
				else
				{
					_meshRend.SetBlendShapeWeight(0, 0f);
					if (blendShapeIndex != -1)
					{
						_meshRend.SetBlendShapeWeight(2, _playerRaceModel._bottomWeight * -1f);
					}
				}
				_meshRend.SetBlendShapeWeight(1, _playerRaceModel._bellyWeight);
			}
		}
		void Set_LeggingsDisplay()
		{
			bool num = (!_pEquip._equips["Leggings"]._scriptableEquip && !_pEquip._equips["Leggings"]._vanityEquip) || (_hideLeggingsVisual && _hideVanityLeggings);
			if ((bool)_pEquip._equips["Leggings"]._vanityEquip && !_hideVanityLeggings)
			{
				ScriptableArmor scriptArmor = (ScriptableArmor)_pEquip._equips["Leggings"]._vanityEquip;
				Apply_ArmorDisplay(scriptArmor);
			}
			else if ((bool)_pEquip._equips["Leggings"]._scriptableEquip)
			{
				if (_hideLeggingsVisual)
				{
					Remove_LeggingsDisplay();
				}
				else
				{
					ScriptableArmor scriptArmor2 = (ScriptableArmor)_pEquip._equips["Leggings"]._scriptableEquip;
					Apply_ArmorDisplay(scriptArmor2);
				}
			}
			else
			{
				Remove_LeggingsDisplay();
			}
			if (num)
			{
				Remove_LeggingsDisplay();
			}
		}
		void Set_ShieldDisplay()
		{
			if ((bool)_pEquip._equips["Shield"]._scriptableEquip || (bool)_pEquip._equips["Shield"]._vanityEquip)
			{
				ScriptableShield scriptableShield = (ScriptableShield)_pEquip._equips["Shield"]._scriptableEquip;
				if ((bool)_pEquip._equips["Shield"]._vanityEquip)
				{
					scriptableShield = (ScriptableShield)_pEquip._equips["Shield"]._vanityEquip;
				}
				if (scriptableShield != null && _pEquip._equips["Shield"]._heldItem != null)
				{
					Apply_ShieldDisplay(scriptableShield, _pEquip._equips["Shield"]._heldItem._modifierID);
				}
			}
			else
			{
				Remove_ShieldDisplay();
			}
		}
		void Set_WeaponDisplay()
		{
			if (!_displayOnly)
			{
				_scriptWeaponType = _pCombat._currentScriptableWeaponType;
			}
			else if (!_playerRaceModel._isCharacterCreationDisplay)
			{
				_scriptWeaponType = _pCombat._currentScriptableWeaponType;
			}
			ScriptableWeapon scriptWeapon = null;
			int modifierID = 0;
			if (!_pCombat._lockWeapon)
			{
				if (_pCombat._isUsingAltWeapon && (bool)_pEquip._equips["Alt Weapon"]._scriptableEquip)
				{
					scriptWeapon = (ScriptableWeapon)_pEquip._equips["Alt Weapon"]._scriptableEquip;
					modifierID = _pEquip._equips["Alt Weapon"]._heldItem._modifierID;
				}
				if (!_pCombat._isUsingAltWeapon && (bool)_pEquip._equips["Weapon"]._scriptableEquip)
				{
					scriptWeapon = (ScriptableWeapon)_pEquip._equips["Weapon"]._scriptableEquip;
					modifierID = _pEquip._equips["Weapon"]._heldItem._modifierID;
				}
			}
			else if ((bool)_pEquip._equips["Weapon"]._scriptableEquip)
			{
				scriptWeapon = (ScriptableWeapon)_pEquip._equips["Weapon"]._scriptableEquip;
				modifierID = _pEquip._equips["Weapon"]._heldItem._modifierID;
			}
			Apply_WeaponDisplay(scriptWeapon, modifierID);
		}
		bool _withinBounds()
		{
			if ((bool)Player._mainPlayer && (bool)_player && Player._mainPlayer.Network_playerMapInstance != _player.Network_playerMapInstance)
			{
				return false;
			}
			return true;
		}
	}

	private bool Check_DisplayBuffering()
	{
		if (!_displayOnly && !Player._mainPlayer)
		{
			return false;
		}
		if (!_playerRaceModel)
		{
			return false;
		}
		if (_displayOnly)
		{
			return true;
		}
		if ((bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus && (bool)_pEquip && !_pEquip._setToDisplay)
		{
			return false;
		}
		return true;
	}

	private RaceArmorRender Retrieve_RaceArmorRender(RaceArmorRender[] _armorRenders)
	{
		for (int i = 0; i < _armorRenders.Length; i++)
		{
			if (_armorRenders[i]._scriptableRace == _playerRaceModel._scriptablePlayerRace)
			{
				return _armorRenders[i];
			}
		}
		return null;
	}

	public void Apply_ArmorDisplay(ScriptableArmor _scriptArmor)
	{
		ScriptablePlayerRace _pR;
		bool _boobsEnabled;
		float _h;
		float _s;
		float _b;
		float _c;
		if (Check_DisplayBuffering())
		{
			_pR = _playerRaceModel._scriptablePlayerRace;
			_setArmorDye = GameManager._current._statLogics._armorDyes[_dyeIndex];
			_boobsEnabled = _playerRaceModel._displayBoobs;
			if (_bodyMats.Length == 0)
			{
				_bodyMats = _playerRaceModel._baseBodyMesh.materials;
			}
			if (_legOverlayMats.Length == 0)
			{
				_legOverlayMats = _chestPieceLegRender.materials;
			}
			if (_armorBodymats.Length == 0)
			{
				_armorBodymats = _armorBodyMesh.materials;
			}
			if (_armorFeetMats.Length == 0)
			{
				_armorFeetMats = _armorFeetMesh.materials;
			}
			if (_armorBoobMats.Length == 0)
			{
				_armorBoobMats = _armorBoobMesh.materials;
			}
			if (_armMats.Length == 0)
			{
				_armMats = _armorArmMesh.materials;
			}
			if (!_upperLegMaterial)
			{
				_upperLegMaterial = _bodyMats[_playerRaceModel._scriptablePlayerRace._upperLegMaterialIndex];
			}
			if (!_lowerLegMaterial)
			{
				_lowerLegMaterial = _bodyMats[_playerRaceModel._scriptablePlayerRace._lowerLegMaterialIndex];
			}
			if (!_upperLegArmorMaterial)
			{
				_upperLegArmorMaterial = _armorBodymats[_playerRaceModel._scriptablePlayerRace._upperLegMaterialIndex];
			}
			if (!_lowerLegArmorMaterial)
			{
				_lowerLegArmorMaterial = _armorBodymats[_playerRaceModel._scriptablePlayerRace._lowerLegMaterialIndex];
			}
			if (_scriptArmor._canDyeArmor)
			{
				_h = GameManager._current._statLogics._defaultArmorDye._dyeParams._hue;
				_s = GameManager._current._statLogics._defaultArmorDye._dyeParams._saturation;
				_b = GameManager._current._statLogics._defaultArmorDye._dyeParams._brightness;
				_c = GameManager._current._statLogics._defaultArmorDye._dyeParams._contrast;
			}
			else
			{
				_h = _scriptArmor._colorAdjustParams._hue;
				_s = _scriptArmor._colorAdjustParams._saturation;
				_b = _scriptArmor._colorAdjustParams._brightness;
				_c = _scriptArmor._colorAdjustParams._contrast;
			}
			if (_scriptArmor.GetType() == typeof(ScriptableHelm))
			{
				Apply_HelmDisplay();
			}
			else if (_scriptArmor.GetType() == typeof(ScriptableChestpiece))
			{
				Apply_ChestDisplay();
			}
			else if (_scriptArmor.GetType() == typeof(ScriptableLeggings))
			{
				Apply_LeggingDisplay();
			}
			else if (_scriptArmor.GetType() == typeof(ScriptableCape))
			{
				Apply_CapeDisplay();
			}
			_playerRaceModel.Apply_CharacterDisplay();
		}
		void Apply_CapeDisplay()
		{
			ScriptableCape scriptableCape = (ScriptableCape)_scriptArmor;
			_capeMeshRender.enabled = true;
			_capeMeshRender.sharedMesh = scriptableCape._capeMesh;
			_capeMeshRender.material.mainTexture = scriptableCape._armorTexture;
			Set_MaterialColorProfile(_capeMeshRender.material, _scriptArmor._colorAdjustParams);
			if (_playerRaceModel._tailDisplays[_playerRaceModel.SetTailIndex].transform.parent.TryGetComponent<Animator>(out var component))
			{
				component.SetBool("_capeTailMode", value: true);
			}
		}
		void Apply_ChestDisplay()
		{
			ScriptableChestpiece _scriptChest = (ScriptableChestpiece)_scriptArmor;
			if (_scriptArmor._canDyeArmor && (bool)_setArmorDye)
			{
				_h = _setArmorDye._dyeParams._hue;
				_s = _setArmorDye._dyeParams._saturation;
				_b = _setArmorDye._dyeParams._brightness;
				_c = _setArmorDye._dyeParams._contrast;
			}
			_usingBoobLockArmor = _scriptChest._lockBoobs;
			_jiggleLockArmor = _scriptChest._disableJiggleBoobBones;
			if (_jiggleLockArmor)
			{
				_playerRaceModel._boobDynamicBones[0].enabled = false;
				_playerRaceModel._boobDynamicBones[1].enabled = false;
			}
			else if (!SettingsManager._current._jiggleBonesToggle.isOn)
			{
				_playerRaceModel._boobDynamicBones[0].enabled = true;
				_playerRaceModel._boobDynamicBones[1].enabled = true;
			}
			Apply_RobeSkirtDisplay();
			Apply_BodyTexturing();
			Apply_CollarMeshDisplay();
			Apply_ChestMeshDisplay();
			Apply_ChestHipDisplay();
			Apply_ArmCuffDisplay();
			Apply_ShoulderPadDisplay();
			void Apply_ArmCuffDisplay()
			{
				_armCuffMeshRender.enabled = _scriptChest._armCuffRender;
				if ((bool)_scriptChest._armCuffRender)
				{
					RaceArmorRender raceArmorRender = Retrieve_RaceArmorRender(_scriptChest._armCuffRender._raceArmorRenders);
					_armCuffMeshRender.sharedMesh = raceArmorRender._raceArmorMesh;
					_armCuffMeshRender.material.mainTexture = _scriptArmor._armorTexture;
					_armCuffMeshRender.material.SetFloat(_HUE, _h);
					_armCuffMeshRender.material.SetFloat(_SATURATION, _s);
					_armCuffMeshRender.material.SetFloat(_BRIGHTNESS, _b);
					_armCuffMeshRender.material.SetFloat(_CONTRAST, _c);
				}
			}
			void Apply_BodyTexturing()
			{
				Texture mainTexture = _scriptArmor._armorTexture;
				if (_playerRaceModel._displayBoobs && (bool)_scriptChest._boobOverrideTexture)
				{
					mainTexture = _scriptChest._boobOverrideTexture;
				}
				_armorBodymats[_pR._bodyMaterialIndex].mainTexture = mainTexture;
				_armorBodymats[_pR._bodyMaterialIndex].SetFloat(_HUE, _h);
				_armorBodymats[_pR._bodyMaterialIndex].SetFloat(_SATURATION, _s);
				_armorBodymats[_pR._bodyMaterialIndex].SetFloat(_BRIGHTNESS, _b);
				_armorBodymats[_pR._bodyMaterialIndex].SetFloat(_CONTRAST, _c);
				_armorBoobMats[0].mainTexture = mainTexture;
				_armorBoobMats[0].SetFloat(_HUE, _h);
				_armorBoobMats[0].SetFloat(_SATURATION, _s);
				_armorBoobMats[0].SetFloat(_BRIGHTNESS, _b);
				_armorBoobMats[0].SetFloat(_CONTRAST, _c);
				if (_scriptChest._textureArms)
				{
					if (_hasModArmWeights)
					{
						_playerRaceModel._baseArmMesh.SetBlendShapeWeight(1, 100f);
						_armorArmMesh.SetBlendShapeWeight(1, 100f);
					}
					for (int i = 0; i < _armMats.Length; i++)
					{
						_armMats[i].mainTexture = _scriptArmor._armorTexture;
						_armMats[i].SetFloat(_HUE, _h);
						_armMats[i].SetFloat(_SATURATION, _s);
						_armMats[i].SetFloat(_BRIGHTNESS, _b);
						_armMats[i].SetFloat(_CONTRAST, _c);
					}
					_armorArmMesh.materials = _armMats;
				}
				else
				{
					for (int j = 0; j < _armMats.Length; j++)
					{
						_armMats[j].mainTexture = _clearTexture;
					}
					_armorArmMesh.materials = _armMats;
				}
				_chestPieceLegRender.enabled = _scriptChest._legOverlayTexture;
				if ((bool)_scriptChest._legOverlayTexture)
				{
					if (_robeSkirtMeshRender.enabled)
					{
						_chestPieceLegRender.enabled = false;
					}
					else
					{
						_legOverlayMats[_pR._upperLegMaterialIndex].mainTexture = _scriptChest._legOverlayTexture;
						_legOverlayMats[_pR._upperLegMaterialIndex].SetFloat(_HUE, _h);
						_legOverlayMats[_pR._upperLegMaterialIndex].SetFloat(_SATURATION, _s);
						_legOverlayMats[_pR._upperLegMaterialIndex].SetFloat(_BRIGHTNESS, _b);
						_legOverlayMats[_pR._upperLegMaterialIndex].SetFloat(_CONTRAST, _c);
						_legOverlayMats[_pR._lowerLegMaterialIndex].mainTexture = _scriptChest._legOverlayTexture;
						_legOverlayMats[_pR._lowerLegMaterialIndex].SetFloat(_HUE, _h);
						_legOverlayMats[_pR._lowerLegMaterialIndex].SetFloat(_SATURATION, _s);
						_legOverlayMats[_pR._lowerLegMaterialIndex].SetFloat(_BRIGHTNESS, _b);
						_legOverlayMats[_pR._lowerLegMaterialIndex].SetFloat(_CONTRAST, _c);
					}
				}
			}
			void Apply_ChestHipDisplay()
			{
				_chestPieceHipMeshRender.enabled = _scriptChest._hipMeshDisplay;
				if ((bool)_scriptChest._hipMeshDisplay)
				{
					Mesh raceArmorMesh = Retrieve_RaceArmorRender(_scriptChest._hipMeshDisplay._raceArmorRenders)._raceArmorMesh;
					_chestPieceHipMeshRender.material.mainTexture = _scriptChest._hipMeshTexture;
					_chestPieceHipMeshRender.material.SetFloat(_HUE, _h);
					_chestPieceHipMeshRender.material.SetFloat(_SATURATION, _s);
					_chestPieceHipMeshRender.material.SetFloat(_BRIGHTNESS, _b);
					_chestPieceHipMeshRender.material.SetFloat(_CONTRAST, _c);
					_chestPieceHipMeshRender.sharedMesh = raceArmorMesh;
					if (_scriptChest._hipMeshDisplay._useThighBlendshape && (bool)_chestPieceHipMeshRender.sharedMesh && _chestPieceHipMeshRender.sharedMesh.GetBlendShapeIndex("_thigh") != -1)
					{
						if (_playerRaceModel._bottomWeight < 0f)
						{
							_chestPieceHipMeshRender.SetBlendShapeWeight(0, 0f);
							_chestPieceHipMeshRender.SetBlendShapeWeight(2, _playerRaceModel._bottomWeight * -1f);
						}
						else
						{
							_chestPieceHipMeshRender.SetBlendShapeWeight(0, _playerRaceModel._bottomWeight);
							_chestPieceHipMeshRender.SetBlendShapeWeight(2, 0f);
						}
					}
				}
			}
			void Apply_ChestMeshDisplay()
			{
				_chestPieceMeshRender.enabled = _scriptChest._chestRenderDisplay;
				if ((bool)_scriptChest._chestRenderDisplay)
				{
					RaceArmorRender raceArmorRender = Retrieve_RaceArmorRender(_scriptChest._chestRenderDisplay._raceArmorRenders);
					Mesh sharedMesh = raceArmorRender._raceArmorMesh;
					_usingBoobOverrideChestMesh = _boobsEnabled && (bool)raceArmorRender._boobOverrideMesh;
					if (_usingBoobOverrideChestMesh)
					{
						sharedMesh = raceArmorRender._boobOverrideMesh;
					}
					_chestPieceMeshRender.sharedMesh = sharedMesh;
					_chestPieceMeshRender.material.mainTexture = _scriptChest._chestRenderTexture;
					_chestPieceMeshRender.material.SetFloat(_HUE, _h);
					_chestPieceMeshRender.material.SetFloat(_SATURATION, _s);
					_chestPieceMeshRender.material.SetFloat(_BRIGHTNESS, _b);
					_chestPieceMeshRender.material.SetFloat(_CONTRAST, _c);
					if ((bool)_scriptChest._chestRenderBoobTexture)
					{
						_armorBoobMesh.material.mainTexture = _scriptChest._chestRenderBoobTexture;
						_armorBoobMesh.material.SetFloat(_HUE, _h);
						_armorBoobMesh.material.SetFloat(_SATURATION, _s);
						_armorBoobMesh.material.SetFloat(_BRIGHTNESS, _b);
						_armorBoobMesh.material.SetFloat(_CONTRAST, _c);
					}
					if (_scriptChest._chestRenderDisplay._useThighBlendshape && (bool)_chestPieceMeshRender.sharedMesh && _chestPieceMeshRender.sharedMesh.GetBlendShapeIndex("_thigh") != -1)
					{
						if (_playerRaceModel._bottomWeight < 0f)
						{
							_chestPieceMeshRender.SetBlendShapeWeight(3, _playerRaceModel._bottomWeight * -1f);
						}
						else
						{
							_chestPieceMeshRender.SetBlendShapeWeight(3, 0f);
						}
					}
				}
			}
			void Apply_CollarMeshDisplay()
			{
				_neckCollarRender.enabled = _scriptChest._neckCollarMesh;
				if ((bool)_scriptChest._neckCollarMesh)
				{
					_neckCollarFilter.mesh = _scriptChest._neckCollarMesh;
					_neckCollarRender.material.mainTexture = _scriptChest._neckCollarTexture;
				}
			}
			void Apply_RobeSkirtDisplay()
			{
				_robeSkirtMeshRender.enabled = _scriptChest._robeSkirtRender;
				_playerRaceModel.Init_CullLegs(_scriptChest._robeSkirtRender);
				if (_robeSkirtMeshRender.enabled)
				{
					RaceArmorRender raceArmorRender = Retrieve_RaceArmorRender(_scriptChest._robeSkirtRender._raceArmorRenders);
					_robeSkirtMeshRender.sharedMesh = raceArmorRender._raceArmorMesh;
					if ((bool)_scriptChest._robeSkirtTexture)
					{
						_robeSkirtMeshRender.material.mainTexture = _scriptChest._robeSkirtTexture;
					}
					else
					{
						_robeSkirtMeshRender.material.mainTexture = _scriptArmor._armorTexture;
					}
					_robeSkirtMeshRender.material.SetFloat(_HUE, _h);
					_robeSkirtMeshRender.material.SetFloat(_SATURATION, _s);
					_robeSkirtMeshRender.material.SetFloat(_BRIGHTNESS, _b);
					_robeSkirtMeshRender.material.SetFloat(_CONTRAST, _c);
					_legPieceMeshRender_01.enabled = false;
					_legPieceMeshRender_02.enabled = false;
					_legPieceMeshRender_03.enabled = false;
					_legPieceMeshRender_04.enabled = false;
				}
			}
			void Apply_ShoulderPadDisplay()
			{
				switch (_scriptChest._shoulderPadDisplayType)
				{
				case ScriptableChestpiece.ShoulderPadDisplayType.None:
					_shoulderpadRenders[0].enabled = false;
					_shoulderpadRenders[1].enabled = false;
					break;
				case ScriptableChestpiece.ShoulderPadDisplayType.Both:
					_shoulderpadRenders[0].enabled = true;
					_shoulderpadRenders[1].enabled = true;
					break;
				case ScriptableChestpiece.ShoulderPadDisplayType.Left:
					_shoulderpadRenders[0].enabled = true;
					_shoulderpadRenders[1].enabled = false;
					break;
				case ScriptableChestpiece.ShoulderPadDisplayType.Right:
					_shoulderpadRenders[0].enabled = false;
					_shoulderpadRenders[1].enabled = true;
					break;
				}
				if ((bool)_scriptChest._shoulderpadMesh)
				{
					_shoulderpadFilters[0].mesh = _scriptChest._shoulderpadMesh;
					_shoulderpadFilters[1].mesh = _scriptChest._shoulderpadMesh;
					_shoulderpadRenders[0].material.mainTexture = _scriptChest._shoulderpadTexture;
					_shoulderpadRenders[1].material.mainTexture = _scriptChest._shoulderpadTexture;
				}
			}
		}
		void Apply_HelmDisplay()
		{
			ScriptableHelm scriptableHelm = (ScriptableHelm)_scriptArmor;
			_helmRender.enabled = scriptableHelm._helmRender;
			if (_scriptArmor._canDyeArmor && (bool)_setArmorDye)
			{
				_h = _setArmorDye._dyeParams._hue;
				_s = _setArmorDye._dyeParams._saturation;
				_b = _setArmorDye._dyeParams._brightness;
				_c = _setArmorDye._dyeParams._contrast;
			}
			if ((bool)scriptableHelm._helmRender)
			{
				RaceArmorRender raceArmorRender = Retrieve_RaceArmorRender(scriptableHelm._helmRender._raceArmorRenders);
				Transform obj = _helmFilter.transform;
				if ((bool)scriptableHelm._helmOverrideMesh)
				{
					_helmFilter.mesh = scriptableHelm._helmOverrideMesh;
				}
				else
				{
					_helmFilter.mesh = raceArmorRender._raceArmorMesh;
				}
				obj.SetLocalPositionAndRotation(raceArmorRender._meshPosition, Quaternion.Euler(raceArmorRender._meshRotation));
				obj.localScale = raceArmorRender._meshScale;
				_helmRender.material.mainTexture = scriptableHelm._armorTexture;
				_helmRender.material.SetFloat(_HUE, _h);
				_helmRender.material.SetFloat(_BRIGHTNESS, _b);
				_helmRender.material.SetFloat(_CONTRAST, _c);
				_helmRender.material.SetFloat(_SATURATION, _s);
			}
			if (_playerRaceModel._scriptablePlayerRace._hairStyleMeshes != null)
			{
				_playerRaceModel._useHelmHair = scriptableHelm._useHelmHairDisplay;
			}
			if (_playerRaceModel._miscSkinnedDisplays.Length == 0)
			{
				_playerRaceModel._miscDisplayContainer.gameObject.SetActive(!scriptableHelm._hideMisc);
			}
			if (_hasHeadBlendWeights)
			{
				_playerRaceModel._baseBodyMesh.SetBlendShapeWeight(4, scriptableHelm._blendHelmValue);
			}
			_playerRaceModel.Init_CullHead(scriptableHelm._isFullHelm);
			_playerRaceModel._hideEars = scriptableHelm._hideEars;
		}
		void Apply_LeggingDisplay()
		{
			ScriptableLeggings _scriptLeggings = (ScriptableLeggings)_scriptArmor;
			if (_scriptArmor._canDyeArmor && (bool)_setArmorDye)
			{
				_h = _setArmorDye._dyeParams._hue;
				_s = _setArmorDye._dyeParams._saturation;
				_b = _setArmorDye._dyeParams._brightness;
				_c = _setArmorDye._dyeParams._contrast;
			}
			if (!_scriptLeggings._textureUpperLegOnly)
			{
				_armorBodymats[_pR._lowerLegMaterialIndex].mainTexture = _scriptArmor._armorTexture;
				_armorBodymats[_pR._lowerLegMaterialIndex].SetFloat(_HUE, _h);
				_armorBodymats[_pR._lowerLegMaterialIndex].SetFloat(_SATURATION, _s);
				_armorBodymats[_pR._lowerLegMaterialIndex].SetFloat(_BRIGHTNESS, _b);
				_armorBodymats[_pR._lowerLegMaterialIndex].SetFloat(_CONTRAST, _c);
			}
			else
			{
				_armorBodymats[_pR._lowerLegMaterialIndex].mainTexture = _clearTexture;
			}
			Apply_LegPieceRenderDisplay();
			if (_scriptLeggings._textureFeet)
			{
				_armorFeetMats[0].mainTexture = _scriptArmor._armorTexture;
				_armorFeetMats[0].SetFloat(_HUE, _h);
				_armorFeetMats[0].SetFloat(_SATURATION, _s);
				_armorFeetMats[0].SetFloat(_BRIGHTNESS, _b);
				_armorFeetMats[0].SetFloat(_CONTRAST, _c);
			}
			else
			{
				_armorFeetMats[0].mainTexture = _clearTexture;
			}
			void Apply_LegPieceRenderDisplay()
			{
				if (_robeSkirtMeshRender.enabled)
				{
					_legPieceMeshRender_01.enabled = false;
					_legPieceMeshRender_02.enabled = false;
					_legPieceMeshRender_03.enabled = false;
					_legPieceMeshRender_04.enabled = false;
				}
				else
				{
					_armorBodymats[_pR._upperLegMaterialIndex].mainTexture = _scriptArmor._armorTexture;
					_armorBodymats[_pR._upperLegMaterialIndex].SetFloat(_HUE, _h);
					_armorBodymats[_pR._upperLegMaterialIndex].SetFloat(_SATURATION, _s);
					_armorBodymats[_pR._upperLegMaterialIndex].SetFloat(_BRIGHTNESS, _b);
					_armorBodymats[_pR._upperLegMaterialIndex].SetFloat(_CONTRAST, _c);
					_legPieceMeshRender_01.enabled = _scriptLeggings._legPieceRender_01;
					_legPieceMeshRender_02.enabled = _scriptLeggings._legPieceRender_02;
					_legPieceMeshRender_03.enabled = _scriptLeggings._legPieceRender_03;
					_legPieceMeshRender_04.enabled = _scriptLeggings._legPieceRender_04;
					if ((bool)_scriptLeggings._legPieceRender_01)
					{
						Set_LegRender(_legPieceMeshRender_01, 0);
					}
					if ((bool)_scriptLeggings._legPieceRender_02)
					{
						Set_LegRender(_legPieceMeshRender_02, 1);
					}
					if ((bool)_scriptLeggings._legPieceRender_03)
					{
						Set_LegRender(_legPieceMeshRender_03, 2);
					}
					if ((bool)_scriptLeggings._legPieceRender_04)
					{
						Set_LegRender(_legPieceMeshRender_04, 3);
					}
				}
			}
		}
		void Set_LegRender(SkinnedMeshRenderer _render, int _index)
		{
			RaceArmorRender raceArmorRender = null;
			switch (_index)
			{
			case 0:
				raceArmorRender = Retrieve_RaceArmorRender(P_3._scriptLeggings._legPieceRender_01._raceArmorRenders);
				_render.material.mainTexture = P_3._scriptLeggings._legPieceTexture_01;
				break;
			case 1:
				raceArmorRender = Retrieve_RaceArmorRender(P_3._scriptLeggings._legPieceRender_02._raceArmorRenders);
				_render.material.mainTexture = P_3._scriptLeggings._legPieceTexture_02;
				break;
			case 2:
				raceArmorRender = Retrieve_RaceArmorRender(P_3._scriptLeggings._legPieceRender_03._raceArmorRenders);
				_render.material.mainTexture = P_3._scriptLeggings._legPieceTexture_03;
				break;
			case 3:
				raceArmorRender = Retrieve_RaceArmorRender(P_3._scriptLeggings._legPieceRender_04._raceArmorRenders);
				_render.material.mainTexture = P_3._scriptLeggings._legPieceTexture_04;
				break;
			}
			_render.sharedMesh = raceArmorRender._raceArmorMesh;
			_render.material.SetFloat(_HUE, _h);
			_render.material.SetFloat(_SATURATION, _s);
			_render.material.SetFloat(_BRIGHTNESS, _b);
			_render.material.SetFloat(_CONTRAST, _c);
		}
	}

	public void Remove_ArmorDisplay(ScriptableArmor _scriptArmor)
	{
		if (_scriptArmor.GetType() == typeof(ScriptableHelm))
		{
			Remove_HelmDisplay();
		}
		else if (_scriptArmor.GetType() == typeof(ScriptableChestpiece))
		{
			Remove_ChestPieceDisplay();
		}
		else if (_scriptArmor.GetType() == typeof(ScriptableLeggings))
		{
			Remove_LeggingsDisplay();
		}
	}

	public void Remove_HelmDisplay()
	{
		_helmRender.enabled = false;
		_playerRaceModel._useHelmHair = false;
		_playerRaceModel._hideEars = false;
		_playerRaceModel._miscDisplayContainer.gameObject.SetActive(value: true);
		if (_hasHeadBlendWeights)
		{
			_playerRaceModel._baseBodyMesh.SetBlendShapeWeight(4, 0f);
		}
		_playerRaceModel.Init_CullHead(_isTrue: false);
		_playerRaceModel.Apply_CharacterDisplay();
	}

	public void Remove_ChestPieceDisplay()
	{
		if (_bodyMats.Length == 0)
		{
			_bodyMats = _playerRaceModel._baseBodyMesh.materials;
		}
		if (_legOverlayMats.Length == 0)
		{
			_legOverlayMats = _chestPieceLegRender.materials;
		}
		if (_armorBodymats.Length == 0)
		{
			_armorBodymats = _armorBodyMesh.materials;
		}
		if (_armorFeetMats.Length == 0)
		{
			_armorFeetMats = _armorFeetMesh.materials;
		}
		if (_armorBoobMats.Length == 0)
		{
			_armorBoobMats = _armorBoobMesh.materials;
		}
		if (_armMats.Length == 0)
		{
			_armMats = _armorArmMesh.materials;
		}
		ScriptablePlayerRace scriptablePlayerRace = _playerRaceModel._scriptablePlayerRace;
		if (_hasModArmWeights)
		{
			_playerRaceModel._baseArmMesh.SetBlendShapeWeight(1, 0f);
			_armorArmMesh.SetBlendShapeWeight(1, 0f);
		}
		if (SettingsManager._current._clearUnderClothesToggle.isOn)
		{
			_armorBodymats[scriptablePlayerRace._bodyMaterialIndex].mainTexture = _clearTexture;
			_armorBoobMats[0].mainTexture = _clearTexture;
		}
		else if (_playerRaceModel._displayBoobs)
		{
			Set_MaterialColorProfile(_armorBoobMats[0], new ColorAdjustShader_Profile
			{
				_hue = 1f,
				_saturation = 1f,
				_brightness = 0f,
				_contrast = 1f
			});
			Set_MaterialColorProfile(_armorBodymats[scriptablePlayerRace._bodyMaterialIndex], new ColorAdjustShader_Profile
			{
				_hue = 1f,
				_saturation = 1f,
				_brightness = 0f,
				_contrast = 1f
			});
			_armorBoobMats[0].mainTexture = _underwearTexture_boob;
			_armorBodymats[scriptablePlayerRace._bodyMaterialIndex].mainTexture = _underwearTexture_boob;
		}
		else
		{
			_armorBodymats[scriptablePlayerRace._bodyMaterialIndex].mainTexture = _clearTexture;
			_armorBoobMats[0].mainTexture = _clearTexture;
		}
		for (int i = 0; i < _armMats.Length; i++)
		{
			_armMats[i].mainTexture = _clearTexture;
		}
		_armorArmMesh.materials = _armMats;
		_chestPieceLegRender.enabled = false;
		_chestPieceMeshRender.enabled = false;
		_chestPieceHipMeshRender.enabled = false;
		_robeSkirtMeshRender.enabled = false;
		_armCuffMeshRender.enabled = false;
		_neckCollarRender.enabled = false;
		_shoulderpadRenders[0].enabled = false;
		_shoulderpadRenders[1].enabled = false;
		_playerRaceModel.Init_CullLegs(_isTrue: false);
		if (!SettingsManager._current._jiggleBonesToggle.isOn)
		{
			_playerRaceModel._boobDynamicBones[0].enabled = true;
			_playerRaceModel._boobDynamicBones[1].enabled = true;
		}
		_usingBoobLockArmor = false;
	}

	public void Remove_LeggingsDisplay()
	{
		if (_armorBodymats.Length == 0)
		{
			_armorBodymats = _armorBodyMesh.materials;
		}
		if (_armorFeetMats.Length == 0)
		{
			_armorFeetMats = _armorFeetMesh.materials;
		}
		ScriptablePlayerRace scriptablePlayerRace = _playerRaceModel._scriptablePlayerRace;
		if (SettingsManager._current._clearUnderClothesToggle.isOn)
		{
			_armorBodymats[scriptablePlayerRace._upperLegMaterialIndex].mainTexture = _clearTexture;
			_armorBodymats[scriptablePlayerRace._lowerLegMaterialIndex].mainTexture = _clearTexture;
		}
		else
		{
			Set_MaterialColorProfile(_armorBodymats[scriptablePlayerRace._upperLegMaterialIndex], new ColorAdjustShader_Profile
			{
				_hue = 1f,
				_saturation = 1f,
				_brightness = 0f,
				_contrast = 1f
			});
			Set_MaterialColorProfile(_armorBodymats[scriptablePlayerRace._lowerLegMaterialIndex], new ColorAdjustShader_Profile
			{
				_hue = 1f,
				_saturation = 1f,
				_brightness = 0f,
				_contrast = 1f
			});
			_armorBodymats[scriptablePlayerRace._upperLegMaterialIndex].mainTexture = _underwearTexutre_bottomLowWeight;
			_armorBodymats[scriptablePlayerRace._lowerLegMaterialIndex].mainTexture = _underwearTexutre_bottomLowWeight;
			if (_playerRaceModel._baseBodyMesh.GetBlendShapeWeight(1) >= 10f)
			{
				_armorBodymats[scriptablePlayerRace._upperLegMaterialIndex].mainTexture = _underwearTexture_bottomMidWeight;
				_armorBodymats[scriptablePlayerRace._lowerLegMaterialIndex].mainTexture = _underwearTexture_bottomMidWeight;
			}
			if (_playerRaceModel._baseBodyMesh.GetBlendShapeWeight(1) >= 40f)
			{
				_armorBodymats[scriptablePlayerRace._upperLegMaterialIndex].mainTexture = _underwearTexture_bottom;
				_armorBodymats[scriptablePlayerRace._lowerLegMaterialIndex].mainTexture = _underwearTexture_bottom;
			}
		}
		_legPieceMeshRender_01.enabled = false;
		_legPieceMeshRender_02.enabled = false;
		_legPieceMeshRender_03.enabled = false;
		_legPieceMeshRender_04.enabled = false;
		_armorFeetMats[0].mainTexture = _clearTexture;
		if (_chestPieceLegRender.enabled)
		{
			_armorBodymats[scriptablePlayerRace._upperLegMaterialIndex].mainTexture = _clearTexture;
			_armorBodymats[scriptablePlayerRace._lowerLegMaterialIndex].mainTexture = _clearTexture;
		}
	}

	public void Remove_CapeDisplay()
	{
		_capeMeshRender.enabled = false;
		if ((bool)_playerRaceModel._tailDisplays[_playerRaceModel.SetTailIndex].transform.parent.GetComponent<Animator>())
		{
			_playerRaceModel._tailDisplays[_playerRaceModel.SetTailIndex].transform.parent.GetComponent<Animator>().SetBool("_capeTailMode", value: false);
		}
	}

	public void Apply_WeaponDisplay(ScriptableWeapon _scriptWeapon, int _modifierID)
	{
		if (!_scriptWeapon)
		{
			return;
		}
		if ((bool)_pCombat && _pCombat._isDisarmed)
		{
			Remove_WeaponDisplay();
			return;
		}
		_scriptWeaponType = _scriptWeapon.weaponType;
		if ((bool)_scriptWeapon.weaponMesh)
		{
			Init_WeaponDisplayMesh();
		}
		else
		{
			Debug.LogError("Weapon Display Scriptable Weapon Object " + _scriptWeapon._itemName + " does not have a mesh.");
		}
		void Init_WeaponDisplayMesh()
		{
			bool _weaponHasModifier = _modifierID > 0 && !SettingsManager._current._disableWeaponGlowToggle.isOn;
			if ((bool)_pCombat)
			{
				if (_player._currentPlayerAction == PlayerAction.CASTING && (bool)_pCasting._currentCastSkill && _pCasting._currentCastSkill._hideWeapon)
				{
					_hideTakenOutWeaponBuffer = 0.85f;
				}
				if (_hideTakenOutWeaponBuffer > 0f)
				{
					_rightHandWeaponRender.enabled = false;
					_leftHandWeaponRender.enabled = false;
					if (_hidePutAwayWeaponBuffer <= 0f)
					{
						Display_PutAwayWeapon();
					}
				}
				else
				{
					switch (_pCombat._currentSheathCondition)
					{
					case WeaponSheatheCondition.TAKEN_OUT:
						Display_TakenOutWeapon();
						break;
					case WeaponSheatheCondition.PUT_AWAY:
						Display_PutAwayWeapon();
						break;
					}
				}
			}
			else if ((bool)Player._mainPlayer)
			{
				switch (Player._mainPlayer._pCombat._currentSheathCondition)
				{
				case WeaponSheatheCondition.TAKEN_OUT:
					Display_TakenOutWeapon();
					break;
				case WeaponSheatheCondition.PUT_AWAY:
					Display_PutAwayWeapon();
					break;
				}
			}
			else
			{
				Display_TakenOutWeapon();
			}
			void Display_PutAwayWeapon()
			{
				if (_hideTakenOutWeaponBuffer <= 0f && _playerRaceModel._raceAnimator.isActiveAndEnabled)
				{
					_playerRaceModel._raceAnimator.CrossFade("Hold_None", 0f, 2);
					_playerRaceModel._canAltIdleAnim = true;
				}
				for (int i = 0; i < _backWeaponRenders.Length; i++)
				{
					if (i == _scriptWeaponType._backWeaponIndex && !_scriptWeaponType._hidePutAwayWeapon)
					{
						_backWeaponRenders[i].enabled = true;
						_backWeaponGlowRenders[i].enabled = _weaponHasModifier;
					}
					else
					{
						_backWeaponRenders[i].enabled = false;
						_backWeaponGlowRenders[i].enabled = false;
					}
				}
				_rightHandWeaponRender.enabled = false;
				_leftHandWeaponRender.enabled = false;
				_rightHandWeaponGlowRender.enabled = false;
				_leftHandWeaponGlowRender.enabled = false;
				MeshFilter meshFilter = _backWeaponFilters[_scriptWeaponType._backWeaponIndex];
				Material material = _backWeaponRenders[_scriptWeaponType._backWeaponIndex].material;
				MeshFilter obj = _backWeaponGlowFilters[_scriptWeaponType._backWeaponIndex];
				Material material2 = _backWeaponGlowRenders[_scriptWeaponType._backWeaponIndex].material;
				meshFilter.mesh = _scriptWeapon.weaponMesh;
				obj.mesh = _scriptWeapon.weaponMesh;
				material.mainTexture = _scriptWeapon.weaponTexture;
				if (_weaponHasModifier && !_scriptWeaponType._hidePutAwayWeapon)
				{
					ScriptableStatModifier scriptableStatModifier = GameManager._current.Locate_StatModifier(_modifierID);
					if ((bool)scriptableStatModifier)
					{
						material2.SetColor("_Color", scriptableStatModifier._glowTintColor);
					}
				}
				Set_MaterialColorProfile(material, _scriptWeapon._textureColorAdjust);
			}
			void Display_TakenOutWeapon()
			{
				switch (_scriptWeaponType._weaponDisplayMode)
				{
				case WeaponRenderDisplaymode.RIGHT:
					_rightHandWeaponRender.enabled = true;
					_leftHandWeaponRender.enabled = false;
					_rightHandWeaponGlowRender.enabled = _weaponHasModifier;
					_leftHandWeaponGlowRender.enabled = false;
					_rightHandWeaponFilter.mesh = _scriptWeapon.weaponMesh;
					_rightHandWeaponRender.material.mainTexture = _scriptWeapon.weaponTexture;
					Set_MaterialColorProfile(_rightHandWeaponRender.material, _scriptWeapon._textureColorAdjust);
					break;
				case WeaponRenderDisplaymode.LEFT:
					_rightHandWeaponRender.enabled = false;
					_leftHandWeaponRender.enabled = true;
					_rightHandWeaponGlowRender.enabled = false;
					_leftHandWeaponGlowRender.enabled = _weaponHasModifier;
					_leftHandWeaponFilter.mesh = _scriptWeapon.weaponMesh;
					_leftHandWeaponRender.material.mainTexture = _scriptWeapon.weaponTexture;
					Set_MaterialColorProfile(_leftHandWeaponRender.material, _scriptWeapon._textureColorAdjust);
					break;
				case WeaponRenderDisplaymode.DUAL:
					_rightHandWeaponRender.enabled = true;
					_leftHandWeaponRender.enabled = true;
					_rightHandWeaponGlowRender.enabled = _weaponHasModifier;
					_leftHandWeaponGlowRender.enabled = _weaponHasModifier;
					_rightHandWeaponFilter.mesh = _scriptWeapon.weaponMesh;
					_rightHandWeaponRender.material.mainTexture = _scriptWeapon.weaponTexture;
					Set_MaterialColorProfile(_rightHandWeaponRender.material, _scriptWeapon._textureColorAdjust);
					_leftHandWeaponFilter.mesh = _scriptWeapon.weaponMesh;
					_leftHandWeaponRender.material.mainTexture = _scriptWeapon.weaponTexture;
					Set_MaterialColorProfile(_leftHandWeaponRender.material, _scriptWeapon._textureColorAdjust);
					break;
				}
				if ((bool)_wepTrail)
				{
					_wepTrail._colors = _scriptWeapon.trailColors;
					_wepTrail._base.localPosition = _scriptWeaponType._weaponTrailBasePosition;
					_wepTrail._tip.localPosition = _scriptWeaponType._weaponTrailEndPosition;
				}
				if (_weaponHasModifier)
				{
					ScriptableStatModifier scriptableStatModifier = GameManager._current.Locate_StatModifier(_modifierID);
					if ((bool)scriptableStatModifier)
					{
						_rightHandWeaponGlowFilter.mesh = _scriptWeapon.weaponMesh;
						_leftHandWeaponGlowFilter.mesh = _scriptWeapon.weaponMesh;
						_rightHandWeaponGlowRender.material.SetColor("_Color", scriptableStatModifier._glowTintColor);
						_leftHandWeaponGlowRender.material.SetColor("_Color", scriptableStatModifier._glowTintColor);
					}
				}
				for (int i = 0; i < _backWeaponRenders.Length; i++)
				{
					_backWeaponRenders[i].enabled = false;
				}
				for (int j = 0; j < _backWeaponGlowRenders.Length; j++)
				{
					_backWeaponGlowRenders[j].enabled = false;
				}
				string weaponHoldClip = _scriptWeapon.weaponType._weaponData._weaponHoldClip;
				if (_playerRaceModel._raceAnimator.isActiveAndEnabled)
				{
					_playerRaceModel._raceAnimator.CrossFade(weaponHoldClip ?? "", 0f, 2);
				}
				_playerRaceModel._canAltIdleAnim = false;
			}
		}
	}

	public void Remove_WeaponDisplay()
	{
		if ((bool)_pCombat)
		{
			if ((bool)_pCombat._equippedWeapon && (bool)_pCombat._currentScriptableWeaponType)
			{
				switch (_pCombat._currentSheathCondition)
				{
				case WeaponSheatheCondition.PUT_AWAY:
				{
					for (int k = 0; k < _backWeaponRenders.Length; k++)
					{
						_backWeaponRenders[k].enabled = false;
					}
					for (int l = 0; l < _backWeaponGlowRenders.Length; l++)
					{
						_backWeaponGlowRenders[l].enabled = false;
					}
					_rightHandWeaponRender.enabled = false;
					_leftHandWeaponRender.enabled = false;
					_rightHandWeaponGlowRender.enabled = false;
					_leftHandWeaponGlowRender.enabled = false;
					break;
				}
				case WeaponSheatheCondition.TAKEN_OUT:
				{
					for (int i = 0; i < _backWeaponRenders.Length; i++)
					{
						_backWeaponRenders[i].enabled = false;
					}
					for (int j = 0; j < _backWeaponGlowRenders.Length; j++)
					{
						_backWeaponGlowRenders[j].enabled = false;
					}
					break;
				}
				}
			}
			else
			{
				Remove_AllWeaponDisplays();
			}
		}
		else
		{
			Remove_AllWeaponDisplays();
		}
		_playerRaceModel._canAltIdleAnim = true;
		void Remove_AllWeaponDisplays()
		{
			_rightHandWeaponRender.enabled = false;
			_leftHandWeaponRender.enabled = false;
			_rightHandWeaponGlowRender.enabled = false;
			_leftHandWeaponGlowRender.enabled = false;
			for (int m = 0; m < _backWeaponRenders.Length; m++)
			{
				_backWeaponRenders[m].enabled = false;
			}
			for (int n = 0; n < _backWeaponGlowRenders.Length; n++)
			{
				_backWeaponGlowRenders[n].enabled = false;
			}
		}
	}

	public void Apply_ShieldDisplay(ScriptableShield _scriptShield, int _modifierID)
	{
		Player player = null;
		if ((bool)_player)
		{
			player = _player;
		}
		_cachedScriptShield = _scriptShield;
		if ((bool)_pCombat)
		{
			PlayerCasting pCasting = player._pCasting;
			if (player._currentPlayerAction == PlayerAction.CASTING && (bool)pCasting._currentCastSkill && pCasting._currentCastSkill._hideWeapon)
			{
				_hideTakenOutWeaponBuffer = 0.85f;
			}
			if (_hideTakenOutWeaponBuffer > 0f)
			{
				if ((bool)_scriptWeaponType && _scriptWeaponType._weaponHandedness == WeaponHandedness.Two_Handed)
				{
					Remove_ShieldDisplay();
				}
				else
				{
					Display_PutAwayShield(_modifierID);
				}
				return;
			}
			switch (_pCombat._currentSheathCondition)
			{
			case WeaponSheatheCondition.TAKEN_OUT:
				if ((bool)_cachedScriptShield && (bool)_scriptWeaponType && _scriptWeaponType._weaponHandedness == WeaponHandedness.Two_Handed)
				{
					Remove_ShieldDisplay();
				}
				else
				{
					Display_TakenOutShield(_modifierID);
				}
				break;
			case WeaponSheatheCondition.PUT_AWAY:
				if ((bool)_scriptWeaponType && _scriptWeaponType._weaponHandedness == WeaponHandedness.Two_Handed)
				{
					Remove_ShieldDisplay();
				}
				else
				{
					Display_PutAwayShield(_modifierID);
				}
				break;
			}
		}
		else
		{
			if (!Player._mainPlayer)
			{
				return;
			}
			switch (Player._mainPlayer._pCombat._currentSheathCondition)
			{
			case WeaponSheatheCondition.TAKEN_OUT:
				if ((bool)_cachedScriptShield && (bool)_scriptWeaponType && _scriptWeaponType._weaponHandedness == WeaponHandedness.Two_Handed)
				{
					Remove_ShieldDisplay();
				}
				else
				{
					Display_TakenOutShield(_modifierID);
				}
				break;
			case WeaponSheatheCondition.PUT_AWAY:
				if ((bool)_scriptWeaponType && _scriptWeaponType._weaponHandedness == WeaponHandedness.Two_Handed)
				{
					Remove_ShieldDisplay();
				}
				else
				{
					Display_PutAwayShield(_modifierID);
				}
				break;
			}
		}
	}

	public void Display_TakenOutShield(int _modifierID)
	{
		if (!_cachedScriptShield)
		{
			return;
		}
		MeshFilter component = _takenOutShieldRender.GetComponent<MeshFilter>();
		component.mesh = _cachedScriptShield._shieldMesh;
		_takenOutShieldRender.material.mainTexture = _cachedScriptShield._shieldTexture;
		Set_MaterialColorProfile(_takenOutShieldRender.material, _cachedScriptShield._textureColorAdjust);
		_takenOutShieldRender.gameObject.SetActive(value: true);
		_putAwayShieldRender.gameObject.SetActive(value: false);
		_takenOutShieldGlowRender.gameObject.SetActive(_modifierID > 0 && !SettingsManager._current._disableWeaponGlowToggle.isOn);
		_putAwayShieldGlowRender.gameObject.SetActive(value: false);
		if (_modifierID > 0 && !SettingsManager._current._disableWeaponGlowToggle.isOn)
		{
			_takenOutShieldGlowRender.GetComponent<MeshFilter>().mesh = component.mesh;
			ScriptableStatModifier scriptableStatModifier = GameManager._current.Locate_StatModifier(_modifierID);
			if ((bool)scriptableStatModifier)
			{
				_takenOutShieldGlowRender.material.SetColor("_Color", scriptableStatModifier._glowTintColor);
			}
		}
		if (_playerRaceModel._raceAnimator.isActiveAndEnabled && (bool)Player._mainPlayer)
		{
			_playerRaceModel._raceAnimator.CrossFade("Hold_Shield", 0f, 4);
		}
	}

	public void Display_PutAwayShield(int _modifierID)
	{
		if (!_cachedScriptShield)
		{
			return;
		}
		MeshFilter component = _putAwayShieldRender.GetComponent<MeshFilter>();
		component.mesh = _cachedScriptShield._shieldMesh;
		_putAwayShieldRender.material.mainTexture = _cachedScriptShield._shieldTexture;
		Set_MaterialColorProfile(_putAwayShieldRender.material, _cachedScriptShield._textureColorAdjust);
		_takenOutShieldRender.gameObject.SetActive(value: false);
		_putAwayShieldRender.gameObject.SetActive(value: true);
		_takenOutShieldGlowRender.gameObject.SetActive(value: false);
		_putAwayShieldGlowRender.gameObject.SetActive(_modifierID > 0 && !SettingsManager._current._disableWeaponGlowToggle.isOn);
		if (_modifierID > 0 && !SettingsManager._current._disableWeaponGlowToggle.isOn)
		{
			_putAwayShieldGlowRender.GetComponent<MeshFilter>().mesh = component.mesh;
			ScriptableStatModifier scriptableStatModifier = GameManager._current.Locate_StatModifier(_modifierID);
			if ((bool)scriptableStatModifier)
			{
				_putAwayShieldGlowRender.material.SetColor("_Color", scriptableStatModifier._glowTintColor);
			}
		}
		if (_hideTakenOutWeaponBuffer <= 0f)
		{
			if (_playerRaceModel._raceAnimator.isActiveAndEnabled && (bool)Player._mainPlayer)
			{
				_playerRaceModel._raceAnimator.CrossFade("Hold_None", 0f, 4);
			}
			_playerRaceModel._canAltIdleAnim = true;
		}
	}

	public void Remove_ShieldDisplay()
	{
		_cachedScriptShield = null;
		_takenOutShieldRender.gameObject.SetActive(value: false);
		_putAwayShieldRender.gameObject.SetActive(value: false);
		if (_playerRaceModel._raceAnimator.isActiveAndEnabled && (bool)GetComponentInParent<Player>())
		{
			_playerRaceModel._raceAnimator.CrossFade("Hold_None", 0f, 4);
		}
	}

	private void Set_MaterialColorProfile(Material _mat, ColorAdjustShader_Profile _profile)
	{
		_mat.SetFloat(_HUE, _profile._hue);
		_mat.SetFloat(_BRIGHTNESS, _profile._brightness);
		_mat.SetFloat(_CONTRAST, _profile._contrast);
		_mat.SetFloat(_SATURATION, _profile._saturation);
	}
}
