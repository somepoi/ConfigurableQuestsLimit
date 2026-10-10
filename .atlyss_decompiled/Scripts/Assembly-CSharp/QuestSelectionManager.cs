using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestSelectionManager : MonoBehaviour
{
	public static QuestSelectionManager _current;

	[Header("Quest Selection Management")]
	public MenuElement _questSelectionElement;

	[SerializeField]
	private GameObject _questSelectionPrefab;

	[SerializeField]
	private Text _questSelectionHeader;

	[SerializeField]
	private Text _emptyQuestSelectionTag;

	[SerializeField]
	private Transform _questSelectionContainer;

	private ScriptableQuest _cachedRepeatable;

	private int _availableQuests;

	[Space]
	[SerializeField]
	private Button _questAcceptButton;

	[SerializeField]
	private Text _questAcceptButtonText;

	[Space]
	public QuestListDataEntry _selectedQuestEntry;

	private readonly List<QuestListDataEntry> _questEntries = new List<QuestListDataEntry>();

	[Header("Experience Display")]
	[SerializeField]
	private Text _text_currentLevel;

	[SerializeField]
	private Text _text_experienceCounter;

	[SerializeField]
	private Image _barFill_experience;

	private float _expFillAmount;

	private float _bufferToOpenWindow = 0.15f;

	private float _questAcceptButtonBuffer;

	[Header("Gamepad Elements")]
	[SerializeField]
	private Image _gamepadIco_acceptQuest;

	private readonly List<SettingsCell> _gamePadEntryCells = new List<SettingsCell>();

	private int _currentSelectedEntry;

	public int CurrentSelectedEntry
	{
		get
		{
			return _currentSelectedEntry;
		}
		set
		{
			if (value < 0)
			{
				value = _gamePadEntryCells.Count - 1;
			}
			if (value > _gamePadEntryCells.Count - 1)
			{
				value = 0;
			}
			_currentSelectedEntry = value;
		}
	}

	private void Awake()
	{
		_current = this;
	}

	private void Start()
	{
		_questAcceptButton.onClick.AddListener(OnClick_QuestAcceptButton);
	}

	private void Update()
	{
		TabMenu._current._questPanelMode = DialogManager._current._currentDialogUIPrompt == DialogUIPrompt.QUEST;
		if (DialogManager._current._currentDialogUIPrompt == DialogUIPrompt.QUEST)
		{
			if (_bufferToOpenWindow > 0f)
			{
				CurrentSelectedEntry = _gamePadEntryCells.Count - 1;
				_bufferToOpenWindow -= Time.deltaTime;
			}
			else
			{
				_questSelectionElement.gameObject.SetActive(value: true);
				_questSelectionElement.isEnabled = true;
			}
		}
		else
		{
			_gamePadEntryCells.Clear();
			CurrentSelectedEntry = 0;
			_cachedRepeatable = null;
			_questSelectionElement.gameObject.SetActive(value: false);
			_bufferToOpenWindow = 0.15f;
		}
		if (_questSelectionElement.isEnabled)
		{
			Handle_QuestSelectControl();
			Handle_QuestSelector();
		}
	}

	private void Handle_QuestSelectControl()
	{
		if (DialogManager._current._currentDialogUIPrompt != DialogUIPrompt.QUEST)
		{
			return;
		}
		bool flag = InputControlManager.current._setGamepadType != GamepadType.Keyboard;
		_gamepadIco_acceptQuest.enabled = flag && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window;
		for (int i = 0; i < _gamePadEntryCells.Count; i++)
		{
			if (_gamePadEntryCells[i] == null)
			{
				_gamePadEntryCells.RemoveAt(i);
				break;
			}
			_gamePadEntryCells[i]._isSelected = flag && DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window && i == _currentSelectedEntry;
		}
		if (_questEntries.Count <= 0)
		{
			_selectedQuestEntry = null;
		}
		else if (!flag || DialogManager._current._currentGamepadTab == DialogManager_GamepadTab.Prompt_Window)
		{
			bool num = InputControlManager.current.DpadDown(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Down) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Down);
			bool flag2 = InputControlManager.current.DpadUp(DpadHoldType.OnDown) || GamepadInput.GetButtonDown(GamepadButton.RightStick_Up) || GamepadInput.GetButtonDown(GamepadButton.LeftStick_Up);
			if (num)
			{
				CurrentSelectedEntry--;
				Select_QuestEntry(_questEntries[_currentSelectedEntry]);
				_questEntries[_currentSelectedEntry].GetComponent<ButtonSound>().Play_ClickSound();
				TabMenu._current._gamepadSelectAsrc.Play();
			}
			if (flag2)
			{
				CurrentSelectedEntry++;
				Select_QuestEntry(_questEntries[_currentSelectedEntry]);
				_questEntries[_currentSelectedEntry].GetComponent<ButtonSound>().Play_ClickSound();
				TabMenu._current._gamepadSelectAsrc.Play();
			}
		}
	}

	private void Handle_QuestSelector()
	{
		PlayerQuesting pQuest = Player._mainPlayer._pQuest;
		_emptyQuestSelectionTag.enabled = _availableQuests <= 0;
		if (_questAcceptButtonBuffer > 0f)
		{
			_questAcceptButtonBuffer -= Time.deltaTime;
			_questAcceptButton.interactable = false;
		}
		else
		{
			_questAcceptButtonBuffer = 0f;
		}
		if ((bool)_selectedQuestEntry)
		{
			if (pQuest._questProgressData.Count > 0)
			{
				for (int i = 0; i < pQuest._scriptableQuests.Count; i++)
				{
					if (_selectedQuestEntry._scriptableQuest == pQuest._scriptableQuests[i])
					{
						if (!pQuest._questProgressData[i]._questComplete)
						{
							_questAcceptButtonText.color = Color.grey;
							_questAcceptButtonText.text = "Quest Incomplete";
							_questAcceptButton.interactable = false;
						}
						else
						{
							_questAcceptButtonText.color = Color.white;
							_questAcceptButtonText.text = "Complete Quest";
							_questAcceptButton.interactable = true;
						}
					}
				}
			}
		}
		else
		{
			_questAcceptButtonText.color = Color.grey;
			_questAcceptButtonText.text = "Select a Quest";
			_questAcceptButton.interactable = false;
		}
		Handle_Expbar();
		void Handle_Expbar()
		{
			if ((bool)GameManager._current)
			{
				GameManager current = GameManager._current;
				PlayerStats pStats = Player._mainPlayer._pStats;
				StatStruct statStruct = pStats._statStruct;
				_text_currentLevel.text = $"{pStats._currentLevel}";
				if (pStats._currentLevel < current._statLogics._maxMainLevel)
				{
					float num = (float)pStats._currentExp / (float)statStruct._experience * 100f;
					_text_experienceCounter.text = string.Format("EXP {0} / {1} ({2}%)", pStats._currentExp, pStats._statStruct._experience, num.ToString("F2"));
					_expFillAmount += ((float)pStats._currentExp / (float)statStruct._experience - _expFillAmount) * 0.2f;
					if (float.IsNaN(_expFillAmount))
					{
						_expFillAmount = 0f;
					}
					_barFill_experience.fillAmount = _expFillAmount;
				}
				else
				{
					_text_experienceCounter.text = "MAX";
					_barFill_experience.fillAmount = 1f;
				}
			}
		}
	}

	public void Create_QuestListing(ScriptableQuest[] _scriptableQuests)
	{
		Clear_QuestListing();
		PlayerQuesting pQuest = Player._mainPlayer._pQuest;
		_availableQuests = 0;
		foreach (ScriptableQuest scriptableQuest in _scriptableQuests)
		{
			if (pQuest._finishedQuests.Count > 0 && (bool)scriptableQuest && pQuest._finishedQuests.ContainsKey(scriptableQuest._questName) && scriptableQuest._questType == QuestType.SINGLE)
			{
				continue;
			}
			if (pQuest.Check_IfCannotAcceptQuest(scriptableQuest))
			{
				if (scriptableQuest._questSubType != QuestSubType.MAIN_QUEST || scriptableQuest._questLevel - Player._mainPlayer._pStats._currentLevel >= 5)
				{
					continue;
				}
				bool flag = false;
				for (int j = 0; j < scriptableQuest._preQuestRequirements.Length; j++)
				{
					if (!pQuest._finishedQuests.ContainsKey(scriptableQuest._preQuestRequirements[j]._questName))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
			}
			GameObject obj = Object.Instantiate(_questSelectionPrefab, Vector3.zero, Quaternion.identity);
			QuestListDataEntry component = obj.GetComponent<QuestListDataEntry>();
			obj.transform.localScale = Vector3.zero;
			component.transform.SetParent(_questSelectionContainer);
			component._scriptableQuest = scriptableQuest;
			_questEntries.Add(component);
			_availableQuests++;
		}
		_gamePadEntryCells.AddRange(_questSelectionContainer.GetComponentsInChildren<SettingsCell>());
		StartCoroutine(Set_SelectedEntry());
	}

	private IEnumerator Set_SelectedEntry()
	{
		yield return new WaitForSeconds(0.12f);
		if ((bool)_cachedRepeatable)
		{
			for (int i = 0; i < _questEntries.Count; i++)
			{
				if (_questEntries[i]._scriptableQuest == _cachedRepeatable)
				{
					CurrentSelectedEntry = i;
					Select_QuestEntry(_questEntries[i]);
					_cachedRepeatable = null;
				}
			}
		}
		else
		{
			_currentSelectedEntry = _gamePadEntryCells.Count - 1;
			if (_questEntries.Count > 0)
			{
				Select_QuestEntry(_questEntries[_currentSelectedEntry]);
			}
		}
	}

	public void Clear_QuestListing()
	{
		_gamePadEntryCells.Clear();
		for (int i = 0; i < _questEntries.Count; i++)
		{
			Object.Destroy(_questEntries[i].gameObject);
		}
		_questEntries.Clear();
		_questAcceptButton.interactable = false;
		_selectedQuestEntry = null;
		TabMenu._current._questCell.Clear_DisplayQuestData();
	}

	public void OnClick_QuestAcceptButton()
	{
		if (!_selectedQuestEntry || _questAcceptButtonBuffer > 0f)
		{
			return;
		}
		if (_selectedQuestEntry._cannotAcceptQuest)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Quest Locked");
			return;
		}
		PlayerQuesting pQuest = Player._mainPlayer._pQuest;
		if (pQuest._questProgressData.Count > 0)
		{
			for (int i = 0; i < pQuest._questProgressData.Count; i++)
			{
				if (!(_selectedQuestEntry._scriptableQuest == pQuest._scriptableQuests[i]))
				{
					continue;
				}
				if (pQuest._questProgressData[i]._questComplete)
				{
					pQuest.Client_CompleteQuest(i);
					if (_selectedQuestEntry._scriptableQuest._questType == QuestType.REPEATABLE)
					{
						_cachedRepeatable = _selectedQuestEntry._scriptableQuest;
					}
					Create_QuestListing(DialogManager._current._scriptableDialog._scriptableQuests);
				}
				else
				{
					ErrorPromptTextManager.current.Init_ErrorPrompt("Quest Incomplete");
				}
				return;
			}
		}
		if (pQuest._scriptableQuests.Count >= pQuest._questLogLimit)
		{
			ErrorPromptTextManager.current.Init_ErrorPrompt("Quest Log Full");
			return;
		}
		_questAcceptButton.interactable = false;
		pQuest.Accept_Quest(_selectedQuestEntry._scriptableQuest);
		_questAcceptButtonBuffer = 0.25f;
	}

	public void Select_QuestEntry(QuestListDataEntry _questEntry)
	{
		PlayerQuesting pQuest = Player._mainPlayer._pQuest;
		for (int i = 0; i < _questEntries.Count; i++)
		{
			_questEntries[i]._isHighlighted = false;
		}
		_selectedQuestEntry = _questEntry;
		_selectedQuestEntry._isHighlighted = true;
		bool flag = false;
		for (int j = 0; j < pQuest._scriptableQuests.Count; j++)
		{
			if (pQuest._scriptableQuests[j] == _selectedQuestEntry._scriptableQuest)
			{
				flag = true;
			}
		}
		TabMenu._current._questCell.Apply_QuestInfo(_selectedQuestEntry._scriptableQuest);
		_questAcceptButton.interactable = !_selectedQuestEntry._cannotAcceptQuest;
		if (!flag)
		{
			_questAcceptButtonText.text = "Accept Quest";
		}
		if (_selectedQuestEntry._cannotAcceptQuest)
		{
			_questAcceptButtonText.text = "Quest Locked";
		}
		if (_questAcceptButton.interactable)
		{
			_questAcceptButtonText.color = Color.white;
		}
		else
		{
			_questAcceptButtonText.color = Color.grey;
		}
	}
}
