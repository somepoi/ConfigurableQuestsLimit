using UnityEngine;
using UnityEngine.UI;

public class DungeonPortalManager : MonoBehaviour
{
	public static DungeonPortalManager _current;

	public Portal _setPortal;

	public ScenePortalData _scenePortal;

	[SerializeField]
	private Button _enterDungeonButton_easy;

	[SerializeField]
	private Button _enterDungeonButton_normal;

	[SerializeField]
	private Button _enterDungeonButton_hard;

	[SerializeField]
	private Button _enterPartyButton;

	[SerializeField]
	private Text _dungeonButtonText_easy;

	[SerializeField]
	private Text _dungeonButtonText_normal;

	[SerializeField]
	private Text _dungeonButtonText_hard;

	[SerializeField]
	private RectTransform _easyButtonRect;

	[SerializeField]
	private RectTransform _normalButtonRect;

	[SerializeField]
	private RectTransform _hardButtonRect;

	[SerializeField]
	private RectTransform _joinPartyButtonRect;

	[Space]
	[SerializeField]
	private MenuElement _menuElement;

	[SerializeField]
	private Text _dungeonNameHeaderText;

	[SerializeField]
	private GameObject _questPartyQueueHeader;

	[SerializeField]
	private RectTransform _gamepadSelectorCursor;

	public bool _isOpen;

	[Header("Gamepad")]
	private float _gamepad_inputBuffer;

	private int _selectedDungeonOption;

	public int SelectedDungeonOption
	{
		get
		{
			return _selectedDungeonOption;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (value > 3)
			{
				value = 3;
			}
			_selectedDungeonOption = value;
		}
	}

	private void Awake()
	{
		_current = this;
		_enterDungeonButton_easy.onClick.AddListener(() =>
		{
			OnPress_EnterDungeon(ZoneDifficulty.EASY);
		});
		_enterDungeonButton_normal.onClick.AddListener(() =>
		{
			OnPress_EnterDungeon(ZoneDifficulty.NORMAL);
		});
		_enterDungeonButton_hard.onClick.AddListener(() =>
		{
			OnPress_EnterDungeon(ZoneDifficulty.HARD);
		});
		_enterPartyButton.onClick.AddListener(OnPress_EnterPartyDungeon);
	}

	private void OnPress_EnterDungeon(ZoneDifficulty _setDifficulty)
	{
		if ((bool)_setPortal)
		{
			Player._mainPlayer._pInteract.Cmd_InteractWithPortal(_setPortal, _setDifficulty);
			Player._mainPlayer._pSound._aSrcGeneral.PlayOneShot(Player._mainPlayer._pSound._portalInteract, 0.85f);
			DialogManager._current.End_Dialog();
		}
	}

	private void OnPress_EnterPartyDungeon()
	{
		if ((bool)_setPortal && (bool)Player._mainPlayer.Network_partyObject)
		{
			Player._mainPlayer._pInteract.Cmd_InteractWithPortal(_setPortal, ZoneDifficulty.NORMAL);
			Player._mainPlayer._pSound._aSrcGeneral.PlayOneShot(Player._mainPlayer._pSound._portalInteract, 0.85f);
			DialogManager._current.End_Dialog();
		}
	}

	private void Update()
	{
		if (!Player._mainPlayer)
		{
			return;
		}
		Player mainPlayer = Player._mainPlayer;
		_isOpen = DialogManager._current._currentDialogUIPrompt == DialogUIPrompt.DUNGEON_PORTAL;
		_menuElement.gameObject.SetActive(_isOpen);
		if (!_isOpen)
		{
			_enterDungeonButton_easy.gameObject.SetActive(value: false);
			_enterPartyButton.gameObject.SetActive(value: false);
			_gamepad_inputBuffer = 0.4f;
			return;
		}
		bool _canJoinParty = (bool)mainPlayer.Network_partyObject && mainPlayer.Network_partyObject.Network_partyLeaderPlayer != mainPlayer && !mainPlayer.Network_partyObject.Network_partyLeaderPlayer._bufferingStatus && mainPlayer.Network_partyObject._inPatternInstance && mainPlayer.Network_partyObject.Network_partyLeaderPlayer._mapName == _scenePortal._portalCaptionTitle;
		bool _isPartyLeader = (bool)mainPlayer.Network_partyObject && mainPlayer.Network_partyObject.Network_partyLeaderPlayer == mainPlayer;
		bool num = (bool)mainPlayer.Network_partyObject && mainPlayer.Network_partyObject.Network_partyLeaderPlayer != mainPlayer;
		bool _lvReq_easy = _scenePortal._easyLevelRequirement > 0 && mainPlayer._pStats._currentLevel >= _scenePortal._easyLevelRequirement;
		bool _lvReq_normal = _scenePortal._normalLevelRequirement > 0 && mainPlayer._pStats._currentLevel >= _scenePortal._normalLevelRequirement;
		bool _lvReq_hard = _scenePortal._hardLevelRequirement > 0 && mainPlayer._pStats._currentLevel >= _scenePortal._hardLevelRequirement;
		if (num)
		{
			_enterDungeonButton_easy.gameObject.SetActive(value: false);
			_enterDungeonButton_normal.gameObject.SetActive(value: false);
			_enterDungeonButton_hard.gameObject.SetActive(value: false);
		}
		else
		{
			_enterDungeonButton_easy.gameObject.SetActive(!_enterPartyButton.gameObject.activeInHierarchy && _scenePortal._easyLevelRequirement > 0);
			_enterDungeonButton_normal.gameObject.SetActive(!_enterPartyButton.gameObject.activeInHierarchy && _scenePortal._normalLevelRequirement > 0);
			_enterDungeonButton_hard.gameObject.SetActive(!_enterPartyButton.gameObject.activeInHierarchy && _scenePortal._hardLevelRequirement > 0);
			_enterDungeonButton_easy.interactable = mainPlayer._pStats._currentLevel >= _scenePortal._easyLevelRequirement;
			_dungeonButtonText_easy.text = $"Enter Dungeon (LV {_scenePortal._easyLevelRequirement}-{_scenePortal._maxEasyLevel})";
			_enterDungeonButton_normal.interactable = mainPlayer._pStats._currentLevel >= _scenePortal._normalLevelRequirement;
			_dungeonButtonText_normal.text = $"Enter Dungeon (LV {_scenePortal._normalLevelRequirement}-{_scenePortal._maxNormalLevel})";
			_enterDungeonButton_hard.interactable = mainPlayer._pStats._currentLevel >= _scenePortal._hardLevelRequirement;
			_dungeonButtonText_hard.text = $"Enter Dungeon (LV {_scenePortal._hardLevelRequirement}-{_scenePortal._maxHardLevel})";
		}
		Handle_GamepadControl();
		_questPartyQueueHeader.SetActive((bool)Player._mainPlayer.Network_partyObject && !_canJoinParty && !_isPartyLeader);
		_enterPartyButton.gameObject.SetActive(_canJoinParty && !_isPartyLeader);
		_dungeonNameHeaderText.text = "- " + _scenePortal._portalCaptionTitle + " -";
		void Handle_GamepadControl()
		{
			if (DialogManager._current._currentDialogUIPrompt == DialogUIPrompt.DUNGEON_PORTAL)
			{
				if (_gamepad_inputBuffer > 0f)
				{
					_gamepad_inputBuffer -= Time.deltaTime;
				}
				else
				{
					_gamepad_inputBuffer = 0f;
				}
				bool num2 = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
				bool flag = num2 && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window;
				bool flag2 = !_enterDungeonButton_easy.gameObject.activeInHierarchy && !_enterDungeonButton_normal.gameObject.activeInHierarchy && !_enterDungeonButton_hard.gameObject.activeInHierarchy && !_canJoinParty;
				_gamepadSelectorCursor.gameObject.SetActive(flag && !flag2);
				bool flag3 = InputControlManager.current.DpadDown(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Down) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Down);
				bool flag4 = InputControlManager.current.DpadUp(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Up) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Up);
				if (num2)
				{
					if (!Player._mainPlayer.Network_partyObject | _isPartyLeader)
					{
						if (_selectedDungeonOption <= 0)
						{
							SelectedDungeonOption = 1;
						}
						if (_selectedDungeonOption == 1 && _scenePortal._easyLevelRequirement <= 0)
						{
							SelectedDungeonOption = 2;
						}
						if (_selectedDungeonOption == 2 && _scenePortal._normalLevelRequirement <= 0)
						{
							SelectedDungeonOption = 3;
						}
						if (_selectedDungeonOption == 3 && _scenePortal._hardLevelRequirement <= 0)
						{
							SelectedDungeonOption = 1;
						}
						if (flag3)
						{
							SelectedDungeonOption++;
							TabMenu._current._gamepadSelectAsrc.Play();
						}
						if (flag4)
						{
							SelectedDungeonOption--;
							TabMenu._current._gamepadSelectAsrc.Play();
						}
					}
					else
					{
						_selectedDungeonOption = 0;
					}
					float num3 = 25.5f;
					switch (_selectedDungeonOption)
					{
					case 0:
						_gamepadSelectorCursor.anchoredPosition = Vector2.Lerp(_gamepadSelectorCursor.anchoredPosition, _joinPartyButtonRect.anchoredPosition, num3 * Time.deltaTime);
						break;
					case 1:
						_gamepadSelectorCursor.anchoredPosition = Vector2.Lerp(_gamepadSelectorCursor.anchoredPosition, _easyButtonRect.anchoredPosition, num3 * Time.deltaTime);
						break;
					case 2:
						_gamepadSelectorCursor.anchoredPosition = Vector2.Lerp(_gamepadSelectorCursor.anchoredPosition, _normalButtonRect.anchoredPosition, num3 * Time.deltaTime);
						break;
					case 3:
						_gamepadSelectorCursor.anchoredPosition = Vector2.Lerp(_gamepadSelectorCursor.anchoredPosition, _hardButtonRect.anchoredPosition, num3 * Time.deltaTime);
						break;
					}
					if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && !flag2 && _gamepad_inputBuffer <= 0f)
					{
						switch (_selectedDungeonOption)
						{
						case 0:
							OnPress_EnterPartyDungeon();
							break;
						case 1:
							if (_lvReq_easy)
							{
								OnPress_EnterDungeon(ZoneDifficulty.EASY);
							}
							break;
						case 2:
							if (_lvReq_normal)
							{
								OnPress_EnterDungeon(ZoneDifficulty.NORMAL);
							}
							break;
						case 3:
							if (_lvReq_hard)
							{
								OnPress_EnterDungeon(ZoneDifficulty.HARD);
							}
							break;
						}
					}
				}
			}
		}
	}
}
