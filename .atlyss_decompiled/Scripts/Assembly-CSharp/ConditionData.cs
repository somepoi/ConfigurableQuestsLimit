using System;
using UnityEngine;

[Serializable]
public class ConditionData
{
	public ScriptableCondition _scriptableCondition;

	public GameObject _conditionEffectObject;

	public StatusEntity _senderStatusEntity;

	public ConditionStruct _conditionStruct;

	public StatStruct _statStruct;

	public DamageType _attribute;

	public bool _isInitialized;

	[Space]
	public int _setConditionPower;

	public int _setDuration;

	public float _setRepeatRate;

	public float _setConditionPercent = 1f;

	[Space]
	public float _currentDuration;

	public float _currentRepeatRate;
}
