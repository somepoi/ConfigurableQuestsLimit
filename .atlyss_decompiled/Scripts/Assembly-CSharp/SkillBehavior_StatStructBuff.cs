using UnityEngine;

public class SkillBehavior_StatStructBuff : SkillObjectBehavior
{
	[Header("Stat Struct Buff")]
	public StatStruct _statStructApply;

	private bool _applied;

	[Header("Movement")]
	public float _dashSpeedBonus;

	public float _revUpModifierBonus;

	public override void End_SkillBehavior()
	{
		if ((bool)_skillObject._parentPlayer && _skillObject._parentPlayer.isServer)
		{
			Apply_Struct(_isRemoval: true);
		}
	}

	public override void Init_SkillBehavior()
	{
		if ((bool)_skillObject._parentPlayer && _skillObject._parentPlayer.isServer)
		{
			Apply_Struct(_isRemoval: false);
		}
	}

	private void Apply_Struct(bool _isRemoval)
	{
		if (!_applied || _isRemoval)
		{
			int num = 1;
			if (_isRemoval)
			{
				num *= -1;
			}
			PlayerStats pStats = _skillObject._parentPlayer._pStats;
			PlayerMove pMove = _skillObject._parentPlayer._pMove;
			pStats.Network_bonusStatStruct = new StatStruct
			{
				_maxHealth = pStats._bonusStatStruct._maxHealth + _statStructApply._maxHealth * num,
				_maxMana = pStats._bonusStatStruct._maxMana + _statStructApply._maxMana * num,
				_maxStamina = pStats._bonusStatStruct._maxStamina + _statStructApply._maxStamina * num,
				_attackPower = pStats._bonusStatStruct._attackPower + _statStructApply._attackPower * num,
				_dexPower = pStats._bonusStatStruct._dexPower + _statStructApply._dexPower * num,
				_magicPower = pStats._bonusStatStruct._magicPower + _statStructApply._magicPower * num,
				_defense = pStats._bonusStatStruct._defense + _statStructApply._defense * num,
				_magicDefense = pStats._bonusStatStruct._magicDefense + _statStructApply._magicDefense * num,
				_criticalRate = pStats._bonusStatStruct._criticalRate + _statStructApply._criticalRate * (float)num,
				_magicCriticalRate = pStats._bonusStatStruct._magicCriticalRate + _statStructApply._magicCriticalRate * (float)num,
				_evasion = pStats._bonusStatStruct._evasion + _statStructApply._evasion * (float)num,
				_fireResist = pStats._bonusStatStruct._fireResist + _statStructApply._fireResist * num,
				_waterResist = pStats._bonusStatStruct._waterResist + _statStructApply._waterResist * num,
				_natureResist = pStats._bonusStatStruct._natureResist + _statStructApply._natureResist * num,
				_earthResist = pStats._bonusStatStruct._earthResist + _statStructApply._earthResist * num,
				_holyResist = pStats._bonusStatStruct._holyResist + _statStructApply._holyResist * num,
				_shadowResist = pStats._bonusStatStruct._shadowResist + _statStructApply._shadowResist * num
			};
			pStats.Apply_StatCalculations();
			pMove._revUpSpeedModifier += _revUpModifierBonus * (float)num;
			pMove.dashSpeed += _dashSpeedBonus * (float)num;
			_applied = true;
		}
	}

	public override void Update_SkillBehavior()
	{
	}

	private void OnDestroy()
	{
		End_SkillBehavior();
	}
}
