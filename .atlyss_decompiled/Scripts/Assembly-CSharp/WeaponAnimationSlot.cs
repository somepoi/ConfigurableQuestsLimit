using System;
using UnityEngine;

[Serializable]
public class WeaponAnimationSlot
{
	public string _weaponHoldClip = "";

	public string _weaponTypeAttackClipHandle = "typeName_atk";

	public string _weaponBlockAnimation = "Block_NoShield";

	[Space]
	public string _weaponTypeChargeHandle = "typeName_charge";

	public string _weaponTypeChargeAttackHandle = "";

	[Space]
	public float _smoothBetweenClips;

	[Space]
	[Tooltip("How many weapon swing animations are in this weapon type?")]
	public int _maxSwingStates = 1;

	public float _movSpeedDecrease = -2.5f;

	public float _attackActionBuffer = 1.25f;

	[Tooltip("If false, jump attacking up against the wall will not knock the player back with a klang sound.")]
	public bool _canWallKnockback = true;

	[Space]
	public float _hitboxTime = 0.15f;

	public float _hitboxPersistantTime = 0.12f;

	public float _percentAdjustEachHit = -0.1f;

	[Space]
	public Vector3 _hitboxScale = Vector3.one;

	public Vector3 _hitboxCenter = Vector3.zero;

	[Space]
	[Header("On Attack Move / Push Force (Applied on Animation Event)")]
	public float _attackMoveForce = 50f;

	public float _attackMoveForceDampen = 200f;

	[Space]
	public WeaponTypeCombatParams[] _weaponTypeCombatParams;
}
