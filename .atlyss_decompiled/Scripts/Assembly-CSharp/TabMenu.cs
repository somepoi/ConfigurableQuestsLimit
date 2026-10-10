using UnityEngine;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
	public static TabMenu _current;

	private Player _mainPlayer;

	private PlayerStats _pStats;

	[Header("In-Game Menu UI Parameters")]
	public TabMenuCell _selectedMenuCell;

	[SerializeField]
	private MenuElement _dollyElement;

	public float _hotkeyBuffer;

	public bool _itemTradeMode;

	public bool _questPanelMode;

	public bool _isOpen;

	[Header("Cells")]
	[SerializeField]
	private TabMenuCell[] _menuCells = new TabMenuCell[5];

	[SerializeField]
	private Button _button_previousCell;

	[SerializeField]
	private Button _button_nextCell;

	[SerializeField]
	private Image _gamepadIco_previousCell;

	[SerializeField]
	private Image _gamepadIco_nextCell;

	[SerializeField]
	private GameObject _pointsAvailDolly_L;

	[SerializeField]
	private GameObject _pointsAvailDolly_R;

	[HideInInspector]
	public OptionsMenuCell _optionsCell;

	[HideInInspector]
	public StatsMenuCell _statsCell;

	[HideInInspector]
	public SkillsMenuCell _skillsCell;

	[HideInInspector]
	public ItemMenuCell _itemCell;

	[HideInInspector]
	public QuestMenuCell _questCell;

	[HideInInspector]
	public WhoMenuCell _whoCell;

	private int _currentCellSelection;

	[Header("Prompt Window")]
	public int _selectedListID;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _aSrc;

	[SerializeField]
	private AudioClip _openTabMenu;

	[SerializeField]
	private AudioClip _closeTabMenu;

	[Space]
	public AudioSource _gamepadSelectAsrc;

	public int CurrentCellSelection
	{
		get
		{
			return _currentCellSelection;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			else if (value > _menuCells.Length - 1)
			{
				value = _menuCells.Length - 1;
			}
			if (value != _currentCellSelection)
			{
				_aSrc.PlayOneShot(_closeTabMenu, 0.55f);
				_itemCell.Reset_ItemCellParams();
				_statsCell.Reset_AttributeProjection();
				_skillsCell._isSkillDragging = false;
				ToolTipManager._current.Disable_AllTooltips();
			}
			_currentCellSelection = value;
			if (_menuCells[_currentCellSelection].GetType() == typeof(OptionsMenuCell))
			{
				_optionsCell._optionsPanelMenu.isEnabled = true;
				_optionsCell._saveAndQuitPromptElement.isEnabled = false;
			}
			if (_menuCells[_currentCellSelection].GetType() != typeof(StatsMenuCell))
			{
				_statsCell.Reset_AttributeProjection();
			}
			if (_menuCells[_currentCellSelection].GetType() == typeof(WhoMenuCell))
			{
				_whoCell.Refresh_PlayerList();
			}
		}
	}

	private void SelectNextCell()
	{
		CurrentCellSelection++;
	}

	private void SelectPreviousCell()
	{
		CurrentCellSelection--;
	}

	private void Awake()
	{
		if (_current == null)
		{
			_current = this;
			_pointsAvailDolly_L.SetActive(value: false);
			_pointsAvailDolly_R.SetActive(value: false);
			_button_previousCell.onClick.AddListener(SelectPreviousCell);
			_button_nextCell.onClick.AddListener(SelectNextCell);
			_itemTradeMode = false;
			_questPanelMode = false;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		Cache_MenuCellTypes();
	}

	private void Cache_MenuCellTypes()
	{
		for (int i = 0; i < _menuCells.Length; i++)
		{
			if (_menuCells[i].GetType() == typeof(OptionsMenuCell))
			{
				_optionsCell = (OptionsMenuCell)_menuCells[i];
			}
			else if (_menuCells[i].GetType() == typeof(StatsMenuCell))
			{
				_statsCell = (StatsMenuCell)_menuCells[i];
			}
			else if (_menuCells[i].GetType() == typeof(SkillsMenuCell))
			{
				_skillsCell = (SkillsMenuCell)_menuCells[i];
			}
			else if (_menuCells[i].GetType() == typeof(ItemMenuCell))
			{
				_itemCell = (ItemMenuCell)_menuCells[i];
			}
			else if (_menuCells[i].GetType() == typeof(QuestMenuCell))
			{
				_questCell = (QuestMenuCell)_menuCells[i];
			}
			else if (_menuCells[i].GetType() == typeof(WhoMenuCell))
			{
				_whoCell = (WhoMenuCell)_menuCells[i];
			}
		}
		_currentCellSelection = 1;
	}

	private void Update()
	{
		_gamepadSelectAsrc.enabled = _isOpen || DialogManager._current._isDialogEnabled;
		if (!_mainPlayer)
		{
			Handle_LocateMainPlayer();
			return;
		}
		if (_isOpen && _mainPlayer._pCasting._currentCastSkill == GameManager._current._statLogics._recallSkill)
		{
			_mainPlayer._pCasting.Cmd_InitCastInterrupt();
		}
		if (_hotkeyBuffer > 0f)
		{
			_hotkeyBuffer -= Time.deltaTime;
		}
		else
		{
			_hotkeyBuffer = 0f;
		}
		_selectedMenuCell = _menuCells[_currentCellSelection];
		Handle_TabMenuControl();
	}

	private void Handle_LocateMainPlayer()
	{
		_dollyElement.isEnabled = false;
		_isOpen = false;
		for (int i = 0; i < _menuCells.Length; i++)
		{
			_menuCells[i]._menuCellElement.isEnabled = false;
			_menuCells[i].Clear_ListData();
		}
		if ((bool)Player._mainPlayer)
		{
			_mainPlayer = Player._mainPlayer;
			_pStats = _mainPlayer._pStats;
		}
	}

	public void Close_TabMenu()
	{
		_dollyElement.isEnabled = false;
		_aSrc.PlayOneShot(_closeTabMenu, 0.75f);
		_itemCell.Cancel_ItemPromptWindow();
		_itemCell.Cancel_QuantityPrompt();
		ToolTipManager._current.Disable_AllTooltips();
		if (!DialogManager._current._isDialogEnabled)
		{
			InGameUI._current._escapeMenuBuffer = 0.2f;
		}
	}

	private void Handle_TabMenuControl()
	{
		_aSrc.enabled = !_mainPlayer._bufferingStatus;
		if (_mainPlayer._bufferingStatus || _mainPlayer._currentPlayerCondition != PlayerCondition.ACTIVE || SettingsManager._current._isOpen)
		{
			_dollyElement.isEnabled = false;
			_isOpen = false;
			return;
		}
		_isOpen = _dollyElement.isEnabled;
		_gamepadIco_previousCell.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_gamepadIco_nextCell.enabled = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_gamepadIco_nextCell.color = Color.grey;
		_gamepadIco_previousCell.color = Color.grey;
		if (GamepadInput.GetButton(GamepadButton.LeftShoulder))
		{
			_gamepadIco_previousCell.color = Color.white;
		}
		if (GamepadInput.GetButton(GamepadButton.RightShoulder))
		{
			_gamepadIco_nextCell.color = Color.white;
		}
		if (_itemTradeMode || _questPanelMode)
		{
			if (_itemTradeMode)
			{
				CurrentCellSelection = 3;
			}
			else if (_questPanelMode)
			{
				CurrentCellSelection = 4;
			}
			_button_previousCell.gameObject.SetActive(value: false);
			_button_nextCell.gameObject.SetActive(value: false);
			_pointsAvailDolly_L.SetActive(value: false);
			_pointsAvailDolly_R.SetActive(value: false);
			_dollyElement.isEnabled = true;
		}
		else if (DialogManager._current._isDialogEnabled)
		{
			_isOpen = false;
			_dollyElement.isEnabled = false;
			_itemCell._currentEquipCellTab = EquipCellTab.EQUIPMENT_PANEL;
			return;
		}
		Handle_HotKeyControl();
		Handle_CellButtons();
		for (int i = 0; i < _menuCells.Length; i++)
		{
			if (i == _currentCellSelection)
			{
				_menuCells[i]._menuCellElement.isEnabled = true;
			}
			else if (_menuCells[i]._menuCellElement.isEnabled)
			{
				_menuCells[i]._menuCellElement.isEnabled = false;
			}
		}
		void Handle_CellButtons()
		{
			if (!_itemTradeMode && !_questPanelMode)
			{
				if (_isOpen && !_itemCell._heldItemListDataEntry)
				{
					if (GamepadInput.GetButtonDown(GamepadButton.RightShoulder))
					{
						SelectNextCell();
						ToolTipManager._current.Disable_AllTooltips();
					}
					else if (GamepadInput.GetButtonDown(GamepadButton.LeftShoulder))
					{
						SelectPreviousCell();
						ToolTipManager._current.Disable_AllTooltips();
					}
				}
				if (_currentCellSelection <= 0)
				{
					_button_previousCell.gameObject.SetActive(value: false);
				}
				else
				{
					_button_previousCell.gameObject.SetActive(value: true);
				}
				if (_currentCellSelection >= _menuCells.Length - 1)
				{
					_button_nextCell.gameObject.SetActive(value: false);
				}
				else
				{
					_button_nextCell.gameObject.SetActive(value: true);
				}
				if (_currentCellSelection > 0)
				{
					_button_previousCell.GetComponentInChildren<Text>().text = "<< " + _menuCells[_currentCellSelection - 1]._menuCell_tag;
				}
				if (_currentCellSelection < _menuCells.Length - 1)
				{
					_button_nextCell.GetComponentInChildren<Text>().text = _menuCells[_currentCellSelection + 1]._menuCell_tag + " >>";
				}
				_pointsAvailDolly_L.SetActive((_pStats._currentAttributePoints > 0 && _currentCellSelection == 2) || (_pStats._currentSkillPoints > 0 && _currentCellSelection == 3));
				_pointsAvailDolly_R.SetActive((_pStats._currentSkillPoints > 0 && _currentCellSelection == 1) || (_pStats._currentAttributePoints > 0 && _currentCellSelection == 0));
			}
		}
		void Handle_HotKeyControl()
		{
			if (!(DialogManager._current._currentDialogTrigger != null) && !_mainPlayer._pProfession._isProfessionAction && !_mainPlayer._inChat && !_itemCell._heldItemListDataEntry && !(_hotkeyBuffer > 0f))
			{
				if (Input.GetKeyDown(InputControlManager.current._tabMenu) || (GamepadInput.GetButtonDown(GamepadButton.Start) && Player._mainPlayer._partyInviteStatus != PartyInviteStatus.INVITED))
				{
					if (_dollyElement.isEnabled)
					{
						Close_TabMenu();
					}
					else
					{
						_aSrc.PlayOneShot(_openTabMenu, 0.75f);
						_dollyElement.isEnabled = true;
						if (_selectedMenuCell == _whoCell)
						{
							_whoCell.Refresh_PlayerList();
						}
					}
				}
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					if (_isOpen)
					{
						Close_TabMenu();
					}
					else
					{
						_aSrc.PlayOneShot(_openTabMenu, 0.75f);
						_dollyElement.isEnabled = true;
						CurrentCellSelection = 0;
					}
				}
				if (Input.GetKeyDown(InputControlManager.current._statMenuHotkey))
				{
					if (!_isOpen)
					{
						_aSrc.PlayOneShot(_openTabMenu, 0.75f);
					}
					else
					{
						_aSrc.PlayOneShot(_closeTabMenu, 0.55f);
					}
					if (_menuCells[_currentCellSelection] == _statsCell && _isOpen)
					{
						_dollyElement.isEnabled = false;
						InGameUI._current._escapeMenuBuffer = 0.2f;
					}
					else
					{
						_dollyElement.isEnabled = true;
					}
					CurrentCellSelection = 1;
				}
				if (Input.GetKeyDown(InputControlManager.current._skillMenuHotkey))
				{
					if (!_isOpen)
					{
						_aSrc.PlayOneShot(_openTabMenu, 0.75f);
					}
					else
					{
						_aSrc.PlayOneShot(_closeTabMenu, 0.55f);
					}
					if (_menuCells[_currentCellSelection] == _skillsCell && _isOpen)
					{
						Close_TabMenu();
					}
					else
					{
						_dollyElement.isEnabled = true;
					}
					CurrentCellSelection = 2;
				}
				if (Input.GetKeyDown(InputControlManager.current._itemMenuHotkey))
				{
					if (!_isOpen)
					{
						_aSrc.PlayOneShot(_openTabMenu, 0.75f);
					}
					else
					{
						_aSrc.PlayOneShot(_closeTabMenu, 0.55f);
					}
					if (_menuCells[_currentCellSelection] == _itemCell && _isOpen)
					{
						Close_TabMenu();
					}
					else
					{
						_dollyElement.isEnabled = true;
					}
					CurrentCellSelection = 3;
				}
				if (Input.GetKeyDown(InputControlManager.current._questMenuHotkey))
				{
					if (!_isOpen)
					{
						_aSrc.PlayOneShot(_openTabMenu, 0.75f);
					}
					else
					{
						_aSrc.PlayOneShot(_closeTabMenu, 0.55f);
					}
					if (_menuCells[_currentCellSelection] == _questCell && _isOpen)
					{
						Close_TabMenu();
					}
					else
					{
						_dollyElement.isEnabled = true;
					}
					CurrentCellSelection = 4;
				}
				if (Input.GetKeyDown(InputControlManager.current._whoMenuHotkey))
				{
					if (!_isOpen)
					{
						_aSrc.PlayOneShot(_openTabMenu, 0.75f);
					}
					else
					{
						_aSrc.PlayOneShot(_closeTabMenu, 0.55f);
					}
					_whoCell.Refresh_PlayerList();
					if (_menuCells[_currentCellSelection] == _whoCell && _isOpen)
					{
						Close_TabMenu();
					}
					else
					{
						_dollyElement.isEnabled = true;
					}
					CurrentCellSelection = 5;
				}
			}
		}
	}
}
