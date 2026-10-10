using System;
using UnityEngine;

[Serializable]
public class SkillParameters
{
	[Header("Base Skill Params")]
	public int _skillPointCost = 1;

	public int _levelRequirement = 1;

	[Space(15f)]
	public ScriptableCombatElement _skillElement;

	public int _baseSkillPower;

	public bool _useWeaponPower;

	[Header("Skill Cost")]
	public int _manaCost = 1;

	public int _healthCost;

	public int _staminaCost;

	[Space]
	public ScriptableItem _baseRequiredItem;

	public int _basedItemCost;

	[Header("Cast & Cooldown Time")]
	[Range(0.12f, 20f)]
	public float _baseCastTime = 0.85f;

	[Range(0.6f, 250f)]
	public float _baseCooldown = 1.25f;

	[Header("Self Condition Skill")]
	public ConditionSlot _selfConditionOutput;

	[Header("Skill Objects")]
	public GameObject _skillObject_onChannel;

	public GameObject _skillObject_onCast;

	[Space]
	public ConditionSlot _skillObjectCondition;

	[Header("Skill Flags")]
	public bool _isSummonSkill;

	public bool _castOnNoConditionOnly;

	public bool _isUninterruptable;

	public bool _checkFullHealth;

	public bool _checkFullMana;

	public bool _checkFullStamina;
}
