using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListManager : MonoBehaviour
{
	public static LobbyListManager _current;

	[SerializeField]
	private CanvasGroup _lobbyListCanvasGroup;

	[SerializeField]
	private CanvasGroup _lobbyHostCanvasGroup;

	[SerializeField]
	private CanvasGroup _lobbyPasswordCanvasGroup;

	public CSteamID _heldCSteamID;

	public float _gamepad_inputBuffer;

	public bool _selectedLobby;

	[Header("Disclaimer Popup")]
	[SerializeField]
	private MenuElement _disclaimerElement;

	[SerializeField]
	private Button _disclaimerAcceptButton;

	[SerializeField]
	private Toggle _toggle_turnOffDisclaimer;

	[SerializeField]
	private Image _gamepad_disclaimerIco_southButton;

	[SerializeField]
	private Image _gamepad_disclaimerIco_westButton;

	private bool _wasDisclaimed;

	public bool _inDisclaimerMode;

	[Header("Lobby List Entries")]
	[SerializeField]
	private ScrollRect _lobbyListScrollRect;

	[SerializeField]
	private GameObject _lobbyListContent;

	[SerializeField]
	private RectTransform _lobbyListContentRect;

	[Space]
	[SerializeField]
	private GameObject _lobbyListEntryPrefab;

	public List<LobbyDataEntry> _lobbyListEntries = new List<LobbyDataEntry>();

	[Header("Lobby List Window UI")]
	public MenuElement lobbyListWindow_element;

	[SerializeField]
	private Text _lobbySearchText;

	[SerializeField]
	private Dropdown dropDown_lobbyListFilter;

	[SerializeField]
	private Button _refreshButton;

	[SerializeField]
	private Button _cancelButton;

	[SerializeField]
	private RotateObject _refreshButtonRotaterDisplay;

	[SerializeField]
	private InputField _input_lobbyNameSearch;

	[Header("Lobby Password Window")]
	public MenuElement passwordWindow_element;

	public InputField _joinLobbyPasswordInput;

	[Space]
	[SerializeField]
	private Button _joinPasswordLobbyButton;

	[SerializeField]
	private Button _cancelPasswordLobbyButton;

	public bool _isPasswordInput;

	[Header("Gamepad Lobby Password Window")]
	[SerializeField]
	private Image _gamepad_confirmPasswordIco;

	[SerializeField]
	private Image _gamepad_cancelPasswordIco;

	[SerializeField]
	private Image _gamepad_inputPasswordIco;

	[Header("Gamepad Lobby Finder Window")]
	[SerializeField]
	private LobbyDataEntry _gamepad_selectedDataEntry;

	[SerializeField]
	private Image _gamepadIco_lobbyFinder_refreshList;

	[SerializeField]
	private Image _gamepadIco_lobbyFinder_return;

	[SerializeField]
	private Image _gamepadIco_lobbyFinder_inputField;

	[SerializeField]
	private Image _gamepadIco_lobbyFinder_clearHiddenLobbies;

	[Space]
	[SerializeField]
	private Image _gamepad_lobbyFinder_leftShoulder;

	[SerializeField]
	private Image _gamepad_lobbyFinder_rightShoulder;

	[Space]
	[SerializeField]
	private float _focusAdjust = 1.85f;

	[Space]
	private int _gamepad_currentLobbyCell;

	private SettingsCell[] _gamepad_foundLobbyCells = new SettingsCell[0];

	[Header("Lobby Hosting Window UI")]
	public AtlyssSteamLobbyTag _hostLobbyRealm;

	[SerializeField]
	private Image[] _lobbyRealmHostHighlights;

	[SerializeField]
	private Button[] _lobbyRealmHostButtons;

	[Space]
	[SerializeField]
	private Text _hostLobbyHeaderText;

	[SerializeField]
	private Text _lobbyTypeTooltipText;

	public MenuElement _lobbyHostWindow_element;

	public Button _cancelHostButton;

	public Button _hostLobbyButton;

	[Space]
	public InputField _lobbyNameInput;

	public InputField _lobbyPasswordInput;

	public InputField _lobbyMotdInput;

	public Dropdown _lobbyTypeDropdown;

	public Slider _lobbyMaxConnectionSlider;

	public Text _lobbyMaxConnectionCounter;

	public Toggle _streamModeToggle;

	[Header("Gamepad Host Window")]
	[SerializeField]
	private Image _gamepadIco_hostWindow_hostLobby;

	[SerializeField]
	private Image _gamepadIco_hostWindow_return;

	[Space]
	[SerializeField]
	private Image _gamepadIco_hostWindow_leftShoulder;

	[SerializeField]
	private Image _gamepadIco_hostWindow_rightShoulder;

	private int _gamepad_currentHostCell;

	private SettingsCell[] _gamepad_hostSettingCells = new SettingsCell[0];

	[Header("Blacklist / Filter Lobbies")]
	[SerializeField]
	private AtlyssSteamLobbyTag _filterLobbyRealm;

	[SerializeField]
	private Image[] _lobbyRealmFilterHighlights;

	[SerializeField]
	private Button[] _lobbyRealmFilterButtons;

	[Space]
	[SerializeField]
	private MenuElement _hiddenLobbyCounterPanel;

	[SerializeField]
	private Text _hiddenLobbyCounter;

	[SerializeField]
	private Button _button_clearHiddenLobbies;

	public List<string> _lobbyBlacklist = new List<string>();

	public Dictionary<string, string> _lobbyIDblacklist = new Dictionary<string, string>();

	[Space]
	public string[] _parseGaiaKeywords;

	public string[] _parseDualosKeywords;

	public string[] _parseNothKeywords;

	public string[] _parseLoodiaKeywords;

	[Space]
	public bool _hideModdedLobbies;

	private bool _refeshingList;

	public int Gamepad_CurrentLobbyCell
	{
		get
		{
			return _gamepad_currentLobbyCell;
		}
		set
		{
			if (value < 0)
			{
				value = _gamepad_foundLobbyCells.Length - 1;
			}
			if (value > _gamepad_foundLobbyCells.Length - 1)
			{
				value = 0;
			}
			_gamepad_currentLobbyCell = value;
		}
	}

	public int Gamepad_CurrentHostCell
	{
		get
		{
			return _gamepad_currentHostCell;
		}
		set
		{
			if (value < 0)
			{
				value = _gamepad_hostSettingCells.Length - 1;
			}
			if (value > _gamepad_hostSettingCells.Length - 1)
			{
				value = 0;
			}
			_gamepad_currentHostCell = value;
		}
	}

	private void Awake()
	{
		if (_current == null)
		{
			_current = this;
		}
		_gamepad_hostSettingCells = _lobbyHostWindow_element.GetComponentsInChildren<SettingsCell>();
	}

	private void Start()
	{
		_lobbySearchText.text = string.Empty;
		dropDown_lobbyListFilter.value = 3;
		Init_ApplyButtons();
	}

	private void Init_ApplyButtons()
	{
		_cancelButton.onClick.AddListener(Disable_LobbyListFinder);
		_refreshButton.onClick.AddListener(Init_RefreshLobbyList);
		_cancelHostButton.onClick.AddListener(Disable_LobbyHostWindow);
		_hostLobbyButton.onClick.AddListener(Init_HostSteamLobby);
		_joinPasswordLobbyButton.onClick.AddListener(Join_PasswordLobby);
		_cancelPasswordLobbyButton.onClick.AddListener(Cancel_JoinPasswordLobby);
		_button_clearHiddenLobbies.onClick.AddListener(Init_ClearHiddenLobbyCache);
		_disclaimerAcceptButton.onClick.AddListener(Confirm_Disclaimer);
		_lobbyRealmFilterButtons[0].onClick.AddListener(() =>
		{
			Init_LobbyRealmFilter(AtlyssSteamLobbyTag.GAIA);
		});
		_lobbyRealmFilterButtons[1].onClick.AddListener(() =>
		{
			Init_LobbyRealmFilter(AtlyssSteamLobbyTag.DUALOS);
		});
		_lobbyRealmFilterButtons[2].onClick.AddListener(() =>
		{
			Init_LobbyRealmFilter(AtlyssSteamLobbyTag.NOTH);
		});
		_lobbyRealmFilterButtons[3].onClick.AddListener(() =>
		{
			Init_LobbyRealmFilter(AtlyssSteamLobbyTag.LOODIA);
		});
		_lobbyRealmHostButtons[0].onClick.AddListener(() =>
		{
			Init_LobbyRealmHost(AtlyssSteamLobbyTag.GAIA);
		});
		_lobbyRealmHostButtons[1].onClick.AddListener(() =>
		{
			Init_LobbyRealmHost(AtlyssSteamLobbyTag.DUALOS);
		});
		_lobbyRealmHostButtons[2].onClick.AddListener(() =>
		{
			Init_LobbyRealmHost(AtlyssSteamLobbyTag.NOTH);
		});
		_lobbyRealmHostButtons[3].onClick.AddListener(() =>
		{
			Init_LobbyRealmHost(AtlyssSteamLobbyTag.LOODIA);
		});
	}

	private void Init_LobbyRealmFilter(AtlyssSteamLobbyTag _lobbyTag)
	{
		if (_filterLobbyRealm != _lobbyTag)
		{
			_filterLobbyRealm = _lobbyTag;
			Init_RefreshLobbyList();
		}
	}

	private void Init_LobbyRealmHost(AtlyssSteamLobbyTag _lobbyTag)
	{
		_hostLobbyRealm = _lobbyTag;
	}

	private void Init_ClearHiddenLobbyCache()
	{
		_lobbyBlacklist.Clear();
		ProfileDataManager._current.Save_HostSettingsData();
		HostConsole._current.Init_NetworkStatusMessage("Cleared hidden lobby cache...");
		Init_RefreshLobbyList();
	}

	private void Update()
	{
		if (_gamepad_inputBuffer > 0f)
		{
			_gamepad_inputBuffer -= Time.deltaTime;
		}
		else
		{
			_gamepad_inputBuffer = 0f;
		}
		_lobbyListCanvasGroup.transform.parent.gameObject.SetActive(!NetworkClient.active);
		_lobbyHostCanvasGroup.gameObject.SetActive(!NetworkClient.active);
		_hiddenLobbyCounter.text = $"Hidden Lobbies: {_lobbyBlacklist.Count}";
		_hiddenLobbyCounterPanel.isEnabled = !NetworkClient.active && SteamManager.Initialized && _lobbyBlacklist.Count > 0;
		_button_clearHiddenLobbies.interactable = _hiddenLobbyCounterPanel.isEnabled;
		_disclaimerAcceptButton.gameObject.SetActive(_inDisclaimerMode);
		_toggle_turnOffDisclaimer.gameObject.SetActive(_inDisclaimerMode);
		for (int i = 0; i < _lobbyRealmFilterHighlights.Length; i++)
		{
			if (i == (int)_filterLobbyRealm)
			{
				_lobbyRealmFilterHighlights[i].enabled = true;
			}
			else
			{
				_lobbyRealmFilterHighlights[i].enabled = false;
			}
		}
		for (int j = 0; j < _lobbyRealmHostHighlights.Length; j++)
		{
			if (j == (int)_hostLobbyRealm)
			{
				_lobbyRealmHostHighlights[j].enabled = true;
			}
			else
			{
				_lobbyRealmHostHighlights[j].enabled = false;
			}
		}
		if (NetworkClient.active)
		{
			Clear_Lobbylist();
			return;
		}
		if (_input_lobbyNameSearch.text != string.Empty && Input.GetKeyDown(KeyCode.Return))
		{
			Init_RefreshLobbyList();
		}
		_lobbyListCanvasGroup.interactable = lobbyListWindow_element.isEnabled;
		_lobbyListCanvasGroup.blocksRaycasts = lobbyListWindow_element.isEnabled;
		_lobbyHostCanvasGroup.interactable = _lobbyHostWindow_element.isEnabled;
		_lobbyHostCanvasGroup.blocksRaycasts = _lobbyHostWindow_element.isEnabled;
		passwordWindow_element.isEnabled = _isPasswordInput;
		_joinPasswordLobbyButton.interactable = _isPasswordInput;
		_lobbyPasswordCanvasGroup.interactable = _isPasswordInput;
		_lobbyPasswordCanvasGroup.blocksRaycasts = _isPasswordInput;
		_lobbyMaxConnectionCounter.text = $"{_lobbyMaxConnectionSlider.value}";
		if (_isPasswordInput)
		{
			lobbyListWindow_element.isEnabled = false;
		}
		if (_inDisclaimerMode)
		{
			lobbyListWindow_element.isEnabled = false;
		}
		Handle_HostingParameters();
		Handle_GamepadControl();
	}

	private void Handle_GamepadControl()
	{
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_gamepadIco_hostWindow_leftShoulder.enabled = flag && !_isPasswordInput;
		_gamepadIco_hostWindow_rightShoulder.enabled = flag && !_isPasswordInput;
		_gamepadIco_hostWindow_hostLobby.enabled = flag && !_isPasswordInput;
		_gamepad_lobbyFinder_leftShoulder.enabled = flag && !_isPasswordInput;
		_gamepad_lobbyFinder_rightShoulder.enabled = flag && !_isPasswordInput;
		_gamepadIco_lobbyFinder_refreshList.enabled = flag && !_isPasswordInput;
		_gamepadIco_hostWindow_return.enabled = flag;
		_gamepadIco_lobbyFinder_return.enabled = flag && !_isPasswordInput;
		_gamepad_disclaimerIco_southButton.enabled = flag;
		_gamepad_disclaimerIco_westButton.enabled = flag;
		_gamepadIco_lobbyFinder_inputField.enabled = flag && !_isPasswordInput;
		_gamepadIco_lobbyFinder_clearHiddenLobbies.enabled = flag && !_isPasswordInput;
		_gamepad_confirmPasswordIco.enabled = flag;
		_gamepad_cancelPasswordIco.enabled = flag;
		_gamepad_inputPasswordIco.enabled = flag;
		for (int i = 0; i < _gamepad_hostSettingCells.Length; i++)
		{
			_gamepad_hostSettingCells[i]._isSelected = flag && _lobbyHostWindow_element.isEnabled && _gamepad_currentHostCell == i;
		}
		for (int j = 0; j < _gamepad_foundLobbyCells.Length; j++)
		{
			_gamepad_foundLobbyCells[j]._isSelected = flag && lobbyListWindow_element.isEnabled && _gamepad_currentLobbyCell == j;
		}
		if (!flag || MainMenuManager._current._characterSelectManager._isSendingCharacterFile)
		{
			return;
		}
		if (_isPasswordInput)
		{
			if (!(_gamepad_inputBuffer <= 0f))
			{
				return;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Start))
			{
				_joinLobbyPasswordInput.ActivateInputField();
				if (SteamManager.Initialized && (SteamUtils.IsSteamRunningOnSteamDeck() || SteamUtils.IsSteamInBigPictureMode()))
				{
					SteamUtils.ShowFloatingGamepadTextInput(EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine, 0, 0, 500, 200);
				}
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_South))
			{
				Join_PasswordLobby();
				MainMenuManager._current._menuAcceptAsrc.Play();
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
			{
				Cancel_JoinPasswordLobby();
				MainMenuManager._current._menuDeclineAsrc.Play();
			}
		}
		else
		{
			if (_selectedLobby)
			{
				return;
			}
			for (int k = 0; k < _gamepad_foundLobbyCells.Length; k++)
			{
				if (_refeshingList)
				{
					break;
				}
				if ((bool)_gamepad_foundLobbyCells[k] && _gamepad_foundLobbyCells[k]._isSelected)
				{
					_gamepad_selectedDataEntry = _gamepad_foundLobbyCells[k].GetComponent<LobbyDataEntry>();
					if (_lobbyListEntries.Count > 3 && InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
					{
						Canvas.ForceUpdateCanvases();
						_lobbyListContentRect.pivot = new Vector2(0.5f, 0f);
						Vector2 b = (Vector2)_lobbyListScrollRect.transform.InverseTransformPoint(_lobbyListContentRect.position) - (Vector2)_lobbyListScrollRect.transform.InverseTransformPoint(_gamepad_foundLobbyCells[k].transform.position);
						_lobbyListContentRect.anchoredPosition = Vector2.Lerp(_lobbyListContentRect.anchoredPosition, b, 12.5f * Time.deltaTime);
					}
					else
					{
						Canvas.ForceUpdateCanvases();
						_lobbyListContentRect.pivot = new Vector2(0.5f, 1f);
					}
				}
			}
			bool flag2 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Up) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Up) || InputControlManager.current.DpadUp(DpadHoldType.OnDown);
			bool flag3 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Down) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Down) || InputControlManager.current.DpadDown(DpadHoldType.OnDown);
			if (_inDisclaimerMode && _gamepad_inputBuffer <= 0f)
			{
				if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && SteamManager.Initialized)
				{
					Confirm_Disclaimer();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_West))
				{
					_toggle_turnOffDisclaimer.isOn = !_toggle_turnOffDisclaimer.isOn;
				}
			}
			if (_lobbyHostWindow_element.isEnabled)
			{
				if (flag2)
				{
					Gamepad_CurrentHostCell--;
					MainMenuManager._current._gamepadSelectAsrc_01.Play();
				}
				if (flag3)
				{
					Gamepad_CurrentHostCell++;
					MainMenuManager._current._gamepadSelectAsrc_01.Play();
				}
				if (GamepadInput.GetButton(GamepadButton.LeftShoulder))
				{
					_gamepadIco_hostWindow_leftShoulder.color = Color.white;
				}
				else
				{
					_gamepadIco_hostWindow_leftShoulder.color = Color.grey;
				}
				if (GamepadInput.GetButton(GamepadButton.RightShoulder))
				{
					_gamepadIco_hostWindow_rightShoulder.color = Color.white;
				}
				else
				{
					_gamepadIco_hostWindow_rightShoulder.color = Color.grey;
				}
				if (GamepadInput.GetButtonDown(GamepadButton.LeftShoulder) && _hostLobbyRealm != AtlyssSteamLobbyTag.GAIA)
				{
					Init_LobbyRealmHost(_hostLobbyRealm - 1);
					MainMenuManager._current._gamepadSelectAsrc_01.Play();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.RightShoulder) && _hostLobbyRealm != AtlyssSteamLobbyTag.LOODIA)
				{
					Init_LobbyRealmHost(_hostLobbyRealm + 1);
					MainMenuManager._current._gamepadSelectAsrc_01.Play();
				}
			}
			else
			{
				_gamepad_currentHostCell = 0;
			}
			if (!lobbyListWindow_element.isEnabled)
			{
				return;
			}
			if (flag2)
			{
				Gamepad_CurrentLobbyCell--;
				MainMenuManager._current._gamepadSelectAsrc_01.Play();
				_input_lobbyNameSearch.DeactivateInputField();
			}
			if (flag3)
			{
				Gamepad_CurrentLobbyCell++;
				MainMenuManager._current._gamepadSelectAsrc_01.Play();
				_input_lobbyNameSearch.DeactivateInputField();
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
			{
				MainMenuManager._current._menuDeclineAsrc.Play();
				_input_lobbyNameSearch.DeactivateInputField();
				Disable_LobbyListFinder();
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_West) && _refreshButton.interactable)
			{
				_refreshButton.onClick.Invoke();
				_input_lobbyNameSearch.DeactivateInputField();
				MainMenuManager._current._gamepadSelectAsrc.Play();
			}
			if (GamepadInput.GetButton(GamepadButton.LeftShoulder))
			{
				_gamepad_lobbyFinder_leftShoulder.color = Color.white;
			}
			else
			{
				_gamepad_lobbyFinder_leftShoulder.color = Color.grey;
			}
			if (GamepadInput.GetButton(GamepadButton.RightShoulder))
			{
				_gamepad_lobbyFinder_rightShoulder.color = Color.white;
			}
			else
			{
				_gamepad_lobbyFinder_rightShoulder.color = Color.grey;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Start))
			{
				_input_lobbyNameSearch.ActivateInputField();
				if (SteamManager.Initialized && (SteamUtils.IsSteamRunningOnSteamDeck() || SteamUtils.IsSteamInBigPictureMode()))
				{
					SteamUtils.ShowFloatingGamepadTextInput(EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine, 0, 0, 500, 200);
				}
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Back) && _lobbyBlacklist.Count > 0)
			{
				Init_ClearHiddenLobbyCache();
				MainMenuManager._current._gamepadSelectAsrc_01.Play();
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_South))
			{
				if ((bool)_gamepad_selectedDataEntry && _gamepad_selectedDataEntry._joinLobbyButton.interactable)
				{
					_gamepad_selectedDataEntry._joinLobbyButton.onClick.Invoke();
				}
				_input_lobbyNameSearch.DeactivateInputField();
				MainMenuManager._current._gamepadSelectAsrc_01.Play();
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_North))
			{
				if ((bool)_gamepad_selectedDataEntry)
				{
					_gamepad_selectedDataEntry._blacklistLobbyButton.onClick.Invoke();
				}
				_input_lobbyNameSearch.DeactivateInputField();
				MainMenuManager._current._gamepadSelectAsrc_01.Play();
			}
			if (_refreshButton.interactable)
			{
				if (GamepadInput.GetButtonDown(GamepadButton.LeftShoulder) && _filterLobbyRealm != AtlyssSteamLobbyTag.GAIA)
				{
					Init_LobbyRealmFilter(_filterLobbyRealm - 1);
					_input_lobbyNameSearch.DeactivateInputField();
					MainMenuManager._current._gamepadSelectAsrc_01.Play();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.RightShoulder) && _filterLobbyRealm != AtlyssSteamLobbyTag.LOODIA)
				{
					Init_LobbyRealmFilter(_filterLobbyRealm + 1);
					_input_lobbyNameSearch.DeactivateInputField();
					MainMenuManager._current._gamepadSelectAsrc_01.Play();
				}
			}
		}
	}

	private void Handle_HostingParameters()
	{
		if (!_lobbyHostWindow_element.isEnabled)
		{
			return;
		}
		if (!SteamManager.Initialized)
		{
			_hostLobbyHeaderText.text = "Steam is not Initialized.";
		}
		else
		{
			_hostLobbyHeaderText.text = "Host Lobby";
		}
		switch (_lobbyTypeDropdown.value)
		{
		case 0:
			_lobbyTypeTooltipText.text = "Public lobbies will be advertised for anyone to join.";
			break;
		case 1:
			_lobbyTypeTooltipText.text = "Friend lobbies are joinable by invite or from the Steam friends list.";
			break;
		case 2:
			_lobbyTypeTooltipText.text = "Private lobbies are by invitation from Host only.";
			break;
		}
		_hostLobbyButton.interactable = SteamManager.Initialized;
		if (_hostLobbyButton.interactable)
		{
			_hostLobbyButton.GetComponentInChildren<Text>().color = Color.white;
		}
		else
		{
			_hostLobbyButton.GetComponentInChildren<Text>().color = Color.grey;
		}
		if (!(MainMenuManager._current._menuInputBuffer > 0f) && InputControlManager.current._setGamepadType != GamepadType.Keyboard && _gamepad_inputBuffer <= 0f && !_inDisclaimerMode)
		{
			if (GamepadInput.GetButtonDown(GamepadButton.Start) && SteamManager.Initialized)
			{
				Init_HostSteamLobby();
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
			{
				Disable_LobbyHostWindow();
			}
		}
	}

	private void Confirm_Disclaimer()
	{
		if (_toggle_turnOffDisclaimer.isOn)
		{
			SettingsManager._current._dontShowDisclaimer = true;
			SettingsManager._current.Save_SettingsData();
		}
		_disclaimerElement.isEnabled = false;
		_wasDisclaimed = true;
		_inDisclaimerMode = false;
		_gamepad_inputBuffer = 0.2f;
		MainMenuManager._current._menuAcceptAsrc.Play();
		if (MainMenuManager._current._networkInitCondition == NetworkInitCondition.Join_Multiplayer)
		{
			lobbyListWindow_element.isEnabled = true;
		}
		if (MainMenuManager._current._mainMenuCondition == MainMenuCondition.ServerMenu)
		{
			Enable_LobbyHostWindow();
		}
	}

	public void Init_RefreshLobbyList()
	{
		ProfileDataManager._current.Load_HostSettingsData();
		_gamepad_currentLobbyCell = 0;
		if (!SteamManager.Initialized)
		{
			_lobbySearchText.text = "Steam is not initialized.";
			_refreshButton.GetComponentInChildren<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
			for (int i = 0; i < _lobbyRealmFilterButtons.Length; i++)
			{
				_lobbyRealmFilterButtons[i].interactable = false;
			}
			_refreshButton.interactable = false;
			dropDown_lobbyListFilter.interactable = false;
			_input_lobbyNameSearch.interactable = false;
			return;
		}
		SteamFriends.SetRichPresence("steam_display", "#Status_InLobbyFinder");
		if (_lobbyListEntries.Count > 0)
		{
			Clear_Lobbylist();
		}
		if (!_wasDisclaimed && !SettingsManager._current._dontShowDisclaimer)
		{
			MainMenuManager._current._menuDeclineAsrc.Play();
			_disclaimerElement.isEnabled = true;
			_wasDisclaimed = true;
			_inDisclaimerMode = true;
		}
		_refreshButtonRotaterDisplay.Alter_RotationValues(new Vector3(0f, 0f, -125.5f));
		_lobbySearchText.text = "Searching...";
		Apply_SearchLobbyFilters();
		SteamLobby._current.GetLobbiesList();
		StopAllCoroutines();
		StartCoroutine(RefreshButtonBuffer());
		void Apply_SearchLobbyFilters()
		{
			if (!string.IsNullOrWhiteSpace(_input_lobbyNameSearch.text))
			{
				SteamMatchmaking.AddRequestLobbyListStringFilter("name", _input_lobbyNameSearch.text, ELobbyComparison.k_ELobbyComparisonEqual);
				_input_lobbyNameSearch.text = string.Empty;
			}
			SteamMatchmaking.AddRequestLobbyListStringFilter("isReady", "1", ELobbyComparison.k_ELobbyComparisonEqual);
			SteamMatchmaking.AddRequestLobbyListStringFilter("lobbyFocus", $"{(int)_filterLobbyRealm}", ELobbyComparison.k_ELobbyComparisonEqual);
			SteamMatchmaking.AddRequestLobbyListStringFilter("ver", Application.version ?? "", ELobbyComparison.k_ELobbyComparisonEqual);
			for (int j = 0; j < _lobbyBlacklist.Count; j++)
			{
				SteamMatchmaking.AddRequestLobbyListStringFilter("name", _lobbyBlacklist[j] ?? "", ELobbyComparison.k_ELobbyComparisonNotEqual);
			}
			if (_hideModdedLobbies)
			{
				SteamMatchmaking.AddRequestLobbyListStringFilter("mod", "1", ELobbyComparison.k_ELobbyComparisonNotEqual);
			}
			foreach (KeyValuePair<string, string> item in _lobbyIDblacklist)
			{
				SteamMatchmaking.AddRequestLobbyListStringFilter("HostAddress", $"{item}", ELobbyComparison.k_ELobbyComparisonNotEqual);
			}
			switch (dropDown_lobbyListFilter.value)
			{
			case 0:
				SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterClose);
				break;
			case 1:
				SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterFar);
				break;
			case 2:
				SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);
				break;
			}
		}
	}

	private IEnumerator RefreshButtonBuffer()
	{
		_refeshingList = true;
		_refreshButton.interactable = false;
		for (int i = 0; i < _lobbyRealmFilterButtons.Length; i++)
		{
			_lobbyRealmFilterButtons[i].interactable = false;
		}
		_refreshButton.GetComponentInChildren<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
		yield return new WaitForSeconds(1f);
		if (_lobbyListEntries.Count <= 0)
		{
			_lobbySearchText.text = "No lobbies found.";
		}
		_refreshButton.interactable = true;
		for (int j = 0; j < _lobbyRealmFilterButtons.Length; j++)
		{
			_lobbyRealmFilterButtons[j].interactable = true;
		}
		for (int k = 0; k < _lobbyListEntries.Count; k++)
		{
			_lobbyListEntries[k].GetComponent<MenuElement>().isEnabled = true;
		}
		_refreshButton.GetComponentInChildren<Image>().color = Color.white;
		_refeshingList = false;
	}

	public void Clear_Lobbylist()
	{
		if (_lobbyListEntries.Count > 0)
		{
			for (int i = 0; i < _lobbyListEntries.Count; i++)
			{
				Object.Destroy(_lobbyListEntries[i].gameObject);
			}
			_lobbyListEntries.Clear();
			_lobbySearchText.text = string.Empty;
			StopAllCoroutines();
		}
	}

	public void Init_SteamLobbyIteration(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
	{
		Iterate_SteamLobbies(lobbyIDs, result);
	}

	private void Iterate_SteamLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
	{
		int _hiddenLobbies = 0;
		for (int i = 0; i < lobbyIDs.Count; i++)
		{
			if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
			{
				Iterate_LobbyEntry(lobbyIDs[i].m_SteamID);
			}
		}
		int count = _lobbyListEntries.Count;
		count -= _hiddenLobbies;
		if (count <= 1)
		{
			_lobbySearchText.text = $"{_lobbyListEntries.Count} lobby found.";
		}
		else
		{
			_lobbySearchText.text = $"{count} lobbies found.";
		}
		_gamepad_foundLobbyCells = _lobbyListContent.GetComponentsInChildren<SettingsCell>();
		Vector2 anchoredPosition = new Vector2(-7.25f, 0f);
		_lobbyListContentRect.anchoredPosition = anchoredPosition;
		if (_lobbyListEntries.Count < 4)
		{
			Canvas.ForceUpdateCanvases();
			_lobbyListContentRect.pivot = new Vector2(0.5f, 1f);
		}
		else
		{
			Canvas.ForceUpdateCanvases();
			_lobbyListContentRect.pivot = new Vector2(0.5f, 0f);
			_lobbyListScrollRect.verticalNormalizedPosition = 1f;
		}
		_gamepad_inputBuffer = 2.85f;
		void Iterate_LobbyEntry(ulong _lobbyID)
		{
			string text = SteamMatchmaking.GetLobbyData((CSteamID)_lobbyID, "name");
			bool _invalidLobby = text.Length > 20 || string.IsNullOrWhiteSpace(text) || text.Contains('<') || text.Contains('>');
			if (text.Length > 20)
			{
				text = "Null";
			}
			LobbyDataEntry lobEntry;
			if (text == "Null")
			{
				_lobbyIDblacklist.Add(text ?? "", SteamMatchmaking.GetLobbyData((CSteamID)_lobbyID, "HostAddress"));
				_hiddenLobbies++;
			}
			else
			{
				GameObject gameObject = Object.Instantiate(_lobbyListEntryPrefab);
				lobEntry = gameObject.GetComponent<LobbyDataEntry>();
				lobEntry.lobbyID = (CSteamID)_lobbyID;
				lobEntry._lobbyName = text;
				lobEntry._lobbyVersion = SteamMatchmaking.GetLobbyData((CSteamID)_lobbyID, "ver");
				int.TryParse(SteamMatchmaking.GetLobbyData((CSteamID)_lobbyID, "lobbyFocus"), out var result2);
				lobEntry._atlyssSteamLobbyTag = (AtlyssSteamLobbyTag)result2;
				int.TryParse(SteamMatchmaking.GetLobbyData((CSteamID)_lobbyID, "mod"), out var result3);
				lobEntry._moddedLobby = result3 == 1;
				lobEntry._lobbyPing = SteamMatchmaking.GetLobbyData((CSteamID)_lobbyID, "pingLocation");
				SteamNetworkingUtils.ParsePingLocationString(lobEntry._lobbyPing, out var result4);
				lobEntry._lobbyPing = SteamNetworkingUtils.EstimatePingTimeFromLocalHost(ref result4).ToString();
				int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)_lobbyID);
				int lobbyMemberLimit = SteamMatchmaking.GetLobbyMemberLimit((CSteamID)_lobbyID);
				lobEntry._lobbyPlayerCount = numLobbyMembers.ToString();
				lobEntry._lobbyMaxPlayers = lobbyMemberLimit.ToString();
				if (numLobbyMembers >= lobbyMemberLimit)
				{
					lobEntry._joinLobbyButton.interactable = false;
					lobEntry._joinLobbyButtonText.text = "Lobby Full";
				}
				if (lobbyMemberLimit > 200)
				{
					_invalidLobby = true;
				}
				ulong.TryParse(SteamMatchmaking.GetLobbyData(lobEntry.lobbyID, "HostAddress"), out var result5);
				if (!SteamFriends.HasFriend(new CSteamID(result5), EFriendFlags.k_EFriendFlagImmediate))
				{
					if (SteamMatchmaking.GetLobbyData((CSteamID)_lobbyID, "password") == "1")
					{
						lobEntry._joinLobbyButtonText.text = "Passworded";
						lobEntry.passwordLobby = true;
					}
				}
				else
				{
					lobEntry._joinLobbyButtonText.text = "Join (Friend)";
				}
				if (Application.version != lobEntry._lobbyVersion)
				{
					lobEntry._joinLobbyButtonText.text = "Different Version";
				}
				if (_invalidLobby)
				{
					lobEntry._joinLobbyButtonText.text = "Invalid";
				}
				gameObject.transform.SetParent(_lobbyListContent.transform);
				gameObject.transform.localScale = Vector3.one;
				lobEntry._joinLobbyButton.interactable = _joinButtonInteractable();
				if (_joinButtonInteractable())
				{
					lobEntry._joinLobbyButtonText.color = Color.white;
				}
				else
				{
					lobEntry._joinLobbyButtonText.color = Color.grey;
				}
				lobEntry.SetLobbyData();
				_lobbyListEntries.Add(lobEntry);
			}
			bool _joinButtonInteractable()
			{
				if (lobEntry._lobbyVersion != Application.version)
				{
					return false;
				}
				if (lobEntry._lobbyPlayerCount == lobEntry._lobbyMaxPlayers)
				{
					return false;
				}
				if (lobEntry._lobbyName.Length > 20)
				{
					return false;
				}
				if (_invalidLobby)
				{
					return false;
				}
				return true;
			}
		}
	}

	private void Disable_LobbyListFinder()
	{
		lobbyListWindow_element.isEnabled = false;
		MainMenuManager._current._menuInputBuffer = 0.2f;
		MainMenuManager._current._characterSelectManager._characterSelectCanvasObject.SetActive(value: true);
		SteamFriends.SetRichPresence("steam_display", "#Status_InCharacterSelection");
	}

	public void Enable_LobbyHostWindow()
	{
		_lobbyHostWindow_element.isEnabled = true;
		if (!_wasDisclaimed && !SettingsManager._current._dontShowDisclaimer)
		{
			MainMenuManager._current._menuDeclineAsrc.Play();
			_disclaimerElement.isEnabled = true;
			_wasDisclaimed = true;
			_inDisclaimerMode = true;
		}
		MainMenuManager._current.Set_MenuCondition(3);
	}

	public void Disable_LobbyHostWindow()
	{
		_lobbyHostWindow_element.isEnabled = false;
		MainMenuManager._current.Save_HostSettingsToProfile();
		MainMenuManager._current.Set_MenuCondition(0);
	}

	private void Init_HostSteamLobby()
	{
		_lobbyHostWindow_element.isEnabled = false;
		MainMenuManager._current._menuInputBuffer = 0.8f;
		MainMenuManager._current.Save_HostSettingsToProfile();
		MainMenuManager._current.Set_MenuCondition(1);
		MainMenuManager._current.Set_HostMode(1);
	}

	private void Join_PasswordLobby()
	{
		_ = _heldCSteamID;
		if (_joinLobbyPasswordInput.text == string.Empty)
		{
			HostConsole._current.Init_NetworkStatusMessage("Input password");
			return;
		}
		AtlyssNetworkManager._current._sentPassword = _joinLobbyPasswordInput.text;
		SteamLobby._current.JoinLobby(_heldCSteamID);
		_selectedLobby = true;
		_joinLobbyPasswordInput.text = string.Empty;
		_isPasswordInput = false;
	}

	private void Cancel_JoinPasswordLobby()
	{
		_isPasswordInput = false;
		_joinLobbyPasswordInput.text = string.Empty;
		lobbyListWindow_element.isEnabled = true;
	}
}
