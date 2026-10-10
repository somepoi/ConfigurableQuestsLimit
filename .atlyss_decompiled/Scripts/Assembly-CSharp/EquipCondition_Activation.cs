using System;
using UnityEngine;

[Serializable]
public class EquipCondition_Activation
{
	[Header("Equip Condition General")]
	public ConditionSlot _equipConditionSlot;

	[Space(15f)]
	[TextArea(2, 2)]
	public string _equipConditionDescription = string.Empty;

	public float _conditionCooldown = 2f;

	public bool _canRefreshCondition;

	[Header("Status Check")]
	[Range(0f, 1f)]
	public float _healthPercentageThreshold = 1f;

	[Range(0f, 1f)]
	public float _manaPercentageThreshold = 1f;

	[Range(0f, 1f)]
	public float _staminaPercentageThreshold = 1f;

	[Header("Callbacks")]
	[Tooltip("Whatever is called from Status Entity, Player Casting, or Player Movement classes will trigger the equipment condition.")]
	public EquipConditionCallback _equipConditionCallback;

	[Space]
	[Tooltip("Setting this will make the equipment apply the condition if the specific skill is casted. This will fully ingore the enum callback and set it to none.")]
	public ScriptableSkill _dedicatedSkillCastCallback;

	public bool _requireSkillHit;
}
