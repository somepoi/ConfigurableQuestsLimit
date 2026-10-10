using System;
using UnityEngine;

[Serializable]
public class Creep_SkillCastSlot
{
	[HideInInspector]
	public string _castSlotTag = string.Empty;

	[HideInInspector]
	public float _currentCooldownTime;

	public ScriptableSkill _scriptableSkill;

	[Range(0f, 1f)]
	public float _chanceToCast = 0.5f;

	[Range(0f, 1f)]
	public float _healthPercentToCast = 1f;

	public float _baseCooldownTime = 1.25f;

	public float _castTimeModifier;
}
