using UnityEngine;
using UnityEngine.UI;

public class QuestMenuCell : TabMenuCell
{
	public static QuestMenuCell _current;

	private ScriptableQuest _selectedQuest;

	[Header("Quest Menu")]
	[SerializeField]
	private RectTransform _contentTransform;

	[Space]
	[SerializeField]
	private Text _questHeaderText;

	[SerializeField]
	private Text _questSummaryText;

	[SerializeField]
	private Text _questErrandsText;

	[SerializeField]
	private Text _questLogCounterText;

	[SerializeField]
	private Text _finishedQuestCounterText;

	[Space]
	[SerializeField]
	private GameObject _questListDollyObject;

	[SerializeField]
	private QuestMenuCellSlot[] _questMenuCellSlots;

	[Space]
	private int _selectedQuestSlotIndex;

	private float _commandBuffer;

	private bool _questSelected;

	[Header("Share Quest Params")]
	[SerializeField]
	private Button _shareQuestButton;

	[SerializeField]
	private Text _shareQuestButtonText;

	[Header("Abandon Quest Params")]
	public Button _abandonQuestButton;

	[SerializeField]
	private Text _abandonQuestButtonText;

	[Header("Objective Item Panel")]
	[SerializeField]
	private Text _questObjectiveHeaderText;

	[SerializeField]
	private GameObject _objectiveItemPanel;

	[SerializeField]
	private ItemListDataEntry _objectiveItemEntry;

	[Header("Rewards Panel")]
	[SerializeField]
	private GameObject _rewardsPanelObject;

	[SerializeField]
	private Text _rewardsPanelText_experience;

	[SerializeField]
	private Text _rewardsPanelText_currency;

	[SerializeField]
	private GameObject _itemRewardsListObject;

	[SerializeField]
	private ItemListDataEntry[] _itemRewardDataEntries;

	[Header("Gamepad Elements")]
	[SerializeField]
	private Image _gamepad_shareQuestIco;

	[SerializeField]
	private Image _gamepad_abandonQuestIco;

	[SerializeField]
	private Image _gamepad_abandonQuestIco_01;

	[Space]
	[SerializeField]
	private Image _gamepad_descriptorScrollHighlight;

	[SerializeField]
	private bool _gamepad_descriptorScrollMode;

	[Space]
	[SerializeField]
	private MenuElement _gamepad_contextPromptElement_southButton;

	public int SelectedQuestSlotIndex
	{
		get
		{
			return _selectedQuestSlotIndex;
		}
		set
		{
			if (value < 0)
			{
				value = _questMenuCellSlots.Length - 1;
			}
			if (value > _questMenuCellSlots.Length - 1)
			{
				value = 0;
			}
			if (_selectedQuestSlotIndex != value && TabMenu._current._isOpen && TabMenu._current._selectedMenuCell == this)
			{
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			_selectedQuestSlotIndex = value;
			Select_QuestSlot(_selectedQuestSlotIndex);
		}
	}

	public override void Cell_OnAwake()
	{
		_current = this;
	}

	private void Start()
	{
		_shareQuestButton.onClick.AddListener(OnClick_ShareQuest);
		_abandonQuestButton.onClick.AddListener(OnClick_AbandonQuest);
	}

	private void Update()
	{
		Handle_CellUpdate();
	}

	public override void Handle_CellUpdate()
	{
		if (!Player._mainPlayer)
		{
			return;
		}
		PlayerQuesting pQuest = Player._mainPlayer._pQuest;
		for (int i = 0; i < _questMenuCellSlots.Length; i++)
		{
			_questMenuCellSlots[i]._isHighlighted = i == _selectedQuestSlotIndex;
			if (i <= pQuest._scriptableQuests.Count - 1 && (bool)pQuest._scriptableQuests[i])
			{
				_questMenuCellSlots[i]._scriptQuest = pQuest._scriptableQuests[i];
				_questMenuCellSlots[i]._questProgressStruct = pQuest._questProgressData[i];
			}
			else
			{
				_questMenuCellSlots[i]._scriptQuest = null;
				_questMenuCellSlots[i]._questProgressStruct = new QuestProgressStruct();
			}
		}
		if (!TabMenu._current._isOpen || TabMenu._current._selectedMenuCell != this)
		{
			_selectedQuestSlotIndex = 0;
			_gamepad_descriptorScrollMode = false;
			Select_QuestSlot(_selectedQuestSlotIndex);
			return;
		}
		if (_commandBuffer > 0f)
		{
			_commandBuffer -= Time.deltaTime;
		}
		else
		{
			_commandBuffer = 0f;
		}
		if (_shareQuestButton.interactable)
		{
			_shareQuestButtonText.color = Color.white;
		}
		else
		{
			_shareQuestButtonText.color = Color.grey;
		}
		if (_abandonQuestButton.interactable)
		{
			_abandonQuestButtonText.color = Color.white;
		}
		else
		{
			_abandonQuestButtonText.color = Color.grey;
		}
		_questListDollyObject.SetActive(!TabMenu._current._questPanelMode);
		_questLogCounterText.text = $"Quest Log ({pQuest._questProgressData.Count} / {pQuest._questLogLimit})";
		_questSelected = (bool)_questMenuCellSlots[_selectedQuestSlotIndex]._scriptQuest && _commandBuffer <= 0f;
		_gamepad_shareQuestIco.color = _shareQuestButtonText.color;
		_gamepad_abandonQuestIco.color = _abandonQuestButtonText.color;
		_gamepad_abandonQuestIco_01.color = _gamepad_abandonQuestIco.color;
		_shareQuestButton.interactable = _questSelected && (bool)Player._mainPlayer.Network_partyObject;
		_abandonQuestButton.interactable = _questSelected;
		Handle_QuestProgressTracking();
		Handle_GamepadControl();
	}

	private void Handle_QuestProgressTracking()
	{
		int num = ProfileDataManager._current._characterFile._questProgressProfile._finishedQuests.Length;
		if (ProfileDataManager._current._characterFile._questProgressProfile._finishedQuests != null)
		{
			_finishedQuestCounterText.text = $"Completed Quests: {num}";
		}
		else
		{
			_finishedQuestCounterText.text = "Completed Quests: 0";
		}
		PlayerQuesting pQuest = Player._mainPlayer._pQuest;
		_questErrandsText.text = string.Empty;
		_questErrandsText.gameObject.SetActive(pQuest._questProgressData.Count > 0 && pQuest._scriptableQuests.Contains(_selectedQuest));
		if (QuestTrackerManager._current._refreshingElements || pQuest._questProgressData.Count <= 0 || !_selectedQuest)
		{
			return;
		}
		for (int i = 0; i < pQuest._questProgressData.Count; i++)
		{
			if (pQuest._questProgressData[i]._questTag != _selectedQuest._questName)
			{
				continue;
			}
			if (!pQuest._questProgressData[i]._questComplete)
			{
				_questErrandsText.text += QuestTrackerManager._current._questTrackElements[i]._trackElementText.text;
				continue;
			}
			for (int j = 0; j < pQuest._scriptableQuests.Count; j++)
			{
				if (!(pQuest._scriptableQuests[j]._questName != pQuest._questProgressData[i]._questTag))
				{
					_questErrandsText.text = "<color=yellow>" + pQuest._scriptableQuests[j]._questCompleteReturnMessage + "</color>\n\n" + QuestTrackerManager._current._questTrackElements[i]._trackElementText.text;
				}
			}
		}
	}

	private void Handle_GamepadControl()
	{
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_gamepad_shareQuestIco.enabled = flag;
		_gamepad_abandonQuestIco.enabled = flag;
		_gamepad_abandonQuestIco_01.enabled = flag;
		_gamepad_descriptorScrollHighlight.enabled = flag && _gamepad_descriptorScrollMode;
		_gamepad_contextPromptElement_southButton.isEnabled = flag && (bool)_selectedQuest && !_gamepad_descriptorScrollMode && !DialogManager._current._isDialogEnabled && !SettingsManager._current._hideQuestTracker.isOn;
		if (!flag || TabMenu._current._selectedMenuCell != this)
		{
			return;
		}
		bool flag2 = InputControlManager.current.DpadUp(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Up);
		bool flag3 = InputControlManager.current.DpadDown(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Down);
		bool flag4 = InputControlManager.current.DpadLeft(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Left);
		bool flag5 = InputControlManager.current.DpadRight(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Right);
		bool flag6 = InputControlManager.current.DpadUp(DpadHoldType.OnHold) || GamepadInput.GetButton(GamepadButton.RightStick_Up);
		bool flag7 = InputControlManager.current.DpadDown(DpadHoldType.OnHold) || GamepadInput.GetButton(GamepadButton.RightStick_Down);
		if (GamepadInput.GetButtonDown(GamepadButton.RightStick))
		{
			_contentTransform.anchoredPosition = Vector2.zero;
		}
		if (!DialogManager._current._isDialogEnabled)
		{
			if (_questSelected)
			{
				if (GamepadInput.GetButton(GamepadButton.LeftTrigger) && GamepadInput.GetButtonDown(GamepadButton.Button_East))
				{
					OnClick_AbandonQuest();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_West))
				{
					OnClick_ShareQuest();
				}
			}
			if (!_gamepad_descriptorScrollMode)
			{
				if (flag3)
				{
					SelectedQuestSlotIndex++;
				}
				if (flag2)
				{
					SelectedQuestSlotIndex--;
				}
				if (flag5)
				{
					_gamepad_descriptorScrollMode = true;
					TabMenu._current._gamepadSelectAsrc.Play();
				}
				if (GamepadInput.GetButtonDown(GamepadButton.Button_South) && (bool)_selectedQuest && !SettingsManager._current._hideQuestTracker.isOn)
				{
					_questMenuCellSlots[_selectedQuestSlotIndex].OnClick_ToggleQuestTrackButton();
					TabMenu._current._gamepadSelectAsrc.Play();
				}
			}
			else if (flag4)
			{
				_gamepad_descriptorScrollMode = false;
				TabMenu._current._gamepadSelectAsrc.Play();
			}
		}
		else
		{
			_gamepad_descriptorScrollMode = DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Tab_Menu;
		}
		if (_gamepad_descriptorScrollMode)
		{
			if (flag7)
			{
				_contentTransform.anchoredPosition += Vector2.up * 4f;
			}
			if (flag6)
			{
				_contentTransform.anchoredPosition -= Vector2.up * 4f;
			}
		}
	}

	public void Select_QuestSlot(int _index)
	{
		_selectedQuestSlotIndex = _index;
		if ((bool)_questMenuCellSlots[_index]._scriptQuest)
		{
			Apply_QuestInfo(_questMenuCellSlots[_index]._scriptQuest);
		}
		else
		{
			Clear_DisplayQuestData();
		}
	}

	public void Apply_QuestInfo(ScriptableQuest _scriptQuest)
	{
		if (_selectedQuest == _scriptQuest)
		{
			return;
		}
		PlayerQuesting pQuest = Player._mainPlayer._pQuest;
		_selectedQuest = _scriptQuest;
		_questHeaderText.text = $"{_scriptQuest._questName} (lv-{_scriptQuest._questLevel})";
		if (pQuest.Check_IfCannotAcceptQuest(_scriptQuest))
		{
			_questSummaryText.text = string.Empty;
			_questSummaryText.text += "Quest Locked.";
			_objectiveItemPanel.SetActive(value: false);
			_rewardsPanelObject.SetActive(value: false);
			_itemRewardsListObject.SetActive(value: false);
			ReAlign_TextObjects();
			return;
		}
		_questSummaryText.text = string.Empty;
		_questSummaryText.text += _scriptQuest._questDescription;
		ReAlign_TextObjects();
		int num = (int)((float)(int)GameManager._current._statLogics._experienceCurve.Evaluate(_scriptQuest._questLevel) * _scriptQuest._questExperiencePercentage);
		_objectiveItemPanel.SetActive(_scriptQuest._questObjectiveItem._scriptItem);
		_rewardsPanelObject.SetActive(num > 0 || _scriptQuest._questCurrencyReward > 0 || _scriptQuest._questItemRewards.Length != 0);
		if ((bool)_scriptQuest._questObjectiveItem._scriptItem)
		{
			_objectiveItemEntry._scriptableItem = _scriptQuest._questObjectiveItem._scriptItem;
			_objectiveItemEntry._itemData._itemName = _scriptQuest._questObjectiveItem._scriptItem._itemName;
			_objectiveItemEntry._itemData._maxQuantity = _scriptQuest._questObjectiveItem._scriptItem._maxStackAmount;
			_objectiveItemEntry._itemData._quantity = _scriptQuest._questObjectiveItem._itemQuantity;
			if ((bool)_scriptQuest._questObjectiveItem._scriptableStatModifier)
			{
				_objectiveItemEntry._itemData._modifierID = _scriptQuest._questObjectiveItem._scriptableStatModifier._modifierID;
			}
			_objectiveItemEntry.Apply_ItemDataInfo();
		}
		if (num > 0)
		{
			_rewardsPanelText_experience.text = $"{num} exp";
		}
		else
		{
			_rewardsPanelText_experience.text = "";
		}
		if (_scriptQuest._questCurrencyReward > 0)
		{
			_rewardsPanelText_currency.text = $"{_scriptQuest._questCurrencyReward} {GameManager._current._statLogics._currencyName}";
		}
		else
		{
			_rewardsPanelText_currency.text = "";
		}
		_itemRewardsListObject.SetActive(_scriptQuest._questItemRewards.Length != 0);
		for (int i = 0; i < _itemRewardDataEntries.Length; i++)
		{
			_itemRewardDataEntries[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < _scriptQuest._questItemRewards.Length; j++)
		{
			_itemRewardDataEntries[j].gameObject.SetActive(value: true);
			_itemRewardDataEntries[j]._scriptableItem = _scriptQuest._questItemRewards[j]._scriptItem;
			_itemRewardDataEntries[j]._itemData._itemName = _scriptQuest._questItemRewards[j]._scriptItem._itemName;
			_itemRewardDataEntries[j]._itemData._maxQuantity = _scriptQuest._questItemRewards[j]._scriptItem._maxStackAmount;
			_itemRewardDataEntries[j]._itemData._quantity = _scriptQuest._questItemRewards[j]._itemQuantity;
			if ((bool)_scriptQuest._questItemRewards[j]._scriptableStatModifier)
			{
				_itemRewardDataEntries[j]._itemData._modifierID = _scriptQuest._questItemRewards[j]._scriptableStatModifier._modifierID;
			}
			_itemRewardDataEntries[j].Apply_ItemDataInfo();
		}
		void ReAlign_TextObjects()
		{
			_questErrandsText.transform.SetParent(null);
			_questSummaryText.transform.SetParent(null);
			_questObjectiveHeaderText.transform.SetParent(null);
			_objectiveItemPanel.transform.SetParent(null);
			_rewardsPanelObject.transform.SetParent(null);
			_contentTransform.anchoredPosition = Vector2.zero;
			_questSummaryText.transform.SetParent(_contentTransform);
			_questErrandsText.transform.SetParent(_contentTransform);
			_questObjectiveHeaderText.transform.SetParent(_contentTransform);
			_objectiveItemPanel.transform.SetParent(_contentTransform);
			_rewardsPanelObject.transform.SetParent(_contentTransform);
			_questObjectiveHeaderText.transform.localScale = Vector2.one;
			_questSummaryText.transform.localScale = Vector2.one;
			_questErrandsText.transform.localScale = Vector2.one;
			_objectiveItemPanel.transform.localScale = Vector2.one;
			_rewardsPanelObject.transform.localScale = Vector2.one;
		}
	}

	public void OnClick_AbandonQuest()
	{
		Player._mainPlayer._pQuest.Abandon_Quest(_selectedQuestSlotIndex);
		Player._mainPlayer._pSound._aSrcPlayerUI.PlayOneShot(Player._mainPlayer._pSound._questAbandonSfx, 1f);
		Clear_DisplayQuestData();
		_commandBuffer = 1.25f;
	}

	public void OnClick_ShareQuest()
	{
		Player._mainPlayer._pQuest.Init_ShareQuest(_selectedQuest);
		_commandBuffer = 1.25f;
	}

	public void Clear_DisplayQuestData()
	{
		_questHeaderText.text = "-";
		_questSummaryText.text = string.Empty;
		_questErrandsText.text = string.Empty;
		_abandonQuestButton.interactable = false;
		_rewardsPanelObject.SetActive(value: false);
		_objectiveItemPanel.SetActive(value: false);
		_selectedQuest = null;
	}
}
