using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestListDataEntry : ListDataEntry, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[Header("Quest Data Entry")]
	public ScriptableQuest _scriptableQuest;

	public bool _cannotAcceptQuest;

	[Space]
	public MenuElement _entryElement;

	public Button _questEntryButton;

	public Image _highlightImage;

	public Image _questIconImage;

	[Space]
	public Sprite _questIco_oneTime;

	public Sprite _questIco_finished;

	public Sprite _questIco_repeatable;

	public Sprite _questIco_undergoing;

	public Sprite _questIco_locked;

	public void OnPointerEnter(PointerEventData eventData)
	{
		_isHighlighted = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_isHighlighted = false;
	}

	private void Awake()
	{
		_questEntryButton.onClick.AddListener(Init_SelectEntry);
	}

	private void Init_SelectEntry()
	{
		if (InputControlManager.current._setGamepadType != GamepadType.Keyboard && InputControlManager.current._gamepadMouseCursorBuffer <= 0f)
		{
			QuestSelectionManager._current.OnClick_QuestAcceptButton();
		}
		else
		{
			QuestSelectionManager._current.Select_QuestEntry(this);
		}
	}

	private void Update()
	{
		_highlightImage.enabled = _isHighlighted || QuestSelectionManager._current._selectedQuestEntry == this;
		_cannotAcceptQuest = Player._mainPlayer._pQuest.Check_IfCannotAcceptQuest(_scriptableQuest);
		if (_cannotAcceptQuest && _scriptableQuest._questSubType == QuestSubType.MAIN_QUEST)
		{
			_dataNameText.text = $"<color=cyan>{_scriptableQuest._questName}</color> <color=red>(lv-{_scriptableQuest._questLevel})</color>";
			_questIconImage.sprite = _questIco_locked;
			return;
		}
		if ((bool)_scriptableQuest._questIco)
		{
			_questIconImage.sprite = _scriptableQuest._questIco;
		}
		else
		{
			switch (_scriptableQuest._questType)
			{
			case QuestType.SINGLE:
				_questIconImage.sprite = _questIco_oneTime;
				break;
			case QuestType.REPEATABLE:
				_questIconImage.sprite = _questIco_repeatable;
				break;
			}
		}
		int num = _scriptableQuest._questLevel - Player._mainPlayer._pStats._currentLevel;
		if (_scriptableQuest._questSubType == QuestSubType.NONE)
		{
			if (num <= -3)
			{
				_dataNameText.text = $"<color=#e8f48c>{_scriptableQuest._questName}</color> <color=#e3dac9>(lv-{_scriptableQuest._questLevel})</color>";
			}
			if (num <= -5)
			{
				_dataNameText.text = $"<color=#e8f48c>{_scriptableQuest._questName}</color> <color=#dcdcdc>(lv-{_scriptableQuest._questLevel})</color>";
			}
			if (num > -3)
			{
				_dataNameText.text = $"<color=#e8f48c>{_scriptableQuest._questName}</color> <color=yellow>(lv-{_scriptableQuest._questLevel})</color>";
			}
			if (num >= 3)
			{
				_dataNameText.text = $"<color=#e8f48c>{_scriptableQuest._questName}</color> <color=orange>(lv-{_scriptableQuest._questLevel})</color>";
			}
			if (num >= 5)
			{
				_dataNameText.text = $"<color=#e8f48c>{_scriptableQuest._questName}</color> <color=red>(lv-{_scriptableQuest._questLevel})</color>";
			}
		}
		else
		{
			switch (_scriptableQuest._questSubType)
			{
			case QuestSubType.MAIN_QUEST:
				_dataNameText.text = $"<color=cyan>{_scriptableQuest._questName} (lv-{_scriptableQuest._questLevel})</color>";
				break;
			case QuestSubType.CLASS:
				_dataNameText.text = "<color=#f7e98e>" + _scriptableQuest._questName + " (Class Tome)</color>";
				break;
			case QuestSubType.SKILL:
				_dataNameText.text = "<color=#f7e98e>" + _scriptableQuest._questName + " (Skill Scroll)</color>";
				break;
			}
		}
		for (int i = 0; i < Player._mainPlayer._pQuest._scriptableQuests.Count; i++)
		{
			if (_scriptableQuest == Player._mainPlayer._pQuest._scriptableQuests[i])
			{
				_questIconImage.sprite = _questIco_undergoing;
				if (Player._mainPlayer._pQuest._questProgressData[i]._questComplete)
				{
					_questIconImage.sprite = _questIco_finished;
					_dataNameText.color = Color.green;
					_dataNameText.text = _scriptableQuest._questName + " (Completed)";
				}
				else
				{
					_dataNameText.color = Color.white;
					_dataNameText.text = "<color=#e8f48c>" + _scriptableQuest._questName + "</color> (In Progress)";
				}
			}
		}
	}
}
