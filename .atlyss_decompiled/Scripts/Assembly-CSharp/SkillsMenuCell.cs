using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsMenuCell : TabMenuCell
{
	[Header("Skills Cell")]
	[SerializeField]
	private Text _skillsCell_classHeader;

	[SerializeField]
	private Text _skillsCell_skillPointCounter;

	[Space]
	[SerializeField]
	private Button _skillsCell_previousPageButton;

	[SerializeField]
	private Button _skillsCell_nextPageButton;

	[SerializeField]
	private Text _skillPageCounter;

	[Space]
	[SerializeField]
	private Button _skillsCell_baseClassTab;

	[SerializeField]
	private Button _skillsCell_subClassTab;

	[SerializeField]
	private Image _skillsCell_noviceIcon;

	[SerializeField]
	private Image _skillsCell_classIcon;

	[SerializeField]
	private Image _skillsCell_subClassIcon;

	[Space]
	[Header("Skill Tabs")]
	[SerializeField]
	private SkillTier _currentSkillTab;

	[SerializeField]
	private SkillListDataEntry _recallEntry;

	public SkillListDataEntry _selectedListEntry;

	[Space]
	[SerializeField]
	private GameObject _noviceSkillGroup;

	[SerializeField]
	private GameObject _classSkillGroup;

	[SerializeField]
	private GameObject _subclassSkillGroup;

	[Space]
	[SerializeField]
	private Transform _noviceSkillContent;

	[SerializeField]
	private RectTransform _noviceSkillRectContent;

	[SerializeField]
	private Transform _classSkillContent;

	[SerializeField]
	private Transform _tier2SkillContent;

	[Space]
	private readonly List<SkillListDataEntry> _noviceSkillEntries = new List<SkillListDataEntry>();

	private readonly List<SkillListDataEntry> _classSkillEntries = new List<SkillListDataEntry>();

	private readonly List<SkillListDataEntry> _tier2SkillEntries = new List<SkillListDataEntry>();

	private int _currentSkillPage;

	private int _maxNoviceSkillPages;

	[Header("Skill Dragging")]
	public ScriptableSkill _scriptSkillDrag;

	public SkillStruct _dragSkillStruct = new SkillStruct();

	public bool _isSkillDragging;

	[Space]
	public RectTransform _skillDragIconRect;

	public GameObject _skillDragIconObject;

	[Space]
	public Image _skillDragIconGamepadCursor;

	public Image _skillDragIconSprite;

	[Header("Gamepad")]
	[SerializeField]
	private MenuElement _gamepad_contextPrompt_cancelSkill;

	[SerializeField]
	private RectTransform _gamepad_tierTabCursor;

	[SerializeField]
	private RectTransform _gamepad_selectHighlight;

	[SerializeField]
	private RectTransform[] _gamepad_highlightPositions;

	[SerializeField]
	private Image _gamepad_previousPageIco;

	[SerializeField]
	private Image _gamepad_nextPageIco;

	[SerializeField]
	private Image _gamepad_pageSelectionHighlight;

	public bool _gamepad_tierSkillSelection;

	private bool _gamepadTooltip;

	private int _selectedSkillSlot;

	private int _selectedActionSlot;

	[Header("Audio")]
	[SerializeField]
	private AudioSource _aSrc_pageFlip;

	[SerializeField]
	private AudioSource _aSrc_setSkill;

	private bool _creatingSkillListEntries;

	public int CurrentSkillPage
	{
		get
		{
			return _currentSkillPage;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			else if (value > _maxNoviceSkillPages)
			{
				value = _maxNoviceSkillPages;
			}
			_currentSkillPage = value;
		}
	}

	public int SelectedSkillSlot
	{
		get
		{
			return _selectedSkillSlot;
		}
		set
		{
			if (_selectedSkillSlot == 1 && value == -1)
			{
				value = 8;
			}
			else if (_selectedSkillSlot == 8 && value == 10)
			{
				value = 1;
			}
			if (_selectedSkillSlot == 0 && value == -1)
			{
				_gamepad_tierSkillSelection = true;
				value = 0;
			}
			else if (_selectedSkillSlot == 2 && value == 1)
			{
				_gamepad_tierSkillSelection = true;
				value = 2;
			}
			else if (_selectedSkillSlot == 4 && value == 3)
			{
				_gamepad_tierSkillSelection = true;
				value = 4;
			}
			else if (_selectedSkillSlot == 6 && value == 5)
			{
				_gamepad_tierSkillSelection = true;
				value = 6;
			}
			else if (_selectedSkillSlot == 8 && value == 7)
			{
				_gamepad_tierSkillSelection = true;
				value = 8;
			}
			if (value < 0)
			{
				value = 0;
			}
			else if (value > 9)
			{
				value = 9;
			}
			if (value != _selectedSkillSlot)
			{
				TabMenu._current._gamepadSelectAsrc.Play();
				ToolTipManager._current._skillToolTip.Disable_ToolTip();
			}
			_selectedSkillSlot = value;
			if (IsInvoking("Init_GamepadToolTip"))
			{
				CancelInvoke("Init_GamepadToolTip");
			}
			Invoke("Init_GamepadToolTip", 0.3f);
		}
	}

	public int SelectedActionSlot
	{
		get
		{
			return _selectedActionSlot;
		}
		set
		{
			if (value > ActionBarManager._current._actionSlotTransforms.Length - 1)
			{
				value = 0;
			}
			if (value < 0)
			{
				value = ActionBarManager._current._actionSlotTransforms.Length - 1;
			}
			_selectedActionSlot = value;
		}
	}

	private void Init_GamepadToolTip()
	{
		if ((bool)_selectedListEntry && (bool)_selectedListEntry._scriptSkill)
		{
			_selectedListEntry.Init_SkillToolTipInfo();
			_gamepadTooltip = true;
		}
	}

	public override void Cell_OnAwake()
	{
		_skillsCell_previousPageButton.onClick.AddListener(Init_PreviousPage);
		_skillsCell_nextPageButton.onClick.AddListener(Init_NextPage);
		_skillDragIconObject.SetActive(value: false);
		_currentSkillTab = SkillTier.NOVICE;
		Invoke("Apply_SkillGroupDisplay", 1.5f);
	}

	private void Init_PreviousPage()
	{
		CurrentSkillPage--;
		_aSrc_pageFlip.pitch = Random.Range(0.9471624f, 1.065239f);
		_aSrc_pageFlip.Play();
	}

	private void Init_NextPage()
	{
		CurrentSkillPage++;
		_aSrc_pageFlip.pitch = Random.Range(0.9471624f, 1.065239f);
		_aSrc_pageFlip.Play();
	}

	public override void Handle_CellUpdate()
	{
		if (!TabMenu._current._isOpen || TabMenu._current._selectedMenuCell != this)
		{
			CurrentSkillPage = 0;
			SelectedSkillSlot = 0;
			_selectedListEntry = null;
			_isSkillDragging = false;
		}
		else if ((bool)Player._mainPlayer)
		{
			if (!_menuCellElement.isEnabled)
			{
				_selectedListEntry = null;
			}
			_noviceSkillRectContent.anchoredPosition = new Vector2(0f, 0f + 235f * (float)_currentSkillPage - 1f);
			if (_currentSkillTab == SkillTier.NOVICE)
			{
				_skillPageCounter.text = $"Page ( {_currentSkillPage + 1} / {_maxNoviceSkillPages + 1} )";
			}
			else
			{
				_skillPageCounter.text = "Page ( 1 / 1 )";
			}
			PlayerStats pStats = Player._mainPlayer._pStats;
			if (_listDataEntries.Count < Player._mainPlayer._pCasting._playerSkillStructs.Count && !_creatingSkillListEntries)
			{
				StartCoroutine(Create_SkillListData());
			}
			_skillsCell_skillPointCounter.text = $"{pStats._currentSkillPoints}";
			_skillsCell_baseClassTab.interactable = pStats._class;
			_skillsCell_subClassTab.interactable = pStats._syncClassTier > 0;
			if ((bool)pStats._class && (bool)pStats._class._classIcon)
			{
				_skillsCell_classIcon.sprite = pStats._class._classIcon;
			}
			if ((bool)pStats._class && pStats._syncClassTier > 0 && (bool)pStats._class._playerClassTiers[pStats._syncClassTier - 1]._classTierIcon)
			{
				_skillsCell_subClassIcon.sprite = pStats._class._playerClassTiers[pStats._syncClassTier - 1]._classTierIcon;
			}
			Handle_DragSkillIconControl();
			Handle_GamepadControl();
		}
	}

	public void Select_SkillTab(int _value)
	{
		if (_value < 0)
		{
			_value = 0;
		}
		else if (_value > 2)
		{
			_value = 2;
		}
		if (((byte)_value != 1 || (bool)Player._mainPlayer._pStats._class) && ((byte)_value != 2 || Player._mainPlayer._pStats._syncClassTier > 0))
		{
			_currentSkillTab = (SkillTier)_value;
			_selectedListEntry = null;
			Apply_SkillGroupDisplay();
			if (!Player._mainPlayer._bufferingStatus)
			{
				_aSrc_pageFlip.pitch = Random.Range(0.9471624f, 1.065239f);
				_aSrc_pageFlip.Play();
			}
		}
	}

	public void Apply_SkillGroupDisplay()
	{
		CurrentSkillPage = 0;
		_noviceSkillGroup.SetActive(_currentSkillTab == SkillTier.NOVICE);
		_classSkillGroup.SetActive(_currentSkillTab == SkillTier.CLASS);
		_subclassSkillGroup.SetActive(_currentSkillTab == SkillTier.SUBCLASS);
		switch (_currentSkillTab)
		{
		case SkillTier.NOVICE:
			_skillsCell_classHeader.text = "General Skillbook";
			_skillsCell_noviceIcon.color = Color.white;
			if ((bool)Player._mainPlayer && (bool)Player._mainPlayer._pStats._class)
			{
				_skillsCell_classIcon.color = Color.grey;
			}
			if ((bool)Player._mainPlayer && Player._mainPlayer._pStats._syncClassTier > 0)
			{
				_skillsCell_subClassIcon.color = Color.grey;
			}
			break;
		case SkillTier.CLASS:
			_skillsCell_classHeader.text = Player._mainPlayer._pStats._class._className + " Skillbook";
			_skillsCell_noviceIcon.color = Color.grey;
			_skillsCell_classIcon.color = Color.white;
			_skillsCell_subClassIcon.color = Color.grey;
			break;
		case SkillTier.SUBCLASS:
			_skillsCell_classHeader.text = Player._mainPlayer._pStats._class._playerClassTiers[Player._mainPlayer._pStats._syncClassTier - 1]._classTierName + " Skillbook";
			_skillsCell_noviceIcon.color = Color.grey;
			_skillsCell_classIcon.color = Color.grey;
			_skillsCell_subClassIcon.color = Color.white;
			break;
		}
	}

	private void Handle_GamepadControl()
	{
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard && InputControlManager.current._gamepadMouseCursorBuffer <= 0f;
		_gamepad_selectHighlight.gameObject.SetActive((!_isSkillDragging && !_gamepad_tierSkillSelection) & flag);
		_gamepad_tierTabCursor.gameObject.SetActive((_gamepad_tierSkillSelection & flag) && TabMenu._current._isOpen);
		_gamepad_selectHighlight.anchoredPosition = _gamepad_highlightPositions[_selectedSkillSlot].anchoredPosition;
		if (_gamepadTooltip && Cursor.visible)
		{
			ToolTipManager._current._skillToolTip.Disable_ToolTip();
			_gamepadTooltip = false;
		}
		_gamepad_pageSelectionHighlight.enabled = _gamepad_tierSkillSelection;
		_gamepad_previousPageIco.enabled = _gamepad_tierSkillSelection;
		_gamepad_nextPageIco.enabled = _gamepad_tierSkillSelection;
		if (InputControlManager.current._setGamepadType == GamepadType.Keyboard || !TabMenu._current._isOpen || TabMenu._current.CurrentCellSelection != 2 || _isSkillDragging)
		{
			return;
		}
		if (!_gamepad_tierSkillSelection)
		{
			if (InputControlManager.current.DpadLeft(DpadHoldType.OnDown))
			{
				SelectedSkillSlot--;
			}
			if (InputControlManager.current.DpadRight(DpadHoldType.OnDown))
			{
				SelectedSkillSlot++;
			}
			if (InputControlManager.current.DpadDown(DpadHoldType.OnDown))
			{
				SelectedSkillSlot += 2;
			}
			if (InputControlManager.current.DpadUp(DpadHoldType.OnDown))
			{
				SelectedSkillSlot -= 2;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.RightStick_Left))
			{
				SelectedSkillSlot--;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.RightStick_Right))
			{
				SelectedSkillSlot++;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.RightStick_Down))
			{
				SelectedSkillSlot += 2;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.RightStick_Up))
			{
				SelectedSkillSlot -= 2;
			}
			int num = _selectedSkillSlot + 10 * _currentSkillPage;
			switch (_currentSkillTab)
			{
			case SkillTier.NOVICE:
				if (num <= _noviceSkillEntries.Count - 1)
				{
					_selectedListEntry = _noviceSkillEntries[num];
				}
				else
				{
					_selectedListEntry = null;
				}
				break;
			case SkillTier.CLASS:
				if (num <= _classSkillEntries.Count - 1)
				{
					_selectedListEntry = _classSkillEntries[num];
				}
				else
				{
					_selectedListEntry = null;
				}
				break;
			case SkillTier.SUBCLASS:
				if (num <= _tier2SkillEntries.Count - 1)
				{
					_selectedListEntry = _tier2SkillEntries[num];
				}
				else
				{
					_selectedListEntry = null;
				}
				break;
			}
			if ((bool)_selectedListEntry && (bool)_selectedListEntry._scriptSkill)
			{
				if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && _selectedListEntry._skillRankUpButton.gameObject.activeInHierarchy)
				{
					_selectedListEntry.Init_UnlockSkill();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.RightTrigger) && InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
				{
					_selectedListEntry.OnClick_SkillIcon();
				}
			}
			return;
		}
		ToolTipManager._current._skillToolTip.Disable_ToolTip();
		if (InputControlManager.current.DpadDown(DpadHoldType.OnDown))
		{
			Select_SkillTab((int)(_currentSkillTab + 1));
		}
		if (InputControlManager.current.DpadUp(DpadHoldType.OnDown))
		{
			Select_SkillTab((int)(_currentSkillTab - 1));
		}
		if (GamepadInput.GetButtonDown(GamepadButton.RightStick_Down))
		{
			Select_SkillTab((int)(_currentSkillTab + 1));
		}
		if (GamepadInput.GetButtonDown(GamepadButton.RightStick_Up))
		{
			Select_SkillTab((int)(_currentSkillTab - 1));
		}
		if (InputControlManager.current.DpadRight(DpadHoldType.OnDown))
		{
			_gamepad_tierSkillSelection = false;
		}
		if (GamepadInput.GetButtonDown(GamepadButton.RightStick_Right))
		{
			_gamepad_tierSkillSelection = false;
		}
		if (GamepadInput.GetButton(GamepadButton.LeftTrigger))
		{
			_gamepad_previousPageIco.color = Color.white;
		}
		else
		{
			_gamepad_previousPageIco.color = Color.grey;
		}
		if (GamepadInput.GetButton(GamepadButton.RightTrigger))
		{
			_gamepad_nextPageIco.color = Color.white;
		}
		else
		{
			_gamepad_nextPageIco.color = Color.grey;
		}
		if (InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
		{
			if (GamepadInput.GetButtonDown(GamepadButton.LeftTrigger))
			{
				Init_PreviousPage();
			}
			if (GamepadInput.GetButtonDown(GamepadButton.RightTrigger))
			{
				Init_NextPage();
			}
		}
		switch (_currentSkillTab)
		{
		case SkillTier.NOVICE:
			_gamepad_tierTabCursor.anchoredPosition = new Vector2(-260f, 110f);
			break;
		case SkillTier.CLASS:
			_gamepad_tierTabCursor.anchoredPosition = new Vector2(-260f, 68f);
			break;
		case SkillTier.SUBCLASS:
			_gamepad_tierTabCursor.anchoredPosition = new Vector2(-260f, 26f);
			break;
		}
	}

	private void Handle_DragSkillIconControl()
	{
		_skillDragIconGamepadCursor.gameObject.SetActive(InputControlManager.current._setGamepadType != GamepadType.Keyboard && !Cursor.visible);
		_gamepad_contextPrompt_cancelSkill.isEnabled = _isSkillDragging && InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_skillDragIconObject.SetActive(_isSkillDragging);
		if (!_isSkillDragging)
		{
			return;
		}
		if (Player._mainPlayer._pCasting._isCasting)
		{
			_isSkillDragging = false;
			return;
		}
		_selectedListEntry = null;
		if (Cursor.visible)
		{
			_skillDragIconObject.transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			if (Input.GetMouseButton(0))
			{
				return;
			}
		}
		else
		{
			_skillDragIconObject.transform.position = ActionBarManager._current._actionSlotTransforms[_selectedActionSlot].transform.position + Vector3.up * 62.85f;
			ActionBarManager._current._currentHighlightedSlotIndex = _selectedActionSlot;
			ActionBarManager._current._isHighlightingSlot = true;
			if (InputControlManager.current.DpadLeft(DpadHoldType.OnDown))
			{
				SelectedActionSlot--;
			}
			if (InputControlManager.current.DpadRight(DpadHoldType.OnDown))
			{
				SelectedActionSlot++;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.RightStick_Left))
			{
				SelectedActionSlot--;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.RightStick_Right))
			{
				SelectedActionSlot++;
			}
			if (GamepadInput.GetButtonDown(GamepadButton.Button_East))
			{
				_isSkillDragging = false;
			}
			if (GamepadInput.GetButton(GamepadButton.RightTrigger))
			{
				return;
			}
		}
		if (ActionBarManager._current._isHighlightingSlot && !_bufferCommandRequest)
		{
			Apply_SkillToLoadoutSlot();
		}
		ProfileDataManager._current.Save_ProfileData();
		ActionBarManager._current._isHighlightingSlot = false;
		_isSkillDragging = false;
	}

	private void Apply_SkillToLoadoutSlot()
	{
		PlayerCasting pCasting = Player._mainPlayer._pCasting;
		int currentHighlightedSlotIndex = ActionBarManager._current._currentHighlightedSlotIndex;
		int num = -1;
		if (!_dragSkillStruct._skillUnlocked)
		{
			return;
		}
		for (int i = 0; i < pCasting._setSkillPresetIndex.Length; i++)
		{
			if (pCasting._setSkillPresetIndex[i] == _dragSkillStruct._skillName)
			{
				pCasting._setSkillPresetIndex[i] = string.Empty;
				num = i;
			}
		}
		if (!string.IsNullOrEmpty(Player._mainPlayer._pCasting._setSkillPresetIndex[currentHighlightedSlotIndex]) && num > -1)
		{
			Player._mainPlayer._pCasting._setSkillPresetIndex[num] = Player._mainPlayer._pCasting._setSkillPresetIndex[currentHighlightedSlotIndex];
		}
		Player._mainPlayer._pCasting._setSkillPresetIndex[currentHighlightedSlotIndex] = _dragSkillStruct._skillName;
		ActionBarManager._current.Glow_ActionBarIcon(currentHighlightedSlotIndex);
		ProfileDataManager._current.Save_ProfileData();
		_aSrc_setSkill.Play();
	}

	public void Set_DragIconInfo(ScriptableSkill _scriptSkill)
	{
		_scriptSkillDrag = _scriptSkill;
		_skillDragIconSprite.sprite = _scriptSkill._skillIcon;
		_isSkillDragging = true;
	}

	private IEnumerator Create_SkillListData()
	{
		if (_creatingSkillListEntries)
		{
			yield break;
		}
		_creatingSkillListEntries = true;
		_maxNoviceSkillPages = 0;
		_listDataEntries.Add(_recallEntry);
		_noviceSkillEntries.Add(_recallEntry);
		for (int i = 0; i < Player._mainPlayer._pCasting._skillLibrary.Count; i++)
		{
			SkillLibrarySlot skillLibrarySlot = Player._mainPlayer._pCasting._skillLibrary[i];
			GameObject gameObject = Object.Instantiate(_listDataObject_prefab);
			SkillListDataEntry component = gameObject.GetComponent<SkillListDataEntry>();
			component._scriptSkill = skillLibrarySlot._scriptSkill;
			component._skillTier = skillLibrarySlot._skillTier;
			component._skillStructIndex = skillLibrarySlot._skillStructIndex;
			gameObject.name = "_skillListDataEntry_" + component._scriptSkill._skillName;
			_listDataEntries.Add(component);
			switch (component._skillTier)
			{
			case SkillTier.NOVICE:
				gameObject.transform.SetParent(_noviceSkillContent);
				_noviceSkillEntries.Add(component);
				break;
			case SkillTier.CLASS:
				gameObject.transform.SetParent(_classSkillContent);
				_classSkillEntries.Add(component);
				break;
			case SkillTier.SUBCLASS:
				gameObject.transform.SetParent(_tier2SkillContent);
				_tier2SkillEntries.Add(component);
				break;
			}
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.position = Vector3.zero;
		}
		int num = 0;
		for (int j = 0; j < _noviceSkillEntries.Count; j++)
		{
			num++;
			if (num == 10)
			{
				_maxNoviceSkillPages++;
				num = 0;
			}
		}
		_creatingSkillListEntries = false;
	}

	public void Clear_SkillListData()
	{
		for (int i = 0; i < _listDataEntries.Count; i++)
		{
			if (!(_listDataEntries[i] == _recallEntry))
			{
				Object.Destroy(_listDataEntries[i].gameObject);
			}
		}
		_noviceSkillEntries.Clear();
		_classSkillEntries.Clear();
		_tier2SkillEntries.Clear();
		_listDataEntries.Clear();
	}

	public void Init_ClassTabTooltip(int _tabValue)
	{
		PlayerStats pStats = Player._mainPlayer._pStats;
		switch ((SkillTier)(byte)_tabValue)
		{
		case SkillTier.NOVICE:
			ToolTipManager._current.Apply_GenericToolTip("General Skills");
			break;
		case SkillTier.CLASS:
			if (!pStats._class)
			{
				return;
			}
			ToolTipManager._current.Apply_GenericToolTip(pStats._class._className + " Skills");
			break;
		case SkillTier.SUBCLASS:
			if (!pStats._class || pStats._syncClassTier <= 0)
			{
				return;
			}
			ToolTipManager._current.Apply_GenericToolTip(pStats._class._playerClassTiers[pStats._syncClassTier - 1]._classTierName + " Skills");
			break;
		}
		ToolTipManager._current._genericToolTip.Set_TooltipAnchorPos(new Vector2(0f, 50f));
		ToolTipManager._current._genericToolTip.Enable_ToolTip();
	}

	public void Disable_ClassTabToolTip()
	{
		ToolTipManager._current._genericToolTip.Disable_ToolTip();
	}
}
