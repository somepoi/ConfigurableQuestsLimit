using System;
using UnityEngine;

[Serializable]
public struct StatStruct
{
	public int _maxHealth;

	public int _maxMana;

	public int _maxStamina;

	public int _experience;

	[Space]
	public int _attackPower;

	public int _magicPower;

	public int _dexPower;

	[Space]
	public float _criticalRate;

	public float _magicCriticalRate;

	public float _evasion;

	[Space]
	public int _defense;

	public int _magicDefense;

	[Space]
	public int _fireResist;

	public int _waterResist;

	public int _natureResist;

	public int _earthResist;

	public int _holyResist;

	public int _shadowResist;
}
