using UnityEngine;

public struct DamageStruct
{
	public StatusEntity _statusEntity;

	public ScriptableCombatElement _damageElement;

	public ScriptableSkill _scriptableSkill;

	public CombatCollider _combatCollider;

	public ConditionSlot _conditionSlot;

	public int _bonusConditionPower;

	public bool _forceConditionApply;

	public int _levelDifference;

	public int _damageValue;

	public float _appliedForce;

	public float _threatAmount;

	public float _siphonHealthPercent;

	public DamageType _damageType;

	public DamageWeight _damageWeight;

	public EntityDamageType _entityDamageType;

	public CombatColliderType _colliderType;

	public Vector3 _damageDirection;

	public bool _setDamageDirection;

	public Vector3 _colliderHitPoint;

	public bool _isCriticalHit;

	public bool _ignoreIFrames;

	public bool _noHitStun;

	public bool _forceOnly;

	public bool _isProjectileCollider;

	public bool _canReflect;

	public bool _ignoreDefenseBlock;

	public bool _allowParryStagger;

	public bool _forceThreat;

	public bool _shatterBlock;
}
