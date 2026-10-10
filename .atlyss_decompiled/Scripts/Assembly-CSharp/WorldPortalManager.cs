using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WorldPortalManager : MonoBehaviour
{
	public static WorldPortalManager _current;

	[Header("World Portal Manager")]
	[SerializeField]
	private MenuElement _zoneSelectElement;

	[SerializeField]
	private Button _previousPageButton;

	[SerializeField]
	private Button _nextPageButton;

	[SerializeField]
	private Button _teleportButton;

	[SerializeField]
	private Text _mapEntryHeaderText;

	[SerializeField]
	private GameObject _zoneSelectPrefab;

	[SerializeField]
	private Transform _mainContainer;

	[SerializeField]
	private RectTransform _mapContainerRect;

	[Header("Gamepad")]
	[SerializeField]
	private Image _gamepadIco_teleportInput;

	[SerializeField]
	private Image _gamepadIco_leftInput;

	[SerializeField]
	private Image _gamepadIco_rightInput;

	[Header("Runtime")]
	public ScriptableMapData _setMapData;

	public WorldPortalEntry _selectedWorldPortalEntry;

	public List<WorldPortalEntry> _worldPortalEntries = new List<WorldPortalEntry>();

	public int _worldPortalSelection;

	public bool _isOpen;

	public int CurrentWorldPortalSelection
	{
		get
		{
			return _worldPortalSelection;
		}
		set
		{
			if (value < 0)
			{
				value = _worldPortalEntries.Count - 1;
			}
			if (value > _worldPortalEntries.Count - 1)
			{
				value = 0;
			}
			_worldPortalSelection = value;
			if (_worldPortalEntries.Count > 0)
			{
				_selectedWorldPortalEntry = _worldPortalEntries[_worldPortalSelection];
			}
		}
	}

	private void Awake()
	{
		_current = this;
		_zoneSelectElement.isEnabled = false;
		_previousPageButton.onClick.AddListener(() =>
		{
			Change_ZoneSelectionPage(-1);
		});
		_nextPageButton.onClick.AddListener(() =>
		{
			Change_ZoneSelectionPage(1);
		});
		_teleportButton.onClick.AddListener(Init_WorldPortalTeleport);
	}

	private void Update()
	{
		if ((bool)Player._mainPlayer)
		{
			if (Player._mainPlayer._bufferingStatus)
			{
				_isOpen = false;
			}
			_isOpen = DialogManager._current._currentDialogUIPrompt == DialogUIPrompt.WORLDPORTAL;
			if (_teleportButton.interactable)
			{
				_gamepadIco_teleportInput.color = Color.white;
			}
			else
			{
				_gamepadIco_teleportInput.color = Color.grey;
			}
			_zoneSelectElement.isEnabled = _isOpen;
			_zoneSelectElement.gameObject.SetActive(_isOpen);
			_nextPageButton.interactable = _worldPortalEntries.Count > 0;
			_previousPageButton.interactable = _worldPortalEntries.Count > 0;
			if ((bool)_selectedWorldPortalEntry)
			{
				_mapEntryHeaderText.text = _selectedWorldPortalEntry._scriptMapData._mapCaptionTitle ?? "";
			}
			else
			{
				_mapEntryHeaderText.text = "-";
			}
			for (int i = 0; i < _worldPortalEntries.Count; i++)
			{
				_worldPortalEntries[i]._isSelected = _worldPortalEntries[i] == _selectedWorldPortalEntry;
			}
			if (!_isOpen)
			{
				_selectedWorldPortalEntry = null;
				_worldPortalSelection = 0;
				_setMapData = null;
			}
			_teleportButton.interactable = Check_ToTeleport();
			Handle_Control();
		}
		bool Check_ToTeleport()
		{
			if (!_selectedWorldPortalEntry)
			{
				return false;
			}
			if ((bool)_setMapData && _setMapData == _selectedWorldPortalEntry._scriptMapData)
			{
				return false;
			}
			return true;
		}
	}

	private void Handle_Control()
	{
		if (DialogManager._current._currentDialogUIPrompt != DialogUIPrompt.WORLDPORTAL)
		{
			return;
		}
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_gamepadIco_teleportInput.enabled = flag && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window;
		_gamepadIco_leftInput.enabled = flag && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window;
		_gamepadIco_rightInput.enabled = flag && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window;
		if (!flag || DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window)
		{
			bool flag2 = Input.GetKeyDown(InputControlManager.current._left) || InputControlManager.current.DpadLeft(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Left) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Left);
			bool num = Input.GetKeyDown(InputControlManager.current._right) || InputControlManager.current.DpadRight(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Right) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Right);
			if (flag2)
			{
				Change_ZoneSelectionPage(-1);
			}
			if (num)
			{
				Change_ZoneSelectionPage(1);
			}
			if (_teleportButton.interactable && GamepadInput.GetButtonDown(GamepadButton.Button_South))
			{
				Init_WorldPortalTeleport();
			}
		}
	}

	private void Init_WorldPortalTeleport()
	{
		if ((bool)_selectedWorldPortalEntry && (bool)Player._mainPlayer && !Player._mainPlayer._bufferingStatus)
		{
			Player._mainPlayer.Cmd_SceneTransport(_selectedWorldPortalEntry._scriptMapData._subScene, _selectedWorldPortalEntry._scriptMapData._spawnPointTag, ZoneDifficulty.NORMAL);
			Player._mainPlayer._pSound._aSrcGeneral.PlayOneShot(Player._mainPlayer._pSound._portalInteract, 0.85f);
			Player._mainPlayer._pVisual.Send_CrossFadeAnim("Idle", 0f, 0, LatencyCheck.IGNORE_LATENCY);
			Player._mainPlayer._recalledMapInstance = string.Empty;
			Player._mainPlayer._recalledMapInstanceName = string.Empty;
			Player._mainPlayer._pInteract._currentCommandBuffer = 2.5f;
			Player._mainPlayer._pMove._playerController.enabled = false;
			DialogManager._current.End_Dialog();
		}
	}

	public void Clear_ZoneEntries()
	{
		if (_worldPortalEntries.Count > 0)
		{
			for (int i = 0; i < _worldPortalEntries.Count; i++)
			{
				Object.Destroy(_worldPortalEntries[i].gameObject);
			}
			_worldPortalEntries.Clear();
		}
	}

	public void Refresh_ZoneEntries()
	{
		Clear_ZoneEntries();
		for (int i = 0; i < Player._mainPlayer._waypointAttunements.Count; i++)
		{
			ScriptableMapData scriptableMapData = GameManager._current.Locate_MapData(Player._mainPlayer._waypointAttunements[i]);
			if ((bool)scriptableMapData)
			{
				WorldPortalEntry component = Object.Instantiate(_zoneSelectPrefab).GetComponent<WorldPortalEntry>();
				component._scriptMapData = scriptableMapData;
				if ((bool)scriptableMapData._mapSelectionIcon)
				{
					component._zoneIcon.sprite = scriptableMapData._mapSelectionIcon;
				}
				if ((bool)scriptableMapData._backdropSprite)
				{
					component._backdropSprite.sprite = scriptableMapData._backdropSprite;
				}
				component._rectTransform.SetParent(_mainContainer);
				_worldPortalEntries.Add(component);
			}
		}
		_worldPortalEntries = _worldPortalEntries.OrderByDescending((WorldPortalEntry ch) => ch._scriptMapData._GUI_index).Reverse().ToList();
		for (int num = 0; num < _worldPortalEntries.Count; num++)
		{
			_worldPortalEntries[num]._index = num;
		}
		if (!_setMapData)
		{
			return;
		}
		for (int num2 = 0; num2 < _worldPortalEntries.Count; num2++)
		{
			if (_worldPortalEntries[num2]._scriptMapData == _setMapData)
			{
				_worldPortalSelection = num2;
				_selectedWorldPortalEntry = _worldPortalEntries[num2];
				break;
			}
		}
	}

	public void Change_ZoneSelectionPage(int _value)
	{
		_previousPageButton.interactable = true;
		_nextPageButton.interactable = true;
		CurrentWorldPortalSelection += _value;
	}
}
