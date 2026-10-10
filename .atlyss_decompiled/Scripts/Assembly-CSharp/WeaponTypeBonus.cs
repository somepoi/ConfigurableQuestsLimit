using System;
using UnityEngine;

[Serializable]
public class WeaponTypeBonus
{
	[HideInInspector]
	public string _weaponTypeTag = string.Empty;

	public ScriptableWeaponType _scriptableWeaponType;

	public string _weaponTypeBonusDescriptor = string.Empty;

	[Space]
	[Range(0f, 1f)]
	public float _weaponTypeBonus;
}
