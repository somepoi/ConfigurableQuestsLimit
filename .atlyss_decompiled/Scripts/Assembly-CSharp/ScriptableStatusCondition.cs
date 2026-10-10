using UnityEngine;

[CreateAssetMenu(fileName = "statusCondition_", menuName = "ScriptableObjects/Buffs and Conditions/Status Condition", order = 0)]
public class ScriptableStatusCondition : ScriptableCondition
{
	[Header("Status Condition")]
	public StatStruct _statStruct;

	[Space]
	public int _applyHealth;

	public int _applyMana;

	public int _applyStamina;

	[Space]
	public bool _replenishHealth;

	public bool _replenishMana;

	public bool _replenishStamina;

	[Header("Special Conditions")]
	public int _damageAbsorbtionAmount;

	public float _accuracyReduction;

	[Range(0f, 1f)]
	public float _chanceToDropCurrency;

	[Header("Status Flags")]
	public bool _knockbackImmunity;

	public bool _hideEntity;

	public bool _autoParry;

	public bool _fatalCondition;

	public bool _siphonHealth;

	[Header("Casting Conditions")]
	public float _castTimerMod;

	public float _cooldownMod;

	public bool _ignoreSkillCost;

	[Header("Control Lock Buffers")]
	public float _moveControlLockBuffer;

	public float _combatControlLockBuffer;

	public float _castControlLockBuffer;

	[Header("Movement")]
	[Range(-1f, 1f)]
	public float _movSpeedPercentChange;

	public float _changeGravityMultiplier;

	public bool _isFearMovement;

	public override void Init_ConditionEffect(StatusEntity _targetEntity, ConditionData _conDat)
	{
		if (_targetEntity._currentHealth <= 0)
		{
			return;
		}
		Apply_StatusValues();
		if (!_conDat._isInitialized)
		{
			Apply_StatStruct(_targetEntity, _conDat._statStruct, _conDat._attribute, _conDat._setConditionPower, _conDat._setConditionPercent, _isRemoval: false);
			Apply_MovementCondition(_targetEntity, _isRemoval: false);
		}
		_conDat._isInitialized = true;
		if (_isFearMovement)
		{
			if ((bool)_targetEntity._isPlayer && _targetEntity.isLocalPlayer)
			{
				_targetEntity._isPlayer._pMove.Toggle_FearMovement(_isTrue: true);
			}
			if ((bool)_targetEntity._isCreep && _targetEntity.isServer)
			{
				_targetEntity._isCreep._pathFindCtrl.Toggle_FearMovement(_isTrue: true);
			}
		}
		void Apply_HealthValue()
		{
			if (_fatalCondition)
			{
				int num = 0;
				if ((bool)_targetEntity._isPlayer)
				{
					num = _targetEntity._isPlayer._pStats._statStruct._maxHealth;
				}
				else if ((bool)_targetEntity._isCreep)
				{
					num = _targetEntity._isCreep._statStruct._maxHealth;
				}
				_targetEntity.Subtract_Health(num + 30);
			}
			else if (_applyHealth != 0)
			{
				int applyHealth = _applyHealth;
				int num2 = Get_ConditionPower(_conDat._statStruct, _conDat._attribute, _conDat._setConditionPower, _conDat._setConditionPercent);
				applyHealth *= num2;
				if (applyHealth > 0)
				{
					_targetEntity.Add_Health(applyHealth);
				}
				else if (applyHealth < 0)
				{
					_targetEntity.Subtract_Health(applyHealth *= -1);
					_targetEntity.ScriptCondition_Callback(this, applyHealth);
					if ((bool)_targetEntity._isPlayer && _targetEntity.isServer)
					{
						if ((bool)_conditionElement)
						{
							_targetEntity._statusEntityGUI.Target_Display_FloatingNumber(_targetEntity, FloatTextColor.TAKE_DAMAGE, applyHealth, _conditionElement._elementID, 0);
						}
						else
						{
							_targetEntity._statusEntityGUI.Target_Display_FloatingNumber(_targetEntity, FloatTextColor.TAKE_DAMAGE, applyHealth, 0, _conditionGroup._conditionGroupID);
						}
					}
				}
			}
		}
		void Apply_ManaValue()
		{
			if (_applyMana != 0)
			{
				int num = 0;
				int num2 = Get_ConditionPower(_conDat._statStruct, _conDat._attribute, _conDat._setConditionPower, _conDat._setConditionPercent);
				num *= num2;
				StatusEntity statusEntity = _targetEntity;
				statusEntity.Network_currentMana = statusEntity._currentMana + num;
			}
		}
		void Apply_SpecialConditions()
		{
			if ((bool)_targetEntity._isPlayer)
			{
				PlayerCasting pCasting = _targetEntity._isPlayer._pCasting;
				pCasting._castTimerMod += _castTimerMod;
				pCasting._cooldownMod += _cooldownMod;
			}
			else if ((bool)_targetEntity._isCreep && !_targetEntity._isCreep._isCreepMugged && _chanceToDropCurrency > 0f)
			{
				ItemDropEntity itemDropEntity = _targetEntity._isCreep._itemDropEntity;
				if (Random.Range(0f, 1.01f) <= _chanceToDropCurrency)
				{
					itemDropEntity.Client_DropCurrency(1, 0.12f);
					_targetEntity._isCreep._isCreepMugged = true;
				}
			}
			if (_damageAbsorbtionAmount > 0)
			{
				StatusEntity statusEntity = _targetEntity;
				statusEntity.Network_damageAbsorbtion = statusEntity._damageAbsorbtion + (float)Mathf.Abs(StatCalculate(_conDat._statStruct, _conDat._attribute, _damageAbsorbtionAmount, _conDat._setConditionPower, _conDat._setConditionPercent));
			}
			if (_accuracyReduction > 0f)
			{
				_targetEntity._hitAccuracyPercent -= _accuracyReduction;
			}
			if (_knockbackImmunity)
			{
				_targetEntity._immuneToKnockback = true;
			}
		}
		void Apply_StaminaValue()
		{
			if (_applyStamina != 0)
			{
				int num = 0;
				int num2 = Get_ConditionPower(_conDat._statStruct, _conDat._attribute, _conDat._setConditionPower, _conDat._setConditionPercent);
				num *= num2;
				StatusEntity statusEntity = _targetEntity;
				statusEntity.Network_currentStamina = statusEntity._currentStamina + num;
			}
		}
		void Apply_StatusValues()
		{
			Apply_SpecialConditions();
			Apply_HealthValue();
			Apply_ManaValue();
			Apply_StaminaValue();
			if (_replenishHealth)
			{
				_targetEntity.Replenish_Health();
			}
			if (_replenishMana)
			{
				_targetEntity.Replenish_Mana();
			}
			if (_replenishStamina)
			{
				_targetEntity.Replenish_Stamina();
			}
		}
	}

	public override void Update_ConditionEffect(StatusEntity _targetEntity, ConditionData _conDat)
	{
		if (_targetEntity._currentHealth > 0)
		{
			if (_autoParry)
			{
				_targetEntity._autoParry = _autoParry;
			}
			OnUpdate_DamageAbsorbtion();
			OnUpdate_PlayerCondition();
			OnUpdate_CreepCondition();
		}
		void OnUpdate_CreepCondition()
		{
			if ((bool)_targetEntity._isCreep && _targetEntity.isServer)
			{
				Creep isCreep = _targetEntity._isCreep;
				if (_movSpeedPercentChange <= -1f)
				{
					isCreep._knockbackBuffer = 0.5f;
					isCreep._pathFindCtrl.Network_dontLookAtTarget = true;
					isCreep.Network_ignoreAggroedTarget = true;
					isCreep._creepAnim.CrossFade("Empty", 0f, 1);
					isCreep._creepPushCollider.enabled = false;
					isCreep._parryBuffer = 0.8f;
				}
			}
		}
		void OnUpdate_DamageAbsorbtion()
		{
			if (_targetEntity.isServer && _damageAbsorbtionAmount > 0 && _targetEntity._damageAbsorbtion <= 0f)
			{
				for (int i = 0; i < _targetEntity._conditionDatas.Count; i++)
				{
					if (_targetEntity._conditionDatas[i]._scriptableCondition == this)
					{
						_targetEntity._conditionDatas[i]._currentDuration = 0f;
					}
				}
			}
		}
		void OnUpdate_PlayerCondition()
		{
			if ((bool)_targetEntity._isPlayer)
			{
				Player isPlayer = _targetEntity._isPlayer;
				PlayerMove pMove = isPlayer._pMove;
				PlayerCombat pCombat = isPlayer._pCombat;
				PlayerCasting pCasting = isPlayer._pCasting;
				PlayerVisual pVisual = isPlayer._pVisual;
				if (_ignoreSkillCost)
				{
					pCasting._ignoreCost = true;
				}
				if (_hideEntity)
				{
					pVisual.Network_forceHidden = true;
				}
				if (_movSpeedPercentChange <= -1f)
				{
					if (pCombat._isBlocking && pCombat.isServer)
					{
						pCombat.Network_isBlocking = false;
					}
					pMove._lockControlBuffer = 0.5f;
					pCombat._lockControlBuffer = 0.5f;
					pCasting._lockControlBuffer = 0.5f;
				}
				if (_moveControlLockBuffer > 0f)
				{
					pMove._lockControlBuffer = _moveControlLockBuffer;
				}
				if (_combatControlLockBuffer > 0f)
				{
					pCombat._lockControlBuffer = _combatControlLockBuffer;
				}
				if (_castControlLockBuffer > 0f)
				{
					pCasting._lockControlBuffer = _castControlLockBuffer;
				}
			}
		}
	}

	public override void End_ConditionEffect(StatusEntity _targetEntity, ConditionData _conDat)
	{
		if (_autoParry)
		{
			_targetEntity._autoParry = false;
		}
		if (_damageAbsorbtionAmount > 0)
		{
			_targetEntity.Network_damageAbsorbtion = _targetEntity._damageAbsorbtion - (float)Mathf.Abs(StatCalculate(_conDat._statStruct, _conDat._attribute, _damageAbsorbtionAmount, _conDat._setConditionPower, _conDat._setConditionPercent));
		}
		if (_accuracyReduction > 0f)
		{
			_targetEntity._hitAccuracyPercent += _accuracyReduction;
		}
		if (_knockbackImmunity)
		{
			_targetEntity._immuneToKnockback = false;
		}
		if ((bool)_targetEntity._isPlayer)
		{
			PlayerCasting pCasting = _targetEntity._isPlayer._pCasting;
			PlayerVisual pVisual = _targetEntity._isPlayer._pVisual;
			pCasting._castTimerMod -= _castTimerMod;
			pCasting._cooldownMod -= _cooldownMod;
			if (pVisual._forceHidden)
			{
				_targetEntity._isPlayer.Network_inIFrame = false;
				pVisual.Network_forceHidden = false;
			}
			pCasting._ignoreCost = false;
		}
		Apply_StatStruct(_targetEntity, _conDat._statStruct, _conDat._attribute, _conDat._setConditionPower, _conDat._setConditionPercent, _isRemoval: true);
		Apply_MovementCondition(_targetEntity, _isRemoval: true);
		if (_isFearMovement)
		{
			if ((bool)_targetEntity._isPlayer && _targetEntity.isLocalPlayer)
			{
				_targetEntity._isPlayer._pMove.Toggle_FearMovement(_isTrue: false);
			}
			if ((bool)_targetEntity._isCreep && _targetEntity.isServer)
			{
				_targetEntity._isCreep._pathFindCtrl.Toggle_FearMovement(_isTrue: false);
			}
		}
	}

	private void Apply_MovementCondition(StatusEntity _targetEntity, bool _isRemoval)
	{
		int num = 1;
		if (_isRemoval)
		{
			num = -1;
		}
		if ((bool)_targetEntity._isPlayer)
		{
			PlayerMove pMove = _targetEntity._isPlayer._pMove;
			float num2 = GameManager._current._statLogics._baseMoveSpeed * _movSpeedPercentChange;
			pMove.Add_MoveSpeed(num2 * (float)num);
		}
		else
		{
			if (!_targetEntity._isCreep)
			{
				return;
			}
			RaycastPathFinder pathFindCtrl = _targetEntity._isCreep._pathFindCtrl;
			float num3 = pathFindCtrl._baseMovSpeed * _movSpeedPercentChange;
			pathFindCtrl._currentMovSpeed += num3 * (float)num;
			if (_isRemoval)
			{
				if (_movSpeedPercentChange <= -1f)
				{
					_targetEntity._isCreep._pathFindCtrl.Network_dontLookAtTarget = false;
					_targetEntity._isCreep._pathFindCtrl._disablePathing = false;
					_targetEntity._isCreep._creepPushCollider.enabled = true;
					_targetEntity._isCreep.Network_ignoreAggroedTarget = false;
					_targetEntity._isCreep._creepBehavior.Reset_Behavior();
				}
				_targetEntity._isCreep._creepAnim.SetFloat("AtkSpeed", 1f);
			}
		}
	}

	private void Apply_StatStruct(StatusEntity _targetEntity, StatStruct _parentStatStruct, DamageType _attribute, int _conditionPower, float _powerPercent, bool _isRemoval)
	{
		StatStruct _newStatStruct;
		if (_targetEntity.isServer)
		{
			int num = 1;
			if (_isRemoval)
			{
				num = -1;
			}
			StatStruct statStruct = default;
			if ((bool)_targetEntity._isPlayer)
			{
				statStruct = _targetEntity._isPlayer._pStats._bonusStatStruct;
			}
			else if ((bool)_targetEntity._isCreep)
			{
				statStruct = _targetEntity._isCreep._statStruct;
			}
			_newStatStruct = new StatStruct
			{
				_maxHealth = statStruct._maxHealth + StatCalculate(_parentStatStruct, _attribute, _statStruct._maxHealth, _conditionPower, _powerPercent) * num,
				_maxMana = statStruct._maxMana + StatCalculate(_parentStatStruct, _attribute, _statStruct._maxMana, _conditionPower, _powerPercent) * num,
				_maxStamina = statStruct._maxStamina + StatCalculate(_parentStatStruct, _attribute, _statStruct._maxStamina, _conditionPower, _powerPercent) * num,
				_attackPower = statStruct._attackPower + StatCalculate(_parentStatStruct, _attribute, _statStruct._attackPower, _conditionPower, _powerPercent) * num,
				_magicPower = statStruct._magicPower + StatCalculate(_parentStatStruct, _attribute, _statStruct._magicPower, _conditionPower, _powerPercent) * num,
				_dexPower = statStruct._dexPower + StatCalculate(_parentStatStruct, _attribute, _statStruct._dexPower, _conditionPower, _powerPercent) * num,
				_defense = statStruct._defense + StatCalculate(_parentStatStruct, _attribute, _statStruct._defense, _conditionPower, _powerPercent) * num,
				_magicDefense = statStruct._magicDefense + StatCalculate(_parentStatStruct, _attribute, _statStruct._magicDefense, _conditionPower, _powerPercent) * num,
				_criticalRate = statStruct._criticalRate + FloatStatCalculate(_parentStatStruct, _attribute, _statStruct._criticalRate, _conditionPower, _powerPercent) * (float)num,
				_magicCriticalRate = statStruct._magicCriticalRate + FloatStatCalculate(_parentStatStruct, _attribute, _statStruct._magicCriticalRate, _conditionPower, _powerPercent) * (float)num,
				_evasion = statStruct._evasion + FloatStatCalculate(_parentStatStruct, _attribute, _statStruct._evasion, _conditionPower, _powerPercent) * (float)num,
				_fireResist = statStruct._fireResist + StatCalculate(_parentStatStruct, _attribute, _statStruct._fireResist, _conditionPower, _powerPercent) * num,
				_waterResist = statStruct._waterResist + StatCalculate(_parentStatStruct, _attribute, _statStruct._waterResist, _conditionPower, _powerPercent) * num,
				_natureResist = statStruct._natureResist + StatCalculate(_parentStatStruct, _attribute, _statStruct._natureResist, _conditionPower, _powerPercent) * num,
				_earthResist = statStruct._earthResist + StatCalculate(_parentStatStruct, _attribute, _statStruct._earthResist, _conditionPower, _powerPercent) * num,
				_holyResist = statStruct._holyResist + StatCalculate(_parentStatStruct, _attribute, _statStruct._holyResist, _conditionPower, _powerPercent) * num,
				_shadowResist = statStruct._shadowResist + StatCalculate(_parentStatStruct, _attribute, _statStruct._shadowResist, _conditionPower, _powerPercent) * num
			};
			float criticalRate = GameManager._current.ConvertFloat_ToN2(_newStatStruct._criticalRate);
			float magicCriticalRate = GameManager._current.ConvertFloat_ToN2(_newStatStruct._magicCriticalRate);
			float evasion = GameManager._current.ConvertFloat_ToN2(_newStatStruct._evasion);
			_newStatStruct._criticalRate = criticalRate;
			_newStatStruct._magicCriticalRate = magicCriticalRate;
			_newStatStruct._evasion = evasion;
			Apply_PlayerStatStruct();
			Apply_CreepStatStruct();
		}
		void Apply_CreepStatStruct()
		{
			if ((bool)_targetEntity._isCreep)
			{
				_targetEntity._isCreep.Network_statStruct = _newStatStruct;
			}
		}
		void Apply_PlayerStatStruct()
		{
			if ((bool)_targetEntity._isPlayer)
			{
				PlayerStats pStats = _targetEntity._isPlayer._pStats;
				if (_statStruct._experience > 0)
				{
					pStats.Network_currentExp = pStats._currentExp + _statStruct._experience;
				}
				pStats.Network_bonusStatStruct = _newStatStruct;
				pStats.Apply_StatCalculations();
			}
		}
	}

	private int StatCalculate(StatStruct _parentStatStruct, DamageType _attribute, int _value, int _conditionPower, float _conditionPercent)
	{
		int num = 1;
		if (_value < 0)
		{
			num = -1;
		}
		else if (_value == 0)
		{
			return 0;
		}
		int num2 = _value;
		switch (_attribute)
		{
		case DamageType.Pure:
			_value = (int)((float)(num2 + _conditionPower) * _conditionPercent);
			break;
		case DamageType.Strength:
			_value += (int)((float)(_parentStatStruct._attackPower + _conditionPower) * _conditionPercent) * num;
			break;
		case DamageType.Mind:
			_value += (int)((float)(_parentStatStruct._magicPower + _conditionPower) * _conditionPercent) * num;
			break;
		case DamageType.Dexterity:
			_value += (int)((float)(_parentStatStruct._dexPower + _conditionPower) * _conditionPercent) * num;
			break;
		case DamageType.Vitality:
			_value += (int)((float)(_parentStatStruct._maxHealth + _conditionPower) * _conditionPercent) * num;
			break;
		}
		return _value;
	}

	private float FloatStatCalculate(StatStruct _parentStatStruct, DamageType _attribute, float _value, int _bonusPower, float _powerPercent)
	{
		int num = 1;
		if (_value < 0f)
		{
			num = -1;
		}
		else if (_value == 0f)
		{
			return 0f;
		}
		float num2 = _value;
		switch (_attribute)
		{
		case DamageType.Pure:
			_value = (num2 + (float)_bonusPower) * _powerPercent;
			break;
		case DamageType.Strength:
			_value += (float)(_parentStatStruct._attackPower + _bonusPower) * 0.0042f * _powerPercent * (float)num;
			break;
		case DamageType.Mind:
			_value += (float)(_parentStatStruct._magicPower + _bonusPower) * 0.0042f * _powerPercent * (float)num;
			break;
		case DamageType.Dexterity:
			_value += (float)(_parentStatStruct._dexPower + _bonusPower) * 0.0042f * _powerPercent * (float)num;
			break;
		case DamageType.Vitality:
			_value += (float)(_parentStatStruct._maxHealth + _bonusPower) * 0.0042f * _powerPercent * (float)num;
			break;
		}
		return _value;
	}

	public override string Generate_ConditionDescriptor(StatStruct _parentStatStruct, DamageType _attribute, int _setPower, int _setDuration, float _setPercent, float _setRepeatRate)
	{
		string conditionDescription = _conditionDescription;
		int num = Get_ConditionPower(_parentStatStruct, _attribute, _setPower, _setPercent);
		conditionDescription = conditionDescription.Replace("$BASEPOWER", $"<color=yellow>{num}</color>");
		conditionDescription = conditionDescription.Replace("$APPLY_HEALTH", $"<color=yellow>{Mathf.Abs(num * _applyHealth)}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_MAXHEALTH", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._maxHealth, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_MAXMANA", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._maxMana, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_MAXSTAMINA", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._maxStamina, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_EXP", $"<color=yellow>{Mathf.Abs(_statStruct._experience)}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_ATTACKPOWER", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._attackPower, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_MAGICPOWER", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._magicPower, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_DEXPOWER", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._dexPower, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_DEFENSE", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._defense, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_MAGICDEFENSE", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._magicDefense, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_CRITRATE", "<color=yellow>" + Mathf.Abs(FloatStatCalculate(_parentStatStruct, _attribute, _statStruct._criticalRate, _setPower, _setPercent) * 100f).ToString("N2") + "%</color>");
		conditionDescription = conditionDescription.Replace("$STAT_MAGICCRITRATE", "<color=yellow>" + Mathf.Abs(FloatStatCalculate(_parentStatStruct, _attribute, _statStruct._magicCriticalRate, _setPower, _setPercent) * 100f).ToString("N2") + "%</color>");
		conditionDescription = conditionDescription.Replace("$STAT_EVASION", "<color=yellow>" + Mathf.Abs(FloatStatCalculate(_parentStatStruct, _attribute, _statStruct._evasion, _setPower, _setPercent) * 100f).ToString("N2") + "%</color>");
		conditionDescription = conditionDescription.Replace("$STAT_RESISTFIRE", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._fireResist, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_RESISTWATER", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._waterResist, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_RESISTNATURE", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._natureResist, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_RESISTEARTH", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._earthResist, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_RESISTHOLY", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._holyResist, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$STAT_RESISTSHADOW", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _statStruct._shadowResist, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$ABSORB", $"<color=yellow>{Mathf.Abs(StatCalculate(_parentStatStruct, _attribute, _damageAbsorbtionAmount, _setPower, _setPercent))}</color>");
		conditionDescription = conditionDescription.Replace("$ACCURACY", $"<color=yellow>{Mathf.Abs(_accuracyReduction) * 100f}%</color>");
		conditionDescription = conditionDescription.Replace("$MOVSPEED", $"<color=yellow>{Mathf.Abs(_movSpeedPercentChange * 100f)}%</color>");
		conditionDescription = conditionDescription.Replace("$DURATION", "");
		conditionDescription = conditionDescription.Replace("$RATE", $"<color=yellow>every {_setRepeatRate} sec</color>.");
		if ((float)_setDuration > 0f)
		{
			conditionDescription += $" <color=yellow>Lasts for {_setDuration} sec</color>.";
		}
		return conditionDescription;
	}
}
