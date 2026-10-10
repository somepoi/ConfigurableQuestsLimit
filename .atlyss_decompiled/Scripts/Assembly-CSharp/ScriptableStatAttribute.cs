using UnityEngine;

[CreateAssetMenu(fileName = "attribute_", menuName = "ScriptableObjects/Game Logic/Stat Attribute", order = 3)]
public class ScriptableStatAttribute : ScriptableObject
{
	public string _attributeName;

	public Sprite _attributeIcon;

	public Color _attributeStringColor = Color.white;

	[TextArea]
	public string _attributeDescriptor;

	[Header("Combat Stat Modifiers")]
	public AttributeModifierStruct[] _statStructs;

	public void Apply_AttributeCalculation(PlayerStats _pStats, int _atbValue, StatStruct _statStruct)
	{
		if (_pStats.isServer)
		{
			StatStruct runtimeStatStruct = Calculate_StatStruct(_pStats, _atbValue, _statStruct);
			_pStats._runtimeStatStruct = runtimeStatStruct;
		}
	}

	public void Project_AttributeCalculation(PlayerStats _pStats, int _atbValue, StatStruct _statStruct)
	{
		if (_pStats.isLocalPlayer && _pStats.isClient)
		{
			StatStruct projectionStatStruct = Calculate_StatStruct(_pStats, _atbValue, _statStruct);
			TabMenu._current._statsCell._projectionStatStruct = projectionStatStruct;
		}
	}

	public StatStruct Calculate_StatStruct(PlayerStats _pStats, int _atbValue, StatStruct _statStruct)
	{
		int _currentLevel = _pStats._currentLevel;
		for (int i = 0; i < _statStructs.Length; i++)
		{
			Init_StatCalculations(i);
		}
		return _statStruct;
		void Init_StatCalculations(int num)
		{
			float _calculatedStat = _statStructs[num]._statIncreasePercentage * (float)_atbValue * ((float)_currentLevel + 1f / _statStructs[num]._statDampen);
			float _multiplier = (float)_currentLevel * _statStructs[num]._statBaseMultiplier;
			Calculate_MaxHealthStat();
			Calculate_MaxManaStat();
			Calculate_MaxStaminaStat();
			Calculate_AttackStat();
			Calculate_DexStat();
			Calculate_MagicPowerStat();
			Calculate_CriticalRateStat();
			Calculate_MagicCriticalRateStat();
			Calculate_DefenseStat();
			Calculate_MagicDefenseStat();
			Calculate_EvasionStat();
			void Calculate_AttackStat()
			{
				if (_statStructs[num]._statStruct._attackPower > 0)
				{
					int attackPower = _atbValue + (int)((float)(_statStructs[num]._statStruct._attackPower * _currentLevel) * _statStructs[num]._statDampen);
					_statStruct._attackPower = attackPower;
				}
			}
			void Calculate_CriticalRateStat()
			{
				if (!(_statStructs[num]._statStruct._criticalRate <= 0f))
				{
					float num2 = 0f;
					num2 += _statStruct._criticalRate + _statStructs[num]._statStruct._criticalRate * (float)_atbValue;
					if (num2 > 1f)
					{
						num2 = 1f;
					}
					_statStruct._criticalRate = num2;
				}
			}
			void Calculate_DefenseStat()
			{
				if (_statStructs[num]._statStruct._defense > 0)
				{
					int num2 = 1;
					num2 += _statStructs[num]._statStruct._defense * (int)(_calculatedStat / _statStructs[num]._statDampen) + (int)(_multiplier * 0.35f);
					_statStruct._defense = num2;
				}
			}
			void Calculate_DexStat()
			{
				if (_statStructs[num]._statStruct._dexPower > 0)
				{
					int dexPower = _atbValue + (int)((float)(_statStructs[num]._statStruct._dexPower * _currentLevel) * _statStructs[num]._statDampen);
					_statStruct._dexPower = dexPower;
				}
			}
			void Calculate_EvasionStat()
			{
				if (!(_statStructs[num]._statStruct._evasion <= 0f))
				{
					float num2 = 0f;
					num2 += _statStruct._evasion + _statStructs[num]._statStruct._evasion * (float)_atbValue;
					if (num2 > 1f)
					{
						num2 = 1f;
					}
					_statStruct._evasion = num2;
				}
			}
			void Calculate_MagicCriticalRateStat()
			{
				if (!(_statStructs[num]._statStruct._magicCriticalRate <= 0f))
				{
					float num2 = 0f;
					num2 += _statStruct._magicCriticalRate + _statStructs[num]._statStruct._magicCriticalRate * (float)_atbValue;
					if (num2 > 1f)
					{
						num2 = 1f;
					}
					_statStruct._magicCriticalRate = num2;
				}
			}
			void Calculate_MagicDefenseStat()
			{
				if (_statStructs[num]._statStruct._magicDefense > 0)
				{
					int num2 = 1;
					num2 += _statStructs[num]._statStruct._magicDefense * (int)(_calculatedStat / _statStructs[num]._statDampen) + (int)(_multiplier * 0.25f);
					_statStruct._magicDefense = num2;
				}
			}
			void Calculate_MagicPowerStat()
			{
				if (_statStructs[num]._statStruct._magicPower > 0)
				{
					int magicPower = _atbValue + (int)((float)(_statStructs[num]._statStruct._magicPower * _currentLevel) * _statStructs[num]._statDampen);
					_statStruct._magicPower = magicPower;
				}
			}
			void Calculate_MaxHealthStat()
			{
				if (_statStructs[num]._statStruct._maxHealth > 0)
				{
					int num2 = (int)GameManager._current._statLogics._maxHealthCurve.Evaluate(_currentLevel - 1);
					num2 += _statStruct._maxHealth + _statStructs[num]._statStruct._maxHealth * _atbValue;
					_statStruct._maxHealth = num2;
				}
			}
			void Calculate_MaxManaStat()
			{
				if (_statStructs[num]._statStruct._maxMana > 0)
				{
					int num2 = (int)GameManager._current._statLogics._maxManaCurve.Evaluate(_currentLevel - 1);
					num2 += _statStruct._maxMana + _statStructs[num]._statStruct._maxMana * (int)((float)_atbValue * 0.45f);
					_statStruct._maxMana = num2;
				}
			}
			void Calculate_MaxStaminaStat()
			{
				if (_statStructs[num]._statStruct._maxStamina > 0)
				{
					int num2 = (int)GameManager._current._statLogics._maxStaminaCurve.Evaluate(_currentLevel - 1);
					num2 += (int)((float)(_statStructs[num]._statStruct._maxStamina * (int)(_calculatedStat / _statStructs[num]._statDampen)) + _multiplier * 0.35f);
					_statStruct._maxStamina = num2;
				}
			}
		}
	}
}
