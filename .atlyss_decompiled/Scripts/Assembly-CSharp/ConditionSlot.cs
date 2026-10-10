using System;
using UnityEngine;

[Serializable]
public class ConditionSlot
{
	public ScriptableCondition _scriptableCondition;

	[Range(0f, 1f)]
	public float _chance = 1f;

	public int _conditionDuration;

	[Tooltip("Repeat rate below or equal to 0 will not repeat this condition.")]
	public float _conditionRepeatRate;

	[Space]
	[Range(0f, 1f)]
	public float _powerPercent = 1f;

	public int _conditionPower;
}
