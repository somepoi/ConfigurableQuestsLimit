using UnityEngine;

public abstract class ScriptableEquipment : ScriptableItem
{
	[Header("Scriptable Equipment")]
	public int _equipmentLevel = 1;

	public ScriptablePlayerBaseClass _classRequirement;

	[HideInInspector]
	public ItemEquipType _equipType;

	[Header("Equip Stats")]
	public StatStruct _statArray;

	public EquipCondition_Activation _equipConditionActivation;

	public SkillBonus[] _equipSkillBonuses;

	public WeaponTypeBonus[] _equipWeaponTypeBonuses;

	[Space]
	public ScriptableStatModifierTable _statModifierTable;

	public StatModifierCost _statModifierCost;

	public virtual void OnValidate()
	{
		if (_equipmentLevel <= 0)
		{
			_equipmentLevel = 1;
		}
		_itemType = ItemType.GEAR;
		if ((bool)_equipConditionActivation._dedicatedSkillCastCallback)
		{
			_equipConditionActivation._equipConditionCallback = EquipConditionCallback.None;
		}
		if (_equipSkillBonuses != null && _equipSkillBonuses.Length != 0)
		{
			SkillBonus[] equipSkillBonuses = _equipSkillBonuses;
			foreach (SkillBonus skillBonus in equipSkillBonuses)
			{
				if (skillBonus._skillConditionDurationBonus < 0)
				{
					skillBonus._skillConditionDurationBonus = 0;
				}
				if (skillBonus._skillConditionPowerBonus < 0f)
				{
					skillBonus._skillConditionPowerBonus = 0f;
				}
				if (!skillBonus._scriptableSkill)
				{
					skillBonus._scriptSkillTag = "Empty Slot";
				}
				else if (!string.IsNullOrWhiteSpace(skillBonus._scriptableSkill._skillName))
				{
					skillBonus._scriptSkillTag = "Equip Skill Bonus: " + skillBonus._scriptableSkill._skillName;
					skillBonus._skillBonusDescriptor = $"Increases skill power on <color=yellow>{skillBonus._scriptableSkill._skillName}</color> by <color=yellow>{skillBonus._skillPowerBonus * 100f}%.</color>";
				}
			}
		}
		if (_equipWeaponTypeBonuses == null || _equipWeaponTypeBonuses.Length == 0)
		{
			return;
		}
		WeaponTypeBonus[] equipWeaponTypeBonuses = _equipWeaponTypeBonuses;
		foreach (WeaponTypeBonus weaponTypeBonus in equipWeaponTypeBonuses)
		{
			if (!weaponTypeBonus._scriptableWeaponType)
			{
				weaponTypeBonus._weaponTypeTag = "Empty Slot";
			}
			else if (!string.IsNullOrWhiteSpace(weaponTypeBonus._scriptableWeaponType._weaponTypeClassTag))
			{
				bool num = !weaponTypeBonus._scriptableWeaponType._weaponTypeClassTag.EndsWith('s');
				weaponTypeBonus._weaponTypeTag = "Equip Weapon Type Bonus: " + weaponTypeBonus._scriptableWeaponType._weaponTypeClassTag;
				if (num)
				{
					weaponTypeBonus._weaponTypeBonusDescriptor = $"Increases damage bonus with <color=yellow>{weaponTypeBonus._scriptableWeaponType._weaponTypeClassTag}s</color> by <color=yellow>{weaponTypeBonus._weaponTypeBonus * 100f}%.</color>";
				}
				else
				{
					weaponTypeBonus._weaponTypeBonusDescriptor = $"Increases damage bonus with <color=yellow>{weaponTypeBonus._scriptableWeaponType._weaponTypeClassTag}</color> by <color=yellow>{weaponTypeBonus._weaponTypeBonus * 100f}%.</color>";
				}
			}
		}
	}

	public bool CanEquipItem(PlayerStats _pStats, bool _isClickPrompt)
	{
		if (!_pStats)
		{
			return false;
		}
		if (_equipmentLevel > _pStats._currentLevel)
		{
			if (_isClickPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot Equip - Low Level");
			}
			return false;
		}
		if ((!_pStats._class && (bool)_classRequirement) || ((bool)_pStats._class && (bool)_classRequirement && _classRequirement != _pStats._class))
		{
			if (_isClickPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("Cannot Equip - Invalid Class");
			}
			return false;
		}
		return true;
	}

	public bool IsConditionMet(StatusEntity _statusEntity, EquipConditionCallback _callback, ScriptableSkill _dedicatedSkill)
	{
		EquipCondition_Activation equipConditionActivation = _equipConditionActivation;
		if (!_statusEntity)
		{
			return false;
		}
		if (!equipConditionActivation._canRefreshCondition)
		{
			foreach (ConditionData conditionData in _statusEntity._conditionDatas)
			{
				if (conditionData._scriptableCondition == equipConditionActivation._equipConditionSlot._scriptableCondition)
				{
					return false;
				}
			}
		}
		if ((float)_statusEntity._currentHealth > (float)_statusEntity._isPlayer._pStats._statStruct._maxHealth * equipConditionActivation._healthPercentageThreshold)
		{
			return false;
		}
		if ((float)_statusEntity._currentMana > (float)_statusEntity._isPlayer._pStats._statStruct._maxMana * equipConditionActivation._manaPercentageThreshold)
		{
			return false;
		}
		if ((float)_statusEntity._currentStamina > (float)_statusEntity._isPlayer._pStats._statStruct._maxStamina * equipConditionActivation._staminaPercentageThreshold)
		{
			return false;
		}
		if ((bool)equipConditionActivation._dedicatedSkillCastCallback)
		{
			if (_dedicatedSkill != equipConditionActivation._dedicatedSkillCastCallback)
			{
				return false;
			}
			if (equipConditionActivation._requireSkillHit && _callback != EquipConditionCallback.Attack_Hit)
			{
				return false;
			}
		}
		else if (equipConditionActivation._equipConditionCallback != _callback)
		{
			return false;
		}
		return true;
	}
}
