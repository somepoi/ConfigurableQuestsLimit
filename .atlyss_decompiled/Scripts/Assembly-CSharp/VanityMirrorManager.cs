using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VanityMirrorManager : MonoBehaviour
{
	private enum Gamepad_VanityMirrorTab
	{
		Customizer_bar = 0,
		Customizer_window = 1,
		Dialog_box = 2
	}

	public static VanityMirrorManager _current;

	[Header("Runtime")]
	public bool _isOpen;

	private bool _initOpenWindow;

	private int _transmogCurrencyCount;

	private float _rotationValue;

	private bool _isRotatingDisplay;

	private bool _clothesOff;

	private bool _changedColor;

	private bool _changedHead;

	private bool _changedBody;

	private bool _displayBoobs;

	private Transform _raceModelDisplayContainer;

	private PlayerVisual _pVisual;

	private ScriptablePlayerRace _scriptRace;

	private PlayerRaceModel _setRaceModel;

	private RaceModelEquipDisplay _eD;

	[Header("Vanity Mirror Manager")]
	[SerializeField]
	private MenuElement _menuElement_00;

	[SerializeField]
	private MenuElement _menuElement_01;

	[SerializeField]
	private Button _applyAppearanceButton;

	[SerializeField]
	private Button _resetAppearanceButton;

	[SerializeField]
	private Text _illusionStoneQuantityCounter;

	[SerializeField]
	private Toggle _clothesOffToggle;

	[Header("Customization Tabs")]
	[SerializeField]
	private Transform _customizerBarContent;

	[SerializeField]
	private MenuElement[] _customizationTabs = new MenuElement[4];

	[SerializeField]
	private int _currentCustomizationTab;

	[Header("Color Customization")]
	[SerializeField]
	private Image _bodyHueSliderImage;

	[SerializeField]
	private Image _hairHueSliderImage;

	[SerializeField]
	private Text _colorMiscTag;

	[SerializeField]
	private Slider _hueSlider;

	[SerializeField]
	private Slider _brightnessSlider;

	[SerializeField]
	private Slider _contrastSlider;

	[SerializeField]
	private Slider _saturationSlider;

	[Space]
	[SerializeField]
	private Toggle _hairIsBodyColorToggle;

	[SerializeField]
	private Slider _hairHueSlider;

	[SerializeField]
	private Slider _hairBrightnessSlider;

	[SerializeField]
	private Slider _hairContrastSlider;

	[SerializeField]
	private Slider _hairSaturationSlider;

	[SerializeField]
	private Slider _miscHueSlider;

	[SerializeField]
	private Slider _miscBrightnessSlider;

	[SerializeField]
	private Slider _miscContrastSlider;

	[SerializeField]
	private Slider _miscSaturationSlider;

	[Header("Head Customization")]
	[SerializeField]
	private GameObject _earSelectorObj;

	[SerializeField]
	private GameObject _mouthSelectorObj;

	[SerializeField]
	private Text _miscTag;

	[SerializeField]
	private Slider _headSlider_headWidth;

	[SerializeField]
	private Slider _headSlider_muzzle;

	[Header("Body Customization")]
	[SerializeField]
	private Slider _bodySlider_height;

	[SerializeField]
	private Slider _bodySlider_width;

	[SerializeField]
	private Slider _bodySlider_armWidth;

	[SerializeField]
	private Slider _bodySlider_chestSize;

	[SerializeField]
	private Slider _bodySlider_bellySize;

	[SerializeField]
	private Slider _bodySlider_bottomWidth;

	[SerializeField]
	private Toggle _leftHandedToggle;

	[SerializeField]
	private Image _chestIcon;

	[SerializeField]
	private Sprite _boobIcoSprite;

	[SerializeField]
	private Sprite _torsoIcoSprite;

	[Header("Audio")]
	[SerializeField]
	private Slider _voicePitchSlider;

	[Header("Gamepad Elements")]
	[SerializeField]
	private Gamepad_VanityMirrorTab _gamepad_currentMirrorTab;

	[SerializeField]
	private Image _gamepad_changeAppearanceIco;

	[SerializeField]
	private Image _gamepad_rsetAppearanceIco;

	[Space]
	[SerializeField]
	private Image _gamepad_viewportIco_leftTrigger;

	[SerializeField]
	private Image _gamepad_viewportIco_rightTrigger;

	[Space]
	[SerializeField]
	private Image[] _gamepad_customizerWindowIcons;

	private int _gamepad_setvanityMirrorTab;

	private int _gamepad_customizerBarSelection;

	private int _gamepad_colorSelection;

	private int _gamepad_headSelection;

	private int _gamepad_bodySelection;

	private SettingsCell[] _gamepad_customizeSelectorCells;

	private SettingsCell[] _gamepad_colorSelectorCells;

	private SettingsCell[] _gamepad_headSelectorCells;

	private SettingsCell[] _gamepad_bodySelectorCells;

	public int Gamepad_SetVanityMirrorTab
	{
		get
		{
			return _gamepad_setvanityMirrorTab;
		}
		set
		{
			if (value < 0)
			{
				value = Enum.GetValues(typeof(Gamepad_VanityMirrorTab)).Length - 1;
			}
			if (value > Enum.GetValues(typeof(Gamepad_VanityMirrorTab)).Length - 1)
			{
				value = 0;
			}
			_gamepad_setvanityMirrorTab = value;
		}
	}

	public int Gamepad_CustomizerBarSelection
	{
		get
		{
			return _gamepad_customizerBarSelection;
		}
		set
		{
			if (value < 0)
			{
				value = _gamepad_customizeSelectorCells.Length - 1;
			}
			if (value > _gamepad_customizeSelectorCells.Length - 1)
			{
				value = 0;
			}
			_gamepad_customizerBarSelection = value;
		}
	}

	public int Gamepad_ColorSelection
	{
		get
		{
			return _gamepad_colorSelection;
		}
		set
		{
			if (value < 0)
			{
				value = _gamepad_colorSelectorCells.Length - 1;
			}
			if (value > _gamepad_colorSelectorCells.Length - 1)
			{
				value = 0;
			}
			_gamepad_colorSelection = value;
		}
	}

	public int Gamepad_HeadSelection
	{
		get
		{
			return _gamepad_headSelection;
		}
		set
		{
			if (value < 0)
			{
				value = _gamepad_headSelectorCells.Length - 1;
			}
			if (value > _gamepad_headSelectorCells.Length - 1)
			{
				value = 0;
			}
			_gamepad_headSelection = value;
		}
	}

	public int Gamepad_BodySelection
	{
		get
		{
			return _gamepad_bodySelection;
		}
		set
		{
			if (value < 0)
			{
				value = _gamepad_bodySelectorCells.Length - 1;
			}
			if (value > _gamepad_bodySelectorCells.Length - 1)
			{
				value = 0;
			}
			_gamepad_bodySelection = value;
		}
	}

	private void Awake()
	{
		_current = this;
	}

	private void Start()
	{
		Init_SetButtons();
		Init_SetGamepadCells();
	}

	private void Init_SetButtons()
	{
		_applyAppearanceButton.onClick.AddListener(Send_NewCharacterAppearance);
		_resetAppearanceButton.onClick.AddListener(Reset_All);
	}

	private void Init_SetGamepadCells()
	{
		_gamepad_customizeSelectorCells = _customizerBarContent.GetComponentsInChildren<SettingsCell>();
		_gamepad_colorSelectorCells = _customizationTabs[0].GetComponentsInChildren<SettingsCell>();
		_gamepad_headSelectorCells = _customizationTabs[1].GetComponentsInChildren<SettingsCell>();
		_gamepad_bodySelectorCells = _customizationTabs[2].GetComponentsInChildren<SettingsCell>();
	}

	private void Update()
	{
		if (!Player._mainPlayer)
		{
			return;
		}
		if (!_pVisual)
		{
			_pVisual = Player._mainPlayer._pVisual;
		}
		if (!_raceModelDisplayContainer)
		{
			_raceModelDisplayContainer = TabMenu._current._itemCell._raceModelDisplayContainer;
		}
		_isOpen = DialogManager._current._currentDialogUIPrompt == DialogUIPrompt.VANITY_MIRROR;
		_menuElement_00.gameObject.SetActive(_isOpen);
		_menuElement_01.gameObject.SetActive(_isOpen);
		if (_isOpen && !_initOpenWindow)
		{
			StartCoroutine(Init_MenuElement());
		}
		if (!_isOpen)
		{
			_menuElement_00.isEnabled = false;
			_menuElement_01.isEnabled = false;
			_currentCustomizationTab = 0;
			_clothesOffToggle.isOn = false;
			_initOpenWindow = false;
			_gamepad_currentMirrorTab = Gamepad_VanityMirrorTab.Customizer_bar;
			_gamepad_customizerBarSelection = 0;
			_gamepad_colorSelection = 0;
			_gamepad_headSelection = 0;
			_gamepad_bodySelection = 0;
			return;
		}
		_transmogCurrencyCount = Retrieve_IllusionStoneCount();
		_illusionStoneQuantityCounter.text = $"x{_transmogCurrencyCount}";
		_applyAppearanceButton.interactable = _changedHead || _changedColor || _changedBody;
		MenuElement[] customizationTabs = _customizationTabs;
		for (int i = 0; i < customizationTabs.Length; i++)
		{
			customizationTabs[i].isEnabled = false;
		}
		_customizationTabs[_currentCustomizationTab].isEnabled = true;
		Handle_RaceModelViewport();
		Handle_GamepadControl();
		if ((bool)_scriptRace)
		{
			_colorMiscTag.text = _scriptRace._miscName ?? "";
			_miscTag.text = _scriptRace._miscName ?? "";
			_earSelectorObj.SetActive(!_scriptRace._disableEarSelector);
			_mouthSelectorObj.SetActive(!_scriptRace._disableMouthSelector);
			_bodyHueSliderImage.sprite = _scriptRace._raceDisplayParams._bodyHueSliderSprite;
			_hairHueSliderImage.sprite = _scriptRace._raceDisplayParams._bodyHueSliderSprite;
		}
		static int Retrieve_IllusionStoneCount()
		{
			return Player._mainPlayer._pInventory.Check_ItemQuantity(GameManager._current._statLogics._transmogCurrency);
		}
	}

	private void Handle_GamepadControl()
	{
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_gamepad_viewportIco_leftTrigger.enabled = flag;
		_gamepad_viewportIco_rightTrigger.enabled = flag;
		_gamepad_changeAppearanceIco.enabled = flag && _gamepad_currentMirrorTab == Gamepad_VanityMirrorTab.Customizer_bar;
		_gamepad_rsetAppearanceIco.enabled = flag && _gamepad_currentMirrorTab == Gamepad_VanityMirrorTab.Customizer_bar;
		for (int i = 0; i < _gamepad_customizerWindowIcons.Length; i++)
		{
			_gamepad_customizerWindowIcons[i].enabled = flag && _gamepad_currentMirrorTab == Gamepad_VanityMirrorTab.Customizer_window;
		}
		if (!flag || DialogManager._current._currentDialogUIPrompt != DialogUIPrompt.VANITY_MIRROR)
		{
			return;
		}
		bool flag2 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Up) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Up) || InputControlManager.current.DpadUp(DpadHoldType.OnDown);
		bool flag3 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Down) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Down) || InputControlManager.current.DpadDown(DpadHoldType.OnDown);
		if (GamepadInput.GetButtonDown(GamepadButton.LeftStick_Left) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Left))
		{
			_ = 1;
		}
		else
			InputControlManager.current.DpadLeft(DpadHoldType.OnDown);
		if (GamepadInput.GetButtonDown(GamepadButton.LeftStick_Right) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Right))
		{
			_ = 1;
		}
		else
			InputControlManager.current.DpadRight(DpadHoldType.OnDown);
		bool buttonDown = GamepadInput.GetButtonDown(GamepadButton.LeftShoulder);
		bool buttonDown2 = GamepadInput.GetButtonDown(GamepadButton.RightShoulder);
		if (GamepadInput.GetButton(GamepadButton.RightTrigger))
		{
			_gamepad_viewportIco_rightTrigger.color = Color.white;
		}
		else
		{
			_gamepad_viewportIco_rightTrigger.color = Color.grey;
		}
		if (GamepadInput.GetButton(GamepadButton.LeftTrigger))
		{
			_gamepad_viewportIco_leftTrigger.color = Color.white;
		}
		else
		{
			_gamepad_viewportIco_leftTrigger.color = Color.grey;
		}
		if (GamepadInput.GetButtonDown(GamepadButton.RightTrigger))
		{
			Start_RaceModelDisplayRotation_Right();
		}
		if (GamepadInput.GetButtonDown(GamepadButton.LeftTrigger))
		{
			Start_RaceModelDisplayRotation_Left();
		}
		if (GamepadInput.GetButtonUp(GamepadButton.RightTrigger))
		{
			Stop_RaceModelDisplayRotation();
		}
		if (GamepadInput.GetButtonUp(GamepadButton.LeftTrigger))
		{
			Stop_RaceModelDisplayRotation();
		}
		if (GamepadInput.GetButtonDown(GamepadButton.RightStick))
		{
			Reset_RaceModelDisplayRotation();
		}
		for (int j = 0; j < _gamepad_customizeSelectorCells.Length; j++)
		{
			_gamepad_customizeSelectorCells[j]._isSelected = _gamepad_currentMirrorTab == Gamepad_VanityMirrorTab.Customizer_bar && j == _gamepad_customizerBarSelection;
		}
		for (int k = 0; k < _gamepad_colorSelectorCells.Length; k++)
		{
			_gamepad_colorSelectorCells[k]._isSelected = _currentCustomizationTab == 0 && _gamepad_currentMirrorTab == Gamepad_VanityMirrorTab.Customizer_window && k == _gamepad_colorSelection;
		}
		for (int l = 0; l < _gamepad_headSelectorCells.Length; l++)
		{
			_gamepad_headSelectorCells[l]._isSelected = _currentCustomizationTab == 1 && _gamepad_currentMirrorTab == Gamepad_VanityMirrorTab.Customizer_window && l == _gamepad_headSelection;
		}
		for (int m = 0; m < _gamepad_bodySelectorCells.Length; m++)
		{
			_gamepad_bodySelectorCells[m]._isSelected = _currentCustomizationTab == 2 && _gamepad_currentMirrorTab == Gamepad_VanityMirrorTab.Customizer_window && m == _gamepad_bodySelection;
		}
		if (buttonDown)
		{
			Gamepad_SetVanityMirrorTab--;
			ToolTipManager._current.Disable_AllTooltips();
			TabMenu._current._gamepadSelectAsrc.Play();
		}
		if (buttonDown2)
		{
			Gamepad_SetVanityMirrorTab++;
			ToolTipManager._current.Disable_AllTooltips();
			TabMenu._current._gamepadSelectAsrc.Play();
		}
		_gamepad_currentMirrorTab = (Gamepad_VanityMirrorTab)_gamepad_setvanityMirrorTab;
		switch (_gamepad_currentMirrorTab)
		{
		case Gamepad_VanityMirrorTab.Customizer_bar:
			if (flag2)
			{
				Gamepad_CustomizerBarSelection--;
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (flag3)
			{
				Gamepad_CustomizerBarSelection++;
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Start))
			{
				Send_NewCharacterAppearance();
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Back))
			{
				Reset_All();
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			_gamepad_colorSelection = 0;
			_gamepad_headSelection = 0;
			_gamepad_bodySelection = 0;
			break;
		case Gamepad_VanityMirrorTab.Customizer_window:
			switch (_currentCustomizationTab)
			{
			case 0:
				if (flag2)
				{
					Gamepad_ColorSelection--;
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (flag3)
				{
					Gamepad_ColorSelection++;
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
				{
					Randomize_ColorSliders();
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				break;
			case 1:
				if (flag2)
				{
					Gamepad_HeadSelection--;
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (flag3)
				{
					Gamepad_HeadSelection++;
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
				{
					Randomize_HeadSettings();
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				break;
			case 2:
				if (flag2)
				{
					Gamepad_BodySelection--;
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (flag3)
				{
					Gamepad_BodySelection++;
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Start))
				{
					SelectBodyPreset_Male();
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Back))
				{
					SelectBodyPreset_Female();
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
				{
					Randomize_BodySliders();
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				break;
			}
			break;
		case Gamepad_VanityMirrorTab.Dialog_box:
			DialogManager._current._currentGamepadTab = DialogManager_GamepadTab.Dialog_Box;
			_gamepad_colorSelection = 0;
			_gamepad_headSelection = 0;
			_gamepad_bodySelection = 0;
			break;
		}
	}

	public void Set_CustomizationTab(int _index)
	{
		_currentCustomizationTab = _index;
	}

	private IEnumerator Init_MenuElement()
	{
		_initOpenWindow = true;
		yield return new WaitForSeconds(0.3f);
		_menuElement_00.isEnabled = true;
		_menuElement_01.isEnabled = true;
	}

	private void Handle_RaceModelViewport()
	{
		if (!_pVisual._playerRaceModel)
		{
			return;
		}
		if (!_setRaceModel)
		{
			for (int i = 0; i < TabMenu._current._itemCell._playerRaceModelDisplays.Length; i++)
			{
				PlayerRaceModel[] playerRaceModelDisplays = TabMenu._current._itemCell._playerRaceModelDisplays;
				if (_pVisual._playerRaceModel._scriptablePlayerRace != playerRaceModelDisplays[i]._scriptablePlayerRace)
				{
					playerRaceModelDisplays[i].gameObject.SetActive(value: false);
				}
				else
				{
					_setRaceModel = TabMenu._current._itemCell._playerRaceModelDisplays[i];
				}
			}
		}
		else
		{
			_scriptRace = _setRaceModel._scriptablePlayerRace;
			Apply_RaceModelDisplayParams();
		}
	}

	private void Apply_RaceModelDisplayParams()
	{
		if (_isRotatingDisplay)
		{
			_raceModelDisplayContainer.Rotate(_rotationValue * Time.deltaTime * Vector3.up);
		}
		ProfileDataManager current = ProfileDataManager._current;
		PlayerAppearance_Profile _pap = current._characterFile._appearanceProfile;
		_eD = _setRaceModel._equipDisplay;
		Check_ColorParams();
		Check_HeadParams();
		Check_BodyParams();
		if (!_setRaceModel._isCharacterCreationDisplay)
		{
			StartCoroutine(Init_ResetParams());
			_setRaceModel._isCharacterCreationDisplay = true;
		}
		_setRaceModel.Apply_CharacterDisplay();
		_setRaceModel._armatureTransform.localScale = new Vector3(_bodySlider_width.value, _bodySlider_height.value, _bodySlider_width.value);
		_setRaceModel._displayBoobs = _displayBoobs;
		_setRaceModel._headBoneTransform.localScale = new Vector3(_headSlider_headWidth.value, 1f, 1f);
		_setRaceModel._assDynamicBones[0].enabled = _setRaceModel._baseBodyMesh.GetBlendShapeWeight(1) >= 45f;
		_setRaceModel._assDynamicBones[1].enabled = _setRaceModel._baseBodyMesh.GetBlendShapeWeight(1) >= 45f;
		_setRaceModel._bellyDynamicBone[0].enabled = _setRaceModel._baseBodyMesh.GetBlendShapeWeight(0) >= 45f;
		if (_leftHandedToggle.isOn)
		{
			_setRaceModel.transform.parent.localScale = new Vector3(-1f, 1f, 1f);
		}
		else
		{
			_setRaceModel.transform.parent.localScale = Vector3.one;
		}
		_eD._robeSkirtMeshRender.SetBlendShapeWeight(0, _bodySlider_bottomWidth.value);
		_eD._robeSkirtMeshRender.SetBlendShapeWeight(1, _bodySlider_bellySize.value);
		if ((bool)_eD._armCuffMeshRender.sharedMesh)
		{
			_eD._armCuffMeshRender.SetBlendShapeWeight(0, _bodySlider_armWidth.value);
		}
		_eD._hideHelmVisual = true;
		_eD._hideCapeVisual = true;
		Init_SliderParams();
		_setRaceModel._faceMesh.SetBlendShapeWeight(1, _headSlider_muzzle.value);
		_setRaceModel._baseBodyMesh.SetBlendShapeWeight(0, _bodySlider_bellySize.value);
		_setRaceModel._baseBodyMesh.SetBlendShapeWeight(1, _bodySlider_bottomWidth.value);
		_setRaceModel._baseBodyMesh.SetBlendShapeWeight(3, _headSlider_muzzle.value);
		_eD._armorBodyMesh.SetBlendShapeWeight(0, _bodySlider_bellySize.value);
		_eD._armorBodyMesh.SetBlendShapeWeight(1, _bodySlider_bottomWidth.value);
		_eD._armorBodyMesh.SetBlendShapeWeight(3, _headSlider_muzzle.value);
		_eD._chestPieceLegRender.SetBlendShapeWeight(0, _bodySlider_bellySize.value);
		_eD._chestPieceLegRender.SetBlendShapeWeight(1, _bodySlider_bottomWidth.value);
		_eD._chestPieceLegRender.SetBlendShapeWeight(3, _headSlider_muzzle.value);
		_setRaceModel._baseArmMesh.SetBlendShapeWeight(0, _bodySlider_armWidth.value);
		_eD._armorArmMesh.SetBlendShapeWeight(0, _bodySlider_armWidth.value);
		int index = 4;
		if (_eD._hasHeadBlendWeights)
		{
			index = 5;
		}
		if (_bodySlider_bottomWidth.value < 0f)
		{
			_setRaceModel._baseBodyMesh.SetBlendShapeWeight(1, 0f);
			_eD._armorBodyMesh.SetBlendShapeWeight(1, 0f);
			_eD._chestPieceLegRender.SetBlendShapeWeight(1, 0f);
			_eD._robeSkirtMeshRender.SetBlendShapeWeight(0, 0f);
			_setRaceModel._baseBodyMesh.SetBlendShapeWeight(index, _bodySlider_bottomWidth.value * -1f);
			_eD._armorBodyMesh.SetBlendShapeWeight(index, _bodySlider_bottomWidth.value * -1f);
			_eD._chestPieceLegRender.SetBlendShapeWeight(index, _bodySlider_bottomWidth.value * -1f);
			_eD._robeSkirtMeshRender.SetBlendShapeWeight(2, _bodySlider_bottomWidth.value * -1f);
		}
		else
		{
			_setRaceModel._baseBodyMesh.SetBlendShapeWeight(index, 0f);
			_eD._armorBodyMesh.SetBlendShapeWeight(index, 0f);
			_eD._chestPieceLegRender.SetBlendShapeWeight(index, 0f);
			_eD._robeSkirtMeshRender.SetBlendShapeWeight(2, 0f);
		}
		_setRaceModel._setSkinColorProfile._hue = _hueSlider.value;
		_setRaceModel._setSkinColorProfile._brightness = _brightnessSlider.value;
		_setRaceModel._setSkinColorProfile._contrast = _contrastSlider.value;
		_setRaceModel._setSkinColorProfile._saturation = _saturationSlider.value;
		_setRaceModel._setHairColorProfile._hue = _hairHueSlider.value;
		_setRaceModel._setHairColorProfile._brightness = _hairBrightnessSlider.value;
		_setRaceModel._setHairColorProfile._contrast = _hairContrastSlider.value;
		_setRaceModel._setHairColorProfile._saturation = _hairSaturationSlider.value;
		_setRaceModel._hairHueIsBodyHue = _hairIsBodyColorToggle.isOn;
		if (!_setRaceModel._hairHueIsBodyHue)
		{
			_hairHueSlider.handleRect.GetComponent<Image>().color = Color.white;
			_hairBrightnessSlider.handleRect.GetComponent<Image>().color = Color.white;
			_hairContrastSlider.handleRect.GetComponent<Image>().color = Color.white;
			_hairSaturationSlider.handleRect.GetComponent<Image>().color = Color.white;
		}
		else
		{
			_hairHueSlider.handleRect.GetComponent<Image>().color = Color.grey;
			_hairBrightnessSlider.handleRect.GetComponent<Image>().color = Color.grey;
			_hairContrastSlider.handleRect.GetComponent<Image>().color = Color.grey;
			_hairSaturationSlider.handleRect.GetComponent<Image>().color = Color.grey;
		}
		_setRaceModel._setMiscColorProfile._hue = _miscHueSlider.value;
		_setRaceModel._setMiscColorProfile._brightness = _miscBrightnessSlider.value;
		_setRaceModel._setMiscColorProfile._contrast = _miscContrastSlider.value;
		_setRaceModel._setMiscColorProfile._saturation = _miscSaturationSlider.value;
		_hairHueSlider.interactable = !_setRaceModel._hairHueIsBodyHue;
		_hairBrightnessSlider.interactable = !_setRaceModel._hairHueIsBodyHue;
		_hairContrastSlider.interactable = !_setRaceModel._hairHueIsBodyHue;
		_hairSaturationSlider.interactable = !_setRaceModel._hairHueIsBodyHue;
		_clothesOff = _clothesOffToggle.isOn;
		_eD._hideChestVisual = _clothesOff;
		_eD._hideLeggingsVisual = _clothesOff;
		void Check_BodyParams()
		{
			PlayerRaceModel _mainRaceModel = Player._mainPlayer._pVisual._playerRaceModel;
			_changedBody = Check_ForChangedBodyParams();
			bool Check_ForChangedBodyParams()
			{
				if (_bodySlider_height.value != _pap._heightWeight)
				{
					return true;
				}
				if (_bodySlider_width.value != _pap._widthWeight)
				{
					return true;
				}
				if (_bodySlider_armWidth.value != _pap._armWeight)
				{
					return true;
				}
				if (_bodySlider_bellySize.value != _mainRaceModel._bellyWeight)
				{
					return true;
				}
				if (_bodySlider_bottomWidth.value != _mainRaceModel._bottomWeight)
				{
					return true;
				}
				if (_leftHandedToggle.isOn != _pap._isLeftHanded)
				{
					return true;
				}
				if (_displayBoobs != _pap._displayBoobs)
				{
					return true;
				}
				if (_pap._displayBoobs)
				{
					if (_bodySlider_chestSize.value != _mainRaceModel._boobWeight)
					{
						return true;
					}
				}
				else if (_bodySlider_chestSize.value != _mainRaceModel._torsoWeight)
				{
					return true;
				}
				if (_setRaceModel.SetTailIndex != _pap._setTail)
				{
					return true;
				}
				return false;
			}
		}
		void Check_ColorParams()
		{
			_changedColor = Check_ForChangedColorParams();
		}
		bool Check_ForChangedColorParams()
		{
			if (_setRaceModel._setSkinColorProfile._hue != _pap._setSkinColorProfile._hue)
			{
				return true;
			}
			if (_setRaceModel._setSkinColorProfile._brightness != _pap._setSkinColorProfile._brightness)
			{
				return true;
			}
			if (_setRaceModel._setSkinColorProfile._contrast != _pap._setSkinColorProfile._contrast)
			{
				return true;
			}
			if (_setRaceModel._setSkinColorProfile._saturation != _pap._setSkinColorProfile._saturation)
			{
				return true;
			}
			if (_setRaceModel._setHairColorProfile._hue != _pap._hairColorProfile._hue)
			{
				return true;
			}
			if (_setRaceModel._setHairColorProfile._brightness != _pap._hairColorProfile._brightness)
			{
				return true;
			}
			if (_setRaceModel._setHairColorProfile._contrast != _pap._hairColorProfile._contrast)
			{
				return true;
			}
			if (_setRaceModel._setHairColorProfile._saturation != _pap._hairColorProfile._saturation)
			{
				return true;
			}
			if (_setRaceModel._hairHueIsBodyHue != _pap._hairIsBodyColor)
			{
				return true;
			}
			if (_setRaceModel._setMiscColorProfile._hue != _pap._miscColorProfile._hue)
			{
				return true;
			}
			if (_setRaceModel._setMiscColorProfile._brightness != _pap._miscColorProfile._brightness)
			{
				return true;
			}
			if (_setRaceModel._setMiscColorProfile._contrast != _pap._miscColorProfile._contrast)
			{
				return true;
			}
			if (_setRaceModel._setMiscColorProfile._saturation != _pap._miscColorProfile._saturation)
			{
				return true;
			}
			if (_setRaceModel.SetTextureIndex != _pap._setTexture)
			{
				return true;
			}
			return false;
		}
		bool Check_ForChangedHeadParams()
		{
			if (_headSlider_headWidth.value != _pap._headWidth)
			{
				return true;
			}
			if (_headSlider_muzzle.value != _pap._muzzleWeight)
			{
				return true;
			}
			if (_setRaceModel.SetHairIndex != _pap._setHairStyle)
			{
				return true;
			}
			if (_setRaceModel.SetEarIndex != _pap._setEar)
			{
				return true;
			}
			if (_setRaceModel.SetEyeIndex != _pap._setEye)
			{
				return true;
			}
			if (_setRaceModel.SetMouthIndex != _pap._setMouth)
			{
				return true;
			}
			if (_setRaceModel.SetMiscIndex != _pap._setMisc)
			{
				return true;
			}
			if (_voicePitchSlider.value != _pap._voicePitch)
			{
				return true;
			}
			return false;
		}
		void Check_HeadParams()
		{
			_changedHead = Check_ForChangedHeadParams();
		}
		void Init_SliderParams()
		{
			CharacterParamsGroup raceDisplayParams = _setRaceModel._scriptablePlayerRace._raceDisplayParams;
			_hueSlider.minValue = raceDisplayParams._bodyHueRange.x;
			_brightnessSlider.minValue = raceDisplayParams._bodyBrightnessRange.x;
			_contrastSlider.minValue = raceDisplayParams._bodyContrastRange.x;
			_saturationSlider.minValue = raceDisplayParams._bodySaturationRange.x;
			_hueSlider.maxValue = raceDisplayParams._bodyHueRange.y;
			_brightnessSlider.maxValue = raceDisplayParams._bodyBrightnessRange.y;
			_contrastSlider.maxValue = raceDisplayParams._bodyContrastRange.y;
			_saturationSlider.maxValue = raceDisplayParams._bodySaturationRange.y;
			_hairHueSlider.minValue = _hueSlider.minValue;
			_hairHueSlider.maxValue = _hueSlider.maxValue;
			_hairBrightnessSlider.minValue = _brightnessSlider.minValue;
			_hairBrightnessSlider.maxValue = _brightnessSlider.maxValue;
			_hairContrastSlider.minValue = _contrastSlider.minValue;
			_hairContrastSlider.maxValue = _contrastSlider.maxValue;
			_hairSaturationSlider.minValue = _saturationSlider.minValue;
			_hairSaturationSlider.maxValue = _saturationSlider.maxValue;
			_miscHueSlider.minValue = _hueSlider.minValue;
			_miscHueSlider.maxValue = _hueSlider.maxValue;
			_miscBrightnessSlider.minValue = _brightnessSlider.minValue;
			_miscBrightnessSlider.maxValue = _brightnessSlider.maxValue;
			_miscContrastSlider.minValue = _contrastSlider.minValue;
			_miscContrastSlider.maxValue = _contrastSlider.maxValue;
			_miscSaturationSlider.minValue = _saturationSlider.minValue;
			_miscSaturationSlider.maxValue = _saturationSlider.maxValue;
			_headSlider_headWidth.minValue = raceDisplayParams._headWidthRange.x;
			_headSlider_headWidth.maxValue = raceDisplayParams._headWidthRange.y;
			_headSlider_muzzle.minValue = raceDisplayParams._headModRange.x;
			_headSlider_muzzle.maxValue = raceDisplayParams._headModRange.y;
			_voicePitchSlider.minValue = raceDisplayParams._pitchRange.x;
			_voicePitchSlider.maxValue = raceDisplayParams._pitchRange.y;
			_bodySlider_height.minValue = raceDisplayParams._heightRange.x;
			_bodySlider_height.maxValue = raceDisplayParams._heightRange.y;
			_bodySlider_width.minValue = raceDisplayParams._widthRange.x;
			_bodySlider_width.maxValue = raceDisplayParams._widthRange.y;
			if (_displayBoobs)
			{
				_setRaceModel._baseBoobMesh.SetBlendShapeWeight(0, _bodySlider_chestSize.value);
				_eD._armorBoobMesh.SetBlendShapeWeight(0, _bodySlider_chestSize.value);
				_setRaceModel._baseBodyMesh.SetBlendShapeWeight(2, 0f);
				_eD._armorBodyMesh.SetBlendShapeWeight(2, 0f);
				_bodySlider_chestSize.minValue = raceDisplayParams._boobRange.x;
				_bodySlider_chestSize.maxValue = raceDisplayParams._boobRange.y;
				_chestIcon.sprite = _boobIcoSprite;
			}
			else
			{
				_setRaceModel._baseBodyMesh.SetBlendShapeWeight(2, _bodySlider_chestSize.value);
				_eD._armorBodyMesh.SetBlendShapeWeight(2, _bodySlider_chestSize.value);
				_setRaceModel._baseBoobMesh.SetBlendShapeWeight(0, 0f);
				_eD._armorBoobMesh.SetBlendShapeWeight(0, 0f);
				_bodySlider_chestSize.minValue = raceDisplayParams._torsoRange.x;
				_chestIcon.sprite = _torsoIcoSprite;
			}
			_bodySlider_armWidth.minValue = raceDisplayParams._armRange.x;
			_bodySlider_armWidth.maxValue = raceDisplayParams._armRange.y;
			_bodySlider_bellySize.minValue = raceDisplayParams._bellyRange.x;
			_bodySlider_bottomWidth.minValue = raceDisplayParams._bottomRange.x;
			if (_displayBoobs)
			{
				_bodySlider_chestSize.maxValue = raceDisplayParams._boobRange.y;
			}
			else
			{
				_bodySlider_chestSize.maxValue = raceDisplayParams._torsoRange.y;
			}
			_bodySlider_bellySize.maxValue = raceDisplayParams._bellyRange.y;
			_bodySlider_bottomWidth.maxValue = raceDisplayParams._bottomRange.y;
		}
	}

	public void Start_RaceModelDisplayRotation_Left()
	{
		_isRotatingDisplay = true;
		_rotationValue = 185.75f;
	}

	public void Start_RaceModelDisplayRotation_Right()
	{
		_isRotatingDisplay = true;
		_rotationValue = -185.75f;
	}

	public void Stop_RaceModelDisplayRotation()
	{
		_isRotatingDisplay = false;
	}

	public void Reset_RaceModelDisplayRotation()
	{
		_raceModelDisplayContainer.rotation = Quaternion.Euler(new Vector3(0f, 25f, 0f));
	}

	public void RandomizeAll()
	{
		Randomize_ColorSliders();
		Randomize_HeadSettings();
		Randomize_BodySliders();
	}

	private void Reset_All()
	{
		Reset_RaceModelDisplayRotation();
		Reset_Colors();
		Reset_Head();
		Reset_Body();
	}

	private IEnumerator Init_ResetParams()
	{
		yield return new WaitForSeconds(0.05f);
		Reset_Colors();
		Reset_Head();
		Reset_Body();
	}

	public void Apply_TextureValue(int _value)
	{
		_setRaceModel.SetTextureIndex -= _value;
	}

	public void Randomize_ColorSliders()
	{
		Randomize_BodyColor();
		Randomize_HairColor();
		Randomize_MiscColor();
		_setRaceModel.SetTextureIndex = UnityEngine.Random.Range(0, _setRaceModel._scriptablePlayerRace._skinTextureGroups.Length);
	}

	private void Randomize_BodyColor()
	{
		_hueSlider.value = UnityEngine.Random.Range(_hueSlider.minValue, _hueSlider.maxValue);
		_brightnessSlider.value = UnityEngine.Random.Range(_brightnessSlider.minValue, _brightnessSlider.maxValue);
		_contrastSlider.value = UnityEngine.Random.Range(_contrastSlider.minValue, _contrastSlider.maxValue);
		_saturationSlider.value = UnityEngine.Random.Range(_saturationSlider.minValue, _saturationSlider.maxValue);
	}

	private void Randomize_HairColor()
	{
		if (_setRaceModel._scriptablePlayerRace._defaultHairColorToBodyValue)
		{
			_hairIsBodyColorToggle.isOn = true;
		}
		else if (UnityEngine.Random.Range(0, 2) == 1)
		{
			_hairIsBodyColorToggle.isOn = true;
		}
		else
		{
			_hairIsBodyColorToggle.isOn = false;
		}
		_hairHueSlider.value = UnityEngine.Random.Range(_hairHueSlider.minValue, _hairHueSlider.maxValue);
		_hairBrightnessSlider.value = UnityEngine.Random.Range(_hairBrightnessSlider.minValue, _hairBrightnessSlider.maxValue);
		_hairContrastSlider.value = UnityEngine.Random.Range(_hairContrastSlider.minValue, _hairContrastSlider.maxValue);
		_hairSaturationSlider.value = UnityEngine.Random.Range(_hairSaturationSlider.minValue, _hairSaturationSlider.maxValue);
	}

	private void Randomize_MiscColor()
	{
		if (_setRaceModel._scriptablePlayerRace._defaultMiscColorValues)
		{
			_miscHueSlider.value = 1f;
			_miscBrightnessSlider.value = 0f;
			_miscContrastSlider.value = 1f;
			_miscSaturationSlider.value = 1f;
		}
		else
		{
			_miscHueSlider.value = UnityEngine.Random.Range(_miscHueSlider.minValue, _miscHueSlider.maxValue);
			_miscBrightnessSlider.value = UnityEngine.Random.Range(_miscBrightnessSlider.minValue, _miscBrightnessSlider.maxValue);
			_miscContrastSlider.value = UnityEngine.Random.Range(_miscContrastSlider.minValue, _miscContrastSlider.maxValue);
			_miscSaturationSlider.value = UnityEngine.Random.Range(_miscSaturationSlider.minValue, _miscSaturationSlider.maxValue);
		}
	}

	public void Reset_Colors()
	{
		PlayerAppearance_Profile appearanceProfile = ProfileDataManager._current._characterFile._appearanceProfile;
		_setRaceModel.SetTextureIndex = appearanceProfile._setTexture;
		_hueSlider.value = appearanceProfile._setSkinColorProfile._hue;
		_brightnessSlider.value = appearanceProfile._setSkinColorProfile._brightness;
		_contrastSlider.value = appearanceProfile._setSkinColorProfile._contrast;
		_saturationSlider.value = appearanceProfile._setSkinColorProfile._saturation;
		_hairHueSlider.value = appearanceProfile._hairColorProfile._hue;
		_hairBrightnessSlider.value = appearanceProfile._hairColorProfile._brightness;
		_hairContrastSlider.value = appearanceProfile._hairColorProfile._contrast;
		_hairSaturationSlider.value = appearanceProfile._hairColorProfile._saturation;
		_hairIsBodyColorToggle.isOn = appearanceProfile._hairIsBodyColor;
		_miscHueSlider.value = appearanceProfile._miscColorProfile._hue;
		_miscBrightnessSlider.value = appearanceProfile._miscColorProfile._brightness;
		_miscContrastSlider.value = appearanceProfile._miscColorProfile._contrast;
		_miscSaturationSlider.value = appearanceProfile._miscColorProfile._saturation;
	}

	public void Apply_EarValue(int _value)
	{
		_setRaceModel.SetEarIndex -= _value;
	}

	public void Apply_EyeValue(int _value)
	{
		_setRaceModel.SetEyeIndex -= _value;
	}

	public void Apply_MouthValue(int _value)
	{
		_setRaceModel.SetMouthIndex -= _value;
	}

	public void Apply_HairValue(int _value)
	{
		_setRaceModel.SetHairIndex -= _value;
	}

	public void Apply_MiscValue(int _value)
	{
		_setRaceModel.SetMiscIndex -= _value;
	}

	public void Reset_Head()
	{
		PlayerAppearance_Profile appearanceProfile = ProfileDataManager._current._characterFile._appearanceProfile;
		_headSlider_headWidth.value = appearanceProfile._headWidth;
		_headSlider_muzzle.value = appearanceProfile._muzzleWeight;
		_voicePitchSlider.value = appearanceProfile._voicePitch;
		_setRaceModel._headWidth = appearanceProfile._headWidth;
		_setRaceModel._muzzleWeight = appearanceProfile._muzzleWeight;
		_setRaceModel.SetHairIndex = appearanceProfile._setHairStyle;
		_setRaceModel.SetEyeIndex = appearanceProfile._setEye;
		_setRaceModel.SetEarIndex = appearanceProfile._setEar;
		_setRaceModel.SetMouthIndex = appearanceProfile._setMouth;
		_setRaceModel.SetMiscIndex = appearanceProfile._setMisc;
	}

	public void Randomize_HeadSettings()
	{
		Randomize_Face();
		Randomize_HairStyle();
		Randomize_Misc();
		_headSlider_headWidth.value = UnityEngine.Random.Range(_headSlider_headWidth.minValue, _headSlider_headWidth.maxValue);
		_headSlider_muzzle.value = UnityEngine.Random.Range(_headSlider_muzzle.minValue, _headSlider_muzzle.maxValue);
		_voicePitchSlider.value = UnityEngine.Random.Range(_voicePitchSlider.minValue, _voicePitchSlider.maxValue);
		_setRaceModel.SetEarIndex = UnityEngine.Random.Range(0, _setRaceModel._earDisplays.Length);
	}

	public void Randomize_HairStyle()
	{
		if (_scriptRace._hairStyleMeshes.Length != 0)
		{
			int setHairIndex = UnityEngine.Random.Range(0, _scriptRace._hairStyleMeshes.Length);
			_setRaceModel.SetHairIndex = setHairIndex;
		}
	}

	public void Randomize_Misc()
	{
		if (_setRaceModel._miscDisplays.Length != 0)
		{
			int setMiscIndex = UnityEngine.Random.Range(0, _setRaceModel._miscDisplays.Length);
			_setRaceModel.SetMiscIndex = setMiscIndex;
		}
		if (_setRaceModel._miscSkinnedDisplays.Length != 0)
		{
			int setMiscIndex2 = UnityEngine.Random.Range(0, _setRaceModel._miscSkinnedDisplays.Length);
			_setRaceModel.SetMiscIndex = setMiscIndex2;
		}
	}

	public void Randomize_Face()
	{
		_setRaceModel.SetEyeIndex = UnityEngine.Random.Range(0, _scriptRace._eyeTextures.Length);
		_setRaceModel.SetMouthIndex = UnityEngine.Random.Range(0, _scriptRace._mouthTextures.Length);
		_setRaceModel.Reset_Expression();
	}

	public void Apply_TailValue(int _value)
	{
		_setRaceModel.SetTailIndex -= _value;
	}

	public void SelectBodyPreset_Male()
	{
		CharacterParamsGroup raceDisplayParams = _setRaceModel._scriptablePlayerRace._raceDisplayParams;
		_displayBoobs = false;
		_bodySlider_height.value = _bodySlider_height.maxValue;
		_bodySlider_armWidth.value = _bodySlider_armWidth.maxValue;
		_bodySlider_bottomWidth.value = _bodySlider_bottomWidth.minValue;
		_bodySlider_chestSize.maxValue = raceDisplayParams._torsoRange.y;
		_bodySlider_chestSize.value = _bodySlider_chestSize.maxValue;
	}

	public void SelectBodyPreset_Female()
	{
		_displayBoobs = true;
		_bodySlider_armWidth.value = 0f;
		_bodySlider_bottomWidth.value = 28.5f;
		_bodySlider_chestSize.value = 12f;
		_bodySlider_height.value = 1f;
	}

	public void Randomize_BodySliders()
	{
		_bodySlider_height.value = UnityEngine.Random.Range(_bodySlider_height.minValue, _bodySlider_height.maxValue);
		_bodySlider_width.value = UnityEngine.Random.Range(_bodySlider_width.minValue, _bodySlider_width.maxValue);
		_bodySlider_armWidth.value = UnityEngine.Random.Range(_bodySlider_armWidth.minValue, _bodySlider_armWidth.maxValue);
		_bodySlider_chestSize.value = UnityEngine.Random.Range(_bodySlider_chestSize.minValue, _bodySlider_chestSize.maxValue);
		_bodySlider_bellySize.value = UnityEngine.Random.Range(_bodySlider_bellySize.minValue, _bodySlider_bellySize.maxValue);
		_bodySlider_bottomWidth.value = UnityEngine.Random.Range(_bodySlider_bottomWidth.minValue, _bodySlider_bottomWidth.maxValue);
		if (UnityEngine.Random.Range(0, 2) == 0)
		{
			ToggleBoobs();
		}
		Apply_TailValue(UnityEngine.Random.Range(0, _setRaceModel._tailDisplays.Length));
	}

	public void ToggleBoobs()
	{
		if (_setRaceModel._baseBoobMesh.enabled)
		{
			_displayBoobs = false;
		}
		else
		{
			_displayBoobs = true;
		}
		_setRaceModel._baseBoobMesh.SetBlendShapeWeight(0, 0f);
		_eD._armorBoobMesh.SetBlendShapeWeight(0, 0f);
		_setRaceModel._baseBodyMesh.SetBlendShapeWeight(2, 0f);
		_eD._armorBodyMesh.SetBlendShapeWeight(2, 0f);
	}

	public void Reset_Body()
	{
		PlayerAppearance_Profile appearanceProfile = ProfileDataManager._current._characterFile._appearanceProfile;
		PlayerRaceModel playerRaceModel = Player._mainPlayer._pVisual._playerRaceModel;
		_bodySlider_height.value = appearanceProfile._heightWeight;
		_bodySlider_width.value = appearanceProfile._widthWeight;
		_bodySlider_armWidth.value = appearanceProfile._armWeight;
		_bodySlider_bellySize.value = playerRaceModel._bellyWeight;
		_bodySlider_bottomWidth.value = playerRaceModel._bottomWeight;
		_displayBoobs = appearanceProfile._displayBoobs;
		if (_displayBoobs)
		{
			_bodySlider_chestSize.value = playerRaceModel._boobWeight;
		}
		else
		{
			_bodySlider_chestSize.value = playerRaceModel._torsoWeight;
		}
		_leftHandedToggle.isOn = appearanceProfile._isLeftHanded;
		_setRaceModel.SetTailIndex = appearanceProfile._setTail;
	}

	public void Send_NewCharacterAppearance()
	{
		if (_transmogCurrencyCount < 1)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Not enough " + GameManager._current._statLogics._transmogCurrency._itemName + "s");
		}
		else
		{
			if (!_applyAppearanceButton.interactable)
			{
				return;
			}
			PlayerInventory pInventory = Player._mainPlayer._pInventory;
			for (int i = 0; i < pInventory._heldItems.Count; i++)
			{
				if (!(pInventory._heldItems[i]._itemName != GameManager._current._statLogics._transmogCurrency._itemName))
				{
					pInventory.Remove_Item(pInventory._heldItems[i], 1);
				}
			}
			float value = _bodySlider_armWidth.value;
			float value2 = _bodySlider_bellySize.value;
			float value3 = _bodySlider_chestSize.value;
			float value4 = _bodySlider_bottomWidth.value;
			float value5 = _headSlider_headWidth.value;
			float value6 = _bodySlider_height.value;
			float value7 = _headSlider_muzzle.value;
			float value8 = _bodySlider_chestSize.value;
			float value9 = _bodySlider_width.value;
			ColorAdjustShader_Profile setSkinColorProfile = new ColorAdjustShader_Profile
			{
				_hue = _setRaceModel._setSkinColorProfile._hue,
				_brightness = _setRaceModel._setSkinColorProfile._brightness,
				_contrast = _setRaceModel._setSkinColorProfile._contrast,
				_saturation = _setRaceModel._setSkinColorProfile._saturation
			};
			ColorAdjustShader_Profile hairColorProfile = new ColorAdjustShader_Profile
			{
				_hue = _setRaceModel._setHairColorProfile._hue,
				_brightness = _setRaceModel._setHairColorProfile._brightness,
				_contrast = _setRaceModel._setHairColorProfile._contrast,
				_saturation = _setRaceModel._setHairColorProfile._saturation
			};
			ColorAdjustShader_Profile miscColorProfile = new ColorAdjustShader_Profile
			{
				_hue = _setRaceModel._setMiscColorProfile._hue,
				_brightness = _setRaceModel._setMiscColorProfile._brightness,
				_contrast = _setRaceModel._setMiscColorProfile._contrast,
				_saturation = _setRaceModel._setMiscColorProfile._saturation
			};
			PlayerAppearance_Profile aP = new PlayerAppearance_Profile
			{
				_setRaceTag = _scriptRace._raceName,
				_armWeight = value,
				_bellyWeight = value2,
				_boobWeight = value3,
				_bottomWeight = value4,
				_displayBoobs = _displayBoobs,
				_hairColorProfile = hairColorProfile,
				_hairIsBodyColor = _setRaceModel._hairHueIsBodyHue,
				_headWidth = value5,
				_heightWeight = value6,
				_helmDyeIndex = _pVisual._playerAppearanceStruct._dyeIndex,
				_hideCape = _pVisual._playerAppearanceStruct._hideCape,
				_hideChest = _pVisual._playerAppearanceStruct._hideChest,
				_hideHelm = _pVisual._playerAppearanceStruct._hideHelm,
				_hideLeggings = _pVisual._playerAppearanceStruct._hideLeggings,
				_miscColorProfile = miscColorProfile,
				_muzzleWeight = value7,
				_setEar = _setRaceModel.SetEarIndex,
				_setEye = _setRaceModel.SetEyeIndex,
				_setHairStyle = _setRaceModel.SetHairIndex,
				_setMisc = _setRaceModel.SetMiscIndex,
				_setMouth = _setRaceModel.SetMouthIndex,
				_setSkinColorProfile = setSkinColorProfile,
				_setTail = _setRaceModel.SetTailIndex,
				_setTexture = _setRaceModel.SetTextureIndex,
				_torsoWeight = value8,
				_voicePitch = _voicePitchSlider.value,
				_widthWeight = value9,
				_isLeftHanded = _leftHandedToggle.isOn,
				_hideVanityHelm = _pVisual._playerAppearanceStruct._hideVanityHelm,
				_hideVanityCape = _pVisual._playerAppearanceStruct._hideVanityCape,
				_hideVanityChest = _pVisual._playerAppearanceStruct._hideVanityChest,
				_hideVanityLeggings = _pVisual._playerAppearanceStruct._hideVanityLeggings
			};
			PlayerAppearance_Profile playerAppearance_Profile = _scriptRace.Init_ParamsCheck(aP);
			PlayerAppearanceStruct newStruct = new PlayerAppearanceStruct
			{
				_setRaceTag = playerAppearance_Profile._setRaceTag,
				_textureID = playerAppearance_Profile._setTexture,
				_bodyColorHue = playerAppearance_Profile._setSkinColorProfile._hue,
				_bodyColorBrightness = playerAppearance_Profile._setSkinColorProfile._brightness,
				_bodyColorContrast = playerAppearance_Profile._setSkinColorProfile._contrast,
				_bodyColorSaturation = playerAppearance_Profile._setSkinColorProfile._saturation,
				_hairStyleHue = playerAppearance_Profile._hairColorProfile._hue,
				_hairStyleBrightness = playerAppearance_Profile._hairColorProfile._brightness,
				_hairStyleContrast = playerAppearance_Profile._hairColorProfile._contrast,
				_hairStyleSaturation = playerAppearance_Profile._hairColorProfile._saturation,
				_hairIsBodyColor = playerAppearance_Profile._hairIsBodyColor,
				_miscHue = playerAppearance_Profile._miscColorProfile._hue,
				_miscBrightness = playerAppearance_Profile._miscColorProfile._brightness,
				_miscContrast = playerAppearance_Profile._miscColorProfile._contrast,
				_miscSaturation = playerAppearance_Profile._miscColorProfile._saturation,
				_dyeIndex = playerAppearance_Profile._helmDyeIndex,
				_headWidth = playerAppearance_Profile._headWidth,
				_muzzleWeight = playerAppearance_Profile._muzzleWeight,
				_hairStyleID = playerAppearance_Profile._setHairStyle,
				_miscID = playerAppearance_Profile._setMisc,
				_mouthID = playerAppearance_Profile._setMouth,
				_earID = playerAppearance_Profile._setEar,
				_eyeID = playerAppearance_Profile._setEye,
				_voicePitch = playerAppearance_Profile._voicePitch,
				_heightWeight = playerAppearance_Profile._heightWeight,
				_widthWeight = playerAppearance_Profile._widthWeight,
				_torsoWeight = playerAppearance_Profile._torsoWeight,
				_displayBoobs = playerAppearance_Profile._displayBoobs,
				_boobWeight = playerAppearance_Profile._boobWeight,
				_armWeight = playerAppearance_Profile._armWeight,
				_bellyWeight = playerAppearance_Profile._bellyWeight,
				_bottomWeight = playerAppearance_Profile._bottomWeight,
				_isLeftHanded = playerAppearance_Profile._isLeftHanded,
				_tailID = playerAppearance_Profile._setTail,
				_hideHelm = playerAppearance_Profile._hideHelm,
				_hideCape = playerAppearance_Profile._hideCape,
				_hideChest = playerAppearance_Profile._hideChest,
				_hideLeggings = playerAppearance_Profile._hideLeggings,
				_hideVanityHelm = playerAppearance_Profile._hideVanityHelm,
				_hideVanityCape = playerAppearance_Profile._hideVanityCape,
				_hideVanityChest = playerAppearance_Profile._hideVanityChest,
				_hideVanityLeggings = playerAppearance_Profile._hideVanityLeggings
			};
			ProfileDataManager._current._characterFile._appearanceProfile = playerAppearance_Profile;
			ProfileDataManager._current.Save_ProfileData();
			_pVisual.Cmd_SendNew_PlayerAppearanceStruct(newStruct);
			SteamAchievementManager.current.Init_EarnAchievement("ATLYSS_ACHIEVEMENT_09");
			DialogManager._current.End_Dialog();
		}
	}
}
