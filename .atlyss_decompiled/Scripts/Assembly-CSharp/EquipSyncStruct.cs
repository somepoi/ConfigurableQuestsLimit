using System;
using UnityEngine;

[Serializable]
public struct EquipSyncStruct
{
	public string _helmTag;

	public int _helmModifier;

	[Space]
	public string _chestTag;

	public int _chestModifier;

	[Space]
	public string _leggingTag;

	public int _leggingModifier;

	[Space]
	public string _capeTag;

	public int _capeModifier;

	[Space]
	public string _ringTag;

	public int _ringModifier;

	[Space]
	public string _weaponTag;

	public int _weaponModifier;

	public DamageType _damageOverride_mainWep;

	public bool _useDamageOverride_mainWep;

	[Space]
	public string _altWeaponTag;

	public int _altWeaponModifier;

	public DamageType _damageOverride_altWep;

	public bool _useDamageOverride_altWep;

	[Space]
	public string _shieldTag;

	public int _shieldModifier;
}
