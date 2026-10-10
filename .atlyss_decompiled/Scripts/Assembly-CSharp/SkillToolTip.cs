using UnityEngine;
using UnityEngine.UI;

public class SkillToolTip : TooltipElement
{
	[Header("Skill ToolTip")]
	public ScriptableSkill _scriptSkill;

	[Space]
	[SerializeField]
	private GameObject _skillScaleTypeTabObject;

	[SerializeField]
	private GameObject _skillCostTabObject;

	[SerializeField]
	private GameObject _manaCostSlot;

	[SerializeField]
	private GameObject _healthCostSlot;

	[SerializeField]
	private GameObject _staminaCostSlot;

	[SerializeField]
	private GameObject _itemCostSlot;

	[SerializeField]
	private GameObject _castTimeTabObject;

	[SerializeField]
	private GameObject _cooldownTabObject;

	[Space]
	[SerializeField]
	private Image _scaleTypeIcon;

	[SerializeField]
	private Image _itemIcon;

	[Space]
	[SerializeField]
	private Text _manaCost;

	[SerializeField]
	private Text _healthCost;

	[SerializeField]
	private Text _staminaCost;

	[SerializeField]
	private Text _itemCost;

	[Space]
	[SerializeField]
	private Text _scaleTypeText;

	[SerializeField]
	private Text _castTime;

	[SerializeField]
	private Text _coolDown;

	[SerializeField]
	private Text _skillConditionsListing;

	[Space]
	public SkillStruct _skillStruct;

	public override void Disable_ToolTip()
	{
		base.Disable_ToolTip();
		_scriptSkill = null;
	}

	public override void Update()
	{
		base.Update();
		if ((bool)TabMenu._current)
		{
			if (!TabMenu._current._isOpen)
			{
				Disable_ToolTip();
			}
			else if (TabMenu._current._skillsCell._isSkillDragging)
			{
				Disable_ToolTip();
			}
			else
			{
				Apply_SkillStats();
			}
		}
	}

	public void Apply_SkillStats()
	{
		if (!Player._mainPlayer)
		{
			Disable_ToolTip();
			return;
		}
		if (!_scriptSkill)
		{
			Disable_ToolTip();
			return;
		}
		_toolTipDescription.text = "";
		_toolTipDescription.gameObject.SetActive(value: false);
		_toolTipIcon.sprite = _scriptSkill._skillIcon;
		_toolTipName.text = _scriptSkill._skillName;
		if (_scriptSkill._skillControlType == SkillControlType.Passive)
		{
			_toolTipSubName.text = "Passive Skill";
		}
		else
		{
			switch (_scriptSkill._skillUtilityType)
			{
			case CombatColliderType.OFFENSIVE:
				_toolTipSubName.text = "Action Skill";
				break;
			case CombatColliderType.SUPPORT:
				_toolTipSubName.text = "Support Skill";
				break;
			}
		}
		_skillScaleTypeTabObject.SetActive(_scriptSkill._skillDamageType != DamageType.Pure && _scriptSkill._skillControlType != SkillControlType.Passive);
		_scaleTypeText.text = $"{_scriptSkill._skillDamageType} Skill";
		switch (_scriptSkill._skillDamageType)
		{
		case DamageType.Strength:
			_scaleTypeIcon.sprite = GameManager._current._statLogics._statAttributes[0]._attributeIcon;
			break;
		case DamageType.Dexterity:
			_scaleTypeIcon.sprite = GameManager._current._statLogics._statAttributes[2]._attributeIcon;
			break;
		case DamageType.Mind:
			_scaleTypeIcon.sprite = GameManager._current._statLogics._statAttributes[1]._attributeIcon;
			break;
		case DamageType.Vitality:
			_scaleTypeIcon.sprite = GameManager._current._statLogics._statAttributes[3]._attributeIcon;
			break;
		}
		if (_scriptSkill._skillControlType == SkillControlType.Passive)
		{
			_skillCostTabObject.SetActive(value: false);
			_cooldownTabObject.SetActive(value: false);
			_castTimeTabObject.SetActive(value: false);
		}
		else
		{
			_skillCostTabObject.SetActive(SkillhasCost());
			if (SkillhasCost())
			{
				_manaCostSlot.SetActive(_scriptSkill._skillRankParams._manaCost > 0);
				_healthCostSlot.SetActive(_scriptSkill._skillRankParams._healthCost > 0);
				_staminaCostSlot.SetActive(_scriptSkill._skillRankParams._staminaCost > 0);
				_itemCostSlot.SetActive((bool)_scriptSkill._skillRankParams._baseRequiredItem && _scriptSkill._skillRankParams._basedItemCost > 0);
				_manaCost.text = $"{_scriptSkill._skillRankParams._manaCost} Mana";
				_healthCost.text = $"{_scriptSkill._skillRankParams._healthCost} Health";
				_staminaCost.text = $"{_scriptSkill._skillRankParams._staminaCost} Stamina";
				if ((bool)_scriptSkill._skillRankParams._baseRequiredItem)
				{
					_itemCost.text = $"{_scriptSkill._skillRankParams._basedItemCost} {_scriptSkill._skillRankParams._baseRequiredItem._itemName}";
					_itemIcon.sprite = _scriptSkill._skillRankParams._baseRequiredItem._itemIcon;
				}
			}
			_castTimeTabObject.SetActive(value: true);
			_cooldownTabObject.SetActive(value: true);
			if (_scriptSkill._skillRankParams._baseCastTime <= 0.12f)
			{
				_castTime.text = "Instant Cast";
			}
			else
			{
				_castTime.text = $"{_scriptSkill._skillRankParams._baseCastTime} sec Cast";
			}
			_coolDown.text = $"{_scriptSkill._skillRankParams._baseCooldown} sec Cooldown";
		}
		_skillConditionsListing.text = string.Empty;
		Apply_SkillDescriptorInfo();
		bool SkillhasCost()
		{
			if (_scriptSkill._skillRankParams._manaCost > 0)
			{
				return true;
			}
			if (_scriptSkill._skillRankParams._healthCost > 0)
			{
				return true;
			}
			if (_scriptSkill._skillRankParams._staminaCost > 0)
			{
				return true;
			}
			if (_scriptSkill._skillRankParams._basedItemCost > 0)
			{
				return true;
			}
			return false;
		}
	}

	private void Apply_SkillDescriptorInfo()
	{
		PlayerStats pStats = Player._mainPlayer._pStats;
		int _skillPower = 0;
		float powerStatPercent = GameManager._current._statLogics._powerStatPercent;
		float num = pStats.Retrieve_SkillBonusPower(_scriptSkill);
		float num2 = powerStatPercent + num;
		switch (_scriptSkill._skillDamageType)
		{
		case DamageType.Strength:
			_skillPower = _scriptSkill._skillRankParams._baseSkillPower + (int)((float)pStats._statStruct._attackPower * num2);
			break;
		case DamageType.Dexterity:
			_skillPower = _scriptSkill._skillRankParams._baseSkillPower + (int)((float)pStats._statStruct._dexPower * num2);
			break;
		case DamageType.Mind:
			_skillPower = _scriptSkill._skillRankParams._baseSkillPower + (int)((float)pStats._statStruct._magicPower * num2);
			break;
		case DamageType.Vitality:
			_skillPower = _scriptSkill._skillRankParams._baseSkillPower + (int)((float)pStats._statStruct._maxHealth * num2);
			break;
		}
		if (_scriptSkill._skillRankParams._useWeaponPower && (bool)pStats._player._pCombat._equippedWeapon)
		{
			_skillPower += pStats._player._pCombat._equippedWeapon.Get_BaseCeilingDamage();
		}
		if (_skillConditionsListing.text != string.Empty)
		{
			_skillConditionsListing.text += "\n";
		}
		Init_TermMacros();
		Apply_ConditionRankInfo();
		void Init_TermMacros()
		{
			if (!(_scriptSkill._skillDescription == string.Empty))
			{
				_toolTipDescription.gameObject.SetActive(value: true);
				string skillDescription = _scriptSkill._skillDescription;
				ScriptableCondition scriptableCondition = null;
				ConditionSlot skillObjectCondition = _scriptSkill._skillRankParams._skillObjectCondition;
				StatStruct baseStatStruct = Player._mainPlayer._pStats._baseStatStruct;
				DamageType skillDamageType = _scriptSkill._skillDamageType;
				int conditionPower = _scriptSkill._skillRankParams._skillObjectCondition._conditionPower;
				int conditionDuration = _scriptSkill._skillRankParams._skillObjectCondition._conditionDuration;
				float powerPercent = _scriptSkill._skillRankParams._skillObjectCondition._powerPercent;
				float conditionRepeatRate = _scriptSkill._skillRankParams._skillObjectCondition._conditionRepeatRate;
				if ((bool)skillObjectCondition._scriptableCondition)
				{
					scriptableCondition = skillObjectCondition._scriptableCondition;
				}
				skillDescription = skillDescription.Replace("$SKP", $"<color=yellow>{_skillPower}</color>");
				skillDescription = skillDescription.Replace("$DMG", $"<color=yellow>({_skillPower} - {_skillPower + 3})</color>");
				skillDescription = skillDescription.Replace("$COOLDWN", $"<color=yellow>{_scriptSkill._skillRankParams._baseCooldown} sec cooldown.</color>");
				skillDescription = skillDescription.Replace("$MANACOST", $"<color=yellow>Costs {_scriptSkill._skillRankParams._manaCost} Mana.</color>");
				skillDescription = skillDescription.Replace("$HEALTHCOST", $"<color=yellow>Costs {_scriptSkill._skillRankParams._healthCost} Health.</color>");
				skillDescription = skillDescription.Replace("$STAMINACOST", $"<color=yellow>Costs {_scriptSkill._skillRankParams._staminaCost} Stamina.</color>");
				skillDescription = ((!(_scriptSkill._skillRankParams._baseCastTime > 0.12f)) ? skillDescription.Replace("$CASTTIME", "<color=yellow>instant cast time.</color>") : skillDescription.Replace("$CASTTIME", $"<color=yellow>{_scriptSkill._skillRankParams._baseCastTime} sec cast time.</color>"));
				if (InputControlManager.current._setGamepadType != GamepadType.Keyboard)
				{
					skillDescription = skillDescription.Replace("$RECALLINPUT", "<color=yellow>[Dpad Left]</color>");
					skillDescription = skillDescription.Replace("$CHARGEATKINPUT", "<color=yellow>hold [Left Shoulder Button + Right Trigger]</color>");
					skillDescription = skillDescription.Replace("$DASHINPUT", "<color=yellow>[B]</color>");
				}
				else
				{
					skillDescription = skillDescription.Replace("$RECALLINPUT", "<color=yellow>[" + InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._recall.ToString()) + "]</color>");
					skillDescription = skillDescription.Replace("$CHARGEATKINPUT", "<color=yellow>hold [" + InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._chargeAttack.ToString()) + "]</color>");
					skillDescription = skillDescription.Replace("$DASHINPUT", "<color=yellow>[" + InputControlManager.current.Convert_KeyCodeName(InputControlManager.current._dash.ToString()) + "]</color>");
				}
				if (_scriptSkill._requireShield)
				{
					skillDescription += " <color=yellow>Requires a shield.</color>";
				}
				switch (_scriptSkill._toolTipRequirement)
				{
				case SkillToolTipRequirement.MELEE:
					skillDescription += " <color=yellow>Requires a melee weapon.</color>";
					break;
				case SkillToolTipRequirement.HEAVY_MELEE:
					skillDescription += " <color=yellow>Requires a heavy melee weapon.</color>";
					break;
				case SkillToolTipRequirement.RANGED:
					skillDescription += " <color=yellow>Requires a ranged weapon.</color>";
					break;
				case SkillToolTipRequirement.HEAVY_RANGED:
					skillDescription += " <color=yellow>Requires a heavy ranged weapon.</color>";
					break;
				case SkillToolTipRequirement.MAGIC:
					skillDescription += " <color=yellow>Requires a magic weapon.</color>";
					break;
				case SkillToolTipRequirement.HEAVY_MAGIC:
					skillDescription += " <color=yellow>Requires a heavy magic weapon.</color>";
					break;
				}
				if ((bool)scriptableCondition && !string.IsNullOrWhiteSpace(scriptableCondition._conditionDescription))
				{
					string text = scriptableCondition.Generate_ConditionDescriptor(baseStatStruct, skillDamageType, conditionPower, conditionDuration, powerPercent, conditionRepeatRate);
					string text2 = ((!(skillObjectCondition._chance < 1f)) ? ("\n\n<color=cyan>" + scriptableCondition._conditionName + " - (" + scriptableCondition._conditionGroup._conditionGroupTag + ")</color>\n" + text) : ($"\n\n<color=cyan>{scriptableCondition._conditionName} - ({scriptableCondition._conditionGroup._conditionGroupTag}) ({skillObjectCondition._chance * 100f}% Chance)</color>" + "\n" + text));
					skillDescription += text2;
				}
				_toolTipDescription.text = skillDescription;
			}
		}
	}

	private void Apply_ConditionRankInfo()
	{
		if (_scriptSkill._skillRankParams._selfConditionOutput == null)
		{
			return;
		}
		ScriptableCondition scriptableCondition = _scriptSkill._skillRankParams._selfConditionOutput._scriptableCondition;
		if ((bool)scriptableCondition && (bool)scriptableCondition._conditionGroup)
		{
			_skillConditionsListing.text += "\n";
			StatStruct baseStatStruct = Player._mainPlayer._pStats._baseStatStruct;
			DamageType skillDamageType = _scriptSkill._skillDamageType;
			int conditionPower = _scriptSkill._skillRankParams._selfConditionOutput._conditionPower;
			int conditionDuration = _scriptSkill._skillRankParams._selfConditionOutput._conditionDuration + Player._mainPlayer._pStats.Retrieve_SkillConditionBonusDuration(_scriptSkill);
			float setPercent = _scriptSkill._skillRankParams._selfConditionOutput._powerPercent + Player._mainPlayer._pStats.Retrieve_SkillConditionBonusPower(_scriptSkill);
			float conditionRepeatRate = _scriptSkill._skillRankParams._selfConditionOutput._conditionRepeatRate;
			string text = "<color=cyan>" + scriptableCondition._conditionName + " - (" + scriptableCondition._conditionGroup._conditionGroupTag + ")</color>\n";
			text += scriptableCondition.Generate_ConditionDescriptor(baseStatStruct, skillDamageType, conditionPower, conditionDuration, setPercent, conditionRepeatRate);
			_skillConditionsListing.text += text;
			if (scriptableCondition._cancelOnHit)
			{
				_skillConditionsListing.text += " <color=yellow>Cancels on hit.</color>";
			}
		}
	}
}
