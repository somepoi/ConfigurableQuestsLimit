using System;
using UnityEngine;

[Serializable]
public class SkillBonus
{
	[HideInInspector]
	public string _scriptSkillTag = string.Empty;

	public ScriptableSkill _scriptableSkill;

	public string _skillBonusDescriptor = string.Empty;

	[Space]
	[Range(0f, 1f)]
	public float _skillPowerBonus;

	[Range(0f, 1f)]
	public float _skillConditionPowerBonus;

	public int _skillConditionDurationBonus;
}
