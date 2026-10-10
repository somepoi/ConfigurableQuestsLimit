using UnityEngine;
using UnityEngine.UI;

public class ActionSlot : MonoBehaviour
{
	[Header("Action slot")]
	public ScriptableSkill _scriptSkill;

	public MenuElement _wepSwapIconElement;

	public CanvasGroup _canvasGroup;

	public CombatColliderType _slotUtilityType;

	public int _actionSlotIndex;

	[Space]
	public Image _skillIcon;

	public Image _skillCoolDown;

	public Image _skillQueueDisplay;

	public Image _gamepadDisplayIcon;

	[Space]
	public Image _offensiveSkillSlotIcon;

	[Space]
	public Image _supportSkillSlotIcon;

	public Image _supportSkillSlotOverlay;

	[Space]
	public GameObject _ammoCounterObject;

	public GameObject _manaCounterObject;

	public GameObject _healthCounterObject;

	public GameObject _staminaCounterObject;

	public Color _defaultAmmoColor;

	public Color _defaultManaColor;

	public Color _defaultHealthColor;

	public Color defaultStaminaColor;

	[Space]
	public Text _actionSlotHotkey;

	public Text _ammoCounter;

	public Text _manaCounter;

	public Text _healthCounter;

	public Text _staminaCounter;

	[Space]
	public Text _cooldownCounter;

	[Space]
	public bool _isInactive;

	private void Start()
	{
		_skillCoolDown.enabled = false;
		_cooldownCounter.text = string.Empty;
	}

	private void Update()
	{
		Assign_ScriptSkillData();
	}

	private void Assign_ScriptSkillData()
	{
		if (TabMenu._current._skillsCell._isSkillDragging && _slotUtilityType != CombatColliderType.NULL)
		{
			if (TabMenu._current._skillsCell._scriptSkillDrag._skillUtilityType == _slotUtilityType)
			{
				_canvasGroup.alpha = 1f;
			}
			else
			{
				_canvasGroup.alpha = 0.3f;
			}
		}
		else
		{
			_canvasGroup.alpha = 1f;
		}
		_offensiveSkillSlotIcon.enabled = _slotUtilityType == CombatColliderType.OFFENSIVE;
		_supportSkillSlotIcon.enabled = _slotUtilityType == CombatColliderType.SUPPORT;
		if ((bool)_scriptSkill)
		{
			_supportSkillSlotOverlay.enabled = _scriptSkill._skillUtilityType == CombatColliderType.SUPPORT;
		}
		else
		{
			_supportSkillSlotOverlay.enabled = false;
		}
		_cooldownCounter.enabled = SettingsManager._current._displaySlotCooldownCounters.isOn;
		if (_cooldownCounter.enabled && (_skillCoolDown.fillAmount <= 0f || !_skillCoolDown.enabled))
		{
			_cooldownCounter.text = string.Empty;
		}
		if (!_scriptSkill)
		{
			_skillIcon.sprite = ActionBarManager._current._emptySkillIcon;
			_skillIcon.color = ActionBarManager._current._inactiveIconColor;
			_manaCounterObject.SetActive(value: false);
			_healthCounterObject.SetActive(value: false);
			_ammoCounterObject.SetActive(value: false);
			_staminaCounterObject.SetActive(value: false);
			_skillQueueDisplay.gameObject.SetActive(value: false);
			_skillCoolDown.enabled = false;
			_cooldownCounter.text = string.Empty;
		}
		else
		{
			_skillIcon.sprite = _scriptSkill._skillIcon;
		}
		if (InputControlManager.current._setGamepadType == GamepadType.Keyboard)
		{
			string text = string.Empty;
			switch (_actionSlotIndex)
			{
			case 0:
				text = InputControlManager.current.actionBar_0.ToString();
				break;
			case 1:
				text = InputControlManager.current.actionBar_1.ToString();
				break;
			case 2:
				text = InputControlManager.current.actionBar_2.ToString();
				break;
			case 3:
				text = InputControlManager.current.actionBar_3.ToString();
				break;
			case 4:
				text = InputControlManager.current.actionBar_4.ToString();
				break;
			case 5:
				text = InputControlManager.current.actionBar_5.ToString();
				break;
			}
			text = InputControlManager.current.Convert_KeyCodeName(text);
			_actionSlotHotkey.text = text;
		}
		else
		{
			_actionSlotHotkey.text = string.Empty;
		}
	}
}
