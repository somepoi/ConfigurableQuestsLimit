using System.Collections;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuCell : TabMenuCell
{
	[Header("Options Menu Tab")]
	public MenuElement _optionsPanelMenu;

	[SerializeField]
	private Text _quitGameButtonText;

	[SerializeField]
	private Text _gameVersionText;

	[SerializeField]
	private Button _settingsButton;

	[SerializeField]
	private Button _saveFileButton;

	[SerializeField]
	private Button _inviteToLobbyButton;

	[SerializeField]
	private Button _hostConsoleButton;

	[SerializeField]
	private Button _saveQuitButton;

	[SerializeField]
	private RectTransform _settingsButtonRect;

	[SerializeField]
	private RectTransform _saveButtonRect;

	[SerializeField]
	private RectTransform _inviteLobbyButtonRect;

	[SerializeField]
	private RectTransform _consoleButtonRect;

	[SerializeField]
	private RectTransform _saveQuitButtonRect;

	[SerializeField]
	private RectTransform _gamepadSelectorCursor;

	[SerializeField]
	private Image _gamepadIco_confirmQuit;

	[SerializeField]
	private Image _gamepadIco_cancelQuit;

	private int _selectedOption;

	[Space]
	public MenuElement _saveAndQuitPromptElement;

	[SerializeField]
	private Button _cancelSaveQuitPromptButton;

	[SerializeField]
	private Button _confirmSaveQuitPromptButton;

	public bool _isDisconnecting;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _aSrc_confirmSave;

	[SerializeField]
	private AudioSource _disconnectAsrc;

	public int SelectedOption
	{
		get
		{
			return _selectedOption;
		}
		set
		{
			if (value < 0)
			{
				value = 4;
			}
			if (value > 4)
			{
				value = 0;
			}
			if (_selectedOption != value && TabMenu._current._isOpen && TabMenu._current._selectedMenuCell == this)
			{
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			_selectedOption = value;
		}
	}

	public override void Cell_OnAwake()
	{
		_settingsButton.onClick.AddListener(Init_SettingsMenu);
		_saveFileButton.onClick.AddListener(Init_SaveFile);
		_inviteToLobbyButton.onClick.AddListener(Init_SteamLobby_InviteFriends);
		_hostConsoleButton.onClick.AddListener(Init_HostConsole);
		_saveQuitButton.onClick.AddListener(Init_SaveQuitPrompt);
		_cancelSaveQuitPromptButton.onClick.AddListener(Init_CancelSaveQuitPrompt);
		_confirmSaveQuitPromptButton.onClick.AddListener(SaveQuit_Game);
	}

	public override void Handle_CellUpdate()
	{
		if (!CameraFunction._current._unlockedCamera || Input.GetKey(KeyCode.Escape))
		{
			_saveAndQuitPromptElement.isEnabled = false;
		}
		_optionsPanelMenu.isEnabled = !_saveAndQuitPromptElement.isEnabled && !_isDisconnecting;
		_inviteToLobbyButton.interactable = !AtlyssNetworkManager._current._soloMode;
		_hostConsoleButton.interactable = (bool)Player._mainPlayer && Player._mainPlayer._isHostPlayer && !AtlyssNetworkManager._current._soloMode;
		_confirmSaveQuitPromptButton.interactable = _saveAndQuitPromptElement.isEnabled;
		_cancelSaveQuitPromptButton.interactable = _saveAndQuitPromptElement.isEnabled;
		_quitGameButtonText.text = "Save & Quit";
		_gameVersionText.text = "ATLYSS - ver. " + Application.version;
		_gamepadSelectorCursor.gameObject.SetActive(InputControlManager.current._setGamepadType != GamepadType.Keyboard);
		_gamepadIco_confirmQuit.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_gamepadIco_cancelQuit.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		if (InputControlManager.current._setGamepadType == GamepadType.Keyboard)
		{
			return;
		}
		if (!TabMenu._current._isOpen)
		{
			SelectedOption = 0;
		}
		else
		{
			if (TabMenu._current.CurrentCellSelection != 0)
			{
				return;
			}
			if (_optionsPanelMenu.isEnabled)
			{
				switch (_selectedOption)
				{
				case 0:
					_gamepadSelectorCursor.anchoredPosition = _settingsButtonRect.anchoredPosition;
					break;
				case 1:
					_gamepadSelectorCursor.anchoredPosition = _saveButtonRect.anchoredPosition;
					break;
				case 2:
					_gamepadSelectorCursor.anchoredPosition = _inviteLobbyButtonRect.anchoredPosition;
					break;
				case 3:
					_gamepadSelectorCursor.anchoredPosition = _consoleButtonRect.anchoredPosition;
					break;
				case 4:
					_gamepadSelectorCursor.anchoredPosition = _saveQuitButtonRect.anchoredPosition;
					break;
				}
				bool num = InputControlManager.current.DpadDown(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Down);
				bool flag = InputControlManager.current.DpadUp(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Up);
				if (num)
				{
					SelectedOption++;
				}
				if (flag)
				{
					SelectedOption--;
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_South))
				{
					switch (_selectedOption)
					{
					case 0:
						Init_SettingsMenu();
						_settingsButton.GetComponent<AudioSource>().Play();
						break;
					case 1:
						Init_SaveFile();
						_saveFileButton.GetComponent<AudioSource>().Play();
						break;
					case 2:
						Init_SteamLobby_InviteFriends();
						_inviteToLobbyButton.GetComponent<AudioSource>().Play();
						break;
					case 3:
						Init_HostConsole();
						_hostConsoleButton.GetComponent<AudioSource>().Play();
						break;
					case 4:
						Init_SaveQuitPrompt();
						_saveQuitButton.GetComponent<AudioSource>().Play();
						break;
					}
				}
			}
			else if (_saveAndQuitPromptElement.isEnabled)
			{
				if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
				{
					_saveAndQuitPromptElement.isEnabled = false;
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_South))
				{
					SaveQuit_Game();
				}
			}
		}
	}

	private void Init_SteamLobby_InviteFriends()
	{
		if (!AtlyssNetworkManager._current._soloMode && NetworkClient.isConnected)
		{
			SteamFriends.ActivateGameOverlayInviteDialog((CSteamID)SteamLobby._current._currentLobbyID);
		}
	}

	private void Init_HostConsole()
	{
		if (!AtlyssNetworkManager._current._soloMode)
		{
			HostConsole._current._hostConsoleElement.isEnabled = true;
		}
	}

	private void Init_SaveQuitPrompt()
	{
		_saveAndQuitPromptElement.isEnabled = true;
	}

	private void Init_CancelSaveQuitPrompt()
	{
		_saveAndQuitPromptElement.isEnabled = false;
	}

	private void Init_SaveFile()
	{
		StartCoroutine(SaveFile_Routine());
	}

	private IEnumerator SaveFile_Routine()
	{
		if (_saveFileButton.interactable)
		{
			_saveFileButton.interactable = false;
			ProfileDataManager._current.Save_ProfileData();
			ProfileDataManager._current.Save_BackupFile();
			HostConsole._current.Init_NetworkStatusMessage("Character file saved...");
			_aSrc_confirmSave.Play();
			yield return new WaitForSeconds(2.3f);
			_saveFileButton.interactable = true;
		}
	}

	public void SaveQuit_Game()
	{
		if (!_isDisconnecting)
		{
			StartCoroutine(Init_SaveQuitGame());
			Init_CancelSaveQuitPrompt();
		}
	}

	private IEnumerator Init_SaveQuitGame()
	{
		Player._mainPlayer.Cmd_SetInactive();
		_isDisconnecting = true;
		ProfileDataManager._current.Save_ProfileData();
		HostConsole._current.Init_NetworkStatusMessage("Character file saved...");
		TabMenu._current.Close_TabMenu();
		_disconnectAsrc.Play();
		yield return new WaitForSeconds(1f);
		ProfileDataManager._current.Save_BackupFile();
		yield return new WaitForSeconds(1.8f);
		if (NetworkServer.active && NetworkClient.isConnected)
		{
			AtlyssNetworkManager._current.StopHost();
		}
		else if (NetworkClient.isConnected)
		{
			AtlyssNetworkManager._current.StopClient();
		}
	}

	public void Init_SettingsMenu()
	{
		SettingsManager._current.Open_SettingsMenu();
	}
}
