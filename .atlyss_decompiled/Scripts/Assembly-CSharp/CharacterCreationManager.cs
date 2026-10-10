using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationManager : MonoBehaviour
{
	public CharacterSelectManager _charSelectManager;

	[Header("Character Creation Manager")]
	public ScriptablePlayerRace[] _scriptablePlayerRaces = new ScriptablePlayerRace[5];

	public PlayerRaceModel[] _raceDisplayModels = new PlayerRaceModel[5];

	private PlayerRaceModel _currentRaceModel;

	private RaceModelEquipDisplay _eD;

	private Animator _raceModelAnim;

	[Space]
	public ParticleSystem _characterRefreshEffect;

	public AudioSource _characterRefreshSound;

	[SerializeField]
	private InputField _characterNameInputField;

	[SerializeField]
	private Button _createCharacterButton;

	[SerializeField]
	private Button _returnToSelectButton;

	private bool _canCreateCharacter;

	private int _setTemplateNameIndex;

	private int _currentTemplateIndex;

	private int _maxTemplateIndex;

	[Header("Race Selection Tab")]
	[SerializeField]
	private int _currentRaceSelected;

	[SerializeField]
	private Image _raceEmblemIcon;

	[SerializeField]
	private Image[] _raceIcons = new Image[5];

	[Space]
	[SerializeField]
	private Text _raceDescriptionHeader;

	[SerializeField]
	private Text _raceDescriptorField;

	[Space]
	[SerializeField]
	private Image _raceInitialSkillIcon;

	[SerializeField]
	private Text _raceInitialSkillTag;

	[SerializeField]
	private Text _raceInitialSkillDescriptor;

	[Header("Customization Tabs")]
	[SerializeField]
	private MenuElement[] _customizationTabs = new MenuElement[4];

	[SerializeField]
	private RectTransform[] _customizeSelectorButtonRects = new RectTransform[4];

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

	private static readonly int _HUE = Shader.PropertyToID("_Hue");

	private static readonly int _SATURATION = Shader.PropertyToID("_Saturation");

	private static readonly int _BRIGHTNESS = Shader.PropertyToID("_Brightness");

	private static readonly int _CONTRAST = Shader.PropertyToID("_Contrast");

	private static readonly int _TINT = Shader.PropertyToID("_ColorTint");

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
	private Image _chestIcon;

	[SerializeField]
	private Sprite _boobIcoSprite;

	[SerializeField]
	private Sprite _torsoIcoSprite;

	[SerializeField]
	private Toggle _leftHandedToggle;

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

	[SerializeField]
	private Vector3 _cameraHeadPosition = Vector3.zero;

	[Header("Audio")]
	[SerializeField]
	private Slider _voicePitchSlider;

	[SerializeField]
	private AudioSource _aSrcCharacterVoice;

	[Header("Trait Customization")]
	[SerializeField]
	private Text _unspentPointsCounter;

	[SerializeField]
	private Text _strengthCounter;

	[SerializeField]
	private Text _mindCounter;

	[SerializeField]
	private Text _dexterityCounter;

	[SerializeField]
	private Text _vitalityCounter;

	[SerializeField]
	private Button _statResetPointButton;

	[SerializeField]
	private Button _statRollButton;

	[SerializeField]
	private Button[] _addPointButtons;

	[SerializeField]
	private Button[] _subtractPointButtons;

	[SerializeField]
	private Toggle _clothesOffToggle;

	[SerializeField]
	private Image _dyeIcon;

	[SerializeField]
	private Image[] _weaponHighlights;

	[SerializeField]
	private GameObject _pointsAvailDolly;

	private ScriptableArmorDye[] _armorDyes = new ScriptableArmorDye[0];

	private bool _isRollingStats;

	private bool _clothesOff;

	private int _availableAttributePoints;

	private readonly int[] _attributeBaseValues = new int[4];

	private int _setArmorDyeIndex;

	private int _setWeaponIndex;

	[Header("Confirm Randomization Elements")]
	[SerializeField]
	private MenuElement _confirmRandomizeDolly;

	[SerializeField]
	private CanvasGroup _confirmRandomizeGroup;

	[SerializeField]
	private Button _confirmRandomize_confirmButton;

	[SerializeField]
	private Button _confirmRandomize_returnButton;

	private bool _isConfirmingRandomize;

	private bool _confirmRandomizeApplied;

	[Header("Gamepad Elements")]
	[SerializeField]
	private MenuElement _gamepad_navigationHeader;

	[SerializeField]
	private Image _gamepad_navigationLeftIco;

	[SerializeField]
	private Image _gamepad_navigationRightIco;

	[SerializeField]
	private Image _gamepad_confirmRandomizeAllIco;

	[SerializeField]
	private Image _gamepad_returnRandomizeAllIco;

	[SerializeField]
	private Image[] _gamepad_selectionHighlights = new Image[0];

	private int _currentGamepadIndex;

	private int _raceSelectionIndex;

	private int _customizeSelectorIndex;

	[Space]
	[SerializeField]
	private Image _gamepad_createCharacterIco;

	[SerializeField]
	private Image _gamepad_returnToCharSelectIco;

	[Space]
	[SerializeField]
	private Image _gamepad_raceNavIco_00;

	[SerializeField]
	private Image _gamepad_raceNavIco_01;

	[Space]
	[SerializeField]
	private RectTransform _gamepad_customizeSelectorCursor;

	[SerializeField]
	private Image _gamepad_customizeSelector_navIco_00;

	[SerializeField]
	private Image _gamepad_customizeSelector_navIco_01;

	[Space]
	[SerializeField]
	private Image _gamepad_maleTemplateIco;

	[SerializeField]
	private Image _gamepad_femaleTemplateIco;

	[SerializeField]
	private Image _gamepad_randomizeColorIco;

	[SerializeField]
	private Image _gamepad_randomizeHeadIco;

	[SerializeField]
	private Image _gamepad_randomizeBodyIco;

	[SerializeField]
	private Image _gamepad_randomizeStatIco;

	[SerializeField]
	private Image _gamepad_resetStatIco;

	[Space]
	[SerializeField]
	private Image _gamepad_nicknameInputIco;

	[SerializeField]
	private Image _gamepad_randomizeNicknameInputIco;

	[Space]
	[SerializeField]
	private SettingsCell[] _colorSettingCells = new SettingsCell[0];

	private int _selectedColorCell;

	[SerializeField]
	private SettingsCell[] _headSettingCells = new SettingsCell[0];

	private int _selectedHeadCell;

	[SerializeField]
	private SettingsCell[] _bodySettingCells = new SettingsCell[0];

	private int _selectedBodyCell;

	[SerializeField]
	private SettingsCell[] _statSettingCells = new SettingsCell[0];

	private int _selectedStatCell;

	public int SetTemplateNameIndex
	{
		get
		{
			return _setTemplateNameIndex;
		}
		set
		{
			if (value < 0)
			{
				value = GameManager._current._statLogics._defaultNames.Length - 1;
			}
			if (value >= GameManager._current._statLogics._defaultNames.Length)
			{
				value = 0;
			}
			_setTemplateNameIndex = value;
		}
	}

	public int SetArmorDyeIndex
	{
		get
		{
			return _setArmorDyeIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _armorDyes.Length - 1;
			}
			if (value >= _armorDyes.Length)
			{
				value = 0;
			}
			_setArmorDyeIndex = value;
		}
	}

	public int SetWeaponIndex
	{
		get
		{
			return _setWeaponIndex;
		}
		set
		{
			StatLogics statLogics = GameManager._current._statLogics;
			if (value < 0)
			{
				value = statLogics._startingWeapons.Length - 1;
			}
			if (value >= statLogics._startingWeapons.Length)
			{
				value = 0;
			}
			_setWeaponIndex = value;
		}
	}

	public int CurrentGamepadIndex
	{
		get
		{
			return _currentGamepadIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _gamepad_selectionHighlights.Length - 1;
			}
			else if (value > _gamepad_selectionHighlights.Length - 1)
			{
				value = 0;
			}
			_currentGamepadIndex = value;
		}
	}

	public int RaceSelectionIndex
	{
		get
		{
			return _raceSelectionIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _raceDisplayModels.Length - 1;
			}
			if (value > _raceDisplayModels.Length - 1)
			{
				value = 0;
			}
			_raceSelectionIndex = value;
		}
	}

	public int CustomizeSelectorIndex
	{
		get
		{
			return _customizeSelectorIndex;
		}
		set
		{
			if (value < 0)
			{
				value = 5;
			}
			if (value > 5)
			{
				value = 0;
			}
			_customizeSelectorIndex = value;
		}
	}

	public int SelectedColorCell
	{
		get
		{
			return _selectedColorCell;
		}
		set
		{
			if (value < 0)
			{
				value = _colorSettingCells.Length - 1;
			}
			if (value > _colorSettingCells.Length - 1)
			{
				value = 0;
			}
			_selectedColorCell = value;
		}
	}

	public int SelectedHeadCell
	{
		get
		{
			return _selectedHeadCell;
		}
		set
		{
			if (value < 0)
			{
				value = _headSettingCells.Length - 1;
			}
			if (value > _headSettingCells.Length - 1)
			{
				value = 0;
			}
			_selectedHeadCell = value;
		}
	}

	public int SelectedBodyCell
	{
		get
		{
			return _selectedBodyCell;
		}
		set
		{
			if (value < 0)
			{
				value = _bodySettingCells.Length - 1;
			}
			if (value > _bodySettingCells.Length - 1)
			{
				value = 0;
			}
			_selectedBodyCell = value;
		}
	}

	public int SelectedStatCell
	{
		get
		{
			return _selectedStatCell;
		}
		set
		{
			if (value < 0)
			{
				value = _statSettingCells.Length - 1;
			}
			if (value > _statSettingCells.Length - 1)
			{
				value = 0;
			}
			_selectedStatCell = value;
		}
	}

	private void Awake()
	{
		_statResetPointButton.onClick.AddListener(Reset_AttributePoints);
		_statRollButton.onClick.AddListener(Roll_AttributePoints);
		_createCharacterButton.onClick.AddListener(Create_Character);
		_confirmRandomize_confirmButton.onClick.AddListener(Confirm_RandomizeAllConfirmation);
		_confirmRandomize_returnButton.onClick.AddListener(Cancel_RandomizeAllConfirmation);
		Init_CacheSettingsCells();
		void Init_CacheSettingsCells()
		{
			_colorSettingCells = _customizationTabs[0].GetComponentsInChildren<SettingsCell>();
			_headSettingCells = _customizationTabs[1].GetComponentsInChildren<SettingsCell>();
			_bodySettingCells = _customizationTabs[2].GetComponentsInChildren<SettingsCell>();
			_statSettingCells = _customizationTabs[3].GetComponentsInChildren<SettingsCell>();
		}
	}

	private void Update()
	{
		if (MainMenuManager._current._mainMenuCondition != MainMenuCondition.CharacterCreation)
		{
			Cancel_RandomizeAllConfirmation();
			return;
		}
		Handle_MenuControl();
		Handle_CharacterCreationDisplay();
		Handle_InterfaceParameters();
		Handle_AttributeSetParams();
	}

	private void Handle_MenuControl()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Cancel_CharacterCreation();
		}
		Handle_GamepadControl();
		void Cancel_CharacterCreation()
		{
			CurrentGamepadIndex = 0;
			MainMenuManager._current._menuInputBuffer = 0.2f;
			MainMenuManager._current.Set_MenuCondition(1);
		}
		void Handle_GamepadControl()
		{
			bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
			_gamepad_navigationHeader.isEnabled = flag;
			_gamepad_navigationLeftIco.enabled = flag;
			_gamepad_navigationRightIco.enabled = flag;
			_gamepad_confirmRandomizeAllIco.enabled = flag;
			_gamepad_returnRandomizeAllIco.enabled = flag;
			for (int i = 0; i < _gamepad_selectionHighlights.Length; i++)
			{
				if (!flag)
				{
					_gamepad_selectionHighlights[i].enabled = false;
				}
				else
				{
					_gamepad_selectionHighlights[i].enabled = i == _currentGamepadIndex;
				}
			}
			_gamepad_createCharacterIco.enabled = flag && _currentGamepadIndex == 0;
			_gamepad_returnToCharSelectIco.enabled = flag && _currentGamepadIndex == 0;
			_gamepad_raceNavIco_00.enabled = flag && _currentGamepadIndex == 1;
			_gamepad_raceNavIco_01.enabled = flag && _currentGamepadIndex == 1;
			_gamepad_customizeSelectorCursor.gameObject.SetActive(flag && _currentGamepadIndex == 2);
			_gamepad_customizeSelectorCursor.anchoredPosition = _customizeSelectorButtonRects[_customizeSelectorIndex].anchoredPosition;
			_gamepad_customizeSelector_navIco_00.enabled = flag && _currentGamepadIndex == 2;
			_gamepad_customizeSelector_navIco_01.enabled = flag && _currentGamepadIndex == 2;
			_gamepad_nicknameInputIco.enabled = flag && _currentGamepadIndex == 4;
			_gamepad_randomizeNicknameInputIco.enabled = flag && _currentGamepadIndex == 4;
			_gamepad_maleTemplateIco.enabled = flag && _currentCustomizationTab == 2 && _currentGamepadIndex == 3;
			_gamepad_femaleTemplateIco.enabled = flag && _currentCustomizationTab == 2 && _currentGamepadIndex == 3;
			_gamepad_randomizeColorIco.enabled = flag && _currentCustomizationTab == 0 && _currentGamepadIndex == 3;
			_gamepad_randomizeHeadIco.enabled = flag && _currentCustomizationTab == 1 && _currentGamepadIndex == 3;
			_gamepad_randomizeBodyIco.enabled = flag && _currentCustomizationTab == 2 && _currentGamepadIndex == 3;
			_gamepad_randomizeStatIco.enabled = flag && _currentCustomizationTab == 3 && _currentGamepadIndex == 3;
			_gamepad_resetStatIco.enabled = flag && _currentCustomizationTab == 3 && _currentGamepadIndex == 3;
			if (flag)
			{
				bool flag2 = InputControlManager.current.DpadUp(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Up);
				bool flag3 = InputControlManager.current.DpadDown(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Down);
				bool flag4 = InputControlManager.current.DpadLeft(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Left);
				bool flag5 = InputControlManager.current.DpadRight(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Right);
				if (GamepadInput.GetButton(GamepadButton.LeftShoulder))
				{
					_gamepad_navigationLeftIco.color = Color.white;
				}
				else
				{
					_gamepad_navigationLeftIco.color = Color.grey;
				}
				if (GamepadInput.GetButton(GamepadButton.RightShoulder))
				{
					_gamepad_navigationRightIco.color = Color.white;
				}
				else
				{
					_gamepad_navigationRightIco.color = Color.grey;
				}
				if (GamepadInput.GetButtonDown(GamepadButton.LeftShoulder))
				{
					CurrentGamepadIndex--;
					_characterNameInputField.DeactivateInputField();
					MainMenuManager._current._gamepadSelectAsrc.Play();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.RightShoulder))
				{
					CurrentGamepadIndex++;
					_characterNameInputField.DeactivateInputField();
					MainMenuManager._current._gamepadSelectAsrc.Play();
				}
				if (_isConfirmingRandomize)
				{
					if (flag2 | flag3 | flag4 | flag5)
					{
						Cancel_RandomizeAllConfirmation();
					}
					if (GamepadInput.GetButtonDown(GamepadButton.LeftShoulder))
					{
						Cancel_RandomizeAllConfirmation();
					}
					if (GamepadInput.GetButtonDown(GamepadButton.RightShoulder))
					{
						Cancel_RandomizeAllConfirmation();
					}
					if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
					{
						Cancel_RandomizeAllConfirmation();
					}
					if (GamepadInput.GetButtonDown(GamepadButton.Button_South))
					{
						Confirm_RandomizeAllConfirmation();
					}
				}
				if (_currentGamepadIndex != 3)
				{
					SelectedColorCell = 0;
					SelectedHeadCell = 0;
					SelectedBodyCell = 0;
					SelectedStatCell = 0;
				}
				for (int j = 0; j < _colorSettingCells.Length; j++)
				{
					_colorSettingCells[j]._isSelected = j == _selectedColorCell && _currentCustomizationTab == 0 && _currentGamepadIndex == 3;
					_colorSettingCells[j].enabled = _currentCustomizationTab == 0;
				}
				for (int k = 0; k < _headSettingCells.Length; k++)
				{
					_headSettingCells[k]._isSelected = k == _selectedHeadCell && _currentCustomizationTab == 1 && _currentGamepadIndex == 3;
					_headSettingCells[k].enabled = _currentCustomizationTab == 1;
				}
				for (int l = 0; l < _bodySettingCells.Length; l++)
				{
					_bodySettingCells[l]._isSelected = l == _selectedBodyCell && _currentCustomizationTab == 2 && _currentGamepadIndex == 3;
					_bodySettingCells[l].enabled = _currentCustomizationTab == 2;
				}
				for (int m = 0; m < _statSettingCells.Length; m++)
				{
					_statSettingCells[m]._isSelected = m == _selectedStatCell && _currentCustomizationTab == 3 && _currentGamepadIndex == 3;
					_statSettingCells[m].enabled = _currentCustomizationTab == 3;
				}
				switch (_currentGamepadIndex)
				{
				case 0:
					if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
					{
						MainMenuManager._current._menuDeclineAsrc.Play();
						Cancel_CharacterCreation();
					}
					if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && _canCreateCharacter)
					{
						MainMenuManager._current._menuAcceptAsrc.Play();
						Create_Character();
					}
					break;
				case 1:
					if (flag4)
					{
						RaceSelectionIndex--;
						SelectRace(_raceSelectionIndex);
						MainMenuManager._current._gamepadSelectAsrc_01.Play();
					}
					if (flag5)
					{
						RaceSelectionIndex++;
						SelectRace(_raceSelectionIndex);
						MainMenuManager._current._gamepadSelectAsrc_01.Play();
					}
					break;
				case 2:
					if (flag2)
					{
						CustomizeSelectorIndex--;
						MainMenuManager._current._gamepadSelectAsrc_01.Play();
					}
					if (flag3)
					{
						CustomizeSelectorIndex++;
						MainMenuManager._current._gamepadSelectAsrc_01.Play();
					}
					if (GamepadInput.GetButtonDown(GamepadButton.Button_South))
					{
						if (_customizeSelectorIndex < 4)
						{
							SelectCustomizationTab(_customizeSelectorIndex);
						}
						else if (_customizeSelectorIndex == 4)
						{
							_clothesOffToggle.isOn = !_clothesOffToggle.isOn;
						}
						else if (_customizeSelectorIndex == 5)
						{
							RandomizeAll();
						}
						_customizeSelectorButtonRects[_customizeSelectorIndex].GetComponent<ButtonSound>().Play_ClickSound();
					}
					break;
				case 3:
					switch (_currentCustomizationTab)
					{
					case 0:
						if (flag2)
						{
							SelectedColorCell--;
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						if (flag3)
						{
							SelectedColorCell++;
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
						{
							Randomize_ColorSliders();
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						break;
					case 1:
						if (flag2)
						{
							SelectedHeadCell--;
							if (!_headSettingCells[_selectedHeadCell].gameObject.activeInHierarchy)
							{
								SelectedHeadCell--;
							}
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						if (flag3)
						{
							SelectedHeadCell++;
							if (!_headSettingCells[_selectedHeadCell].gameObject.activeInHierarchy)
							{
								SelectedHeadCell++;
							}
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
						{
							Randomize_HeadSettings();
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						break;
					case 2:
						if (flag2)
						{
							SelectedBodyCell--;
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						if (flag3)
						{
							SelectedBodyCell++;
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						if (GamepadInput.GetButtonDown(GamepadButton.Start))
						{
							SelectBodyPreset_Male();
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						if (GamepadInput.GetButtonDown(GamepadButton.Back))
						{
							SelectBodyPreset_Female();
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
						{
							Randomize_BodySliders();
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						break;
					case 3:
						if (flag2)
						{
							SelectedStatCell--;
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						if (flag3)
						{
							SelectedStatCell++;
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
						{
							Roll_AttributePoints();
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						if (GamepadInput.GetButtonDown(GamepadButton.Button_West))
						{
							Reset_AttributePoints();
							MainMenuManager._current._gamepadSelectAsrc_01.Play();
						}
						break;
					}
					break;
				case 4:
					if (GamepadInput.GetButtonDown(GamepadButton.Start))
					{
						_characterNameInputField.ActivateInputField();
						if (SteamManager.Initialized && (SteamUtils.IsSteamRunningOnSteamDeck() || SteamUtils.IsSteamInBigPictureMode()))
						{
							SteamUtils.ShowFloatingGamepadTextInput(EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine, 0, 0, 500, 200);
						}
					}
					if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
					{
						RandomizeCharacterName();
						MainMenuManager._current._gamepadSelectAsrc_01.Play();
					}
					break;
				}
			}
		}
	}

	private void Handle_InterfaceParameters()
	{
		_canCreateCharacter = !string.IsNullOrWhiteSpace(_characterNameInputField.text) && !_isRollingStats && _availableAttributePoints <= 0;
		_createCharacterButton.interactable = _canCreateCharacter;
		_pointsAvailDolly.SetActive(_availableAttributePoints > 0);
		for (int i = 0; i < _customizationTabs.Length; i++)
		{
			_customizationTabs[i].isEnabled = _currentCustomizationTab == i;
		}
		for (int j = 0; j < _weaponHighlights.Length; j++)
		{
			_weaponHighlights[j].enabled = _setWeaponIndex == j;
		}
		if ((bool)_currentRaceModel)
		{
			_cameraHeadPosition = _currentRaceModel._scriptablePlayerRace._headFocusPosition;
		}
		if (_currentCustomizationTab == 1)
		{
			_charSelectManager._mainCamTransform.localPosition = Vector3.Lerp(_charSelectManager._mainCamTransform.localPosition, _cameraHeadPosition, 8.5f * Time.deltaTime);
		}
		else
		{
			_charSelectManager._mainCamTransform.localPosition = Vector3.Lerp(_charSelectManager._mainCamTransform.localPosition, _charSelectManager._defaultCameraPosition, 8.5f * Time.deltaTime);
		}
		_dyeIcon.sprite = GameManager._current._statLogics._armorDyes[_setArmorDyeIndex]._itemIcon;
		_confirmRandomizeDolly.isEnabled = _isConfirmingRandomize;
		_confirmRandomizeGroup.interactable = _isConfirmingRandomize;
		Handle_SelectedRaceInterface();
		void Handle_SelectedRaceInterface()
		{
			ScriptablePlayerRace scriptablePlayerRace = _scriptablePlayerRaces[_currentRaceSelected];
			Image[] raceIcons = _raceIcons;
			for (int k = 0; k < raceIcons.Length; k++)
			{
				raceIcons[k].color = Color.grey;
			}
			_raceIcons[_currentRaceSelected].color = Color.white;
			_raceEmblemIcon.sprite = scriptablePlayerRace._raceEmblemIcon;
			_raceDescriptionHeader.text = scriptablePlayerRace._raceName ?? "";
			_raceDescriptorField.text = scriptablePlayerRace._raceDescription ?? "";
			_colorMiscTag.text = scriptablePlayerRace._miscName ?? "";
			_miscTag.text = scriptablePlayerRace._miscName ?? "";
			_earSelectorObj.SetActive(!scriptablePlayerRace._disableEarSelector);
			_mouthSelectorObj.SetActive(!scriptablePlayerRace._disableMouthSelector);
			_bodyHueSliderImage.sprite = scriptablePlayerRace._raceDisplayParams._bodyHueSliderSprite;
			_hairHueSliderImage.sprite = scriptablePlayerRace._raceDisplayParams._bodyHueSliderSprite;
			_raceInitialSkillIcon.sprite = scriptablePlayerRace._racialSkill._skillIcon;
			_raceInitialSkillTag.text = scriptablePlayerRace._racialSkill._skillName ?? "";
			_raceInitialSkillDescriptor.text = scriptablePlayerRace._racialSkill._skillDescription ?? "";
		}
	}

	public void SelectCustomizationTab(int _i)
	{
		Cancel_RandomizeAllConfirmation();
		_currentCustomizationTab = _i;
		if (_currentRaceModel == null)
		{
			return;
		}
		if (_i == 3)
		{
			Apply_WeaponValue(_setWeaponIndex);
		}
		else
		{
			if (_currentRaceModel._raceAnimator.isActiveAndEnabled)
			{
				_currentRaceModel._raceAnimator.CrossFade("Hold_None", 0.1f, 2);
			}
			_eD.Remove_WeaponDisplay();
		}
		_currentRaceModel._canAltIdleAnim = false;
	}

	public void ClearCharacterNameInputField()
	{
		_characterNameInputField.text = string.Empty;
	}

	public void RandomizeCharacterName()
	{
		Cancel_RandomizeAllConfirmation();
		_characterNameInputField.text = GameManager._current._statLogics._defaultNames[SetTemplateNameIndex] ?? "";
		SetTemplateNameIndex++;
	}

	public void SelectRace(int _i)
	{
		Cancel_RandomizeAllConfirmation();
		_currentRaceSelected = _i;
		_raceSelectionIndex = _i;
		PlayerRaceModel[] raceDisplayModels = _raceDisplayModels;
		for (int i = 0; i < raceDisplayModels.Length; i++)
		{
			raceDisplayModels[i].gameObject.SetActive(value: false);
		}
		_raceDisplayModels[_currentRaceSelected].gameObject.SetActive(value: true);
		_currentRaceModel = _raceDisplayModels[_currentRaceSelected];
		_raceModelAnim = _currentRaceModel.GetComponentInChildren<Animator>();
		_eD = _currentRaceModel.GetComponent<RaceModelEquipDisplay>();
		_eD.Remove_HelmDisplay();
		_eD.Remove_ChestPieceDisplay();
		_eD.Remove_LeggingsDisplay();
		_eD.Remove_WeaponDisplay();
		_eD.Remove_ShieldDisplay();
		_eD.Remove_CapeDisplay();
		_eD.DyeIndex = 0;
		_currentRaceModel.Reset_Expression();
		if (_currentCustomizationTab == 3)
		{
			Apply_WeaponValue(_setWeaponIndex);
		}
		_setTemplateNameIndex = Random.Range(0, GameManager._current._statLogics._defaultNames.Length - 1);
		_characterRefreshEffect.Play();
		_characterRefreshSound.Stop();
		_characterRefreshSound.Play();
		ApplyRaceTemplate();
	}

	public void ApplyRaceTemplate()
	{
		PlayerAppearance_Profile appearanceProfile = ProfileDataManager._current._characterFile._appearanceProfile;
		ScriptablePlayerRace scriptablePlayerRace = _scriptablePlayerRaces[_currentRaceSelected];
		if (scriptablePlayerRace._appearanceTemplates != null && scriptablePlayerRace._appearanceTemplates.Length != 0)
		{
			_maxTemplateIndex = scriptablePlayerRace._appearanceTemplates.Length - 1;
			if (_currentTemplateIndex > _maxTemplateIndex)
			{
				_currentTemplateIndex = 0;
			}
			PlayerAppearanceStruct playerAppearanceStruct = scriptablePlayerRace._appearanceTemplates[_currentTemplateIndex];
			_currentTemplateIndex++;
			_currentRaceModel.SetEyeIndex = playerAppearanceStruct._eyeID;
			_currentRaceModel.SetEarIndex = playerAppearanceStruct._earID;
			_currentRaceModel.SetHairIndex = playerAppearanceStruct._hairStyleID;
			_currentRaceModel.SetMiscIndex = playerAppearanceStruct._miscID;
			_currentRaceModel.SetMouthIndex = playerAppearanceStruct._mouthID;
			_currentRaceModel.SetTailIndex = playerAppearanceStruct._tailID;
			_currentRaceModel.SetTextureIndex = playerAppearanceStruct._textureID;
			Apply_ArmorDyeValue(playerAppearanceStruct._dyeIndex);
			_hueSlider.value = playerAppearanceStruct._bodyColorHue;
			_brightnessSlider.value = playerAppearanceStruct._bodyColorBrightness;
			_contrastSlider.value = playerAppearanceStruct._bodyColorContrast;
			_saturationSlider.value = playerAppearanceStruct._bodyColorSaturation;
			_hairIsBodyColorToggle.isOn = playerAppearanceStruct._hairIsBodyColor;
			_hairHueSlider.value = playerAppearanceStruct._hairStyleHue;
			_hairBrightnessSlider.value = playerAppearanceStruct._hairStyleBrightness;
			_hairContrastSlider.value = playerAppearanceStruct._hairStyleContrast;
			_hairSaturationSlider.value = playerAppearanceStruct._hairStyleSaturation;
			_miscHueSlider.value = playerAppearanceStruct._miscHue;
			_miscBrightnessSlider.value = playerAppearanceStruct._miscBrightness;
			_miscContrastSlider.value = playerAppearanceStruct._miscContrast;
			_miscSaturationSlider.value = playerAppearanceStruct._miscSaturation;
			_headSlider_headWidth.value = playerAppearanceStruct._headWidth;
			_headSlider_muzzle.value = playerAppearanceStruct._muzzleWeight;
			_voicePitchSlider.value = playerAppearanceStruct._voicePitch;
			_bodySlider_height.value = playerAppearanceStruct._heightWeight;
			_bodySlider_width.value = playerAppearanceStruct._widthWeight;
			_bodySlider_armWidth.value = playerAppearanceStruct._armWeight;
			_bodySlider_bellySize.value = playerAppearanceStruct._bellyWeight;
			_bodySlider_bottomWidth.value = playerAppearanceStruct._bottomWeight;
			appearanceProfile._displayBoobs = playerAppearanceStruct._displayBoobs;
			if (playerAppearanceStruct._displayBoobs)
			{
				_bodySlider_chestSize.value = playerAppearanceStruct._boobWeight;
			}
			else
			{
				_bodySlider_chestSize.value = playerAppearanceStruct._torsoWeight;
			}
		}
	}

	private void Cancel_RandomizeAllConfirmation()
	{
		if (_isConfirmingRandomize)
		{
			MainMenuManager._current._menuDeclineAsrc.Play();
			_isConfirmingRandomize = false;
		}
	}

	private void Confirm_RandomizeAllConfirmation()
	{
		MainMenuManager._current._menuAcceptAsrc.Play();
		_confirmRandomizeApplied = true;
		_isConfirmingRandomize = false;
		RandomizeAll();
	}

	public void RandomizeAll()
	{
		StatLogics statLogics = GameManager._current._statLogics;
		if (!_confirmRandomizeApplied)
		{
			MainMenuManager._current._menuAcceptAsrc.Play();
			_isConfirmingRandomize = true;
			return;
		}
		Randomize_ColorSliders();
		Randomize_HeadSettings();
		Apply_TailValue(Random.Range(0, _currentRaceModel._tailDisplays.Length));
		_leftHandedToggle.isOn = false;
		switch (Random.Range(0, 2))
		{
		case 0:
			SelectBodyPreset_Male();
			break;
		case 1:
			SelectBodyPreset_Female();
			break;
		}
		Apply_ArmorDyeValue(Random.Range(0, GameManager._current._statLogics._armorDyes.Length));
		Apply_WeaponValue(Random.Range(0, statLogics._startingWeapons.Length));
		Roll_AttributePoints();
		if (_currentCustomizationTab != 3)
		{
			if (_currentRaceModel._raceAnimator.isActiveAndEnabled)
			{
				_currentRaceModel._raceAnimator.CrossFade("Hold_None", 0.1f, 2);
			}
			_eD.Remove_WeaponDisplay();
			_eD.Remove_ShieldDisplay();
		}
	}

	public void Apply_TextureValue(int _value)
	{
		_currentRaceModel.SetTextureIndex += _value;
	}

	public void Randomize_ColorSliders()
	{
		Cancel_RandomizeAllConfirmation();
		Randomize_BodyColor();
		Randomize_HairColor();
		Randomize_MiscColor();
		_currentRaceModel.SetTextureIndex = Random.Range(0, _currentRaceModel._scriptablePlayerRace._skinTextureGroups.Length);
	}

	private void Randomize_BodyColor()
	{
		Cancel_RandomizeAllConfirmation();
		_hueSlider.value = Random.Range(_hueSlider.minValue, _hueSlider.maxValue);
		_brightnessSlider.value = Random.Range(_brightnessSlider.minValue, _brightnessSlider.maxValue);
		_contrastSlider.value = Random.Range(_contrastSlider.minValue, _contrastSlider.maxValue);
		_saturationSlider.value = Random.Range(_saturationSlider.minValue, _saturationSlider.maxValue);
	}

	private void Randomize_HairColor()
	{
		Cancel_RandomizeAllConfirmation();
		if (_currentRaceModel._scriptablePlayerRace._defaultHairColorToBodyValue)
		{
			_hairIsBodyColorToggle.isOn = true;
		}
		else if (Random.Range(0, 2) == 1)
		{
			_hairIsBodyColorToggle.isOn = true;
		}
		else
		{
			_hairIsBodyColorToggle.isOn = false;
		}
		_hairHueSlider.value = Random.Range(_hairHueSlider.minValue, _hairHueSlider.maxValue);
		_hairBrightnessSlider.value = Random.Range(_hairBrightnessSlider.minValue, _hairBrightnessSlider.maxValue);
		_hairContrastSlider.value = Random.Range(_hairContrastSlider.minValue, _hairContrastSlider.maxValue);
		_hairSaturationSlider.value = Random.Range(_hairSaturationSlider.minValue, _hairSaturationSlider.maxValue);
	}

	private void Randomize_MiscColor()
	{
		Cancel_RandomizeAllConfirmation();
		if (_currentRaceModel._scriptablePlayerRace._defaultMiscColorValues)
		{
			_miscHueSlider.value = 1f;
			_miscBrightnessSlider.value = 0f;
			_miscContrastSlider.value = 1f;
			_miscSaturationSlider.value = 1f;
		}
		else
		{
			_miscHueSlider.value = Random.Range(_miscHueSlider.minValue, _miscHueSlider.maxValue);
			_miscBrightnessSlider.value = Random.Range(_miscBrightnessSlider.minValue, _miscBrightnessSlider.maxValue);
			_miscContrastSlider.value = Random.Range(_miscContrastSlider.minValue, _miscContrastSlider.maxValue);
			_miscSaturationSlider.value = Random.Range(_miscSaturationSlider.minValue, _miscSaturationSlider.maxValue);
		}
	}

	public void Apply_EarValue(int _value)
	{
		_currentRaceModel.SetEarIndex += _value;
	}

	public void Apply_EyeValue(int _value)
	{
		_currentRaceModel.SetEyeIndex += _value;
	}

	public void Apply_MouthValue(int _value)
	{
		_currentRaceModel.SetMouthIndex += _value;
	}

	public void Apply_HairValue(int _value)
	{
		_currentRaceModel.SetHairIndex += _value;
	}

	public void Apply_MiscValue(int _value)
	{
		_currentRaceModel.SetMiscIndex += _value;
	}

	public void Randomize_HeadSettings()
	{
		Randomize_HairStyle();
		Randomize_Face();
		Randomize_Misc();
		_headSlider_headWidth.value = Random.Range(_headSlider_headWidth.minValue, _headSlider_headWidth.maxValue);
		_headSlider_muzzle.value = Random.Range(_headSlider_muzzle.minValue, _headSlider_muzzle.maxValue);
		_voicePitchSlider.value = Random.Range(_voicePitchSlider.minValue, _voicePitchSlider.maxValue);
		_currentRaceModel.SetEarIndex = Random.Range(0, _currentRaceModel._earDisplays.Length);
	}

	public void Randomize_HairStyle()
	{
		Cancel_RandomizeAllConfirmation();
		if (_scriptablePlayerRaces[_currentRaceSelected]._hairStyleMeshes.Length != 0)
		{
			int setHairIndex = Random.Range(0, _scriptablePlayerRaces[_currentRaceSelected]._hairStyleMeshes.Length);
			_currentRaceModel.SetHairIndex = setHairIndex;
		}
	}

	public void Randomize_Face()
	{
		Cancel_RandomizeAllConfirmation();
		_currentRaceModel.SetEyeIndex = Random.Range(0, _scriptablePlayerRaces[_currentRaceSelected]._eyeTextures.Length);
		_currentRaceModel.SetMouthIndex = Random.Range(0, _scriptablePlayerRaces[_currentRaceSelected]._mouthTextures.Length);
		_currentRaceModel.Reset_Expression();
	}

	public void Randomize_Misc()
	{
		Cancel_RandomizeAllConfirmation();
		if (_currentRaceModel._miscDisplays.Length != 0)
		{
			int setMiscIndex = Random.Range(0, _currentRaceModel._miscDisplays.Length);
			_currentRaceModel.SetMiscIndex = setMiscIndex;
		}
		if (_currentRaceModel._miscSkinnedDisplays.Length != 0)
		{
			int setMiscIndex2 = Random.Range(0, _currentRaceModel._miscSkinnedDisplays.Length);
			_currentRaceModel.SetMiscIndex = setMiscIndex2;
		}
	}

	public void Apply_TailValue(int _value)
	{
		_currentRaceModel.SetTailIndex += _value;
	}

	public void SelectBodyPreset_Male()
	{
		Cancel_RandomizeAllConfirmation();
		PlayerAppearance_Profile appearanceProfile = ProfileDataManager._current._characterFile._appearanceProfile;
		CharacterParamsGroup raceDisplayParams = _currentRaceModel._scriptablePlayerRace._raceDisplayParams;
		_bodySlider_height.value = _bodySlider_height.maxValue;
		_bodySlider_armWidth.value = _bodySlider_armWidth.maxValue;
		_bodySlider_bottomWidth.value = _bodySlider_bottomWidth.minValue;
		appearanceProfile._displayBoobs = false;
		_bodySlider_chestSize.maxValue = raceDisplayParams._torsoRange.y;
		_bodySlider_chestSize.value = _bodySlider_chestSize.maxValue;
		_bodySlider_bellySize.value = 0f;
	}

	public void SelectBodyPreset_Female()
	{
		Cancel_RandomizeAllConfirmation();
		PlayerAppearance_Profile appearanceProfile = ProfileDataManager._current._characterFile._appearanceProfile;
		_bodySlider_armWidth.value = 0f;
		_bodySlider_bottomWidth.value = 28.5f;
		_bodySlider_chestSize.value = 12f;
		_bodySlider_height.value = 1f;
		appearanceProfile._displayBoobs = true;
		_bodySlider_bellySize.value = 15.5f;
	}

	public void ToggleBoobs()
	{
		Cancel_RandomizeAllConfirmation();
		PlayerAppearance_Profile appearanceProfile = ProfileDataManager._current._characterFile._appearanceProfile;
		if (_currentRaceModel._baseBoobMesh.enabled)
		{
			appearanceProfile._displayBoobs = false;
		}
		else
		{
			appearanceProfile._displayBoobs = true;
		}
		_currentRaceModel._baseBoobMesh.SetBlendShapeWeight(0, 0f);
		_eD._armorBoobMesh.SetBlendShapeWeight(0, 0f);
		_currentRaceModel._baseBodyMesh.SetBlendShapeWeight(2, 0f);
		_eD._armorBodyMesh.SetBlendShapeWeight(2, 0f);
	}

	public void Randomize_BodySliders()
	{
		Cancel_RandomizeAllConfirmation();
		_bodySlider_height.value = Random.Range(_bodySlider_height.minValue, _bodySlider_height.maxValue);
		_bodySlider_width.value = Random.Range(_bodySlider_width.minValue, _bodySlider_width.maxValue);
		_bodySlider_armWidth.value = Random.Range(_bodySlider_armWidth.minValue, _bodySlider_armWidth.maxValue);
		_bodySlider_chestSize.value = Random.Range(_bodySlider_chestSize.minValue, _bodySlider_chestSize.maxValue);
		_bodySlider_bellySize.value = Random.Range(_bodySlider_bellySize.minValue, _bodySlider_bellySize.maxValue);
		_bodySlider_bottomWidth.value = Random.Range(_bodySlider_bottomWidth.minValue, _bodySlider_bottomWidth.maxValue);
		if (Random.Range(0, 2) == 0)
		{
			ToggleBoobs();
		}
	}

	public void Add_PointToAttribute(int _index)
	{
		if (_availableAttributePoints > 0)
		{
			_attributeBaseValues[_index]++;
			_availableAttributePoints--;
		}
	}

	public void Subtract_PointToAttribute(int _index)
	{
		if (_availableAttributePoints <= GameManager._current._statLogics._attributePointsEachLevel * 2)
		{
			_attributeBaseValues[_index]--;
			_availableAttributePoints++;
		}
	}

	private void Stop_StatRoll()
	{
		_isRollingStats = false;
	}

	public void Roll_AttributePoints()
	{
		if (!_isRollingStats)
		{
			Reset_AttributePoints();
			_isRollingStats = true;
		}
	}

	public void Reset_AttributePoints()
	{
		Cancel_RandomizeAllConfirmation();
		_availableAttributePoints = GameManager._current._statLogics._attributePointsEachLevel * 2;
		for (int i = 0; i < _attributeBaseValues.Length; i++)
		{
			_attributeBaseValues[i] = 1;
		}
	}

	private void Handle_AttributeSetParams()
	{
		for (int i = 0; i < _attributeBaseValues.Length; i++)
		{
			_subtractPointButtons[i].interactable = _attributeBaseValues[i] > 1;
			_addPointButtons[i].interactable = _availableAttributePoints > 0;
		}
		if (_isRollingStats)
		{
			for (int j = 0; j < _attributeBaseValues.Length; j++)
			{
				if (_availableAttributePoints <= 0)
				{
					break;
				}
				if (Random.Range(0, 2) != 0)
				{
					_attributeBaseValues[j]++;
					_availableAttributePoints--;
				}
			}
			if (_availableAttributePoints <= 0 && !IsInvoking("Stop_StatRoll"))
			{
				Invoke("Stop_StatRoll", 0.12f);
			}
		}
		_unspentPointsCounter.text = $"{_availableAttributePoints}";
		_strengthCounter.text = $"{_attributeBaseValues[0]}";
		_mindCounter.text = $"{_attributeBaseValues[1]}";
		_dexterityCounter.text = $"{_attributeBaseValues[2]}";
		_vitalityCounter.text = $"{_attributeBaseValues[3]}";
		_statRollButton.interactable = !_isRollingStats;
	}

	public void Next_WeaponValue()
	{
		Apply_WeaponValue(_setWeaponIndex + 1);
	}

	public void Previous_WeaponValue()
	{
		Apply_WeaponValue(_setWeaponIndex - 1);
	}

	public void Apply_WeaponValue(int _value)
	{
		Cancel_RandomizeAllConfirmation();
		StatLogics statLogics = GameManager._current._statLogics;
		SetWeaponIndex = _value;
		if (!(statLogics._startingWeapons[_setWeaponIndex] == null) && _currentCustomizationTab == 3)
		{
			_eD.Apply_WeaponDisplay(statLogics._startingWeapons[_setWeaponIndex], 0);
		}
	}

	public void Apply_Chestpiece()
	{
		if (_currentRaceModel._scriptablePlayerRace._startingChestpiece != null)
		{
			_eD.Apply_ArmorDisplay(_currentRaceModel._scriptablePlayerRace._startingChestpiece);
		}
		else
		{
			_eD.Remove_ChestPieceDisplay();
		}
	}

	public void Apply_Leggings()
	{
		if (_currentRaceModel._scriptablePlayerRace._startingLeggings != null)
		{
			_eD.Apply_ArmorDisplay(_currentRaceModel._scriptablePlayerRace._startingLeggings);
		}
		else
		{
			_eD.Remove_LeggingsDisplay();
		}
	}

	public void Apply_ArmorDyeValue(int _value)
	{
		Cancel_RandomizeAllConfirmation();
		_armorDyes = GameManager._current._statLogics._armorDyes;
		SetArmorDyeIndex += _value;
		_eD.DyeIndex = _setArmorDyeIndex;
		_clothesOffToggle.isOn = false;
		_eD.Apply_ArmorDisplay(_currentRaceModel._scriptablePlayerRace._startingChestpiece);
		_eD.Apply_ArmorDisplay(_currentRaceModel._scriptablePlayerRace._startingLeggings);
	}

	private void Handle_CharacterCreationDisplay()
	{
		ProfileDataManager current = ProfileDataManager._current;
		PlayerAppearance_Profile _pap = current._characterFile._appearanceProfile;
		_currentRaceModel = _raceDisplayModels[_currentRaceSelected];
		_eD = _currentRaceModel.GetComponent<RaceModelEquipDisplay>();
		_currentRaceModel._isCharacterCreationDisplay = true;
		_currentRaceModel.Apply_CharacterDisplay();
		_currentRaceModel._armatureTransform.localScale = new Vector3(_bodySlider_width.value, _bodySlider_height.value, _bodySlider_width.value);
		_currentRaceModel._displayBoobs = _pap._displayBoobs;
		_currentRaceModel._headBoneTransform.localScale = new Vector3(_headSlider_headWidth.value, 1f, 1f);
		_currentRaceModel._assDynamicBones[0].enabled = _currentRaceModel._baseBodyMesh.GetBlendShapeWeight(1) >= 45f;
		_currentRaceModel._assDynamicBones[1].enabled = _currentRaceModel._baseBodyMesh.GetBlendShapeWeight(1) >= 45f;
		_currentRaceModel._bellyDynamicBone[0].enabled = _currentRaceModel._baseBodyMesh.GetBlendShapeWeight(0) >= 45f;
		_eD._robeSkirtMeshRender.SetBlendShapeWeight(0, _bodySlider_bottomWidth.value);
		_eD._robeSkirtMeshRender.SetBlendShapeWeight(1, _bodySlider_bellySize.value);
		Init_SliderParams();
		_currentRaceModel._faceMesh.SetBlendShapeWeight(1, _headSlider_muzzle.value);
		_currentRaceModel._baseBodyMesh.SetBlendShapeWeight(0, _bodySlider_bellySize.value);
		_currentRaceModel._baseBodyMesh.SetBlendShapeWeight(1, _bodySlider_bottomWidth.value);
		_currentRaceModel._baseBodyMesh.SetBlendShapeWeight(3, _headSlider_muzzle.value);
		_eD._armorBodyMesh.SetBlendShapeWeight(0, _bodySlider_bellySize.value);
		_eD._armorBodyMesh.SetBlendShapeWeight(1, _bodySlider_bottomWidth.value);
		_eD._armorBodyMesh.SetBlendShapeWeight(3, _headSlider_muzzle.value);
		_eD._chestPieceLegRender.SetBlendShapeWeight(0, _bodySlider_bellySize.value);
		_eD._chestPieceLegRender.SetBlendShapeWeight(1, _bodySlider_bottomWidth.value);
		_currentRaceModel._baseArmMesh.SetBlendShapeWeight(0, _bodySlider_armWidth.value);
		_eD._armorArmMesh.SetBlendShapeWeight(0, _bodySlider_armWidth.value);
		int index = 4;
		if (_eD._hasHeadBlendWeights)
		{
			index = 5;
		}
		if (_bodySlider_bottomWidth.value < 0f)
		{
			_currentRaceModel._baseBodyMesh.SetBlendShapeWeight(index, _bodySlider_bottomWidth.value * -1f);
			_currentRaceModel._baseBodyMesh.SetBlendShapeWeight(1, 0f);
			_eD._armorBodyMesh.SetBlendShapeWeight(1, 0f);
			_eD._chestPieceLegRender.SetBlendShapeWeight(1, 0f);
			_eD._robeSkirtMeshRender.SetBlendShapeWeight(0, 0f);
			_eD._armorBodyMesh.SetBlendShapeWeight(index, _bodySlider_bottomWidth.value * -1f);
			_eD._chestPieceLegRender.SetBlendShapeWeight(index, _bodySlider_bottomWidth.value * -1f);
			_eD._robeSkirtMeshRender.SetBlendShapeWeight(2, _bodySlider_bottomWidth.value * -1f);
			if (_currentRaceModel._scriptablePlayerRace._feetUseThighWeights)
			{
				_currentRaceModel._baseFeetMesh.SetBlendShapeWeight(0, _bodySlider_bottomWidth.value * -1f);
				_eD._armorFeetMesh.SetBlendShapeWeight(0, _bodySlider_bottomWidth.value * -1f);
			}
		}
		else
		{
			_currentRaceModel._baseBodyMesh.SetBlendShapeWeight(index, 0f);
			_eD._armorBodyMesh.SetBlendShapeWeight(index, 0f);
			_eD._chestPieceLegRender.SetBlendShapeWeight(index, 0f);
			_eD._robeSkirtMeshRender.SetBlendShapeWeight(2, 0f);
			if (_currentRaceModel._scriptablePlayerRace._feetUseThighWeights)
			{
				_currentRaceModel._baseFeetMesh.SetBlendShapeWeight(0, 0f);
				_eD._armorFeetMesh.SetBlendShapeWeight(0, 0f);
			}
		}
		if (_leftHandedToggle.isOn)
		{
			_currentRaceModel.transform.parent.localScale = new Vector3(-1f, 1f, 1f);
		}
		else
		{
			_currentRaceModel.transform.parent.localScale = Vector3.one;
		}
		_currentRaceModel._setSkinColorProfile._hue = _hueSlider.value;
		_currentRaceModel._setSkinColorProfile._brightness = _brightnessSlider.value;
		_currentRaceModel._setSkinColorProfile._contrast = _contrastSlider.value;
		_currentRaceModel._setSkinColorProfile._saturation = _saturationSlider.value;
		_currentRaceModel._setHairColorProfile._hue = _hairHueSlider.value;
		_currentRaceModel._setHairColorProfile._brightness = _hairBrightnessSlider.value;
		_currentRaceModel._setHairColorProfile._contrast = _hairContrastSlider.value;
		_currentRaceModel._setHairColorProfile._saturation = _hairSaturationSlider.value;
		_currentRaceModel._hairHueIsBodyHue = _hairIsBodyColorToggle.isOn;
		if (!_currentRaceModel._hairHueIsBodyHue)
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
		_currentRaceModel._setMiscColorProfile._hue = _miscHueSlider.value;
		_currentRaceModel._setMiscColorProfile._brightness = _miscBrightnessSlider.value;
		_currentRaceModel._setMiscColorProfile._contrast = _miscContrastSlider.value;
		_currentRaceModel._setMiscColorProfile._saturation = _miscSaturationSlider.value;
		_hairHueSlider.interactable = !_currentRaceModel._hairHueIsBodyHue;
		_hairBrightnessSlider.interactable = !_currentRaceModel._hairHueIsBodyHue;
		_hairContrastSlider.interactable = !_currentRaceModel._hairHueIsBodyHue;
		_hairSaturationSlider.interactable = !_currentRaceModel._hairHueIsBodyHue;
		_clothesOff = _clothesOffToggle.isOn;
		if (_clothesOff)
		{
			_eD.Remove_ChestPieceDisplay();
			_eD.Remove_LeggingsDisplay();
		}
		else
		{
			Apply_Chestpiece();
			Apply_Leggings();
		}
		void Init_SliderParams()
		{
			CharacterParamsGroup raceDisplayParams = _currentRaceModel._scriptablePlayerRace._raceDisplayParams;
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
			if (_currentRaceModel._displayBoobs)
			{
				_currentRaceModel._baseBoobMesh.SetBlendShapeWeight(0, _bodySlider_chestSize.value);
				_eD._armorBoobMesh.SetBlendShapeWeight(0, _bodySlider_chestSize.value);
				_currentRaceModel._baseBodyMesh.SetBlendShapeWeight(2, 0f);
				_eD._armorBodyMesh.SetBlendShapeWeight(2, 0f);
				_bodySlider_chestSize.minValue = raceDisplayParams._boobRange.x;
				_bodySlider_chestSize.maxValue = raceDisplayParams._boobRange.y;
				_chestIcon.sprite = _boobIcoSprite;
			}
			else
			{
				_currentRaceModel._baseBodyMesh.SetBlendShapeWeight(2, _bodySlider_chestSize.value);
				_eD._armorBodyMesh.SetBlendShapeWeight(2, _bodySlider_chestSize.value);
				_currentRaceModel._baseBoobMesh.SetBlendShapeWeight(0, 0f);
				_eD._armorBoobMesh.SetBlendShapeWeight(0, 0f);
				_bodySlider_chestSize.minValue = raceDisplayParams._torsoRange.x;
				_chestIcon.sprite = _torsoIcoSprite;
			}
			_bodySlider_armWidth.minValue = raceDisplayParams._armRange.x;
			_bodySlider_armWidth.maxValue = raceDisplayParams._armRange.y;
			_bodySlider_bellySize.minValue = raceDisplayParams._bellyRange.x;
			_bodySlider_bottomWidth.minValue = raceDisplayParams._bottomRange.x;
			if (_pap._displayBoobs)
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

	public void BoobSurpriseEffect()
	{
		_raceModelAnim.CrossFade("cc_boobSurprise", 0f, 5);
	}

	public void OnBoobSliderHold()
	{
		_raceModelAnim.SetBool("_cc_isModifyingBoob", value: true);
	}

	public void OnBoobSliderRelease()
	{
		_raceModelAnim.SetBool("_cc_isModifyingBoob", value: false);
	}

	public void BottomSurpriseEffect()
	{
		_raceModelAnim.CrossFade("cc_bottomSurprise", 0.1f, 5);
	}

	public void OnBottomSliderHold()
	{
		_raceModelAnim.SetBool("_cc_isModifyingBottom", value: true);
	}

	public void OnBottomSliderRelease()
	{
		_raceModelAnim.SetBool("_cc_isModifyingBottom", value: false);
	}

	public void OnArmSliderRelease()
	{
		_raceModelAnim.SetBool("_cc_isModifyingArm", value: false);
	}

	public void OnArmSliderHold()
	{
		_raceModelAnim.CrossFade("cc_armCheck", 0.2f, 5);
		_raceModelAnim.SetBool("_cc_isModifyingArm", value: true);
	}

	public void OnVoiceSliderRelease()
	{
		if (_currentRaceModel._scriptablePlayerRace._chatVoiceClips.Length != 0)
		{
			_aSrcCharacterVoice.Stop();
			_aSrcCharacterVoice.pitch = _voicePitchSlider.value;
			_aSrcCharacterVoice.PlayOneShot(_currentRaceModel._scriptablePlayerRace._chatVoiceClips[Random.Range(0, _currentRaceModel._scriptablePlayerRace._chatVoiceClips.Length)], 0.65f);
			_currentRaceModel.Set_MouthCondition(MouthCondition.Open, 0.45f);
		}
	}

	public void Create_Character()
	{
		ProfileDataManager _pdm;
		PlayerAppearance_Profile _pap;
		ScriptablePlayerRace _scriptRace;
		if (_canCreateCharacter)
		{
			Cancel_RandomizeAllConfirmation();
			_pdm = ProfileDataManager._current;
			_pap = _pdm._characterFile._appearanceProfile;
			_scriptRace = _currentRaceModel._scriptablePlayerRace;
			Apply_CharacterBaseData();
			Apply_StartingItemsAndEquipment();
			Apply_StartingAttributes();
			ClearCharacterNameInputField();
			ProfileDataManager._current.Save_ProfileData();
			MainMenuManager._current._menuInputBuffer = 0.2f;
			MainMenuManager._current.Set_MenuCondition(1);
		}
		void Apply_CharacterBaseData()
		{
			_currentRaceModel = _raceDisplayModels[_currentRaceSelected];
			_eD = _raceDisplayModels[_currentRaceSelected].GetComponent<RaceModelEquipDisplay>();
			_pap._setRaceTag = _scriptablePlayerRaces[_currentRaceSelected]._raceName;
			_pdm._characterFile._nickName = _characterNameInputField.text;
			_pdm._characterFile._version = Application.version;
			_pdm._characterFile._currency = 0;
			_pdm._characterFile._isNewCharacter = true;
			_pdm._characterFile._statsProfile._currentLevel = 1;
			_pap._setEar = _currentRaceModel.SetEarIndex;
			_pap._setHairStyle = _currentRaceModel.SetHairIndex;
			_pap._setMisc = _currentRaceModel.SetMiscIndex;
			_pap._setEye = _currentRaceModel.SetEyeIndex;
			_pap._setMouth = _currentRaceModel.SetMouthIndex;
			_pap._setTail = _currentRaceModel.SetTailIndex;
			_pap._displayBoobs = _currentRaceModel._displayBoobs;
			_pap._isLeftHanded = _leftHandedToggle.isOn;
			if (_pap._displayBoobs)
			{
				_pap._torsoWeight = 0f;
				_pap._boobWeight = _bodySlider_chestSize.value;
			}
			else
			{
				_pap._torsoWeight = _bodySlider_chestSize.value;
				_pap._boobWeight = 0f;
			}
			_pap._headWidth = _headSlider_headWidth.value;
			_pap._muzzleWeight = _headSlider_muzzle.value;
			_pap._heightWeight = _bodySlider_height.value;
			_pap._widthWeight = _bodySlider_width.value;
			_pap._armWeight = _bodySlider_armWidth.value;
			_pap._bellyWeight = _bodySlider_bellySize.value;
			_pap._bottomWeight = _bodySlider_bottomWidth.value;
			_pap._voicePitch = _voicePitchSlider.value;
			_pap._setTexture = _currentRaceModel.SetTextureIndex;
			_pap._setSkinColorProfile._hue = _hueSlider.value;
			_pap._setSkinColorProfile._brightness = _brightnessSlider.value;
			_pap._setSkinColorProfile._contrast = _contrastSlider.value;
			_pap._setSkinColorProfile._saturation = _saturationSlider.value;
			_pap._hairColorProfile._hue = _hairHueSlider.value;
			_pap._hairColorProfile._brightness = _hairBrightnessSlider.value;
			_pap._hairColorProfile._contrast = _hairContrastSlider.value;
			_pap._hairColorProfile._saturation = _hairSaturationSlider.value;
			_pap._hairIsBodyColor = _hairIsBodyColorToggle.isOn;
			_pap._miscColorProfile._hue = _miscHueSlider.value;
			_pap._miscColorProfile._brightness = _miscBrightnessSlider.value;
			_pap._miscColorProfile._contrast = _miscContrastSlider.value;
			_pap._miscColorProfile._saturation = _miscSaturationSlider.value;
			_pap._helmDyeIndex = _eD.DyeIndex;
		}
		void Apply_StartingAttributes()
		{
			List<AttributeStruct> list = new List<AttributeStruct>();
			for (int i = 0; i < GameManager._current._statLogics._statAttributes.Length; i++)
			{
				AttributeStruct item = new AttributeStruct
				{
					_attributeName = GameManager._current._statLogics._statAttributes[i]._attributeName,
					_attributeID = i,
					_attributeValue = _attributeBaseValues[i]
				};
				list.Add(item);
			}
			_pdm._characterFile._statsProfile._attributes = list.ToArray();
		}
		void Apply_StartingItemsAndEquipment()
		{
			StatLogics statLogics = GameManager._current._statLogics;
			List<ItemData> list = new List<ItemData>();
			ItemData itemData = new ItemData();
			ItemData itemData2 = new ItemData();
			ItemData itemData3 = new ItemData();
			if (_scriptRace._startingChestpiece != null)
			{
				itemData._itemName = _scriptRace._startingChestpiece._itemName;
				itemData._modifierID = 0;
				itemData._isEquipped = true;
				list.Add(itemData);
			}
			if (_scriptRace._startingLeggings != null)
			{
				itemData2._itemName = _scriptRace._startingLeggings._itemName;
				itemData2._modifierID = 0;
				itemData2._isEquipped = true;
				list.Add(itemData2);
			}
			if (statLogics._startingWeapons[_setWeaponIndex] != null)
			{
				itemData3._itemName = statLogics._startingWeapons[_setWeaponIndex]._itemName;
				itemData3._modifierID = 0;
				itemData3._isEquipped = true;
				list.Add(itemData3);
			}
			_pdm._characterFile._inventoryProfile = list.ToArray();
		}
	}

	public void Init_AttributeToolTip(int _index)
	{
		string attributeDescriptor = GameManager._current._statLogics._statAttributes[_index]._attributeDescriptor;
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector2(185f, 25f));
		ToolTipManager._current.Apply_GenericToolTip(attributeDescriptor);
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
	}

	public void Init_CustomizeSelectButtonToolTip(string _string)
	{
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector2(65f, 25f));
		ToolTipManager._current.Apply_GenericToolTip(_string);
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
	}

	public void Disable_Tooltip()
	{
		ToolTipManager._current._genericToolTip.Disable_ToolTip();
	}
}
