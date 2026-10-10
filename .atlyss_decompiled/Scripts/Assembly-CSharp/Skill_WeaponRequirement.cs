using System;
using UnityEngine;

[Serializable]
public class Skill_WeaponRequirement
{
	[HideInInspector]
	public string _weaponTypeTag = "";

	public ScriptableWeaponType _wepType;

	[Space]
	public string _animOverride_charge = string.Empty;

	public string _animOverride_cast = string.Empty;
}
