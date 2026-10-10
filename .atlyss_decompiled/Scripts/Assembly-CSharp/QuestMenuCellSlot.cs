using UnityEngine;
using UnityEngine.UI;

public class QuestMenuCellSlot : MonoBehaviour
{
	[Header("Quest Menu Cell Slot")]
	public ScriptableQuest _scriptQuest;

	public QuestProgressStruct _questProgressStruct;

	[Space]
	public bool _isHighlighted;

	public int _index;

	[Space]
	[SerializeField]
	private Button _questCellSlotButton;

	[SerializeField]
	private GameObject _questStatusIconObject;

	[SerializeField]
	private Sprite _questStatusSprite_inProgress;

	[SerializeField]
	private Sprite _questStatusSprite_completed;

	[SerializeField]
	private Text _slotTag;

	[Space]
	[SerializeField]
	private Button _trackQuestToggleButton;

	[SerializeField]
	private Image _trackQuestToggleButtonIco;

	[SerializeField]
	private Sprite _trackQuestToggle_enabledIco;

	[SerializeField]
	private Sprite _trackQuestToggle_disabledIco;

	[Space]
	[SerializeField]
	private GameObject _partyQuestCounterObject;

	[SerializeField]
	private Text _partyQuestCounterText;

	[Space]
	[SerializeField]
	private Image _slotHighlight;

	[SerializeField]
	private Image _slotFillImage;

	[SerializeField]
	private Image _questStatusIcon;

	[SerializeField]
	private Image _gamepadSelectIcon;

	private void Start()
	{
		_questCellSlotButton.onClick.AddListener(OnClick_QuestCellButton);
		_trackQuestToggleButton.onClick.AddListener(OnClick_ToggleQuestTrackButton);
	}

	public void OnClick_QuestCellButton()
	{
		QuestMenuCell._current.Select_QuestSlot(_index);
	}

	public void OnClick_ToggleQuestTrackButton()
	{
		_questProgressStruct._hideQuestTrack = !_questProgressStruct._hideQuestTrack;
		QuestTrackerManager._current.Refresh_ProgressElements();
	}

	private void Update()
	{
		if (!Player._mainPlayer || Player._mainPlayer._bufferingStatus)
		{
			return;
		}
		if (_questProgressStruct._hideQuestTrack)
		{
			_trackQuestToggleButtonIco.sprite = _trackQuestToggle_disabledIco;
		}
		else
		{
			_trackQuestToggleButtonIco.sprite = _trackQuestToggle_enabledIco;
		}
		_slotHighlight.enabled = _isHighlighted;
		_slotFillImage.enabled = _scriptQuest;
		_questStatusIconObject.SetActive(_scriptQuest);
		_gamepadSelectIcon.gameObject.SetActive(InputControlManager.current._setGamepadType != GamepadType.Keyboard && _isHighlighted);
		_trackQuestToggleButton.gameObject.SetActive((bool)_scriptQuest && !SettingsManager._current._hideQuestTracker.isOn);
		_partyQuestCounterObject.SetActive((bool)_scriptQuest && (bool)Player._mainPlayer.Network_partyObject);
		if ((bool)_scriptQuest)
		{
			_slotTag.text = $"{_scriptQuest._questName}\n(lv-{_scriptQuest._questLevel})";
			switch (_scriptQuest._questSubType)
			{
			case QuestSubType.MAIN_QUEST:
				_slotTag.text = $"<color=cyan>{_scriptQuest._questName}</color>\n<color=cyan>(lv-{_scriptQuest._questLevel})</color>";
				break;
			case QuestSubType.CLASS:
				_slotTag.text = "<color=#f7e98e>" + _scriptQuest._questName + "</color>\n<color=#f7e98e>(Class Tome)</color>";
				break;
			case QuestSubType.SKILL:
				_slotTag.text = "<color=#f7e98e>" + _scriptQuest._questName + "</color>\n<color=#f7e98e>(Skill Scroll)</color>";
				break;
			}
			if ((bool)Player._mainPlayer.Network_partyObject)
			{
				int num = 0;
				for (int i = 0; i < Player._mainPlayer.Network_partyObject._syncPartyPeers.Count; i++)
				{
					if ((bool)Player._mainPlayer.Network_partyObject._syncPartyPeers[i] && Player._mainPlayer.Network_partyObject._syncPartyPeers[i]._pQuest.Check_HasQuestTag(_scriptQuest._questName))
					{
						num++;
					}
				}
				_partyQuestCounterText.text = $"({num})";
			}
		}
		else
		{
			_slotTag.text = "Empty Slot";
		}
		if (_questProgressStruct._questComplete)
		{
			_questStatusIcon.sprite = _questStatusSprite_completed;
		}
		else
		{
			_questStatusIcon.sprite = _questStatusSprite_inProgress;
		}
	}
}
