using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillListDataEntry : ListDataEntry
{
	[Header("Skill List Data Entry")]
	public ScriptableSkill _scriptSkill;

	public SkillTier _skillTier;

	[Space]
	public SkillStruct _skillStruct;

	public int _skillStructIndex;

	[Space]
	public bool _emptySkillStruct;

	public bool _isBonusSkillStruct;

	[Space]
	[SerializeField]
	private TextMeshProUGUI _skillNameText;

	[SerializeField]
	private TextMeshProUGUI _skillRankText;

	[Space]
	[SerializeField]
	private Button _inputPointButton;

	[SerializeField]
	private Image _gamepadIcon_rankUp;

	[Space]
	[SerializeField]
	private GameObject _primaryLoadoutIcon;

	[SerializeField]
	private GameObject _altLoadoutIcon;

	[Space]
	[SerializeField]
	private RectTransform _skillIconTransform;

	[SerializeField]
	private Image _skillBackdrop;

	[Space]
	[SerializeField]
	private Image _offensiveSkillIcon;

	[SerializeField]
	private Image _supportSkillIcon;

	[Space]
	[SerializeField]
	private Color _offensiveSkillBackdropColor = Color.white;

	[SerializeField]
	private Color _supportSkillBackdropColor = Color.white;

	[SerializeField]
	private Color _passiveSkillBackdropColor = Color.white;

	[Space]
	[SerializeField]
	private Image _skillIcon;

	[SerializeField]
	private Image _skillIconOverlay;

	[SerializeField]
	private Image _skillIconoverlay_support;

	[SerializeField]
	private Sprite _skillOverlaySprite;

	[SerializeField]
	private Sprite _passiveOverlaySprite;

	[Space]
	public Button _skillRankUpButton;

	[Header("Tooltip Config")]
	[SerializeField]
	private Vector3 _skillToolTipAnchor = new Vector3(0f, 0f, 0f);

	[Header("Audio")]
	[SerializeField]
	private AudioSource _aSrc_inputPoint;

	private void Start()
	{
		Apply_SkillData();
	}

	private void Apply_SkillData()
	{
		if ((bool)_inputPointButton)
		{
			_inputPointButton.onClick.AddListener(Init_UnlockSkill);
		}
		if (!_scriptSkill)
		{
			return;
		}
		_skillNameText.text = _scriptSkill._skillName;
		if ((bool)_skillBackdrop)
		{
			switch (_scriptSkill._skillUtilityType)
			{
			case CombatColliderType.OFFENSIVE:
				_skillBackdrop.color = _offensiveSkillBackdropColor;
				break;
			case CombatColliderType.SUPPORT:
				_skillBackdrop.color = _supportSkillBackdropColor;
				break;
			}
			_offensiveSkillIcon.enabled = _scriptSkill._skillUtilityType == CombatColliderType.OFFENSIVE && _scriptSkill._skillControlType != SkillControlType.Passive;
			_supportSkillIcon.enabled = _scriptSkill._skillUtilityType == CombatColliderType.SUPPORT && _scriptSkill._skillControlType != SkillControlType.Passive;
		}
		if ((bool)_skillIconoverlay_support)
		{
			_skillIconoverlay_support.enabled = _scriptSkill._skillUtilityType == CombatColliderType.SUPPORT;
		}
		if (_scriptSkill._skillControlType == SkillControlType.Passive)
		{
			_skillIconOverlay.sprite = _passiveOverlaySprite;
			_skillBackdrop.color = _passiveSkillBackdropColor;
		}
		else
		{
			_skillIconOverlay.sprite = _skillOverlaySprite;
		}
	}

	private bool Check_CanBuySkill()
	{
		if (Player._mainPlayer._pStats._currentSkillPoints < _scriptSkill._skillRankParams._skillPointCost)
		{
			return false;
		}
		return true;
	}

	public void Init_UnlockSkill()
	{
		if (Check_CanBuySkill())
		{
			Player._mainPlayer._pCasting.Client_Unlock_Skill(_dataID);
			if ((bool)_aSrc_inputPoint)
			{
				_aSrc_inputPoint.Play();
			}
		}
	}

	private void Update()
	{
		if (!_scriptSkill)
		{
			return;
		}
		_dataID = _skillStruct._skillLibraryIndex;
		if ((bool)_skillIcon)
		{
			_skillIcon.sprite = _scriptSkill._skillIcon;
			if (!_skillStruct._skillUnlocked)
			{
				_skillIcon.color = Color.gray;
			}
			else
			{
				_skillIcon.color = Color.white;
			}
			if (Player._mainPlayer._playerZoneType != ZoneType.Safe && (bool)_skillIcon)
			{
				_skillIcon.color = Color.gray;
			}
		}
		if (!_emptySkillStruct)
		{
			_skillStruct = Player._mainPlayer._pCasting._playerSkillStructs[_skillStructIndex];
			if ((bool)_skillRankUpButton && (bool)_gamepadIcon_rankUp)
			{
				_gamepadIcon_rankUp.enabled = _skillRankUpButton.interactable && InputControlManager.current._setGamepadType != GamepadType.Keyboard && !TabMenu._current._skillsCell._gamepad_tierSkillSelection && TabMenu._current._skillsCell._selectedListEntry == this;
			}
		}
		else
		{
			_skillStruct = new SkillStruct
			{
				_skillUnlocked = false
			};
		}
		if ((bool)_skillRankUpButton)
		{
			_skillRankUpButton.gameObject.SetActive(!_skillStruct._skillUnlocked);
			_skillRankUpButton.interactable = Check_CanBuySkill();
		}
		if ((bool)_skillRankText)
		{
			if (_skillStruct._skillUnlocked)
			{
				if (_scriptSkill._skillControlType == SkillControlType.Passive)
				{
					_skillRankText.text = "Passive Skill";
				}
				else
				{
					switch (_scriptSkill._skillUtilityType)
					{
					case CombatColliderType.OFFENSIVE:
						_skillRankText.text = "Action Skill";
						break;
					case CombatColliderType.SUPPORT:
						_skillRankText.text = "Support Skill";
						break;
					}
				}
			}
			else
			{
				_skillRankText.text = $"Point Cost: {_scriptSkill._skillRankParams._skillPointCost}";
			}
		}
		if (!_primaryLoadoutIcon || !_altLoadoutIcon)
		{
			return;
		}
		bool active = false;
		bool active2 = false;
		for (int i = 0; i < Player._mainPlayer._pCasting._skillPresetProfile._skillPresetIndex.Length; i++)
		{
			if (Player._mainPlayer._pCasting._skillPresetProfile._skillPresetIndex[i] == _scriptSkill._skillName)
			{
				active = true;
			}
			if (Player._mainPlayer._pCasting._skillPresetProfile._altSkillPresetIndex[i] == _scriptSkill._skillName)
			{
				active2 = true;
			}
		}
		_primaryLoadoutIcon.SetActive(active);
		_altLoadoutIcon.SetActive(active2);
	}

	public void Init_SkillIconIndicationScale()
	{
		if (_skillStruct._skillUnlocked && _scriptSkill._skillControlType != SkillControlType.Passive && Player._mainPlayer._playerZoneType == ZoneType.Safe)
		{
			_skillIconTransform.localScale = Vector3.one * 1.5f;
		}
	}

	public void Init_SkillToolTipInfo()
	{
		if (!Player._mainPlayer || Player._mainPlayer._bufferingStatus)
		{
			return;
		}
		SkillStruct skillStruct = (_emptySkillStruct ? new SkillStruct
		{
			_skillUnlocked = false
		} : Player._mainPlayer._pCasting._playerSkillStructs[_skillStructIndex]);
		if (Player._mainPlayer._pCasting._skillLibrary[skillStruct._skillLibraryIndex] == null)
		{
			ToolTipManager._current._skillToolTip.Disable_ToolTip();
			return;
		}
		ToolTipManager._current.Apply_SkillDataToolTip(_scriptSkill, skillStruct);
		if (InputControlManager.current._setGamepadType != GamepadType.Keyboard)
		{
			ToolTipManager._current._skillToolTip._anchorReference = _skillIconTransform.transform;
		}
		ToolTipManager._current._skillToolTip.Set_TooltipAnchorPos(_skillToolTipAnchor);
		ToolTipManager._current._skillToolTip.Enable_ToolTip();
	}

	public void Disable_SkillToolTip()
	{
		ToolTipManager._current._skillToolTip.Disable_ToolTip();
	}

	public void OnClick_SkillIcon()
	{
		if (Input.GetMouseButton(0) || GamepadInput.GetButton(GamepadButton.RightTrigger))
		{
			if (!_skillStruct._skillUnlocked)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Unlearned Skill");
				return;
			}
			if (_emptySkillStruct)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Soulbound Skill");
				return;
			}
			if (_scriptSkill._skillControlType == SkillControlType.Passive)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Passive Skill");
				return;
			}
			if (Player._mainPlayer._playerZoneType != ZoneType.Safe)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot Attune Here");
				return;
			}
			TabMenu._current._skillsCell.Set_DragIconInfo(_scriptSkill);
			TabMenu._current._skillsCell._dragSkillStruct = _skillStruct;
		}
		if (Input.GetMouseButton(1) && (_isBonusSkillStruct || _skillStruct._skillUnlocked))
		{
			Disable_SkillToolTip();
		}
	}
}
