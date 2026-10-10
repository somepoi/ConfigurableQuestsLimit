using System.Collections;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	public static MainMenuManager _current;

	public CharacterSelectManager _characterSelectManager;

	public CharacterCreationManager _characterCreationManager;

	[Space]
	[Header("Main Menu Management")]
	public MainMenuCondition _mainMenuCondition;

	public NetworkInitCondition _networkInitCondition;

	[SerializeField]
	private Camera _mainMenuCamera;

	[SerializeField]
	private Text _versionDisplayText;

	public float _menuInputBuffer;

	[Space]
	[SerializeField]
	private RectTransform _mainMenu_gamepadCursorIco;

	[SerializeField]
	private Button _singleplayerButton;

	[SerializeField]
	private Button _multiplayerButton;

	[SerializeField]
	private Button _settingsButton;

	[SerializeField]
	private Button _quitGameButton;

	[Space]
	[SerializeField]
	private MenuElement _gamepadDetectPrompt;

	[SerializeField]
	private Button _gamepadDetectButton_gamepadMode;

	[SerializeField]
	private Button _gamepadDetectButton_keyboardMode;

	[Header("Steamworks")]
	[SerializeField]
	private MenuElement _steamProfileDisplayElement;

	[SerializeField]
	private RawImage _steamAvatar_icon;

	[SerializeField]
	private Text _steamProfile_text;

	[Header("Multiplayer Menus")]
	[SerializeField]
	private MenuElement _multiplayerSelectionMenu;

	[SerializeField]
	private Button _multiplayer_joinOptionButton;

	[SerializeField]
	private Button _multiplayer_hostOptionButton;

	[SerializeField]
	private Button _multiplayer_returnButton;

	[SerializeField]
	private RectTransform _multiplayerMenu_gamepadCursorIco;

	private bool _inMultiplayerMenu;

	[Header("UI Canvas Objects")]
	[SerializeField]
	private GameObject _mainMenuParentObject;

	[SerializeField]
	private GameObject _mainMenuCanvasObject;

	[SerializeField]
	private GameObject _mainMenuSelectionBarObject;

	[Space]
	public GameObject _inGameUIObject;

	public GameObject _inGameMenuUIObject;

	[Space]
	public GameObject _characterMenuParentObject;

	public GameObject _characterSelectUIObject;

	public GameObject _characterCreationUIObject;

	[Header("Effects")]
	[SerializeField]
	private Color _mainMenuFogColor = Color.white;

	[SerializeField]
	private Color _mainmenuAmbientColor = Color.white;

	[SerializeField]
	private float _mainMenuFogDensity = 0.025f;

	[Space]
	[SerializeField]
	private Color _characterSelectFogColor = Color.white;

	[SerializeField]
	private Color _characterSelectAmbientColor = Color.white;

	[SerializeField]
	private float _characterselectFogDensity = 0.025f;

	[Header("Audio / Music")]
	public AudioSource _menuFlyByAsrc;

	public AudioSource _characterSelectAsrc;

	public AudioSource _characterCreateAsrc;

	public AudioSource _menuAcceptAsrc;

	public AudioSource _menuDeclineAsrc;

	public AudioSource _gamepadSelectAsrc;

	public AudioSource _gamepadSelectAsrc_01;

	[Header("Gamepad")]
	private int _mainMenuSelection;

	private int _multiplayerMenuSelection;

	private bool _gamepadIsDetected;

	public int MainMenuSelection
	{
		get
		{
			return _mainMenuSelection;
		}
		set
		{
			if (value > 3)
			{
				value = 0;
			}
			else if (value < 0)
			{
				value = 3;
			}
			if (_mainMenuSelection != value)
			{
				_gamepadSelectAsrc.Play();
			}
			_mainMenuSelection = value;
		}
	}

	public int MultiplayerMenuSelection
	{
		get
		{
			return _multiplayerMenuSelection;
		}
		set
		{
			if (value > 2)
			{
				value = 0;
			}
			else if (value < 0)
			{
				value = 2;
			}
			if (_multiplayerMenuSelection != value)
			{
				_gamepadSelectAsrc.Play();
			}
			_multiplayerMenuSelection = value;
		}
	}

	private void Awake()
	{
		_current = this;
		if (AtlyssNetworkManager._current._steamworksMode)
		{
			_multiplayer_hostOptionButton.onClick.AddListener(LobbyListManager._current.Enable_LobbyHostWindow);
		}
		_singleplayerButton.onClick.AddListener(Init_SingleplayerGame);
		_multiplayerButton.onClick.AddListener(Init_MultiplayerMenu);
		_quitGameButton.onClick.AddListener(Quit_Game);
		_settingsButton.onClick.AddListener(Open_SettingsMenu);
		_gamepadDetectButton_gamepadMode.interactable = false;
		_gamepadDetectButton_keyboardMode.interactable = false;
		_gamepadDetectPrompt.isEnabled = false;
		_gamepadDetectButton_gamepadMode.onClick.AddListener(Set_GamepadMode);
		_gamepadDetectButton_keyboardMode.onClick.AddListener(Set_KeyboardMode);
		_characterSelectAsrc.volume = 0f;
		_characterCreateAsrc.volume = 0f;
		_menuInputBuffer = 0.12f;
	}

	public void Reset_MainMenuConditions()
	{
		Set_MenuCondition(0);
	}

	private void Quit_Game()
	{
		Application.Quit();
	}

	private void Open_SettingsMenu()
	{
		SettingsManager._current.Open_SettingsMenu();
		_mainMenuCanvasObject.SetActive(value: false);
		_mainMenuSelectionBarObject.SetActive(value: false);
		_menuAcceptAsrc.Play();
	}

	private void Init_SingleplayerGame()
	{
		Set_MenuCondition(1);
		Set_HostMode(0);
	}

	private void Init_MultiplayerMenu()
	{
		_mainMenuSelectionBarObject.SetActive(value: false);
		_multiplayerSelectionMenu.isEnabled = true;
		_menuAcceptAsrc.Play();
		_inMultiplayerMenu = true;
	}

	private void Start()
	{
		Set_MenuCondition(0);
		_steamProfileDisplayElement.isEnabled = SteamManager.Initialized;
		if (SteamManager.Initialized)
		{
			CSteamID steamID = SteamUser.GetSteamID();
			int largeFriendAvatar = SteamFriends.GetLargeFriendAvatar(steamID);
			string friendlyName = SteamLobby._current.GetFriendlyName(steamID);
			Texture2D steamImageAsTexture2D = SteamLobby._current.GetSteamImageAsTexture2D(largeFriendAvatar);
			_steamProfile_text.text = friendlyName ?? "";
			_steamAvatar_icon.texture = steamImageAsTexture2D;
		}
		StartCoroutine(CheckForControllers());
	}

	private IEnumerator CheckForControllers()
	{
		yield return new WaitForSeconds(0.2f);
		if (!SettingsManager._current._detectedGamepadOnMenu)
		{
			string[] joystickNames = Input.GetJoystickNames();
			if (!_gamepadIsDetected && joystickNames.Length != 0)
			{
				_gamepadDetectPrompt.isEnabled = true;
				_gamepadIsDetected = true;
				_gamepadDetectButton_gamepadMode.interactable = true;
				_gamepadDetectButton_keyboardMode.interactable = true;
				_mainMenuSelectionBarObject.SetActive(value: false);
				_mainMenu_gamepadCursorIco.gameObject.SetActive(value: false);
				_multiplayerMenu_gamepadCursorIco.gameObject.SetActive(value: false);
			}
			else if (_gamepadIsDetected && joystickNames.Length == 0)
			{
				_gamepadIsDetected = false;
			}
		}
	}

	private void Set_GamepadMode()
	{
		SettingsManager._current._inputAxisTypeDropdown.value = 1;
		SettingsManager._current._detectedGamepadOnMenu = true;
		SettingsManager._current.Save_SettingsData();
		_gamepadIsDetected = false;
		_mainMenuCanvasObject.SetActive(value: true);
		_mainMenuSelectionBarObject.SetActive(value: true);
		_mainMenu_gamepadCursorIco.gameObject.SetActive(value: true);
		_gamepadDetectPrompt.isEnabled = false;
		_gamepadDetectButton_gamepadMode.interactable = false;
		_gamepadDetectButton_keyboardMode.interactable = false;
		_menuAcceptAsrc.Play();
	}

	private void Set_KeyboardMode()
	{
		SettingsManager._current._inputAxisTypeDropdown.value = 0;
		SettingsManager._current._detectedGamepadOnMenu = true;
		SettingsManager._current.Save_SettingsData();
		_gamepadIsDetected = false;
		_mainMenuCanvasObject.SetActive(value: true);
		_mainMenuSelectionBarObject.SetActive(value: true);
		_gamepadDetectPrompt.isEnabled = false;
		_gamepadDetectButton_gamepadMode.interactable = false;
		_gamepadDetectButton_keyboardMode.interactable = false;
		_menuAcceptAsrc.Play();
	}

	public void Set_MenuCondition(int _index)
	{
		_mainMenuCondition = (MainMenuCondition)_index;
		_mainMenuCamera.enabled = _mainMenuCondition == MainMenuCondition.MainMenu;
		_mainMenuParentObject.SetActive(_mainMenuCondition == MainMenuCondition.MainMenu || _mainMenuCondition == MainMenuCondition.ServerMenu);
		_inGameUIObject.SetActive(_mainMenuCondition == MainMenuCondition.In_Game);
		_inGameMenuUIObject.SetActive(_mainMenuCondition == MainMenuCondition.In_Game);
		_characterMenuParentObject.SetActive(_mainMenuCondition == MainMenuCondition.CharacterSelect || _mainMenuCondition == MainMenuCondition.CharacterCreation);
		_characterSelectUIObject.SetActive(_mainMenuCondition == MainMenuCondition.CharacterSelect);
		_characterCreationUIObject.SetActive(_mainMenuCondition == MainMenuCondition.CharacterCreation);
		if ((byte)_index == 0)
		{
			if (SteamManager.Initialized)
			{
				SteamFriends.SetRichPresence("steam_display", "#Status_AtMainMenu");
			}
			Cursor.lockState = CursorLockMode.None;
			if (InputControlManager.current._setGamepadType == GamepadType.Keyboard)
			{
				Cursor.visible = true;
			}
			else
			{
				Cursor.visible = false;
			}
			_mainMenuCanvasObject.SetActive(value: true);
			_mainMenuSelectionBarObject.SetActive(value: true);
			_versionDisplayText.text = Application.version;
			MultiplayerMenuSelection = 0;
			_inMultiplayerMenu = false;
			ProfileDataManager._current.Load_HostSettingsData();
			ProfileDataManager._current._loadPreviousCharacter = true;
			if (_characterSelectAsrc.isActiveAndEnabled)
			{
				_characterSelectAsrc.Stop();
			}
			if (_characterCreateAsrc.isActiveAndEnabled)
			{
				_characterCreateAsrc.Stop();
			}
			if (_menuFlyByAsrc.isActiveAndEnabled && !_menuFlyByAsrc.isPlaying)
			{
				_menuFlyByAsrc.Play();
			}
			if (AtlyssNetworkManager._current._steamworksMode && _characterSelectManager._cachedSteamLobby)
			{
				HostConsole._current.Init_NetworkStatusMessage("Cancelled Lobby Query.");
				SteamLobby._current._cachedSteamID = default;
				_characterSelectManager._cachedSteamLobby = false;
			}
		}
		else
		{
			_mainMenuCanvasObject.SetActive(value: false);
			_mainMenuSelectionBarObject.SetActive(value: false);
		}
		if ((byte)_index == 1)
		{
			if (SteamManager.Initialized)
			{
				SteamFriends.SetRichPresence("steam_display", "#Status_InCharacterSelection");
			}
			if (ProfileDataManager._current._loadPreviousCharacter)
			{
				_characterSelectManager.SelectedSlotPage = SettingsManager._current._settingsProfile._lastCharacterPage;
				ProfileDataManager._current.Set_FileIndex(SettingsManager._current._settingsProfile._lastCharacterFile);
			}
			else
			{
				ProfileDataManager._current.Set_FileIndex(ProfileDataManager._current.SelectedFileIndex);
			}
			_characterSelectManager.Reset_CameraRotation();
			Load_HostSettings();
		}
		if ((byte)_index == 2)
		{
			if (SteamManager.Initialized)
			{
				SteamFriends.SetRichPresence("steam_display", "#Status_InCharacterCreation");
			}
			ProfileDataManager._current._loadPreviousCharacter = false;
			_characterCreationManager.SelectCustomizationTab(0);
			_characterCreationManager.SelectRace(Random.Range(0, _characterCreationManager._raceDisplayModels.Length));
			_characterCreationManager.ClearCharacterNameInputField();
			_characterCreationManager.Reset_AttributePoints();
		}
		if ((byte)_index == 4)
		{
			ProfileDataManager._current._loadPreviousCharacter = true;
			SteamLobby._current._cachedSteamID = default;
			_characterSelectManager._cachedSteamLobby = false;
			base.transform.parent.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (_menuInputBuffer <= 0f)
		{
			if (InputControlManager.current._setGamepadType != GamepadType.Keyboard)
			{
				if (InputControlManager.current._gamepadMouseCursorBuffer > 0f)
				{
					Cursor.visible = true;
					Cursor.lockState = CursorLockMode.None;
				}
				else
				{
					Cursor.visible = false;
					Cursor.lockState = CursorLockMode.Locked;
				}
			}
			else
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
		}
		if (_menuInputBuffer > 0f)
		{
			_menuInputBuffer -= Time.deltaTime;
		}
		else
		{
			_menuInputBuffer = 0f;
		}
		_multiplayer_joinOptionButton.interactable = _multiplayerSelectionMenu.isEnabled;
		_multiplayer_hostOptionButton.interactable = _multiplayerSelectionMenu.isEnabled;
		_multiplayer_returnButton.interactable = _multiplayerSelectionMenu.isEnabled;
		switch (_mainMenuCondition)
		{
		case MainMenuCondition.MainMenu:
			RenderSettings.fog = base.enabled;
			RenderSettings.fogDensity = _mainMenuFogDensity;
			RenderSettings.fogColor = _mainMenuFogColor;
			RenderSettings.ambientMode = AmbientMode.Flat;
			RenderSettings.ambientSkyColor = _mainmenuAmbientColor;
			_menuFlyByAsrc.volume = Mathf.Lerp(_menuFlyByAsrc.volume, 0.85f, 3.25f * Time.deltaTime);
			_multiplayerSelectionMenu.gameObject.SetActive(value: true);
			Handle_GamepadMainMenuControl();
			break;
		case MainMenuCondition.CharacterSelect:
			RenderSettings.fog = base.enabled;
			RenderSettings.fogDensity = _characterselectFogDensity;
			RenderSettings.fogColor = _characterSelectFogColor;
			RenderSettings.ambientMode = AmbientMode.Flat;
			RenderSettings.ambientSkyColor = _characterSelectAmbientColor;
			if (_characterCreateAsrc.volume <= 0.1f)
			{
				_characterSelectAsrc.volume = Mathf.Lerp(_characterSelectAsrc.volume, 0.85f, 3.5f * Time.deltaTime);
			}
			_characterCreateAsrc.volume = Mathf.Lerp(_characterCreateAsrc.volume, 0f, 3.5f * Time.deltaTime);
			_menuFlyByAsrc.volume = Mathf.Lerp(_menuFlyByAsrc.volume, 0f, 3.25f * Time.deltaTime);
			break;
		case MainMenuCondition.CharacterCreation:
			RenderSettings.fog = base.enabled;
			RenderSettings.fogDensity = _characterselectFogDensity;
			RenderSettings.fogColor = _characterSelectFogColor;
			RenderSettings.ambientMode = AmbientMode.Flat;
			RenderSettings.ambientSkyColor = _characterSelectAmbientColor;
			if (_characterSelectAsrc.volume <= 0.1f)
			{
				_characterCreateAsrc.volume = Mathf.Lerp(_characterCreateAsrc.volume, 0.95f, 3.5f * Time.deltaTime);
			}
			_characterSelectAsrc.volume = Mathf.Lerp(_characterSelectAsrc.volume, 0f, 3.5f * Time.deltaTime);
			_menuFlyByAsrc.volume = Mathf.Lerp(_menuFlyByAsrc.volume, 0f, 3.25f * Time.deltaTime);
			break;
		}
		void Handle_GamepadMainMenuControl()
		{
			if (_gamepadIsDetected)
			{
				if (GamepadInput.GetButtonDown(GamepadButton.Button_South))
				{
					Set_GamepadMode();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
				{
					Set_KeyboardMode();
				}
			}
			else
			{
				_mainMenu_gamepadCursorIco.gameObject.SetActive(InputControlManager.current._setGamepadType != GamepadType.Keyboard && !_inMultiplayerMenu);
				_multiplayerMenu_gamepadCursorIco.gameObject.SetActive(InputControlManager.current._setGamepadType != GamepadType.Keyboard);
				float num = 25.5f;
				switch (_mainMenuSelection)
				{
				case 0:
					_mainMenu_gamepadCursorIco.anchoredPosition = Vector2.Lerp(_mainMenu_gamepadCursorIco.anchoredPosition, _singleplayerButton.GetComponent<RectTransform>().anchoredPosition, num * Time.deltaTime);
					break;
				case 1:
					_mainMenu_gamepadCursorIco.anchoredPosition = Vector2.Lerp(_mainMenu_gamepadCursorIco.anchoredPosition, _multiplayerButton.GetComponent<RectTransform>().anchoredPosition, num * Time.deltaTime);
					break;
				case 2:
					_mainMenu_gamepadCursorIco.anchoredPosition = Vector2.Lerp(_mainMenu_gamepadCursorIco.anchoredPosition, _settingsButton.GetComponent<RectTransform>().anchoredPosition, num * Time.deltaTime);
					break;
				case 3:
					_mainMenu_gamepadCursorIco.anchoredPosition = Vector2.Lerp(_mainMenu_gamepadCursorIco.anchoredPosition, _quitGameButton.GetComponent<RectTransform>().anchoredPosition, num * Time.deltaTime);
					break;
				}
				switch (_multiplayerMenuSelection)
				{
				case 0:
					_multiplayerMenu_gamepadCursorIco.anchoredPosition = Vector2.Lerp(_multiplayerMenu_gamepadCursorIco.anchoredPosition, _multiplayer_joinOptionButton.GetComponent<RectTransform>().anchoredPosition, num * Time.deltaTime);
					break;
				case 1:
					_multiplayerMenu_gamepadCursorIco.anchoredPosition = Vector2.Lerp(_multiplayerMenu_gamepadCursorIco.anchoredPosition, _multiplayer_hostOptionButton.GetComponent<RectTransform>().anchoredPosition, num * Time.deltaTime);
					break;
				case 2:
					_multiplayerMenu_gamepadCursorIco.anchoredPosition = Vector2.Lerp(_multiplayerMenu_gamepadCursorIco.anchoredPosition, _multiplayer_returnButton.GetComponent<RectTransform>().anchoredPosition, num * Time.deltaTime);
					break;
				}
				if (!(_menuInputBuffer > 0f) && !SettingsManager._current._isOpen && InputControlManager.current._setGamepadType != GamepadType.Keyboard)
				{
					bool num2 = InputControlManager.current.DpadLeft(DpadHoldType.OnDown);
					bool flag = InputControlManager.current.DpadRight(DpadHoldType.OnDown);
					bool flag2 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Left) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Left);
					bool flag3 = GamepadInput.GetButtonDown(GamepadButton.LeftStick_Right) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Right);
					bool buttonDown = GamepadInput.GetButtonDown(GamepadButton.LeftShoulder);
					bool buttonDown2 = GamepadInput.GetButtonDown(GamepadButton.RightShoulder);
					bool num3 = num2 | flag2 | buttonDown;
					bool flag4 = flag | flag3 | buttonDown2;
					if (num3)
					{
						if (_inMultiplayerMenu)
						{
							MultiplayerMenuSelection--;
						}
						else
						{
							MainMenuSelection--;
						}
					}
					else if (flag4)
					{
						if (_inMultiplayerMenu)
						{
							MultiplayerMenuSelection++;
						}
						else
						{
							MainMenuSelection++;
						}
					}
					if (GamepadInput.GetButtonDown(GamepadButton.Button_South))
					{
						_menuInputBuffer = 0.2f;
						if (_inMultiplayerMenu)
						{
							switch (_multiplayerMenuSelection)
							{
							case 0:
								_menuInputBuffer = 0.2f;
								Set_MenuCondition(1);
								Set_HostMode(2);
								break;
							case 1:
								_menuInputBuffer = 0.2f;
								LobbyListManager._current.Enable_LobbyHostWindow();
								_menuAcceptAsrc.Play();
								break;
							case 2:
								_menuInputBuffer = 0.2f;
								Set_MenuCondition(0);
								_menuDeclineAsrc.Play();
								break;
							}
							_multiplayerSelectionMenu.isEnabled = false;
							_inMultiplayerMenu = false;
						}
						else
						{
							switch (_mainMenuSelection)
							{
							case 0:
								_menuInputBuffer = 0.2f;
								Init_SingleplayerGame();
								break;
							case 1:
								_menuInputBuffer = 0.2f;
								Init_MultiplayerMenu();
								break;
							case 2:
								_menuInputBuffer = 0.2f;
								Open_SettingsMenu();
								break;
							case 3:
								Quit_Game();
								break;
							}
						}
					}
					if (GamepadInput.GetButtonDown(GamepadButton.Button_East) && _inMultiplayerMenu)
					{
						_multiplayerSelectionMenu.isEnabled = false;
						_menuDeclineAsrc.Play();
						Set_MenuCondition(0);
					}
				}
			}
		}
	}

	public void Save_HostSettingsToProfile()
	{
		ServerHostSettings_Profile hostSettingsProfile = ProfileDataManager._current._hostSettingsProfile;
		AtlyssNetworkManager._current._bannedClientList.Clear();
		AtlyssNetworkManager._current._mutedClientList.Clear();
		if (AtlyssNetworkManager._current._steamworksMode)
		{
			AtlyssNetworkManager._current._serverName = LobbyListManager._current._lobbyNameInput.text;
			AtlyssNetworkManager._current._serverPassword = LobbyListManager._current._lobbyPasswordInput.text;
			AtlyssNetworkManager._current._serverMotd = LobbyListManager._current._lobbyMotdInput.text;
			AtlyssNetworkManager._current.maxConnections = (int)LobbyListManager._current._lobbyMaxConnectionSlider.value;
			AtlyssNetworkManager._current._steamLobbyRealm = LobbyListManager._current._hostLobbyRealm;
			if (hostSettingsProfile._banList != null)
			{
				AtlyssNetworkManager._current._bannedClientList.AddRange(hostSettingsProfile._banList);
			}
			if (hostSettingsProfile._mutedList != null)
			{
				AtlyssNetworkManager._current._mutedClientList.AddRange(hostSettingsProfile._mutedList);
			}
		}
		ProfileDataManager._current.Save_HostSettingsData();
	}

	public void Load_HostSettings()
	{
		ServerHostSettings_Profile hostSettingsProfile = ProfileDataManager._current._hostSettingsProfile;
		_characterSelectManager._globalNickanmeInput.text = hostSettingsProfile._globalNickname;
		AtlyssNetworkManager._current._bannedClientList.Clear();
		AtlyssNetworkManager._current._mutedClientList.Clear();
		if (AtlyssNetworkManager._current._steamworksMode)
		{
			LobbyListManager._current._lobbyNameInput.text = hostSettingsProfile._serverName;
			LobbyListManager._current._lobbyPasswordInput.text = hostSettingsProfile._serverPassword;
			LobbyListManager._current._lobbyMotdInput.text = hostSettingsProfile._serverMotd;
			LobbyListManager._current._lobbyMaxConnectionSlider.value = hostSettingsProfile._maxAllowedConnections;
			LobbyListManager._current._streamModeToggle.isOn = hostSettingsProfile._streamMode;
		}
		if (hostSettingsProfile._banList != null)
		{
			AtlyssNetworkManager._current._bannedClientList.AddRange(hostSettingsProfile._banList);
		}
		if (hostSettingsProfile._mutedList != null)
		{
			AtlyssNetworkManager._current._mutedClientList.AddRange(hostSettingsProfile._mutedList);
		}
	}

	public void Set_HostMode(int _value)
	{
		_networkInitCondition = (NetworkInitCondition)_value;
	}

	public void Init_SingleplayerHost()
	{
		Save_HostSettingsToProfile();
		Load_HostSettings();
		if (!NetworkServer.active)
		{
			NetworkServer.dontListen = true;
			AtlyssNetworkManager._current._soloMode = true;
			AtlyssNetworkManager._current._serverMode = false;
			AtlyssNetworkManager._current.StartHost();
		}
	}

	public void Init_MultiplayerHost()
	{
		Save_HostSettingsToProfile();
		Load_HostSettings();
		if (NetworkServer.active)
		{
			return;
		}
		AtlyssNetworkManager._current._soloMode = false;
		AtlyssNetworkManager._current._serverMode = false;
		if (AtlyssNetworkManager._current._steamworksMode)
		{
			AtlyssNetworkManager._current._serverPassword = LobbyListManager._current._lobbyPasswordInput.text;
			AtlyssNetworkManager._current._sentPassword = AtlyssNetworkManager._current._serverPassword;
			AtlyssNetworkManager._current._serverMotd = LobbyListManager._current._lobbyMotdInput.text;
			AtlyssNetworkManager._current.maxConnections = (int)LobbyListManager._current._lobbyMaxConnectionSlider.value;
			ELobbyType lobbyType = ELobbyType.k_ELobbyTypePublic;
			switch (LobbyListManager._current._lobbyTypeDropdown.value)
			{
			case 0:
				lobbyType = ELobbyType.k_ELobbyTypePublic;
				break;
			case 1:
				lobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
				break;
			case 2:
				lobbyType = ELobbyType.k_ELobbyTypePrivate;
				break;
			}
			SteamLobby._current.HostLobby(lobbyType);
		}
		else
		{
			AtlyssNetworkManager._current.StartHost();
		}
	}

	public void Init_MultiplayerServer()
	{
		Save_HostSettingsToProfile();
		Load_HostSettings();
		if (!NetworkServer.active)
		{
			AtlyssNetworkManager._current._soloMode = false;
			AtlyssNetworkManager._current._serverMode = true;
			AtlyssNetworkManager._current.StartServer();
			Set_MenuCondition(3);
		}
	}

	public void Init_GenericTooltip(string _message)
	{
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector3(0f, 60f, 0f));
		ToolTipManager._current.Apply_GenericToolTip(_message);
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
	}

	public void Disable_Tooltips()
	{
		ToolTipManager._current.Disable_AllTooltips();
	}
}
