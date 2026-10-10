using UnityEngine;

[CreateAssetMenu(fileName = "STATUSCONSUMABLE_", menuName = "ScriptableObjects/Item/Consumables/Status Consumable", order = 0)]
public class ScriptableStatusConsumable : ScriptableConsumable
{
	[Header("Status Consumable")]
	public int _healthApply;

	public int _manaApply;

	public int _staminaApply;

	public int _expGain;

	[Space]
	public bool _resetSkillPoints;

	public bool _resetAttributePoints;

	[Space]
	public ConditionSlot _consumableCondition;

	public override bool CanUseConsumable(StatusEntity _statusEntity, bool _isButtonPrompt)
	{
		Player isPlayer = _statusEntity._isPlayer;
		PlayerStats pStats = isPlayer._pStats;
		PlayerCasting pCasting = isPlayer._pCasting;
		if (_statusEntity._currentHealth >= pStats._statStruct._maxHealth && _healthApply > 0 && _manaApply <= 0 && _staminaApply <= 0)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("At Max health");
			}
			return false;
		}
		if (_statusEntity._currentMana >= pStats._statStruct._maxMana && _manaApply > 0 && _healthApply <= 0 && _staminaApply <= 0)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("At Max mana");
			}
			return false;
		}
		if (_statusEntity._currentStamina >= pStats._statStruct._maxStamina && _staminaApply > 0 && _healthApply <= 0 && _manaApply <= 0)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("At Max stamina");
			}
			return false;
		}
		if (_statusEntity._currentHealth >= pStats._statStruct._maxHealth && _statusEntity._currentMana >= pStats._statStruct._maxMana && _healthApply > 0 && _manaApply > 0)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("At max health and mana");
			}
			return false;
		}
		if (pStats._currentLevel >= GameManager._current._statLogics._maxMainLevel && _expGain > 0)
		{
			if (_isButtonPrompt)
			{
				ErrorPromptTextManager.current.Init_ErrorPrompt("At max level");
			}
			return false;
		}
		if (_consumableCondition != null && (bool)_consumableCondition._scriptableCondition)
		{
			for (int i = 0; i < _statusEntity._conditionDatas.Count; i++)
			{
				if (_consumableCondition._scriptableCondition == _statusEntity._conditionDatas[i]._scriptableCondition && _statusEntity._conditionDatas[i]._setConditionPower > _consumableCondition._conditionPower)
				{
					if (_isButtonPrompt)
					{
						ErrorPromptTextManager.current.Init_ErrorPrompt("Has stronger condition");
					}
					return false;
				}
			}
		}
		if (_resetSkillPoints)
		{
			int num = 0;
			for (int j = 0; j < pCasting._skillLibrary.Count; j++)
			{
				num += pCasting._skillLibrary[j]._scriptSkill._skillRankParams._skillPointCost;
			}
			if (num <= 0)
			{
				if (_isButtonPrompt)
				{
					ErrorPromptTextManager.current.Init_ErrorPrompt("No Skill Points Spent");
				}
				return false;
			}
		}
		if (_resetAttributePoints)
		{
			int num2 = 0;
			for (int k = 0; k < pStats._syncAttributes.Count; k++)
			{
				num2 += pStats._syncAttributes[k]._attributeValue;
			}
			if (num2 <= 4)
			{
				if (_isButtonPrompt)
				{
					ErrorPromptTextManager.current.Init_ErrorPrompt("No Attribute Points Spent");
				}
				return false;
			}
		}
		return true;
	}

	public override void Use_Consumable(StatusEntity _statusEntity)
	{
		Apply_ConsumableCalculation(_statusEntity);
	}

	private void Apply_ConsumableCalculation(StatusEntity _statusEntity)
	{
		if ((bool)_statusEntity._isPlayer)
		{
			Apply_ServerParams();
		}
		void Apply_ServerParams()
		{
			if (_statusEntity.isServer)
			{
				PlayerStats pStats = _statusEntity._isPlayer._pStats;
				if (_healthApply != 0)
				{
					if (_healthApply > 0)
					{
						_statusEntity.Add_Health(_healthApply);
					}
					else if (_healthApply < 0)
					{
						_statusEntity.Subtract_Health(_healthApply);
					}
				}
				if (_manaApply != 0)
				{
					if (_manaApply > 0)
					{
						_statusEntity.Add_Mana(_manaApply);
					}
					else if (_manaApply < 0)
					{
						_statusEntity.Subtract_Mana(_manaApply);
					}
				}
				if (_staminaApply != 0)
				{
					if (_staminaApply > 0)
					{
						_statusEntity.Add_Stamina(_staminaApply);
					}
					else if (_staminaApply < 0)
					{
						_statusEntity.Subtract_Stamina(_staminaApply);
					}
				}
				if (_expGain > 0)
				{
					pStats.GainExp(_expGain, pStats._currentLevel);
				}
				if (_consumableCondition != null && (bool)_consumableCondition._scriptableCondition)
				{
					int conditionPower = _consumableCondition._conditionPower;
					int conditionDuration = _consumableCondition._conditionDuration;
					float powerPercent = _consumableCondition._powerPercent;
					float conditionRepeatRate = _consumableCondition._conditionRepeatRate;
					_consumableCondition._scriptableCondition.Apply_ConditionToEntity(_statusEntity, _statusEntity, DamageType.Pure, conditionPower, conditionDuration, powerPercent, conditionRepeatRate);
				}
				if (_resetAttributePoints)
				{
					pStats.Server_ResetAttributePoints();
				}
				if (_resetSkillPoints)
				{
					pStats.Server_ResetSkillPoints();
				}
			}
		}
	}
}
