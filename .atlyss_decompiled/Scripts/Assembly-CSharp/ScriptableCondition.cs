using UnityEngine;

public abstract class ScriptableCondition : ScriptableObject
{
	[Header("Scriptable Condition")]
	public string _conditionName = "new condition";

	public Sprite _conditionIcon;

	[Space]
	public ScriptableConditionGroup _conditionGroup;

	public ScriptableCombatElement _conditionElement;

	[Space]
	[TextArea(3, 3)]
	public string _conditionDescription = "new description";

	[Header("Unique ID")]
	public int _conditionID = -1;

	[Header("Condition Flags")]
	[Tooltip("The stack type of the condition.\n\nDONT_STACK: Never stack the condition when applied. Always refresh.\n\nFORCE_STACK: Always stack the condition when applied. Never refresh.\n\nUNIQUE_SENDER_STACK: Stack the condition if the sender is different from a previous sender.")]
	public ConditionStackType _conditionStackType;

	[Tooltip("Cancels the condition if the entity successfully casts a skill of a type.")]
	public CancelConditionOnCastType _cancelOnSkillCast;

	[Tooltip("Cancels the condition if the entity is hit, regardless if they block it or not.")]
	public bool _cancelOnHit;

	[Tooltip("Cancels the condition if the entity attacks.")]
	public bool _cancelOnAttack;

	[Tooltip("If true, elites will be immune to this condition.")]
	public bool _ignoreEliteTargets;

	[Tooltip("If true, players will be immune to this condition.")]
	public bool _ignorePlayerTargets;

	[Header("Condition Objects")]
	public GameObject _conditionObject;

	public GameObject _endConditionObject;

	public int Get_ConditionPower(StatStruct _parentStatStruct, DamageType _attribute, int _conditionPower, float _powerPercent)
	{
		int num = 0;
		switch (_attribute)
		{
		case DamageType.Pure:
			num = (int)((float)_conditionPower * _powerPercent);
			break;
		case DamageType.Strength:
			num += (int)((float)(_parentStatStruct._attackPower + _conditionPower) * _powerPercent);
			break;
		case DamageType.Mind:
			num += (int)((float)(_parentStatStruct._magicPower + _conditionPower) * _powerPercent);
			break;
		case DamageType.Dexterity:
			num += (int)((float)(_parentStatStruct._dexPower + _conditionPower) * _powerPercent);
			break;
		case DamageType.Vitality:
			num += (int)((float)(_parentStatStruct._maxHealth + _conditionPower) * _powerPercent);
			break;
		}
		if (num <= 0)
		{
			num = 1;
		}
		return num;
	}

	public void Apply_ConditionToEntity(StatusEntity _senderEntity, StatusEntity _targetEntity, DamageType _attribute, int _conditionPower, int _conditionDuration, float _conditionPercent, float _conditionRepeatRate)
	{
		if (_targetEntity._currentHealth <= 0 || _conditionDuration <= 0)
		{
			return;
		}
		bool flag = false;
		if ((bool)_targetEntity._isCreep && _targetEntity._isCreep._scriptCreep._isElite && _ignoreEliteTargets)
		{
			_targetEntity.ResistCondition_FloatValue(_senderEntity, _targetEntity);
		}
		else
		{
			if ((bool)_targetEntity._isPlayer && _ignorePlayerTargets)
			{
				return;
			}
			for (int i = 0; i < _targetEntity._conditionDatas.Count; i++)
			{
				ConditionData conditionData = _targetEntity._conditionDatas[i];
				bool flag2 = false;
				if (conditionData._scriptableCondition != this)
				{
					continue;
				}
				switch (conditionData._scriptableCondition._conditionStackType)
				{
				case ConditionStackType.FORCE_STACK:
					flag2 = true;
					break;
				case ConditionStackType.UNIQUE_SENDER_STACK:
					flag2 = (bool)_senderEntity && _senderEntity != conditionData._senderStatusEntity;
					break;
				}
				if (flag2)
				{
					break;
				}
				StatStruct statStruct = default;
				if ((bool)_senderEntity)
				{
					if ((bool)_senderEntity._isPlayer)
					{
						statStruct = _senderEntity._isPlayer._pStats._baseStatStruct;
					}
					else if ((bool)_senderEntity._isCreep)
					{
						statStruct = _senderEntity._isCreep._statStruct;
					}
				}
				int num = Get_ConditionPower(conditionData._statStruct, _attribute, conditionData._setConditionPower, conditionData._setConditionPercent);
				int num2 = Get_ConditionPower(statStruct, _attribute, _conditionPower, _conditionPercent);
				if (num < num2)
				{
					End_ConditionEffect(_targetEntity, _targetEntity._conditionDatas[i]);
					conditionData._senderStatusEntity = _senderEntity;
					conditionData._statStruct = statStruct;
					conditionData._setConditionPower = _conditionPower;
					_targetEntity.Init_ResetConditionEffect(this, _targetEntity._conditionDatas[i]);
				}
				_targetEntity.Refresh_Condition(i);
				flag = true;
				break;
			}
			if (_targetEntity.isServer && !flag)
			{
				ConditionStruct item = new ConditionStruct
				{
					_senderEntity = _senderEntity,
					_attribute = _attribute,
					_conditionID = _conditionID,
					_conditionPower = _conditionPower,
					_conditionDuration = _conditionDuration,
					_conditionPowerPercent = _conditionPercent,
					_conditionRepeatRate = _conditionRepeatRate
				};
				_targetEntity._syncConditions.Add(item);
			}
		}
	}

	public abstract void Init_ConditionEffect(StatusEntity _targetEntity, ConditionData _conDat);

	public abstract void Update_ConditionEffect(StatusEntity _targetEntity, ConditionData _conDat);

	public abstract void End_ConditionEffect(StatusEntity _targetEntity, ConditionData _conDat);

	public virtual string Generate_ConditionDescriptor(StatStruct _parentStatStruct, DamageType _attribute, int _conditionPower, int _conditionDuration, float _setPercent, float _setRepeatRate)
	{
		return "";
	}
}
