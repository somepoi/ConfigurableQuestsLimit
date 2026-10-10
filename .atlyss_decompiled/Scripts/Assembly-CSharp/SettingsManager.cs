using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MilkShake;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
	public static SettingsManager _current;

	private MainMenuManager _mainMenuManager;

	[Header("Settings Manager")]
	[SerializeField]
	private Canvas _settingsMenuCanvas;

	[SerializeField]
	private MenuElement _settingsMenuElement;

	[SerializeField]
	private string _dataPath = string.Empty;

	public SettingsMenuSelection _currentSettingsMenuSelection;

	public bool _isOpen;

	[Space]
	[SerializeField]
	private Button _videoSettingsButton;

	[SerializeField]
	private Button _audioSettingsButton;

	[SerializeField]
	private Button _inputSettingsButton;

	[SerializeField]
	private Button _networkSettingsButton;

	[SerializeField]
	private RectTransform _selectHighlight;

	[Header("Save Profile")]
	public SettingsProfile _settingsProfile;

	[Header("Cursor Parameters")]
	public Texture2D[] _cursorIcons;

	[SerializeField]
	private RawImage _cursorPreviewImage;

	[SerializeField]
	private Button _cursorIndex_subtract;

	[SerializeField]
	private Button _cursorIndex_add;

	[SerializeField]
	private Toggle _hardwareCursorToggle;

	private int _setCursorIndex;

	[Header("Video Settings Elements")]
	[SerializeField]
	private RectTransform _videoTabContent;

	[SerializeField]
	private MenuElement _videoTabElement;

	[SerializeField]
	private Dropdown _screenModeDropdown;

	[SerializeField]
	private Dropdown _fullScreenResolutionDropdown;

	[SerializeField]
	private Dropdown _antiAliasingDropdown;

	[SerializeField]
	private Dropdown _textureFilterDropdown;

	[SerializeField]
	private Dropdown _textureQualityDropdown;

	[SerializeField]
	private Toggle _anisoTropicTexturingToggle;

	[SerializeField]
	private Toggle _vSyncToggle;

	[SerializeField]
	private Slider _cameraFieldOfViewSlider;

	[SerializeField]
	private Text _cameraFieldOfViewText;

	[SerializeField]
	private Slider _cameraSmoothingSlider;

	[SerializeField]
	private Slider _cameraHorizPosSlider;

	[SerializeField]
	private Slider _cameraVertPosSlider;

	[SerializeField]
	private Dropdown _cameraRenderDistanceDropdown;

	[SerializeField]
	private Text _cameraSmoothingText;

	[SerializeField]
	private Text _cameraHorizText;

	[SerializeField]
	private Text _cameraVertText;

	[SerializeField]
	private Toggle _cameraBitCrushShaderToggle;

	[SerializeField]
	private Toggle _cameraUnderwaterShaderToggle;

	[SerializeField]
	private Toggle _cameraShakeToggle;

	public Toggle _disableWeaponGlowToggle;

	public Toggle _jiggleBonesToggle;

	public Toggle _clearUnderClothesToggle;

	public Toggle _disableGibsToggle;

	[Space]
	public List<Scr_QualityCrush> _foundQualityCrushes = new List<Scr_QualityCrush>();

	public List<Shaker> _foundCamShakers = new List<Shaker>();

	private Resolution[] _resolutions;

	private List<Resolution> _filteredResolutions;

	private bool _setResolution;

	private int _resolutionIndex;

	private float _currentRefreshRate;

	[Header("Audio Settings Elements")]
	[SerializeField]
	private RectTransform _audioTabContent;

	[SerializeField]
	private MenuElement _audioTabElement;

	[SerializeField]
	private AudioMixer _masterMixer;

	[SerializeField]
	private Slider _masterVolumeSlider;

	[SerializeField]
	private Slider _gameVolumeSlider;

	[SerializeField]
	private Slider _guiVolumeSlider;

	[SerializeField]
	private Slider _ambienceVolumeSlider;

	[SerializeField]
	private Slider _musicVolumeSlider;

	[SerializeField]
	private Slider _voiceVolumeSlider;

	[SerializeField]
	private Toggle _muteAllSoundToggle;

	[SerializeField]
	private Toggle _muteMusicToggle;

	[Header("Input Settings Elements")]
	[SerializeField]
	private GameObject _keyboardInputDollyObject;

	[SerializeField]
	private GameObject _gamepadInputDollyObject;

	[Space]
	public Dropdown _inputAxisTypeDropdown;

	[SerializeField]
	private RectTransform _inputTabContent;

	[SerializeField]
	private MenuElement _inputTabElement;

	[SerializeField]
	private Slider _cameraSensitivitySlider;

	[SerializeField]
	private Text _cameraSensitivityText;

	public Toggle _invertXAxisToggle;

	public Toggle _invertYCameraAxisToggle;

	public Toggle _analogDirectionMode;

	public Dropdown _inputStrafeMode;

	public Toggle _inputStrafeWhileHoldingWeapon;

	public Toggle _inputStrafeWhileMoveAttack;

	public Toggle _inputStrafeWhileCastingOffensiveSkill;

	public bool _detectedGamepadOnMenu;

	private float _bindKeyBuffer;

	[Space]
	[SerializeField]
	private KeyBindButton[] keyBindButtons = new KeyBindButton[5];

	private KeyBindButton _currentKeyBindButton;

	private Event _keyEvent;

	private bool _waitingForKey;

	private KeyCode _pressedKey;

	[Header("Game Settings / Interface")]
	[SerializeField]
	private RectTransform _networkTabContent;

	[SerializeField]
	private MenuElement _networkTabElement;

	public Toggle _displayGlobalNickNameTags;

	public Toggle _networkDisplayHostTag;

	public Toggle _networkDisplayLocalNameTag;

	public Toggle _hideDungeonMinimap;

	public Toggle _networkFlagPvPOnMapEnter;

	public Toggle _persistDisconnectHotkey;

	public Toggle _hideFPSCounter;

	public Toggle _hidePingCounter;

	public Toggle _hideStatPointCounter;

	public Toggle _hideSkillPointCounter;

	public Toggle _hideQuestTracker;

	public Toggle _hideDamageValueIcons;

	public Toggle _hidePlayerInstanceCounter;

	public Toggle _enableUnicode;

	public Toggle _fadeChatText;

	public Toggle _fadeGameFeed;

	public Toggle _hideNpcHeadIcons;

	public Toggle _displaySlotCooldownCounters;

	public bool _dontShowDisclaimer;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _keyBindPressAsrc;

	[SerializeField]
	private AudioSource _gamepadSelectAsrc;

	[Header("Gamepad")]
	[SerializeField]
	private Image _gamepad_leftButtonIco;

	[SerializeField]
	private Image _gamepad_rightButtonIco;

	[SerializeField]
	private Image _gamepad_returnIco;

	[SerializeField]
	private Image _gamepad_acceptIco;

	public float _gamepadInputBuffer;

	[Space]
	[SerializeField]
	private MenuElement _gamepad_contextElement_toggle;

	[SerializeField]
	private MenuElement _gamepad_contextElement_horizontal;

	[Space]
	[SerializeField]
	private List<SettingsCell> _displaySettingCells = new List<SettingsCell>();

	private int _selectedDisplayCell;

	[SerializeField]
	private List<SettingsCell> _audioSettingCells = new List<SettingsCell>();

	private int _selectedAudioCell;

	[SerializeField]
	private List<SettingsCell> _inputSettingCells = new List<SettingsCell>();

	private int _selectedInputCell;

	[SerializeField]
	private List<SettingsCell> _gameSettingCells = new List<SettingsCell>();

	private int _selectedGameCell;

	public int SetCursorIndex
	{
		get
		{
			return _setCursorIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _cursorIcons.Length - 1;
			}
			if (value >= _cursorIcons.Length)
			{
				value = 0;
			}
			_setCursorIndex = value;
		}
	}

	public int SelectedDisplayCell
	{
		get
		{
			return _selectedDisplayCell;
		}
		set
		{
			if (value > _displaySettingCells.Count - 1)
			{
				value = 0;
			}
			if (value < 0)
			{
				value = _displaySettingCells.Count - 1;
			}
			_selectedDisplayCell = value;
		}
	}

	public int SelectedAudioCell
	{
		get
		{
			return _selectedAudioCell;
		}
		set
		{
			if (value > _audioSettingCells.Count - 1)
			{
				value = 0;
			}
			if (value < 0)
			{
				value = _audioSettingCells.Count - 1;
			}
			_selectedAudioCell = value;
		}
	}

	public int SelectedInputCell
	{
		get
		{
			return _selectedInputCell;
		}
		set
		{
			if (value > _inputSettingCells.Count - 1)
			{
				value = 0;
			}
			if (value < 0)
			{
				value = _inputSettingCells.Count - 1;
			}
			_selectedInputCell = value;
		}
	}

	public int SelectedGameCell
	{
		get
		{
			return _selectedGameCell;
		}
		set
		{
			if (value > _gameSettingCells.Count - 1)
			{
				value = 0;
			}
			if (value < 0)
			{
				value = _gameSettingCells.Count - 1;
			}
			_selectedGameCell = value;
		}
	}

	[ContextMenu("Save Settings Profile")]
	public void Save_SettingsData()
	{
		if (!Directory.Exists(_dataPath))
		{
			Directory.CreateDirectory(_dataPath);
		}
		_settingsProfile._screenMode = _screenModeDropdown.value;
		_settingsProfile._setResolution = _setResolution;
		_settingsProfile._cursorIndex = _setCursorIndex;
		_settingsProfile._isHardwareCursor = _hardwareCursorToggle.isOn;
		_settingsProfile._textureFilterSetting = _textureFilterDropdown.value;
		_settingsProfile._textureQualitySetting = _textureQualityDropdown.value;
		_settingsProfile._anisotropicTextureSetting = _anisoTropicTexturingToggle.isOn;
		_settingsProfile._antiAliasingSetting = _antiAliasingDropdown.value;
		_settingsProfile._isVsync = _vSyncToggle.isOn;
		_settingsProfile._cameraFieldOfView = _cameraFieldOfViewSlider.value;
		_settingsProfile._cameraSmoothing = _cameraSmoothingSlider.value;
		_settingsProfile._cameraHoriz = _cameraHorizPosSlider.value;
		_settingsProfile._cameraVert = _cameraVertPosSlider.value;
		_settingsProfile._cameraRenderDistanceValue = _cameraRenderDistanceDropdown.value;
		_settingsProfile._cameraBitCrushEnabled = _cameraBitCrushShaderToggle.isOn;
		_settingsProfile._cameraUnderwaterShaderEnabled = _cameraUnderwaterShaderToggle.isOn;
		_settingsProfile._cameraShakeEnabled = _cameraShakeToggle.isOn;
		_settingsProfile._disableWeaponGlow = _disableWeaponGlowToggle.isOn;
		_settingsProfile._disableGibs = _disableGibsToggle.isOn;
		_settingsProfile._disableJiggleBones = _jiggleBonesToggle.isOn;
		_settingsProfile._clearUnderClothes = _clearUnderClothesToggle.isOn;
		_settingsProfile._masterVolume = _masterVolumeSlider.value;
		_settingsProfile._gameVolume = _gameVolumeSlider.value;
		_settingsProfile._guiVolume = _guiVolumeSlider.value;
		_settingsProfile._ambienceVolume = _ambienceVolumeSlider.value;
		_settingsProfile._musicVolume = _musicVolumeSlider.value;
		_settingsProfile._voiceVolume = _voiceVolumeSlider.value;
		_settingsProfile._muteApplication = _muteAllSoundToggle.isOn;
		_settingsProfile._muteMusic = _muteMusicToggle.isOn;
		_settingsProfile._setGamepadType = _inputAxisTypeDropdown.value;
		_settingsProfile._cameraSensitivity = _cameraSensitivitySlider.value;
		_settingsProfile._invertXAxis = _invertXAxisToggle.isOn;
		_settingsProfile._invertYAxis = _invertYCameraAxisToggle.isOn;
		_settingsProfile._analogDirectionMode = _analogDirectionMode.isOn;
		_settingsProfile._inputStrafeMode = _inputStrafeMode.value;
		_settingsProfile._inputStrafeWhileHoldingWeapon = _inputStrafeWhileHoldingWeapon.isOn;
		_settingsProfile._inputStrafeWhileMoveAttack = _inputStrafeWhileMoveAttack.isOn;
		_settingsProfile._inputStrafeWhileCastingOffensiveSkill = _inputStrafeWhileCastingOffensiveSkill.isOn;
		_settingsProfile._detectedGamepadOnMenu = _detectedGamepadOnMenu;
		InputControlManager.current.SaveInputData();
		_settingsProfile._displayGlobalNameTags = _displayGlobalNickNameTags.isOn;
		_settingsProfile._networkDisplayHostTag = _networkDisplayHostTag.isOn;
		_settingsProfile._networkDisplayLocalPlayerNameTag = _networkDisplayLocalNameTag.isOn;
		_settingsProfile._hideDungeonMinimap = _hideDungeonMinimap.isOn;
		_settingsProfile._flagPvPOnEnterMap = _networkFlagPvPOnMapEnter.isOn;
		_settingsProfile._persistDisconnectHotkey = _persistDisconnectHotkey.isOn;
		_settingsProfile._hideFpsCounter = _hideFPSCounter.isOn;
		_settingsProfile._hidePingCounter = _hidePingCounter.isOn;
		_settingsProfile._hideStatPointCounter = _hideStatPointCounter.isOn;
		_settingsProfile._hideSkillPointCounter = _hideSkillPointCounter.isOn;
		_settingsProfile._hideQuestTracker = _hideQuestTracker.isOn;
		_settingsProfile._enableUnicodeCharacters = _enableUnicode.isOn;
		_settingsProfile._fadeChatText = _fadeChatText.isOn;
		_settingsProfile._fadeGameFeed = _fadeGameFeed.isOn;
		_settingsProfile._hideDamageValueIcons = _hideDamageValueIcons.isOn;
		_settingsProfile._hidePlayerInstanceCounter = _hidePlayerInstanceCounter.isOn;
		_settingsProfile._hideNpcHeadIcons = _hideNpcHeadIcons.isOn;
		_settingsProfile._displaySlotCoolDownCounters = _displaySlotCooldownCounters.isOn;
		_settingsProfile._dontShowDisclaimer = _dontShowDisclaimer;
		if (File.Exists(_dataPath + "/settings.json"))
		{
			HostConsole._current.Init_NetworkStatusMessage("Settings Profile Saved...");
		}
		string contents = JsonUtility.ToJson(_settingsProfile, prettyPrint: true);
		File.WriteAllText(_dataPath + "/settings.json", contents);
		Load_SettingsData();
	}

	public void Save_CharacterSelected()
	{
		_settingsProfile._lastCharacterFile = ProfileDataManager._current.SelectedFileIndex;
		_settingsProfile._lastCharacterPage = MainMenuManager._current._characterSelectManager.SelectedSlotPage;
		string contents = JsonUtility.ToJson(_settingsProfile, prettyPrint: true);
		File.WriteAllText(_dataPath + "/settings.json", contents);
	}

	[ContextMenu("Load Settings Profile")]
	public void Load_SettingsData()
	{
		if (File.Exists(_dataPath + "/settings.json"))
		{
			string json = File.ReadAllText(_dataPath + "/settings.json");
			_settingsProfile = JsonUtility.FromJson<SettingsProfile>(json);
			_screenModeDropdown.value = _settingsProfile._screenMode;
			_setResolution = _settingsProfile._setResolution;
			_setCursorIndex = _settingsProfile._cursorIndex;
			_hardwareCursorToggle.isOn = _settingsProfile._isHardwareCursor;
			if (!_hardwareCursorToggle.isOn)
			{
				Cursor.SetCursor(_cursorIcons[_setCursorIndex], Vector2.one * 15f, CursorMode.ForceSoftware);
			}
			else
			{
				Cursor.SetCursor(_cursorIcons[_setCursorIndex], Vector2.one * 15f, CursorMode.Auto);
			}
			_textureFilterDropdown.value = _settingsProfile._textureFilterSetting;
			_textureQualityDropdown.value = _settingsProfile._textureQualitySetting;
			_anisoTropicTexturingToggle.isOn = _settingsProfile._anisotropicTextureSetting;
			_antiAliasingDropdown.value = _settingsProfile._antiAliasingSetting;
			_vSyncToggle.isOn = _settingsProfile._isVsync;
			_cameraFieldOfViewSlider.value = _settingsProfile._cameraFieldOfView;
			_cameraSmoothingSlider.value = _settingsProfile._cameraSmoothing;
			_cameraHorizPosSlider.value = _settingsProfile._cameraHoriz;
			_cameraVertPosSlider.value = _settingsProfile._cameraVert;
			_cameraRenderDistanceDropdown.value = _settingsProfile._cameraRenderDistanceValue;
			_cameraBitCrushShaderToggle.isOn = _settingsProfile._cameraBitCrushEnabled;
			_cameraUnderwaterShaderToggle.isOn = _settingsProfile._cameraUnderwaterShaderEnabled;
			_cameraShakeToggle.isOn = _settingsProfile._cameraShakeEnabled;
			_disableWeaponGlowToggle.isOn = _settingsProfile._disableWeaponGlow;
			_disableGibsToggle.isOn = _settingsProfile._disableGibs;
			_jiggleBonesToggle.isOn = _settingsProfile._disableJiggleBones;
			_clearUnderClothesToggle.isOn = _settingsProfile._clearUnderClothes;
			_masterVolumeSlider.value = _settingsProfile._masterVolume;
			_gameVolumeSlider.value = _settingsProfile._gameVolume;
			_guiVolumeSlider.value = _settingsProfile._guiVolume;
			_ambienceVolumeSlider.value = _settingsProfile._ambienceVolume;
			_musicVolumeSlider.value = _settingsProfile._musicVolume;
			_voiceVolumeSlider.value = _settingsProfile._voiceVolume;
			_muteAllSoundToggle.isOn = _settingsProfile._muteApplication;
			_muteMusicToggle.isOn = _settingsProfile._muteMusic;
			if (_settingsProfile._setGamepadType > 1)
			{
				_settingsProfile._setGamepadType = 1;
			}
			_inputAxisTypeDropdown.value = _settingsProfile._setGamepadType;
			_cameraSensitivitySlider.value = _settingsProfile._cameraSensitivity;
			_invertXAxisToggle.isOn = _settingsProfile._invertXAxis;
			_invertYCameraAxisToggle.isOn = _settingsProfile._invertYAxis;
			_analogDirectionMode.isOn = _settingsProfile._analogDirectionMode;
			_inputStrafeMode.value = _settingsProfile._inputStrafeMode;
			_inputStrafeWhileHoldingWeapon.isOn = _settingsProfile._inputStrafeWhileHoldingWeapon;
			_inputStrafeWhileMoveAttack.isOn = _settingsProfile._inputStrafeWhileMoveAttack;
			_inputStrafeWhileCastingOffensiveSkill.isOn = _settingsProfile._inputStrafeWhileCastingOffensiveSkill;
			_detectedGamepadOnMenu = _settingsProfile._detectedGamepadOnMenu;
			InputControlManager.current.LoadInputData();
			_displayGlobalNickNameTags.isOn = _settingsProfile._displayGlobalNameTags;
			_networkDisplayHostTag.isOn = _settingsProfile._networkDisplayHostTag;
			_networkDisplayLocalNameTag.isOn = _settingsProfile._networkDisplayLocalPlayerNameTag;
			_hideDungeonMinimap.isOn = _settingsProfile._hideDungeonMinimap;
			_networkFlagPvPOnMapEnter.isOn = _settingsProfile._flagPvPOnEnterMap;
			_persistDisconnectHotkey.isOn = _settingsProfile._persistDisconnectHotkey;
			_hideFPSCounter.isOn = _settingsProfile._hideFpsCounter;
			_hidePingCounter.isOn = _settingsProfile._hidePingCounter;
			_hideStatPointCounter.isOn = _settingsProfile._hideStatPointCounter;
			_hideSkillPointCounter.isOn = _settingsProfile._hideSkillPointCounter;
			_hideQuestTracker.isOn = _settingsProfile._hideQuestTracker;
			_enableUnicode.isOn = _settingsProfile._enableUnicodeCharacters;
			_fadeChatText.isOn = _settingsProfile._fadeChatText;
			_fadeGameFeed.isOn = _settingsProfile._fadeGameFeed;
			_hideDamageValueIcons.isOn = _settingsProfile._hideDamageValueIcons;
			_hidePlayerInstanceCounter.isOn = _settingsProfile._hidePlayerInstanceCounter;
			_hideNpcHeadIcons.isOn = _settingsProfile._hideNpcHeadIcons;
			_displaySlotCooldownCounters.isOn = _settingsProfile._displaySlotCoolDownCounters;
			_dontShowDisclaimer = _settingsProfile._dontShowDisclaimer;
			if (_vSyncToggle.isOn)
			{
				QualitySettings.vSyncCount = 1;
				Application.targetFrameRate = 60;
			}
			else
			{
				QualitySettings.vSyncCount = -1;
				Application.targetFrameRate = 999;
			}
			switch (_antiAliasingDropdown.value)
			{
			case 0:
				QualitySettings.antiAliasing = 0;
				break;
			case 1:
				QualitySettings.antiAliasing = 2;
				break;
			case 2:
				QualitySettings.antiAliasing = 4;
				break;
			case 3:
				QualitySettings.antiAliasing = 8;
				break;
			}
			QualitySettings.globalTextureMipmapLimit = _textureQualityDropdown.value;
			if (_anisoTropicTexturingToggle.isOn)
			{
				QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			}
			else
			{
				QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
			}
			Invoke("Set_TextureFilterToggle", 0.2f);
		}
	}

	private void Awake()
	{
		if (_current == null)
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
			_current = this;
			_dataPath = Application.dataPath + "/Settings/";
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		_settingsMenuElement.isEnabled = false;
		Set_SettingMenuSelectionIndex(0);
		_videoSettingsButton.onClick.AddListener(() =>
		{
			Set_SettingMenuSelectionIndex(0);
		});
		_audioSettingsButton.onClick.AddListener(() =>
		{
			Set_SettingMenuSelectionIndex(1);
		});
		_inputSettingsButton.onClick.AddListener(() =>
		{
			Set_SettingMenuSelectionIndex(2);
		});
		_networkSettingsButton.onClick.AddListener(() =>
		{
			Set_SettingMenuSelectionIndex(3);
		});
		_cursorIndex_add.onClick.AddListener(OnClick_AddCursorIndex);
		_cursorIndex_subtract.onClick.AddListener(OnClick_SubCursorIndex);
		Init_CacheGamepadNavigationCells();
		Invoke("Init_InputBindingButtons", 1.25f);
		Load_SettingsData();
		Init_ResolutionCache();
		void Init_CacheGamepadNavigationCells()
		{
			SettingsCell[] componentsInChildren = _videoTabContent.GetComponentsInChildren<SettingsCell>();
			_displaySettingCells.AddRange(componentsInChildren);
			SettingsCell[] componentsInChildren2 = _audioTabContent.GetComponentsInChildren<SettingsCell>();
			_audioSettingCells.AddRange(componentsInChildren2);
			SettingsCell[] componentsInChildren3 = _inputTabContent.GetComponentsInChildren<SettingsCell>();
			_inputSettingCells.AddRange(componentsInChildren3);
			SettingsCell[] componentsInChildren4 = _networkTabContent.GetComponentsInChildren<SettingsCell>();
			_gameSettingCells.AddRange(componentsInChildren4);
		}
		void Init_ResolutionCache()
		{
			_resolutions = Screen.resolutions;
			_filteredResolutions = new List<Resolution>();
			_fullScreenResolutionDropdown.ClearOptions();
			_currentRefreshRate = Screen.currentResolution.refreshRate;
			_resolutionIndex = 0;
			for (int i = 0; i < _resolutions.Length; i++)
			{
				if ((float)_resolutions[i].refreshRate == _currentRefreshRate)
				{
					_filteredResolutions.Add(_resolutions[i]);
				}
			}
			List<string> list = new List<string>();
			for (int j = 0; j < _filteredResolutions.Count; j++)
			{
				string item = _filteredResolutions[j].width + "x" + _filteredResolutions[j].height + " " + _filteredResolutions[j].refreshRate + " Hz";
				list.Add(item);
				if (_filteredResolutions[j].width == Screen.width && _filteredResolutions[j].height == Screen.height)
				{
					_resolutionIndex = j;
				}
			}
			_fullScreenResolutionDropdown.AddOptions(list);
			_fullScreenResolutionDropdown.value = _resolutionIndex;
			_fullScreenResolutionDropdown.RefreshShownValue();
			StartCoroutine(Init_CheckSteamDeckResolution());
		}
	}

	private IEnumerator Init_CheckSteamDeckResolution()
	{
		while (!MainMenuManager._current)
		{
			yield return null;
		}
		if (!_setResolution)
		{
			if (SteamManager.Initialized && SteamUtils.IsSteamRunningOnSteamDeck())
			{
				Screen.SetResolution(1280, 800, fullscreen: true);
				Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
				_screenModeDropdown.value = 1;
			}
			_setResolution = true;
		}
	}

	private void Init_InputBindingButtons()
	{
		for (int i = 0; i < keyBindButtons.Length; i++)
		{
			KeyBindButton keyBindButton = keyBindButtons[i];
			int _index = i;
			keyBindButton._button.onClick.AddListener(() =>
			{
				OnClick_KeybindButton(_index);
			});
			KeyBindButton_SearchReference(keyBindButton);
		}
	}

	public void Set_SettingMenuSelectionIndex(int _index)
	{
		_currentSettingsMenuSelection = (SettingsMenuSelection)_index;
		_videoTabElement.isEnabled = _currentSettingsMenuSelection == SettingsMenuSelection.Video;
		_audioTabElement.isEnabled = _currentSettingsMenuSelection == SettingsMenuSelection.Audio;
		_inputTabElement.isEnabled = _currentSettingsMenuSelection == SettingsMenuSelection.Input;
		_networkTabElement.isEnabled = _currentSettingsMenuSelection == SettingsMenuSelection.Network;
		_videoTabContent.anchoredPosition = Vector2.zero;
		_audioTabContent.anchoredPosition = Vector2.zero;
		_inputTabContent.anchoredPosition = Vector2.zero;
		_networkTabContent.anchoredPosition = Vector2.zero;
		_waitingForKey = false;
	}

	public void Open_SettingsMenu()
	{
		_mainMenuManager = MainMenuManager._current;
		_settingsMenuElement.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		_settingsMenuElement.isEnabled = true;
		Set_SettingMenuSelectionIndex(0);
		_isOpen = true;
	}

	public void Close_SettingsMenu()
	{
		_mainMenuManager = MainMenuManager._current;
		if (_mainMenuManager._mainMenuCondition == MainMenuCondition.MainMenu)
		{
			_mainMenuManager.Set_MenuCondition(0);
		}
		CameraFunction._current._unlockedCamera = false;
		_settingsMenuElement.isEnabled = false;
		Set_SettingMenuSelectionIndex(0);
		_waitingForKey = false;
		_isOpen = false;
	}

	private void Update()
	{
		_settingsMenuCanvas.enabled = _isOpen;
		_fullScreenResolutionDropdown.interactable = _filteredResolutions.Count > 0;
		_cursorPreviewImage.texture = _cursorIcons[_setCursorIndex];
		if (_bindKeyBuffer > 0f)
		{
			_bindKeyBuffer -= Time.deltaTime;
		}
		else
		{
			_bindKeyBuffer = 0f;
		}
		if (_gamepadInputBuffer > 0f)
		{
			_gamepadInputBuffer -= Time.deltaTime;
		}
		else
		{
			_gamepadInputBuffer = 0f;
		}
		if (_isOpen && _bindKeyBuffer <= 0f && Input.GetKeyDown(KeyCode.Escape))
		{
			Close_SettingsMenu();
		}
		switch (_currentSettingsMenuSelection)
		{
		case SettingsMenuSelection.Video:
			_selectHighlight.transform.position = _videoSettingsButton.transform.position;
			break;
		case SettingsMenuSelection.Audio:
			_selectHighlight.transform.position = _audioSettingsButton.transform.position;
			break;
		case SettingsMenuSelection.Input:
			_selectHighlight.transform.position = _inputSettingsButton.transform.position;
			break;
		case SettingsMenuSelection.Network:
			_selectHighlight.transform.position = _networkSettingsButton.transform.position;
			break;
		}
		Handle_GamePadControl();
		Handle_AudioParameters();
		Handle_InputParameters();
		Apply_CameraParameters();
		void Handle_GamePadControl()
		{
			_gamepad_acceptIco.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
			_gamepad_returnIco.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
			_gamepad_leftButtonIco.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
			_gamepad_rightButtonIco.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
			for (int i = 0; i < _displaySettingCells.Count; i++)
			{
				_displaySettingCells[i]._isSelected = i == _selectedDisplayCell && _isOpen;
				if (i == _selectedDisplayCell && !Cursor.visible)
				{
					_videoTabContent.anchoredPosition = Vector2.Lerp(_videoTabContent.anchoredPosition, _displaySettingCells[i]._menuPosition, 18.5f * Time.deltaTime);
				}
			}
			for (int j = 0; j < _audioSettingCells.Count; j++)
			{
				_audioSettingCells[j]._isSelected = j == _selectedAudioCell && _isOpen;
				if (j == _selectedAudioCell && !Cursor.visible)
				{
					_audioTabContent.anchoredPosition = Vector2.Lerp(_audioTabContent.anchoredPosition, _audioSettingCells[j]._menuPosition, 18.5f * Time.deltaTime);
				}
			}
			for (int k = 0; k < _inputSettingCells.Count; k++)
			{
				_inputSettingCells[k]._isSelected = k == _selectedInputCell && _isOpen;
				if (k == _selectedInputCell && !Cursor.visible)
				{
					_inputTabContent.anchoredPosition = Vector2.Lerp(_inputTabContent.anchoredPosition, _inputSettingCells[k]._menuPosition, 18.5f * Time.deltaTime);
				}
			}
			for (int l = 0; l < _gameSettingCells.Count; l++)
			{
				_gameSettingCells[l]._isSelected = l == _selectedGameCell && _isOpen;
				if (l == _selectedGameCell && !Cursor.visible)
				{
					_networkTabContent.anchoredPosition = Vector2.Lerp(_networkTabContent.anchoredPosition, _gameSettingCells[l]._menuPosition, 18.5f * Time.deltaTime);
				}
			}
			if (InputControlManager.current._setGamepadType == GamepadType.Keyboard || !_isOpen)
			{
				_gamepad_contextElement_toggle.isEnabled = false;
				_gamepad_contextElement_horizontal.isEnabled = false;
				SelectedDisplayCell = 0;
				SelectedAudioCell = 0;
				SelectedInputCell = 0;
				SelectedGameCell = 0;
			}
			else
			{
				switch (_currentSettingsMenuSelection)
				{
				case SettingsMenuSelection.Video:
					_gamepad_contextElement_toggle.isEnabled = _displaySettingCells[_selectedDisplayCell]._setToggle;
					_gamepad_contextElement_horizontal.isEnabled = (bool)_displaySettingCells[_selectedDisplayCell]._setSlider || (bool)_displaySettingCells[_selectedDisplayCell]._setDropdown || (bool)_displaySettingCells[_selectedDisplayCell]._leftOptionButton || (bool)_displaySettingCells[_selectedDisplayCell]._rightOptionButton;
					SelectedAudioCell = 0;
					SelectedInputCell = 0;
					SelectedGameCell = 0;
					break;
				case SettingsMenuSelection.Audio:
					_gamepad_contextElement_toggle.isEnabled = _audioSettingCells[_selectedAudioCell]._setToggle;
					_gamepad_contextElement_horizontal.isEnabled = (bool)_audioSettingCells[_selectedAudioCell]._setSlider || (bool)_audioSettingCells[_selectedAudioCell]._setDropdown;
					SelectedDisplayCell = 0;
					SelectedInputCell = 0;
					SelectedGameCell = 0;
					break;
				case SettingsMenuSelection.Input:
					_gamepad_contextElement_toggle.isEnabled = _inputSettingCells[_selectedInputCell]._setToggle;
					_gamepad_contextElement_horizontal.isEnabled = (bool)_inputSettingCells[_selectedInputCell]._setSlider || (bool)_inputSettingCells[_selectedInputCell]._setDropdown;
					SelectedDisplayCell = 0;
					SelectedAudioCell = 0;
					SelectedGameCell = 0;
					break;
				case SettingsMenuSelection.Network:
					_gamepad_contextElement_toggle.isEnabled = _gameSettingCells[_selectedGameCell]._setToggle;
					_gamepad_contextElement_horizontal.isEnabled = (bool)_gameSettingCells[_selectedGameCell]._setSlider || (bool)_gameSettingCells[_selectedGameCell]._setDropdown;
					SelectedDisplayCell = 0;
					SelectedAudioCell = 0;
					SelectedInputCell = 0;
					break;
				}
				if (GamepadInput.GetButton(GamepadButton.LeftShoulder))
				{
					_gamepad_leftButtonIco.color = Color.white;
				}
				else
				{
					_gamepad_leftButtonIco.color = Color.grey;
				}
				if (GamepadInput.GetButton(GamepadButton.RightShoulder))
				{
					_gamepad_rightButtonIco.color = Color.white;
				}
				else
				{
					_gamepad_rightButtonIco.color = Color.grey;
				}
				if (GamepadInput.GetButtonDown(GamepadButton.LeftShoulder))
				{
					switch (_currentSettingsMenuSelection)
					{
					case SettingsMenuSelection.Audio:
						Set_SettingMenuSelectionIndex(0);
						_gamepadSelectAsrc.Play();
						break;
					case SettingsMenuSelection.Input:
						Set_SettingMenuSelectionIndex(1);
						_gamepadSelectAsrc.Play();
						break;
					case SettingsMenuSelection.Network:
						Set_SettingMenuSelectionIndex(2);
						_gamepadSelectAsrc.Play();
						break;
					}
				}
				if (GamepadInput.GetButtonDown(GamepadButton.RightShoulder))
				{
					switch (_currentSettingsMenuSelection)
					{
					case SettingsMenuSelection.Video:
						Set_SettingMenuSelectionIndex(1);
						_gamepadSelectAsrc.Play();
						break;
					case SettingsMenuSelection.Audio:
						Set_SettingMenuSelectionIndex(2);
						_gamepadSelectAsrc.Play();
						break;
					case SettingsMenuSelection.Input:
						Set_SettingMenuSelectionIndex(3);
						_gamepadSelectAsrc.Play();
						break;
					}
				}
				if ((!Player._mainPlayer && GamepadInput.GetButtonDown(GamepadButton.LeftStick_Down)) || InputControlManager.current.DpadDown(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Down))
				{
					switch (_currentSettingsMenuSelection)
					{
					case SettingsMenuSelection.Video:
						SelectedDisplayCell++;
						_gamepadSelectAsrc.Play();
						break;
					case SettingsMenuSelection.Audio:
						SelectedAudioCell++;
						_gamepadSelectAsrc.Play();
						break;
					case SettingsMenuSelection.Input:
						SelectedInputCell++;
						_gamepadSelectAsrc.Play();
						break;
					case SettingsMenuSelection.Network:
						SelectedGameCell++;
						_gamepadSelectAsrc.Play();
						break;
					}
				}
				if ((!Player._mainPlayer && GamepadInput.GetButtonDown(GamepadButton.LeftStick_Up)) || InputControlManager.current.DpadUp(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Up))
				{
					switch (_currentSettingsMenuSelection)
					{
					case SettingsMenuSelection.Video:
						SelectedDisplayCell--;
						_gamepadSelectAsrc.Play();
						break;
					case SettingsMenuSelection.Audio:
						SelectedAudioCell--;
						_gamepadSelectAsrc.Play();
						break;
					case SettingsMenuSelection.Input:
						SelectedInputCell--;
						_gamepadSelectAsrc.Play();
						break;
					case SettingsMenuSelection.Network:
						SelectedGameCell--;
						_gamepadSelectAsrc.Play();
						break;
					}
				}
				if (_bindKeyBuffer <= 0f)
				{
					if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
					{
						Save_SettingsData();
						Close_SettingsMenu();
						_gamepadInputBuffer = 0.2f;
					}
					if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
					{
						Close_SettingsMenu();
						_gamepadInputBuffer = 0.2f;
					}
					if (GamepadInput.GetButtonDown(GamepadButton.Start))
					{
						Close_SettingsMenu();
						_gamepadInputBuffer = 0.2f;
					}
				}
			}
		}
	}

	private void OnClick_AddCursorIndex()
	{
		SetCursorIndex++;
	}

	private void OnClick_SubCursorIndex()
	{
		SetCursorIndex--;
	}

	public void Set_WindowMode(int _windowIndex)
	{
		switch (_windowIndex)
		{
		case 0:
			Screen.fullScreenMode = FullScreenMode.Windowed;
			break;
		case 1:
			Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
			break;
		case 2:
			Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
			break;
		}
	}

	public void Set_ResolutionParams(int _resolutionIndex)
	{
		Resolution resolution = _filteredResolutions[_resolutionIndex];
		switch (_screenModeDropdown.value)
		{
		case 0:
			Screen.SetResolution(resolution.width, resolution.height, fullscreen: false);
			break;
		case 1:
			Screen.SetResolution(resolution.width, resolution.height, fullscreen: true);
			break;
		case 2:
			Screen.SetResolution(resolution.width, resolution.height, fullscreen: true);
			break;
		}
	}

	public void Set_TextureFilterToggle()
	{
		VisualManager component = GetComponent<VisualManager>();
		if (_textureFilterDropdown.value == 0)
		{
			component.Apply_FilteringMode(FilterMode.Bilinear);
		}
		else
		{
			component.Apply_FilteringMode(FilterMode.Point);
		}
	}

	private void Apply_CameraParameters()
	{
		for (int i = 0; i < _foundQualityCrushes.Count; i++)
		{
			if (!_foundQualityCrushes[i])
			{
				_foundQualityCrushes.RemoveAt(i);
				break;
			}
			_foundQualityCrushes[i].enabled = _cameraBitCrushShaderToggle.isOn;
		}
		for (int j = 0; j < _foundCamShakers.Count; j++)
		{
			if (!_foundCamShakers[j])
			{
				_foundCamShakers.RemoveAt(j);
				break;
			}
			_foundCamShakers[j].enabled = _cameraShakeToggle.isOn;
		}
		_cameraFieldOfViewText.text = $"{GameManager._current.ConvertFloat_ToN2(_cameraFieldOfViewSlider.value)}";
		_cameraSensitivityText.text = $"{GameManager._current.ConvertFloat_ToN2(_cameraSensitivitySlider.value)}";
		_cameraSmoothingText.text = $"{GameManager._current.ConvertFloat_ToN2(_cameraSmoothingSlider.value)}";
		_cameraHorizText.text = $"{GameManager._current.ConvertFloat_ToN2(_cameraHorizPosSlider.value)}";
		_cameraVertText.text = $"{GameManager._current.ConvertFloat_ToN2(_cameraVertPosSlider.value)}";
		if (!CameraFunction._current)
		{
			return;
		}
		CameraFunction._current._mainCamera.fieldOfView = _cameraFieldOfViewSlider.value;
		CameraFunction._current.step = _cameraSmoothingSlider.value;
		CameraFunction._current._mainCamera.GetComponent<CameraCollision>().dollyDir = new Vector3(_cameraHorizPosSlider.value, _cameraVertPosSlider.value, -0.4f);
		switch (_cameraRenderDistanceDropdown.value)
		{
		case 0:
			CameraFunction._current._mainCamera.farClipPlane = 150f;
			CameraFunction._current._UICamera.farClipPlane = 150f;
			break;
		case 1:
			CameraFunction._current._mainCamera.farClipPlane = 350f;
			CameraFunction._current._UICamera.farClipPlane = 350f;
			break;
		case 2:
			CameraFunction._current._mainCamera.farClipPlane = 650f;
			CameraFunction._current._UICamera.farClipPlane = 650f;
			break;
		case 3:
			CameraFunction._current._mainCamera.farClipPlane = 1000f;
			CameraFunction._current._UICamera.farClipPlane = 1000f;
			break;
		}
		if ((bool)Player._mainPlayer)
		{
			GameObject gameObject = CameraFunction._current._mainCamera.gameObject;
			if (!_cameraUnderwaterShaderToggle.isOn)
			{
				gameObject.GetComponent<WaterCamera>().Wobble.SetFloat("_Intensity", 0f);
			}
			else
			{
				gameObject.GetComponent<WaterCamera>().Wobble.SetFloat("_Intensity", 8f);
			}
		}
	}

	public void Recache_PlayerDisplayRaceModels()
	{
		if (NetworkClient.active || NetworkClient.isConnected)
		{
			PlayerVisual[] array = UnityEngine.Object.FindObjectsOfType<PlayerVisual>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Apply_NetworkedCharacterDisplay();
			}
		}
	}

	private void Handle_AudioParameters()
	{
		if (NetworkServer.active && !NetworkClient.active)
		{
			AudioListener.volume = 0f;
		}
		else if (_muteAllSoundToggle.isOn)
		{
			AudioListener.volume = 0f;
		}
		else
		{
			AudioListener.volume = 1f;
		}
		if (_muteMusicToggle.isOn)
		{
			_masterMixer.SetFloat("musicVolume", -80f);
		}
		else
		{
			_masterMixer.SetFloat("musicVolume", Mathf.Log10(_musicVolumeSlider.value) * 20f);
		}
		_masterMixer.SetFloat("masterVolume", Mathf.Log10(_masterVolumeSlider.value) * 20f);
		_masterMixer.SetFloat("gameVolume", Mathf.Log10(_gameVolumeSlider.value) * 20f);
		_masterMixer.SetFloat("ambienceVolume", Mathf.Log10(_ambienceVolumeSlider.value) * 20f);
		_masterMixer.SetFloat("guiVolume", Mathf.Log10(_guiVolumeSlider.value) * 20f);
		_masterMixer.SetFloat("voiceVolume", Mathf.Log10(_voiceVolumeSlider.value) * 20f);
	}

	public void Handle_InputParameters()
	{
		InputControlManager current = InputControlManager.current;
		switch (_inputStrafeMode.value)
		{
		case 0:
			current.isLockDirectionToggled = false;
			break;
		case 1:
			current.isLockDirectionToggled = true;
			break;
		}
		switch (_inputAxisTypeDropdown.value)
		{
		case 0:
			current._setGamepadType = GamepadType.Keyboard;
			break;
		case 1:
			current._setGamepadType = GamepadType.XboxOne;
			break;
		}
		_keyboardInputDollyObject.SetActive(current._setGamepadType == GamepadType.Keyboard);
		_gamepadInputDollyObject.SetActive(current._setGamepadType != GamepadType.Keyboard);
		if ((bool)CameraFunction._current)
		{
			CameraFunction._current.inputSensitivity = _cameraSensitivitySlider.value;
		}
		current._invert_altHoriz = _invertXAxisToggle.isOn;
		current._invert_vertHoriz = _invertYCameraAxisToggle.isOn;
		for (int i = 0; i < keyBindButtons.Length; i++)
		{
			KeyBindButton keyBindButton = keyBindButtons[i];
			if (keyBindButton.waitForKeyPress)
			{
				keyBindButton._buttonLabel.text = "Press a key";
				_bindKeyBuffer = 0.25f;
			}
			if (keyBindButton._keyBind != string.Empty && !keyBindButton.waitForKeyPress)
			{
				keyBindButton._buttonLabel.text = current.Convert_KeyCodeName(keyBindButton._keyBind);
			}
			if (keyBindButton._keyBind == string.Empty && !keyBindButton.waitForKeyPress)
			{
				keyBindButton._buttonLabel.text = "Unbound";
			}
		}
		if (!_waitingForKey)
		{
			return;
		}
		bool flag = false;
		if (Input.GetMouseButtonDown(3))
		{
			_pressedKey = KeyCode.Mouse3;
			flag = true;
		}
		if (Input.GetMouseButtonDown(4))
		{
			_pressedKey = KeyCode.Mouse4;
			flag = true;
		}
		if (Input.GetMouseButtonDown(5))
		{
			_pressedKey = KeyCode.Mouse5;
			flag = true;
		}
		if (Input.GetMouseButtonDown(6))
		{
			_pressedKey = KeyCode.Mouse6;
			flag = true;
		}
		foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
		{
			if (Input.GetKey(value))
			{
				_pressedKey = value;
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		_currentKeyBindButton._keyBind = _pressedKey.ToString();
		KeyBindButton[] array = keyBindButtons;
		foreach (KeyBindButton keyBindButton2 in array)
		{
			if (keyBindButton2 != null)
			{
				keyBindButton2._button.interactable = true;
			}
		}
		_currentKeyBindButton.waitForKeyPress = false;
		_currentKeyBindButton._buttonLabel.text = _currentKeyBindButton._keyBind;
		array = keyBindButtons;
		foreach (KeyBindButton keyBindButton3 in array)
		{
			if (keyBindButton3 != null)
			{
				if (keyBindButton3 != _currentKeyBindButton && keyBindButton3._keyBind == _currentKeyBindButton._keyBind)
				{
					keyBindButton3._keyBind = KeyCode.None.ToString();
				}
				Apply_KeyToProfile(keyBindButton3, keyBindButton3._inputReference);
			}
		}
		_currentKeyBindButton = null;
		_waitingForKey = false;
		current.SaveInputData();
		current.LoadInputData();
	}

	public void OnClick_KeybindButton(int index)
	{
		if (!(_bindKeyBuffer > 0f))
		{
			KeyBindButton[] array = keyBindButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i]._button.interactable = false;
			}
			_keyBindPressAsrc.Play();
			_currentKeyBindButton = keyBindButtons[index];
			_currentKeyBindButton.waitForKeyPress = true;
			_waitingForKey = true;
		}
	}

	public void ResetAllInputs()
	{
		InputControlManager.current.InputProfile_SetDefaults();
		KeyBindButton[] array = keyBindButtons;
		foreach (KeyBindButton keyBindButton in array)
		{
			if (keyBindButton != null)
			{
				KeyBindButton_SearchReference(keyBindButton);
			}
		}
	}

	public void KeyBindButton_SearchReference(KeyBindButton _keybindButton)
	{
		InputControlManager current = InputControlManager.current;
		if (_keybindButton._inputReference == "Up")
		{
			_keybindButton._keyBind = current._inputProfile._forwardInput;
		}
		if (_keybindButton._inputReference == "Down")
		{
			_keybindButton._keyBind = current._inputProfile._backwardInput;
		}
		if (_keybindButton._inputReference == "Left")
		{
			_keybindButton._keyBind = current._inputProfile._leftInput;
		}
		if (_keybindButton._inputReference == "Right")
		{
			_keybindButton._keyBind = current._inputProfile._rightInput;
		}
		if (_keybindButton._inputReference == "Jump")
		{
			_keybindButton._keyBind = current._inputProfile._jumpInput;
		}
		if (_keybindButton._inputReference == "Dash")
		{
			_keybindButton._keyBind = current._inputProfile._dashInput;
		}
		if (_keybindButton._inputReference == "LockDirection")
		{
			_keybindButton._keyBind = current._inputProfile._lockDirection;
		}
		if (_keybindButton._inputReference == "Attack")
		{
			_keybindButton._keyBind = current._inputProfile._attackInput;
		}
		if (_keybindButton._inputReference == "ChargeAttack")
		{
			_keybindButton._keyBind = current._inputProfile._chargeAttackInput;
		}
		if (_keybindButton._inputReference == "Block")
		{
			_keybindButton._keyBind = current._inputProfile._blockInput;
		}
		if (_keybindButton._inputReference == "Target")
		{
			_keybindButton._keyBind = current._inputProfile._targetInput;
		}
		if (_keybindButton._inputReference == "Interact")
		{
			_keybindButton._keyBind = current._inputProfile._interactInput;
		}
		if (_keybindButton._inputReference == "PvpFlag")
		{
			_keybindButton._keyBind = current._inputProfile._flagPvpInput;
		}
		if (_keybindButton._inputReference == "ResetCamera")
		{
			_keybindButton._keyBind = current._inputProfile._resetCameraInput;
		}
		if (_keybindButton._inputReference == "SkillSlot01")
		{
			_keybindButton._keyBind = current._inputProfile.actionBarInput_0;
		}
		if (_keybindButton._inputReference == "SkillSlot02")
		{
			_keybindButton._keyBind = current._inputProfile.actionBarInput_1;
		}
		if (_keybindButton._inputReference == "SkillSlot03")
		{
			_keybindButton._keyBind = current._inputProfile.actionBarInput_2;
		}
		if (_keybindButton._inputReference == "SkillSlot04")
		{
			_keybindButton._keyBind = current._inputProfile.actionBarInput_3;
		}
		if (_keybindButton._inputReference == "SkillSlot05")
		{
			_keybindButton._keyBind = current._inputProfile.actionBarInput_4;
		}
		if (_keybindButton._inputReference == "SkillSlot06")
		{
			_keybindButton._keyBind = current._inputProfile.actionBarInput_5;
		}
		if (_keybindButton._inputReference == "Recall")
		{
			_keybindButton._keyBind = current._inputProfile._recallInput;
		}
		if (_keybindButton._inputReference == "QuickSwapWeapon")
		{
			_keybindButton._keyBind = current._inputProfile._quickSwapWeaponInput;
		}
		if (_keybindButton._inputReference == "SheatheWeapon")
		{
			_keybindButton._keyBind = current._inputProfile._sheatheWeaponInput;
		}
		if (_keybindButton._inputReference == "Sit")
		{
			_keybindButton._keyBind = current._inputProfile._sitInput;
		}
		if (_keybindButton._inputReference == "QuickSlot01")
		{
			_keybindButton._keyBind = current._inputProfile._consumableSlot_0;
		}
		if (_keybindButton._inputReference == "QuickSlot02")
		{
			_keybindButton._keyBind = current._inputProfile._consumableSlot_1;
		}
		if (_keybindButton._inputReference == "QuickSlot03")
		{
			_keybindButton._keyBind = current._inputProfile._consumableSlot_2;
		}
		if (_keybindButton._inputReference == "QuickSlot04")
		{
			_keybindButton._keyBind = current._inputProfile._consumableSlot_3;
		}
		if (_keybindButton._inputReference == "QuickSlot05")
		{
			_keybindButton._keyBind = current._inputProfile._consumableSlot_4;
		}
		if (_keybindButton._inputReference == "Lexicon")
		{
			_keybindButton._keyBind = current._inputProfile._lexiconHotkeyInput;
		}
		if (_keybindButton._inputReference == "TabMenu")
		{
			_keybindButton._keyBind = current._inputProfile._tabMenuInput;
		}
		if (_keybindButton._inputReference == "StatsTab")
		{
			_keybindButton._keyBind = current._inputProfile._statMenuHotkeyInput;
		}
		if (_keybindButton._inputReference == "SkillsTab")
		{
			_keybindButton._keyBind = current._inputProfile._skillMenuHotkeyInput;
		}
		if (_keybindButton._inputReference == "ItemTab")
		{
			_keybindButton._keyBind = current._inputProfile._itemMenuHotkeyInput;
		}
		if (_keybindButton._inputReference == "QuestTab")
		{
			_keybindButton._keyBind = current._inputProfile._questMenuHotkeyInput;
		}
		if (_keybindButton._inputReference == "WhoTab")
		{
			_keybindButton._keyBind = current._inputProfile._whoMenuHotkeyInput;
		}
		if (_keybindButton._inputReference == "HideUI")
		{
			_keybindButton._keyBind = current._inputProfile._hideUIInput;
		}
		if (_keybindButton._inputReference == "HostConsole")
		{
			_keybindButton._keyBind = current._inputProfile._hostConsoleInput;
		}
	}

	private void OnGUI()
	{
		InputControlManager current = InputControlManager.current;
		_keyEvent = Event.current;
		if (!_waitingForKey || (!_keyEvent.isKey && !_keyEvent.isMouse))
		{
			return;
		}
		foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
		{
			if (Input.GetKeyDown(value))
			{
				_pressedKey = value;
			}
		}
		if (_keyEvent.keyCode != KeyCode.Escape && _keyEvent.keyCode != KeyCode.BackQuote && !_keyEvent.keyCode.ToString().Contains("Joystick"))
		{
			_currentKeyBindButton._keyBind = _pressedKey.ToString();
		}
		if (_keyEvent.keyCode == KeyCode.Escape || _pressedKey.ToString().Contains("Joystick"))
		{
			_currentKeyBindButton._keyBind = KeyCode.None.ToString();
		}
		KeyBindButton[] array = keyBindButtons;
		foreach (KeyBindButton keyBindButton in array)
		{
			if (keyBindButton != null)
			{
				keyBindButton._button.interactable = true;
			}
		}
		_currentKeyBindButton.waitForKeyPress = false;
		_currentKeyBindButton._buttonLabel.text = _currentKeyBindButton._keyBind;
		array = keyBindButtons;
		foreach (KeyBindButton keyBindButton2 in array)
		{
			if (keyBindButton2 != null)
			{
				if (keyBindButton2 != _currentKeyBindButton && keyBindButton2._keyBind == _currentKeyBindButton._keyBind)
				{
					keyBindButton2._keyBind = KeyCode.None.ToString();
				}
				Apply_KeyToProfile(keyBindButton2, keyBindButton2._inputReference);
			}
		}
		_currentKeyBindButton = null;
		_waitingForKey = false;
		current.SaveInputData();
		current.LoadInputData();
	}

	private void Apply_KeyToProfile(KeyBindButton _keybindButton, string _inputRef)
	{
		InputControlManager current = InputControlManager.current;
		if (_inputRef == "Up")
		{
			current._inputProfile._forwardInput = _keybindButton._keyBind;
		}
		if (_inputRef == "Down")
		{
			current._inputProfile._backwardInput = _keybindButton._keyBind;
		}
		if (_inputRef == "Left")
		{
			current._inputProfile._leftInput = _keybindButton._keyBind;
		}
		if (_inputRef == "Right")
		{
			current._inputProfile._rightInput = _keybindButton._keyBind;
		}
		if (_inputRef == "Jump")
		{
			current._inputProfile._jumpInput = _keybindButton._keyBind;
		}
		if (_inputRef == "Dash")
		{
			current._inputProfile._dashInput = _keybindButton._keyBind;
		}
		if (_inputRef == "LockDirection")
		{
			current._inputProfile._lockDirection = _keybindButton._keyBind;
		}
		if (_inputRef == "Attack")
		{
			current._inputProfile._attackInput = _keybindButton._keyBind;
		}
		if (_inputRef == "ChargeAttack")
		{
			current._inputProfile._chargeAttackInput = _keybindButton._keyBind;
		}
		if (_inputRef == "Block")
		{
			current._inputProfile._blockInput = _keybindButton._keyBind;
		}
		if (_inputRef == "Target")
		{
			current._inputProfile._targetInput = _keybindButton._keyBind;
		}
		if (_inputRef == "Interact")
		{
			current._inputProfile._interactInput = _keybindButton._keyBind;
		}
		if (_inputRef == "PvpFlag")
		{
			current._inputProfile._flagPvpInput = _keybindButton._keyBind;
		}
		if (_inputRef == "ResetCamera")
		{
			current._inputProfile._resetCameraInput = _keybindButton._keyBind;
		}
		if (_keybindButton._inputReference == "SkillSlot01")
		{
			current._inputProfile.actionBarInput_0 = _keybindButton._keyBind;
		}
		if (_keybindButton._inputReference == "SkillSlot02")
		{
			current._inputProfile.actionBarInput_1 = _keybindButton._keyBind;
		}
		if (_keybindButton._inputReference == "SkillSlot03")
		{
			current._inputProfile.actionBarInput_2 = _keybindButton._keyBind;
		}
		if (_keybindButton._inputReference == "SkillSlot04")
		{
			current._inputProfile.actionBarInput_3 = _keybindButton._keyBind;
		}
		if (_keybindButton._inputReference == "SkillSlot05")
		{
			current._inputProfile.actionBarInput_4 = _keybindButton._keyBind;
		}
		if (_keybindButton._inputReference == "SkillSlot06")
		{
			current._inputProfile.actionBarInput_5 = _keybindButton._keyBind;
		}
		if (_keybindButton._inputReference == "Recall")
		{
			current._inputProfile._recallInput = _keybindButton._keyBind;
		}
		if (_keybindButton._inputReference == "QuickSwapWeapon")
		{
			current._inputProfile._quickSwapWeaponInput = _keybindButton._keyBind;
		}
		if (_keybindButton._inputReference == "SheatheWeapon")
		{
			current._inputProfile._sheatheWeaponInput = _keybindButton._keyBind;
		}
		if (_keybindButton._inputReference == "Sit")
		{
			current._inputProfile._sitInput = _keybindButton._keyBind;
		}
		if (_inputRef == "QuickSlot01")
		{
			current._inputProfile._consumableSlot_0 = _keybindButton._keyBind;
		}
		if (_inputRef == "QuickSlot02")
		{
			current._inputProfile._consumableSlot_1 = _keybindButton._keyBind;
		}
		if (_inputRef == "QuickSlot03")
		{
			current._inputProfile._consumableSlot_2 = _keybindButton._keyBind;
		}
		if (_inputRef == "QuickSlot04")
		{
			current._inputProfile._consumableSlot_3 = _keybindButton._keyBind;
		}
		if (_inputRef == "QuickSlot05")
		{
			current._inputProfile._consumableSlot_4 = _keybindButton._keyBind;
		}
		if (_inputRef == "Lexicon")
		{
			current._inputProfile._lexiconHotkeyInput = _keybindButton._keyBind;
		}
		if (_inputRef == "TabMenu")
		{
			current._inputProfile._tabMenuInput = _keybindButton._keyBind;
		}
		if (_inputRef == "StatsTab")
		{
			current._inputProfile._statMenuHotkeyInput = _keybindButton._keyBind;
		}
		if (_inputRef == "SkillsTab")
		{
			current._inputProfile._skillMenuHotkeyInput = _keybindButton._keyBind;
		}
		if (_inputRef == "ItemTab")
		{
			current._inputProfile._itemMenuHotkeyInput = _keybindButton._keyBind;
		}
		if (_inputRef == "QuestTab")
		{
			current._inputProfile._questMenuHotkeyInput = _keybindButton._keyBind;
		}
		if (_inputRef == "WhoTab")
		{
			current._inputProfile._whoMenuHotkeyInput = _keybindButton._keyBind;
		}
		if (_inputRef == "HideUI")
		{
			current._inputProfile._hideUIInput = _keybindButton._keyBind;
		}
		if (_inputRef == "HostConsole")
		{
			current._inputProfile._hostConsoleInput = _keybindButton._keyBind;
		}
	}
}
