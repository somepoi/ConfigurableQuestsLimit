using System;
using UnityEngine;

[Serializable]
public class WeaponTypeCombatParams
{
	[HideInInspector]
	public string _paramTag = string.Empty;

	[Header("Jump Attacking")]
	public string _weaponTypeJumpAttackClipHandle = "typeName_jumpAtk";

	public string _weaponTypeJumpLandClipHandle = "typeName_jumpLand";

	public bool _allowJumpAttack = true;

	[Space]
	public float _jumpAttackActionBuffer = 1.25f;

	public float _timeBeforeJumpAttack;

	public float _JumpAttackJumpForce = 5f;

	public float _JumpAttackFowardForce = 25f;

	public float _JumpAttackJumpGravityMultiply = 50f;

	[Space]
	public float _jumpDamagePercent = 1f;

	public float _persistantDamageRate = 0.12f;

	[Space]
	public Vector3 _jumpAttackColliderCenter = Vector3.zero;

	public Vector3 _jumpAttackColliderScale = Vector3.one;

	[Header("Jump Attack Land")]
	[Range(0f, 5f)]
	public float _jumpLandDamagePercent = 1f;

	[Range(0f, 85.65f)]
	public int _jumpLandForce;

	public float _jumpLandDelayTime = 0.4f;

	[Space]
	public float _jumpAtkLandPersistantDamageRate = 0.12f;

	[Space]
	[Range(0f, 0.8f)]
	public float _jumpAtkLandHitboxTime = 0.25f;

	public Vector3 _jumpAttackLandColliderCenter = Vector3.zero;

	public Vector3 _jumpAttackLandColliderScale = Vector3.one;

	[Header("Charge Attacking")]
	public bool _allowChargeAttack;

	[Space]
	public float _chargeAtkDamagePercent = 1f;

	public float _chargeAtkPersistantDamageRate = 0.12f;

	public float _chargeMovSpeedDecrease = -2.5f;

	[Space]
	public float _chargeInputBuffer = 0.08f;

	public float _chargeTime = 1f;

	[Space]
	public float _chargeAttackCooldown = 1.2f;

	public float _chargeAttackActionBuffer = 0.8f;

	[Space]
	public float _timeBeforeChargeAttackStrafeRotationLock;

	public float _chargeAttackStrafeRotationLock;

	[Space]
	[Header("Charge Attack Forces")]
	public int _chargeAttackForce;

	public float _chargeAttackMoveForce = 150f;

	public float _chargeAttackMoveForceDampen = 3f;

	public bool _applyChargeAttackForceOnNoInput;

	[Space]
	[Range(0f, 0.8f)]
	public float _chargeAttackHitboxTime = 0.85f;

	public Vector3 _chargeAttackColliderCenter = Vector3.zero;

	public Vector3 _chargeAttackColliderScale = Vector3.one;
}
